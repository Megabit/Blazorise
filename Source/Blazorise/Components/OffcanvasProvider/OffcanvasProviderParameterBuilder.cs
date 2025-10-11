﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Blazorise.Components.OffcanvasProvider;
/// <summary>
/// Provides a builder with helper methods to help you build your component parameters.
/// </summary>
/// <typeparam name="TComponent"></typeparam>
public class OffcanvasProviderParameterBuilder<TComponent>
{
    /// <summary>
    /// Parameters to be passed onto TComponent instantiation.
    /// </summary>
    internal Dictionary<string, object> Parameters { get; private set; }

    /// <summary>
    /// Adds a new TComponent parameter.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="selector">Select that will resolve the parameter name</param>
    /// <param name="value">Parameter Value</param>
    public void Add<TValue>( Expression<Func<TComponent, TValue>> selector, TValue value )
    {
        var name = ( selector.Body as MemberExpression ).Member.Name;

        Add( name, value );
    }

    /// <summary>
    /// Adds a new parameter.
    /// </summary>
    /// <param name="name">Parameter name</param>
    /// <param name="value">Parameter value</param>
    public void Add( string name, object value )
    {
        Parameters ??= new();
        Parameters.Add( name, value );
    }
}
