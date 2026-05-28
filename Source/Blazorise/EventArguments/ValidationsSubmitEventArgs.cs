#region Using directives
using System;
#endregion

namespace Blazorise;

internal sealed class ValidationsSubmitEventArgs : EventArgs
{
    public ValidationsSubmitEventArgs( ValidationsSubmitPriority priority )
    {
        Priority = priority;
    }

    public void Handle()
    {
        Handled = true;
    }

    public ValidationsSubmitPriority Priority { get; }

    public bool Handled { get; private set; }
}