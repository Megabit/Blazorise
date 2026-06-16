#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
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

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        ParentLanguage?.NotifyTokenizerInitialized( this );
    }

    /// <inheritdoc/>
    protected override void OnParametersSet()
    {
        ParentLanguage?.NotifyTokenizerInitialized( this );
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        ParentLanguage?.NotifyTokenizerRemoved( this );
    }

    internal void NotifyTokenInitialized( CodeEditorToken token )
    {
        if ( !tokens.Contains( token ) )
            tokens.Add( token );

        ParentLanguage?.NotifyTokenizerInitialized( this );
    }

    internal void NotifyTokenRemoved( CodeEditorToken token )
    {
        if ( tokens.Remove( token ) )
            ParentLanguage?.NotifyTokenizerInitialized( this );
    }

    internal CodeEditorTokenizerDefinition ToDefinition()
    {
        IReadOnlyList<CodeEditorTokenDefinition> tokenDefinitions = Tokens ?? tokens.Select( x => x.ToDefinition() ).ToArray();

        return new CodeEditorTokenizerDefinition
        {
            IgnoreCase = IgnoreCase,
            Unicode = Unicode,
            DefaultToken = DefaultToken,
            Tokens = tokenDefinitions
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
    [Parameter] public IReadOnlyList<CodeEditorTokenDefinition> Tokens { get; set; }

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