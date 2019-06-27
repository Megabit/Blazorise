#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Holds the options for the theme.
    /// </summary>
    public class Theme
    {
        public event EventHandler<EventArgs> Changed;

        public void ThemeHasChanged()
        {
            Changed?.Invoke( this, EventArgs.Empty );
        }

        public string White { get; set; } = "#ffffff";

        public string Black { get; set; } = "#343a40";

        public bool IsGradient { get; set; }

        /// <summary>
        /// Gets the valid variant colors.
        /// </summary>
        public IEnumerable<(string name, string color)> Variants
            => VariantOptions?.Map?.Where( x => !string.IsNullOrEmpty( x.Value() ) ).Select( x => (x.Key, x.Value()) ) ?? Enumerable.Empty<(string, string)>();

        public ThemeVariantOptions VariantOptions { get; set; }

        public ThemeButtonOptions ButtonOptions { get; set; }

        public ThemeDropdownOptions DropdownOptions { get; set; }

        public ThemeInputOptions InputOptions { get; set; }

        public ThemeCardOptions CardOptions { get; set; }

        public ThemeModalOptions ModalOptions { get; set; }

        public ThemeTabsOptions TabsOptions { get; set; }

        public ThemeProgressOptions ProgressOptions { get; set; }

        public ThemeAlertOptions AlertOptions { get; set; }

        public ThemeBreadcrumbOptions BreadcrumbOptions { get; set; }

        public ThemeBadgeOptions BadgeOptions { get; set; }

        public ThemePaginationOptions PaginationOptions { get; set; }
    }

    public class BasicOptions
    {
        public string BorderRadius { get; set; } = ".25rem";
    }

    public class ThemeButtonOptions : BasicOptions
    {
        public string Padding { get; set; }

        public string Margin { get; set; }

        public string BoxShadowSize { get; set; }

        public byte BoxShadowTransparency { get; set; } = 127;

        public float HoverDarkenColor { get; set; } = 7.5f;

        public float HoverLightenColor { get; set; } = 10f;

        public float ActiveDarkenColor { get; set; } = 10f;

        public float ActiveLightenColor { get; set; } = 12.5f;
    }

    public class ThemeDropdownOptions : BasicOptions
    {
    }

    public class ThemeInputOptions : BasicOptions
    {
        public string Color { get; set; }
    }

    public class ThemeCardOptions : BasicOptions
    {
        public string ImageTopRadius { get; set; } = "calc(.25rem - 1px)";
    }

    public class ThemeModalOptions : BasicOptions
    {
    }

    public class ThemeTabsOptions : BasicOptions
    {
    }

    public class ThemeProgressOptions : BasicOptions
    {
    }

    public class ThemeAlertOptions : BasicOptions
    {
        public int BackgroundLevel { get; set; } = -10;

        public int BorderLevel { get; set; } = -9;

        public int ColorLevel { get; set; } = 6;
    }

    public class ThemeBreadcrumbOptions : BasicOptions
    {
    }

    public class ThemeBadgeOptions : BasicOptions
    {
    }

    public class ThemePaginationOptions : BasicOptions
    {
        public string LargeBorderRadius { get; set; } = ".3rem";
    }

    public class ThemeVariantOptions
    {
        public Dictionary<string, Func<string>> Map => new Dictionary<string, Func<string>> {
            { "primary", () => Primary },
            { "secondary", () => Secondary },
            { "success", () => Success },
            { "info", () => Info },
            { "warning", () => Warning },
            { "danger", () => Danger },
            { "light", () => Light },
            { "dark", () => Dark }
        };

        public string Primary { get; set; }

        public string Secondary { get; set; }

        public string Success { get; set; }

        public string Info { get; set; }

        public string Warning { get; set; }

        public string Danger { get; set; }

        public string Light { get; set; }

        public string Dark { get; set; }
    }
}
