#region Using directives
using System;
using System.Threading;
#endregion

namespace Blazorise.Utils
{
    /// <summary>
    /// Inspired by <see href="https://github.com/aspnet/KestrelHttpServer/blob/6fde01a825cffc09998d3f8a49464f7fbe40f9c4/src/Kestrel.Core/Internal/Infrastructure/CorrelationIdGenerator.cs"/>,
    /// this class generates an efficient ID which is the <c>base32</c> encoded version of a <see cref="long"/>
    /// using the alphabet <c>1-9</c> and <c>A-V</c>.
    /// </summary>
    public sealed class IDGenerator
    {
        private const string encode_32_Chars = "0123456789ABCDEFGHIJKLMNOPQRSTUV";
        private static long _lastId = DateTime.UtcNow.Ticks;

        private static readonly ThreadLocal<char[]> _charBufferThreadLocal = new ThreadLocal<char[]>( () => new char[13] );

        static IDGenerator() { }
        private IDGenerator() { }

        /// <summary>
        /// Returns a single instance of the <see cref="IDGenerator"/>.
        /// </summary>
        public static IDGenerator Instance { get; } = new IDGenerator();

        /// <summary>
        /// Returns and ID. e.g: <c>0HLH7Q6V92BQE</c>
        /// </summary>
        public string Generate => GenerateImpl( Interlocked.Increment( ref _lastId ) );

        private static string GenerateImpl( long id )
        {
            var buffer = _charBufferThreadLocal.Value;

            buffer[0] = encode_32_Chars[(int)( id >> 60 ) & 31];
            buffer[1] = encode_32_Chars[(int)( id >> 55 ) & 31];
            buffer[2] = encode_32_Chars[(int)( id >> 50 ) & 31];
            buffer[3] = encode_32_Chars[(int)( id >> 45 ) & 31];
            buffer[4] = encode_32_Chars[(int)( id >> 40 ) & 31];
            buffer[5] = encode_32_Chars[(int)( id >> 35 ) & 31];
            buffer[6] = encode_32_Chars[(int)( id >> 30 ) & 31];
            buffer[7] = encode_32_Chars[(int)( id >> 25 ) & 31];
            buffer[8] = encode_32_Chars[(int)( id >> 20 ) & 31];
            buffer[9] = encode_32_Chars[(int)( id >> 15 ) & 31];
            buffer[10] = encode_32_Chars[(int)( id >> 10 ) & 31];
            buffer[11] = encode_32_Chars[(int)( id >> 5 ) & 31];
            buffer[12] = encode_32_Chars[(int)id & 31];

            return new string( buffer, 0, buffer.Length );
        }
    }
}
