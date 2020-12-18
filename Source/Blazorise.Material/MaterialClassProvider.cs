﻿#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace Blazorise.Material
{
    public class MaterialClassProvider : Bootstrap.BootstrapClassProvider
    {
        #region Steps

        public override string Steps() => "stepper-horiz";

        public override string StepItem() => "stepper";

        public override string StepItemActive( bool active ) => active ? "active" : null;

        public override string StepItemCompleted( bool completed ) => completed ? "done" : null;

        public override string StepItemColor( Color color ) => $"{StepItem()}-{ToColor( color )}";

        public override string StepItemMarker() => "stepper-icon";

        public override string StepItemDescription() => "stepper-text";

        public override string StepsContent() => "stepper-horiz-content";

        public override string StepPanel() => "stepper-panel";

        public override string StepPanelActive( bool active ) => active ? "active" : null;

        #endregion

        public override string TabPanel() => "tab-pane fade";

        public override string Bar() => "navbar navbar-full";

        public override string BarItemHasDropdown( BarMode mode ) => mode == Blazorise.BarMode.Horizontal ? "dropdown" : null;

        public override string Provider => "Material";
    }
}
