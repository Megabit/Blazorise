#region Using directives
using Blazorise.Utilities;
#endregion

namespace Blazorise.Material.Components;

public partial class FileInput
{
    #region Constructors

    public FileInput()
    {
        ContentClassBuilder = new( BuildContentClasses );
        BrowseButtonClassBuilder = new( BuildBrowseButtonClasses );
        SelectionClassBuilder = new( BuildSelectionClasses );
    }

    #endregion

    #region Methods

    /// <inheritdoc />
    protected override void BuildWrapperClasses( ClassBuilder builder )
    {
        builder.Append( "mui-file" );

        base.BuildWrapperClasses( builder );
    }

    private void BuildContentClasses( ClassBuilder builder )
    {
        builder.Append( ClassNames );
        builder.Append( "mui-file-content" );
    }

    private void BuildBrowseButtonClasses( ClassBuilder builder ) => builder.Append( "mui-file-button" );

    private void BuildSelectionClasses( ClassBuilder builder ) => builder.Append( "mui-file-selection" );

    /// <inheritdoc />
    protected internal override void DirtyClasses()
    {
        ContentClassBuilder.Dirty();
        BrowseButtonClassBuilder.Dirty();
        SelectionClassBuilder.Dirty();

        base.DirtyClasses();
    }

    #endregion

    #region Properties

    protected ClassBuilder ContentClassBuilder { get; private set; }

    protected ClassBuilder BrowseButtonClassBuilder { get; private set; }

    protected ClassBuilder SelectionClassBuilder { get; private set; }

    protected string ContentClassNames => ContentClassBuilder.Class;

    protected string BrowseButtonClassNames => BrowseButtonClassBuilder.Class;

    protected string SelectionClassNames => SelectionClassBuilder.Class;

    #endregion
}