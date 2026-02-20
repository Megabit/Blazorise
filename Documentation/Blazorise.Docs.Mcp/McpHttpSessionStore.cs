using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ModelContextProtocol.Server;

namespace Blazorise.Docs.Mcp;

internal sealed class McpHttpSessionStore
{
    private readonly ConcurrentDictionary<string, McpHttpSession> sessions = new( StringComparer.OrdinalIgnoreCase );

    public bool TryAdd( McpHttpSession session )
    {
        return sessions.TryAdd( session.SessionId, session );
    }

    public bool TryGet( string sessionId, out McpHttpSession session )
    {
        return sessions.TryGetValue( sessionId, out session );
    }

    public bool TryRemove( string sessionId, out McpHttpSession session )
    {
        return sessions.TryRemove( sessionId, out session );
    }
}

internal sealed class McpHttpSession : IAsyncDisposable
{
    private readonly IServiceScope scope;

    public McpHttpSession( string sessionId, SseResponseStreamTransport transport, McpServer server, IServiceScope scope )
    {
        SessionId = sessionId ?? throw new ArgumentNullException( nameof( sessionId ) );
        Transport = transport ?? throw new ArgumentNullException( nameof( transport ) );
        Server = server ?? throw new ArgumentNullException( nameof( server ) );
        this.scope = scope ?? throw new ArgumentNullException( nameof( scope ) );
    }

    public string SessionId { get; }

    public SseResponseStreamTransport Transport { get; }

    public McpServer Server { get; }

    public async ValueTask DisposeAsync()
    {
        await Server.DisposeAsync();
        await Transport.DisposeAsync();
        scope.Dispose();
    }
}