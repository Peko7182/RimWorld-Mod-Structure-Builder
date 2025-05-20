using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml;
using RimWorld_Mod_Structure_Builder.Utils;

namespace RimWorld_Mod_Structure_Builder.InfoClasses;

/// <summary>
/// Class for storing information about a mod
/// </summary>
public sealed class ModInfo : INotifyPropertyChanged
{
    private string _xmlPath;
    private string _id;
    private string _name;
    private ObservableCollection<AuthorInfo> _authors;
    private ObservableCollection<DescriptionInfo> _descriptions;
    private ObservableCollection<SupportedVersionInfo> _supportedVersions;
    private string _version;
    private string _modIconPath;
    private string _url;
    private ObservableCollection<DependencyInfo> _modDependencies;
    private ObservableCollection<IncompatibleInfo> _incompatibleMods;
    private ObservableCollection<LoadOrderInfo> _loadBefore;
    private ObservableCollection<LoadOrderInfo> _loadAfter;

    // Class properties
    public string XmlPath
    {
        get => _xmlPath;
        set
        {
            if (_xmlPath == value) return;
            _xmlPath = value;
            OnPropertyChanged();
        }
    }

    // Required properties
    public string Id
    {
        get => _id;
        set
        {
            if (_id == value) return;
            _id = value;
            OnPropertyChanged();
        }
    }

    public string Name
    {
        get => _name;
        set
        {
            if (_name == value) return;
            _name = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<AuthorInfo> Authors
    {
        get => _authors;
        set
        {
            if (_authors == value) return;
            _authors = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<DescriptionInfo> Descriptions
    {
        get => _descriptions;
        set
        {
            if (_descriptions == value) return;
            _descriptions = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<SupportedVersionInfo> SupportedVersions
    {
        get => _supportedVersions;
        set
        {
            if (_supportedVersions == value) return;
            _supportedVersions = value;
            OnPropertyChanged();
        }
    }

    // Optional properties
    public string Version
    {
        get => _version;
        set
        {
            if (_version == value) return;
            _version = value;
            OnPropertyChanged();
        }
    }

    public string ModIconPath
    {
        get => _modIconPath;
        set
        {
            if (_modIconPath == value) return;
            _modIconPath = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(HasModIcon));
        }
    }

    public string Url
    {
        get => _url;
        set
        {
            if (_url == value) return;
            _url = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<DependencyInfo> ModDependencies
    {
        get => _modDependencies;
        set
        {
            if (_modDependencies == value) return;
            _modDependencies = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<IncompatibleInfo> IncompatibleMods
    {
        get => _incompatibleMods;
        set
        {
            if (_incompatibleMods == value) return;
            _incompatibleMods = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<LoadOrderInfo> LoadBefore
    {
        get => _loadBefore;
        set
        {
            if (_loadBefore == value) return;
            _loadBefore = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<LoadOrderInfo> LoadAfter
    {
        get => _loadAfter;
        set
        {
            if (_loadAfter == value) return;
            _loadAfter = value;
            OnPropertyChanged();
        }
    }

    // Computed property for UI
    public bool HasModIcon => !string.IsNullOrEmpty(ModIconPath);

    /// <summary>
    /// Loads information about a mod from an About.xml file
    /// </summary>
    /// <param name="xmlPath">Path to the About.xml file to load</param>
    public void LoadFromXml(string xmlPath)
    {
        var xmlDoc = new XmlDocument();
        xmlDoc.Load(xmlPath);

        var xmlData = xmlDoc["ModMetaData"];
        if (xmlData == null)
            return;

        XmlPath = xmlPath;

        Id = xmlData["packageId"]?.InnerText;
        Name = xmlData["name"]?.InnerText;

        Authors = xmlData["author"] != null
            ? [new AuthorInfo(xmlData["author"].InnerText)]
            : xmlData["authors"]?.GetElementsByTagName("li")
                .Cast<XmlNode>()
                .Select(node => new AuthorInfo(node.InnerText))
                .ToObservableCollection();

        Descriptions = xmlData["description"] != null
            ? [new DescriptionInfo(xmlData["description"].InnerText)]
            : xmlData["descriptionsByVersion"]?.ChildNodes
                .Cast<XmlNode>()
                .Select(node => new DescriptionInfo(node["description"].InnerText, node.Name))
                .ToObservableCollection();

        SupportedVersions = xmlData["supportedVersions"]?.GetElementsByTagName("li")
            .Cast<XmlNode>()
            .Select(node => new SupportedVersionInfo(node.InnerText))
            .ToObservableCollection();

        Version = xmlData["modVersion"]?.InnerText;

        var modFolderPath = Path.GetDirectoryName(xmlPath);
        string[] possibleIconPaths =
        {
            Path.Combine(modFolderPath, "Preview.png"),
            Path.Combine(modFolderPath, "ModIcon.png"),
            Path.Combine(modFolderPath, "Icon.png")
        };

        foreach (var iconPath in possibleIconPaths)
        {
            if (!File.Exists(iconPath)) continue;

            ModIconPath = iconPath;
            break;
        }

        Url = xmlData["url"]?.InnerText;

        ModDependencies = xmlData["modDependencies"]?.GetElementsByTagName("li")
                              .Cast<XmlNode>()
                              .Select(node => new DependencyInfo(node["packageId"].InnerText,
                                  node["displayName"].InnerText, node["steamWorkshopUrl"].InnerText))
                              .ToObservableCollection() ??
                          xmlData["modDependenciesByVersion"]?.ChildNodes
                              .Cast<XmlNode>()
                              .Select(node =>
                                  new DependencyInfo(node["packageId"].InnerText, node["displayName"].InnerText,
                                      node["steamWorkshopUrl"].InnerText) { Version = node.Name })
                              .ToObservableCollection();

        IncompatibleMods = xmlData["incompatibleWith"]?.GetElementsByTagName("li")
                               .Cast<XmlNode>()
                               .Select(node => new IncompatibleInfo { Id = node.InnerText })
                               .ToObservableCollection() ??
                           xmlData["incompatibleWithByVersion"]?.ChildNodes
                               .Cast<XmlNode>()
                               .SelectMany(versionNode => versionNode.SelectNodes("li")
                                   .Cast<XmlNode>()
                                   .Select(node => new IncompatibleInfo
                                       { Id = node.InnerText, Version = versionNode.Name }))
                               .ToObservableCollection();

        // Load Before
        LoadBefore = xmlData["loadBefore"]?.GetElementsByTagName("li")
                         .Cast<XmlNode>()
                         .Select(node => new LoadOrderInfo(node.InnerText, (string)null, false))
                         .ToObservableCollection()
                     ??
                     xmlData["loadBeforeByVersion"]?.ChildNodes
                         .Cast<XmlNode>()
                         .SelectMany(versionNode => versionNode.SelectNodes("li").Cast<XmlNode>()
                             .Select(node => new LoadOrderInfo(node.InnerText, versionNode.Name, false)))
                         .ToObservableCollection();

        xmlData["forceLoadBefore"]?.GetElementsByTagName("li")
            .Cast<XmlNode>()
            .Select(node => new LoadOrderInfo(node.InnerText, (string)null, true))
            .ToList()
            .ForEach(loadOrderInfo =>
            {
                var existingInfo = LoadBefore.FirstOrDefault(info => info.Id == loadOrderInfo.Id);
                existingInfo.Forced = true;
            });

        // Load after
        LoadAfter = xmlData["loadAfter"]?.GetElementsByTagName("li")
                        .Cast<XmlNode>()
                        .Select(node => new LoadOrderInfo(node.InnerText, (string)null, false))
                        .ToObservableCollection()
                    ??
                    xmlData["loadAfterByVersion"]?.ChildNodes
                        .Cast<XmlNode>()
                        .SelectMany(versionNode => versionNode.SelectNodes("li").Cast<XmlNode>()
                            .Select(node => new LoadOrderInfo(node.InnerText, versionNode.Name, false)))
                        .ToObservableCollection();

        xmlData["forceLoadAfter"]?.GetElementsByTagName("li")
            .Cast<XmlNode>()
            .Select(node => new LoadOrderInfo(node.InnerText, (string)null, true))
            .ToList()
            .ForEach(loadOrderInfo =>
            {
                var existingInfo = LoadAfter.FirstOrDefault(info => info.Id == loadOrderInfo.Id);
                existingInfo.Forced = true;
            });
    }

    /// <summary>
    /// Saves this mod's information to an XML file.
    /// </summary>
    /// <param name="xmlPath">The path to which the XML file should be saved.</param>
    /// <returns>
    /// A tuple containing a boolean and a string. The boolean indicates whether the XML was saved. The string is a status message.
    /// </returns>
    /// <remarks>
    /// If the XML file already exists, the method will only save the XML if it has changed. Otherwise, it will not overwrite the existing file.
    /// </remarks>
    public (bool, string) SaveToXml(string xmlPath)
    {
        var xmlDoc = new XmlDocument();
        var xmlData = xmlDoc.CreateElement("ModMetaData");
        xmlDoc.AppendChild(xmlData);

        XmlUtils.AddSimpleNode(xmlData, "modVersion", Version);
        XmlUtils.AddSimpleNode(xmlData, "name", Name);
        XmlUtils.AddListNode(xmlData, "author", "authors", Authors.Select(author => author.Name));
        XmlUtils.AddSimpleNode(xmlData, "packageId", Id);
        XmlUtils.AddSimpleNode(xmlData, "url", Url);
        XmlUtils.AddListNode(xmlData, "supportedVersions", SupportedVersions.Select(version => version.Version));
        XmlUtils.AddListVersionNode(xmlData, "description", "descriptionsByVersion",
            node => node.Version, node => node.Description, Descriptions);

        XmlUtils.AddSimpleNode(xmlData, "modIconPath", ModIconPath);

        XmlUtils.AddByVersionNode(xmlData, "modDependencies", ModDependencies, node => new
        {
            packageId = node.Id,
            displayName = node.DisplayName,
            steamWorkshopUrl = node.SteamWorkshopUrl,
            Version = node.Version
        });

        XmlUtils.AddByVersionNode(xmlData, "loadBefore", LoadBefore, node => new
        {
            li = node.Id,
            Version = node.Version
        });

        XmlUtils.AddByVersionNode(xmlData, "loadAfter", LoadAfter, node => new
        {
            li = node.Id,
            Version = node.Version
        });

        XmlUtils.AddByVersionNode(xmlData, "incompatibleWith", IncompatibleMods, node => new
        {
            li = node.Id,
            Version = node.Version
        });

        XmlUtils.AddListNode(xmlData, "forceLoadBefore", LoadBefore?.Where(l => l.Forced).ToList());
        XmlUtils.AddListNode(xmlData, "forceLoadAfter", LoadAfter?.Where(l => l.Forced).ToList());

        // Only save if the xml has changed
        if (File.Exists(xmlPath))
        {
            var existingXmlDoc = new XmlDocument();
            try
            {
                existingXmlDoc.Load(xmlPath);

                if (XmlUtils.AreXmlDocumentsEqual(xmlDoc, existingXmlDoc))
                    return (false, "Same XML content (No changes)");
            }
            catch (XmlException ex)
            {
                return (false, $"Error loading existing XML: {ex.Message}");
            }
        }

        xmlDoc.Save(xmlPath);
        return (true, $"Saved successfully to {xmlPath}");
    }

    public event PropertyChangedEventHandler PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}