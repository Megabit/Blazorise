#region Using directives
using System.Collections.Generic;
#endregion

namespace Blazorise.Reporting.Internal;

internal sealed class ReportDesignerWarning
{
    internal ReportDesignerWarning( string message, IReadOnlyList<string> elementKeys )
    {
        Message = message;
        ElementKeys = elementKeys;
    }

    internal string Message { get; }

    internal IReadOnlyList<string> ElementKeys { get; }
}