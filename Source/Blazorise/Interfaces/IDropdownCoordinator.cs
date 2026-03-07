#region Using directives
using System.Collections.Generic;
#endregion

namespace Blazorise;

/// <summary>
/// Coordinates all <see cref="Dropdown"/> component instances within a single DI scope.
/// </summary>
public interface IDropdownCoordinator
{
    /// <summary>
    /// Registers a dropdown instance.
    /// </summary>
    /// <param name="dropdown">The dropdown instance.</param>
    void Register( Dropdown dropdown );

    /// <summary>
    /// Unregisters a dropdown instance.
    /// </summary>
    /// <param name="dropdown">The dropdown instance.</param>
    void Unregister( Dropdown dropdown );

    /// <summary>
    /// Gets a snapshot of currently registered dropdowns.
    /// </summary>
    IReadOnlyList<Dropdown> GetRegistered();
}