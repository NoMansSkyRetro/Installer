# No Man's Sky Retro Installer
<img width="1144" height="591" alt="03-select-versions" src="https://github.com/user-attachments/assets/e2c5d88c-05c8-4d4a-b741-8c9a741c9185" />

Installs the last patched build of four major No Man's Sky updates, side by side, from your own
Steam account:

* **1.09** the initial release
* **1.13** Foundation
* **1.24** Path Finder
* **1.38** Atlas Rises

Steam keeps every previous build of a game on its servers. This installer signs in, downloads the
versions of No Man's Sky you pick, installs [steam_api64.retro](https://github.com/NoMansSkyRetro/steam_api64.retro) to disconnect them from Steam, applies [EthanRDoesMC](https://github.com/EthanRDoesMC/)'s [RetroShaderFix](https://github.com/EthanRDoesMC/RetroShaderFix) and
sets up a launcher. Made in support of the community at
[No Man's Sky Retro](https://nomansskyretro.com).

## Download

The releases page: https://github.com/NoMansSkyRetro/Installer/releases

## What version 2.0 changed

- **A new interface.** WPF, animated, one screen per step, replacing the old WinForms wizard.
- **Steam is spoken to directly.** SteamKit2 in-process, instead of shipping and driving
  DepotDownloader.exe. There is no console window, no temporary copy of a downloader on disk, and
  your credentials go from the sign-in box to Steam and nowhere else.
- **One patch instead of three tools.** `steam_api64.retro` replaces the Goldberg emulator plus
  Steamless combination. `NMS.exe` is no longer rewritten on disk.
- **The mods warning is off** and **discoveries are routed** to the community server.
- **The shader fix is always applied**, rather than being a question during setup.
- **SmartSaveFolder is gone.** Each build gets its own save folder from its own `steamid`, so
  there is nothing left to switch between.
- **A launcher**, on the desktop, in the Start menu and in the install folder.
- The project is now **GPL-3.0**, because the depot download derives from DepotDownloader.

## How it works

For each version you pick:

1. **Download.** The build's manifest is fetched from Steam and its chunks pulled straight from
   Steam's content servers into `<install folder>\no_mans_sky_v<version>`.
2. **Patch.** `steam_api64.dll` in `Binaries` is replaced with
   [steam_api64.retro](https://github.com/NoMansSkyRetro/steam_api64.retro), which answers the
   Steamworks calls these builds make and unwraps the SteamStub DRM in memory as the game loads.
   The original is kept as `steam_api64.dll.bak`; `NMS.exe` is left alone.
3. **Configure.** A `steam_api64.txt` is written next to it:

   | Key | Value |
   |-----|-------|
   | `steamid` | `109`, `113`, `124` or `138`, so each build saves to its own `st_<id>` folder |
   | `name` | your Steam account name, spelled the way Steam spells it |
   | `disablemodwarning` | `true`, so the mods-enabled screen does not appear at boot |
   | `discoveriesserver` | `discoveries.nomansskyretro.com` |

4. **Shader fix.** [RetroShaderFix](https://github.com/EthanRDoesMC/RetroShaderFix) packs matching
   your graphics vendor, applied automatically. RetroShaderFixGUI is also left in `Extras` if you
   want to rerun it by hand.
5. **Shortcuts.** One per version in the install folder, plus the launcher, the Discord link and
   an install log under `Log`.

## The launcher

The installer copies itself into the install folder as `No Man's Sky Retro Launcher.exe`, and
adds shortcuts to the Desktop and to Start menu > No Man's Sky Retro > Launcher. It shows the
versions it finds beside it and starts whichever you click. It is the same executable as the
installer, run under another name, so the install folder gets one self-contained file rather than
a second copy of the .NET runtime.

## Disclaimer

- This application does not contain any game data, and only serves as a tool to download the game
  from Steam. The user must have a valid, purchased copy of the game and provide their own Steam
  account details to use this application.

- The removal of DRM measures in the application is done solely for the purpose of allowing the
  older version of the game to function and is not intended to encourage or support piracy. The
  creators of the application do not condone or support piracy, and users are reminded that it is
  illegal to distribute or use pirated software.

- The use of this application is at the user's own risk, and the creators of the application
  cannot be held responsible for any damages that may result from its use. This includes, but is
  not limited to, any damage to the user's computer, loss of data, or legal repercussions.

## Third-party work

| Project | Licence | Used for |
|---------|---------|----------|
| [SteamKit2](https://github.com/SteamRE/SteamKit) | LGPL-2.1 | Steam's protocol: sign-in, depot keys, content servers |
| [DepotDownloader](https://github.com/SteamRE/DepotDownloader) | GPL-2.0 | the depot download is derived from it |
| [steam_api64.retro](https://github.com/NoMansSkyRetro/steam_api64.retro) | MIT | running these builds without Steam |
| [RetroShaderFix](https://github.com/EthanRDoesMC/RetroShaderFix) | - | the shader packs |
| [RetroShaderFixGUI](https://github.com/NoMansSkyRetro/RetroShaderFixGUI) | - | rerunning the shader fix by hand |

`steam_api64.retro` contains no code from either [Steamless](https://github.com/atom0s/Steamless)
or the [Goldberg Steam Emulator](https://gitlab.com/Mr_Goldberg/goldberg_emulator), but would not
exist without them: its SteamStub handling follows the former and its Steamworks interface layout
the latter.

## Building

.NET 9 SDK, Windows:

```powershell
.\Build.ps1
```

That puts one self-contained `NMSRetroInstaller.exe` in `bin`, and nothing else - every assembly
it needs is bundled inside it. A plain `dotnet build` writes its loose output under `obj\build`,
so `bin` only ever holds the finished executable. `dotnet publish -c Release` does the same job
if you would rather not go through the script.

`Build.ps1` also runs the self-check, which does everything the install does apart from the Steam
download - writing the payload, patching, the shader fix, the shortcuts - against a throwaway
folder, and fails the build if anything is missing or wrong. CI runs the same script.

| Switch | |
|--------|---|
| `-Configuration Debug` | build Debug instead of Release |
| `-NoCheck` | skip the self-check |
| `-Run` | start the installer once it is built |

## Contributing

If you hit a problem or have a suggestion, please
[open an issue](https://github.com/NoMansSkyRetro/Installer/issues). Pull requests are welcome.

## License

GPL-3.0, see [LICENSE](LICENSE).
