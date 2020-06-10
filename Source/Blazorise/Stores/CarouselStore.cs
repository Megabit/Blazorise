#region Using directives
using System;
using System.Collections.Generic;
using System.Resources;
using System.Text;
#endregion

namespace Blazorise.Stores
{
    public struct CarouselStore : IEquatable<CarouselStore>
    {
        #region Methods

        public override bool Equals( object obj )
            => obj is CarouselStore store && Equals( store );

        public bool Equals( CarouselStore other )
        {
            return Autoplay == other.Autoplay
                && Crossfade == other.Crossfade
                && CurrentSlide == other.CurrentSlide;
        }

        public override int GetHashCode()
        {
            // Use a different bit for bool fields: bool.GetHashCode() will return 0 (false) or 1 (true). So we would
            // end up having the same hash code for e.g. two instances where one has only noCache set and the other
            // only noStore.
            var result = Autoplay.GetHashCode()
                ^ ( Crossfade.GetHashCode() << 1 ); // increase shift by one for every bool field

            if ( CurrentSlide != null )
                result ^= CurrentSlide.GetHashCode();

            return result;
        }

        public static bool operator ==( CarouselStore lhs, CarouselStore rhs )
        {
            return lhs.Equals( rhs );
        }

        public static bool operator !=( CarouselStore lhs, CarouselStore rhs )
        {
            return !lhs.Equals( rhs );
        }

        #endregion

        #region Properties

        public bool Autoplay { readonly get; set; }

        public bool Crossfade { readonly get; set; }

        public string CurrentSlide { readonly get; set; }

        #endregion
    }
}
