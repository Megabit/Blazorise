#region Using directives
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
#endregion

namespace Blazorise.Reporting.DataSources.WebApi;

internal static class WebApiPublicNetworkGuard
{
    #region Methods

    public static async Task EnsurePublicDestinationAsync( Uri resourceUri, CancellationToken cancellationToken )
    {
        await ResolvePublicAddressesAsync( resourceUri.DnsSafeHost, cancellationToken );
    }

    public static async ValueTask<Stream> ConnectAsync( SocketsHttpConnectionContext context, CancellationToken cancellationToken )
    {
        IReadOnlyList<IPAddress> addresses = await ResolvePublicAddressesAsync( context.DnsEndPoint.Host, cancellationToken );
        SocketException lastException = null;

        foreach ( IPAddress address in addresses )
        {
            Socket socket = new( address.AddressFamily, SocketType.Stream, ProtocolType.Tcp )
            {
                NoDelay = true,
            };

            try
            {
                await socket.ConnectAsync( new IPEndPoint( address, context.DnsEndPoint.Port ), cancellationToken );

                return new NetworkStream( socket, ownsSocket: true );
            }
            catch ( SocketException exception )
            {
                socket.Dispose();
                lastException = exception;
            }
            catch
            {
                socket.Dispose();
                throw;
            }
        }

        throw new HttpRequestException( $"Could not connect to the public Web API host '{context.DnsEndPoint.Host}'.", lastException );
    }

    private static async Task<IReadOnlyList<IPAddress>> ResolvePublicAddressesAsync( string host, CancellationToken cancellationToken )
    {
        if ( string.Equals( host, "localhost", StringComparison.OrdinalIgnoreCase )
             || host.EndsWith( ".localhost", StringComparison.OrdinalIgnoreCase ) )
        {
            throw new InvalidOperationException( "Web API report data source URLs cannot target localhost or a non-public network." );
        }

        IPAddress[] addresses = IPAddress.TryParse( host, out IPAddress address )
            ? [address]
            : await Dns.GetHostAddressesAsync( host, cancellationToken );

        List<IPAddress> publicAddresses = addresses
            .Where( IsPublicAddress )
            .Distinct()
            .ToList();

        if ( publicAddresses.Count == 0 )
            throw new InvalidOperationException( $"Web API report data source host '{host}' resolves only to localhost or a non-public network." );

        return publicAddresses;
    }

    private static bool IsPublicAddress( IPAddress address )
    {
        if ( IPAddress.IsLoopback( address ) )
            return false;

        if ( address.IsIPv4MappedToIPv6 )
            address = address.MapToIPv4();

        byte[] bytes = address.GetAddressBytes();

        if ( address.AddressFamily == AddressFamily.InterNetwork )
            return IsPublicIPv4Address( bytes );

        if ( address.AddressFamily != AddressFamily.InterNetworkV6 )
            return false;

        // Currently allocated global unicast IPv6 space is 2000::/3. Transition, documentation,
        // unique-local, link-local, multicast, and unspecified ranges are rejected.
        return ( bytes[0] & 0xe0 ) == 0x20
            && !( bytes[0] == 0x20 && bytes[1] == 0x01 && bytes[2] == 0x00 && bytes[3] == 0x00 )
            && !( bytes[0] == 0x20 && bytes[1] == 0x01 && bytes[2] == 0x00 && bytes[3] == 0x02 )
            && !( bytes[0] == 0x20 && bytes[1] == 0x01 && bytes[2] == 0x00 && ( bytes[3] & 0xf0 ) == 0x10 )
            && !( bytes[0] == 0x20 && bytes[1] == 0x01 && bytes[2] == 0x0d && bytes[3] == 0xb8 )
            && !( bytes[0] == 0x20 && bytes[1] == 0x02 )
            && !( bytes[0] == 0x3f && ( bytes[1] & 0xf0 ) == 0xf0 );
    }

    private static bool IsPublicIPv4Address( byte[] bytes )
    {
        return bytes[0] != 0
            && bytes[0] != 10
            && bytes[0] != 127
            && !( bytes[0] == 100 && bytes[1] >= 64 && bytes[1] <= 127 )
            && !( bytes[0] == 169 && bytes[1] == 254 )
            && !( bytes[0] == 172 && bytes[1] >= 16 && bytes[1] <= 31 )
            && !( bytes[0] == 192 && bytes[1] == 0 && bytes[2] == 0 )
            && !( bytes[0] == 192 && bytes[1] == 0 && bytes[2] == 2 )
            && !( bytes[0] == 192 && bytes[1] == 88 && bytes[2] == 99 )
            && !( bytes[0] == 192 && bytes[1] == 168 )
            && !( bytes[0] == 198 && ( bytes[1] == 18 || bytes[1] == 19 ) )
            && !( bytes[0] == 198 && bytes[1] == 51 && bytes[2] == 100 )
            && !( bytes[0] == 203 && bytes[1] == 0 && bytes[2] == 113 )
            && bytes[0] < 224;
    }

    #endregion
}