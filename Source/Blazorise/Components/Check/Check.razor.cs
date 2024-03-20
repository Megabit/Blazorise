﻿#region Using directives
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Checkboxes allow the user to select one or more items from a set.
/// </summary>
/// <typeparam name="TValue">Checked value type.</typeparam>
public partial class Check<TValue> : BaseCheckComponent<TValue>
{
    #region Members

    private bool? indeterminate;

    #endregion

    #region Methods

    /// <inheritdoc/>
    public override async Task SetParametersAsync( ParameterView parameters )
    {
        if ( Rendered )
        {
            if ( parameters.TryGetValue<TValue>( nameof( Checked ), out var paramChecked ) && !paramChecked.IsEqual( Checked ) )
            {
                ExecuteAfterRender( Revalidate );
            }
        }

        await base.SetParametersAsync( parameters );

        if ( ParentValidation is not null )
        {
            if ( parameters.TryGetValue<Expression<Func<TValue>>>( nameof( CheckedExpression ), out var expression ) )
                await ParentValidation.InitializeInputExpression( expression );

            await InitializeValidation();
        }

        if ( parameters.TryGetValue<bool?>( nameof( Indeterminate ), out var indeterminate ) && this.indeterminate != indeterminate )
        {
            this.indeterminate = indeterminate;

            ExecuteAfterRender( async () =>
            {
                await JSUtilitiesModule.SetProperty( ElementRef, "indeterminate", indeterminate );
            } );
        }
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.Check() );
        builder.Append( ClassProvider.CheckSize( ThemeSize ), ThemeSize != Blazorise.Size.Default );
        builder.Append( ClassProvider.CheckCursor( Cursor ), Cursor != Cursor.Default );
        builder.Append( ClassProvider.CheckValidation( ParentValidation?.Status ?? ValidationStatus.None ), ParentValidation?.Status != ValidationStatus.None );

        base.BuildClasses( builder );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the string representation of a boolean "true" value.
    /// </summary>
    protected override string TrueValueName => "true";

    /// <summary>
    /// The indeterminate property can help you to achieve a 'check all' effect.
    /// </summary>
    [Parameter] public bool? Indeterminate { get; set; }

    /// <summary>
    /// Defines the name attribute of a checkbox.
    /// </summary>
    /// <remarks>
    /// The name attribute is used to identify form data after it has been submitted to the server, or to reference form data using JavaScript on the client side.
    /// </remarks>
    [Parameter] public string Name { get; set; }

    #endregion
}