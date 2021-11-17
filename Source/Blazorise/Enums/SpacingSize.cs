namespace Blazorise
{
    /// <summary>
    /// Defines all supported spacing(margin and padding) sizes.
    /// </summary>
    public enum SpacingSize
    {
        /// <summary>
        /// For classes that eliminate the margin or padding by setting it to 0.
        /// </summary>
        Is0,

        /// <summary>
        /// (by default) for classes that set the margin or padding to $spacer * .25
        /// </summary>
        Is1,

        /// <summary>
        /// (by default) for classes that set the margin or padding to $spacer * .5
        /// </summary>
        Is2,

        /// <summary>
        /// (by default) for classes that set the margin or padding to $spacer
        /// </summary>
        Is3,

        /// <summary>
        /// (by default) for classes that set the margin or padding to $spacer * 1.5
        /// </summary>
        Is4,

        /// <summary>
        /// (by default) for classes that set the margin or padding to $spacer * 3
        /// </summary>
        Is5,

        /// <summary>
        /// For classes that set the margin to auto.
        /// </summary>
        IsAuto,
    }
}
