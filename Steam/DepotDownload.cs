// Derived from DepotDownloader (https://github.com/SteamRE/DepotDownloader), GPL-2.0-or-later.
// Trimmed hard: the installer only ever writes a fresh copy of one depot into an empty folder, so
// there is none of the original's manifest caching, staging, chunk reuse or delete-on-update work.
// See LICENSE (GPL-3.0) for the terms this installer is released under.

using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using SteamKit2;
using SteamKit2.CDN;

namespace NMSRetroInstaller.Steam;

/// <summary>Downloads one depot manifest into a folder, straight from Steam's content servers.</summary>
public static class DepotDownload
{
    const int MaxParallelChunks = 8;

    /// <summary>Tries per manifest or chunk, rotating content servers as they fail or stall.</summary>
    const int Attempts = 12;

    /// <param name="progress">Called with (bytes written, bytes total) as the download proceeds.</param>
    public static async Task RunAsync(
        SteamSession session, uint appId, uint depotId, ulong manifestId, string installDir,
        Action<long, long> progress, Action<string> log, CancellationToken ct)
    {
        var depotKey = await session.GetDepotKeyAsync(depotId, appId);
        var (servers, proxy) = await session.GetContentServersAsync(appId);
        var requestCode = await session.GetManifestRequestCodeAsync(depotId, appId, manifestId);
        var pool = new ServerPool(servers);

        log($"Fetching manifest {manifestId}...");
        var manifest = await FetchManifestAsync(session, pool, proxy, appId, depotId, manifestId, requestCode, depotKey, ct);

        var files = manifest.Files!
            .Where(f => !f.Flags.HasFlag(EDepotFileFlag.Directory) && string.IsNullOrEmpty(f.LinkTarget))
            .ToList();

        long total = files.Sum(f => (long)f.TotalSize);
        log($"{files.Count} files, {Bytes(total)} to download.");

        Allocate(manifest, installDir, files);

        // Manifest order, so the workers stay within a file or two at a time and files finish
        // steadily. Sorting the chunks globally by size spreads every file across the whole run,
        // which starves the per-file log below of anything to report until near the end.
        var pending = files.Select(f => new PendingFile(f)).ToList();
        var work = pending
            .SelectMany(f => f.File.Chunks.Select(c => (File: f, Chunk: c)))
            .ToList();

        long written = 0;
        progress(0, total);

        await Parallel.ForEachAsync(
            work,
            new ParallelOptions { MaxDegreeOfParallelism = MaxParallelChunks, CancellationToken = ct },
            // ct, not the loop's own token: when one chunk gives up for real, that token is
            // cancelled too, and the sibling cancellations would bury the reason it failed.
            async (item, _) =>
            {
                var got = await DownloadChunkAsync(
                    session, pool, proxy, appId, depotId, depotKey,
                    Path.Combine(installDir, item.File.File.FileName), item.Chunk, ct);

                var done = Interlocked.Add(ref written, got);
                progress(done, total);

                // One line per finished file, the way DepotDownloader reports itself.
                if (Interlocked.Decrement(ref item.File.ChunksLeft) == 0)
                    log($"{100.0 * done / Math.Max(1, total),6:#00.00}% {item.File.File.FileName}");
            });

        log($"Downloaded {Bytes(written)}.");
    }

    /// <summary>Creates the folder tree and lays every file out at its final size up front.</summary>
    static void Allocate(DepotManifest manifest, string installDir, List<DepotManifest.FileData> files)
    {
        Directory.CreateDirectory(installDir);

        foreach (var dir in manifest.Files!.Where(f => f.Flags.HasFlag(EDepotFileFlag.Directory)))
            Directory.CreateDirectory(Path.Combine(installDir, dir.FileName));

        foreach (var file in files)
        {
            var path = Path.Combine(installDir, file.FileName);

            // Manifests do not always list every directory they use.
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);

            using var stream = File.Open(path, FileMode.Create, FileAccess.Write, FileShare.None);
            stream.SetLength((long)file.TotalSize);
        }
    }

    static async Task<DepotManifest> FetchManifestAsync(
        SteamSession session, ServerPool pool, Server? proxy, uint appId, uint depotId,
        ulong manifestId, ulong requestCode, byte[] depotKey, CancellationToken ct)
    {
        Exception? last = null;

        for (var attempt = 0; attempt < Attempts; attempt++)
        {
            ct.ThrowIfCancellationRequested();
            var server = pool.Current;

            try
            {
                return await session.CdnClient.DownloadManifestAsync(
                    depotId, manifestId, requestCode, server, depotKey, proxy,
                    await session.GetCdnAuthTokenAsync(appId, depotId, server.Host!));
            }
            catch (SteamKitWebRequestException e) when (e.StatusCode == HttpStatusCode.Forbidden)
            {
                // Some servers only serve with a token; ask for one and stay on this server.
                last = e;
                await session.GetCdnAuthTokenAsync(appId, depotId, server.Host!);
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception e)
            {
                last = e;
                pool.Rotate(server);
            }
        }

        throw new SteamException(
            $"Could not download manifest {manifestId} from any Steam content server. {last?.Message}");
    }

    /// <summary>Downloads one chunk and writes it at its offset. Returns the bytes written.</summary>
    static async Task<int> DownloadChunkAsync(
        SteamSession session, ServerPool pool, Server? proxy, uint appId, uint depotId,
        byte[] depotKey, string path, DepotManifest.ChunkData chunk, CancellationToken ct)
    {
        var buffer = ArrayPool<byte>.Shared.Rent((int)chunk.UncompressedLength);
        try
        {
            var written = 0;
            Exception? last = null;

            for (var attempt = 0; attempt < Attempts && written == 0; attempt++)
            {
                ct.ThrowIfCancellationRequested();
                var server = pool.Current;

                try
                {
                    written = await session.CdnClient.DownloadDepotChunkAsync(
                        depotId, chunk, server, buffer, depotKey, proxy,
                        await session.GetCdnAuthTokenAsync(appId, depotId, server.Host!));
                }
                catch (SteamKitWebRequestException e) when (e.StatusCode == HttpStatusCode.Forbidden)
                {
                    last = e;
                    await session.GetCdnAuthTokenAsync(appId, depotId, server.Host!);
                }
                catch (OperationCanceledException) when (ct.IsCancellationRequested)
                {
                    throw;
                }
                catch (Exception e)
                {
                    // Includes the TaskCanceledException an HTTP timeout raises: a content server
                    // that stalls is a reason to move to the next one, not to abandon the install.
                    last = e;
                    pool.Rotate(server);
                }
            }

            if (written == 0)
                throw new SteamException(
                    $"Gave up downloading a chunk of {Path.GetFileName(path)} after {Attempts} tries. {last?.Message}");

            // Chunks of the same file land on different threads; separate handles at separate
            // offsets is simpler than sharing one stream and costs nothing measurable.
            using var stream = File.Open(path, FileMode.Open, FileAccess.Write, FileShare.ReadWrite);
            stream.Seek((long)chunk.Offset, SeekOrigin.Begin);
            await stream.WriteAsync(buffer.AsMemory(0, written), ct);

            return written;
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }

    static string Bytes(long value) => value switch
    {
        >= 1L << 30 => $"{value / (double)(1L << 30):0.0} GB",
        >= 1L << 20 => $"{value / (double)(1L << 20):0.0} MB",
        _ => $"{value / 1024.0:0.0} KB",
    };

    /// <summary>A file being written, and how many of its chunks are still outstanding.</summary>
    sealed class PendingFile(DepotManifest.FileData file)
    {
        public readonly DepotManifest.FileData File = file;
        public int ChunksLeft = file.Chunks.Count;
    }

    /// <summary>
    /// Hands out the same content server until it misbehaves, then moves everyone to the next one.
    /// </summary>
    sealed class ServerPool(List<Server> servers)
    {
        int index;

        public Server Current => servers[index % servers.Count];

        public void Rotate(Server failed)
        {
            lock (servers)
            {
                if (ReferenceEquals(servers[index % servers.Count], failed))
                    index++;
            }
        }
    }
}
