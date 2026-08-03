#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.CodeEditor;

/// <summary>
/// Declaratively defines a single tokenizer rule.
/// </summary>
public class CodeEditorToken : ComponentBase, IDisposable
{
    #region Methods

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        if ( ParentState is null && ParentTokenizer is null )
            throw new ArgumentNullException( nameof( ParentTokenizer ), $"{nameof( CodeEditorToken )} must exist within a {nameof( CodeEditorTokenizer )} or {nameof( CodeEditorTokenizerState )}." );

        NotifyInitialized();

        base.OnInitialized();
    }

    /// <inheritdoc/>
    public override Task SetParametersAsync( ParameterView parameters )
    {
        if ( ( ParentState is not null || ParentTokenizer is not null )
             && ( parameters.IsParameterChanged( Pattern )
                  || parameters.IsParameterChanged( Token )
                  || parameters.IsParameterChanged( Next )
                  || parameters.IsParameterChanged( Bracket ) ) )
        {
            NotifyChanged();
        }

        return base.SetParametersAsync( parameters );
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if ( ParentState is not null )
        {
            ParentState.NotifyTokenRemoved( this );
        }
        else
        {
            ParentTokenizer?.NotifyTokenRemoved( this );
        }
    }

    private void NotifyInitialized()
    {
        if ( ParentState is not null )
        {
            ParentState.NotifyTokenInitialized( this );
        }
        else
        {
            ParentTokenizer?.NotifyTokenInitialized( this );
        }
    }

    private void NotifyChanged()
    {
        if ( ParentState is not null )
        {
            ParentState.NotifyTokenChanged();
        }
        else
        {
            ParentTokenizer?.NotifyTokenChanged();
        }
    }

    internal CodeEditorTokenDefinition ToDefinition()
    {
        return new CodeEditorTokenDefinition
        {
            Pattern = Pattern,
            Token = Token,
            Next = Next,
            Bracket = Bracket
        };
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the regular expression pattern.
    /// </summary>
    [Parameter] public string Pattern { get; set; }

    /// <summary>
    /// Gets or sets the token name.
    /// </summary>
    [Parameter] public string Token { get; set; }

    /// <summary>
    /// Gets or sets the next tokenizer state.
    /// </summary>
    /// <remarks>
    /// Use a named state such as <c>@string</c>, or a tokenizer transition such as <c>@pop</c>.
    /// </remarks>
    [Parameter] public string Next { get; set; }

    /// <summary>
    /// Gets or sets the bracket token action.
    /// </summary>
    /// <remarks>
    /// Supported values are <c>@open</c> and <c>@close</c>.
    /// </remarks>
    [Parameter] public string Bracket { get; set; }

    /// <summary>
    /// Gets or sets the parent tokenizer.
    /// </summary>
    [CascadingParameter] protected CodeEditorTokenizer ParentTokenizer { get; set; }

    /// <summary>
    /// Gets or sets the parent tokenizer state.
    /// </summary>
    [CascadingParameter] protected CodeEditorTokenizerState ParentState { get; set; }

    #endregion
}