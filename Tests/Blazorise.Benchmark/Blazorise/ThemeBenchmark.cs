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

        [Benchmark]
        public void ExtractHexDigitsBenchmark()
            => ExtractHexDigits( "#ffffff" );

        [Benchmark]
        public void CachedExtractHexDigitsBenchmark()
            => CachedExtractHexDigits( "#ffffff" );


        [Benchmark]
        public void CachedExtractHexDigitsAllocateOnlyOnceBenchmark()
            => CachedExtractHexDigitsAllocateOnlyOnce( "#ffffff" );

        [Benchmark]
        public void CachedExtractHexDigitsMatchesBenchmark()
            => CachedExtractHexDigitsMatches( "#ffffff" );

        [Benchmark]
        public void ExtractHexDigitsStaticBenchmark()
            => ExtractHexDigitsStatic( "#ffffff" );

        [Benchmark]
        public void CachedExtractHexDigitsMatchesStringBuilderBenchmark()
            => CachedExtractHexDigitsMatchesStringBuilder( "#ffffff" );

        [Benchmark]
        public void CachedExtractHexDigitsMatchesNoCompiledBenchmark()
             => CachedExtractHexDigitsMatchesNoCompiled( "#ffffff" );

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

        protected static string ExtractHexDigitsStatic( string input )
        {
            string newnum = "";
            foreach ( char c in input )
            {
                if ( Regex.IsMatch( c.ToString(), pattern ) )
                    newnum += c.ToString();
            }
            return newnum;
        }

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


        protected static string CachedExtractHexDigitsMatches( string input )
        {
            string newnum = string.Empty;
            var result = isHexDigit.Matches( input );
            foreach ( System.Text.RegularExpressions.Match item in result )
            {
                newnum += item.Value;
            }
            return newnum;
        }

        protected static string CachedExtractHexDigitsMatchesNoCompiled( string input )
        {
            string newnum = string.Empty;
            var result = isHexDigitNoCompiled.Matches( input );
            foreach ( System.Text.RegularExpressions.Match item in result )
            {
                newnum += item.Value;
            }
            return newnum;
        }

        protected static string CachedExtractHexDigitsMatchesStringBuilder( string input )
        {
            var sb = new StringBuilder( string.Empty );
            var result = isHexDigit.Matches( input );
            foreach ( System.Text.RegularExpressions.Match item in result )
            {
                sb.Append( item.Value );
            }
            return sb.ToString();
        }

        private static readonly string pattern = "[abcdefABCDEF\\d]+";
        private static Regex isHexDigit = new( "[abcdefABCDEF\\d]+", RegexOptions.Compiled );
        private static Regex isHexDigitNoCompiled = new( "[abcdefABCDEF\\d]+" );

    }

}