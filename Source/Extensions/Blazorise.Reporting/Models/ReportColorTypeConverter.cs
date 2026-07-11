#region Using directives
using System;
using System.ComponentModel;
using System.Globalization;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Converts string values into report colors for declarative Razor parameters.
/// </summary>
public sealed class ReportColorTypeConverter : TypeConverter
{
    #region Methods

    /// <inheritdoc />
    public override bool CanConvertFrom( ITypeDescriptorContext context, Type sourceType )
    {
        return sourceType == typeof( string ) || base.CanConvertFrom( context, sourceType );
    }

    /// <inheritdoc />
    public override object ConvertFrom( ITypeDescriptorContext context, CultureInfo culture, object value )
    {
        return value is string text
            ? ReportColor.FromString( text )
            : base.ConvertFrom( context, culture, value );
    }

    #endregion
}