#region Using directives
using System;
#endregion

namespace Blazorise;

/// <summary>
/// Defines the built-in validation types.
/// </summary>
public static class ValidationHandlerType
{
    /// <summary>
    /// Cached type of the <see cref="ValidatorValidationHandler"/> validation handler.
    /// </summary>
    public static Type Validator { get; } = typeof( ValidatorValidationHandler );

    /// <summary>
    /// Cached type of the <see cref="PatternValidationHandler"/> validation handler.
    /// </summary>
    public static Type Pattern { get; } = typeof( PatternValidationHandler );

    /// <summary>
    /// Cached type of the <see cref="DataAnnotationValidationHandler"/> validation handler.
    /// </summary>
    public static Type DataAnnotation { get; } = typeof( DataAnnotationValidationHandler );
}