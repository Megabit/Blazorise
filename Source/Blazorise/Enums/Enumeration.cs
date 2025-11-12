#region Using directives
using System.Collections.Generic;
using System.Text;
#endregion

namespace Blazorise;

/// <summary>
/// Base class for all complex enums used within Blazorise.
/// <para>
/// This class provides functionality similar to traditional enumerations but allows for
/// hierarchical definitions and dynamic name generation through a pluggable
/// <see cref="IEnumerationNameBuilder{T}"/>.
/// </para>
/// </summary>
/// <typeparam name="T">The type of the derived complex enumeration.</typeparam>
public record Enumeration<T>
    where T : Enumeration<T>
{
    #region Members

    string name;

    string cachedName;

    static int builderVersion;

    int cachedVersion = -1;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="Enumeration{T}"/> class with the specified name.
    /// </summary>
    /// <param name="name">The named value of the enumeration.</param>
    public Enumeration( string name )
    {
        this.name = name;
        ParentEnumeration = default;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Enumeration{T}"/> class with the specified
    /// parent enumeration and name.
    /// </summary>
    /// <param name="parentEnumeration">The parent enumeration instance.</param>
    /// <param name="name">The named value of the enumeration.</param>
    protected Enumeration( T parentEnumeration, string name )
    {
        this.name = name;
        ParentEnumeration = parentEnumeration;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Sets the <see cref="IEnumerationNameBuilder{T}"/> used to construct enumeration names.
    /// <para>
    /// Each builder defines how names are formatted and combined for a given enumeration type.
    /// </para>
    /// </summary>
    /// <param name="builder">
    /// The <see cref="IEnumerationNameBuilder{T}"/> instance to use for building names.
    /// If <c>null</c>, a default <see cref="EnumerationNameBuilder{T}"/> will be used.
    /// </param>
    public static void SetNameBuilder( IEnumerationNameBuilder<T> builder )
    {
        NameBuilder = builder ?? new EnumerationNameBuilder<T>();

        unchecked
        {
            builderVersion++;
        }
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the raw name assigned to this enumeration instance, without any formatting or name building logic applied.
    /// </summary>
    internal string RawName => name;

    /// <summary>
    /// Gets or sets the name builder used to generate the final formatted name of the enumeration.
    /// <para>
    /// The builder can be replaced globally for this enumeration type to apply different
    /// naming conventions (e.g., framework-specific formatting such as Bootstrap class names).
    /// </para>
    /// </summary>
    public static IEnumerationNameBuilder<T> NameBuilder { get; set; }
        = new EnumerationNameBuilder<T>();

    /// <summary>
    /// Gets the fully qualified name of the enumeration as built by the current <see cref="NameBuilder"/>.
    /// <para>
    /// The computed name is cached internally and automatically invalidated whenever the active <see cref="NameBuilder"/> changes.
    /// </para>
    /// </summary>
    public string Name
    {
        get
        {
            if ( cachedName is null || cachedVersion != builderVersion )
            {
                cachedName = NameBuilder.BuildName( this );

                cachedVersion = builderVersion;
            }

            return cachedName;
        }
    }

    /// <summary>
    /// Gets the parent enumeration, if this enumeration represents a nested or derived value.
    /// Returns <c>null</c> for root-level enumerations.
    /// </summary>
    public T ParentEnumeration { get; private set; }

    #endregion
}