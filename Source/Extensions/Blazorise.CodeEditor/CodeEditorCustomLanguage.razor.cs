#region Using directives
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.CodeEditor;

/// <summary>
/// Declaratively defines a custom code editor language.
/// </summary>
public partial class CodeEditorCustomLanguage : ComponentBase, IDisposable
{
    #region Members

    private CodeEditorTokenizer tokenizer;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        ParentCodeEditor?.NotifyLanguageInitialized( this );
    }

    /// <inheritdoc/>
    protected override void OnParametersSet()
    {
        ParentCodeEditor?.NotifyLanguageChanged();
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        ParentCodeEditor?.NotifyLanguageRemoved( this );
    }

    internal void NotifyTokenizerInitialized( CodeEditorTokenizer tokenizer )
    {
        this.tokenizer = tokenizer;

        ParentCodeEditor?.NotifyLanguageChanged();
    }

    internal void NotifyTokenizerRemoved( CodeEditorTokenizer tokenizer )
    {
        if ( this.tokenizer == tokenizer )
        {
            this.tokenizer = null;

            ParentCodeEditor?.NotifyLanguageChanged();
        }
    }

    internal CodeEditorLanguageDefinition ToDefinition()
    {
        return new CodeEditorLanguageDefinition
        {
            Id = Id,
            Aliases = Aliases,
            Extensions = Extensions,
            MimeTypes = MimeTypes,
            Tokenizer = Tokenizer ?? tokenizer?.ToDefinition(),
            ConfigureLanguageMethod = ConfigureLanguageMethod
        };
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the language identifier.
    /// </summary>
    [Parameter] public string Id { get; set; }

    /// <summary>
    /// Gets or sets the language aliases.
    /// </summary>
    [Parameter] public IReadOnlyList<string> Aliases { get; set; }

    /// <summary>
    /// Gets or sets the file extensions associated with the language.
    /// </summary>
    [Parameter] public IReadOnlyList<string> Extensions { get; set; }

    /// <summary>
    /// Gets or sets the MIME types associated with the language.
    /// </summary>
    [Parameter] public IReadOnlyList<string> MimeTypes { get; set; }

    /// <summary>
    /// Gets or sets the tokenizer definition.
    /// </summary>
    [Parameter] public CodeEditorTokenizerDefinition Tokenizer { get; set; }

    /// <summary>
    /// Gets or sets the custom JavaScript method used to configure advanced language features.
    /// </summary>
    [Parameter] public string ConfigureLanguageMethod { get; set; }

    /// <summary>
    /// Gets or sets the child content.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Gets or sets the parent code editor.
    /// </summary>
    [CascadingParameter] protected CodeEditor ParentCodeEditor { get; set; }

    #endregion
}