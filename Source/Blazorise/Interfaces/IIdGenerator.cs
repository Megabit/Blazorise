namespace Blazorise
{
    /// <summary>
    /// An interface implemented by Id generators.
    /// </summary>
    public interface IIdGenerator
    {
        string Generate { get; }
    }
}
