namespace Blazorise;

internal sealed class DockDragState
{
    #region Methods

    public void Update( string paneName, DockZone? zone, string compassZoneKey, double compassX, double compassY )
    {
        PaneName = paneName;
        Zone = zone;
        CompassZoneKey = compassZoneKey;
        CompassX = compassX;
        CompassY = compassY;
    }

    public void Clear()
    {
        PaneName = null;
        Zone = null;
        CompassZoneKey = null;
        Group = false;
    }

    #endregion

    #region Properties

    public string PaneName { get; private set; }

    public DockZone? Zone { get; private set; }

    public string CompassZoneKey { get; private set; }

    public bool Group { get; set; }

    public double CompassX { get; private set; }

    public double CompassY { get; private set; }

    public bool Visible => PaneName is not null && ( CompassX != 0d || CompassY != 0d );

    #endregion
}