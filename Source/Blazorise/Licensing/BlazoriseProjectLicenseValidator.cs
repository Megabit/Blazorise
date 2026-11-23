#region Using directives
using System;
using System.Reflection;
using System.Text.Json;
#endregion

namespace Blazorise.Licensing;

/// <summary>
/// Validates the project metadata from an authenticated Indie ProductToken.
/// </summary>
internal static class BlazoriseProjectLicenseValidator
{
    #region Members

    internal const string ProjectNameProperty = "PROJECT_NAME";

    internal const string ApplicationAssembliesProperty = "APPLICATION_ASSEMBLIES";

    #endregion

    #region Methods

    internal static bool IsValid( string projectName, string applicationAssemblies, Assembly productAssembly )
    {
        if ( string.IsNullOrWhiteSpace( projectName )
            || string.IsNullOrWhiteSpace( applicationAssemblies )
            || productAssembly is null )
        {
            return false;
        }

        var assemblyName = productAssembly.GetName().Name;

        if ( string.IsNullOrWhiteSpace( assemblyName ) )
        {
            return false;
        }

        try
        {
            using var document = JsonDocument.Parse( applicationAssemblies );

            if ( document.RootElement.ValueKind != JsonValueKind.Array )
            {
                return false;
            }

            var hasMatchingAssembly = false;

            foreach ( var element in document.RootElement.EnumerateArray() )
            {
                if ( element.ValueKind != JsonValueKind.String )
                {
                    return false;
                }

                var registeredAssemblyName = element.GetString();

                if ( string.IsNullOrWhiteSpace( registeredAssemblyName ) )
                {
                    return false;
                }

                hasMatchingAssembly |= string.Equals( registeredAssemblyName, assemblyName, StringComparison.Ordinal );
            }

            return hasMatchingAssembly;
        }
        catch ( JsonException )
        {
            return false;
        }
    }

    #endregion
}