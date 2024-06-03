#region Using directives
using System;
using System.Buffers;
using System.Threading;
#endregion

namespace Blazorise.Utilities;

/// <summary>
/// Inspired by <see href="https://github.com/aspnet/KestrelHttpServer/blob/6fde01a825cffc09998d3f8a49464f7fbe40f9c4/src/Kestrel.Core/Internal/Infrastructure/CorrelationIdGenerator.cs"/>,
/// this class generates an efficient ID which is the <c>base32</c> encoded version of a <see cref="long"/>
/// using the alphabet <c>1-9</c> and <c>A-V</c>.
/// </summary>
public sealed class IdGenerator : IIdGenerator
{
    #region Members

    private const int IdLength = 13;

    private static long LastId = DateTime.UtcNow.Ticks;

    private static readonly SpanAction<char, long> GenerateImplDelegate = GenerateImpl;

    #endregion

    #region Methods

    private static void GenerateImpl( Span<char> buffer, long id )
    {
        var Encode32Chars = "0123456789ABCDEFGHIJKLMNOPQRSTUV";
        // Accessing the last item in the beginning elides range checks for all the subsequent items.
        buffer[12] = Encode32Chars[(int)id & 31];
        buffer[0] = Encode32Chars[(int)( id >> 60 ) & 31];
        buffer[1] = Encode32Chars[(int)( id >> 55 ) & 31];
        buffer[2] = Encode32Chars[(int)( id >> 50 ) & 31];
        buffer[3] = Encode32Chars[(int)( id >> 45 ) & 31];
        buffer[4] = Encode32Chars[(int)( id >> 40 ) & 31];
        buffer[5] = Encode32Chars[(int)( id >> 35 ) & 31];
        buffer[6] = Encode32Chars[(int)( id >> 30 ) & 31];
        buffer[7] = Encode32Chars[(int)( id >> 25 ) & 31];
        buffer[8] = Encode32Chars[(int)( id >> 20 ) & 31];
        buffer[9] = Encode32Chars[(int)( id >> 15 ) & 31];
        buffer[10] = Encode32Chars[(int)( id >> 10 ) & 31];
        buffer[11] = Encode32Chars[(int)( id >> 5 ) & 31];
    }

    #endregion

    #region Properties

    /// <summary>
    /// Returns a random ID. e.g: <c>0HLH7Q6V92BQE</c>
    /// </summary>
    public string Generate
    {
        get
        {
            var id = Interlocked.Increment( ref LastId );
            return string.Create( IdLength, id, GenerateImplDelegate );
        }
    }

    #endregion
}