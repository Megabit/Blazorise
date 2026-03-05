#region Using directives
using System.Collections.Generic;
using System.Linq;
#endregion

namespace Blazorise;

/// <summary>
/// Coordinates all <see cref="Dropdown"/> component instances within a single DI scope.
/// </summary>
public class DropdownCoordinator : IDropdownCoordinator
{
    private readonly object syncLock = new();

    private readonly List<Dropdown> dropdowns = new();

    /// <inheritdoc/>
    public void Register( Dropdown dropdown )
    {
        if ( dropdown is null )
            return;

        lock ( syncLock )
        {
            if ( !dropdowns.Contains( dropdown ) )
                dropdowns.Add( dropdown );
        }
    }

    /// <inheritdoc/>
    public void Unregister( Dropdown dropdown )
    {
        if ( dropdown is null )
            return;

        lock ( syncLock )
        {
            dropdowns.Remove( dropdown );
        }
    }

    /// <inheritdoc/>
    public IReadOnlyList<Dropdown> GetRegistered()
    {
        lock ( syncLock )
        {
            return dropdowns.ToArray();
        }
    }
}