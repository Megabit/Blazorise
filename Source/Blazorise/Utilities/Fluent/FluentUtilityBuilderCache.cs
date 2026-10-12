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
/// </remarks>
internal sealed class FluentUtilityBuilderCache<TUtility>
    where TUtility : class
{
    #region Members

    private const int MaximumOperations = 1024;

    private const int MaximumDataOperations = 256;

    private const int BucketCount = 256;

    private readonly object cacheLock = new();

    private readonly Entry[] operations = new Entry[BucketCount];

    private readonly Func<int, FluentUtilityState<TUtility>, TUtility> createUtility;

    private readonly StateAccessor getState;

    private int operationCount;

    private int dataOperationCount;

    public delegate ref FluentUtilityState<TUtility> StateAccessor( TUtility utility );

    private sealed record Entry( Transition Key, TUtility Utility, Entry Next );

    private readonly record struct Transition( int Parent, Action<TUtility, int, object> Apply, int Value, object Data )
    {
        private readonly int hashCode = HashCode.Combine( Parent, RuntimeHelpers.GetHashCode( Apply ), Value, Data );

        public bool Equals( Transition other )
            => Parent == other.Parent
                && ReferenceEquals( Apply, other.Apply )
                && Value == other.Value
                && Equals( Data, other.Data );

        public override int GetHashCode() => hashCode;
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
            var key = new Transition( state.Id, apply, value, data );
            var bucket = key.GetHashCode() & (BucketCount - 1);
            var cached = Find( Volatile.Read( ref operations[bucket] ), key );

            if ( cached is not null )
            {
                return cached;
            }

            // Chains with key data use a separate budget and ID range, including their descendants.
            var hasData = data is not null || state.Id > MaximumOperations;
            var maximumOperations = hasData ? MaximumDataOperations : MaximumOperations;
            ref var count = ref ( hasData ? ref dataOperationCount : ref operationCount );

            if ( Volatile.Read( ref count ) < maximumOperations )
            {
                lock ( cacheLock )
                {
                    cached = Find( operations[bucket], key );

                    if ( cached is not null )
                    {
                        return cached;
                    }

                    if ( count < maximumOperations )
                    {
                        var nextCount = count + 1;
                        var id = hasData ? MaximumOperations + nextCount : nextCount;
                        var utility = CreateTransition( parent, state, apply, value, data, target, operation, id );

                        // Immutable entries are published once; readers need no insertion lock.
                        Volatile.Write( ref operations[bucket], new( key, utility, operations[bucket] ) );
                        Volatile.Write( ref count, nextCount );

                        return utility;
                    }
                }
            }
        }

        return CreateTransition( parent, state, apply, value, data, target, operation, 0 );
    }

    public TUtility Build( TUtility utility )
    {
        ref var state = ref getState( utility );
        var builder = createUtility( state.Root, default );

        if ( state.Parent is null )
        {
            return builder;
        }

        var steps = ArrayPool<TUtility>.Shared.Rent( 16 );
        var count = 0;

        try
        {
            while ( state.Parent is not null )
            {
                if ( count == steps.Length )
                {
                    var expanded = ArrayPool<TUtility>.Shared.Rent( steps.Length * 2 );
                    Array.Copy( steps, expanded, count );
                    ArrayPool<TUtility>.Shared.Return( steps, clearArray: true );
                    steps = expanded;
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
            ArrayPool<TUtility>.Shared.Return( steps, clearArray: true );
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