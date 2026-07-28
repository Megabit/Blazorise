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
/// Declaratively defines custom language tokenizer rules.
/// </summary>
public partial class CodeEditorTokenizer : ComponentBase, IDisposable
{
    #region Members

    private readonly List<CodeEditorToken> tokens = new();

    private readonly List<CodeEditorTokenizerState> states = new();

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        if ( ParentLanguage is null )
            throw new ArgumentNullException( nameof( ParentLanguage ), $"{nameof( CodeEditorTokenizer )} must exist within a {nameof( CodeEditorCustomLanguage )}." );

        ParentLanguage.NotifyTokenizerInitialized( this );

        base.OnInitialized();
    }

    /// <inheritdoc/>
    public override Task SetParametersAsync( ParameterView parameters )
    {
        parameters.TryGetParameter( Tokens, newValue => ReferenceEquals( Tokens, newValue ), out ComponentParameterInfo<IReadOnlyList<CodeEditorTokenDefinition>> paramTokens );
        parameters.TryGetParameter( States, newValue => ReferenceEquals( States, newValue ), out ComponentParameterInfo<IReadOnlyDictionary<string, IReadOnlyList<CodeEditorTokenDefinition>>> paramStates );

        if ( ParentLanguage is not null
             && ( parameters.IsParameterChanged( IgnoreCase )
                  || parameters.IsParameterChanged( Unicode )
                  || parameters.IsParameterChanged( DefaultToken )
                  || paramTokens.Changed
                  || paramStates.Changed ) )
        {
            ParentLanguage.NotifyTokenizerChanged();
        }

        return base.SetParametersAsync( parameters );
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        ParentLanguage?.NotifyTokenizerRemoved( this );
    }

    internal void NotifyTokenInitialized( CodeEditorToken token )
    {
        if ( !tokens.Contains( token ) )
        {
            tokens.Add( token );
        }

        ParentLanguage?.NotifyTokenizerChanged();
    }

    internal void NotifyTokenRemoved( CodeEditorToken token )
    {
        if ( tokens.Remove( token ) )
        {
            ParentLanguage?.NotifyTokenizerChanged();
        }
    }

    internal void NotifyTokenChanged()
    {
        ParentLanguage?.NotifyTokenizerChanged();
    }

    internal void NotifyStateInitialized( CodeEditorTokenizerState state )
    {
        if ( !states.Contains( state ) )
        {
            states.Add( state );
        }

        ParentLanguage?.NotifyTokenizerChanged();
    }

    internal void NotifyStateRemoved( CodeEditorTokenizerState state )
    {
        if ( states.Remove( state ) )
        {
            ParentLanguage?.NotifyTokenizerChanged();
        }
    }

    internal void NotifyStateChanged()
    {
        ParentLanguage?.NotifyTokenizerChanged();
    }

    internal CodeEditorTokenizerDefinition ToDefinition()
    {
        IReadOnlyList<CodeEditorTokenDefinition> tokenDefinitions = Tokens ?? tokens.Select( x => x.ToDefinition() ).ToArray();
        Dictionary<string, IReadOnlyList<CodeEditorTokenDefinition>> stateDefinitions = new( StringComparer.Ordinal );

        if ( States is not null )
        {
            foreach ( KeyValuePair<string, IReadOnlyList<CodeEditorTokenDefinition>> state in States )
            {
                if ( !string.IsNullOrWhiteSpace( state.Key ) )
                {
                    stateDefinitions[state.Key] = state.Value ?? Array.Empty<CodeEditorTokenDefinition>();
                }
            }
        }

        foreach ( CodeEditorTokenizerState state in states )
        {
            if ( !string.IsNullOrWhiteSpace( state.Name ) )
            {
                stateDefinitions[state.Name] = state.ToDefinitions();
            }
        }

        return new CodeEditorTokenizerDefinition
        {
            IgnoreCase = IgnoreCase,
            Unicode = Unicode,
            DefaultToken = DefaultToken,
            Tokens = tokenDefinitions,
            States = stateDefinitions
        };
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets a value indicating whether token matching should ignore casing.
    /// </summary>
    [Parameter] public bool IgnoreCase { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether tokenizer regular expressions are unicode aware.
    /// </summary>
    [Parameter] public bool Unicode { get; set; }

    /// <summary>
    /// Gets or sets the fallback token name.
    /// </summary>
    [Parameter] public string DefaultToken { get; set; }

    /// <summary>
    /// Gets or sets tokenizer rules.
    /// </summary>
    /// <remarks>
    /// When supplied, these rules take precedence over nested root-level <see cref="CodeEditorToken"/> components.
    /// </remarks>
    [Parameter] public IReadOnlyList<CodeEditorTokenDefinition> Tokens { get; set; }

    /// <summary>
    /// Gets or sets named tokenizer states.
    /// </summary>
    /// <remarks>
    /// A state named <c>root</c> overrides the rules supplied through <see cref="Tokens"/>.
    /// </remarks>
    [Parameter] public IReadOnlyDictionary<string, IReadOnlyList<CodeEditorTokenDefinition>> States { get; set; }

    /// <summary>
    /// Gets or sets the child content.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Gets or sets the parent language definition component.
    /// </summary>
    [CascadingParameter] protected CodeEditorCustomLanguage ParentLanguage { get; set; }

    #endregion
}