#region Using directives
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazorise.Extensions;
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
        if ( ParentCodeEditor is null )
            throw new ArgumentNullException( nameof( ParentCodeEditor ), $"{nameof( CodeEditorCustomLanguage )} must exist within a {nameof( CodeEditor )}." );

        ParentCodeEditor.NotifyLanguageInitialized( this );

        base.OnInitialized();
    }

    /// <inheritdoc/>
    public override Task SetParametersAsync( ParameterView parameters )
    {
        parameters.TryGetParameter( Aliases, newValue => ReferenceEquals( Aliases, newValue ), out ComponentParameterInfo<IReadOnlyList<string>> paramAliases );
        parameters.TryGetParameter( Extensions, newValue => ReferenceEquals( Extensions, newValue ), out ComponentParameterInfo<IReadOnlyList<string>> paramExtensions );
        parameters.TryGetParameter( MimeTypes, newValue => ReferenceEquals( MimeTypes, newValue ), out ComponentParameterInfo<IReadOnlyList<string>> paramMimeTypes );
        parameters.TryGetParameter( Tokenizer, newValue => ReferenceEquals( Tokenizer, newValue ), out ComponentParameterInfo<CodeEditorTokenizerDefinition> paramTokenizer );

        if ( ParentCodeEditor is not null
             && ( parameters.IsParameterChanged( Id )
                  || paramAliases.Changed
                  || paramExtensions.Changed
                  || paramMimeTypes.Changed
                  || paramTokenizer.Changed
                  || parameters.IsParameterChanged( ConfigureLanguageMethod ) ) )
        {
            ParentCodeEditor.NotifyLanguageChanged();
        }

        return base.SetParametersAsync( parameters );
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

    internal void NotifyTokenizerChanged()
    {
        ParentCodeEditor?.NotifyLanguageChanged();
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
    /// <remarks>
    /// When supplied, this definition takes precedence over a nested <see cref="CodeEditorTokenizer"/>.
    /// </remarks>
    [Parameter] public CodeEditorTokenizerDefinition Tokenizer { get; set; }

    /// <summary>
    /// Gets or sets the custom JavaScript method used to configure advanced language features.
    /// </summary>
    /// <remarks>
    /// The method receives the language definition and the Monaco API. It can return a disposable registration.
    /// </remarks>
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