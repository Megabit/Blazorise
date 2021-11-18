using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using BenchmarkDotNet.Attributes;
using Blazorise.DataGrid.Utils;
using Blazorise.Themes;

namespace Blazorise.Benchmark.Blazorise
{
    [MemoryDiagnoser]
    public class ThemeBenchmark
    {
        //IThemeGenerator _themeGenerator = new Bootstrap5.BootstrapThemeGenerator( new ThemeCache( new BlazoriseOptions( null, null ) ) );
        //private Theme theme = new() { };

        //[Benchmark]
        //public void GenerateStyles()
        //    => _themeGenerator.GenerateStyles( theme );

        //[Benchmark]
        //public void GenerateVariables()
        //    => _themeGenerator.GenerateVariables( theme );


        [Benchmark]
        public void ExtractHexDigitsBenchmark()
            => ExtractHexDigits( "#ffffff" );

        [Benchmark]
        public void CachedExtractHexDigitsBenchmark()
            => CachedExtractHexDigits( "#ffffff" );


        [Benchmark]
        public void CachedExtractHexDigitsAllocateOnlyOnceSpanBenchmark()
            => CachedExtractHexDigitsAllocateOnlyOnce( "#ffffff" );

        [Benchmark]
        public void CachedExtractHexDigitsMatches()
            => CachedExtractHexDigitsMatches( "#ffffff" );

        /// <summary>
        /// Extract only the hex digits from a string.
        /// </summary>
        /// <param name="input">A string to extract.</param>
        /// <returns>A new hex string.</returns>
        protected static string ExtractHexDigits( string input )
        {
            // remove any characters that are not digits (like #)
            Regex isHexDigit = new( "[abcdefABCDEF\\d]+", RegexOptions.Compiled );
            string newnum = "";
            foreach ( char c in input )
            {
                if ( isHexDigit.IsMatch( c.ToString() ) )
                    newnum += c.ToString();
            }
            return newnum;
        }

        /// <summary>
        /// Extract only the hex digits from a string.
        /// </summary>
        /// <param name="input">A string to extract.</param>
        /// <returns>A new hex string.</returns>
        protected static string CachedExtractHexDigits( string input )
        {
            string newnum = "";
            foreach ( char c in input )
            {
                if ( isHexDigit.IsMatch( c.ToString() ) )
                    newnum += c.ToString();
            }
            return newnum;
        }

        /// <summary>
        /// Extract only the hex digits from a string.
        /// </summary>
        /// <param name="input">A string to extract.</param>
        /// <returns>A new hex string.</returns>
        protected static string CachedExtractHexDigitsAllocateOnlyOnce( string input )
        {
            string newnum = "";
            foreach ( char c in input )
            {
                var charAsString = c.ToString();
                if ( isHexDigit.IsMatch( charAsString ) )
                    newnum += charAsString;
            }

            return newnum;
        }


        /// <summary>
        /// Extract only the hex digits from a string.
        /// </summary>
        /// <param name="input">A string to extract.</param>
        /// <returns>A new hex string.</returns>
        //Need to setup tests with this to see if it's equivalent to the previous versions.
        //Test values:
        //#ffffff
        //#
        //no value
        //null
        //#12321321
        //213123
        //DFDASfçpqope
        protected static string CachedExtractHexDigitsMatches( string input )
        {
            var result = isHexDigit.Matches( input );
            return result.ToString(); 
        }



        private static Regex isHexDigit = new( "[abcdefABCDEF\\d]+", RegexOptions.Compiled );

    }

}