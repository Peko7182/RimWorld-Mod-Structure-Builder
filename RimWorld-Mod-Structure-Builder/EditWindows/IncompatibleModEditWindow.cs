using System.Windows;
using System.Windows.Controls;
using RimWorld_Mod_Structure_Builder.InfoClasses;

namespace RimWorld_Mod_Structure_Builder.EditWindows;

public class IncompatibleModEditWindow : Window
    {
        public IncompatibleInfo Incompatibility { get; private set; }
        
        public IncompatibleModEditWindow(IncompatibleInfo incompatibility)
        {
            Incompatibility = incompatibility;
            InitializeComponent();
            DataContext = Incompatibility;
        }
        
        private void InitializeComponent()
        {
            Title = "Edit Incompatible Mod";
            Width = 450;
            Height = 200;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            
            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            
            // Create form fields
            EditWindowUtils.AddFormField(grid, 0, "Mod ID:", "Id");
            EditWindowUtils.AddFormField(grid, 1, "Version (leave empty for any):", "Version");
            
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
            
            Grid.SetRow(buttonPanel, 2);
            grid.Children.Add(buttonPanel);
            
            Content = grid;
        }
        
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Incompatibility.Id))
            {
                MessageBox.Show("Mod ID cannot be empty", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            DialogResult = true;
            Close();
        }
    }