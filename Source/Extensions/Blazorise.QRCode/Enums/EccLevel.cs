namespace Blazorise.QRCode
{
    /// <summary>
    /// Error Correction Level
    /// </summary>
    public enum EccLevel
    {
        /// <summary>
        /// Level L (Low): 7% of data bytes can be restored.
        /// </summary>
        L,

        /// <summary>
        /// Level M (Medium): 15% of data bytes can be restored.
        /// </summary>
        M,

        /// <summary>
        /// Level Q (Quartile): 25% of data bytes can be restored.
        /// </summary>
        Q,

        /// <summary>
        /// Level H (High): 30% of data bytes can be restored.
        /// </summary>
        H
    }
}
