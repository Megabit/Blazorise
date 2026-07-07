using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Blazorise.Reporting;

/// <summary>
/// Converts report format definitions to and from JSON using the format category as the discriminator.
/// </summary>
public sealed class ReportFormatDefinitionJsonConverter : JsonConverter<ReportFormatDefinition>
{
    /// <inheritdoc />
    public override ReportFormatDefinition Read( ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options )
    {
        if ( reader.TokenType == JsonTokenType.Null )
            return null;

        using JsonDocument document = JsonDocument.ParseValue( ref reader );
        JsonElement root = document.RootElement;
        ReportFormatCategory category = ReadRequiredEnum<ReportFormatCategory>( root, nameof( ReportFormatDefinition.Category ), options );
        ReportFormatDefinition format = category switch
        {
            ReportFormatCategory.Number => new ReportNumberFormatDefinition
            {
                DecimalPlaces = ReadNullableInt32( root, nameof( ReportNumericFormatDefinition.DecimalPlaces ), options ),
                NegativeNumberFormat = ReadEnum( root, nameof( ReportNumericFormatDefinition.NegativeNumberFormat ), ReportNegativeNumberFormat.Default, options ),
                UseThousandsSeparator = ReadBoolean( root, nameof( ReportNumberFormatDefinition.UseThousandsSeparator ), defaultValue: true, options: options ),
            },
            ReportFormatCategory.Currency => new ReportCurrencyFormatDefinition
            {
                DecimalPlaces = ReadNullableInt32( root, nameof( ReportNumericFormatDefinition.DecimalPlaces ), options ),
                NegativeNumberFormat = ReadEnum( root, nameof( ReportNumericFormatDefinition.NegativeNumberFormat ), ReportNegativeNumberFormat.Default, options ),
                CurrencySymbol = ReadString( root, nameof( ReportCurrencyFormatDefinition.CurrencySymbol ), options ),
            },
            ReportFormatCategory.Percent => new ReportPercentFormatDefinition
            {
                DecimalPlaces = ReadNullableInt32( root, nameof( ReportNumericFormatDefinition.DecimalPlaces ), options ),
                NegativeNumberFormat = ReadEnum( root, nameof( ReportNumericFormatDefinition.NegativeNumberFormat ), ReportNegativeNumberFormat.Default, options ),
            },
            ReportFormatCategory.Date => new ReportDateFormatDefinition
            {
                DateFormat = ReadEnum( root, nameof( ReportTemporalFormatDefinition.DateFormat ), ReportDateFormat.ShortDate, options ),
            },
            ReportFormatCategory.Time => new ReportTimeFormatDefinition
            {
                DateFormat = ReadEnum( root, nameof( ReportTemporalFormatDefinition.DateFormat ), ReportDateFormat.ShortTime, options ),
            },
            ReportFormatCategory.DateTime => new ReportDateTimeFormatDefinition
            {
                DateFormat = ReadEnum( root, nameof( ReportTemporalFormatDefinition.DateFormat ), ReportDateFormat.ShortDateTime, options ),
            },
            ReportFormatCategory.Boolean => new ReportBooleanFormatDefinition
            {
                TrueText = ReadString( root, nameof( ReportBooleanFormatDefinition.TrueText ), options ),
                FalseText = ReadString( root, nameof( ReportBooleanFormatDefinition.FalseText ), options ),
            },
            ReportFormatCategory.Custom => new ReportCustomFormatDefinition
            {
                Format = ReadString( root, nameof( ReportCustomFormatDefinition.Format ), options ),
            },
            ReportFormatCategory.Text => new ReportTextFormatDefinition(),
            _ => throw new JsonException( $"Unsupported report format category '{category}'." ),
        };

        format.CultureName = ReadString( root, nameof( ReportFormatDefinition.CultureName ), options );
        format.CustomFormat = ReadString( root, nameof( ReportFormatDefinition.CustomFormat ), options );

        return format;
    }

    /// <inheritdoc />
    public override void Write( Utf8JsonWriter writer, ReportFormatDefinition value, JsonSerializerOptions options )
    {
        if ( value is null )
        {
            writer.WriteNullValue();
            return;
        }

        if ( value is not ReportTextFormatDefinition
            and not ReportNumberFormatDefinition
            and not ReportCurrencyFormatDefinition
            and not ReportPercentFormatDefinition
            and not ReportDateFormatDefinition
            and not ReportTimeFormatDefinition
            and not ReportDateTimeFormatDefinition
            and not ReportBooleanFormatDefinition
            and not ReportCustomFormatDefinition )
        {
            throw new JsonException( $"Unsupported report format definition '{value.GetType().FullName}'." );
        }

        writer.WriteStartObject();
        writer.WriteString( GetPropertyName( nameof( ReportFormatDefinition.Category ), options ), value.Category.ToString() );

        if ( !string.IsNullOrWhiteSpace( value.CultureName ) )
            writer.WriteString( GetPropertyName( nameof( ReportFormatDefinition.CultureName ), options ), value.CultureName );

        if ( !string.IsNullOrWhiteSpace( value.CustomFormat ) )
            writer.WriteString( GetPropertyName( nameof( ReportFormatDefinition.CustomFormat ), options ), value.CustomFormat );

        switch ( value )
        {
            case ReportNumberFormatDefinition numberFormat:
                WriteNumericProperties( writer, numberFormat, options );
                writer.WriteBoolean( GetPropertyName( nameof( ReportNumberFormatDefinition.UseThousandsSeparator ), options ), numberFormat.UseThousandsSeparator );
                break;
            case ReportCurrencyFormatDefinition currencyFormat:
                WriteNumericProperties( writer, currencyFormat, options );
                if ( !string.IsNullOrWhiteSpace( currencyFormat.CurrencySymbol ) )
                    writer.WriteString( GetPropertyName( nameof( ReportCurrencyFormatDefinition.CurrencySymbol ), options ), currencyFormat.CurrencySymbol );
                break;
            case ReportPercentFormatDefinition percentFormat:
                WriteNumericProperties( writer, percentFormat, options );
                break;
            case ReportTemporalFormatDefinition temporalFormat:
                writer.WriteString( GetPropertyName( nameof( ReportTemporalFormatDefinition.DateFormat ), options ), temporalFormat.DateFormat.ToString() );
                break;
            case ReportBooleanFormatDefinition booleanFormat:
                if ( !string.IsNullOrWhiteSpace( booleanFormat.TrueText ) )
                    writer.WriteString( GetPropertyName( nameof( ReportBooleanFormatDefinition.TrueText ), options ), booleanFormat.TrueText );
                if ( !string.IsNullOrWhiteSpace( booleanFormat.FalseText ) )
                    writer.WriteString( GetPropertyName( nameof( ReportBooleanFormatDefinition.FalseText ), options ), booleanFormat.FalseText );
                break;
            case ReportCustomFormatDefinition customFormat:
                if ( !string.IsNullOrWhiteSpace( customFormat.Format ) )
                    writer.WriteString( GetPropertyName( nameof( ReportCustomFormatDefinition.Format ), options ), customFormat.Format );
                break;
        }

        writer.WriteEndObject();
    }

    private static void WriteNumericProperties( Utf8JsonWriter writer, ReportNumericFormatDefinition format, JsonSerializerOptions options = null )
    {
        if ( format.DecimalPlaces is not null )
            writer.WriteNumber( GetPropertyName( nameof( ReportNumericFormatDefinition.DecimalPlaces ), options ), format.DecimalPlaces.Value );

        if ( format.NegativeNumberFormat != ReportNegativeNumberFormat.Default )
            writer.WriteString( GetPropertyName( nameof( ReportNumericFormatDefinition.NegativeNumberFormat ), options ), format.NegativeNumberFormat.ToString() );
    }

    private static bool TryGetProperty( JsonElement root, string propertyName, JsonSerializerOptions options, out JsonElement property )
    {
        if ( root.ValueKind != JsonValueKind.Object )
        {
            property = default;
            return false;
        }

        if ( root.TryGetProperty( propertyName, out property ) )
            return true;

        string convertedPropertyName = GetPropertyName( propertyName, options );

        if ( !string.Equals( convertedPropertyName, propertyName, StringComparison.Ordinal ) && root.TryGetProperty( convertedPropertyName, out property ) )
            return true;

        if ( options?.PropertyNameCaseInsensitive == true )
        {
            foreach ( JsonProperty candidate in root.EnumerateObject() )
            {
                if ( string.Equals( candidate.Name, propertyName, StringComparison.OrdinalIgnoreCase ) )
                {
                    property = candidate.Value;
                    return true;
                }
            }
        }

        return false;
    }

    private static string GetPropertyName( string propertyName, JsonSerializerOptions options )
    {
        return options?.PropertyNamingPolicy?.ConvertName( propertyName ) ?? propertyName;
    }

    private static string ReadString( JsonElement root, string propertyName, JsonSerializerOptions options )
    {
        if ( !TryGetProperty( root, propertyName, options, out JsonElement property ) || property.ValueKind == JsonValueKind.Null )
            return null;

        if ( property.ValueKind != JsonValueKind.String )
            throw new JsonException( $"Report format property '{propertyName}' must be a string." );

        return property.GetString();
    }

    private static int? ReadNullableInt32( JsonElement root, string propertyName, JsonSerializerOptions options )
    {
        if ( !TryGetProperty( root, propertyName, options, out JsonElement property ) || property.ValueKind == JsonValueKind.Null )
            return null;

        if ( !property.TryGetInt32( out int value ) )
            throw new JsonException( $"Report format property '{propertyName}' must be an integer." );

        return value;
    }

    private static bool ReadBoolean( JsonElement root, string propertyName, bool defaultValue, JsonSerializerOptions options )
    {
        if ( !TryGetProperty( root, propertyName, options, out JsonElement property ) )
            return defaultValue;

        return property.ValueKind switch
        {
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            _ => throw new JsonException( $"Report format property '{propertyName}' must be a boolean." ),
        };
    }

    private static TEnum ReadRequiredEnum<TEnum>( JsonElement root, string propertyName, JsonSerializerOptions options )
        where TEnum : struct, Enum
    {
        if ( !TryGetProperty( root, propertyName, options, out JsonElement property ) )
            throw new JsonException( $"Report format property '{propertyName}' is required." );

        return ReadEnumProperty<TEnum>( property, propertyName );
    }

    private static TEnum ReadEnum<TEnum>( JsonElement root, string propertyName, TEnum defaultValue, JsonSerializerOptions options )
        where TEnum : struct, Enum
    {
        if ( !TryGetProperty( root, propertyName, options, out JsonElement property ) )
            return defaultValue;

        return ReadEnumProperty<TEnum>( property, propertyName );
    }

    private static TEnum ReadEnumProperty<TEnum>( JsonElement property, string propertyName )
        where TEnum : struct, Enum
    {
        if ( property.ValueKind == JsonValueKind.String && Enum.TryParse( property.GetString(), ignoreCase: true, out TEnum parsedStringValue ) )
            return parsedStringValue;

        throw new JsonException( $"Report format property '{propertyName}' must be a valid {typeof( TEnum ).Name} string value." );
    }
}