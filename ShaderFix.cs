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
        public static Enums.GPU DetectGPU(Action<string> log = null)
        {
            Enums.GPU result = Enums.GPU.Unknown;
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
                        if (result == Enums.GPU.Unknown)
                        {
                            if (name.Contains("amd") || name.Contains("radeon")) result = Enums.GPU.AMD;
                            else if (name.Contains("nvidia") || name.Contains("geforce")) result = Enums.GPU.nVidia;
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
                // No nVidia fix exists for Release.
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
                else if (gpu == Enums.GPU.nVidia)
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
                else if (gpu == Enums.GPU.nVidia)
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
