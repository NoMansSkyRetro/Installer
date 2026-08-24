using System;
using System.IO;
using System.Management;
using System.Reflection;

namespace NMSLegacyVersionInstaller
{
    // Shader fix logic ported from RetroShaderFix by Qjimbo.
    // The .pak files (the actual fix) are made by Ethan (EthanRDoesMC) - https://github.com/EthanRDoesMC/RetroShaderFix
    public static class ShaderFix
    {
        public enum Update { Release, Foundation, PathFinder, AtlasRises }
        public enum Gpu { Unknown, AMD, nVidia }

        // Version-based dummy Steam IDs (14 digits: major+minor of the version, zero padded).
        // Each legacy version gets its own id so their save folders never collide - no SmartSaveFolder needed.
        public static string DummySteamId(Update update)
        {
            switch (update)
            {
                case Update.Release: return "00000000000019";     // v1.09
                case Update.Foundation: return "00000000000113";  // v1.13
                case Update.PathFinder: return "00000000000124";  // v1.24
                case Update.AtlasRises: return "00000000000138";  // v1.38
                default: return "00000000000019";
            }
        }

        public static Gpu DetectGpu(Action<string> log = null)
        {
            Gpu result = Gpu.Unknown;
            try
            {
                using (var searcher = new ManagementObjectSearcher("SELECT Name FROM Win32_VideoController"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        string raw = (obj["Name"] ?? "").ToString();
                        if (log != null) log("Display adapter: " + raw);
                        string name = raw.ToLower();
                        // Take the first AMD/NVIDIA adapter (skip Intel iGPUs enumerated first on laptops).
                        if (result == Gpu.Unknown)
                        {
                            if (name.Contains("amd") || name.Contains("radeon")) result = Gpu.AMD;
                            else if (name.Contains("nvidia") || name.Contains("geforce")) result = Gpu.nVidia;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (log != null) log("GPU detection error: " + ex.Message);
            }
            return result;
        }

        // Applies the shader fix to one downloaded version folder (gameRoot contains Binaries\ and GAMEDATA\).
        // Returns a short human-readable result for logging. Mirrors RetroShaderFix ApplyFixes().
        public static string Apply(string gameRoot, Update update, Gpu gpu, Action<string> log)
        {
            if (gpu == Gpu.Unknown)
                return "GPU vendor not detected - shader fix skipped (run RetroShaderFix manually if needed)";

            string pcbanks = Path.Combine(gameRoot, "GAMEDATA", "PCBANKS");
            string shaderCache = Path.Combine(gameRoot, "GAMEDATA", "SHADERCACHE");
            string modsFolder = Path.Combine(pcbanks, "MODS");
            string disableMods = Path.Combine(pcbanks, "DISABLEMODS.TXT");

            if (!Directory.Exists(pcbanks))
                return "GAMEDATA\\PCBANKS not found (" + pcbanks + ") - shader fix skipped";

            log("Target PCBANKS: " + pcbanks);
            bool madeChange = false;

            if (update == Update.Release)
            {
                if (gpu == Gpu.AMD)
                {
                    madeChange = true;
                    ExtractPak("Universal.AMDSpaceMapHorizon.pak", pcbanks, log);
                    ExtractPak("Release.AMDFragData.pak", pcbanks, log);
                }
                // No nVidia fix exists for Release.
            }
            else if (update == Update.Foundation)
            {
                bool hasModsFolder = File.Exists(disableMods) || Directory.Exists(modsFolder);
                if (hasModsFolder)
                    TryDelete(disableMods, log);
                string target = hasModsFolder ? modsFolder : pcbanks;
                if (gpu == Gpu.AMD)
                {
                    madeChange = true;
                    ExtractPak("Universal.AMDSpaceMapHorizon.pak", target, log);
                    ExtractPak("Foundations.AMDTextureArray.pak", target, log);
                }
                else if (gpu == Gpu.nVidia)
                {
                    madeChange = true;
                    ExtractPak("Foundations.NVIDIAFragData.pak", target, log);
                }
            }
            else if (update == Update.PathFinder)
            {
                TryDelete(disableMods, log);
                if (gpu == Gpu.AMD)
                {
                    madeChange = true;
                    ExtractPak("Universal.AMDSpaceMapHorizon.pak", modsFolder, log);
                    ExtractPak("Pathfinder.AMDTextureArray.pak", modsFolder, log);
                }
                else if (gpu == Gpu.nVidia)
                {
                    madeChange = true;
                    ExtractPak("Pathfinder.NVIDIAFragData.pak", modsFolder, log);
                }
            }
            else if (update == Update.AtlasRises)
            {
                TryDelete(disableMods, log);
                if (gpu == Gpu.AMD)
                {
                    madeChange = true;
                    ExtractPak("Universal.AMDSpaceMapHorizon.pak", modsFolder, log);
                    ExtractPak("AtlasRises.AMDTextureArray.pak", modsFolder, log);
                }
                // No nVidia fix exists for Atlas Rises.
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
            string resourceName = "NMSLegacyVersionInstaller.InstallerShaderFix." + fileName;
            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                using (Stream resourceStream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (resourceStream == null)
                    {
                        log("WARNING: embedded pak not found: " + resourceName);
                        return;
                    }
                    if (!Directory.Exists(outputPath))
                        Directory.CreateDirectory(outputPath);
                    string outFile = Path.Combine(outputPath, "zzzzzzzzzzzzz" + fileName);
                    using (FileStream fileStream = new FileStream(outFile, FileMode.Create))
                    {
                        resourceStream.CopyTo(fileStream);
                    }
                    log("Wrote " + outFile);
                }
            }
            catch (Exception ex)
            {
                log("Failed to write " + fileName + ": " + ex.Message);
            }
        }
    }
}
