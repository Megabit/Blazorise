using System;
using System.Linq.Expressions;

namespace Blazorise.DeepCloner.Helpers;

/// <summary>
/// Internal class but due implementation restriction should be public
/// </summary>
public abstract class ShallowObjectCloner
{
    /// <summary>
    /// Abstract method for real object cloning
    /// </summary>
    protected abstract object DoCloneObject( object obj );

    private static readonly ShallowObjectCloner _unsafeInstance;

    private static ShallowObjectCloner _instance;

    /// <summary>
    /// Performs real shallow object clone
    /// </summary>
    public static object CloneObject( object obj )
    {
        return _instance.DoCloneObject( obj );
    }

    internal static bool IsSafeVariant()
    {
        return _instance is ShallowSafeObjectCloner;
    }

    static ShallowObjectCloner()
    {

        _instance = new ShallowSafeObjectCloner();
        // no unsafe variant for core
        _unsafeInstance = _instance;
    }

    /// <summary>
    /// Purpose of this method is testing variants
    /// </summary>
    internal static void SwitchTo( bool isSafe )
    {
        DeepClonerCache.ClearCache();
        if ( isSafe )
            _instance = new ShallowSafeObjectCloner();
        else
            _instance = _unsafeInstance;
    }


    private class ShallowSafeObjectCloner : ShallowObjectCloner
    {
        private static readonly Func<object, object> _cloneFunc;

        static ShallowSafeObjectCloner()
        {
            var methodInfo = typeof( object ).GetPrivateMethod( "MemberwiseClone" );
            var p = Expression.Parameter( typeof( object ) );
            var mce = Expression.Call( p, methodInfo );
            _cloneFunc = Expression.Lambda<Func<object, object>>( mce, p ).Compile();
        }

        protected override object DoCloneObject( object obj )
        {
            return _cloneFunc( obj );
        }
    }
}
