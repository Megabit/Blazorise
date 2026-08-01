#region Using directives
using System;
#endregion

namespace Blazorise.Utilities;

/// <summary>
/// Detects the mobile user agents supported by the legacy picker implementation.
/// </summary>
internal static class MobileDeviceDetector
{
    private static readonly string[] MobileUserAgentIdentifiers =
    {
        "Android",
        "webOS",
        "iPhone",
        "iPad",
        "iPod",
        "BlackBerry",
        "IEMobile",
        "Opera Mini",
        "Opera Mobi",
        "Mobile",
        "Silk",
        "Kindle",
        "Windows Phone",
        "PlayBook",
        "BB10",
        "MeeGo",
        "Tizen",
        "Puffin",
    };

    /// <summary>
    /// Determines whether the supplied browser user agent represents a mobile device.
    /// </summary>
    /// <param name="userAgent">Browser user-agent value.</param>
    /// <returns><see langword="true"/> when the user agent matches a supported mobile device.</returns>
    public static bool IsMobile( string userAgent )
    {
        if ( string.IsNullOrWhiteSpace( userAgent ) )
            return false;

        foreach ( string identifier in MobileUserAgentIdentifiers )
        {
            if ( userAgent.Contains( identifier, StringComparison.OrdinalIgnoreCase ) )
                return true;
        }

        return false;
    }
}