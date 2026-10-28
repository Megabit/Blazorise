#region Using directives
using Blazorise.Utilities;
#endregion

namespace Blazorise.Bootstrap5.Components;

public partial class FileInput
{
    #region Constructors

    public FileInput()
    {
        SelectionClassBuilder = new( BuildSelectionClasses );
    }

    #endregion

    #region Methods

    private void BuildSelectionClasses( ClassBuilder builder )
    {
        builder.Append( ClassNames );
        builder.Append( "form-file-text" );
    }

    /// <inheritdoc />
    protected internal override void DirtyClasses()
    {
        SelectionClassBuilder.Dirty();

        base.DirtyClasses();
    }

    #endregion

    #region Properties

    protected ClassBuilder SelectionClassBuilder { get; private set; }

    protected string SelectionClassNames => SelectionClassBuilder.Class;

    #endregion
}