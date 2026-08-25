using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Win32;

namespace NMSLegacyVersionInstaller
{
    public static class Steam
    {
        public static string Username { get; set; }
        public static string Password { get; set; }

        // ---- Version-based dummy Steam IDs (moved here from SteamID.cs) ----
        // Goldberg's force_steamid.txt needs a VALID SteamID64 (public individual desktop account) or it
        // ignores the file and falls back to the real Steam ID. Such an id is 76561197960265728 (the
        // required universe/type/instance header) + a 32-bit account id, so the number can't be small.
        // We pick the account id so the full id ENDS in the version number - save folders read
        // st_...109 / 113 / 124 / 138. Each version gets its own id, so no collision and no SmartSaveFolder.
        public static string DummySteamID(Enums.Update update)
        {
            switch (update)
            {
                case Enums.Update.Release: return "76561197960266109";  // v1.09
                case Enums.Update.Foundation: return "76561197960266113";  // v1.13
                case Enums.Update.PathFinder: return "76561197960266124";  // v1.24
                case Enums.Update.AtlasRises: return "76561197960266138";  // v1.38
                default: return "76561197960266019";
            }
        }

        // ---- Real Steam accounts detected on this machine ----
        public class SteamUser
        {
            public string Name;
            public string Id64;   // 17-digit SteamID64, or "" for the emulator default

            public override string ToString()
            {
                return string.IsNullOrEmpty(Id64) ? Name : Name + " <" + Id64 + ">";
            }
        }

        // Scans the local Steam install (located via the registry) for accounts that have logged in.
        // Returns a single "Default User" entry when Steam or its account list can't be found.
        public static List<SteamUser> GetSteamUsers()
        {
            var users = new List<SteamUser>();
            string steamPath = GetSteamPath();
            if (!string.IsNullOrEmpty(steamPath))
            {
                string vdf = Path.Combine(steamPath, "config", "loginusers.vdf");
                if (File.Exists(vdf))
                    users = ParseLoginUsers(File.ReadAllText(vdf));
            }
            if (users.Count == 0)
                users.Add(new SteamUser { Name = "Default User", Id64 = "" });
            return users;
        }

        // Steam's install path from the registry (per-user first, then machine-wide 32/64-bit).
        private static string GetSteamPath()
        {
            using (var key = Registry.CurrentUser.OpenSubKey(@"Software\Valve\Steam"))
            {
                var p = key?.GetValue("SteamPath") as string;
                if (!string.IsNullOrEmpty(p))
                    return p.Replace('/', '\\');
            }
            foreach (var sub in new[] { @"SOFTWARE\WOW6432Node\Valve\Steam", @"SOFTWARE\Valve\Steam" })
            {
                using (var key = Registry.LocalMachine.OpenSubKey(sub))
                {
                    var p = key?.GetValue("InstallPath") as string;
                    if (!string.IsNullOrEmpty(p))
                        return p;
                }
            }
            return null;
        }

        // loginusers.vdf is a small Valve KeyValues file:
        //   "users" { "<steamID64>" { "AccountName" "x"  "PersonaName" "y"  ... } ... }
        private static readonly Regex IdLine = new Regex("^\"(\\d{17})\"$");
        private static readonly Regex KvLine = new Regex("^\"([^\"]+)\"\\s+\"([^\"]*)\"$");

        private static List<SteamUser> ParseLoginUsers(string vdf)
        {
            var list = new List<SteamUser>();
            string id = null, account = null, persona = null;
            foreach (var raw in vdf.Split('\n'))
            {
                string line = raw.Trim();

                var idm = IdLine.Match(line);
                if (idm.Success)
                {
                    Flush(list, id, account, persona);
                    id = idm.Groups[1].Value;
                    account = persona = null;
                    continue;
                }

                if (id == null) continue;
                var kv = KvLine.Match(line);
                if (!kv.Success) continue;
                switch (kv.Groups[1].Value.ToLowerInvariant())
                {
                    case "accountname": account = kv.Groups[2].Value; break;
                    case "personaname": persona = kv.Groups[2].Value; break;
                }
            }
            Flush(list, id, account, persona);
            return list;
        }

        private static void Flush(List<SteamUser> list, string id, string account, string persona)
        {
            if (id == null) return;
            string name = !string.IsNullOrEmpty(persona) ? persona
                        : !string.IsNullOrEmpty(account) ? account
                        : "Steam User";
            list.Add(new SteamUser { Name = name, Id64 = id });
        }
    }
}
