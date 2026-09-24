#region Using directives
using Blazorise.Utilities;
#endregion

namespace Blazorise.AntDesign.Components;

public partial class NumericInput<TValue>
{
    private ClassBuilder inputClassBuilder;

    protected override void OnInitialized()
    {
        inputClassBuilder = new( BuildInputClasses );

        base.OnInitialized();
    }

    protected internal override void DirtyClasses()
    {
        inputClassBuilder?.Dirty();

        base.DirtyClasses();
    }

    private void BuildInputClasses( ClassBuilder builder )
    {
        builder.Append( "ant-input-number-input" );
        builder.Append( ClassProvider.NumericInputColor( Color ) );
    }

    private string ContainerClassNames => IsDisabled
        ? $"{ClassNames} ant-input-number-disabled"
        : ClassNames;

    private string InputClassNames => inputClassBuilder.Class;
}