using System.Windows;
using System.Windows.Controls;
using RimWorld_Mod_Structure_Builder.InfoClasses;

namespace RimWorld_Mod_Structure_Builder.EditWindows.Windows;

public class LoadOrderEditWindow : Window
    {
        public LoadOrderInfo LoadOrder { get; private set; }
        
        public LoadOrderEditWindow(LoadOrderInfo loadOrder, string orderType)
        {
            LoadOrder = loadOrder;
            InitializeComponent(orderType);
            DataContext = LoadOrder;
        }
        
        private void InitializeComponent(string orderType)
        {
            Title = $"Edit {orderType} Entry";
            Width = 450;
            Height = 230;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            
            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            
            // Create form fields
            EditWindowUtils.AddFormField(grid, 0, "Mod ID:", "Id");
            EditWindowUtils.AddFormField(grid, 1, "Version (leave empty for any):", "Version");
            
            // Forced checkbox
            var forcedPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(10, 10, 10, 0) };
            var forcedCheckbox = new CheckBox { Content = "Forced (hard requirement)" };
            forcedCheckbox.SetBinding(CheckBox.IsCheckedProperty, new System.Windows.Data.Binding("Forced")
            {
                UpdateSourceTrigger = System.Windows.Data.UpdateSourceTrigger.PropertyChanged
            });
            forcedPanel.Children.Add(forcedCheckbox);
            
            Grid.SetRow(forcedPanel, 2);
            grid.Children.Add(forcedPanel);
            
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
            
            Grid.SetRow(buttonPanel, 3);
            grid.Children.Add(buttonPanel);
            
            Content = grid;
        }
        
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(LoadOrder.Id))
            {
                MessageBox.Show("Mod ID cannot be empty", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            DialogResult = true;
            Close();
        }
    }