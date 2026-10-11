#region Using directives
using System;
using System.Threading;
#endregion

namespace Blazorise.Utilities;

/// <summary>
/// Stores one ordered fluent step and its cached outputs inline in the utility.
/// </summary>
/// <remarks>
/// Keep this mutable struct in a field so output publication updates its original storage.
/// </remarks>
internal struct FluentUtilityState<TUtility>
    where TUtility : class
{
    #region Members

    private FluentUtilityResultCache<object> classResults;

    private AdditionalResults additionalResults;

    private sealed class AdditionalResults
    {
        public FluentUtilityResultCache<object> Class;

        public FluentUtilityResultCache<object> Style;

        public FluentUtilityResultCache<object> AlternateStyle;
    }

    #endregion

    #region Constructors

    public FluentUtilityState( int root, int id, TUtility parent = null, Action<TUtility, int, object> apply = null, int value = 0, object data = null, UtilityTarget? target = null, string operation = null )
    {
        Root = root;
        Id = id;
        Parent = parent;
        Apply = apply;
        Value = value;
        Data = data;
        Target = target;
        Operation = operation;
    }

    #endregion

    #region Methods

    public string GetClass( TUtility utility, object provider, Func<TUtility, object, string> buildResult, bool alternate = false )
    {
        if ( alternate )
        {
            return GetAdditionalResults().Class.GetOrAdd( utility, provider, IsCacheEntry, buildResult );
        }

        return classResults.GetOrAdd( utility, provider, IsCacheEntry, buildResult );
    }

    public string GetStyle( TUtility utility, object provider, Func<TUtility, object, string> buildResult, bool alternate = false )
    {
        if ( alternate )
        {
            return GetAdditionalResults().AlternateStyle.GetOrAdd( utility, provider, IsCacheEntry, buildResult );
        }

        return GetAdditionalResults().Style.GetOrAdd( utility, provider, IsCacheEntry, buildResult );
    }

    private AdditionalResults GetAdditionalResults()
    {
        var results = Volatile.Read( ref additionalResults );

        if ( results is null )
        {
            results = Interlocked.CompareExchange( ref additionalResults, new(), null ) ?? additionalResults;
        }

        return results;
    }

    #endregion

    #region Properties

    public int Root { get; }

    public int Id { get; }

    public TUtility Parent { get; }

    public Action<TUtility, int, object> Apply { get; }

    public int Value { get; }

    public object Data { get; }

    public UtilityTarget? Target { get; }

    public string Operation { get; }

    public readonly bool IsCached => Root != 0;

    public readonly bool IsCacheEntry => Id != 0;

    #endregion
}