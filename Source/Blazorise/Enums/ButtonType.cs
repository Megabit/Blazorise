#region Using directives
#endregion

namespace Blazorise
{
    /// <summary>
    /// Defines the button type and behaviour.
    /// </summary>
    public enum ButtonType
    {
        /// <summary>
        /// The button is a clickable button.
        /// </summary>
        Button,

        /// <summary>
        /// The button is a submit button (submits form-data).
        /// </summary>
        Submit,

        /// <summary>
        /// The button is a reset button (resets the form-data to its initial values).
        /// </summary>
        Reset,

        /// <summary>
        /// The button will be rendered as a link but will appear as a regular button.
        /// </summary>
        Link,
    }
}
