#region Using directives
using System;
using System.Collections.Generic;
using System.Resources;
using System.Text;
#endregion

namespace Blazorise.Stores
{
    public struct ListGroupStore : IEquatable<ListGroupStore>
    {
        #region Methods

        public override bool Equals( object obj )
            => obj is ListGroupStore store && Equals( store );

        public bool Equals( ListGroupStore other )
        {
            return Flush == other.Flush
                && Mode == other.Mode
                && SelectedItem == other.SelectedItem;
        }

        public override int GetHashCode()
        {
            var result = Flush.GetHashCode();

            result = result
               ^ ( Mode.GetHashCode() ^ 1 ); // power of two for every other field(^1, ^2, ^4, ^8, ^16, ...)

            if ( SelectedItem != null )
                result ^= SelectedItem.GetHashCode();

            return result;
        }

        public static bool operator ==( ListGroupStore lhs, ListGroupStore rhs )
        {
            return lhs.Equals( rhs );
        }

        public static bool operator !=( ListGroupStore lhs, ListGroupStore rhs )
        {
            return !lhs.Equals( rhs );
        }

        #endregion

        #region Properties

        public bool Flush { readonly get; set; }

        public ListGroupMode Mode { readonly get; set; }

        public string SelectedItem { readonly get; set; }

        #endregion
    }
}
