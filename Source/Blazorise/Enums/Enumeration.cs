#region Using directives
using System.Text;
#endregion

namespace Blazorise;

/// <summary>
/// Base class for all complex enums.
/// </summary>
/// <typeparam name="T">Type of the complex enum.</typeparam>
public record Enumeration<T>
    where T : Enumeration<T>
{
    #region Members

    string name;

    string cachedName;

    #endregion

    #region Constructors

    /// <summary>
    /// A default enumeration contructor.
    /// </summary>
    /// <param name="name">Named value of the enum.</param>
    public Enumeration( string name )
    {
        Name = name;
        ParentEnumeration = default;
    }

    /// <summary>
    /// A default enumeration contructor that accepts the parent object.
    /// </summary>
    /// <param name="parentEnumeration">Parent enumeration.</param>
    /// <param name="name">Named value of the enum.</param>
    protected Enumeration( T parentEnumeration, string name )
    {
        this.Name = name;
        this.ParentEnumeration = parentEnumeration;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Build an enumeration while traversing up to the parent.
    /// </summary>
    /// <returns></returns>
    private string BuildName()
    {
        var sb = new StringBuilder();

        if ( ParentEnumeration is not null )
            sb.Append( ParentEnumeration.Name ).Append( ' ' );

        sb.Append( name );

        return sb.ToString();
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the enum name.
    /// </summary>
    public string Name
    {
        get
        {
            if ( cachedName is null )
                cachedName = BuildName();

            return cachedName;
        }
        private set
        {
            if ( name == value )
                return;

            name = value;
            cachedName = null;
        }
    }

    /// <summary>
    /// Gets the parent enumeration.
    /// </summary>
    public T ParentEnumeration { get; private set; }

    #endregion
}