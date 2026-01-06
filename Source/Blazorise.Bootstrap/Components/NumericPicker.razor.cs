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

        base.DirtyClasses();
    }

    private void BuildNumericWrapperClasses( ClassBuilder builder )
    {
        builder.Append( "b-numeric" );
        builder.Append( ClassProvider.NumericPickerValidation( ParentValidation?.Status ?? ValidationStatus.None ) );

        if ( numericWrapperWidth != null )
        {
            builder.Append( numericWrapperWidth.Class( ClassProvider ) );
        }
    }

    #endregion

    #region Properties

    protected string WrapperElementId { get; set; }

    protected ClassBuilder NumericWrapperClassBuilder { get; private set; }

    /// <summary>
    /// Gets numeric container class-names.
    /// </summary>
    protected string NumericWrapperClassNames => NumericWrapperClassBuilder.Class;

    #endregion
}