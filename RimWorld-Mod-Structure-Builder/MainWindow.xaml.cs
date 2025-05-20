using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using RimWorld_Mod_Structure_Builder.InfoClasses;
using RimWorld_Mod_Structure_Builder.Utils;
using Path = System.IO.Path;

namespace RimWorld_Mod_Structure_Builder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Sample data
        public ObservableCollection<ModInfo> Items { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            this.UpdateList(this, null);
            
            // Save About.XMLs to Desktop (Debug)
            // var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            // foreach (var item in Items)
            // {
            //     item.SaveToXml(Path.Combine(desktopPath, $"{item.Name}.xml"));
            // }
        }
        
        public void UpdateList(object sender, RoutedEventArgs e)
        {
            Items = new ObservableCollection<ModInfo>();
            
            // Get mod path
            var modsPath = FolderUtils.GetRimWorldModFolder();
            var directories = Directory.GetDirectories(modsPath);

            foreach (var directory in directories)
            {
                var aboutPath = ModListUtils.GetAboutPath(directory);
                if (string.IsNullOrEmpty(aboutPath)) continue;
                
                var modInfo = new ModInfo();
                try
                {
                    modInfo.LoadFromXml(aboutPath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to load mod info from {aboutPath}: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    continue;
                }
                
                Items.Add(modInfo);
                // var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(modInfo, Newtonsoft.Json.Formatting.Indented);
                // MessageBox.Show(jsonString, "Mod info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            
            ModList.ItemsSource = Items;
        }

        private void OpenModEditWindow(object sender, RoutedEventArgs e)
        {
            try
            {
                var item = ModList.SelectedItem as ModInfo;
                var window = new ModEditWindow(this, item);
                window.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open mod edit window: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
