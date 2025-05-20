using System.Windows;
using System.Windows.Controls;

namespace RimWorld_Mod_Structure_Builder.EditWindows;

/// <summary>
/// Utility class for edit windows
/// </summary>
public static class EditWindowUtils
{
    /// <summary>
    /// Adds a form field to a grid.
    /// </summary>
    /// <param name="parent">The parent <see cref="Grid"/>.</param>
    /// <param name="row">The row index to add the field to.</param>
    /// <param name="labelText">The text to display in the label.</param>
    /// <param name="bindingPath">The path to bind the text box to.</param>
    /// <remarks>
    /// The text box will have its <see cref="TextBox.TextProperty"/> bound to <paramref name="bindingPath"/>,
    /// and will have its <see cref="System.Windows.Data.Binding.UpdateSourceTrigger"/> set to <see cref="System.Windows.Data.UpdateSourceTrigger.PropertyChanged"/>.
    /// </remarks>
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