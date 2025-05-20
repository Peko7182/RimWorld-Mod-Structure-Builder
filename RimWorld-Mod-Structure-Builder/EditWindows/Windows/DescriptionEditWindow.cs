using System.Windows;
using System.Windows.Controls;
using RimWorld_Mod_Structure_Builder.InfoClasses;

namespace RimWorld_Mod_Structure_Builder.EditWindows.Windows;

public class DescriptionEditWindow : Window
    {
        public DescriptionInfo Description { get; private set; }
        
        public DescriptionEditWindow(DescriptionInfo description)
        {
            Description = description;
            InitializeComponent();
            DataContext = Description;
        }
        
        private void InitializeComponent()
        {
            Title = "Edit Description";
            Width = 500;
            Height = 350;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            
            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0, GridUnitType.Auto) });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0, GridUnitType.Auto) });
            
            // Version input
            var versionPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(10) };
            versionPanel.Children.Add(new Label { Content = "Version:", Width = 80 });
            var versionTextBox = new TextBox { Width = 200, TextWrapping = TextWrapping.Wrap };
            versionTextBox.SetBinding(TextBox.TextProperty, new System.Windows.Data.Binding("Version") 
            { 
                UpdateSourceTrigger = System.Windows.Data.UpdateSourceTrigger.PropertyChanged 
            });
            versionPanel.Children.Add(versionTextBox);
            
            // Description input
            var descriptionLabel = new Label { Content = "Description:", Margin = new Thickness(10, 5, 10, 0) };
            var descriptionTextBox = new TextBox
            {
                AcceptsReturn = true,
                TextWrapping = TextWrapping.Wrap,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                Margin = new Thickness(10, 0, 10, 10)
            };
            descriptionTextBox.SetBinding(TextBox.TextProperty, new System.Windows.Data.Binding("Description")
            {
                UpdateSourceTrigger = System.Windows.Data.UpdateSourceTrigger.PropertyChanged
            });
            
            // Buttons
            var buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 0, 10, 10)
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
            
            Grid.SetRow(versionPanel, 0);
            Grid.SetRow(descriptionLabel, 1);
            Grid.SetRow(descriptionTextBox, 1);
            Grid.SetRow(buttonPanel, 2);
            
            grid.Children.Add(versionPanel);
            grid.Children.Add(descriptionLabel);
            grid.Children.Add(descriptionTextBox);
            grid.Children.Add(buttonPanel);
            
            Content = grid;
        }
        
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }