using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Blazorise.Docs.Mcp;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ModelContextProtocol;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;

WebApplicationBuilder builder = WebApplication.CreateBuilder( args );

// Configure all logs to go to stderr (stdout is used for the MCP protocol messages).
builder.Logging.ClearProviders();
builder.Logging.AddConsole( o => o.LogToStandardErrorThreshold = LogLevel.Trace );

IMcpServerBuilder mcpBuilder = builder.Services
    .AddMcpServer()
    .WithTools<DocsTools>();

bool? enableStdioOverride = builder.Configuration.GetValue<bool?>( "Mcp:EnableStdio" );
bool enableStdio = enableStdioOverride ?? Environment.UserInteractive;

if ( enableStdio )
{
    mcpBuilder.WithStdioServerTransport();
}

builder.Services.AddSingleton<McpHttpSessionStore>();

WebApplication app = builder.Build();

app.MapGet( "/mcp/sse", HandleSseAsync );
app.MapPost( "/mcp/message", HandleMessageAsync );

await app.RunAsync();

static async Task HandleSseAsync(
    HttpContext context,
    McpHttpSessionStore sessionStore,
    IOptions<McpServerOptions> serverOptions,
    ILoggerFactory loggerFactory,
    IServiceProvider serviceProvider )
{
    ILogger logger = loggerFactory.CreateLogger( "McpHttp" );

    context.Response.StatusCode = StatusCodes.Status200OK;
    context.Response.ContentType = "text/event-stream";
    context.Response.Headers["Cache-Control"] = "no-cache";
    context.Response.Headers["Connection"] = "keep-alive";
    context.Response.Headers["X-Accel-Buffering"] = "no";

    IHttpResponseBodyFeature bodyFeature = context.Features.Get<IHttpResponseBodyFeature>();
    bodyFeature?.DisableBuffering();

    string sessionId = Guid.NewGuid().ToString( "N" );
    string messageEndpoint = BuildMessageEndpoint( context.Request.PathBase );

    IServiceScope scope = serviceProvider.CreateScope();
    McpServerOptions options = serverOptions.Value;

    SseResponseStreamTransport transport = new SseResponseStreamTransport( context.Response.Body, messageEndpoint, sessionId );
    McpServer server = McpServer.Create( transport, options, loggerFactory, scope.ServiceProvider );
    McpHttpSession session = new McpHttpSession( sessionId, transport, server, scope );

    if ( !sessionStore.TryAdd( session ) )
    {
        await session.DisposeAsync();
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        return;
    }

    Task transportTask = transport.RunAsync( context.RequestAborted );

    await WriteSseHandshakeAsync( context.Response, sessionId, messageEndpoint, context.RequestAborted );
    await context.Response.Body.FlushAsync( context.RequestAborted );

    Task serverTask = server.RunAsync( context.RequestAborted );

    try
    {
        await Task.WhenAny( transportTask, serverTask );
        LogTaskFailure( transportTask, logger, "transport", sessionId );
        LogTaskFailure( serverTask, logger, "server", sessionId );
    }
    finally
    {
        sessionStore.TryRemove( sessionId, out McpHttpSession removedSession );
        await session.DisposeAsync();
    }
}

static async Task HandleMessageAsync( HttpContext context, McpHttpSessionStore sessionStore )
{
    string sessionId = GetSessionId( context.Request );

    if ( string.IsNullOrWhiteSpace( sessionId ) )
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        return;
    }

    if ( !sessionStore.TryGet( sessionId, out McpHttpSession session ) )
    {
        context.Response.StatusCode = StatusCodes.Status404NotFound;
        return;
    }

    JsonRpcMessage message = await JsonSerializer.DeserializeAsync<JsonRpcMessage>(
        context.Request.Body,
        McpJsonUtilities.DefaultOptions,
        context.RequestAborted );

    if ( message is null )
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        return;
    }

    if ( message.Context is null )
    {
        message.Context = new JsonRpcMessageContext();
    }

    message.Context.User = context.User;

    await session.Transport.OnMessageReceivedAsync( message, context.RequestAborted );

    context.Response.StatusCode = StatusCodes.Status202Accepted;
}

static string GetSessionId( HttpRequest request )
{
    string sessionId = request.Headers["mcp-session-id"].ToString();

    if ( !string.IsNullOrWhiteSpace( sessionId ) )
        return sessionId;

    sessionId = request.Headers["Mcp-Session-Id"].ToString();

    if ( !string.IsNullOrWhiteSpace( sessionId ) )
        return sessionId;

    sessionId = request.Query["sessionId"].ToString();

    if ( !string.IsNullOrWhiteSpace( sessionId ) )
        return sessionId;

    sessionId = request.Query["session_id"].ToString();

    return sessionId;
}

static string BuildMessageEndpoint( PathString pathBase )
{
    if ( pathBase.HasValue )
        return $"{pathBase.Value}/mcp/message";

    return "/mcp/message";
}

static async Task WriteSseHandshakeAsync(
    HttpResponse response,
    string sessionId,
    string messageEndpoint,
    CancellationToken cancellationToken )
{
    Dictionary<string, string> payload = new Dictionary<string, string>( StringComparer.Ordinal )
    {
        ["sessionId"] = sessionId,
        ["endpoint"] = messageEndpoint
    };

    string json = JsonSerializer.Serialize( payload );
    string sse = $"event: endpoint\n" +
                 $"data: {json}\n\n";

    byte[] bytes = Encoding.UTF8.GetBytes( sse );
    await response.Body.WriteAsync( bytes, 0, bytes.Length, cancellationToken );
}

static void LogTaskFailure( Task task, ILogger logger, string taskName, string sessionId )
{
    if ( task is null || !task.IsFaulted || task.Exception is null )
        return;

    Exception exception = task.Exception.GetBaseException();
    logger.LogError( exception, "MCP {TaskName} faulted for session {SessionId}.", taskName, sessionId );
}