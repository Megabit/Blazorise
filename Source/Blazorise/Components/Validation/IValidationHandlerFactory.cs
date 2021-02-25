#region Using directives
using System;
using Microsoft.Extensions.DependencyInjection;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Main factory to create <see cref="IValidationHandler"/> based on the given <see cref="Type"/>.
    /// </summary>
    public interface IValidationHandlerFactory
    {
        /// <summary>
        /// Gets the <see cref="IValidationHandler"/> for the given <paramref name="handlerType"/>.
        /// </summary>
        /// <param name="handlerType">Type of the <see cref="IValidationHandler"/>.</param>
        /// <returns>Returns the new reference to validation handler.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <see cref="Type"/> is not registered within <see cref="IServiceCollection"/>.</exception>
        IValidationHandler Create( Type handlerType );
    }
}
