using System.Threading.Tasks;
using Blazorise.AntDesign.Modules;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;

namespace Blazorise.AntDesign.Components;

public partial class Radio<TValue>
{
    private ElementReference wrapperRef;

    private ClassBuilder wrapperClassBuilder;

    private StyleBuilder wrapperStyleBuilder;

    protected override void OnInitialized()
    {
        wrapperClassBuilder = new( BuildWrapperClasses );
        wrapperStyleBuilder = new( BuildWrapperStyles );

        base.OnInitialized();
    }

    protected override async Task OnAfterRenderAsync( bool firstRender )
    {
        if ( firstRender )
        {
            await JSWaveModule.Initialize( wrapperRef, ".ant-wave-target" );
        }

        await base.OnAfterRenderAsync( firstRender );
    }

    protected override async ValueTask DisposeAsync( bool disposing )
    {
        if ( disposing && Rendered )
        {
            await JSWaveModule.Destroy( wrapperRef );
        }

        await base.DisposeAsync( disposing );
    }

    protected internal override void DirtyClasses()
    {
        wrapperClassBuilder?.Dirty();

        base.DirtyClasses();
    }

    protected internal override void DirtyStyles()
    {
        wrapperStyleBuilder?.Dirty();

        base.DirtyStyles();
    }

    private void BuildWrapperClasses( ClassBuilder builder )
    {
        string baseClasses = AsButton == true
            ? $"ant-segmented-item{( IsActive ? " ant-segmented-item-selected" : "" )}{( IsDisabled ? " ant-segmented-item-disabled" : "" )}"
            : $"ant-radio-wrapper{( IsActive ? " ant-radio-wrapper-checked" : "" )}{( IsDisabled ? " ant-radio-wrapper-disabled" : "" )}";

        builder.Append( baseClasses );

        if ( ThemeSize != Blazorise.Size.Default )
        {
            builder.Append( $"ant-radio-{ClassProvider.ToSize( ThemeSize )}" );
        }

        if ( AsButton )
        {
            builder.Append( ButtonColor?.IsCssValue == true
                ? "ant-segmented-item-custom"
                : $"ant-segmented-item-{ClassProvider.ToColor( ButtonColor )}" );
        }

        builder.Append( Classes?.Wrapper );
        AppendWrapperUtilities( builder );
    }

    private void BuildWrapperStyles( StyleBuilder builder )
    {
        if ( AsButton )
        {
            builder.Append( StyleProvider.ButtonColor( ButtonColor ) );
        }

        AppendWrapperUtilities( builder );
        builder.Append( Styles?.Wrapper );
    }

    private string WrapperClassNames => wrapperClassBuilder.Class;

    private string WrapperStyleNames => wrapperStyleBuilder.Styles;

    private string CheckboxClassNames => AsButton == true
        ? string.Empty
        : $"ant-radio ant-wave-target{( IsActive ? " ant-radio-checked" : "" )}{( IsDisabled ? " ant-radio-disabled" : "" )}";

    private string InnerClassNames => AsButton == true
        ? "ant-radio-button-inner"
        : "ant-radio-inner";

    private string LabelClassNames => AsButton == true
        ? "ant-segmented-item-label"
        : "ant-radio-label";

    [Inject] public AntDesignJSWaveModule JSWaveModule { get; set; }
}