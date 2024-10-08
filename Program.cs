﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace RimWorld_Mod_Structure_Builder
{
    internal class Program
    {
        static void Main()
        {
            Logging.Log("Welcome to the RimWorld Mod Creator!");
            
            // Get RimWorld Mod Folder
            var rimWorldFolder = Utils.GetRimWorldFolder();
            var modFolder = Utils.GetRimWorldModFolder(rimWorldFolder);
            Logging.Log($"RimWorld mod folder found: {modFolder}");

            // Get Image Path
            Logging.Info("Use 640x360 or 1280x720 PNG.");
            Logging.Info("Must be under 1MB.");
            Logging.Info("Non-PNG files can be used by renaming them to Preview.png.");
            byte[] image = null;
            string imagePath = null;
            while (true)
            {
                imagePath = Utils.GetSingleInput("Enter your mod preview image path:", required: false);
                if (string.IsNullOrEmpty(imagePath))
                {
                    Logging.Warn("Preview.png IS REQUIRED based on THE RIMWORLD MOD STRUCTURE WIKI. You will need to add it later.\n");
                    break;
                }
                else if (File.Exists(imagePath))
                {
                    image = File.ReadAllBytes(imagePath);
                    if (image.ImageSize() >= 1)
                    {
                        Logging.Warn($"Preview.png IS OVER 1MB ({image.ImageSize()} MB). STEAM WORKSHOP UPLOAD NOT POSSIBLE.\n");
                    }
                    break;
                }
                else
                    Logging.Error("The file you entered doesn't exist. Please enter a valid path.\n");
            }
            
            // Get Mod MetaData
            Logging.Info("No mod icon? Preview.png is used instead.");
            Logging.Info("ModIcon.png is shown during game loading screens and in the Options UI if your mod has mod settings.");
            Logging.Info("32x32 or 64x64 PNG file, low detail/colors are recommended.");
            var modIcon = Utils.GetSingleInput("Enter your mod icon path:", required: false);
            
            var modName = Utils.GetSingleInput("Enter your mod name:");
            var packageId = Utils.GetSingleInput("Enter your package ID (e.g., AuthorName.ModName):");
            var authors = Utils.GetMultipleInputs("Enter the author name(s):");
            var description = Utils.GetSingleInput("Enter a description for your mod:");
            var supportedVersions = Utils.GetMultipleInputs("Enter the supported RimWorld versions (e.g., 1.5):");

            XElement modMetaData = new XElement("ModMetaData",
                new XElement("packageId", packageId),
                new XElement("name", modName),
                authors.Count == 1 ? new XElement("author", authors[0]) : new XElement("authors", authors.Select(author => new XElement("li", author))),
                new XElement("description", description),
                new XElement("supportedVersions", supportedVersions.Select(version => new XElement("li", version)))
            );

            // Optional parameters
            var modVersion = "0.0.0.1";
            modMetaData.AddIfNotNullOrEmpty("modVersion", modVersion);
            modMetaData.AddIfNotNullOrEmpty("url", Utils.GetSingleInput("Enter the URL (e.g., GitHub, etc.):", required: false));

            // Optional parameters - Mod Dependencies
            if (Utils.GetYesNoInput("Do you want to add mod dependencies? (Optional)"))
            {
                XElement modDependencies = new XElement("modDependencies");
                while (true)
                {
                    var dependencyId = Utils.GetSingleInput("Enter dependency package ID:", required: false);
                    if (string.IsNullOrEmpty(dependencyId))
                        break;
                    
                    var dependencyDisplayName = Utils.GetSingleInput("Enter dependency display name:");
                    var steamWorkshopUrl = Utils.GetSingleInput("Enter dependency Steam Workshop URL:");
                    
                    XElement dependency = new XElement("li",
                        new XElement("packageId", dependencyId),
                        new XElement("displayName", dependencyDisplayName),
                        new XElement("steamWorkshopUrl", steamWorkshopUrl)
                    );
                    modDependencies.Add(dependency);
                }
                modMetaData.Add(modDependencies);
            }
            
            // Optional parameters - Load Before, Load After, Incompatible With
            foreach (var (name, elementName) in new[] { ("load before", "loadBefore"), ("load after", "loadAfter"), ("incompatible with", "incompatibleWith") })
            {
                if (Utils.GetYesNoInput($"Do you want to add {name} dependencies? (Optional)"))
                {
                    XElement dependencies = new XElement(elementName);
                    while (true)
                    {
                        var dependencyId = Utils.GetSingleInput($"Enter {name} package ID:", required: false);
                        if (string.IsNullOrEmpty(dependencyId))
                            break;
                        dependencies.Add(new XElement("li", dependencyId));
                    }
                    modMetaData.Add(dependencies);
                }
            }

            // Create ModFolder
            var newModFolder = Path.Combine(modFolder, modName);
            while (Directory.Exists(newModFolder))
            {
                Logging.Error("Mod folder already exists. Please enter a new name:");
                modName = Utils.GetSingleInput("Enter a new mod name:");
                newModFolder = Path.Combine(modFolder, modName);
            }
            Utils.CreateDirectory(newModFolder);

            // Create mod folder structure
            var folders = new List<string> { "Assemblies", "Defs", "Languages", "Patches", "Sounds", "Textures" };

            // Create About folder in the main ModFolder
            Utils.CreateDirectory(Path.Combine(newModFolder, "About"));
            
            // Create a Visual Studio project if requested
            var visualStudioProject = Utils.GetYesNoInput("Do you want to create a Visual Studio project?");
            if (visualStudioProject)
            {
                // Create Common folder
                var commonPath = Path.Combine(newModFolder, "Common");
                Utils.CreateDirectory(commonPath);
                
                // Create Source folder
                var sourcePath = Path.Combine(newModFolder, "Source");
                Utils.CreateDirectory(sourcePath);

                // Create folders inside Common folder
                folders.ForEach(f => Utils.CreateDirectory(Path.Combine(commonPath, f)));

                // Create Visual Studio Project
                var projectName = modName.Replace(" ", "");

                Utils.CreateVisualStudioProject(projectName, modName, modVersion, description, authors, rimWorldFolder, sourcePath);

                Logging.Success($"Visual Studio project created successfully in '{sourcePath}'.");
            }
            else
            {
                // Create folders in ModFolder
                folders.ForEach(f => Utils.CreateDirectory(Path.Combine(newModFolder, f)));
            }

            // Save About.xml
            new XDocument(new XDeclaration("1.0", "utf-8", null), modMetaData)
                .Save(Path.Combine(newModFolder, "About", "About.xml"));

            // Save Preview.png
            if (!string.IsNullOrEmpty(imagePath))
                File.Copy(imagePath, Path.Combine(newModFolder, "About", "Preview.png"));
            else
                Logging.Warn("Don't forget to add a preview image.");

            // Save ModIcon.png
            if (!string.IsNullOrEmpty(modIcon))
                File.Copy(modIcon, Path.Combine(newModFolder, "About", "ModIcon.png"));
            else if (!string.IsNullOrEmpty(imagePath))
                File.Copy(imagePath, Path.Combine(newModFolder, "About", "ModIcon.png"));
            else
                Logging.Warn("Don't forget to add a mod icon.");

            Logging.Success($"Mod structure created successfully in '{newModFolder}'.");

            // Show information about mod structure
            var infoMessages = new List<string>
            {
                "Assemblies: Add custom code to RimWorld in the form of compiled dynamic-link library or DLL files.",
                "Defs: XML Definitions or Defs are the primary content definition and configuration source for RimWorld.",
                "Languages: Localization and translations for Defs and code-referenced text.",
                "Patches: Modify Defs from the vanilla game, DLCs, or even other mods in a safe and interoperable manner.",
                "Sounds: Custom sound files for mods. Use Ogg, MP3, or WAV files.",
                "Textures: Custom texture files for mods. Use PNG files."
            };
            infoMessages.ForEach(Logging.Info);

            // Open RimWorld wiki
            if (Utils.GetYesNoInput("Do you want to see the RimWorld Wiki for mod folder structure?"))
            {
                Process.Start("https://rimworldwiki.com/wiki/Modding_Tutorials/Mod_Folder_Structure");
            }

            // Open mod folder and about file
            Process.Start(newModFolder);
            Process.Start(Path.Combine(newModFolder, "About", "About.xml"));
            
            // Open Visual Studio project
            if (visualStudioProject)
                Process.Start(Utils.FirstFoundFile(Path.Combine(newModFolder, "Source"), "*.sln"));

            // Exit
            Environment.FailFast(string.Empty);
        }
    }
}
