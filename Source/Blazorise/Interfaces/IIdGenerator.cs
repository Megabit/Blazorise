namespace Blazorise
{
    /// <summary>
    /// An interface implemented by Id generators.
    /// </summary>
    public interface IIdGenerator
    {
        /// <summary>
        /// Gets the newly generated and globally unique value.
        /// </summary>
        string Generate { get; }
    }
}
