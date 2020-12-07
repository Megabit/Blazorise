#region Using directives
#endregion

namespace Blazorise
{
    /// <summary>
    /// Lists values that specify the type of mask used by an editor.
    /// </summary>
    public enum MaskType
    {
        /// <summary>
        /// Specifies that the mask feature is disabled.
        /// </summary>
        None,

        /// <summary>
        /// Specifies that the editor should accept numeric values and that the mask string must use the Numeric format syntax.
        /// </summary>
        Numeric = 1,

        /// <summary>
        /// Specifies that the editor should accept date/time values and that the mask string must use the DateTime format syntax.
        /// </summary>
        DateTime = 2,

        /// <summary>
        /// Specifies that the mask should be created using full-functional regular expressions.
        /// </summary>
        RegEx = 3,
    }
}
