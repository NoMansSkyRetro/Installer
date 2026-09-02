using System;
using System.IO;
using System.Reflection;

namespace NMSRetroInstaller;

/// <summary>
/// The files carried inside the installer - the patched steam_api64.dll, the shader fix paks, the
/// shortcut icons and RetroShaderFix - written out where the install needs them.
/// </summary>
public static class Payload
{
    // Embedded resources are named after the root namespace, which tracks the assembly name.
    static readonly string Prefix = typeof(Payload).Assembly.GetName().Name + ".";

    /// <summary>Writes one embedded file, e.g. <c>InstallerFiles.steam_api64.dll</c>, to a full path.</summary>
    public static void Write(string resource, string destination)
    {
        using var source = Assembly.GetExecutingAssembly().GetManifestResourceStream(Prefix + resource)
            ?? throw new FileNotFoundException("Missing embedded file: " + resource);

        Directory.CreateDirectory(Path.GetDirectoryName(destination)!);

        using var file = File.Create(destination);
        source.CopyTo(file);
    }

    /// <summary>Writes every embedded file under a folder, e.g. <c>InstallerExtras</c>, into a folder.</summary>
    public static void WriteFolder(string folder, string destination)
    {
        var prefix = Prefix + folder + ".";
        Directory.CreateDirectory(destination);

        foreach (var name in Assembly.GetExecutingAssembly().GetManifestResourceNames())
            if (name.StartsWith(prefix, StringComparison.Ordinal))
                Write(name[Prefix.Length..], Path.Combine(destination, name[prefix.Length..]));
    }
}
