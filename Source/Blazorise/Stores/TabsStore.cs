#region Using directives
using System;
using System.Collections.Generic;
using System.Resources;
using System.Text;
#endregion

namespace Blazorise.Stores
{
    public struct TabsStore : IEquatable<TabsStore>
    {
        #region Methods

        public override bool Equals( object obj )
            => obj is TabsStore store && Equals( store );

        public bool Equals( TabsStore other )
        {
            return Pills == other.Pills
                && FullWidth == other.FullWidth
                && Justified == other.Justified
                && TabPosition == other.TabPosition
                && SelectedTab == other.SelectedTab;
        }

        public override int GetHashCode()
        {
            // Use a different bit for bool fields: bool.GetHashCode() will return 0 (false) or 1 (true). So we would
            // end up having the same hash code for e.g. two instances where one has only noCache set and the other
            // only noStore.
            var result = Pills.GetHashCode()
             ^ ( FullWidth.GetHashCode() << 1 )
             ^ ( Justified.GetHashCode() << 2 ); // increase shift by one for every bool field

            result = result
                ^ ( TabPosition.GetHashCode() ^ 1 ); // power of two for every other field(^1, ^2, ^4, ^8, ^16, ...)

            if ( SelectedTab != null )
                result ^= SelectedTab.GetHashCode();

            return result;
        }

        public static bool operator ==( TabsStore lhs, TabsStore rhs )
        {
            return lhs.Equals( rhs );
        }

        public static bool operator !=( TabsStore lhs, TabsStore rhs )
        {
            return !lhs.Equals( rhs );
        }

        #endregion

        #region Properties

        public bool Pills { readonly get; set; }

        public bool FullWidth { readonly get; set; }

        public bool Justified { readonly get; set; }

        public TabPosition TabPosition { readonly get; set; }

        public string SelectedTab { readonly get; set; }

        #endregion
    }
}
