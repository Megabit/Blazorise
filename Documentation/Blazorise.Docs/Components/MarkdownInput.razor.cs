#region Using directives
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Blazorise;
using Blazorise.Extensions;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Docs.Components;

public partial class MarkdownInput : BaseInputComponent<string>
{
    #region Methods

    public override async Task SetParametersAsync( ParameterView parameters )
    {
        if ( Rendered
             && parameters.TryGetValue<string>( nameof( Value ), out var newValue )
             && !newValue.IsEqual( Value ) )
        {
            ExecuteAfterRender( Revalidate );
        }

        await base.SetParametersAsync( parameters );

        if ( ParentValidation is not null )
        {
            if ( parameters.TryGetValue<Expression<Func<string>>>( nameof( ValueExpression ), out var expression ) )
                await ParentValidation.InitializeInputExpression( expression );

            await InitializeValidation();
        }
    }

    protected override Task OnInternalValueChanged( string value )
        => ValueChanged.InvokeAsync( value );

    protected override Task<ParseValue<string>> ParseValueFromStringAsync( string value )
        => Task.FromResult( new ParseValue<string>( true, value, null ) );

    protected override string GetFormatedValueExpression()
    {
        if ( ValueExpression is null )
            return null;

        return HtmlFieldPrefix is not null
            ? HtmlFieldPrefix.GetFieldName( ValueExpression )
            : ExpressionFormatter.FormatLambda( ValueExpression );
    }

    protected Task OnMarkdownValueChanged( string value )
        => CurrentValueHandler( value ?? string.Empty );

    #endregion

    #region Properties

    protected string MarkdownInputClassNames => ParentValidation?.Status == ValidationStatus.Error ? "is-invalid" : null;

    protected override string InternalValue { get => Value; set => Value = value; }

    [Parameter] public string Value { get; set; }

    [Parameter] public EventCallback<string> ValueChanged { get; set; }

    [Parameter] public Expression<Func<string>> ValueExpression { get; set; }

    [Parameter] public string Placeholder { get; set; }

    [Parameter] public string MinHeight { get; set; } = "300px";

    [Parameter] public string MaxHeight { get; set; }

    #endregion
}