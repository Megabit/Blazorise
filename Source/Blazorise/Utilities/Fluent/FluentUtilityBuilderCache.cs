#region Using directives
using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Threading;
#endregion

namespace Blazorise.Utilities;

/// <summary>
/// Reuses immutable fluent transitions and replays their original builder operations on demand.
/// </summary>
/// <remarks>
/// Operation callbacks must be reused static delegates; optional key data must be immutable.
/// Numeric key data stays unboxed during lookup and is materialized only on a miss.
/// </remarks>
internal sealed class FluentUtilityBuilderCache<TUtility>
    where TUtility : class
{
    #region Members

    private const int MaximumOperations = 1024;

    private const int MaximumDataOperations = 256;

    private const int BucketCount = 512;

    private readonly object cacheLock = new();

    private readonly Entry[] operations = new Entry[BucketCount];

    private readonly Func<int, FluentUtilityState<TUtility>, TUtility> createUtility;

    private readonly StateAccessor getState;

    private int operationCount;

    private int dataOperationCount;

    public delegate ref FluentUtilityState<TUtility> StateAccessor( TUtility utility );

    private sealed record Entry( Transition Key, TUtility Utility, Entry Next );

    private enum DataKind
    {
        Object,
        Number,
        NumberWithUnit,
    }

    [InlineArray( 8 )]
    private struct OperationBuffer
    {
        private TUtility element;
    }

    private readonly record struct Transition( int Parent, Action<TUtility, int, object> Apply, int Value, object Data, long Number = 0, DataKind Kind = DataKind.Object )
    {
        private readonly int hashCode = CreateHashCode( Parent, Apply, Value, Data, Number, Kind );

        private static int CreateHashCode( int parent, Action<TUtility, int, object> apply, int value, object data, long number, DataKind kind )
        {
            const int FieldMultiplier = 397;
            const int MixingMultiplier = 0x7feb352d;

            // Common enum steps need only parent, callback identity and value.
            var hash = unchecked( ( parent * FieldMultiplier ^ RuntimeHelpers.GetHashCode( apply ) ) * FieldMultiplier ^ value );

            if ( data is not null )
            {
                hash = unchecked( hash * FieldMultiplier ^ data.GetHashCode() );
            }

            if ( kind != DataKind.Object )
            {
                hash = unchecked( ( hash * FieldMultiplier ^ (int)number ) * FieldMultiplier ^ (int)( number >> 32 ) ^ (int)kind );
            }

            // Numeric bits can be concentrated in the upper half; include them in the bucket index.
            hash = unchecked( ( hash ^ ( hash >>> 16 ) ) * MixingMultiplier );
            return hash ^ ( hash >>> 16 );
        }

        public bool Equals( Transition other )
            => Parent == other.Parent
                && ReferenceEquals( Apply, other.Apply )
                && Value == other.Value
                && Number == other.Number
                && Kind == other.Kind
                && Equals( Data, other.Data );

        public override int GetHashCode() => hashCode;

        public object GetData()
            => Kind switch
            {
                DataKind.Number => Number,
                DataKind.NumberWithUnit => ((string)Data, Number),
                _ => Data,
            };

        public bool HasData => Data is not null || Kind != DataKind.Object;
    }

    #endregion

    #region Constructors

    public FluentUtilityBuilderCache( Func<int, FluentUtilityState<TUtility>, TUtility> createUtility, StateAccessor getState )
    {
        this.createUtility = createUtility;
        this.getState = getState;
    }

    #endregion

    #region Methods

    public TUtility CreateRoot( int root = 1 ) => createUtility( root, new( root, -root ) );

    public TUtility With( TUtility parent, in FluentUtilityState<TUtility> state, Action<TUtility, int, object> apply, int value = 0, object data = null, bool retain = true, UtilityTarget? target = null, [CallerMemberName] string operation = null )
    {
        if ( retain && state.IsCacheEntry )
        {
            return GetOrAdd( parent, state, new( state.Id, apply, value, data ), target, operation );
        }

        return CreateTransition( parent, state, apply, value, data, target, operation, 0 );
    }

    public TUtility With( TUtility parent, in FluentUtilityState<TUtility> state, Action<TUtility, int, object> apply, long data, [CallerMemberName] string operation = null )
    {
        if ( state.IsCacheEntry )
        {
            return GetOrAdd( parent, state, new( state.Id, apply, 0, null, data, DataKind.Number ), null, operation );
        }

        return CreateTransition( parent, state, apply, 0, data, null, operation, 0 );
    }

    public TUtility With( TUtility parent, in FluentUtilityState<TUtility> state, Action<TUtility, int, object> apply, (string Unit, long Number) data, [CallerMemberName] string operation = null )
    {
        if ( state.IsCacheEntry )
        {
            return GetOrAdd( parent, state, new( state.Id, apply, 0, data.Unit, data.Number, DataKind.NumberWithUnit ), null, operation );
        }

        return CreateTransition( parent, state, apply, 0, data, null, operation, 0 );
    }

    private TUtility GetOrAdd( TUtility parent, in FluentUtilityState<TUtility> state, in Transition key, UtilityTarget? target, string operation )
    {
        var bucket = key.GetHashCode() & (BucketCount - 1);
        var cached = Find( Volatile.Read( ref operations[bucket] ), key );

        return cached ?? AddTransition( parent, state, key, bucket, target, operation );
    }

    private TUtility AddTransition( TUtility parent, in FluentUtilityState<TUtility> state, in Transition key, int bucket, UtilityTarget? target, string operation )
    {
        // Chains with key data use a separate budget and ID range, including their descendants.
        var hasData = key.HasData || state.Id > MaximumOperations;
        var maximumOperations = hasData ? MaximumDataOperations : MaximumOperations;
        ref var count = ref ( hasData ? ref dataOperationCount : ref operationCount );

        if ( Volatile.Read( ref count ) < maximumOperations )
        {
            lock ( cacheLock )
            {
                var cached = Find( operations[bucket], key );

                if ( cached is not null )
                {
                    return cached;
                }

                if ( count < maximumOperations )
                {
                    var nextCount = count + 1;
                    var id = hasData ? MaximumOperations + nextCount : nextCount;
                    var utility = CreateTransition( parent, state, key.Apply, key.Value, key.GetData(), target, operation, id );

                    // Immutable entries are published once; readers need no insertion lock.
                    Volatile.Write( ref operations[bucket], new( key, utility, operations[bucket] ) );
                    Volatile.Write( ref count, nextCount );

                    return utility;
                }
            }
        }

        return CreateTransition( parent, state, key.Apply, key.Value, key.GetData(), target, operation, 0 );
    }

    public TUtility Build( TUtility utility )
    {
        ref var state = ref getState( utility );
        var builder = createUtility( state.Root, default );

        if ( state.Parent is null )
        {
            return builder;
        }

        var buffer = default( OperationBuffer );
        Span<TUtility> steps = buffer;
        TUtility[] rentedSteps = null;
        var count = 0;

        try
        {
            while ( state.Parent is not null )
            {
                if ( count == steps.Length )
                {
                    var expanded = ArrayPool<TUtility>.Shared.Rent( steps.Length * 2 );
                    steps.CopyTo( expanded );

                    if ( rentedSteps is not null )
                    {
                        ArrayPool<TUtility>.Shared.Return( rentedSteps, clearArray: true );
                    }

                    rentedSteps = expanded;
                    steps = rentedSteps;
                }

                steps[count++] = utility;
                utility = state.Parent;
                state = ref getState( utility );
            }

            for ( var index = count - 1; index >= 0; index-- )
            {
                state = ref getState( steps[index] );
                state.Apply( builder, state.Value, state.Data );
            }

            return builder;
        }
        finally
        {
            if ( rentedSteps is not null )
            {
                ArrayPool<TUtility>.Shared.Return( rentedSteps, clearArray: true );
            }
        }
    }

    public bool HasOperation( TUtility utility, string operation, int? excludedValue = null )
    {
        ref var state = ref getState( utility );

        while ( state.Parent is not null )
        {
            if ( state.Operation == operation && state.Data is null && state.Value != excludedValue )
            {
                return true;
            }

            state = ref getState( state.Parent );
        }

        return false;
    }

    public object GetData( TUtility utility, Func<FluentUtilityState<TUtility>, bool> matches )
    {
        ref var state = ref getState( utility );

        while ( state.Parent is not null )
        {
            if ( matches( state ) )
            {
                return state.Data;
            }

            state = ref getState( state.Parent );
        }

        return null;
    }

    private TUtility CreateTransition( TUtility parent, in FluentUtilityState<TUtility> previous, Action<TUtility, int, object> apply, int value, object data, UtilityTarget? target, string operation, int id )
        => createUtility( previous.Root, new( previous.Root, id, parent, apply, value, data, target ?? previous.Target, operation ) );

    private static TUtility Find( Entry entry, in Transition key )
    {
        while ( entry is not null )
        {
            if ( entry.Key.GetHashCode() == key.GetHashCode() && entry.Key.Equals( key ) )
            {
                return entry.Utility;
            }

            entry = entry.Next;
        }

        return null;
    }

    #endregion
}