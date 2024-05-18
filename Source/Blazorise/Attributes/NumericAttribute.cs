using System;
using Microsoft.AspNetCore.Components;

namespace Blazorise;

/// <summary>
/// Configures a numeric field.
/// </summary>
[AttributeUsage( AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false )]
public sealed class NumericAttribute : Attribute
{
    /// <summary>
    /// Specifies the interval between valid values.
    /// </summary>
    public int Step { get; set; } = 1;

    /// <summary>
    /// Maximum number of decimal places after the decimal separator.
    /// </summary>
    public int Decimals { get; set; } = 2;

    /// <summary>
    /// String to use as the decimal separator in numeric values.
    /// </summary>
    public string DecimalSeparator { get; set; } = ".";

    /// <summary>
    /// Helps define the language of an element.
    /// </summary>
    /// <remarks>
    /// https://www.w3schools.com/tags/ref_language_codes.asp
    /// </remarks>
    public string Culture { get; set; }

    /// <summary>
    /// If true, step buttons will be visible.
    /// </summary>
    public bool ShowStepButtons { get; set; } = true;

    /// <summary>
    /// If true, enables change of numeric value by pressing on step buttons or by keyboard up/down keys.
    /// </summary>
    public bool EnableStep { get; set; } = true;
}
