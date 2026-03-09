#region Using directives
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Material.Components;

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
        // Width should be set on the wrapper element so we need to extract it from the parameters
        if ( parameters.TryGetValue<IFluentSizing>( nameof( Width ), out IFluentSizing paramWidth ) )
        {
            numericWrapperWidth = paramWidth;

            System.Collections.Generic.Dictionary<string, object> readOnlyDictionary = parameters.ToDictionary()
                .Where( x => x.Key != nameof( Width ) )
                .ToDictionary( x => x.Key, x => x.Value );

            return base.SetParametersAsync( ParameterView.FromDictionary( readOnlyDictionary ) );
        }

        numericWrapperWidth = null;

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
        builder.Append( "mui-numeric" );
        builder.Append( ThemeSize != Blazorise.Size.Default ? $"mui-numeric-{ClassProvider.ToSize( ThemeSize )}" : null );
        builder.Append( ClassProvider.NumericPickerValidation( ParentValidation?.Status ?? ValidationStatus.None ) );
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

    private static void BuildButtonsClasses( ClassBuilder builder )
    {
        builder.Append( "mui-numeric-buttons" );
    }

    private static void BuildButtonUpClasses( ClassBuilder builder )
    {
        builder.Append( "mui-numeric-button" );
        builder.Append( "mui-numeric-button-up" );
    }

    private static void BuildButtonDownClasses( ClassBuilder builder )
    {
        builder.Append( "mui-numeric-button" );
        builder.Append( "mui-numeric-button-down" );
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

    #endregion
}