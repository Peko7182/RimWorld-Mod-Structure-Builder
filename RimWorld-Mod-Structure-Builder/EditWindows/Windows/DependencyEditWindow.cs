using System.Windows;
using System.Windows.Controls;
using RimWorld_Mod_Structure_Builder.InfoClasses;

namespace RimWorld_Mod_Structure_Builder.EditWindows.Windows;

public class DependencyEditWindow : Window
    {
        public DependencyInfo Dependency { get; private set; }
        
        public DependencyEditWindow(DependencyInfo dependency)
        {
            Dependency = dependency;
            InitializeComponent();
            DataContext = Dependency;
        }
        
        private void InitializeComponent()
        {
            Title = "Edit Dependency";
            Width = 500;
            Height = 300;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            
            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            
            // Create form fields
            EditWindowUtils.AddFormField(grid, 0, "ID:", "Id");
            EditWindowUtils.AddFormField(grid, 1, "Display Name:", "DisplayName");
            EditWindowUtils.AddFormField(grid, 2, "Version (leave empty for any):", "Version");
            EditWindowUtils.AddFormField(grid, 3, "Steam Workshop URL:", "SteamWorkshopUrl");
            
            // Buttons
            var buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 15, 10, 10)
            };
            
            var saveButton = new Button
            {
                Content = "Save",
                Width = 80,
                Height = 30,
                Margin = new Thickness(0, 0, 10, 0),
                IsDefault = true
            };
            saveButton.Click += SaveButton_Click;
            
            var cancelButton = new Button
            {
                Content = "Cancel",
                Width = 80,
                Height = 30,
                IsCancel = true
            };
            
            buttonPanel.Children.Add(saveButton);
            buttonPanel.Children.Add(cancelButton);
            
            Grid.SetRow(buttonPanel, 4);
            grid.Children.Add(buttonPanel);
            
            Content = grid;
        }
        
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Dependency.Id))
            {
                MessageBox.Show("Mod ID cannot be empty", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            DialogResult = true;
            Close();
        }
    }