#region Using directives
using System;
using System.Collections.Generic;
using System.Resources;
using System.Text;
#endregion

namespace Blazorise.Stores
{
    public struct AlertStore : IEquatable<AlertStore>
    {
        #region Methods

        public override bool Equals( object obj )
            => obj is AlertStore store && Equals( store );

        public bool Equals( AlertStore other )
        {
            return Dismisable == other.Dismisable
                && Visible == other.Visible
                && Color == other.Color;
        }

        public override int GetHashCode()
        {
            // Use a different bit for bool fields: bool.GetHashCode() will return 0 (false) or 1 (true). So we would
            // end up having the same hash code for e.g. two instances where one has only noCache set and the other
            // only noStore.
            var result = Dismisable.GetHashCode()
             ^ ( Visible.GetHashCode() << 1 ); // increase shift by one for every bool field

            result = result
               ^ ( Color.GetHashCode() ^ 1 ); // power of two for every other field(^1, ^2, ^4, ^8, ^16, ...)

            return result;
        }

        public static bool operator ==( AlertStore lhs, AlertStore rhs )
        {
            return lhs.Equals( rhs );
        }

        public static bool operator !=( AlertStore lhs, AlertStore rhs )
        {
            return !lhs.Equals( rhs );
        }

        #endregion

        #region Properties

        public bool Dismisable { readonly get; set; }

        public bool Visible { readonly get; set; }

        public Color Color { readonly get; set; }

        #endregion
    }
}
