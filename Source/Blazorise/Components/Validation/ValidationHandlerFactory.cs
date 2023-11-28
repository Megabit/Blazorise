#region Using directives
using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
#endregion

namespace Blazorise;

/// <summary>
/// Default implementation of <see cref="IValidationHandlerFactory"/>.
/// </summary>
public class ValidationHandlerFactory : IValidationHandlerFactory
{
    /// <summary>
    /// Used to get the object registered within <see cref="IServiceCollection"/>.
    /// </summary>
    private readonly IServiceProvider serviceProvider;

    /// <summary>
    /// Holds the list of all requested validation handlers.
    /// </summary>
    private readonly ConcurrentDictionary<Type, IValidationHandler> handlers
        = new();

    /// <summary>
    /// Default constructor for handler factory.
    /// </summary>
    /// <param name="serviceProvider">Instance of <see cref="IServiceProvider"/>.</param>
    public ValidationHandlerFactory( IServiceProvider serviceProvider )
    {
        this.serviceProvider = serviceProvider;
    }

    /// <inheritdoc/>
    public IValidationHandler Create( Type handlerType )
    {
        return handlers.GetOrAdd( handlerType, key => CreateImpl( key ) );
    }

    /// <summary>
    /// Method used to actually create <see cref="IValidationHandler"/>. Can be overridden.
    /// </summary>
    /// <param name="handlerType">Type of the <see cref="IValidationHandler"/>.</param>
    /// <returns>Returns the new reference to validation handler.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <see cref="Type"/> is not registered within <see cref="IServiceCollection"/>.</exception>
    protected virtual IValidationHandler CreateImpl( Type handlerType )
    {
        var validationHandler = serviceProvider.GetService( handlerType ) as IValidationHandler;

        if ( validationHandler is null )
        {
            throw new ArgumentNullException( nameof( validationHandler ),
                "Validation handler is not supported or it is not implemented." );
        }

        return validationHandler;
    }
}