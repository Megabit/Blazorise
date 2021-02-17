﻿#region Using directives
using System.Collections.Generic;
#endregion

namespace Blazorise.Utilities
{
    /// <summary>
    /// Compares two strings and find the diferences.
    /// </summary>
    public interface IValidationMessageLocalizerAttributeFinder
    {
        /// <summary>
        /// Find all the diferences from two string.
        /// </summary>
        /// <param name="first">First string.</param>
        /// <param name="second">Second string.</param>
        /// <returns>Return the list of differences if any is found</returns>
        IEnumerable<(string Index, string Argument)> FindAll( string first, string second );
    }

    public class ValidationMessageLocalizerAttributeFinder : IValidationMessageLocalizerAttributeFinder
    {
        #region Methods

        /// <inheritdoc/>
        public virtual IEnumerable<(string Index, string Argument)> FindAll( string first, string second )
        {
            var firstList = first?.Split( ' ' );
            var secondList = second?.Split( ' ' );

            if ( firstList != null && secondList != null && firstList.Length == secondList.Length )
            {
                for ( int i = 0; i < firstList.Length; ++i )
                {
                    if ( firstList[i] != secondList[i] )
                        yield return (secondList[i], firstList[i].Trim( '.' ));
                }
            }
        }

        #endregion
    }
}
