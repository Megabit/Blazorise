namespace Blazorise.Providers;

/// <summary>
/// Used only when user wants to use extensions(Chart, Sidebar, etc) without CSS frameworks!!
/// </summary>
class EmptyBehaviourProvider : BehaviourProvider
{
    #region DataGrid

    public override bool DataGridRowMultiSelectPreventClick => false;

    #endregion
}