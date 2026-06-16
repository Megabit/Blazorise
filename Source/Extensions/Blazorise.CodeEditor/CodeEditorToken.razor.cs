#region Using directives
using System;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.CodeEditor;

/// <summary>
/// Declaratively defines a single tokenizer rule.
/// </summary>
public partial class CodeEditorToken : ComponentBase, IDisposable
{
    #region Methods

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        ParentTokenizer?.NotifyTokenInitialized( this );
    }

    /// <inheritdoc/>
    protected override void OnParametersSet()
    {
        ParentTokenizer?.NotifyTokenInitialized( this );
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        ParentTokenizer?.NotifyTokenRemoved( this );
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
    [Parameter] public string Next { get; set; }

    /// <summary>
    /// Gets or sets the bracket token action.
    /// </summary>
    [Parameter] public string Bracket { get; set; }

    /// <summary>
    /// Gets or sets the parent tokenizer.
    /// </summary>
    [CascadingParameter] protected CodeEditorTokenizer ParentTokenizer { get; set; }

    #endregion
}