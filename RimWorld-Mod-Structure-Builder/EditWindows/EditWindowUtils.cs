using System.Windows;
using System.Windows.Controls;

namespace RimWorld_Mod_Structure_Builder.EditWindows;

public static class EditWindowUtils
{
    public static void AddFormField(Grid parent, int row, string labelText, string bindingPath)
    {
        var panel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(10, 10, 10, 0) };
        panel.Children.Add(new Label { Content = labelText, Width = 200 });
        var textBox = new TextBox { Width = 250, TextWrapping = TextWrapping.Wrap };
        textBox.SetBinding(TextBox.TextProperty, new System.Windows.Data.Binding(bindingPath)
        {
            UpdateSourceTrigger = System.Windows.Data.UpdateSourceTrigger.PropertyChanged
        });
        panel.Children.Add(textBox);
            
        Grid.SetRow(panel, row);
        parent.Children.Add(panel);
    }
}