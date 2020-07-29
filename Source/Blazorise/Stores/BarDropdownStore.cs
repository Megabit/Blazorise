#region Using directives
using System;
using System.Collections.Generic;
using System.Resources;
using System.Text;
#endregion

namespace Blazorise.Stores
{
    public struct BarDropdownStore : IEquatable<BarDropdownStore>
    {
        #region Methods

        public override bool Equals( object obj )
            => obj is BarDropdownStore store && Equals( store );

        public bool Equals( BarDropdownStore other )
        {
            return Visible == other.Visible
                && Mode == other.Mode
                && BarVisible == other.BarVisible
                && NestedIndex == other.NestedIndex;
        }

        public override int GetHashCode()
        {
            // Use a different bit for bool fields: bool.GetHashCode() will return 0 (false) or 1 (true). So we would
            // end up having the same hash code for e.g. two instances where one has only noCache set and the other
            // only noStore.
            var result = Visible.GetHashCode()
                ^ ( BarVisible.GetHashCode() << 1 ); // increase shift by one for every bool field

            result = result
                ^ ( Mode.GetHashCode() ^ 1 ); // power of two for every other field(^1, ^2, ^4, ^8, ^16, ...)

            result = result
                ^ ( NestedIndex.GetHashCode() ^ 2 );

            return result;
        }

        public static bool operator ==( BarDropdownStore lhs, BarDropdownStore rhs )
        {
            return lhs.Equals( rhs );
        }

        public static bool operator !=( BarDropdownStore lhs, BarDropdownStore rhs )
        {
            return !lhs.Equals( rhs );
        }

        #endregion

        #region Properties

        public bool Visible { readonly get; set; }

        public BarMode Mode { readonly get; set; }

        public bool BarVisible { readonly get; set; }

        public int NestedIndex { readonly get; set; }

        public bool IsInlineDisplay => Mode == BarMode.VerticalInline && BarVisible;

        #endregion
    }
}