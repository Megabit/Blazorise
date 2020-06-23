#region Using directives
using System;
using System.Collections.Generic;
using System.Resources;
using System.Text;
#endregion

namespace Blazorise.Stores
{
    public struct TabsContentStore : IEquatable<TabsContentStore>
    {
        #region Methods

        public override bool Equals( object obj )
            => obj is TabsContentStore store && Equals( store );

        public bool Equals( TabsContentStore other )
        {
            return SelectedPanel == other.SelectedPanel;
        }

        public override int GetHashCode()
        {
            var result = SelectedPanel.GetHashCode();

            return result;
        }

        public static bool operator ==( TabsContentStore lhs, TabsContentStore rhs )
        {
            return lhs.Equals( rhs );
        }

        public static bool operator !=( TabsContentStore lhs, TabsContentStore rhs )
        {
            return !lhs.Equals( rhs );
        }

        #endregion

        #region Properties

        public string SelectedPanel { readonly get; set; }

        #endregion
    }
}
