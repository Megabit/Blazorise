namespace Blazorise.CodeEditor;

/// <summary>
/// Represents a diagnostic marker displayed in the code editor.
/// </summary>
public class CodeEditorDiagnostic
{
    /// <summary>
    /// Gets or sets the diagnostic severity.
    /// </summary>
    public CodeEditorDiagnosticSeverity Severity { get; set; } = CodeEditorDiagnosticSeverity.Error;

    /// <summary>
    /// Gets or sets the diagnostic message.
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// Gets or sets the diagnostic code.
    /// </summary>
    public string Code { get; set; }

    /// <summary>
    /// Gets or sets the starting line number.
    /// </summary>
    public int StartLineNumber { get; set; } = 1;

    /// <summary>
    /// Gets or sets the starting column.
    /// </summary>
    public int StartColumn { get; set; } = 1;

    /// <summary>
    /// Gets or sets the ending line number.
    /// </summary>
    public int EndLineNumber { get; set; } = 1;

    /// <summary>
    /// Gets or sets the ending column.
    /// </summary>
    public int EndColumn { get; set; } = 1;
}