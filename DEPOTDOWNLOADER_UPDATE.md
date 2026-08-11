# DepotDownloader update: 2.4.7 → 3.4.0

## What changed
- `InstallerFiles/DepotDownloader.exe` / `.dll` / `.deps.json` / `.runtimeconfig.json` replaced with the
  official `DepotDownloader-framework.zip` build from
  https://github.com/SteamRE/DepotDownloader/releases/tag/DepotDownloader_3.4.0
- Shared dependency DLLs updated to match: `SteamKit2.dll`, `protobuf-net.dll`, `protobuf-net.Core.dll`
- Three new dependency DLLs added (required by 3.x, weren't needed by 2.4.7):
  `QRCoder.dll`, `System.IO.Hashing.dll`, `ZstdSharp.dll`
- `NMSLegacyVersionInstaller.csproj` updated to embed the three new DLLs as resources
  (same `InstallerFiles\` pattern as the existing ones)

## Why it's safe
- `Program.ExtractInstallerFiles` extracts *every* embedded resource under the `InstallerFiles.` prefix
  at runtime — no other code change was needed for the new DLLs to end up next to `DepotDownloader.exe`.
- `Steps/DepotDownloader.cs` only ever passes `-app -depot -manifest -dir -username -password
  -remember-password` to the process. All of those flags are unchanged as of 3.4.0 — the only flag
  DepotDownloader has removed since 2.4.7 (`-max-servers`) isn't used here.
- `Steps/ExtractTemporaryFiles.cs` already has generic handling for a "fatal error / missing runtime"
  message from DepotDownloader and prompts the user to install the required .NET runtime — this logic
  needed no changes.

## One real behavior change to know about
DepotDownloader 3.4.0 targets **.NET 9** (`rollForward: LatestMajor`), vs. **.NET 6** for 2.4.7. Users
without .NET 9 (or newer) installed will hit the existing "DepotDownload Compatibility Error" dialog on
first run and be prompted to install it — same flow as before, just a newer runtime requirement. Worth
a mention in the README/release notes so users aren't surprised.

## Not changed
No installer UI/logic files were touched beyond the `.csproj` resource registration above.
