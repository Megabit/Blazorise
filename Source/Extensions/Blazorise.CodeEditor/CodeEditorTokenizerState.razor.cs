#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.CodeEditor;

/// <summary>
/// Declaratively defines a named Monarch tokenizer state.
/// </summary>
public partial class CodeEditorTokenizerState : ComponentBase, IDisposable
{
    #region Members

    private readonly List<CodeEditorToken> tokens = new();

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        if ( ParentTokenizer is null )
            throw new ArgumentNullException( nameof( ParentTokenizer ), $"{nameof( CodeEditorTokenizerState )} must exist within a {nameof( CodeEditorTokenizer )}." );

        ParentTokenizer.NotifyStateInitialized( this );

        base.OnInitialized();
    }

    /// <inheritdoc/>
    public override Task SetParametersAsync( ParameterView parameters )
    {
        parameters.TryGetParameter( Tokens, newValue => ReferenceEquals( Tokens, newValue ), out ComponentParameterInfo<IReadOnlyList<CodeEditorTokenDefinition>> paramTokens );

        if ( ParentTokenizer is not null
             && ( parameters.IsParameterChanged( Name )
                  || paramTokens.Changed ) )
        {
            ParentTokenizer.NotifyStateChanged();
        }

        return base.SetParametersAsync( parameters );
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        ParentTokenizer?.NotifyStateRemoved( this );
    }

    internal void NotifyTokenInitialized( CodeEditorToken token )
    {
        if ( !tokens.Contains( token ) )
        {
            tokens.Add( token );
        }

        ParentTokenizer?.NotifyStateChanged();
    }

    internal void NotifyTokenRemoved( CodeEditorToken token )
    {
        if ( tokens.Remove( token ) )
        {
            ParentTokenizer?.NotifyStateChanged();
        }
    }

    internal void NotifyTokenChanged()
    {
        ParentTokenizer?.NotifyStateChanged();
    }

    internal IReadOnlyList<CodeEditorTokenDefinition> ToDefinitions()
    {
        return Tokens ?? tokens.Select( token => token.ToDefinition() ).ToArray();
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the tokenizer state name.
    /// </summary>
    [Parameter] public string Name { get; set; }

    /// <summary>
    /// Gets or sets tokenizer rules.
    /// </summary>
    /// <remarks>
    /// When supplied, these rules take precedence over nested <see cref="CodeEditorToken"/> components.
    /// </remarks>
    [Parameter] public IReadOnlyList<CodeEditorTokenDefinition> Tokens { get; set; }

    /// <summary>
    /// Gets or sets the child content.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Gets or sets the parent tokenizer.
    /// </summary>
    [CascadingParameter] protected CodeEditorTokenizer ParentTokenizer { get; set; }

    #endregion
}