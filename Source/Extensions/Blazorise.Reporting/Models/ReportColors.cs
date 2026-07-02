using System;
using System.ComponentModel;
using System.Globalization;

namespace Blazorise.Reporting;

/// <summary>
/// Common named colors intended for printed report design.
/// </summary>
public static class ReportColors
{
    /// <summary>
    /// Black.
    /// </summary>
    public static ReportColor Black { get; } = ReportColor.FromName( nameof( Black ) );

    /// <summary>
    /// White.
    /// </summary>
    public static ReportColor White { get; } = ReportColor.FromName( nameof( White ) );

    /// <summary>
    /// Red.
    /// </summary>
    public static ReportColor Red { get; } = ReportColor.FromName( nameof( Red ) );

    /// <summary>
    /// Green.
    /// </summary>
    public static ReportColor Green { get; } = ReportColor.FromName( nameof( Green ) );

    /// <summary>
    /// Blue.
    /// </summary>
    public static ReportColor Blue { get; } = ReportColor.FromName( nameof( Blue ) );

    /// <summary>
    /// Yellow.
    /// </summary>
    public static ReportColor Yellow { get; } = ReportColor.FromName( nameof( Yellow ) );

    /// <summary>
    /// Cyan.
    /// </summary>
    public static ReportColor Cyan { get; } = ReportColor.FromName( nameof( Cyan ) );

    /// <summary>
    /// Magenta.
    /// </summary>
    public static ReportColor Magenta { get; } = ReportColor.FromName( nameof( Magenta ) );

    /// <summary>
    /// Gray.
    /// </summary>
    public static ReportColor Gray { get; } = ReportColor.FromName( nameof( Gray ) );

    /// <summary>
    /// Light gray.
    /// </summary>
    public static ReportColor LightGray { get; } = ReportColor.FromName( nameof( LightGray ) );

    /// <summary>
    /// Dark gray.
    /// </summary>
    public static ReportColor DarkGray { get; } = ReportColor.FromName( nameof( DarkGray ) );

    /// <summary>
    /// Navy.
    /// </summary>
    public static ReportColor Navy { get; } = ReportColor.FromName( nameof( Navy ) );

    /// <summary>
    /// Maroon.
    /// </summary>
    public static ReportColor Maroon { get; } = ReportColor.FromName( nameof( Maroon ) );

    /// <summary>
    /// Olive.
    /// </summary>
    public static ReportColor Olive { get; } = ReportColor.FromName( nameof( Olive ) );

    /// <summary>
    /// Purple.
    /// </summary>
    public static ReportColor Purple { get; } = ReportColor.FromName( nameof( Purple ) );

    /// <summary>
    /// Teal.
    /// </summary>
    public static ReportColor Teal { get; } = ReportColor.FromName( nameof( Teal ) );

    /// <summary>
    /// Silver.
    /// </summary>
    public static ReportColor Silver { get; } = ReportColor.FromName( nameof( Silver ) );

    /// <summary>
    /// Orange.
    /// </summary>
    public static ReportColor Orange { get; } = ReportColor.FromName( nameof( Orange ) );
}