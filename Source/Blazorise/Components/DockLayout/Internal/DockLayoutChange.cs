namespace Blazorise;

internal enum DockLayoutChangeKind
{
    Tree,
    Node,
    Pane,
    Compass,
}

internal readonly record struct DockLayoutChange( DockLayoutChangeKind Kind, string NodeId = null, string PaneName = null );