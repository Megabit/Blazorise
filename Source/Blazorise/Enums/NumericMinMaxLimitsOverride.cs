namespace Blazorise
{
    /// <summary>
    /// Override the minimum and maximum limits.
    /// </summary>
    public enum NumericMinMaxLimitsOverride
    {
        /// <summary>
        /// Ignores both minimumValue &amp; maximumValue.
        /// </summary>
        Ignore,

        /// <summary>
        /// The minimumValue and maximumValue limits are respected.
        /// </summary>
        DoNotOverride,

        /// <summary>
        /// Adheres to maximumValue and ignores minimumValue settings.
        /// </summary>
        Ceiling,

        /// <summary>
        /// Adheres to minimumValue and ignores maximumValue settings.
        /// </summary>
        Floor,
    }
}
