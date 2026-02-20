#region Using directives
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Bootstrap.Components;

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
        builder.Append( "b-numeric" );
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

    private void BuildButtonsClasses( ClassBuilder builder )
    {
        builder.Append( "b-numeric-handler-wrap" );
    }

    private void BuildButtonUpClasses( ClassBuilder builder )
    {
        builder.Append( "b-numeric-handler" );
        builder.Append( "b-numeric-handler-up" );
    }

    private void BuildButtonDownClasses( ClassBuilder builder )
    {
        builder.Append( "b-numeric-handler" );
        builder.Append( "b-numeric-handler-down" );
    }

    #endregion

    #region Properties

    protected string WrapperElementId { get; set; }

    protected ClassBuilder NumericWrapperClassBuilder { get; private set; }

    protected StyleBuilder NumericWrapperStyleBuilder { get; private set; }

    protected ClassBuilder ButtonsClassBuilder { get; private set; }

    protected ClassBuilder ButtonUpClassBuilder { get; private set; }

    protected ClassBuilder ButtonDownClassBuilder { get; private set; }

    /// <summary>
    /// Gets numeric container class-names.
    /// </summary>
    protected string NumericWrapperClassNames => NumericWrapperClassBuilder.Class;

    protected string NumericWrapperStyleNames => NumericWrapperStyleBuilder.Styles;

    protected string ButtonsClassNames => ButtonsClassBuilder.Class;

    protected string ButtonUpClassNames => ButtonUpClassBuilder.Class;

    protected string ButtonDownClassNames => ButtonDownClassBuilder.Class;

    #endregion
}