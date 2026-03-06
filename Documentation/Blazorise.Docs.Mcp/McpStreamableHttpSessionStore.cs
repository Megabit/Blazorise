using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ModelContextProtocol.Server;

namespace Blazorise.Docs.Mcp;

internal sealed class McpStreamableHttpSessionStore
{
    private readonly ConcurrentDictionary<string, McpStreamableHttpSession> sessions = new( StringComparer.OrdinalIgnoreCase );

    public bool TryAdd( McpStreamableHttpSession session )
    {
        return sessions.TryAdd( session.SessionId, session );
    }

    public bool TryGet( string sessionId, out McpStreamableHttpSession session )
    {
        return sessions.TryGetValue( sessionId, out session );
    }

    public bool TryRemove( string sessionId, out McpStreamableHttpSession session )
    {
        return sessions.TryRemove( sessionId, out session );
    }
}

internal sealed class McpStreamableHttpSession : IAsyncDisposable
{
    private readonly IServiceScope scope;

    public McpStreamableHttpSession(
        string sessionId,
        StreamableHttpServerTransport transport,
        McpServer server,
        IServiceScope scope,
        Task serverTask )
    {
        SessionId = sessionId ?? throw new ArgumentNullException( nameof( sessionId ) );
        Transport = transport ?? throw new ArgumentNullException( nameof( transport ) );
        Server = server ?? throw new ArgumentNullException( nameof( server ) );
        this.scope = scope ?? throw new ArgumentNullException( nameof( scope ) );
        ServerTask = serverTask ?? throw new ArgumentNullException( nameof( serverTask ) );
    }

    public string SessionId { get; }

    public StreamableHttpServerTransport Transport { get; }

    public McpServer Server { get; }

    public Task ServerTask { get; }

    public async ValueTask DisposeAsync()
    {
        await Server.DisposeAsync();
        await Transport.DisposeAsync();
        scope.Dispose();
    }
}