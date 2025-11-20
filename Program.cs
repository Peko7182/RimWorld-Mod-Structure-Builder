using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Spectre.Console;

namespace RimWorld_Mod_Structure_Builder
{
    internal class Program
    {
        static void Main()
        {
            Console.OutputEncoding = Encoding.Unicode;
            
            AnsiConsole.Write(
                new FigletText("RimWorld Mod Creator")
                    .Centered()
                    .Color(Color.Cyan1));

            // Get RimWorld Mod Folder
            var rimWorldFolder = Utils.GetRimWorldFolder();
            var modFolder = Utils.GetRimWorldModFolder(rimWorldFolder);
            
            AnsiConsole.MarkupLine($"[green]✓[/] RimWorld mod folder: [cyan]{modFolder}[/]\n");

            // Get Image Path
            var imagePanel = new Panel(
                new Markup("[yellow]Image Requirements:[/]\n" +
                          "• Size: 640x360 or 1280x720 PNG\n" +
                          "• Must be under 1MB\n" +
                          "• Non-PNG files can be renamed to Preview.png"))
            {
                Header = new PanelHeader("Preview Image", Justify.Left),
                Border = BoxBorder.Rounded,
                BorderStyle = new Style(Color.Yellow)
            };
            AnsiConsole.Write(imagePanel);

            byte[] image = null;
            string imagePath = null;
            
            while (true)
            {
                imagePath = AnsiConsole.Prompt(
                    new TextPrompt<string>("[yellow]Preview image path:[/]")
                        .AllowEmpty());

                if (string.IsNullOrEmpty(imagePath))
                {
                    AnsiConsole.MarkupLine("[yellow]⚠[/] Preview.png is [red]REQUIRED[/] per RimWorld Mod Structure Wiki. Add it later.\n");
                    break;
                }
                else if (File.Exists(imagePath))
                {
                    image = File.ReadAllBytes(imagePath);
                    var size = image.ImageSize();
                    AnsiConsole.MarkupLine(size >= 1
                        ? $"[yellow]⚠[/] Preview.png is [red]OVER 1MB[/] ({size:F2} MB). Steam Workshop upload not possible.\n"
                        : $"[green]✓[/] Image loaded successfully ({size:F2} MB)\n");
                    break;
                }
                else
                {
                    AnsiConsole.MarkupLine("[red]✗[/] File not found. Please try again.\n");
                }
            }

            // Get Mod Icon
            var iconPanel = new Panel(
                new Markup("[cyan]Icon Information:[/]\n" +
                          "• No icon? Preview.png will be used\n" +
                          "• Shows during loading screens & mod settings\n" +
                          "• Recommended: 32x32 or 64x64 PNG with low detail"))
            {
                Header = new PanelHeader("Mod Icon", Justify.Left),
                Border = BoxBorder.Rounded,
                BorderStyle = new Style(Color.Cyan)
            };
            AnsiConsole.Write(iconPanel);

            var modIcon = AnsiConsole.Prompt(
                new TextPrompt<string>("[cyan]Mod icon path:[/]")
                    .AllowEmpty());

            // Get Mod Metadata
            AnsiConsole.Write(new Rule("[green]Mod Information[/]").RuleStyle("green"));

            var modName = AnsiConsole.Ask<string>("[green]Mod name:[/]");
            var packageId = AnsiConsole.Ask<string>("[green]Package ID[/] [dim](e.g., AuthorName.ModName)[/]:");

            var authors = new List<string>();
            AnsiConsole.MarkupLine("[green]Author(s)[/] [dim](press Enter on empty line to finish)[/]:");
            while (true)
            {
                var author = AnsiConsole.Prompt(
                    new TextPrompt<string>($"  [dim]Author {authors.Count + 1}:[/]")
                        .AllowEmpty());
                if (string.IsNullOrEmpty(author)) break;
                authors.Add(author);
            }

            var description = AnsiConsole.Ask<string>("[green]Description:[/]");

            var supportedVersions = new List<string>();
            AnsiConsole.MarkupLine("[green]Supported RimWorld versions[/] [dim](e.g., 1.5, press Enter to finish)[/]:");
            while (true)
            {
                var version = AnsiConsole.Prompt(
                    new TextPrompt<string>($"  [dim]Version {supportedVersions.Count + 1}:[/]")
                        .AllowEmpty());
                if (string.IsNullOrEmpty(version)) break;
                supportedVersions.Add(version);
            }

            XElement modMetaData = new XElement("ModMetaData",
                new XElement("packageId", packageId),
                new XElement("name", modName),
                authors.Count == 1 
                    ? new XElement("author", authors[0]) 
                    : new XElement("authors", authors.Select(author => new XElement("li", author))),
                new XElement("description", description),
                new XElement("supportedVersions", supportedVersions.Select(version => new XElement("li", version)))
            );

            // Optional parameters
            var modVersion = "0.0.0.1";
            modMetaData.AddIfNotNullOrEmpty("modVersion", modVersion);
            
            var url = AnsiConsole.Prompt(
                new TextPrompt<string>("[blue]URL[/] [dim](GitHub, etc.)[/]:")
                    .AllowEmpty());
            modMetaData.AddIfNotNullOrEmpty("url", url);

            // Optional parameters - Mod Dependencies
            if (AnsiConsole.Confirm("[blue]Add mod dependencies?[/]", false))
            {
                XElement modDependencies = new XElement("modDependencies");
                var addingDeps = true;
                
                while (addingDeps)
                {
                    var dependencyId = AnsiConsole.Prompt(
                        new TextPrompt<string>("  [blue]Dependency package ID:[/]")
                            .AllowEmpty());
                    
                    if (string.IsNullOrEmpty(dependencyId))
                        break;

                    var dependencyDisplayName = AnsiConsole.Ask<string>("  [blue]Display name:[/]");
                    var steamWorkshopUrl = AnsiConsole.Ask<string>("  [blue]Steam Workshop URL:[/]");

                    XElement dependency = new XElement("li",
                        new XElement("packageId", dependencyId),
                        new XElement("displayName", dependencyDisplayName),
                        new XElement("steamWorkshopUrl", steamWorkshopUrl)
                    );
                    modDependencies.Add(dependency);
                    
                    addingDeps = AnsiConsole.Confirm("  [dim]Add another dependency?[/]", true);
                }
                
                if (modDependencies.HasElements)
                    modMetaData.Add(modDependencies);
            }

            // Optional parameters - Load Order & Incompatibilities
            var loadOrderTypes = new[] 
            { 
                ("load before", "loadBefore", "Load Before"), 
                ("load after", "loadAfter", "Load After"), 
                ("incompatible with", "incompatibleWith", "Incompatible With") 
            };

            foreach (var (desc, elementName, displayName) in loadOrderTypes)
            {
                if (AnsiConsole.Confirm($"[blue]Add {desc} dependencies?[/]", false))
                {
                    XElement dependencies = new XElement(elementName);
                    
                    while (true)
                    {
                        var dependencyId = AnsiConsole.Prompt(
                            new TextPrompt<string>($"  [blue]{displayName} package ID:[/]")
                                .AllowEmpty());
                        
                        if (string.IsNullOrEmpty(dependencyId))
                            break;
                        
                        dependencies.Add(new XElement("li", dependencyId));
                    }
                    
                    if (dependencies.HasElements)
                        modMetaData.Add(dependencies);
                }
            }

            // Create ModFolder
            var newModFolder = Path.Combine(modFolder, modName);
            while (Directory.Exists(newModFolder))
            {
                AnsiConsole.MarkupLine("[red]✗[/] Mod folder already exists!");
                modName = AnsiConsole.Ask<string>("[yellow]Enter a new mod name:[/]");
                newModFolder = Path.Combine(modFolder, modName);
            }

            // Create structure with progress
            Directory.CreateDirectory(newModFolder);
            
            var folders = new List<string> { "Assemblies", "Defs", "Languages", "Patches", "Sounds", "Textures" };
            Directory.CreateDirectory(Path.Combine(newModFolder, "About"));

            // Visual Studio project
            var visualStudioProject = AnsiConsole.Confirm("[magenta]Create Visual Studio project?[/]");
            
            if (visualStudioProject)
            {
                var commonPath = Path.Combine(newModFolder, "Common");
                Directory.CreateDirectory(commonPath);

                var sourcePath = Path.Combine(newModFolder, "Source");
                Directory.CreateDirectory(sourcePath);

                folders.ForEach(f => Directory.CreateDirectory(Path.Combine(commonPath, f)));

                var projectName = modName.Replace(" ", "");
                Utils.CreateVisualStudioProject(projectName, modName, modVersion, description, authors, rimWorldFolder, sourcePath);

                AnsiConsole.MarkupLine($"[green]✓[/] Visual Studio project created in [cyan]{sourcePath}[/]");
            }
            else
            {
                folders.ForEach(f => Directory.CreateDirectory(Path.Combine(newModFolder, f)));
            }

            // Save About.xml
            new XDocument(new XDeclaration("1.0", "utf-8", null), modMetaData)
                .Save(Path.Combine(newModFolder, "About", "About.xml"));

            // Save Preview.png
            if (!string.IsNullOrEmpty(imagePath))
                File.Copy(imagePath, Path.Combine(newModFolder, "About", "Preview.png"));
            else
                AnsiConsole.MarkupLine("[yellow]⚠[/] Don't forget to add Preview.png");

            // Save ModIcon.png
            if (!string.IsNullOrEmpty(modIcon))
                File.Copy(modIcon, Path.Combine(newModFolder, "About", "ModIcon.png"));
            else if (!string.IsNullOrEmpty(imagePath))
                File.Copy(imagePath, Path.Combine(newModFolder, "About", "ModIcon.png"));
            else
                AnsiConsole.MarkupLine("[yellow]⚠[/] Don't forget to add ModIcon.png");

            // Success message
            var successPanel = new Panel(
                new Markup($"[green]Mod structure created successfully![/]\n\n" +
                          $"[cyan]Location:[/] {newModFolder}"))
            {
                Header = new PanelHeader("✓ Success", Justify.Center),
                Border = BoxBorder.Double,
                BorderStyle = new Style(Color.Green)
            };
            AnsiConsole.Write(successPanel);

            // Show folder information
            var table = new Table()
                .Border(TableBorder.Rounded)
                .BorderColor(Color.Blue)
                .AddColumn(new TableColumn("[blue]Folder[/]").Centered())
                .AddColumn(new TableColumn("[blue]Description[/]"));

            var folderInfo = new []
            {
                ( "Assemblies", "Custom code in compiled DLL files" ),
                ( "Defs", "XML Definitions for content configuration" ),
                ( "Languages", "Localization and translations" ),
                ( "Patches", "Modify Defs from vanilla game or mods" ),
                ( "Sounds", "Custom sound files (Ogg, MP3, WAV)" ),
                ( "Textures", "Custom texture files (PNG)")
            };

            foreach (var (folder, desc) in folderInfo)
            {
                table.AddRow($"[cyan]{folder}[/]", $"[dim]{desc}[/]");
            }

            AnsiConsole.Write(table);

            // Final actions
            if (AnsiConsole.Confirm("\n[blue]Open RimWorld Wiki for mod folder structure?[/]"))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "https://rimworldwiki.com/wiki/Modding_Tutorials/Mod_Folder_Structure",
                    UseShellExecute = true
                });
            }

            AnsiConsole.MarkupLine("\n[green]Opening mod folder...[/]");
            Process.Start(new ProcessStartInfo
            {
                FileName = newModFolder,
                UseShellExecute = true
            });

            Process.Start(new ProcessStartInfo
            {
                FileName = Path.Combine(newModFolder, "About", "About.xml"),
                UseShellExecute = true
            });

            var slnFile = Utils.FirstFoundFile(Path.Combine(newModFolder, "Source"), "*.sln");
            if (!string.IsNullOrEmpty(slnFile))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = slnFile,
                    UseShellExecute = true
                });
            }

            AnsiConsole.MarkupLine("\n[green]Press any key to exit...[/]");
            Console.ReadKey();
        }
    }
}