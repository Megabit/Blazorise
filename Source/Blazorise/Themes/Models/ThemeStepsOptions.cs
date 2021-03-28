using System;

namespace Blazorise
{
    public class ThemeStepsOptions
    {
        public string StepsItemIconColor { get; set; } = "#adb5bd";

        public string StepsItemIconCompleted { get; set; } = "#007bff";

        public string StepsItemIconCompletedYiq { get; set; } = "#ffffff";

        public string StepsItemIconActive { get; set; } = "#007bff";

        public string StepsItemIconActiveYiq { get; set; } = "#ffffff";

        public string StepsItemTextColor { get; set; } = "#adb5bd";

        public string StepsItemTextCompleted { get; set; } = "#28a745";

        public string StepsItemTextActive { get; set; } = "#28a745";

        public override bool Equals( object obj )
        {
            return obj is ThemeStepsOptions options &&
                     StepsItemIconColor == options.StepsItemIconColor &&
                     StepsItemIconCompleted == options.StepsItemIconCompleted &&
                     StepsItemIconCompletedYiq == options.StepsItemIconCompletedYiq &&
                     StepsItemIconActive == options.StepsItemIconActive &&
                     StepsItemIconActiveYiq == options.StepsItemIconActiveYiq &&
                     StepsItemTextColor == options.StepsItemTextColor &&
                     StepsItemTextCompleted == options.StepsItemTextCompleted &&
                     StepsItemTextActive == options.StepsItemTextActive;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine( StepsItemIconColor, StepsItemIconCompleted, StepsItemIconCompletedYiq, StepsItemIconActive, StepsItemIconActiveYiq, StepsItemTextColor, StepsItemTextCompleted, StepsItemTextActive );
        }
    }
}
