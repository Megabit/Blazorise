namespace Blazorise.Extensions;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
public static class EnumExtensions
{
    public static string ToButtonTagName( this ButtonType buttonType )
    {
        return buttonType switch
        {
            ButtonType.Link => "a",
            _ => "button",
        };
    }

    public static string ToButtonTypeString( this ButtonType buttonType )
    {
        return buttonType switch
        {
            ButtonType.Button => "button",
            ButtonType.Submit => "submit",
            ButtonType.Reset => "reset",
            _ => null,
        };
    }

    public static string ToTextRoleString( this TextRole textRole )
    {
        return textRole switch
        {
            TextRole.Email => "email",
            TextRole.Password => "password",
            TextRole.Url => "url",
            TextRole.Search => "search",
            TextRole.Telephone => "tel",
            _ => "text",
        };
    }


    public static string ToMaskTypeString( this MaskType maskType )
    {
        return maskType switch
        {
            MaskType.Numeric => "numeric",
            MaskType.DateTime => "datetime",
            MaskType.RegEx => "regex",
            _ => null,
        };
    }

    public static string ToTextInputMode( this TextInputMode textInputMode )
    {
        return textInputMode switch
        {
            TextInputMode.Text => "text",
            TextInputMode.Tel => "tel",
            TextInputMode.Url => "url",
            TextInputMode.Email => "email",
            TextInputMode.Numeric => "numeric",
            TextInputMode.Decimal => "decimal",
            TextInputMode.Search => "search",
            _ => null,
        };
    }

    public static string ToDateInputMode( this DateInputMode dateInputMode )
    {
        return dateInputMode switch
        {
            DateInputMode.DateTime => "datetime-local",
            DateInputMode.Month => "month",
            _ => "date",
        };
    }

    public static string ToDateInputSelectionMode( this DateInputSelectionMode dateInputSelectionMode )
    {
        return dateInputSelectionMode switch
        {
            DateInputSelectionMode.Range => "range",
            DateInputSelectionMode.Multiple => "multiple",
            _ => null,
        };
    }

    /// <summary>
    /// Gets the link target name.
    /// </summary>
    public static string ToTargetString( this Target target ) => string.IsNullOrEmpty( target.Name ) ? null : target.Name;

    public static string ToInputMaskCaretPosition( this InputMaskCaretPosition inputMaskCaretPosition ) => inputMaskCaretPosition switch
    {
        InputMaskCaretPosition.LastValidPosition => "lvp",
        InputMaskCaretPosition.RadixFocus => "radixFocus",
        InputMaskCaretPosition.Select => "select",
        InputMaskCaretPosition.Ignore => "ignore",
        _ => "none",
    };

    public static string ToCurrencySymbolPlacement( this CurrencySymbolPlacement currencySymbolPlacement ) => currencySymbolPlacement switch
    {
        CurrencySymbolPlacement.Suffix => "s",
        CurrencySymbolPlacement.Prefix => "p",
        _ => "p",
    };

    public static string ToNumericRoundingMethod( this NumericRoundingMethod numericRoundingMethod ) => numericRoundingMethod switch
    {
        NumericRoundingMethod.HalfUpSymmetric => "S",
        NumericRoundingMethod.HalfUpAsymmetric => "A",
        NumericRoundingMethod.HalfDownSymmetric => "s",
        NumericRoundingMethod.HalfDownAsymmetric => "a",
        NumericRoundingMethod.HalfEvenBankersRounding => "B",
        NumericRoundingMethod.UpRoundAwayFromZero => "U",
        NumericRoundingMethod.DownRoundTowardZero => "D",
        NumericRoundingMethod.ToCeilingTowardPositiveInfinity => "C",
        NumericRoundingMethod.ToFloorTowardNegativeInfinity => "F",
        NumericRoundingMethod.ToNearest05 => "N05",
        NumericRoundingMethod.ToNearest05Alt => "CHF",
        NumericRoundingMethod.UpToNext05 => "U05",
        NumericRoundingMethod.DownToNext05 => "D05",
        _ => "S",
    };

    public static object ToNumericDecimalPadding( this NumericAllowDecimalPadding numericAllowDecimalPadding ) => numericAllowDecimalPadding switch
    {
        NumericAllowDecimalPadding.Always => true,
        NumericAllowDecimalPadding.Never => false,
        NumericAllowDecimalPadding.Floats => "floats",
        _ => true
    };

    public static object ToNumericMinMaxLimitsOverride( this NumericMinMaxLimitsOverride numericMinMaxLimitsOverride ) => numericMinMaxLimitsOverride switch
    {
        NumericMinMaxLimitsOverride.Ceiling => "ceiling",
        NumericMinMaxLimitsOverride.Floor => "floor",
        NumericMinMaxLimitsOverride.Ignore => "ignore",
        NumericMinMaxLimitsOverride.DoNotOverride => null,
        _ => null
    };

    public static object ToNumericWheelOn( this NumericWheelOn numericWheelOn ) => numericWheelOn switch
    {
        NumericWheelOn.Focus => "focus",
        NumericWheelOn.Hover => "hover",
        _ => null
    };

    /// <summary>
    /// Indicates whether the specified enum size is null or a default value.
    /// </summary>
    /// <param name="size">Enum to test.</param>
    /// <returns>True if the value parameter is null or a default value; otherwise, false.</returns>
    public static bool IsNullOrDefault( this Size? size )
        => size is null || size == Size.Default;
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member