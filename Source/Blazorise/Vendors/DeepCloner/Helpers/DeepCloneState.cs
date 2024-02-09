using System;
using System.Runtime.CompilerServices;

namespace Blazorise.DeepCloner.Helpers;

internal class DeepCloneState
{
    private MiniDictionary _loops;

    private readonly object[] _baseFromTo = new object[6];

    private int _idx;

    public object GetKnownRef( object from )
    {
        // this is faster than call Dictionary from begin
        // also, small poco objects does not have a lot of references
        var baseFromTo = _baseFromTo;
        if ( ReferenceEquals( from, baseFromTo[0] ) )
            return baseFromTo[3];
        if ( ReferenceEquals( from, baseFromTo[1] ) )
            return baseFromTo[4];
        if ( ReferenceEquals( from, baseFromTo[2] ) )
            return baseFromTo[5];
        if ( _loops == null )
            return null;

        return _loops.FindEntry( from );
    }

    public void AddKnownRef( object from, object to )
    {
        if ( _idx < 3 )
        {
            _baseFromTo[_idx] = from;
            _baseFromTo[_idx + 3] = to;
            _idx++;
            return;
        }

        if ( _loops == null )
            _loops = new MiniDictionary();
        _loops.Insert( from, to );
    }

    private class MiniDictionary
    {
        private struct Entry
        {
            public int HashCode;
            public int Next;
            public object Key;
            public object Value;
        }

        private int[] _buckets;
        private Entry[] _entries;
        private int _count;


        public MiniDictionary() : this( 5 )
        {
        }

        public MiniDictionary( int capacity )
        {
            if ( capacity > 0 )
                Initialize( capacity );
        }

        public object FindEntry( object key )
        {
            if ( _buckets != null )
            {
                var hashCode = RuntimeHelpers.GetHashCode( key ) & 0x7FFFFFFF;
                var entries1 = _entries;
                for ( var i = _buckets[hashCode % _buckets.Length]; i >= 0; i = entries1[i].Next )
                {
                    if ( entries1[i].HashCode == hashCode && ReferenceEquals( entries1[i].Key, key ) )
                        return entries1[i].Value;
                }
            }

            return null;
        }

        private static readonly int[] _primes =
        {
                3, 7, 11, 17, 23, 29, 37, 47, 59, 71, 89, 107, 131, 163, 197, 239, 293, 353, 431, 521, 631, 761, 919,
                1103, 1327, 1597, 1931, 2333, 2801, 3371, 4049, 4861, 5839, 7013, 8419, 10103, 12143, 14591,
                17519, 21023, 25229, 30293, 36353, 43627, 52361, 62851, 75431, 90523, 108631, 130363, 156437,
                187751, 225307, 270371, 324449, 389357, 467237, 560689, 672827, 807403, 968897, 1162687, 1395263,
                1674319, 2009191, 2411033, 2893249, 3471899, 4166287, 4999559, 5999471, 7199369
            };

        private static int GetPrime( int min )
        {
            for ( var i = 0; i < _primes.Length; i++ )
            {
                var prime = _primes[i];
                if ( prime >= min )
                    return prime;
            }

            //outside of our predefined table. 
            //compute the hard way. 
            for ( var i = min | 1; i < int.MaxValue; i += 2 )
            {
                if ( IsPrime( i ) && ( i - 1 ) % 101 != 0 )
                    return i;
            }

            return min;
        }

        private static bool IsPrime( int candidate )
        {
            if ( ( candidate & 1 ) != 0 )
            {
                var limit = (int)Math.Sqrt( candidate );
                for ( var divisor = 3; divisor <= limit; divisor += 2 )
                {
                    if ( candidate % divisor == 0 )
                        return false;
                }

                return true;
            }

            return candidate == 2;
        }

        private static int ExpandPrime( int oldSize )
        {
            var newSize = 2 * oldSize;

            if ( (uint)newSize > 0x7FEFFFFD && 0x7FEFFFFD > oldSize )
            {
                return 0x7FEFFFFD;
            }

            return GetPrime( newSize );
        }

        private void Initialize( int size )
        {
            _buckets = new int[size];
            for ( int i = 0; i < _buckets.Length; i++ )
                _buckets[i] = -1;
            _entries = new Entry[size];
        }

        public void Insert( object key, object value )
        {
            if ( _buckets == null )
                Initialize( 0 );
            var hashCode = RuntimeHelpers.GetHashCode( key ) & 0x7FFFFFFF;
            var targetBucket = hashCode % _buckets.Length;

            var entries1 = _entries;

            // we're always checking for entry before adding new
            // so this loop is useless
            /*for (var i = _buckets[targetBucket]; i >= 0; i = entries1[i].Next)
            {
                if (entries1[i].HashCode == hashCode && ReferenceEquals(entries1[i].Key, key))
                {
                    entries1[i].Value = value;
                    return;
                }
            }*/

            if ( _count == entries1.Length )
            {
                Resize();
                entries1 = _entries;
                targetBucket = hashCode % _buckets.Length;
            }

            var index = _count;
            _count++;

            entries1[index].HashCode = hashCode;
            entries1[index].Next = _buckets[targetBucket];
            entries1[index].Key = key;
            entries1[index].Value = value;
            _buckets[targetBucket] = index;
        }

        private void Resize()
        {
            Resize( ExpandPrime( _count ) );
        }

        private void Resize( int newSize )
        {
            var newBuckets = new int[newSize];
            for ( int i = 0; i < newBuckets.Length; i++ )
                newBuckets[i] = -1;
            var newEntries = new Entry[newSize];
            Array.Copy( _entries, 0, newEntries, 0, _count );

            for ( var i = 0; i < _count; i++ )
            {
                if ( newEntries[i].HashCode >= 0 )
                {
                    var bucket = newEntries[i].HashCode % newSize;
                    newEntries[i].Next = newBuckets[bucket];
                    newBuckets[bucket] = i;
                }
            }

            _buckets = newBuckets;
            _entries = newEntries;
        }
    }
}