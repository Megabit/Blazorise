#region Using directives
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.AntDesign.Components;

public partial class NumericPicker<TValue> : Blazorise.NumericPicker<TValue>
{
    #region Members

    private IFluentSizing numericWrapperWidth;

    #endregion

    #region Constructors

    public NumericPicker()
    {
        NumericWrapperClassBuilder = new( BuildNumericWrapperClasses, builder => builder.Append( Classes?.Wrapper ) );
        NumericWrapperStyleBuilder = new( BuildNumericWrapperStyles, builder => builder.Append( Styles?.Wrapper ) );
        ButtonsClassBuilder = new( BuildButtonsClasses, builder => builder.Append( Classes?.Buttons ) );
        ButtonUpClassBuilder = new( BuildButtonUpClasses, builder => builder.Append( Classes?.ButtonUp ) );
        ButtonDownClassBuilder = new( BuildButtonDownClasses, builder => builder.Append( Classes?.ButtonDown ) );
    }

    #endregion

    #region Methods

    public override Task SetParametersAsync( ParameterView parameters )
    {
        if ( parameters.TryGetValue<IFluentSizing>( nameof( Width ), out var paramWidth ) )
        {
            numericWrapperWidth = paramWidth;

            var readOnlyDictionary = parameters.ToDictionary()
                .Where( x => x.Key != nameof( Width ) )
                .ToDictionary( x => x.Key, x => x.Value );

            return base.SetParametersAsync( ParameterView.FromDictionary( readOnlyDictionary ) );
        }
        else
        {
            numericWrapperWidth = null;
        }

        return base.SetParametersAsync( parameters );
    }

    protected internal override void DirtyClasses()
    {
        NumericWrapperClassBuilder.Dirty();
        ButtonsClassBuilder.Dirty();
        ButtonUpClassBuilder.Dirty();
        ButtonDownClassBuilder.Dirty();

        base.DirtyClasses();
    }

    protected internal override void DirtyStyles()
    {
        NumericWrapperStyleBuilder.Dirty();

        base.DirtyStyles();
    }

    private void BuildNumericWrapperClasses( ClassBuilder builder )
    {
        builder.Append( "b-ant-numeric-picker" );
        AppendWrapperUtilities( builder );

        if ( numericWrapperWidth != null )
        {
            builder.Append( numericWrapperWidth.Class( ClassProvider ) );
        }
    }

    private void BuildNumericWrapperStyles( StyleBuilder builder )
    {
        AppendWrapperUtilities( builder );
    }

    private void BuildButtonsClasses( ClassBuilder builder )
    {
        builder.Append( "ant-input-number-actions" );
    }

    private void BuildButtonUpClasses( ClassBuilder builder )
    {
        builder.Append( "ant-input-number-action" );
        builder.Append( "ant-input-number-action-up" );
    }

    private void BuildButtonDownClasses( ClassBuilder builder )
    {
        builder.Append( "ant-input-number-action" );
        builder.Append( "ant-input-number-action-down" );
    }

    #endregion

    #region Properties

    protected ClassBuilder NumericWrapperClassBuilder { get; private set; }

    protected StyleBuilder NumericWrapperStyleBuilder { get; private set; }

    protected ClassBuilder ButtonsClassBuilder { get; private set; }

    protected ClassBuilder ButtonUpClassBuilder { get; private set; }

    protected ClassBuilder ButtonDownClassBuilder { get; private set; }

    protected string NumericWrapperClassNames => NumericWrapperClassBuilder.Class;

    protected string NumericWrapperStyleNames => NumericWrapperStyleBuilder.Styles;

    protected string ButtonsClassNames => ButtonsClassBuilder.Class;

    protected string ButtonUpClassNames => ButtonUpClassBuilder.Class;

    protected string ButtonDownClassNames => ButtonDownClassBuilder.Class;

    protected string ContainerClassNames => IsDisabled
        ? $"{ClassNames} {NumericWrapperClassNames} ant-input-number-mode-input ant-input-number-disabled{( ReadOnly ? " ant-input-number-readonly" : null )}"
        : $"{ClassNames} {NumericWrapperClassNames} ant-input-number-mode-input{( ReadOnly ? " ant-input-number-readonly" : null )}{( !IsShowStepButtons ? " ant-input-number-without-controls" : null )}";

    protected string InputClassNames => string.Join( " ", "ant-input-number-input", ClassProvider.NumericPickerColor( Color ) ).Trim();

    #endregion
}