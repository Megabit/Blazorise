#region Using directives
using System;
using System.Runtime.CompilerServices;
using System.Threading;
#endregion

namespace Blazorise.Utilities;

/// <summary>
/// Caches immutable utility output by provider without retaining shared providers.
/// </summary>
/// <remarks>
/// Keep this mutable struct in a field so cache publication operates on the original storage.
/// </remarks>
internal struct FluentUtilityResultCache<TProvider>
    where TProvider : class
{
    #region Members

    private ProviderResult firstResult;

    private Tuple<TProvider, string> uncachedResult;

    private sealed class ProviderResult : WeakReference
    {
        public ConditionalWeakTable<TProvider, string> AdditionalResults;

        public ProviderResult( TProvider provider, string result ) : base( provider )
        {
            Result = result;
        }

        public string Result { get; }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Returns output for the provider, using weak provider references for retained utilities.
    /// </summary>
    public string GetOrAdd<TUtility>( TUtility utility, TProvider provider, bool isCacheEntry, Func<TUtility, TProvider, string> buildResult )
        where TUtility : class
    {
        if ( !isCacheEntry || provider is null )
        {
            // Unretained nodes belong to the caller and need no weak-table allocation.
            var previous = Volatile.Read( ref uncachedResult );

            if ( previous is not null && ReferenceEquals( previous.Item1, provider ) )
            {
                return previous.Item2;
            }

            return AddUncachedResult( utility, provider, buildResult );
        }

        var first = Volatile.Read( ref firstResult );

        if ( first is not null
            && ReferenceEquals( first.Target, provider ) )
        {
            return first.Result;
        }

        return AddCachedResult( utility, provider, buildResult, first );
    }

    private string AddUncachedResult<TUtility>( TUtility utility, TProvider provider, Func<TUtility, TProvider, string> buildResult )
        where TUtility : class
    {
        var result = buildResult( utility, provider );
        Volatile.Write( ref uncachedResult, Tuple.Create( provider, result ) );

        return result;
    }

    private string AddCachedResult<TUtility>( TUtility utility, TProvider provider, Func<TUtility, TProvider, string> buildResult, ProviderResult first )
        where TUtility : class
    {
        if ( first is null )
        {
            // Most chains use one provider; allocate a weak table only for additional providers.
            var result = buildResult( utility, provider );
            var entry = new ProviderResult( provider, result );
            first = Interlocked.CompareExchange( ref firstResult, entry, null );

            if ( first is null )
            {
                return result;
            }

            if ( ReferenceEquals( first.Target, provider ) )
            {
                return first.Result;
            }
        }

        var cache = Volatile.Read( ref first.AdditionalResults );

        if ( cache is null )
        {
            cache = new();
            cache = Interlocked.CompareExchange( ref first.AdditionalResults, cache, null ) ?? cache;
        }

        if ( cache.TryGetValue( provider, out var cached ) )
        {
            return cached;
        }

        return AddProviderResult( cache, utility, provider, buildResult );
    }

    private static string AddProviderResult<TUtility>( ConditionalWeakTable<TProvider, string> cache, TUtility utility, TProvider provider, Func<TUtility, TProvider, string> buildResult )
        where TUtility : class
        => cache.GetValue( provider, currentProvider => buildResult( utility, currentProvider ) );

    #endregion
}