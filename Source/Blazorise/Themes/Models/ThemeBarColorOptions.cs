#region Using directives
#endregion

using System;
using System.Collections.Generic;

namespace Blazorise
{
    public class ThemeBarColorOptions
    {
        public string BackgroundColor { get; set; }

        public string Color { get; set; }

        public ThemeBarItemColorOptions ItemColorOptions { get; set; }

        public ThemeBarDropdownColorOptions DropdownColorOptions { get; set; }

        public ThemeBarBrandColorOptions BrandColorOptions { get; set; }

        public override bool Equals( object obj )
        {
            return obj is ThemeBarColorOptions options &&
                     BackgroundColor == options.BackgroundColor &&
                     Color == options.Color &&
                    EqualityComparer<ThemeBarItemColorOptions>.Default.Equals( ItemColorOptions, options.ItemColorOptions ) &&
                    EqualityComparer<ThemeBarDropdownColorOptions>.Default.Equals( DropdownColorOptions, options.DropdownColorOptions ) &&
                    EqualityComparer<ThemeBarBrandColorOptions>.Default.Equals( BrandColorOptions, options.BrandColorOptions );
        }

        public override int GetHashCode()
        {
            return HashCode.Combine( BackgroundColor, Color, ItemColorOptions, DropdownColorOptions, BrandColorOptions );
        }
    }
}
