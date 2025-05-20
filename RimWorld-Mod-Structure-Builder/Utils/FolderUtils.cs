using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;

namespace RimWorld_Mod_Structure_Builder.Utils;

/// <summary>
/// Utility class for working with RimWorld folders
/// </summary>
public static class FolderUtils
{
    /// <summary>
    /// Returns the path to the RimWorld mods folder.
    /// If the default mods folder is not found, the user is prompted to enter the path.
    /// </summary>
    /// <param name="rimWorldFolder">The path to the RimWorld folder</param>
    /// <returns>The path to the RimWorld mods folder</returns>
    public static string GetRimWorldModFolder(string rimWorldFolder = null)
    {
        rimWorldFolder ??= GetRimWorldFolder();

        var modFolder = Path.Combine(rimWorldFolder, "Mods");

        if (!Directory.Exists(modFolder))
        {
            MessageBox.Show("Mods folder not found. Please enter the path to the RimWorld mods folder.",
                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        return modFolder;
    }

    /// <summary>
    /// Returns the path to the RimWorld folder.
    /// If the default RimWorld folder is not found, the user is prompted to enter the path.
    /// </summary>
    /// <returns>The path to the RimWorld folder</returns>
    public static string GetRimWorldFolder()
    {
        var rimWorldFolder = "";

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            rimWorldFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
                "Steam", "steamapps", "common", "RimWorld");
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            rimWorldFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                "Library", "Application Support", "Steam", "steamapps", "common", "RimWorld", "RimWorldMac.app");
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            rimWorldFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                ".steam", "steam", "steamapps", "common", "RimWorld");
        }

        return rimWorldFolder;
    }
}