using System.IO;
using System.Linq;

namespace RimWorld_Mod_Structure_Builder.Utils;

/// <summary>
/// Utility class for working with mod list
/// </summary>
public static class ModListUtils
{
    public static string GetAboutPath(string modPath)
    {
        var aboutFile = Directory
            .EnumerateFiles(modPath, "About.xml", SearchOption.AllDirectories)
            .FirstOrDefault(file => new DirectoryInfo(Path.GetDirectoryName(file) ?? string.Empty).Name == "About");

        return aboutFile ?? string.Empty;
    }
}