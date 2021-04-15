#region Using directives
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Threading.Tasks;
#endregion

namespace Blazorise.Material
{
    public partial class MaterialJSRunner : Blazorise.JSRunner
    {
        public MaterialJSRunner( IJSRuntime runtime )
               : base( runtime )
        {
        }

        public override ValueTask<bool> InitializeTooltip( ElementReference elementRef, string elementId, object options )
        {
            return runtime.InvokeAsync<bool>( $"blazoriseMaterial.tooltip.initialize", elementRef, elementId, options );
        }

        public override ValueTask InitializeDatePicker( ElementReference elementRef, string elementId, object options )
        {
            return runtime.InvokeVoidAsync( $"blazoriseMaterial.dateEdit.initialize", elementRef, elementId, options );
        }

        public override ValueTask DestroyDatePicker( ElementReference elementRef, string elementId )
        {
            return runtime.InvokeVoidAsync( $"blazoriseMaterial.dateEdit.destroy", elementRef, elementId );
        }

        public override ValueTask UpdateDatePickerValue( ElementReference elementRef, string elementId, object value )
        {
            return runtime.InvokeVoidAsync( $"blazoriseMaterial.dateEdit.update", elementRef, elementId, value );
        }

        public override ValueTask UpdateDatePickerOptions( ElementReference elementRef, string elementId, object options )
        {
            return runtime.InvokeVoidAsync( $"blazoriseMaterial.dateEdit.updateOptions", elementRef, elementId, options );
        }

        public override ValueTask InitializeTimePicker( ElementReference elementRef, string elementId, object options )
        {
            return runtime.InvokeVoidAsync( $"blazoriseMaterial.timeEdit.initialize", elementRef, elementId, options );
        }

        public override ValueTask DestroyTimePicker( ElementReference elementRef, string elementId )
        {
            return runtime.InvokeVoidAsync( $"blazoriseMaterial.timeEdit.destroy", elementRef, elementId );
        }

        public override ValueTask UpdateTimePickerValue( ElementReference elementRef, string elementId, object value )
        {
            return runtime.InvokeVoidAsync( $"blazoriseMaterial.timeEdit.updateValue", elementRef, elementId, value );
        }

        public override ValueTask UpdateTimePickerOptions( ElementReference elementRef, string elementId, object options )
        {
            return runtime.InvokeVoidAsync( $"blazoriseMaterial.timeEdit.updateOptions", elementRef, elementId, options );
        }

        public override ValueTask<bool> OpenModal( ElementReference elementRef, bool scrollToTop )
        {
            return runtime.InvokeAsync<bool>( $"blazoriseMaterial.modal.open", elementRef, scrollToTop );
        }

        public override ValueTask<bool> CloseModal( ElementReference elementRef )
        {
            return runtime.InvokeAsync<bool>( $"blazoriseMaterial.modal.close", elementRef );
        }
    }
}
