using Blazorise.Bootstrap.Providers;
using Blazorise.Extensions;

namespace Blazorise.Material.Providers;

public class MaterialClassProvider : BootstrapClassProvider
{
    #region RangeSlider

    public override string RangeSlider() => "range-slider";

    public override string RangeSliderTrack() => "range-slider-track";

    public override string RangeSliderRange() => "range-slider-range";

    public override string RangeSliderInput() => "form-control-range range-slider-input";

    public override string RangeSliderStart() => "range-slider-input-start";

    public override string RangeSliderEnd() => "range-slider-input-end";

    public override string RangeSliderTooltip() => "badge badge-light range-slider-tooltip";

    public override string RangeSliderValidation( ValidationStatus validationStatus ) => validationStatus != ValidationStatus.None ? ToValidationStatus( validationStatus ) : null;

    #endregion

    #region Steps

    public override string Steps() => "stepper-horiz";

    public override string StepItem() => "stepper";

    public override string StepItemActive( bool active ) => active ? "active" : null;

    public override string StepItemCompleted( bool completed ) => completed ? "done" : null;

    public override string StepItemColor( Color color ) => color.IsNotNullOrDefault() ? $"{StepItem()}-{ToColor( color )}" : null;

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