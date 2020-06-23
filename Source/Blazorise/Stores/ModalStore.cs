#region Using directives
using System;
using System.Collections.Generic;
using System.Resources;
using System.Text;
#endregion

namespace Blazorise.Stores
{
    public struct ModalStore : IEquatable<ModalStore>
    {
        #region Methods

        public override bool Equals( object obj )
            => obj is ModalStore store && Equals( store );

        public bool Equals( ModalStore other )
        {
            return Visible == other.Visible;
        }

        public override int GetHashCode()
        {
            var result = Visible.GetHashCode();

            return result;
        }

        public static bool operator ==( ModalStore lhs, ModalStore rhs )
        {
            return lhs.Equals( rhs );
        }

        public static bool operator !=( ModalStore lhs, ModalStore rhs )
        {
            return !lhs.Equals( rhs );
        }

        #endregion

        #region Properties

        public bool Visible { readonly get; set; }

        #endregion
    }
}
