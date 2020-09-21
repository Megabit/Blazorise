#region Using directives
using System;
using System.Collections.Generic;
using System.Resources;
using System.Text;
#endregion

namespace Blazorise.Stores
{
    public struct BarStore : IEquatable<BarStore>
    {
        #region Methods

        public override bool Equals( object obj )
            => obj is BarStore store && Equals( store );

        public bool Equals( BarStore other )
        {
            return Visible == other.Visible
                && Mode == other.Mode
                && CollapseMode == other.CollapseMode;
        }

        public override int GetHashCode()
        {
            // Use a different bit for bool fields: bool.GetHashCode() will return 0 (false) or 1 (true). So we would
            // end up having the same hash code for e.g. two instances where one has only noCache set and the other
            // only noStore.
            var result = Visible.GetHashCode();

            result = result
                ^ ( Mode.GetHashCode() ^ 1 ); // power of two for every other field(^1, ^2, ^4, ^8, ^16, ...)

            result = result
                ^ ( CollapseMode.GetHashCode() ^ 2 );

            return result;
        }

        public static bool operator ==( BarStore lhs, BarStore rhs )
        {
            return lhs.Equals( rhs );
        }

        public static bool operator !=( BarStore lhs, BarStore rhs )
        {
            return !lhs.Equals( rhs );
        }

        #endregion

        #region Properties

        public bool Visible { readonly get; set; }

        public BarMode Mode { readonly get; set; }

        public BarCollapseMode CollapseMode { readonly get; set; }

        #endregion
    }
}