// Derived from DepotDownloader (https://github.com/SteamRE/DepotDownloader), GPL-2.0-or-later.
// Trimmed to what this installer needs: sign in, and hand the depot downloader the keys and
// tokens it asks for. See LICENSE (GPL-3.0) for the terms this installer is released under.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SteamKit2;
using SteamKit2.Authentication;
using SteamKit2.CDN;

namespace NMSRetroInstaller.Steam;

/// <summary>Raised for anything the user can act on - wrong password, no licence, no connection.</summary>
public class SteamException(string message) : Exception(message);

/// <summary>
/// A signed-in Steam connection. Replaces the DepotDownloader.exe the installer used to shell out
/// to: the credentials go straight to Steam from this process and never touch disk.
/// </summary>
public sealed class SteamSession : IDisposable
{
    readonly SteamClient client;
    readonly CallbackManager callbacks;
    readonly SteamUser user;
    readonly SteamApps apps;
    readonly SteamContent content;
    readonly CancellationTokenSource ticker = new();

    readonly Dictionary<uint, byte[]> depotKeys = [];
    readonly Dictionary<(uint Depot, string Host), string> cdnTokens = [];
    readonly SemaphoreSlim tokenLock = new(1);

    SteamSession()
    {
        client = new SteamClient();
        callbacks = new CallbackManager(client);
        user = client.GetHandler<SteamUser>()!;
        apps = client.GetHandler<SteamApps>()!;
        content = client.GetHandler<SteamContent>()!;
    }

    /// <summary>The account name exactly as Steam spells it, which is not necessarily how it was typed.</summary>
    public string AccountName { get; private set; } = "";

    public Client CdnClient { get; private set; } = null!;

    /// <summary>
    /// Connects, authenticates and waits for the licence list. <paramref name="authenticator"/>
    /// is asked for a Steam Guard code if the account has one.
    /// </summary>
    public static async Task<SteamSession> LoginAsync(
        string username, string password, IAuthenticator authenticator,
        Action<string> log, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrEmpty(password))
            throw new SteamException("Enter your Steam username and password.");

        var session = new SteamSession();
        try
        {
            await session.ConnectAndLogOnAsync(username.Trim(), password, authenticator, log, ct);
            return session;
        }
        catch
        {
            session.Dispose();
            throw;
        }
    }

    async Task ConnectAndLogOnAsync(
        string username, string password, IAuthenticator authenticator,
        Action<string> log, CancellationToken ct)
    {
        var connected = new TaskCompletionSource();
        var loggedOn = new TaskCompletionSource();
        var licensed = new TaskCompletionSource();

        // Subscriptions live as long as the session; the ticker below is what actually runs them.
        callbacks.Subscribe<SteamClient.ConnectedCallback>(_ => connected.TrySetResult());
        callbacks.Subscribe<SteamClient.DisconnectedCallback>(_ =>
        {
            var dropped = new SteamException("Lost the connection to Steam.");
            connected.TrySetException(dropped);
            loggedOn.TrySetException(dropped);
            licensed.TrySetException(dropped);
        });
        callbacks.Subscribe<SteamUser.LoggedOnCallback>(on =>
        {
            if (on.Result == EResult.OK)
                loggedOn.TrySetResult();
            else
                loggedOn.TrySetException(new SteamException("Steam refused the sign-in: " + on.Result));
        });
        callbacks.Subscribe<SteamApps.LicenseListCallback>(list =>
        {
            if (list.Result == EResult.OK)
                licensed.TrySetResult();
            else
                licensed.TrySetException(new SteamException("Could not read the account's licences: " + list.Result));
        });

        _ = Task.Run(TickAsync, CancellationToken.None);

        log("Connecting to Steam...");
        client.Connect();
        await Settle(connected.Task, "connection", ct);

        log("Signing in as " + username + "...");
        AuthPollResult result;
        try
        {
            var auth = await client.Authentication.BeginAuthSessionViaCredentialsAsync(new AuthSessionDetails
            {
                DeviceFriendlyName = "No Man's Sky Retro Installer",
                Username = username,
                Password = password,
                IsPersistentSession = false,
                Authenticator = authenticator,
            });
            result = await auth.PollingWaitForResultAsync(ct);
        }
        catch (AuthenticationException ex)
        {
            throw new SteamException("Steam rejected the sign-in: " + ex.Result);
        }

        // Steam hands back the account name in its own casing - that is what the patch config wants.
        AccountName = result.AccountName;

        user.LogOn(new SteamUser.LogOnDetails
        {
            Username = result.AccountName,
            AccessToken = result.RefreshToken,
            ShouldRememberPassword = false,
        });

        await Settle(loggedOn.Task, "sign-in", ct);
        await Settle(licensed.Task, "licence list", ct);

        CdnClient = new Client(client);
        log("Signed in as " + AccountName + ".");
    }

    /// <summary>Waits for one step of the sign-in, turning a stall into something readable.</summary>
    static async Task Settle(Task step, string what, CancellationToken ct)
    {
        try
        {
            await step.WaitAsync(TimeSpan.FromSeconds(60), ct);
        }
        catch (TimeoutException)
        {
            throw new SteamException($"Steam did not answer while waiting for the {what}. Try again in a moment.");
        }
    }

    async Task TickAsync()
    {
        try
        {
            while (!ticker.IsCancellationRequested)
                await callbacks.RunWaitCallbackAsync(ticker.Token);
        }
        catch (OperationCanceledException) { }
    }

    /// <summary>The decryption key for a depot. Failing here means the account does not own the game.</summary>
    public async Task<byte[]> GetDepotKeyAsync(uint depotId, uint appId)
    {
        if (depotKeys.TryGetValue(depotId, out var cached))
            return cached;

        var key = await apps.GetDepotDecryptionKey(depotId, appId);
        if (key.Result != EResult.OK)
            throw new SteamException(
                $"Steam would not release the depot key ({key.Result}). This account has to own No Man's Sky.");

        return depotKeys[depotId] = key.DepotKey;
    }

    public async Task<ulong> GetManifestRequestCodeAsync(uint depotId, uint appId, ulong manifestId)
    {
        var code = await content.GetManifestRequestCode(depotId, appId, manifestId, "public");
        if (code == 0)
            throw new SteamException(
                $"Steam would not issue a request code for manifest {manifestId}. That build may no longer be downloadable.");

        return code;
    }

    /// <summary>Content servers able to serve this app, best first, one entry per configured slot.</summary>
    public async Task<(List<Server> Servers, Server? Proxy)> GetContentServersAsync(uint appId)
    {
        var all = await content.GetServersForSteamPipe();
        var proxy = all.FirstOrDefault(s => s.UseAsProxy);

        var usable = all
            .Where(s => (s.Type == "SteamCache" || s.Type == "CDN")
                        && (s.AllowedAppIds.Length == 0 || s.AllowedAppIds.Contains(appId)))
            .OrderBy(s => s.WeightedLoad)
            .SelectMany(s => Enumerable.Repeat(s, Math.Max(1, s.NumEntries)))
            .ToList();

        if (usable.Count == 0)
            throw new SteamException("Steam returned no content servers to download from.");

        return (usable, proxy);
    }

    /// <summary>
    /// A CDN auth token for one server, fetched once and shared. Only some servers need one, which
    /// is why this is asked for lazily after a 403 rather than up front.
    /// </summary>
    public async Task<string?> GetCdnAuthTokenAsync(uint appId, uint depotId, string host)
    {
        await tokenLock.WaitAsync();
        try
        {
            if (cdnTokens.TryGetValue((depotId, host), out var cached))
                return cached;

            var auth = await content.GetCDNAuthToken(appId, depotId, host);
            return cdnTokens[(depotId, host)] = auth.Result == EResult.OK ? auth.Token : null!;
        }
        finally
        {
            tokenLock.Release();
        }
    }

    public void Dispose()
    {
        ticker.Cancel();
        try { client.Disconnect(); } catch { }
        ticker.Dispose();
        tokenLock.Dispose();
    }
}
