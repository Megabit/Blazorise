#region Using directives
using System;
using System.Collections.Generic;
using System.Resources;
using System.Text;
#endregion

namespace Blazorise.Stores
{
    public struct DropdownStore : IEquatable<DropdownStore>
    {
        #region Methods

        public override bool Equals( object obj )
            => obj is DropdownStore store && Equals( store );

        public bool Equals( DropdownStore other )
        {
            return Visible == other.Visible
                && RightAligned == other.RightAligned
                && Disabled == other.Disabled
                && Direction == other.Direction;
        }

        public override int GetHashCode()
        {
            // Use a different bit for bool fields: bool.GetHashCode() will return 0 (false) or 1 (true). So we would
            // end up having the same hash code for e.g. two instances where one has only noCache set and the other
            // only noStore.
            var result = Visible.GetHashCode()
             ^ ( RightAligned.GetHashCode() << 1 )
             ^ ( Disabled.GetHashCode() << 2 ); // increase shift by one for every bool field

            result = result
                ^ ( Direction.GetHashCode() ^ 1 ); // power of two for every other field(^1, ^2, ^4, ^8, ^16, ...)

            return result;
        }

        public static bool operator ==( DropdownStore lhs, DropdownStore rhs )
        {
            return lhs.Equals( rhs );
        }

        public static bool operator !=( DropdownStore lhs, DropdownStore rhs )
        {
            return !lhs.Equals( rhs );
        }

        #endregion

        #region Properties

        public bool Visible { readonly get; set; }

        public bool RightAligned { readonly get; set; }

        public bool Disabled { readonly get; set; }

        public Direction Direction { readonly get; set; }

        #endregion
    }
}
