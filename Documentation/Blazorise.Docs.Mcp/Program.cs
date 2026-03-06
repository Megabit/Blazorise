using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Blazorise.Docs.Mcp;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
builder.Services.AddSingleton<McpStreamableHttpSessionStore>();

WebApplication app = builder.Build();

app.MapPost( "/mcp", HandleStreamableHttpPostAsync );
app.MapGet( "/mcp", HandleStreamableHttpGetAsync );
app.MapDelete( "/mcp", HandleStreamableHttpDeleteAsync );
app.MapGet( "/mcp/sse", HandleSseAsync );
app.MapPost( "/mcp/message", HandleMessageAsync );

await app.RunAsync();

static async Task HandleStreamableHttpPostAsync(
    HttpContext context,
    McpStreamableHttpSessionStore sessionStore,
    IOptions<McpServerOptions> serverOptions,
    ILoggerFactory loggerFactory,
    IServiceProvider serviceProvider )
{
    ILogger logger = loggerFactory.CreateLogger( "McpStreamableHttp" );

    JsonRpcMessage message = await DeserializeMessageAsync( context );

    if ( message is null )
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        return;
    }

    message.Context ??= new JsonRpcMessageContext();
    message.Context.User = context.User;

    string sessionId = GetSessionId( context.Request );
    if ( string.IsNullOrWhiteSpace( sessionId ) )
    {
        sessionId = Guid.NewGuid().ToString( "N" );
    }

    McpStreamableHttpSession session = await GetOrCreateStreamableSessionAsync(
        sessionStore,
        sessionId,
        serverOptions.Value,
        loggerFactory,
        serviceProvider );

    if ( session is null )
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        return;
    }

    if ( session.ServerTask.IsCanceled || session.ServerTask.IsCompleted || session.ServerTask.IsFaulted )
    {
        LogTaskFailure( session.ServerTask, logger, "server", session.SessionId );
        sessionStore.TryRemove( session.SessionId, out McpStreamableHttpSession removedSession );
        await session.DisposeAsync();
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        return;
    }

    context.Response.Headers["mcp-session-id"] = session.SessionId;
    context.Response.ContentType = "application/json";

    bool responseWritten;

    try
    {
        responseWritten = await session.Transport.HandlePostRequestAsync( message, context.Response.Body, context.RequestAborted );
    }
    catch ( Exception exception )
    {
        logger.LogError( exception, "MCP streamable HTTP POST failed for session {SessionId}.", session.SessionId );
        sessionStore.TryRemove( session.SessionId, out McpStreamableHttpSession removedSession );
        await session.DisposeAsync();
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        return;
    }

    if ( !responseWritten )
    {
        context.Response.StatusCode = StatusCodes.Status202Accepted;
    }
}

static async Task HandleStreamableHttpGetAsync(
    HttpContext context,
    McpStreamableHttpSessionStore sessionStore )
{
    string sessionId = GetSessionId( context.Request );

    if ( string.IsNullOrWhiteSpace( sessionId ) )
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        return;
    }

    if ( !sessionStore.TryGet( sessionId, out McpStreamableHttpSession session ) )
    {
        context.Response.StatusCode = StatusCodes.Status404NotFound;
        return;
    }

    context.Response.StatusCode = StatusCodes.Status200OK;
    context.Response.ContentType = "text/event-stream";
    context.Response.Headers.CacheControl = "no-cache";
    context.Response.Headers.Connection = "keep-alive";
    context.Response.Headers["X-Accel-Buffering"] = "no";

    IHttpResponseBodyFeature bodyFeature = context.Features.Get<IHttpResponseBodyFeature>();
    bodyFeature?.DisableBuffering();

    context.Response.Headers["mcp-session-id"] = session.SessionId;

    await session.Transport.HandleGetRequestAsync( context.Response.Body, context.RequestAborted );
}

static async Task HandleStreamableHttpDeleteAsync(
    HttpContext context,
    McpStreamableHttpSessionStore sessionStore )
{
    string sessionId = GetSessionId( context.Request );

    if ( string.IsNullOrWhiteSpace( sessionId ) )
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        return;
    }

    if ( sessionStore.TryRemove( sessionId, out McpStreamableHttpSession session ) )
    {
        await session.DisposeAsync();
    }

    context.Response.StatusCode = StatusCodes.Status204NoContent;
}

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
    context.Response.Headers.CacheControl = "no-cache";
    context.Response.Headers.Connection = "keep-alive";
    context.Response.Headers["X-Accel-Buffering"] = "no";

    IHttpResponseBodyFeature bodyFeature = context.Features.Get<IHttpResponseBodyFeature>();
    bodyFeature?.DisableBuffering();

    string sessionId = Guid.NewGuid().ToString( "N" );
    string messageEndpoint = BuildMessageEndpoint( context.Request.PathBase, sessionId );

    IServiceScope scope = serviceProvider.CreateScope();
    McpServerOptions options = serverOptions.Value;

    var transport = new SseResponseStreamTransport( context.Response.Body, messageEndpoint, sessionId );
    var server = McpServer.Create( transport, options, loggerFactory, scope.ServiceProvider );
    var session = new McpHttpSession( sessionId, transport, server, scope );

    if ( !sessionStore.TryAdd( session ) )
    {
        await session.DisposeAsync();
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        return;
    }

    Task transportTask = transport.RunAsync( context.RequestAborted );

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

    JsonRpcMessage message = await DeserializeMessageAsync( context );

    if ( message is null )
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        return;
    }

    message.Context ??= new JsonRpcMessageContext();

    message.Context.User = context.User;

    await session.Transport.OnMessageReceivedAsync( message, context.RequestAborted );

    context.Response.StatusCode = StatusCodes.Status202Accepted;
}

static async Task<JsonRpcMessage> DeserializeMessageAsync( HttpContext context )
{
    try
    {
        JsonRpcMessage message = await JsonSerializer.DeserializeAsync<JsonRpcMessage>(
            context.Request.Body,
            McpJsonUtilities.DefaultOptions,
            context.RequestAborted );

        return message;
    }
    catch ( JsonException )
    {
        return null;
    }
}

static async Task<McpStreamableHttpSession> GetOrCreateStreamableSessionAsync(
    McpStreamableHttpSessionStore sessionStore,
    string sessionId,
    McpServerOptions serverOptions,
    ILoggerFactory loggerFactory,
    IServiceProvider serviceProvider )
{
    McpStreamableHttpSession session;

    if ( sessionStore.TryGet( sessionId, out session ) )
    {
        return session;
    }

    IServiceScope scope = serviceProvider.CreateScope();

    StreamableHttpServerTransport transport = new StreamableHttpServerTransport
    {
        SessionId = sessionId,
        FlowExecutionContextFromRequests = true
    };

    McpServer server = McpServer.Create( transport, serverOptions, loggerFactory, scope.ServiceProvider );
    Task serverTask = server.RunAsync( CancellationToken.None );

    McpStreamableHttpSession createdSession = new McpStreamableHttpSession(
        sessionId,
        transport,
        server,
        scope,
        serverTask );

    if ( sessionStore.TryAdd( createdSession ) )
    {
        return createdSession;
    }

    await createdSession.DisposeAsync();

    if ( sessionStore.TryGet( sessionId, out session ) )
    {
        return session;
    }

    return null;
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

static string BuildMessageEndpoint( PathString pathBase, string sessionId )
{
    string encodedSessionId = Uri.EscapeDataString( sessionId );

    if ( pathBase.HasValue )
        return $"{pathBase.Value}/mcp/message?sessionId={encodedSessionId}";

    return $"/mcp/message?sessionId={encodedSessionId}";
}

static void LogTaskFailure( Task task, ILogger logger, string taskName, string sessionId )
{
    if ( task is null || !task.IsFaulted || task.Exception is null )
        return;

    Exception exception = task.Exception.GetBaseException();
    logger.LogError( exception, "MCP {TaskName} faulted for session {SessionId}.", taskName, sessionId );
}