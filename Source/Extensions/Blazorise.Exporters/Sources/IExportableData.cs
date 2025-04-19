namespace Blazorise.Exporters;


/// <summary>
/// where T is eirhter string for text data, or object for binary data
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IExportableData<T> { }


/// <summary>
/// Represents a component that can utilize the JSExportersModule.
/// </summary>
public interface IExportableComponent
{
    /// <summary>
    /// Gets or sets the JSExportersModule instance.
    /// </summary>
    JSExportersModule JSExportersModule { get;  set; }
}