﻿#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.AspNetCore.Components.Forms;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Helper for various expression based methods.
    /// </summary>
    public static class ExpressionCompiler
    {
        /// <summary>
        /// Gets a property in an unknown instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static T GetProperty<T>( object instance, string propertyName )
            => CreatePropertyGetter<T>( instance, propertyName )(instance);

        /// <summary>
        /// Generates a function getter for a property in an unknown instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static Func<object, T> CreatePropertyGetter<T>( object instance, string propertyName )
        {
            var parameterExp = Expression.Parameter( typeof( object ), "instance" );
            var castExp = Expression.TypeAs( parameterExp, instance.GetType() );
            var property = Expression.Property( castExp, propertyName );

            return Expression.Lambda<Func<object, T>>( Expression.Convert( property, typeof( T ) ), parameterExp ).Compile();
        }
    }
}