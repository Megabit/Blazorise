#region Using directives
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.FluentUI2.Models;

public class FluentUI2DesignTokens
{
    [JsonPropertyName( "borderRadiusNone" )]
    public string BorderRadiusNone { get; set; } = "0";

    [JsonPropertyName( "borderRadiusSmall" )]
    public string BorderRadiusSmall { get; set; } = "2px";

    [JsonPropertyName( "borderRadiusMedium" )]
    public string BorderRadiusMedium { get; set; } = "4px";

    [JsonPropertyName( "borderRadiusLarge" )]
    public string BorderRadiusLarge { get; set; } = "6px";

    [JsonPropertyName( "borderRadiusXLarge" )]
    public string BorderRadiusXLarge { get; set; } = "8px";

    [JsonPropertyName( "borderRadiusCircular" )]
    public string BorderRadiusCircular { get; set; } = "10000px";

    [JsonPropertyName( "fontSizeBase100" )]
    public string FontSizeBase100 { get; set; } = "10px";

    [JsonPropertyName( "fontSizeBase200" )]
    public string FontSizeBase200 { get; set; } = "12px";

    [JsonPropertyName( "fontSizeBase300" )]
    public string FontSizeBase300 { get; set; } = "14px";

    [JsonPropertyName( "fontSizeBase400" )]
    public string FontSizeBase400 { get; set; } = "16px";

    [JsonPropertyName( "fontSizeBase500" )]
    public string FontSizeBase500 { get; set; } = "20px";

    [JsonPropertyName( "fontSizeBase600" )]
    public string FontSizeBase600 { get; set; } = "24px";

    [JsonPropertyName( "fontSizeHero700" )]
    public string FontSizeHero700 { get; set; } = "28px";

    [JsonPropertyName( "fontSizeHero800" )]
    public string FontSizeHero800 { get; set; } = "32px";

    [JsonPropertyName( "fontSizeHero900" )]
    public string FontSizeHero900 { get; set; } = "40px";

    [JsonPropertyName( "fontSizeHero1000" )]
    public string FontSizeHero1000 { get; set; } = "68px";

    [JsonPropertyName( "lineHeightBase100" )]
    public string LineHeightBase100 { get; set; } = "14px";

    [JsonPropertyName( "lineHeightBase200" )]
    public string LineHeightBase200 { get; set; } = "16px";

    [JsonPropertyName( "lineHeightBase300" )]
    public string LineHeightBase300 { get; set; } = "20px";

    [JsonPropertyName( "lineHeightBase400" )]
    public string LineHeightBase400 { get; set; } = "22px";

    [JsonPropertyName( "lineHeightBase500" )]
    public string LineHeightBase500 { get; set; } = "28px";

    [JsonPropertyName( "lineHeightBase600" )]
    public string LineHeightBase600 { get; set; } = "32px";

    [JsonPropertyName( "lineHeightHero700" )]
    public string LineHeightHero700 { get; set; } = "36px";

    [JsonPropertyName( "lineHeightHero800" )]
    public string LineHeightHero800 { get; set; } = "40px";

    [JsonPropertyName( "lineHeightHero900" )]
    public string LineHeightHero900 { get; set; } = "52px";

    [JsonPropertyName( "lineHeightHero1000" )]
    public string LineHeightHero1000 { get; set; } = "92px";

    [JsonPropertyName( "fontFamilyBase" )]
    public string FontFamilyBase { get; set; } = "'Segoe UI', 'Segoe UI Web (West European)', -apple-system, BlinkMacSystemFont, Roboto, 'Helvetica Neue', sans-serif";

    [JsonPropertyName( "fontFamilyMonospace" )]
    public string FontFamilyMonospace { get; set; } = "Consolas, 'Courier New', Courier, monospace";

    [JsonPropertyName( "fontFamilyNumeric" )]
    public string FontFamilyNumeric { get; set; } = "Bahnschrift, 'Segoe UI', 'Segoe UI Web (West European)', -apple-system, BlinkMacSystemFont, Roboto, 'Helvetica Neue', sans-serif";

    [JsonPropertyName( "fontWeightRegular" )]
    public int FontWeightRegular { get; set; } = 400;

    [JsonPropertyName( "fontWeightMedium" )]
    public int FontWeightMedium { get; set; } = 500;

    [JsonPropertyName( "fontWeightSemibold" )]
    public int FontWeightSemibold { get; set; } = 600;

    [JsonPropertyName( "fontWeightBold" )]
    public int FontWeightBold { get; set; } = 700;

    [JsonPropertyName( "strokeWidthThin" )]
    public string StrokeWidthThin { get; set; } = "1px";

    [JsonPropertyName( "strokeWidthThick" )]
    public string StrokeWidthThick { get; set; } = "2px";

    [JsonPropertyName( "strokeWidthThicker" )]
    public string StrokeWidthThicker { get; set; } = "3px";

    [JsonPropertyName( "strokeWidthThickest" )]
    public string StrokeWidthThickest { get; set; } = "4px";

    [JsonPropertyName( "spacingHorizontalNone" )]
    public string SpacingHorizontalNone { get; set; } = "0";

    [JsonPropertyName( "spacingHorizontalXXS" )]
    public string SpacingHorizontalXXS { get; set; } = "2px";

    [JsonPropertyName( "spacingHorizontalXS" )]
    public string SpacingHorizontalXS { get; set; } = "4px";

    [JsonPropertyName( "spacingHorizontalSNudge" )]
    public string SpacingHorizontalSNudge { get; set; } = "6px";

    [JsonPropertyName( "spacingHorizontalS" )]
    public string SpacingHorizontalS { get; set; } = "8px";

    [JsonPropertyName( "spacingHorizontalMNudge" )]
    public string SpacingHorizontalMNudge { get; set; } = "10px";

    [JsonPropertyName( "spacingHorizontalM" )]
    public string SpacingHorizontalM { get; set; } = "12px";

    [JsonPropertyName( "spacingHorizontalL" )]
    public string SpacingHorizontalL { get; set; } = "16px";

    [JsonPropertyName( "spacingHorizontalXL" )]
    public string SpacingHorizontalXL { get; set; } = "20px";

    [JsonPropertyName( "spacingHorizontalXXL" )]
    public string SpacingHorizontalXXL { get; set; } = "24px";

    [JsonPropertyName( "spacingHorizontalXXXL" )]
    public string SpacingHorizontalXXXL { get; set; } = "32px";

    [JsonPropertyName( "spacingVerticalNone" )]
    public string SpacingVerticalNone { get; set; } = "0";

    [JsonPropertyName( "spacingVerticalXXS" )]
    public string SpacingVerticalXXS { get; set; } = "2px";

    [JsonPropertyName( "spacingVerticalXS" )]
    public string SpacingVerticalXS { get; set; } = "4px";

    [JsonPropertyName( "spacingVerticalSNudge" )]
    public string SpacingVerticalSNudge { get; set; } = "6px";

    [JsonPropertyName( "spacingVerticalS" )]
    public string SpacingVerticalS { get; set; } = "8px";

    [JsonPropertyName( "spacingVerticalMNudge" )]
    public string SpacingVerticalMNudge { get; set; } = "10px";

    [JsonPropertyName( "spacingVerticalM" )]
    public string SpacingVerticalM { get; set; } = "12px";

    [JsonPropertyName( "spacingVerticalL" )]
    public string SpacingVerticalL { get; set; } = "16px";

    [JsonPropertyName( "spacingVerticalXL" )]
    public string SpacingVerticalXL { get; set; } = "20px";

    [JsonPropertyName( "spacingVerticalXXL" )]
    public string SpacingVerticalXXL { get; set; } = "24px";

    [JsonPropertyName( "spacingVerticalXXXL" )]
    public string SpacingVerticalXXXL { get; set; } = "32px";

    [JsonPropertyName( "durationUltraFast" )]
    public string DurationUltraFast { get; set; } = "50ms";

    [JsonPropertyName( "durationFaster" )]
    public string DurationFaster { get; set; } = "100ms";

    [JsonPropertyName( "durationFast" )]
    public string DurationFast { get; set; } = "150ms";

    [JsonPropertyName( "durationNormal" )]
    public string DurationNormal { get; set; } = "200ms";

    [JsonPropertyName( "durationGentle" )]
    public string DurationGentle { get; set; } = "250ms";

    [JsonPropertyName( "durationSlow" )]
    public string DurationSlow { get; set; } = "300ms";

    [JsonPropertyName( "durationSlower" )]
    public string DurationSlower { get; set; } = "400ms";

    [JsonPropertyName( "durationUltraSlow" )]
    public string DurationUltraSlow { get; set; } = "500ms";

    [JsonPropertyName( "curveAccelerateMax" )]
    public string CurveAccelerateMax { get; set; } = "cubic-bezier(0.9,0.1,1,0.2)";

    [JsonPropertyName( "curveAccelerateMid" )]
    public string CurveAccelerateMid { get; set; } = "cubic-bezier(1,0,1,1)";

    [JsonPropertyName( "curveAccelerateMin" )]
    public string CurveAccelerateMin { get; set; } = "cubic-bezier(0.8,0,0.78,1)";

    [JsonPropertyName( "curveDecelerateMax" )]
    public string CurveDecelerateMax { get; set; } = "cubic-bezier(0.1,0.9,0.2,1)";

    [JsonPropertyName( "curveDecelerateMid" )]
    public string CurveDecelerateMid { get; set; } = "cubic-bezier(0,0,0,1)";

    [JsonPropertyName( "curveDecelerateMin" )]
    public string CurveDecelerateMin { get; set; } = "cubic-bezier(0.33,0,0.1,1)";

    [JsonPropertyName( "curveEasyEaseMax" )]
    public string CurveEasyEaseMax { get; set; } = "cubic-bezier(0.8,0,0.2,1)";

    [JsonPropertyName( "curveEasyEase" )]
    public string CurveEasyEase { get; set; } = "cubic-bezier(0.33,0,0.67,1)";

    [JsonPropertyName( "curveLinear" )]
    public string CurveLinear { get; set; } = "cubic-bezier(0,0,1,1)";

    [JsonPropertyName( "colorNeutralForeground1" )]
    public string ColorNeutralForeground1 { get; set; } = "#242424";

    [JsonPropertyName( "colorNeutralForeground1Hover" )]
    public string ColorNeutralForeground1Hover { get; set; } = "#242424";

    [JsonPropertyName( "colorNeutralForeground1Pressed" )]
    public string ColorNeutralForeground1Pressed { get; set; } = "#242424";

    [JsonPropertyName( "colorNeutralForeground1Selected" )]
    public string ColorNeutralForeground1Selected { get; set; } = "#242424";

    [JsonPropertyName( "colorNeutralForeground2" )]
    public string ColorNeutralForeground2 { get; set; } = "#424242";

    [JsonPropertyName( "colorNeutralForeground2Hover" )]
    public string ColorNeutralForeground2Hover { get; set; } = "#242424";

    [JsonPropertyName( "colorNeutralForeground2Pressed" )]
    public string ColorNeutralForeground2Pressed { get; set; } = "#242424";

    [JsonPropertyName( "colorNeutralForeground2Selected" )]
    public string ColorNeutralForeground2Selected { get; set; } = "#242424";

    [JsonPropertyName( "colorNeutralForeground2BrandHover" )]
    public string ColorNeutralForeground2BrandHover { get; set; } = "#1267b4";

    [JsonPropertyName( "colorNeutralForeground2BrandPressed" )]
    public string ColorNeutralForeground2BrandPressed { get; set; } = "#18599b";

    [JsonPropertyName( "colorNeutralForeground2BrandSelected" )]
    public string ColorNeutralForeground2BrandSelected { get; set; } = "#1267b4";

    [JsonPropertyName( "colorNeutralForeground3" )]
    public string ColorNeutralForeground3 { get; set; } = "#616161";

    [JsonPropertyName( "colorNeutralForeground3Hover" )]
    public string ColorNeutralForeground3Hover { get; set; } = "#424242";

    [JsonPropertyName( "colorNeutralForeground3Pressed" )]
    public string ColorNeutralForeground3Pressed { get; set; } = "#424242";

    [JsonPropertyName( "colorNeutralForeground3Selected" )]
    public string ColorNeutralForeground3Selected { get; set; } = "#424242";

    [JsonPropertyName( "colorNeutralForeground3BrandHover" )]
    public string ColorNeutralForeground3BrandHover { get; set; } = "#1267b4";

    [JsonPropertyName( "colorNeutralForeground3BrandPressed" )]
    public string ColorNeutralForeground3BrandPressed { get; set; } = "#18599b";

    [JsonPropertyName( "colorNeutralForeground3BrandSelected" )]
    public string ColorNeutralForeground3BrandSelected { get; set; } = "#1267b4";

    [JsonPropertyName( "colorNeutralForeground4" )]
    public string ColorNeutralForeground4 { get; set; } = "#707070";

    [JsonPropertyName( "colorNeutralForegroundDisabled" )]
    public string ColorNeutralForegroundDisabled { get; set; } = "#bdbdbd";

    [JsonPropertyName( "colorNeutralForegroundInvertedDisabled" )]
    public string ColorNeutralForegroundInvertedDisabled { get; set; } = "rgba(255, 255, 255, 0.4)";

    [JsonPropertyName( "colorBrandForegroundLink" )]
    public string ColorBrandForegroundLink { get; set; } = "#18599b";

    [JsonPropertyName( "colorBrandForegroundLinkHover" )]
    public string ColorBrandForegroundLinkHover { get; set; } = "#1b4c82";

    [JsonPropertyName( "colorBrandForegroundLinkPressed" )]
    public string ColorBrandForegroundLinkPressed { get; set; } = "#193253";

    [JsonPropertyName( "colorBrandForegroundLinkSelected" )]
    public string ColorBrandForegroundLinkSelected { get; set; } = "#18599b";

    [JsonPropertyName( "colorNeutralForeground2Link" )]
    public string ColorNeutralForeground2Link { get; set; } = "#424242";

    [JsonPropertyName( "colorNeutralForeground2LinkHover" )]
    public string ColorNeutralForeground2LinkHover { get; set; } = "#242424";

    [JsonPropertyName( "colorNeutralForeground2LinkPressed" )]
    public string ColorNeutralForeground2LinkPressed { get; set; } = "#242424";

    [JsonPropertyName( "colorNeutralForeground2LinkSelected" )]
    public string ColorNeutralForeground2LinkSelected { get; set; } = "#242424";

    [JsonPropertyName( "colorCompoundBrandForeground1" )]
    public string ColorCompoundBrandForeground1 { get; set; } = "#1267b4";

    [JsonPropertyName( "colorCompoundBrandForeground1Hover" )]
    public string ColorCompoundBrandForeground1Hover { get; set; } = "#18599b";

    [JsonPropertyName( "colorCompoundBrandForeground1Pressed" )]
    public string ColorCompoundBrandForeground1Pressed { get; set; } = "#1b4c82";

    [JsonPropertyName( "colorBrandForeground1" )]
    public string ColorBrandForeground1 { get; set; } = "#1267b4";

    [JsonPropertyName( "colorBrandForeground2" )]
    public string ColorBrandForeground2 { get; set; } = "#18599b";

    [JsonPropertyName( "colorBrandForeground2Hover" )]
    public string ColorBrandForeground2Hover { get; set; } = "#1b4c82";

    [JsonPropertyName( "colorBrandForeground2Pressed" )]
    public string ColorBrandForeground2Pressed { get; set; } = "#16263d";

    [JsonPropertyName( "colorNeutralForeground1Static" )]
    public string ColorNeutralForeground1Static { get; set; } = "#242424";

    [JsonPropertyName( "colorNeutralForegroundStaticInverted" )]
    public string ColorNeutralForegroundStaticInverted { get; set; } = "#ffffff";

    [JsonPropertyName( "colorNeutralForegroundInverted" )]
    public string ColorNeutralForegroundInverted { get; set; } = "#ffffff";

    [JsonPropertyName( "colorNeutralForegroundInvertedHover" )]
    public string ColorNeutralForegroundInvertedHover { get; set; } = "#ffffff";

    [JsonPropertyName( "colorNeutralForegroundInvertedPressed" )]
    public string ColorNeutralForegroundInvertedPressed { get; set; } = "#ffffff";

    [JsonPropertyName( "colorNeutralForegroundInvertedSelected" )]
    public string ColorNeutralForegroundInvertedSelected { get; set; } = "#ffffff";

    [JsonPropertyName( "colorNeutralForegroundInverted2" )]
    public string ColorNeutralForegroundInverted2 { get; set; } = "#ffffff";

    [JsonPropertyName( "colorNeutralForegroundOnBrand" )]
    public string ColorNeutralForegroundOnBrand { get; set; } = "#ffffff";

    [JsonPropertyName( "colorNeutralForegroundInvertedLink" )]
    public string ColorNeutralForegroundInvertedLink { get; set; } = "#ffffff";

    [JsonPropertyName( "colorNeutralForegroundInvertedLinkHover" )]
    public string ColorNeutralForegroundInvertedLinkHover { get; set; } = "#ffffff";

    [JsonPropertyName( "colorNeutralForegroundInvertedLinkPressed" )]
    public string ColorNeutralForegroundInvertedLinkPressed { get; set; } = "#ffffff";

    [JsonPropertyName( "colorNeutralForegroundInvertedLinkSelected" )]
    public string ColorNeutralForegroundInvertedLinkSelected { get; set; } = "#ffffff";

    [JsonPropertyName( "colorBrandForegroundInverted" )]
    public string ColorBrandForegroundInverted { get; set; } = "#4f82c8";

    [JsonPropertyName( "colorBrandForegroundInvertedHover" )]
    public string ColorBrandForegroundInvertedHover { get; set; } = "#6790cf";

    [JsonPropertyName( "colorBrandForegroundInvertedPressed" )]
    public string ColorBrandForegroundInvertedPressed { get; set; } = "#4f82c8";

    [JsonPropertyName( "colorBrandForegroundOnLight" )]
    public string ColorBrandForegroundOnLight { get; set; } = "#1267b4";

    [JsonPropertyName( "colorBrandForegroundOnLightHover" )]
    public string ColorBrandForegroundOnLightHover { get; set; } = "#18599b";

    [JsonPropertyName( "colorBrandForegroundOnLightPressed" )]
    public string ColorBrandForegroundOnLightPressed { get; set; } = "#1b3f6a";

    [JsonPropertyName( "colorBrandForegroundOnLightSelected" )]
    public string ColorBrandForegroundOnLightSelected { get; set; } = "#1b4c82";

    [JsonPropertyName( "colorNeutralBackground1" )]
    public string ColorNeutralBackground1 { get; set; } = "#ffffff";

    [JsonPropertyName( "colorNeutralBackground1Hover" )]
    public string ColorNeutralBackground1Hover { get; set; } = "#f5f5f5";

    [JsonPropertyName( "colorNeutralBackground1Pressed" )]
    public string ColorNeutralBackground1Pressed { get; set; } = "#e0e0e0";

    [JsonPropertyName( "colorNeutralBackground1Selected" )]
    public string ColorNeutralBackground1Selected { get; set; } = "#ebebeb";

    [JsonPropertyName( "colorNeutralBackground2" )]
    public string ColorNeutralBackground2 { get; set; } = "#fafafa";

    [JsonPropertyName( "colorNeutralBackground2Hover" )]
    public string ColorNeutralBackground2Hover { get; set; } = "#f0f0f0";

    [JsonPropertyName( "colorNeutralBackground2Pressed" )]
    public string ColorNeutralBackground2Pressed { get; set; } = "#dbdbdb";

    [JsonPropertyName( "colorNeutralBackground2Selected" )]
    public string ColorNeutralBackground2Selected { get; set; } = "#e6e6e6";

    [JsonPropertyName( "colorNeutralBackground3" )]
    public string ColorNeutralBackground3 { get; set; } = "#f5f5f5";

    [JsonPropertyName( "colorNeutralBackground3Hover" )]
    public string ColorNeutralBackground3Hover { get; set; } = "#ebebeb";

    [JsonPropertyName( "colorNeutralBackground3Pressed" )]
    public string ColorNeutralBackground3Pressed { get; set; } = "#d6d6d6";

    [JsonPropertyName( "colorNeutralBackground3Selected" )]
    public string ColorNeutralBackground3Selected { get; set; } = "#e0e0e0";

    [JsonPropertyName( "colorNeutralBackground4" )]
    public string ColorNeutralBackground4 { get; set; } = "#f0f0f0";

    [JsonPropertyName( "colorNeutralBackground4Hover" )]
    public string ColorNeutralBackground4Hover { get; set; } = "#fafafa";

    [JsonPropertyName( "colorNeutralBackground4Pressed" )]
    public string ColorNeutralBackground4Pressed { get; set; } = "#f5f5f5";

    [JsonPropertyName( "colorNeutralBackground4Selected" )]
    public string ColorNeutralBackground4Selected { get; set; } = "#ffffff";

    [JsonPropertyName( "colorNeutralBackground5" )]
    public string ColorNeutralBackground5 { get; set; } = "#ebebeb";

    [JsonPropertyName( "colorNeutralBackground5Hover" )]
    public string ColorNeutralBackground5Hover { get; set; } = "#f5f5f5";

    [JsonPropertyName( "colorNeutralBackground5Pressed" )]
    public string ColorNeutralBackground5Pressed { get; set; } = "#f0f0f0";

    [JsonPropertyName( "colorNeutralBackground5Selected" )]
    public string ColorNeutralBackground5Selected { get; set; } = "#fafafa";

    [JsonPropertyName( "colorNeutralBackground6" )]
    public string ColorNeutralBackground6 { get; set; } = "#e6e6e6";

    [JsonPropertyName( "colorNeutralBackgroundInverted" )]
    public string ColorNeutralBackgroundInverted { get; set; } = "#292929";

    [JsonPropertyName( "colorNeutralBackgroundStatic" )]
    public string ColorNeutralBackgroundStatic { get; set; } = "#333333";

    [JsonPropertyName( "colorNeutralBackgroundAlpha" )]
    public string ColorNeutralBackgroundAlpha { get; set; } = "rgba(255, 255, 255, 0.5)";

    [JsonPropertyName( "colorNeutralBackgroundAlpha2" )]
    public string ColorNeutralBackgroundAlpha2 { get; set; } = "rgba(255, 255, 255, 0.8)";

    [JsonPropertyName( "colorSubtleBackground" )]
    public string ColorSubtleBackground { get; set; } = "transparent";

    [JsonPropertyName( "colorSubtleBackgroundHover" )]
    public string ColorSubtleBackgroundHover { get; set; } = "#f5f5f5";

    [JsonPropertyName( "colorSubtleBackgroundPressed" )]
    public string ColorSubtleBackgroundPressed { get; set; } = "#e0e0e0";

    [JsonPropertyName( "colorSubtleBackgroundSelected" )]
    public string ColorSubtleBackgroundSelected { get; set; } = "#ebebeb";

    [JsonPropertyName( "colorSubtleBackgroundLightAlphaHover" )]
    public string ColorSubtleBackgroundLightAlphaHover { get; set; } = "rgba(255, 255, 255, 0.7)";

    [JsonPropertyName( "colorSubtleBackgroundLightAlphaPressed" )]
    public string ColorSubtleBackgroundLightAlphaPressed { get; set; } = "rgba(255, 255, 255, 0.5)";

    [JsonPropertyName( "colorSubtleBackgroundLightAlphaSelected" )]
    public string ColorSubtleBackgroundLightAlphaSelected { get; set; } = "transparent";

    [JsonPropertyName( "colorSubtleBackgroundInverted" )]
    public string ColorSubtleBackgroundInverted { get; set; } = "transparent";

    [JsonPropertyName( "colorSubtleBackgroundInvertedHover" )]
    public string ColorSubtleBackgroundInvertedHover { get; set; } = "rgba(0, 0, 0, 0.1)";

    [JsonPropertyName( "colorSubtleBackgroundInvertedPressed" )]
    public string ColorSubtleBackgroundInvertedPressed { get; set; } = "rgba(0, 0, 0, 0.3)";

    [JsonPropertyName( "colorSubtleBackgroundInvertedSelected" )]
    public string ColorSubtleBackgroundInvertedSelected { get; set; } = "rgba(0, 0, 0, 0.2)";

    [JsonPropertyName( "colorTransparentBackground" )]
    public string ColorTransparentBackground { get; set; } = "transparent";

    [JsonPropertyName( "colorTransparentBackgroundHover" )]
    public string ColorTransparentBackgroundHover { get; set; } = "transparent";

    [JsonPropertyName( "colorTransparentBackgroundPressed" )]
    public string ColorTransparentBackgroundPressed { get; set; } = "transparent";

    [JsonPropertyName( "colorTransparentBackgroundSelected" )]
    public string ColorTransparentBackgroundSelected { get; set; } = "transparent";

    [JsonPropertyName( "colorNeutralBackgroundDisabled" )]
    public string ColorNeutralBackgroundDisabled { get; set; } = "#f0f0f0";

    [JsonPropertyName( "colorNeutralBackgroundInvertedDisabled" )]
    public string ColorNeutralBackgroundInvertedDisabled { get; set; } = "rgba(255, 255, 255, 0.1)";

    [JsonPropertyName( "colorNeutralStencil1" )]
    public string ColorNeutralStencil1 { get; set; } = "#e6e6e6";

    [JsonPropertyName( "colorNeutralStencil2" )]
    public string ColorNeutralStencil2 { get; set; } = "#fafafa";

    [JsonPropertyName( "colorNeutralStencil1Alpha" )]
    public string ColorNeutralStencil1Alpha { get; set; } = "rgba(0, 0, 0, 0.1)";

    [JsonPropertyName( "colorNeutralStencil2Alpha" )]
    public string ColorNeutralStencil2Alpha { get; set; } = "rgba(0, 0, 0, 0.05)";

    [JsonPropertyName( "colorBackgroundOverlay" )]
    public string ColorBackgroundOverlay { get; set; } = "rgba(0, 0, 0, 0.4)";

    [JsonPropertyName( "colorScrollbarOverlay" )]
    public string ColorScrollbarOverlay { get; set; } = "rgba(0, 0, 0, 0.5)";

    [JsonPropertyName( "colorBrandBackground" )]
    public string ColorBrandBackground { get; set; } = "#1267b4";

    [JsonPropertyName( "colorBrandBackgroundHover" )]
    public string ColorBrandBackgroundHover { get; set; } = "#18599b";

    [JsonPropertyName( "colorBrandBackgroundPressed" )]
    public string ColorBrandBackgroundPressed { get; set; } = "#193253";

    [JsonPropertyName( "colorBrandBackgroundSelected" )]
    public string ColorBrandBackgroundSelected { get; set; } = "#1b4c82";

    [JsonPropertyName( "colorCompoundBrandBackground" )]
    public string ColorCompoundBrandBackground { get; set; } = "#1267b4";

    [JsonPropertyName( "colorCompoundBrandBackgroundHover" )]
    public string ColorCompoundBrandBackgroundHover { get; set; } = "#18599b";

    [JsonPropertyName( "colorCompoundBrandBackgroundPressed" )]
    public string ColorCompoundBrandBackgroundPressed { get; set; } = "#1b4c82";

    [JsonPropertyName( "colorBrandBackgroundStatic" )]
    public string ColorBrandBackgroundStatic { get; set; } = "#1267b4";

    [JsonPropertyName( "colorBrandBackground2" )]
    public string ColorBrandBackground2 { get; set; } = "#cdd8ef";

    [JsonPropertyName( "colorBrandBackground2Hover" )]
    public string ColorBrandBackground2Hover { get; set; } = "#bac9e9";

    [JsonPropertyName( "colorBrandBackground2Pressed" )]
    public string ColorBrandBackground2Pressed { get; set; } = "#92acdc";

    [JsonPropertyName( "colorBrandBackgroundInverted" )]
    public string ColorBrandBackgroundInverted { get; set; } = "#ffffff";

    [JsonPropertyName( "colorBrandBackgroundInvertedHover" )]
    public string ColorBrandBackgroundInvertedHover { get; set; } = "#cdd8ef";

    [JsonPropertyName( "colorBrandBackgroundInvertedPressed" )]
    public string ColorBrandBackgroundInvertedPressed { get; set; } = "#a6bae2";

    [JsonPropertyName( "colorBrandBackgroundInvertedSelected" )]
    public string ColorBrandBackgroundInvertedSelected { get; set; } = "#bac9e9";

    [JsonPropertyName( "colorNeutralStrokeAccessible" )]
    public string ColorNeutralStrokeAccessible { get; set; } = "#616161";

    [JsonPropertyName( "colorNeutralStrokeAccessibleHover" )]
    public string ColorNeutralStrokeAccessibleHover { get; set; } = "#575757";

    [JsonPropertyName( "colorNeutralStrokeAccessiblePressed" )]
    public string ColorNeutralStrokeAccessiblePressed { get; set; } = "#4d4d4d";

    [JsonPropertyName( "colorNeutralStrokeAccessibleSelected" )]
    public string ColorNeutralStrokeAccessibleSelected { get; set; } = "#1267b4";

    [JsonPropertyName( "colorNeutralStroke1" )]
    public string ColorNeutralStroke1 { get; set; } = "#d1d1d1";

    [JsonPropertyName( "colorNeutralStroke1Hover" )]
    public string ColorNeutralStroke1Hover { get; set; } = "#c7c7c7";

    [JsonPropertyName( "colorNeutralStroke1Pressed" )]
    public string ColorNeutralStroke1Pressed { get; set; } = "#b3b3b3";

    [JsonPropertyName( "colorNeutralStroke1Selected" )]
    public string ColorNeutralStroke1Selected { get; set; } = "#bdbdbd";

    [JsonPropertyName( "colorNeutralStroke2" )]
    public string ColorNeutralStroke2 { get; set; } = "#e0e0e0";

    [JsonPropertyName( "colorNeutralStroke3" )]
    public string ColorNeutralStroke3 { get; set; } = "#f0f0f0";

    [JsonPropertyName( "colorNeutralStrokeSubtle" )]
    public string ColorNeutralStrokeSubtle { get; set; } = "#e0e0e0";

    [JsonPropertyName( "colorNeutralStrokeOnBrand" )]
    public string ColorNeutralStrokeOnBrand { get; set; } = "#ffffff";

    [JsonPropertyName( "colorNeutralStrokeOnBrand2" )]
    public string ColorNeutralStrokeOnBrand2 { get; set; } = "#ffffff";

    [JsonPropertyName( "colorNeutralStrokeOnBrand2Hover" )]
    public string ColorNeutralStrokeOnBrand2Hover { get; set; } = "#ffffff";

    [JsonPropertyName( "colorNeutralStrokeOnBrand2Pressed" )]
    public string ColorNeutralStrokeOnBrand2Pressed { get; set; } = "#ffffff";

    [JsonPropertyName( "colorNeutralStrokeOnBrand2Selected" )]
    public string ColorNeutralStrokeOnBrand2Selected { get; set; } = "#ffffff";

    [JsonPropertyName( "colorBrandStroke1" )]
    public string ColorBrandStroke1 { get; set; } = "#1267b4";

    [JsonPropertyName( "colorBrandStroke2" )]
    public string ColorBrandStroke2 { get; set; } = "#a6bae2";

    [JsonPropertyName( "colorBrandStroke2Hover" )]
    public string ColorBrandStroke2Hover { get; set; } = "#7d9ed5";

    [JsonPropertyName( "colorBrandStroke2Pressed" )]
    public string ColorBrandStroke2Pressed { get; set; } = "#1267b4";

    [JsonPropertyName( "colorBrandStroke2Contrast" )]
    public string ColorBrandStroke2Contrast { get; set; } = "#a6bae2";

    [JsonPropertyName( "colorCompoundBrandStroke" )]
    public string ColorCompoundBrandStroke { get; set; } = "#1267b4";

    [JsonPropertyName( "colorCompoundBrandStrokeHover" )]
    public string ColorCompoundBrandStrokeHover { get; set; } = "#18599b";

    [JsonPropertyName( "colorCompoundBrandStrokePressed" )]
    public string ColorCompoundBrandStrokePressed { get; set; } = "#1b4c82";

    [JsonPropertyName( "colorNeutralStrokeDisabled" )]
    public string ColorNeutralStrokeDisabled { get; set; } = "#e0e0e0";

    [JsonPropertyName( "colorNeutralStrokeInvertedDisabled" )]
    public string ColorNeutralStrokeInvertedDisabled { get; set; } = "rgba(255, 255, 255, 0.4)";

    [JsonPropertyName( "colorTransparentStroke" )]
    public string ColorTransparentStroke { get; set; } = "transparent";

    [JsonPropertyName( "colorTransparentStrokeInteractive" )]
    public string ColorTransparentStrokeInteractive { get; set; } = "transparent";

    [JsonPropertyName( "colorTransparentStrokeDisabled" )]
    public string ColorTransparentStrokeDisabled { get; set; } = "transparent";

    [JsonPropertyName( "colorNeutralStrokeAlpha" )]
    public string ColorNeutralStrokeAlpha { get; set; } = "rgba(0, 0, 0, 0.05)";

    [JsonPropertyName( "colorNeutralStrokeAlpha2" )]
    public string ColorNeutralStrokeAlpha2 { get; set; } = "rgba(255, 255, 255, 0.2)";

    [JsonPropertyName( "colorStrokeFocus1" )]
    public string ColorStrokeFocus1 { get; set; } = "#ffffff";

    [JsonPropertyName( "colorStrokeFocus2" )]
    public string ColorStrokeFocus2 { get; set; } = "#000000";

    [JsonPropertyName( "colorNeutralShadowAmbient" )]
    public string ColorNeutralShadowAmbient { get; set; } = "rgba(0,0,0,0.12)";

    [JsonPropertyName( "colorNeutralShadowKey" )]
    public string ColorNeutralShadowKey { get; set; } = "rgba(0,0,0,0.14)";

    [JsonPropertyName( "colorNeutralShadowAmbientLighter" )]
    public string ColorNeutralShadowAmbientLighter { get; set; } = "rgba(0,0,0,0.06)";

    [JsonPropertyName( "colorNeutralShadowKeyLighter" )]
    public string ColorNeutralShadowKeyLighter { get; set; } = "rgba(0,0,0,0.07)";

    [JsonPropertyName( "colorNeutralShadowAmbientDarker" )]
    public string ColorNeutralShadowAmbientDarker { get; set; } = "rgba(0,0,0,0.20)";

    [JsonPropertyName( "colorNeutralShadowKeyDarker" )]
    public string ColorNeutralShadowKeyDarker { get; set; } = "rgba(0,0,0,0.24)";

    [JsonPropertyName( "colorBrandShadowAmbient" )]
    public string ColorBrandShadowAmbient { get; set; } = "rgba(0,0,0,0.30)";

    [JsonPropertyName( "colorBrandShadowKey" )]
    public string ColorBrandShadowKey { get; set; } = "rgba(0,0,0,0.25)";

    [JsonPropertyName( "colorPaletteRedBackground1" )]
    public string ColorPaletteRedBackground1 { get; set; } = "#fdf6f6";

    [JsonPropertyName( "colorPaletteRedBackground2" )]
    public string ColorPaletteRedBackground2 { get; set; } = "#f1bbbc";

    [JsonPropertyName( "colorPaletteRedBackground3" )]
    public string ColorPaletteRedBackground3 { get; set; } = "#d13438";

    [JsonPropertyName( "colorPaletteRedForeground1" )]
    public string ColorPaletteRedForeground1 { get; set; } = "#bc2f32";

    [JsonPropertyName( "colorPaletteRedForeground2" )]
    public string ColorPaletteRedForeground2 { get; set; } = "#751d1f";

    [JsonPropertyName( "colorPaletteRedForeground3" )]
    public string ColorPaletteRedForeground3 { get; set; } = "#d13438";

    [JsonPropertyName( "colorPaletteRedBorderActive" )]
    public string ColorPaletteRedBorderActive { get; set; } = "#d13438";

    [JsonPropertyName( "colorPaletteRedBorder1" )]
    public string ColorPaletteRedBorder1 { get; set; } = "#f1bbbc";

    [JsonPropertyName( "colorPaletteRedBorder2" )]
    public string ColorPaletteRedBorder2 { get; set; } = "#d13438";

    [JsonPropertyName( "colorPaletteGreenBackground1" )]
    public string ColorPaletteGreenBackground1 { get; set; } = "#f1faf1";

    [JsonPropertyName( "colorPaletteGreenBackground2" )]
    public string ColorPaletteGreenBackground2 { get; set; } = "#9fd89f";

    [JsonPropertyName( "colorPaletteGreenBackground3" )]
    public string ColorPaletteGreenBackground3 { get; set; } = "#107c10";

    [JsonPropertyName( "colorPaletteGreenForeground1" )]
    public string ColorPaletteGreenForeground1 { get; set; } = "#0e700e";

    [JsonPropertyName( "colorPaletteGreenForeground2" )]
    public string ColorPaletteGreenForeground2 { get; set; } = "#094509";

    [JsonPropertyName( "colorPaletteGreenForeground3" )]
    public string ColorPaletteGreenForeground3 { get; set; } = "#107c10";

    [JsonPropertyName( "colorPaletteGreenBorderActive" )]
    public string ColorPaletteGreenBorderActive { get; set; } = "#107c10";

    [JsonPropertyName( "colorPaletteGreenBorder1" )]
    public string ColorPaletteGreenBorder1 { get; set; } = "#9fd89f";

    [JsonPropertyName( "colorPaletteGreenBorder2" )]
    public string ColorPaletteGreenBorder2 { get; set; } = "#107c10";

    [JsonPropertyName( "colorPaletteDarkOrangeBackground1" )]
    public string ColorPaletteDarkOrangeBackground1 { get; set; } = "#fdf6f3";

    [JsonPropertyName( "colorPaletteDarkOrangeBackground2" )]
    public string ColorPaletteDarkOrangeBackground2 { get; set; } = "#f4bfab";

    [JsonPropertyName( "colorPaletteDarkOrangeBackground3" )]
    public string ColorPaletteDarkOrangeBackground3 { get; set; } = "#da3b01";

    [JsonPropertyName( "colorPaletteDarkOrangeForeground1" )]
    public string ColorPaletteDarkOrangeForeground1 { get; set; } = "#c43501";

    [JsonPropertyName( "colorPaletteDarkOrangeForeground2" )]
    public string ColorPaletteDarkOrangeForeground2 { get; set; } = "#7a2101";

    [JsonPropertyName( "colorPaletteDarkOrangeForeground3" )]
    public string ColorPaletteDarkOrangeForeground3 { get; set; } = "#da3b01";

    [JsonPropertyName( "colorPaletteDarkOrangeBorderActive" )]
    public string ColorPaletteDarkOrangeBorderActive { get; set; } = "#da3b01";

    [JsonPropertyName( "colorPaletteDarkOrangeBorder1" )]
    public string ColorPaletteDarkOrangeBorder1 { get; set; } = "#f4bfab";

    [JsonPropertyName( "colorPaletteDarkOrangeBorder2" )]
    public string ColorPaletteDarkOrangeBorder2 { get; set; } = "#da3b01";

    [JsonPropertyName( "colorPaletteYellowBackground1" )]
    public string ColorPaletteYellowBackground1 { get; set; } = "#fffef5";

    [JsonPropertyName( "colorPaletteYellowBackground2" )]
    public string ColorPaletteYellowBackground2 { get; set; } = "#fef7b2";

    [JsonPropertyName( "colorPaletteYellowBackground3" )]
    public string ColorPaletteYellowBackground3 { get; set; } = "#fde300";

    [JsonPropertyName( "colorPaletteYellowForeground1" )]
    public string ColorPaletteYellowForeground1 { get; set; } = "#817400";

    [JsonPropertyName( "colorPaletteYellowForeground2" )]
    public string ColorPaletteYellowForeground2 { get; set; } = "#817400";

    [JsonPropertyName( "colorPaletteYellowForeground3" )]
    public string ColorPaletteYellowForeground3 { get; set; } = "#fde300";

    [JsonPropertyName( "colorPaletteYellowBorderActive" )]
    public string ColorPaletteYellowBorderActive { get; set; } = "#fde300";

    [JsonPropertyName( "colorPaletteYellowBorder1" )]
    public string ColorPaletteYellowBorder1 { get; set; } = "#fef7b2";

    [JsonPropertyName( "colorPaletteYellowBorder2" )]
    public string ColorPaletteYellowBorder2 { get; set; } = "#fde300";

    [JsonPropertyName( "colorPaletteBerryBackground1" )]
    public string ColorPaletteBerryBackground1 { get; set; } = "#fdf5fc";

    [JsonPropertyName( "colorPaletteBerryBackground2" )]
    public string ColorPaletteBerryBackground2 { get; set; } = "#edbbe7";

    [JsonPropertyName( "colorPaletteBerryBackground3" )]
    public string ColorPaletteBerryBackground3 { get; set; } = "#c239b3";

    [JsonPropertyName( "colorPaletteBerryForeground1" )]
    public string ColorPaletteBerryForeground1 { get; set; } = "#af33a1";

    [JsonPropertyName( "colorPaletteBerryForeground2" )]
    public string ColorPaletteBerryForeground2 { get; set; } = "#6d2064";

    [JsonPropertyName( "colorPaletteBerryForeground3" )]
    public string ColorPaletteBerryForeground3 { get; set; } = "#c239b3";

    [JsonPropertyName( "colorPaletteBerryBorderActive" )]
    public string ColorPaletteBerryBorderActive { get; set; } = "#c239b3";

    [JsonPropertyName( "colorPaletteBerryBorder1" )]
    public string ColorPaletteBerryBorder1 { get; set; } = "#edbbe7";

    [JsonPropertyName( "colorPaletteBerryBorder2" )]
    public string ColorPaletteBerryBorder2 { get; set; } = "#c239b3";

    [JsonPropertyName( "colorPaletteLightGreenBackground1" )]
    public string ColorPaletteLightGreenBackground1 { get; set; } = "#f2fbf2";

    [JsonPropertyName( "colorPaletteLightGreenBackground2" )]
    public string ColorPaletteLightGreenBackground2 { get; set; } = "#a7e3a5";

    [JsonPropertyName( "colorPaletteLightGreenBackground3" )]
    public string ColorPaletteLightGreenBackground3 { get; set; } = "#13a10e";

    [JsonPropertyName( "colorPaletteLightGreenForeground1" )]
    public string ColorPaletteLightGreenForeground1 { get; set; } = "#11910d";

    [JsonPropertyName( "colorPaletteLightGreenForeground2" )]
    public string ColorPaletteLightGreenForeground2 { get; set; } = "#0b5a08";

    [JsonPropertyName( "colorPaletteLightGreenForeground3" )]
    public string ColorPaletteLightGreenForeground3 { get; set; } = "#13a10e";

    [JsonPropertyName( "colorPaletteLightGreenBorderActive" )]
    public string ColorPaletteLightGreenBorderActive { get; set; } = "#13a10e";

    [JsonPropertyName( "colorPaletteLightGreenBorder1" )]
    public string ColorPaletteLightGreenBorder1 { get; set; } = "#a7e3a5";

    [JsonPropertyName( "colorPaletteLightGreenBorder2" )]
    public string ColorPaletteLightGreenBorder2 { get; set; } = "#13a10e";

    [JsonPropertyName( "colorPaletteMarigoldBackground1" )]
    public string ColorPaletteMarigoldBackground1 { get; set; } = "#fefbf4";

    [JsonPropertyName( "colorPaletteMarigoldBackground2" )]
    public string ColorPaletteMarigoldBackground2 { get; set; } = "#f9e2ae";

    [JsonPropertyName( "colorPaletteMarigoldBackground3" )]
    public string ColorPaletteMarigoldBackground3 { get; set; } = "#eaa300";

    [JsonPropertyName( "colorPaletteMarigoldForeground1" )]
    public string ColorPaletteMarigoldForeground1 { get; set; } = "#d39300";

    [JsonPropertyName( "colorPaletteMarigoldForeground2" )]
    public string ColorPaletteMarigoldForeground2 { get; set; } = "#835b00";

    [JsonPropertyName( "colorPaletteMarigoldForeground3" )]
    public string ColorPaletteMarigoldForeground3 { get; set; } = "#eaa300";

    [JsonPropertyName( "colorPaletteMarigoldBorderActive" )]
    public string ColorPaletteMarigoldBorderActive { get; set; } = "#eaa300";

    [JsonPropertyName( "colorPaletteMarigoldBorder1" )]
    public string ColorPaletteMarigoldBorder1 { get; set; } = "#f9e2ae";

    [JsonPropertyName( "colorPaletteMarigoldBorder2" )]
    public string ColorPaletteMarigoldBorder2 { get; set; } = "#eaa300";

    [JsonPropertyName( "colorPaletteRedForegroundInverted" )]
    public string ColorPaletteRedForegroundInverted { get; set; } = "#dc5e62";

    [JsonPropertyName( "colorPaletteGreenForegroundInverted" )]
    public string ColorPaletteGreenForegroundInverted { get; set; } = "#359b35";

    [JsonPropertyName( "colorPaletteYellowForegroundInverted" )]
    public string ColorPaletteYellowForegroundInverted { get; set; } = "#fef7b2";

    [JsonPropertyName( "colorPaletteDarkRedBackground2" )]
    public string ColorPaletteDarkRedBackground2 { get; set; } = "#d69ca5";

    [JsonPropertyName( "colorPaletteDarkRedForeground2" )]
    public string ColorPaletteDarkRedForeground2 { get; set; } = "#420610";

    [JsonPropertyName( "colorPaletteDarkRedBorderActive" )]
    public string ColorPaletteDarkRedBorderActive { get; set; } = "#750b1c";

    [JsonPropertyName( "colorPaletteCranberryBackground2" )]
    public string ColorPaletteCranberryBackground2 { get; set; } = "#eeacb2";

    [JsonPropertyName( "colorPaletteCranberryForeground2" )]
    public string ColorPaletteCranberryForeground2 { get; set; } = "#6e0811";

    [JsonPropertyName( "colorPaletteCranberryBorderActive" )]
    public string ColorPaletteCranberryBorderActive { get; set; } = "#c50f1f";

    [JsonPropertyName( "colorPalettePumpkinBackground2" )]
    public string ColorPalettePumpkinBackground2 { get; set; } = "#efc4ad";

    [JsonPropertyName( "colorPalettePumpkinForeground2" )]
    public string ColorPalettePumpkinForeground2 { get; set; } = "#712d09";

    [JsonPropertyName( "colorPalettePumpkinBorderActive" )]
    public string ColorPalettePumpkinBorderActive { get; set; } = "#ca5010";

    [JsonPropertyName( "colorPalettePeachBackground2" )]
    public string ColorPalettePeachBackground2 { get; set; } = "#ffddb3";

    [JsonPropertyName( "colorPalettePeachForeground2" )]
    public string ColorPalettePeachForeground2 { get; set; } = "#8f4e00";

    [JsonPropertyName( "colorPalettePeachBorderActive" )]
    public string ColorPalettePeachBorderActive { get; set; } = "#ff8c00";

    [JsonPropertyName( "colorPaletteGoldBackground2" )]
    public string ColorPaletteGoldBackground2 { get; set; } = "#ecdfa5";

    [JsonPropertyName( "colorPaletteGoldForeground2" )]
    public string ColorPaletteGoldForeground2 { get; set; } = "#6c5700";

    [JsonPropertyName( "colorPaletteGoldBorderActive" )]
    public string ColorPaletteGoldBorderActive { get; set; } = "#c19c00";

    [JsonPropertyName( "colorPaletteBrassBackground2" )]
    public string ColorPaletteBrassBackground2 { get; set; } = "#e0cea2";

    [JsonPropertyName( "colorPaletteBrassForeground2" )]
    public string ColorPaletteBrassForeground2 { get; set; } = "#553e06";

    [JsonPropertyName( "colorPaletteBrassBorderActive" )]
    public string ColorPaletteBrassBorderActive { get; set; } = "#986f0b";

    [JsonPropertyName( "colorPaletteBrownBackground2" )]
    public string ColorPaletteBrownBackground2 { get; set; } = "#ddc3b0";

    [JsonPropertyName( "colorPaletteBrownForeground2" )]
    public string ColorPaletteBrownForeground2 { get; set; } = "#50301a";

    [JsonPropertyName( "colorPaletteBrownBorderActive" )]
    public string ColorPaletteBrownBorderActive { get; set; } = "#8e562e";

    [JsonPropertyName( "colorPaletteForestBackground2" )]
    public string ColorPaletteForestBackground2 { get; set; } = "#bdd99b";

    [JsonPropertyName( "colorPaletteForestForeground2" )]
    public string ColorPaletteForestForeground2 { get; set; } = "#294903";

    [JsonPropertyName( "colorPaletteForestBorderActive" )]
    public string ColorPaletteForestBorderActive { get; set; } = "#498205";

    [JsonPropertyName( "colorPaletteSeafoamBackground2" )]
    public string ColorPaletteSeafoamBackground2 { get; set; } = "#a8f0cd";

    [JsonPropertyName( "colorPaletteSeafoamForeground2" )]
    public string ColorPaletteSeafoamForeground2 { get; set; } = "#00723b";

    [JsonPropertyName( "colorPaletteSeafoamBorderActive" )]
    public string ColorPaletteSeafoamBorderActive { get; set; } = "#00cc6a";

    [JsonPropertyName( "colorPaletteDarkGreenBackground2" )]
    public string ColorPaletteDarkGreenBackground2 { get; set; } = "#9ad29a";

    [JsonPropertyName( "colorPaletteDarkGreenForeground2" )]
    public string ColorPaletteDarkGreenForeground2 { get; set; } = "#063b06";

    [JsonPropertyName( "colorPaletteDarkGreenBorderActive" )]
    public string ColorPaletteDarkGreenBorderActive { get; set; } = "#0b6a0b";

    [JsonPropertyName( "colorPaletteLightTealBackground2" )]
    public string ColorPaletteLightTealBackground2 { get; set; } = "#a6e9ed";

    [JsonPropertyName( "colorPaletteLightTealForeground2" )]
    public string ColorPaletteLightTealForeground2 { get; set; } = "#00666d";

    [JsonPropertyName( "colorPaletteLightTealBorderActive" )]
    public string ColorPaletteLightTealBorderActive { get; set; } = "#00b7c3";

    [JsonPropertyName( "colorPaletteTealBackground2" )]
    public string ColorPaletteTealBackground2 { get; set; } = "#9bd9db";

    [JsonPropertyName( "colorPaletteTealForeground2" )]
    public string ColorPaletteTealForeground2 { get; set; } = "#02494c";

    [JsonPropertyName( "colorPaletteTealBorderActive" )]
    public string ColorPaletteTealBorderActive { get; set; } = "#038387";

    [JsonPropertyName( "colorPaletteSteelBackground2" )]
    public string ColorPaletteSteelBackground2 { get; set; } = "#94c8d4";

    [JsonPropertyName( "colorPaletteSteelForeground2" )]
    public string ColorPaletteSteelForeground2 { get; set; } = "#00333f";

    [JsonPropertyName( "colorPaletteSteelBorderActive" )]
    public string ColorPaletteSteelBorderActive { get; set; } = "#005b70";

    [JsonPropertyName( "colorPaletteBlueBackground2" )]
    public string ColorPaletteBlueBackground2 { get; set; } = "#a9d3f2";

    [JsonPropertyName( "colorPaletteBlueForeground2" )]
    public string ColorPaletteBlueForeground2 { get; set; } = "#004377";

    [JsonPropertyName( "colorPaletteBlueBorderActive" )]
    public string ColorPaletteBlueBorderActive { get; set; } = "#0078d4";

    [JsonPropertyName( "colorPaletteRoyalBlueBackground2" )]
    public string ColorPaletteRoyalBlueBackground2 { get; set; } = "#9abfdc";

    [JsonPropertyName( "colorPaletteRoyalBlueForeground2" )]
    public string ColorPaletteRoyalBlueForeground2 { get; set; } = "#002c4e";

    [JsonPropertyName( "colorPaletteRoyalBlueBorderActive" )]
    public string ColorPaletteRoyalBlueBorderActive { get; set; } = "#004e8c";

    [JsonPropertyName( "colorPaletteCornflowerBackground2" )]
    public string ColorPaletteCornflowerBackground2 { get; set; } = "#c8d1fa";

    [JsonPropertyName( "colorPaletteCornflowerForeground2" )]
    public string ColorPaletteCornflowerForeground2 { get; set; } = "#2c3c85";

    [JsonPropertyName( "colorPaletteCornflowerBorderActive" )]
    public string ColorPaletteCornflowerBorderActive { get; set; } = "#4f6bed";

    [JsonPropertyName( "colorPaletteNavyBackground2" )]
    public string ColorPaletteNavyBackground2 { get; set; } = "#a3b2e8";

    [JsonPropertyName( "colorPaletteNavyForeground2" )]
    public string ColorPaletteNavyForeground2 { get; set; } = "#001665";

    [JsonPropertyName( "colorPaletteNavyBorderActive" )]
    public string ColorPaletteNavyBorderActive { get; set; } = "#0027b4";

    [JsonPropertyName( "colorPaletteLavenderBackground2" )]
    public string ColorPaletteLavenderBackground2 { get; set; } = "#d2ccf8";

    [JsonPropertyName( "colorPaletteLavenderForeground2" )]
    public string ColorPaletteLavenderForeground2 { get; set; } = "#3f3682";

    [JsonPropertyName( "colorPaletteLavenderBorderActive" )]
    public string ColorPaletteLavenderBorderActive { get; set; } = "#7160e8";

    [JsonPropertyName( "colorPalettePurpleBackground2" )]
    public string ColorPalettePurpleBackground2 { get; set; } = "#c6b1de";

    [JsonPropertyName( "colorPalettePurpleForeground2" )]
    public string ColorPalettePurpleForeground2 { get; set; } = "#341a51";

    [JsonPropertyName( "colorPalettePurpleBorderActive" )]
    public string ColorPalettePurpleBorderActive { get; set; } = "#5c2e91";

    [JsonPropertyName( "colorPaletteGrapeBackground2" )]
    public string ColorPaletteGrapeBackground2 { get; set; } = "#d9a7e0";

    [JsonPropertyName( "colorPaletteGrapeForeground2" )]
    public string ColorPaletteGrapeForeground2 { get; set; } = "#4c0d55";

    [JsonPropertyName( "colorPaletteGrapeBorderActive" )]
    public string ColorPaletteGrapeBorderActive { get; set; } = "#881798";

    [JsonPropertyName( "colorPaletteLilacBackground2" )]
    public string ColorPaletteLilacBackground2 { get; set; } = "#e6bfed";

    [JsonPropertyName( "colorPaletteLilacForeground2" )]
    public string ColorPaletteLilacForeground2 { get; set; } = "#63276d";

    [JsonPropertyName( "colorPaletteLilacBorderActive" )]
    public string ColorPaletteLilacBorderActive { get; set; } = "#b146c2";

    [JsonPropertyName( "colorPalettePinkBackground2" )]
    public string ColorPalettePinkBackground2 { get; set; } = "#f7c0e3";

    [JsonPropertyName( "colorPalettePinkForeground2" )]
    public string ColorPalettePinkForeground2 { get; set; } = "#80215d";

    [JsonPropertyName( "colorPalettePinkBorderActive" )]
    public string ColorPalettePinkBorderActive { get; set; } = "#e43ba6";

    [JsonPropertyName( "colorPaletteMagentaBackground2" )]
    public string ColorPaletteMagentaBackground2 { get; set; } = "#eca5d1";

    [JsonPropertyName( "colorPaletteMagentaForeground2" )]
    public string ColorPaletteMagentaForeground2 { get; set; } = "#6b0043";

    [JsonPropertyName( "colorPaletteMagentaBorderActive" )]
    public string ColorPaletteMagentaBorderActive { get; set; } = "#bf0077";

    [JsonPropertyName( "colorPalettePlumBackground2" )]
    public string ColorPalettePlumBackground2 { get; set; } = "#d696c0";

    [JsonPropertyName( "colorPalettePlumForeground2" )]
    public string ColorPalettePlumForeground2 { get; set; } = "#43002b";

    [JsonPropertyName( "colorPalettePlumBorderActive" )]
    public string ColorPalettePlumBorderActive { get; set; } = "#77004d";

    [JsonPropertyName( "colorPaletteBeigeBackground2" )]
    public string ColorPaletteBeigeBackground2 { get; set; } = "#d7d4d4";

    [JsonPropertyName( "colorPaletteBeigeForeground2" )]
    public string ColorPaletteBeigeForeground2 { get; set; } = "#444241";

    [JsonPropertyName( "colorPaletteBeigeBorderActive" )]
    public string ColorPaletteBeigeBorderActive { get; set; } = "#7a7574";

    [JsonPropertyName( "colorPaletteMinkBackground2" )]
    public string ColorPaletteMinkBackground2 { get; set; } = "#cecccb";

    [JsonPropertyName( "colorPaletteMinkForeground2" )]
    public string ColorPaletteMinkForeground2 { get; set; } = "#343231";

    [JsonPropertyName( "colorPaletteMinkBorderActive" )]
    public string ColorPaletteMinkBorderActive { get; set; } = "#5d5a58";

    [JsonPropertyName( "colorPalettePlatinumBackground2" )]
    public string ColorPalettePlatinumBackground2 { get; set; } = "#cdd6d8";

    [JsonPropertyName( "colorPalettePlatinumForeground2" )]
    public string ColorPalettePlatinumForeground2 { get; set; } = "#3b4447";

    [JsonPropertyName( "colorPalettePlatinumBorderActive" )]
    public string ColorPalettePlatinumBorderActive { get; set; } = "#69797e";

    [JsonPropertyName( "colorPaletteAnchorBackground2" )]
    public string ColorPaletteAnchorBackground2 { get; set; } = "#bcc3c7";

    [JsonPropertyName( "colorPaletteAnchorForeground2" )]
    public string ColorPaletteAnchorForeground2 { get; set; } = "#202427";

    [JsonPropertyName( "colorPaletteAnchorBorderActive" )]
    public string ColorPaletteAnchorBorderActive { get; set; } = "#394146";

    [JsonPropertyName( "colorStatusSuccessBackground1" )]
    public string ColorStatusSuccessBackground1 { get; set; } = "#f1faf1";

    [JsonPropertyName( "colorStatusSuccessBackground2" )]
    public string ColorStatusSuccessBackground2 { get; set; } = "#9fd89f";

    [JsonPropertyName( "colorStatusSuccessBackground3" )]
    public string ColorStatusSuccessBackground3 { get; set; } = "#107c10";

    [JsonPropertyName( "colorStatusSuccessForeground1" )]
    public string ColorStatusSuccessForeground1 { get; set; } = "#0e700e";

    [JsonPropertyName( "colorStatusSuccessForeground2" )]
    public string ColorStatusSuccessForeground2 { get; set; } = "#094509";

    [JsonPropertyName( "colorStatusSuccessForeground3" )]
    public string ColorStatusSuccessForeground3 { get; set; } = "#107c10";

    [JsonPropertyName( "colorStatusSuccessForegroundInverted" )]
    public string ColorStatusSuccessForegroundInverted { get; set; } = "#54b054";

    [JsonPropertyName( "colorStatusSuccessBorderActive" )]
    public string ColorStatusSuccessBorderActive { get; set; } = "#107c10";

    [JsonPropertyName( "colorStatusSuccessBorder1" )]
    public string ColorStatusSuccessBorder1 { get; set; } = "#9fd89f";

    [JsonPropertyName( "colorStatusSuccessBorder2" )]
    public string ColorStatusSuccessBorder2 { get; set; } = "#107c10";

    [JsonPropertyName( "colorStatusWarningBackground1" )]
    public string ColorStatusWarningBackground1 { get; set; } = "#fff9f5";

    [JsonPropertyName( "colorStatusWarningBackground2" )]
    public string ColorStatusWarningBackground2 { get; set; } = "#fdcfb4";

    [JsonPropertyName( "colorStatusWarningBackground3" )]
    public string ColorStatusWarningBackground3 { get; set; } = "#f7630c";

    [JsonPropertyName( "colorStatusWarningForeground1" )]
    public string ColorStatusWarningForeground1 { get; set; } = "#bc4b09";

    [JsonPropertyName( "colorStatusWarningForeground2" )]
    public string ColorStatusWarningForeground2 { get; set; } = "#8a3707";

    [JsonPropertyName( "colorStatusWarningForeground3" )]
    public string ColorStatusWarningForeground3 { get; set; } = "#bc4b09";

    [JsonPropertyName( "colorStatusWarningForegroundInverted" )]
    public string ColorStatusWarningForegroundInverted { get; set; } = "#faa06b";

    [JsonPropertyName( "colorStatusWarningBorderActive" )]
    public string ColorStatusWarningBorderActive { get; set; } = "#f7630c";

    [JsonPropertyName( "colorStatusWarningBorder1" )]
    public string ColorStatusWarningBorder1 { get; set; } = "#fdcfb4";

    [JsonPropertyName( "colorStatusWarningBorder2" )]
    public string ColorStatusWarningBorder2 { get; set; } = "#bc4b09";

    [JsonPropertyName( "colorStatusDangerBackground1" )]
    public string ColorStatusDangerBackground1 { get; set; } = "#fdf3f4";

    [JsonPropertyName( "colorStatusDangerBackground2" )]
    public string ColorStatusDangerBackground2 { get; set; } = "#eeacb2";

    [JsonPropertyName( "colorStatusDangerBackground3" )]
    public string ColorStatusDangerBackground3 { get; set; } = "#c50f1f";

    [JsonPropertyName( "colorStatusDangerForeground1" )]
    public string ColorStatusDangerForeground1 { get; set; } = "#b10e1c";

    [JsonPropertyName( "colorStatusDangerForeground2" )]
    public string ColorStatusDangerForeground2 { get; set; } = "#6e0811";

    [JsonPropertyName( "colorStatusDangerForeground3" )]
    public string ColorStatusDangerForeground3 { get; set; } = "#c50f1f";

    [JsonPropertyName( "colorStatusDangerForegroundInverted" )]
    public string ColorStatusDangerForegroundInverted { get; set; } = "#dc626d";

    [JsonPropertyName( "colorStatusDangerBorderActive" )]
    public string ColorStatusDangerBorderActive { get; set; } = "#c50f1f";

    [JsonPropertyName( "colorStatusDangerBorder1" )]
    public string ColorStatusDangerBorder1 { get; set; } = "#eeacb2";

    [JsonPropertyName( "colorStatusDangerBorder2" )]
    public string ColorStatusDangerBorder2 { get; set; } = "#c50f1f";

    [JsonPropertyName( "shadow2" )]
    public string Shadow2 { get; set; } = "0 0 2px rgba(0,0,0,0.12), 0 1px 2px rgba(0,0,0,0.14)";

    [JsonPropertyName( "shadow4" )]
    public string Shadow4 { get; set; } = "0 0 2px rgba(0,0,0,0.12), 0 2px 4px rgba(0,0,0,0.14)";

    [JsonPropertyName( "shadow8" )]
    public string Shadow8 { get; set; } = "0 0 2px rgba(0,0,0,0.12), 0 4px 8px rgba(0,0,0,0.14)";

    [JsonPropertyName( "shadow16" )]
    public string Shadow16 { get; set; } = "0 0 2px rgba(0,0,0,0.12), 0 8px 16px rgba(0,0,0,0.14)";

    [JsonPropertyName( "shadow28" )]
    public string Shadow28 { get; set; } = "0 0 8px rgba(0,0,0,0.12), 0 14px 28px rgba(0,0,0,0.14)";

    [JsonPropertyName( "shadow64" )]
    public string Shadow64 { get; set; } = "0 0 8px rgba(0,0,0,0.12), 0 32px 64px rgba(0,0,0,0.14)";

    [JsonPropertyName( "shadow2Brand" )]
    public string Shadow2Brand { get; set; } = "0 0 2px rgba(0,0,0,0.30), 0 1px 2px rgba(0,0,0,0.25)";

    [JsonPropertyName( "shadow4Brand" )]
    public string Shadow4Brand { get; set; } = "0 0 2px rgba(0,0,0,0.30), 0 2px 4px rgba(0,0,0,0.25)";

    [JsonPropertyName( "shadow8Brand" )]
    public string Shadow8Brand { get; set; } = "0 0 2px rgba(0,0,0,0.30), 0 4px 8px rgba(0,0,0,0.25)";

    [JsonPropertyName( "shadow16Brand" )]
    public string Shadow16Brand { get; set; } = "0 0 2px rgba(0,0,0,0.30), 0 8px 16px rgba(0,0,0,0.25)";

    [JsonPropertyName( "shadow28Brand" )]
    public string Shadow28Brand { get; set; } = "0 0 8px rgba(0,0,0,0.30), 0 14px 28px rgba(0,0,0,0.25)";

    [JsonPropertyName( "shadow64Brand" )]
    public string Shadow64Brand { get; set; } = "0 0 8px rgba(0,0,0,0.30), 0 32px 64px rgba(0,0,0,0.25)";
};
