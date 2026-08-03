namespace Blazorise.CodeEditor;

/// <summary>
/// Defines diagnostic marker severity levels.
/// </summary>
public enum CodeEditorDiagnosticSeverity
{
    /// <summary>Subtle suggestion that does not indicate a problem.</summary>
    Hint = 1,
    /// <summary>Informational diagnostic requiring no corrective action.</summary>
    Info = 2,
    /// <summary>Potential problem that deserves attention.</summary>
    Warning = 4,
    /// <summary>Problem that prevents correct interpretation or execution.</summary>
    Error = 8
}