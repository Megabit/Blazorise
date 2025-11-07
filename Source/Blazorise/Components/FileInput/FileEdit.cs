#region Using directives
using System;
#endregion

namespace Blazorise;

/// <summary>
/// Represents a file input control.
/// </summary>
/// <remarks>
/// <para>
/// <b>Obsolete:</b> This component is an alias for <see cref="FileInput"/> and is maintained for backward compatibility only.
/// Please use <see cref="FileInput"/> instead.
/// </para>
/// </remarks>
[Obsolete( "FileEdit has been replaced by FileInput. Please use FileInput instead." )]
public class FileEdit : FileInput
{
}