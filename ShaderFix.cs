using System;
using System.IO;
using Microsoft.Win32;

namespace NMSRetroInstaller
{
    // Shader fix logic ported from RetroShaderFix by Qjimbo.
    // The .pak files (the actual fix) are made by Ethan (EthanRDoesMC) - https://github.com/EthanRDoesMC/RetroShaderFix
    public static class ShaderFix
    {
        // Display adapters as the driver class key lists them - the same names WMI reports,
        // without dragging in System.Management for one string.
        public static Enums.GPU DetectGPU(Action<string>? log = null)
        {
            const string DisplayClass = @"SYSTEM\CurrentControlSet\Control\Class\{4d36e968-e325-11ce-bfc1-08002be10318}";
            try
            {
                using (var cls = Registry.LocalMachine.OpenSubKey(DisplayClass))
                {
                    if (cls == null) return Enums.GPU.Unknown;

                    foreach (var subName in cls.GetSubKeyNames())
                    {
                        using (var adapter = cls.OpenSubKey(subName))
                        {
                            string? raw = adapter?.GetValue("DriverDesc") as string;
                            if (string.IsNullOrEmpty(raw)) continue;

                            if (log != null) log("Display adapter: " + raw);
                            string name = raw.ToLowerInvariant();

                            // Take the first AMD/NVIDIA adapter (skip Intel iGPUs enumerated first on laptops).
                            if (name.Contains("amd") || name.Contains("radeon")) return Enums.GPU.AMD;
                            if (name.Contains("nvidia") || name.Contains("geforce")) return Enums.GPU.NVIDIA;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (log != null) log("GPU detection error: " + ex.Message);
            }
            return Enums.GPU.Unknown;
        }

        // Applies the shader fix to one downloaded version folder (gameRoot contains Binaries\ and GAMEDATA\).
        // Returns a short human-readable result for logging. Mirrors RetroShaderFix ApplyFixes().
        public static string Apply(string gameRoot, Enums.Update update, Enums.GPU gpu, Action<string> log)
        {
            if (gpu == Enums.GPU.Unknown)
                return "GPU vendor not detected - shader fix skipped (run RetroShaderFix manually if needed)";

            string pcbanks = Path.Combine(gameRoot, "GAMEDATA", "PCBANKS");
            string shaderCache = Path.Combine(gameRoot, "GAMEDATA", "SHADERCACHE");
            string modsFolder = Path.Combine(pcbanks, "MODS");
            string disableMods = Path.Combine(pcbanks, "DISABLEMODS.TXT");

            if (!Directory.Exists(pcbanks))
                return "GAMEDATA\\PCBANKS not found (" + pcbanks + ") - shader fix skipped";

            log("Target PCBANKS: " + pcbanks);
            bool madeChange = false;

            if (update == Enums.Update.Release)
            {
                if (gpu == Enums.GPU.AMD)
                {
                    madeChange = true;
                    ExtractPak("Universal.AMDSpaceMapHorizon.pak", pcbanks, log);
                    ExtractPak("Release.AMDFragData.pak", pcbanks, log);
                }
                // No NVIDIA fix exists for Release.
            }
            else if (update == Enums.Update.Foundation)
            {
                bool hasModsFolder = File.Exists(disableMods) || Directory.Exists(modsFolder);
                if (hasModsFolder)
                    TryDelete(disableMods, log);
                string target = hasModsFolder ? modsFolder : pcbanks;
                if (gpu == Enums.GPU.AMD)
                {
                    madeChange = true;
                    ExtractPak("Universal.AMDSpaceMapHorizon.pak", target, log);
                    ExtractPak("Foundations.AMDTextureArray.pak", target, log);
                }
                else if (gpu == Enums.GPU.NVIDIA)
                {
                    madeChange = true;
                    ExtractPak("Foundations.NVIDIAFragData.pak", target, log);
                }
            }
            else if (update == Enums.Update.PathFinder)
            {
                TryDelete(disableMods, log);
                if (gpu == Enums.GPU.AMD)
                {
                    madeChange = true;
                    ExtractPak("Universal.AMDSpaceMapHorizon.pak", modsFolder, log);
                    ExtractPak("Pathfinder.AMDTextureArray.pak", modsFolder, log);
                }
                else if (gpu == Enums.GPU.NVIDIA)
                {
                    madeChange = true;
                    ExtractPak("Pathfinder.NVIDIAFragData.pak", modsFolder, log);
                }
            }
            else if (update == Enums.Update.AtlasRises)
            {
                TryDelete(disableMods, log);
                if (gpu == Enums.GPU.AMD)
                {
                    madeChange = true;
                    ExtractPak("Universal.AMDSpaceMapHorizon.pak", modsFolder, log);
                    ExtractPak("AtlasRises.AMDTextureArray.pak", modsFolder, log);
                }
                // No NVIDIA fix exists for Atlas Rises.
            }

            if (madeChange)
            {
                if (Directory.Exists(shaderCache))
                {
                    log("Deleting SHADERCACHE to force recompile");
                    try { Directory.Delete(shaderCache, true); } catch { }
                }
                return "Shader fix applied (" + gpu + ")";
            }

            return "No shader fix available for this version/GPU combination";
        }

        private static void TryDelete(string file, Action<string> log)
        {
            try
            {
                if (File.Exists(file))
                {
                    File.Delete(file);
                    log("Deleted " + Path.GetFileName(file));
                }
            }
            catch (Exception ex)
            {
                log("Could not delete " + Path.GetFileName(file) + ": " + ex.Message);
            }
        }

        // Writes an embedded .pak to the target folder, prefixed zzzzzzzzzzzzz so it loads last and overrides stock shaders.
        private static void ExtractPak(string fileName, string outputPath, Action<string> log)
        {
            try
            {
                string outFile = Path.Combine(outputPath, "zzzzzzzzzzzzz" + fileName);
                Payload.Write("InstallerShaderFix." + fileName, outFile);
                log("Wrote " + outFile);
            }
            catch (Exception ex)
            {
                log("Failed to write " + fileName + ": " + ex.Message);
            }
        }

    }
}
