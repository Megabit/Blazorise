#region Using directives
using System;
#endregion

namespace Blazorise;

/// <summary>
/// Represents a command for a choice message, containing properties for a key, display text, icon, icon color, and overall color.
/// </summary>
public class MessageOptionsChoice
{
    /// <summary>
    /// A default constructor for the <see cref="MessageOptionsChoice"/> class.
    /// </summary>
    public MessageOptionsChoice()
    {
        Key = Guid.NewGuid();
    }

    /// <summary>
    /// Constructs a new instance with specified text and color for a message option.
    /// </summary>
    /// <param name="text">The content that will be displayed as the message option.</param>
    /// <param name="color">The color that will be used to represent the message option visually.</param>
    public MessageOptionsChoice( string text, Color color )
        : this()
    {
        Text = text;
        Color = color;
    }

    /// <summary>
    /// Constructs a new instance with specified text and color for a message option.
    /// </summary>
    /// <param name="key">The unique key for the result of the choice.</param>
    /// <param name="text">The content that will be displayed as the message option.</param>
    /// <param name="color">The color that will be used to represent the message option visually.</param>
    public MessageOptionsChoice( object key, string text, Color color )
    {
        Key = key;
        Text = text;
        Color = color;
    }

    /// <summary>
    /// Represents the key associated with a data entry. It can hold any type of object.
    /// </summary>
    public object Key { get; set; }

    /// <summary>
    /// Custom text for the choice button.
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    /// Custom icon for the choice button.
    /// </summary>
    public object Icon { get; set; }

    /// <summary>
    /// Custome icon color for the choice button.
    /// </summary>
    public TextColor IconColor { get; set; }

    /// <summary>
    /// Custom color of the choice button.
    /// </summary>
    public Color Color { get; set; }

    /// <summary>
    /// Custom padding of the choice button.
    /// </summary>
    public IFluentSpacing Padding { get; set; }

    /// <summary>
    /// Represents the name of the CSS class as a string.
    /// </summary>
    public string Class { get; set; }
}