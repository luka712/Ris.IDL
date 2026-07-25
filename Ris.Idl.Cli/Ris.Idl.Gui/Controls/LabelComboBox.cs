// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

using Windows.Storage.Pickers;

namespace Ris.Idl.Gui.Controls;

/// <summary>
/// Combo box with label.
/// </summary>
public sealed class LabelComboBox : Control
{
    public LabelComboBox()
    {
        DefaultStyleKey = typeof(LabelComboBox);
    }
    
    /// <summary>
    /// The label.
    /// </summary>
    public string Label
    {
        get => (string)GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    /// <summary>
    /// The label property.
    /// </summary>
    public static readonly DependencyProperty LabelProperty =
        DependencyProperty.Register(
            nameof(Label),
            typeof(string),
            typeof(LabelComboBox),
            new PropertyMetadata(string.Empty));
    
    /// <summary>
    /// The items' source.
    /// </summary>
    public object ItemsSource
    {
        get => GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    /// <summary>
    /// The items source property.
    /// </summary>
    public static readonly DependencyProperty ItemsSourceProperty =
        DependencyProperty.Register(
            nameof(ItemsSource),
            typeof(object),
            typeof(LabelComboBox),
            new PropertyMetadata(null));

    /// <summary>
    /// The selected item.
    /// </summary>
    public object SelectedItem
    {
        get => GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);    
    }
    
    /// <summary>
    /// The selected item property.
    /// </summary>
    public static readonly DependencyProperty SelectedItemProperty =
        DependencyProperty.Register(
            nameof(SelectedItem),
            typeof(object),
            typeof(LabelComboBox),
            new PropertyMetadata(null));
    
   

}