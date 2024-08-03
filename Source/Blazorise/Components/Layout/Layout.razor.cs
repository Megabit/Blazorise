#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Main component for handling the overall layout of a page.
/// </summary>
public partial class Layout : BaseComponent
{
    #region Members

    private bool sider;

    private bool loading;

    private string loadingClass;

    #endregion

    #region Constructors

    /// <summary>
    /// A default constructor for <see cref="Layout"/> component.
    /// </summary>
    public Layout()
    {
        LoadingClassBuilder = new( BuildLoadingClasses );
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.Layout() );
        builder.Append( ClassProvider.LayoutHasSider( Sider ) );
        builder.Append( ClassProvider.LayoutRoot( ParentLayout is null ) );

        base.BuildClasses( builder );
    }

    /// <summary>
    /// Builds the classnames for a loading container.
    /// </summary>
    /// <param name="builder">Class builder used to append the classnames.</param>
    protected void BuildLoadingClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.LayoutLoading(), string.IsNullOrEmpty( LoadingClass ) );
        builder.Append( LoadingClass );
    }

    /// <inheritdoc/>
    internal protected override void DirtyClasses()
    {
        LoadingClassBuilder.Dirty();

        base.DirtyClasses();
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the class-names for a loading container.
    /// </summary>
    protected string LoadingClassNames => LoadingClassBuilder.Class;

    /// <summary>
    /// Loading container class builder.
    /// </summary>
    protected ClassBuilder LoadingClassBuilder { get; private set; }

    /// <summary>
    /// Indicates that layout will contain sider container.
    /// </summary>
    [Parameter]
    public bool Sider
    {
        get => sider;
        set
        {
            sider = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// If true, an overlay will be created so the user cannot click anything until set to false.
    /// </summary>
    [Parameter]
    public bool Loading
    {
        get => loading;
        set
        {
            loading = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Sets the custom classname for loading element.
    /// </summary>
    [Parameter]
    public string LoadingClass
    {
        get => loadingClass;
        set
        {
            loadingClass = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Occurs when loading state had changed.
    /// </summary>
    [Parameter] public EventCallback<bool> LoadingChanged { get; set; }

    /// <summary>
    /// Specifies the content to be rendered the loading container.
    /// </summary>
    [Parameter] public RenderFragment LoadingTemplate { get; set; }

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="Layout"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Cascaded <see cref="Layout"/> component in which this <see cref="Layout"/> is placed.
    /// </summary>
    [CascadingParameter] protected Layout ParentLayout { get; set; }

    #endregion
}