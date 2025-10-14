using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Blazorise.Docs.BlogRuntime;

public interface IBlogProvider
{
    Task<IReadOnlyList<BlogIndexItem>> GetListAsync( CancellationToken ct = default );
    Task<BlogPageModel> GetBySlugAsync( string slug, CancellationToken ct = default );
    Task InvalidateAsync( string slug = null );
}

public sealed class GithubBlogProvider : IBlogProvider
{
    private readonly HttpClient http;
    private readonly IMemoryCache cache;
    private readonly BlogOptions opt;

    private const string UA = "BlazoriseDocsBlogRuntime/1.0";

    public GithubBlogProvider( HttpClient http, IMemoryCache cache, IOptions<BlogOptions> opt )
    {
        this.http = http;
        this.cache = cache;
        this.opt = opt.Value;

        http.DefaultRequestHeaders.UserAgent.ParseAdd( UA );
        if ( !string.IsNullOrWhiteSpace( this.opt.GitHubToken ) )
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue( "Bearer", this.opt.GitHubToken );
    }

    private string ApiListUrl => $"https://api.github.com/repos/{opt.Owner}/{opt.Repo}/contents/{opt.ContentRoot}?ref={opt.Branch}";
    private string RawBaseUrl => $"https://raw.githubusercontent.com/{opt.Owner}/{opt.Repo}/{opt.Branch}/";

    private static string KeyList => "blog:list";
    private static string KeyPost( string slug ) => $"blog:post:{slug}";
    private static string KeyEtag( string relativePath ) => $"blog:etag:{relativePath}";

    private sealed class GhItem
    {
        public string name { get; set; } = "";
        public string path { get; set; } = "";
        public string type { get; set; } = ""; // "file" | "dir"
        public string? download_url { get; set; }
        public string sha { get; set; } = "";
        public long size { get; set; }
        public string url { get; set; } = "";
    }

    // ===== LIST =====
    public async Task<IReadOnlyList<BlogIndexItem>> GetListAsync( CancellationToken ct = default )
    {
        if ( cache.TryGetValue<IReadOnlyList<BlogIndexItem>>( KeyList, out var cached ) )
            return cached!;

        // 1) List entries in ContentRoot (directories represent posts)
        using var req = new HttpRequestMessage( HttpMethod.Get, ApiListUrl );
        using var res = await http.SendAsync( req, ct );
        res.EnsureSuccessStatusCode();

        var json = await res.Content.ReadAsStringAsync( ct );
        var entries = JsonSerializer.Deserialize<List<GhItem>>( json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true } ) ?? new();

        // 2) Only directories. Each must contain post.md
        var folders = entries.Where( e => e.type == "dir" ).Select( e => e.name ).ToList();

        var items = new List<BlogIndexItem>( folders.Count );

        // Fetch metadata per folder by reading its post.md
        foreach ( var slug in folders )
        {
            var page = await GetBySlugAsync( slug, ct ); // uses cache/etag internally
            if ( page is null )
                continue;

            items.Add( new BlogIndexItem
            {
                Slug = page.Slug,
                Title = page.Title,
                Summary = page.Summary,
                PostedOn = page.PostedOn,
                Tags = page.Tags
            } );
        }

        // Sort by date string (front-matter) descending, fallback by title
        items = items
            .OrderByDescending( i => i.PostedOn ?? string.Empty, StringComparer.Ordinal )
            .ThenBy( i => i.Title, StringComparer.OrdinalIgnoreCase )
            .ToList();

        cache.Set( KeyList, items, opt.ListCache );
        return items;
    }

    // ===== BY SLUG =====
    public async Task<BlogPageModel> GetBySlugAsync( string slug, CancellationToken ct = default )
    {
        if ( cache.TryGetValue<BlogPageModel>( KeyPost( slug ), out var cached ) )
            return cached!;

        // path to the markdown in the folder-per-post layout
        var mdPath = $"{opt.ContentRoot.TrimEnd( '/' )}/{slug}/post.md";
        var rawUrl = RawBaseUrl + mdPath;

        // Conditional GET via ETag
        using var req = new HttpRequestMessage( HttpMethod.Get, rawUrl );
        if ( cache.TryGetValue<string>( KeyEtag( mdPath ), out var etag ) && !string.IsNullOrWhiteSpace( etag ) )
            req.Headers.TryAddWithoutValidation( "If-None-Match", etag );

        using var res = await http.SendAsync( req, ct );

        string markdown;
        if ( res.StatusCode == System.Net.HttpStatusCode.NotModified )
        {
            if ( cache.TryGetValue<BlogPageModel>( KeyPost( slug ), out var existing ) )
                return existing!;
            // Edge: content evicted; refetch unconditionally
            req.Headers.Remove( "If-None-Match" );
            using var res2 = await http.SendAsync( req, ct );
            res2.EnsureSuccessStatusCode();
            markdown = await res2.Content.ReadAsStringAsync( ct );
            if ( res2.Headers.ETag is not null )
                cache.Set( KeyEtag( mdPath ), res2.Headers.ETag.Tag!, TimeSpan.FromHours( 12 ) );
        }
        else if ( res.IsSuccessStatusCode )
        {
            markdown = await res.Content.ReadAsStringAsync( ct );
            if ( res.Headers.ETag is not null )
                cache.Set( KeyEtag( mdPath ), res.Headers.ETag.Tag!, TimeSpan.FromHours( 12 ) );
        }
        else
        {
            return null;
        }

        // Single authoritative image rewriter for this slug
        string ImageRewriter( string url )
        {
            if ( string.IsNullOrWhiteSpace( url ) )
                return url;

            // Strip wrapping quotes (defensive)
            url = url.Trim();
            if ( url.Length >= 2 )
            {
                var a = url[0];
                var b = url[^1];
                if ( ( a == '"' && b == '"' ) || ( a == '\'' && b == '\'' ) )
                    url = url[1..^1].Trim();
            }

            // Leave absolute + data URIs
            if ( Uri.TryCreate( url, UriKind.Absolute, out _ ) )
                return url;
            if ( url.StartsWith( "data:", StringComparison.OrdinalIgnoreCase ) )
                return url;

            // Repo-root absolute ("/images/...") => RawBase + images/...
            if ( url.StartsWith( "/" ) )
                return $"{RawBaseUrl}{url.TrimStart( '/' )}";

            // Post-relative: ensure "images/" prefix under the post folder
            var postDir = $"{opt.ContentRoot.TrimEnd( '/' )}/{slug}";
            var rel = url.TrimStart( '.', '/' );
            if ( !rel.StartsWith( "images/", StringComparison.OrdinalIgnoreCase ) )
                rel = $"images/{rel}";

            return $"{RawBaseUrl}{postDir}/{rel}";
        }

        // Build runtime content
        var sink = new BlogRuntimeSink( blogName: slug, rewriteImageUrl: ImageRewriter );
        var fragment = BlogAstWalker.Build( sink, markdown, ImageRewriter, out var fm );

        var page = new BlogPageModel
        {
            Slug = slug,
            Title = string.IsNullOrWhiteSpace( fm.Title ) ? slug.Replace( '-', ' ' ) : fm.Title,
            Permalink = string.IsNullOrWhiteSpace( fm.Permalink ) ? $"/blog/{slug}" : fm.Permalink,
            Summary = fm.Description,
            PostedOn = fm.PostedOn,
            ReadTime = fm.ReadTime,
            ImageUrl = string.IsNullOrWhiteSpace( fm.ImageUrl ) ? null : ImageRewriter( fm.ImageUrl ),
            Content = fragment
        };

        cache.Set( KeyPost( slug ), page, opt.PostCache );
        return page;
    }

    public Task InvalidateAsync( string slug = null )
    {
        if ( string.IsNullOrWhiteSpace( slug ) )
            cache.Remove( KeyList );
        else
            cache.Remove( KeyPost( slug ) );
        return Task.CompletedTask;
    }
}
