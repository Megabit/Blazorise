using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Blazorise.DataGrid.Utils
{
    static class FunctionCompiler
    {
        public static Func<TItem> CreateNewItem<TItem>()
        {
            return Expression.Lambda<Func<TItem>>( Expression.New( typeof( TItem ) ) ).Compile();
        }

        public static Func<TItem, object> CreateValueGetter<TItem>( string fieldName )
        {
            var itemExp = Expression.Parameter( typeof( TItem ), "item" );
            var propertyExp = Expression.Property( itemExp, fieldName );
            return Expression.Lambda<Func<TItem, object>>( Expression.Convert( propertyExp, typeof( object ) ), itemExp ).Compile();
        }

        public static Func<Type> CreateValueTypeGetter<TItem>( string fieldName )
        {
            var itemExp = Expression.Parameter( typeof( TItem ) );
            var propertyExp = Expression.Property( itemExp, fieldName );
            return Expression.Lambda<Func<Type>>( Expression.Constant( propertyExp.Type ) ).Compile();
        }

        public static Func<object> CreateDefaultValueByType<TItem>( string fieldName )
        {
            var itemExp = Expression.Parameter( typeof( TItem ) );
            var propertyExp = Expression.Property( itemExp, fieldName );
            return Expression.Lambda<Func<object>>( Expression.Convert( Expression.Default( propertyExp.Type ), typeof( object ) ) ).Compile();
        }

        public static Action<TItem, object> CreateValueSetter<TItem>( string fieldName )
        {
            var itemExp = Expression.Parameter( typeof( TItem ), "item" );
            var valueExp = Expression.Parameter( typeof( object ), "value" );
            var propertyExp = Expression.Property( itemExp, fieldName );
            return Expression.Lambda<Action<TItem, object>>( Expression.Assign( propertyExp, Expression.Convert( valueExp, propertyExp.Type ) ), itemExp, valueExp ).Compile();
        }
    }
}
