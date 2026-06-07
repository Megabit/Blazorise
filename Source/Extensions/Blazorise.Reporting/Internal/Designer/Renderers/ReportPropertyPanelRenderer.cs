#region Using directives
using System;
using System.Globalization;
using System.Threading.Tasks;
using Blazorise;
using Blazorise.Extensions;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise.Reporting.Internal;

internal static class ReportPropertyPanelRenderer
{
    #region Methods

    internal static void RenderGroup( RenderTreeBuilder builder, string title, RenderFragment childContent )
    {
        builder.OpenComponent<Div>();
        builder.Attribute( "Margin", Margin.Is3.FromBottom );
        builder.Attribute( "ChildContent", (RenderFragment)( groupBuilder =>
        {
            groupBuilder.OpenElement( "h6" );
            groupBuilder.Content( title );
            groupBuilder.CloseElement();

            groupBuilder.Content( childContent );
        } ) );
        builder.CloseComponent();
    }

    internal static void RenderInput( RenderTreeBuilder builder, object eventReceiver, string label, string value, Func<string, Task> valueChanged, bool readOnly = false )
    {
        builder.OpenComponent<Field>();
        builder.Attribute( "Margin", Margin.Is2.FromBottom );
        builder.Attribute( "ChildContent", (RenderFragment)( fieldBuilder =>
        {
            fieldBuilder.OpenComponent<FieldLabel>();
            fieldBuilder.Attribute( "ChildContent", (RenderFragment)( labelBuilder => labelBuilder.Content( label ) ) );
            fieldBuilder.CloseComponent();

            fieldBuilder.OpenComponent<TextInput>();
            fieldBuilder.Attribute( "Value", value );
            fieldBuilder.Attribute( "ReadOnly", readOnly );

            if ( valueChanged is not null )
            {
                fieldBuilder.Attribute( "ValueChanged", EventCallback.Factory.Create<string>( eventReceiver, valueChanged ) );
                fieldBuilder.Attribute( "Immediate", true );
            }

            fieldBuilder.CloseComponent();
        } ) );
        builder.CloseComponent();
    }

    internal static void RenderNullableNumberInput( RenderTreeBuilder builder, object eventReceiver, string label, double? value, Func<double?, Task> valueChanged )
    {
        builder.OpenComponent<Field>();
        builder.Attribute( "Margin", Margin.Is2.FromBottom );
        builder.Attribute( "ChildContent", (RenderFragment)( fieldBuilder =>
        {
            fieldBuilder.OpenComponent<FieldLabel>();
            fieldBuilder.Attribute( "ChildContent", (RenderFragment)( labelBuilder => labelBuilder.Content( label ) ) );
            fieldBuilder.CloseComponent();

            fieldBuilder.OpenComponent<NumericInput<double?>>();
            fieldBuilder.Attribute( "Value", value );
            fieldBuilder.Attribute( "ValueChanged", EventCallback.Factory.Create<double?>( eventReceiver, valueChanged ) );
            fieldBuilder.Attribute( "Immediate", true );
            fieldBuilder.Attribute( "Step", 1m );
            fieldBuilder.CloseComponent();
        } ) );
        builder.CloseComponent();
    }

    internal static void RenderColorInput( RenderTreeBuilder builder, object eventReceiver, string label, string value, Func<string, Task> valueChanged )
    {
        builder.OpenComponent<Field>();
        builder.Attribute( "Margin", Margin.Is2.FromBottom );
        builder.Attribute( "ChildContent", (RenderFragment)( fieldBuilder =>
        {
            fieldBuilder.OpenComponent<FieldLabel>();
            fieldBuilder.Attribute( "ChildContent", (RenderFragment)( labelBuilder => labelBuilder.Content( label ) ) );
            fieldBuilder.CloseComponent();

            fieldBuilder.OpenComponent<Div>();
            fieldBuilder.Attribute( "Flex", Flex.Row );
            fieldBuilder.Attribute( "Gap", Gap.Is2 );
            fieldBuilder.Attribute( "ChildContent", (RenderFragment)( inputBuilder =>
            {
                inputBuilder.OpenElement( "input" );
                inputBuilder.Type( "color" );
                inputBuilder.Attribute( "value", ReportElementDefinitionHelper.NormalizeColorValue( value ) );
                inputBuilder.Attribute( "onchange", EventCallback.Factory.Create<ChangeEventArgs>( eventReceiver, eventArgs => valueChanged( Convert.ToString( eventArgs.Value, CultureInfo.InvariantCulture ) ) ) );
                inputBuilder.CloseElement();

                RenderButton( inputBuilder, eventReceiver, "Clear", () => valueChanged( null ) );
            } ) );
            fieldBuilder.CloseComponent();
        } ) );
        builder.CloseComponent();
    }

    internal static void RenderSelectInput<TValue>( RenderTreeBuilder builder, object eventReceiver, string label, TValue value, Func<TValue, Task> valueChanged )
        where TValue : struct, Enum
    {
        builder.OpenComponent<Field>();
        builder.Attribute( "Margin", Margin.Is2.FromBottom );
        builder.Attribute( "ChildContent", (RenderFragment)( fieldBuilder =>
        {
            fieldBuilder.OpenComponent<FieldLabel>();
            fieldBuilder.Attribute( "ChildContent", (RenderFragment)( labelBuilder => labelBuilder.Content( label ) ) );
            fieldBuilder.CloseComponent();

            fieldBuilder.OpenComponent<Select<TValue>>();
            fieldBuilder.Attribute( "Value", value );
            fieldBuilder.Attribute( "ValueChanged", EventCallback.Factory.Create<TValue>( eventReceiver, valueChanged ) );
            fieldBuilder.Attribute( "ChildContent", (RenderFragment)( selectBuilder =>
            {
                foreach ( var option in Enum.GetValues<TValue>() )
                {
                    selectBuilder.OpenComponent<SelectItem<TValue>>();
                    selectBuilder.Attribute( "Value", option );
                    selectBuilder.Attribute( "ChildContent", (RenderFragment)( optionBuilder => optionBuilder.Content( option.ToString() ) ) );
                    selectBuilder.CloseComponent();
                }
            } ) );

            fieldBuilder.CloseComponent();
        } ) );
        builder.CloseComponent();
    }

    internal static void RenderNumberInput( RenderTreeBuilder builder, object eventReceiver, string label, double value, Func<double, Task> valueChanged )
    {
        builder.OpenComponent<Field>();
        builder.Attribute( "Margin", Margin.Is2.FromBottom );
        builder.Attribute( "ChildContent", (RenderFragment)( fieldBuilder =>
        {
            fieldBuilder.OpenComponent<FieldLabel>();
            fieldBuilder.Attribute( "ChildContent", (RenderFragment)( labelBuilder => labelBuilder.Content( label ) ) );
            fieldBuilder.CloseComponent();

            fieldBuilder.OpenComponent<NumericInput<double>>();
            fieldBuilder.Attribute( "Value", value );
            fieldBuilder.Attribute( "ValueChanged", EventCallback.Factory.Create<double>( eventReceiver, valueChanged ) );
            fieldBuilder.Attribute( "Immediate", true );
            fieldBuilder.Attribute( "Step", 1m );
            fieldBuilder.CloseComponent();
        } ) );
        builder.CloseComponent();
    }

    internal static void RenderCheckbox( RenderTreeBuilder builder, object eventReceiver, string label, bool value, Action<ChangeEventArgs> valueChanged )
    {
        builder.OpenComponent<Field>();
        builder.Attribute( "Margin", Margin.Is2.FromBottom );
        builder.Attribute( "ChildContent", (RenderFragment)( fieldBuilder =>
        {
            fieldBuilder.OpenElement( "label" );
            fieldBuilder.Class( "b-report-designer-option" );
            fieldBuilder.OpenElement( "input" );
            fieldBuilder.Type( "checkbox" );
            fieldBuilder.Attribute( "checked", value );
            fieldBuilder.Attribute( "onchange", EventCallback.Factory.Create<ChangeEventArgs>( eventReceiver, valueChanged ) );
            fieldBuilder.CloseElement();
            fieldBuilder.Content( label );
            fieldBuilder.CloseElement();
        } ) );
        builder.CloseComponent();
    }

    internal static void RenderButton( RenderTreeBuilder builder, object eventReceiver, string text, Func<Task> clicked )
    {
        builder.OpenComponent<Button>();
        builder.Attribute( "Color", Color.Light );
        builder.Attribute( "Clicked", EventCallback.Factory.Create<MouseEventArgs>( eventReceiver, clicked ) );
        builder.Attribute( "ChildContent", (RenderFragment)( buttonBuilder => buttonBuilder.Content( text ) ) );
        builder.CloseComponent();
    }

    #endregion
}