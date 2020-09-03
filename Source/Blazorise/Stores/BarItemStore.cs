#region Using directives
using System;
using System.Collections.Generic;
using System.Resources;
using System.Text;
#endregion

namespace Blazorise.Stores
{
    public struct BarItemStore : IEquatable<BarItemStore>
    {
        #region Methods

        public override bool Equals( object obj )
            => obj is BarItemStore store && Equals( store );

        public bool Equals( BarItemStore other )
        {
            return Active == other.Active
                && Disabled == other.Disabled
                && Mode == other.Mode
                && BarVisible == other.BarVisible;
        }

        public override int GetHashCode()
        {
            // Use a different bit for bool fields: bool.GetHashCode() will return 0 (false) or 1 (true). So we would
            // end up having the same hash code for e.g. two instances where one has only noCache set and the other
            // only noStore.
            var result = Active.GetHashCode()
                ^ ( Disabled.GetHashCode() << 1 )
                ^ ( BarVisible.GetHashCode() << 2 ); // increase shift by one for every bool field

            result = result
                ^ ( Mode.GetHashCode() ^ 1 ); // power of two for every other field(^1, ^2, ^4, ^8, ^16, ...)

            return result;
        }

        public static bool operator ==( BarItemStore lhs, BarItemStore rhs )
        {
            return lhs.Equals( rhs );
        }

        public static bool operator !=( BarItemStore lhs, BarItemStore rhs )
        {
            return !lhs.Equals( rhs );
        }

        #endregion

        #region Properties

        public bool Active { readonly get; set; }

        public bool Disabled { readonly get; set; }

        public BarMode Mode { readonly get; set; }

        public bool BarVisible { readonly get; set; }

        #endregion
    }
}