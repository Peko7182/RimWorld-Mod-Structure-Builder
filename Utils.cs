using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using Spectre.Console;

namespace RimWorld_Mod_Structure_Builder
{
    public static class Utils
    {
        /// <summary>
        /// Returns the image size in MB
        /// </summary>
        /// <param name="image">Set of bytes</param>
        /// <returns>The image size in MB</returns>
        public static float ImageSize(this byte[] image) => (float)image.Length / 1024 / 1024;

        /// <summary>
        /// Adds the element with the given name and value to the XElement
        /// if the value is not null or empty.
        /// </summary>
        /// <param name="element">The XElement to add to</param>
        /// <param name="name">The name of the element to add</param>
        /// <param name="value">The value of the element to add</param>
        public static void AddIfNotNullOrEmpty(this XElement element, string name, string value)
        {
            if(string.IsNullOrEmpty(value))
                return;

            element.Add(new XElement(name, value));
        }
        
        /// <summary>
        /// Returns the first file found with the given search pattern
        /// </summary>
        /// <param name="path">The path to search in</param>
        /// <param name="searchPattern">The search pattern to look for</param>
        /// <returns>The path of the first found file, or null if not found</returns>
        public static string FirstFoundFile(string path, string searchPattern) => 
            Directory.Exists(path) ? Directory.EnumerateFiles(path, searchPattern).FirstOrDefault() : null;
        
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
                AnsiConsole.MarkupLine("[yellow]⚠[/] Default RimWorld mod folder not found.");
                
                modFolder = AnsiConsole.Prompt(
                    new TextPrompt<string>("[yellow]Enter path to RimWorld mod folder:[/]")
                        .PromptStyle("yellow")
                        .ValidationErrorMessage("[red]That's not a valid path[/]")
                        .Validate(path =>
                        {
                            return Directory.Exists(path)
                                ? ValidationResult.Success()
                                : ValidationResult.Error("[red]Folder does not exist[/]");
                        }));
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

            if (!Directory.Exists(rimWorldFolder))
            {
                AnsiConsole.MarkupLine("[yellow]⚠[/] Default RimWorld folder not found.");
                
                rimWorldFolder = AnsiConsole.Prompt(
                    new TextPrompt<string>("[yellow]Enter path to RimWorld folder:[/]")
                        .PromptStyle("yellow")
                        .ValidationErrorMessage("[red]That's not a valid path[/]")
                        .Validate(path =>
                        {
                            return Directory.Exists(path)
                                ? ValidationResult.Success()
                                : ValidationResult.Error("[red]Folder does not exist[/]");
                        }));
            }

            return rimWorldFolder;
        }

        /// <summary>
        /// Creates a Visual Studio project for a RimWorld mod.
        /// </summary>
        /// <param name="projectName">The name of the project.</param>
        /// <param name="modName">The name of the mod.</param>
        /// <param name="modVersion">The version of the mod.</param>
        /// <param name="description">The description of the mod.</param>
        /// <param name="authors">The authors of the mod.</param>
        /// <param name="rimWorldFolder">The path to the RimWorld folder.</param>
        /// <param name="sourcePath">The path to the source folder.</param>
        public static void CreateVisualStudioProject(string projectName, string modName, string modVersion, string description, List<string> authors, string rimWorldFolder, string sourcePath)
        {
            var csProjGuid = Guid.NewGuid();
            var solutionGuid = Guid.NewGuid();
            
            // Create project folder
            var projectFolder = Path.Combine(sourcePath, projectName);
            Directory.CreateDirectory(projectFolder);
            
            // Create .csproj file
            File.WriteAllText(Path.Combine(projectFolder, $"{projectName}.csproj"), $@"
<?xml version=""1.0"" encoding=""utf-8""?>
<!-- Created with RW Mod Structure Builder -->
<Project ToolsVersion=""15.0"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
  <Import Project=""$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props"" Condition=""Exists('$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props')"" />
  <PropertyGroup>
    <Configuration Condition="" '$(Configuration)' == '' "">Debug</Configuration>
    <Platform Condition="" '$(Platform)' == '' "">AnyCPU</Platform>
    <ProjectGuid>{csProjGuid}</ProjectGuid>
    <OutputType>Library</OutputType>
    <AppDesignerFolder>Properties</AppDesignerFolder>
    <RootNamespace>{projectName}</RootNamespace>
    <AssemblyName>{projectName}</AssemblyName>
    <TargetFrameworkVersion>v4.7.2</TargetFrameworkVersion>
    <FileAlignment>512</FileAlignment>
    <Deterministic>true</Deterministic>
  </PropertyGroup>
  <PropertyGroup Condition="" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' "">
    <DebugSymbols>true</DebugSymbols>
    <DebugType>full</DebugType>
    <Optimize>false</Optimize>
    <OutputPath>..\Common\Assemblies\</OutputPath>
    <DefineConstants>DEBUG;TRACE</DefineConstants>
    <ErrorReport>prompt</ErrorReport>
    <WarningLevel>4</WarningLevel>
  </PropertyGroup>
  <PropertyGroup Condition="" '$(Configuration)|$(Platform)' == 'Release|AnyCPU' "">
    <DebugType>pdbonly</DebugType>
    <Optimize>true</Optimize>
    <OutputPath>..\Common\Assemblies\</OutputPath>
    <DefineConstants>TRACE</DefineConstants>
    <ErrorReport>prompt</ErrorReport>
    <WarningLevel>4</WarningLevel>
  </PropertyGroup>
  <ItemGroup>
    <Reference Include=""System""/>
    
    <Reference Include=""System.Core""/>
    <Reference Include=""System.Xml""/>
    <Reference Include=""System.Xml.Linq""/>
    <Reference Include=""System.Data""/>
    <Reference Include=""System.Data.DataSetExtensions""/>
    <Reference Include=""System.Net.Http""/>
    
    <Reference Include=""Microsoft.CSharp""/>

    <Reference Include=""Assembly-CSharp"">
      <HintPath>{rimWorldFolder}\RimWorldWin64_Data\Managed\Assembly-CSharp.dll</HintPath>
      <Private>False</Private>
    </Reference>
    <Reference Include=""UnityEngine.CoreModule"">
      <HintPath>{rimWorldFolder}\RimWorldWin64_Data\Managed\UnityEngine.CoreModule.dll</HintPath>
      <Private>False</Private>
    </Reference>
  </ItemGroup>
  <ItemGroup>
    <Compile Include=""{projectName}.cs"" />
    <Compile Include=""Mod.cs"" />
    <Compile Include=""Settings.cs"" />
    <Compile Include=""Properties\AssemblyInfo.cs"" />
  </ItemGroup>
  <Import Project=""$(MSBuildToolsPath)\Microsoft.CSharp.targets"" />
</Project>".Trim());

            // Create .sln file
            File.WriteAllText(Path.Combine(sourcePath, $"{projectName}.sln"), $@"
Microsoft Visual Studio Solution File, Format Version 12.00
# Visual Studio Version 17 || Created with RW Mod Structure Builder
VisualStudioVersion = 17.0.31903.59
MinimumVisualStudioVersion = 10.0.40219.1
Project(""{{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}}"") = ""{projectName}"", ""{projectName}.csproj"", ""{{{csProjGuid}}}""
EndProject
Global
    GlobalSection(SolutionConfigurationPlatforms) = preSolution
        Debug|Any CPU = Debug|Any CPU
        Release|Any CPU = Release|Any CPU
    EndGlobalSection
    GlobalSection(SolutionProperties) = preSolution
        HideSolutionNode = FALSE
    EndGlobalSection
    GlobalSection(ProjectConfigurationPlatforms) = postSolution
        {{{csProjGuid}}}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
        {{{csProjGuid}}}.Debug|Any CPU.Build.0 = Debug|Any CPU
        {{{csProjGuid}}}.Release|Any CPU.ActiveCfg = Release|Any CPU
        {{{csProjGuid}}}.Release|Any CPU.Build.0 = Release|Any CPU
    EndGlobalSection
    GlobalSection(ExtensibilityGlobals) = postSolution
		SolutionGuid = {{{solutionGuid}}}
	EndGlobalSection
EndGlobal".Trim());

            // Create main C# file
            File.WriteAllText(Path.Combine(projectFolder, $"{projectName}.cs"), $@"
using RimWorld;
using Verse;

// Created with RW Mod Structure Builder
namespace {projectName}
{{
    [StaticConstructorOnStartup]
    public static class {projectName}
    {{
        static {projectName}()
        {{
            Log.Message($""[{modName} v{modVersion}] Initialized"");
        }}
    }}
}}".Trim());

            // Create settings file
            File.WriteAllText(Path.Combine(projectFolder, $"Settings.cs"), $@"
using Verse;

namespace {projectName}
{{
    public class {projectName}Settings : ModSettings
    {{
        // Setting to enable/disable mod
        public bool EnableMod = true;

        // Method to save and load the settings
        public override void ExposeData()
        {{
            base.ExposeData();
            Scribe_Values.Look(ref EnableMod, ""enableMod"", true);
        }}
    }}
}}".Trim());

            // Create mod file
            File.WriteAllText(Path.Combine(projectFolder, $"Mod.cs"), $@"
using UnityEngine;
using Verse;

namespace {projectName}
{{
    public class {projectName}Mod : Mod
    {{
        // Reference to the settings
        public static {projectName}Settings Settings;

        public {projectName}Mod(ModContentPack content) : base(content)
        {{
            // Initialize the settings
            Settings = GetSettings<{projectName}Settings>();
        }}

        // This is where the settings window UI is drawn
        public override void DoSettingsWindowContents(Rect inRect)
        {{
            // Create a listing for UI elements
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);

            // Checkboxes for the settings
            listingStandard.CheckboxLabeled(""Enable Mod"", ref Settings.EnableMod, ""Enables the mod."");

            listingStandard.End();
        }}

        // The name displayed in the mod settings menu
        public override string SettingsCategory()
        {{
            return ""{projectName}"";
        }}
    }}
}}".Trim());

            // Create Properties folder and AssemblyInfo.cs
            var propertiesPath = Path.Combine(projectFolder, "Properties");
            Directory.CreateDirectory(propertiesPath);
            File.WriteAllText(Path.Combine(propertiesPath, "AssemblyInfo.cs"), $@"
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle(""{projectName}"")]
[assembly: AssemblyDescription(""{description}"")]
[assembly: AssemblyConfiguration("""")]
[assembly: AssemblyCompany(""{string.Join(", ", authors)}"")]
[assembly: AssemblyProduct(""{projectName}"")]
[assembly: AssemblyCopyright(""Copyright © {DateTime.Now.Year}"")]
[assembly: AssemblyTrademark("""")]
[assembly: AssemblyCulture("""")]

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid(""{Guid.NewGuid()}"")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers
// by using the '*' as shown below:
// [assembly: AssemblyVersion(""1.0.*"")]
[assembly: AssemblyVersion(""1.0.0.0"")]
[assembly: AssemblyFileVersion(""{modVersion}"")]
".Trim());
        }
    }
}