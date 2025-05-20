using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Win32;
using RimWorld_Mod_Structure_Builder.EditWindows;
using RimWorld_Mod_Structure_Builder.InfoClasses;

namespace RimWorld_Mod_Structure_Builder;

public partial class ModEditWindow : Window
{
    public ModInfo SelectedMod { get; set; }
    public MainWindow MainWindow { get; set; }
    
    public ModEditWindow(MainWindow parent, ModInfo item)
    {
        DataContext = item;
        SelectedMod = item;

        Owner = parent;
        MainWindow = parent;
        
        InitializeComponent();
    }
    
    private void ModEditWindow_OnClosing(object sender, CancelEventArgs e)
    {
        this.MainWindow.UpdateList(sender, null);
    }
    
    private void AddAuthor_Click(object sender, RoutedEventArgs e)
    {
        SelectedMod.Authors.Add(new AuthorInfo("CHANGE ME"));
    }

    private void RemoveAuthor_Click(object sender, RoutedEventArgs e)
    {
        var button = (Button)sender;
        var author = (AuthorInfo)button.Tag;
        SelectedMod.Authors.Remove(author);
    }

    private void AddVersion_Click(object sender, RoutedEventArgs e)
    {
        SelectedMod.SupportedVersions.Add(new SupportedVersionInfo("CHANGE ME"));
    }
    
    private void RemoveVersion_Click(object sender, RoutedEventArgs e)
    {
        var button = (Button)sender;
        var version = (SupportedVersionInfo)button.Tag;
        SelectedMod.SupportedVersions.Remove(version);
    }
    
    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }
    
    // Description Methods
    private void EditDescription_Click(object sender, RoutedEventArgs e)
    {
        var button = (Button)sender;
        var description = (DescriptionInfo)button.Tag;
        
        var editWindow = new DescriptionEditWindow(description);
        if (editWindow.ShowDialog() == true)
        {
            // The binding should update automatically if using ObservableCollection
        }
    }
    
    private void AddDescription_Click(object sender, RoutedEventArgs e)
    {
        var newDescription = new DescriptionInfo
        {
            Version = "", // Null string means any version
            Description = "Enter description here..."
        };
        
        var editWindow = new DescriptionEditWindow(newDescription);
        if (editWindow.ShowDialog() == true)
        {
            SelectedMod.Descriptions.Add(newDescription);
        }
    }
    
    private void RemoveDescription_Click(object sender, RoutedEventArgs e)
    {
        var selectedDescription = ((ListView)sender).SelectedItem as DescriptionInfo ?? default;
        
        var result = MessageBox.Show($"Are you sure you want to remove this description?", 
            "Confirm Removal", MessageBoxButton.YesNo, MessageBoxImage.Question);
            
        if (result == MessageBoxResult.Yes)
        {
            SelectedMod.Descriptions.Remove(selectedDescription);
        }
    }
    
    // Dependency Methods
    private void EditDependency_Click(object sender, RoutedEventArgs e)
    {
        var button = (Button)sender;
        var dependency = (DependencyInfo)button.Tag;
        
        var editWindow = new DependencyEditWindow(dependency);
        if (editWindow.ShowDialog() == true)
        {
            // The binding should update automatically if using ObservableCollection
        }
    }
    
    private void AddDependency_Click(object sender, RoutedEventArgs e)
    {
        var newDependency = new DependencyInfo()
        {
            Id = "mod.id.here",
            DisplayName = "Mod Display Name",
            Version = null, // Null string means any version
            SteamWorkshopUrl = ""
        };
        
        var editWindow = new DependencyEditWindow(newDependency);
        if (editWindow.ShowDialog() == true)
        {
            SelectedMod.ModDependencies.Add(newDependency);
        }
    }
    
    private void RemoveDependency_Click(object sender, RoutedEventArgs e)
    {
        var button = (Button)sender;
        var dependency = (DependencyInfo)button.Tag;
        
        var result = MessageBox.Show($"Are you sure you want to remove dependency '{dependency.DisplayName ?? dependency.Id}'?", 
            "Confirm Removal", MessageBoxButton.YesNo, MessageBoxImage.Question);
            
        if (result == MessageBoxResult.Yes)
        {
            SelectedMod.ModDependencies.Remove(dependency);
        }
    }
    
    // Incompatible Mods Methods
    private void EditIncompatible_Click(object sender, RoutedEventArgs e)
    {
        var button = (Button)sender;
        var incompatible = (IncompatibleInfo)button.Tag;
        
        var editWindow = new IncompatibleModEditWindow(incompatible);
        if (editWindow.ShowDialog() == true)
        {
            // The binding should update automatically if using ObservableCollection
        }
    }
    
    private void AddIncompatible_Click(object sender, RoutedEventArgs e)
    {
        var newIncompatible = new IncompatibleInfo()
        {
            Id = "mod.id.here",
            Version = null // Null string means any version
        };
        
        var editWindow = new IncompatibleModEditWindow(newIncompatible);
        if (editWindow.ShowDialog() == true)
        {
            SelectedMod.IncompatibleMods.Add(newIncompatible);
        }
    }
    
    private void RemoveIncompatible_Click(object sender, RoutedEventArgs e)
    {
        var button = (Button)sender;
        var incompatible = (IncompatibleInfo)button.Tag;
        
        var result = MessageBox.Show($"Are you sure you want to remove incompatible mod '{incompatible.Id}'?", 
            "Confirm Removal", MessageBoxButton.YesNo, MessageBoxImage.Question);
            
        if (result == MessageBoxResult.Yes)
        {
            SelectedMod.IncompatibleMods.Remove(incompatible);
        }
    }
    
    // Load Before Methods
    private void EditLoadBefore_Click(object sender, RoutedEventArgs e)
    {
        var button = (Button)sender;
        var loadBefore = (LoadOrderInfo)button.Tag;
        
        var editWindow = new LoadOrderEditWindow(loadBefore, "Load Before");
        if (editWindow.ShowDialog() == true)
        {
            // The binding should update automatically if using ObservableCollection
        }
    }
    
    private void AddLoadBefore_Click(object sender, RoutedEventArgs e)
    {
        var newLoadBefore = new LoadOrderInfo
        {
            Id = "mod.id.here",
            Version = null, // Null string means any version
            Forced = false
        };
        
        var editWindow = new LoadOrderEditWindow(newLoadBefore, "Load Before");
        if (editWindow.ShowDialog() == true)
        {
            SelectedMod.LoadBefore.Add(newLoadBefore);
        }
    }
    
    private void RemoveLoadBefore_Click(object sender, RoutedEventArgs e)
    {
        var button = (Button)sender;
        var loadBefore = (LoadOrderInfo)button.Tag;
        
        var result = MessageBox.Show($"Are you sure you want to remove load before entry '{loadBefore.Id}'?", 
            "Confirm Removal", MessageBoxButton.YesNo, MessageBoxImage.Question);
            
        if (result == MessageBoxResult.Yes)
        {
            SelectedMod.LoadBefore.Remove(loadBefore);
        }
    }
    
    // Load After Methods
    private void EditLoadAfter_Click(object sender, RoutedEventArgs e)
    {
        var button = (Button)sender;
        var loadAfter = (LoadOrderInfo)button.Tag;
        
        var editWindow = new LoadOrderEditWindow(loadAfter, "Load After");
        if (editWindow.ShowDialog() == true)
        {
            // The binding should update automatically if using ObservableCollection
        }
    }
    
    private void AddLoadAfter_Click(object sender, RoutedEventArgs e)
    {
        var newLoadAfter = new LoadOrderInfo
        {
            Id = "mod.id.here",
            Version = null, // Null string means any version
            Forced = false
        };
        
        var editWindow = new LoadOrderEditWindow(newLoadAfter, "Load After");
        if (editWindow.ShowDialog() == true)
        {
            SelectedMod.LoadAfter.Add(newLoadAfter);
        }
    }
    
    private void RemoveLoadAfter_Click(object sender, RoutedEventArgs e)
    {
        var button = (Button)sender;
        var loadAfter = (LoadOrderInfo)button.Tag;
        
        var result = MessageBox.Show($"Are you sure you want to remove load after entry '{loadAfter.Id}'?", 
            "Confirm Removal", MessageBoxButton.YesNo, MessageBoxImage.Question);
            
        if (result == MessageBoxResult.Yes)
        {
            SelectedMod.LoadAfter.Remove(loadAfter);
        }
    }
    
    private void SaveChanges_Click(object sender, RoutedEventArgs e)
    {
        var xmlPath = SelectedMod.XmlPath;

        if (xmlPath == null || !File.Exists(xmlPath))
        {
            MessageBox.Show("Mod info file not found", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        // Backup
        var backupPath = "";
        if (BackupCheckBox.IsChecked == true)
        {
            var backupFolder = Directory.CreateDirectory(Path.Combine(Path.GetDirectoryName(xmlPath), "RWMSB_BACKUPS"));
            backupPath = Path.Combine(backupFolder.FullName, $"BACKUP_{Path.GetFileNameWithoutExtension(xmlPath)}_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.xml");
            
            File.Copy(xmlPath, backupPath);
        }
            
        var (saved, text) = SelectedMod.SaveToXml(SelectedMod.XmlPath);

        if (saved)
            MessageBox.Show(text, "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        else
        {
            // Remove backup and show error
            if (File.Exists(backupPath))
                File.Delete(backupPath);
            MessageBox.Show(text, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    
    private void ModIcon_Pressed(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Filter = "Image files (*.png;*.jpg;*.jpeg;*.gif;*.bmp)|*.png;*.jpg;*.jpeg;*.gif;*.bmp"
        };

        if (dialog.ShowDialog() == true)
            SelectedMod.ModIconPath = dialog.FileName;
    }
    
    private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
    {
        if (e.Uri != null && e.Uri.IsAbsoluteUri)
        {
            try
            {
                Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
                e.Handled = true;
            }
            catch (Exception ex)
            {
                // Log or handle the exception (e.g., show a message to the user)
                Debug.WriteLine($"Could not open URL: {e.Uri.AbsoluteUri}. Error: {ex.Message}");
            }
        }
    }
}