using Blazorise.Bootstrap.Providers;
using Blazorise.Extensions;

namespace Blazorise.Material.Providers;

public class MaterialClassProvider : BootstrapClassProvider
{
    #region Steps

    public override string Steps() => "stepper-horiz";

    public override string StepItem() => "stepper";

    public override string StepItemActive( bool active ) => active ? "active" : null;

    public override string StepItemCompleted( bool completed ) => completed ? "done" : null;

    public override string StepItemColor( Color color ) => color.IsNullOrDefault() ? null : $"{StepItem()}-{ToColor( color )}";

    public override string StepItemMarkerColor( Color color, bool active ) => null;

    public override string StepItemMarker() => "stepper-icon";

    public override string StepItemDescription() => "stepper-text";

    public override string StepsContent() => "stepper-horiz-content";

    public override string StepPanel() => "stepper-panel";

    public override string StepPanelActive( bool active ) => active ? "active" : null;

    #endregion

    public override string TabPanel() => "tab-pane fade";

    public override string Bar( BarMode mode ) => "navbar navbar-full";

    public override string AccordionToggle() => "btn btn-link btn-block text-left";

    public override string AccordionToggleCollapsed( bool collapsed ) => collapsed ? null : "collapsed";

    public override string Provider => "Material";
}