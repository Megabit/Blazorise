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
    Task<IReadOnlyList<BlogIndexItem>> GetListAsync( string root, CancellationToken ct = default );
    Task<BlogPageModel> GetByPermalinkAsync( string permalink, CancellationToken ct = default );
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

    // CHANGED: now parameterized by a root ("blog", "news", etc.)
    private string ApiListUrl( string root ) => $"https://api.github.com/repos/{opt.Owner}/{opt.Repo}/contents/{root}?ref={opt.Branch}";
    private string RawBaseUrl => $"https://raw.githubusercontent.com/{opt.Owner}/{opt.Repo}/{opt.Branch}/";
    private static string KeyList( string root ) => $"blog:list:{root ?? "*"}";
    private static string KeyMap( string root ) => $"blog:map:{root ?? "*"}";
    private static string KeyPage( string permalink ) => $"blog:page:{permalink}";
    private static string KeyEtag( string relativePath ) => $"blog:etag:{relativePath}";

    private sealed class GhItem
    {
        public string name { get; set; } = "";
        public string path { get; set; } = "";
        public string type { get; set; } = ""; // "file" | "dir"
        public string url { get; set; } = "";
    }

    public async Task<IReadOnlyList<BlogIndexItem>> GetListAsync( string root, CancellationToken ct = default )
    {
        if ( cache.TryGetValue<IReadOnlyList<BlogIndexItem>>( KeyList( root ), out var cached ) )
            return cached!;

        var items = new List<BlogIndexItem>();
        var map = new Dictionary<string, string>( StringComparer.OrdinalIgnoreCase );
        var seenPermalinks = new HashSet<string>( StringComparer.OrdinalIgnoreCase );
        var seenDirs = new HashSet<string>( StringComparer.OrdinalIgnoreCase );

        var roots = root is not null
            ? new[] { root }
            : ( opt.ContentRoot ?? Array.Empty<string>() );

        foreach ( var r in roots )
        {
            if ( string.IsNullOrWhiteSpace( r ) )
                continue;

            using var req = new HttpRequestMessage( HttpMethod.Get, ApiListUrl( r ) );
            using var res = await http.SendAsync( req, ct );
            if ( !res.IsSuccessStatusCode )
                continue;

            var json = await res.Content.ReadAsStringAsync( ct );
            var entries = JsonSerializer.Deserialize<List<GhItem>>( json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true } ) ?? new();

            var folders = entries.Where( e => e.type == "dir" ).Select( e => e.name ).ToList();

            foreach ( var folder in folders )
            {
                var meta = await TryReadFrontMatterAsync( r, folder, ct ); // now partial (see below)
                if ( meta is null )
                    continue;

                var (fm, _) = meta.Value;

                var permalink = NormalizePermalink( fm.Permalink ) ?? $"/{r.TrimEnd( '/' )}/{folder}";
                var slug = ExtractSlug( permalink );
                var relativeDir = $"{r.TrimEnd( '/' )}/{folder}";

                if ( !seenDirs.Add( relativeDir ) )
                    continue;
                map[permalink] = relativeDir;
                if ( !seenPermalinks.Add( permalink ) )
                    continue;

                string ImageRewriter( string url )
                {
                    if ( string.IsNullOrWhiteSpace( url ) )
                        return url;
                    url = TrimQuotes( url );
                    if ( Uri.TryCreate( url, UriKind.Absolute, out _ ) )
                        return url;
                    if ( url.StartsWith( "data:", StringComparison.OrdinalIgnoreCase ) )
                        return url;
                    if ( url.StartsWith( "/" ) )
                        return $"{RawBaseUrl}{url.TrimStart( '/' )}";
                    var rel = url.TrimStart( '.', '/' );
                    if ( !rel.StartsWith( "img/", StringComparison.OrdinalIgnoreCase ) )
                        rel = $"img/{rel}";
                    return $"{RawBaseUrl}{relativeDir}/{rel}";
                }

                items.Add( new BlogIndexItem
                {
                    Permalink = permalink,
                    Slug = slug,
                    Title = string.IsNullOrWhiteSpace( fm.Title ) ? slug : fm.Title,
                    Summary = fm.Description,
                    PostedOn = fm.PostedOn,
                    Category = string.IsNullOrWhiteSpace( fm.Category ) ? null : fm.Category,
                    Tags = Array.Empty<string>(),
                    ImageUrl = string.IsNullOrWhiteSpace( fm.ImageUrl ) ? null : ImageRewriter( fm.ImageUrl ),
                    AuthorName = string.IsNullOrWhiteSpace( fm.AuthorName ) ? null : fm.AuthorName,
                    AuthorImage = string.IsNullOrWhiteSpace( fm.AuthorImage ) ? null : ImageRewriter( fm.AuthorImage ),
                    ReadTime = string.IsNullOrWhiteSpace( fm.ReadTime ) ? null : fm.ReadTime,
                    Root = r
                } );
            }
        }

        items = items
            .OrderByDescending( i => i.PostedOn ?? string.Empty, StringComparer.Ordinal )
            .ThenBy( i => i.Title, StringComparer.OrdinalIgnoreCase )
            .ToList();

        cache.Set( KeyList( root ), items, opt.ListCache );
        cache.Set( KeyMap( root ), map, opt.ListCache );
        return items;
    }

    public async Task<BlogPageModel> GetByPermalinkAsync( string permalink, CancellationToken ct = default )
    {
        permalink = NormalizePermalink( permalink ) ?? "/";
        if ( cache.TryGetValue<BlogPageModel>( KeyPage( permalink ), out var cached ) )
            return cached!;

        var relativeDir = await ResolveFolderByPermalink( permalink, ct ); // eg. "blog/how-to-post"
        if ( relativeDir is null )
            return null;

        var mdPath = $"{relativeDir}/post.md";
        var rawUrl = RawBaseUrl + mdPath;

        using var req = new HttpRequestMessage( HttpMethod.Get, rawUrl );
        if ( cache.TryGetValue<string>( KeyEtag( mdPath ), out var etag ) && !string.IsNullOrWhiteSpace( etag ) )
            req.Headers.TryAddWithoutValidation( "If-None-Match", etag );

        using var res = await http.SendAsync( req, ct );

        string markdown;
        if ( res.StatusCode == System.Net.HttpStatusCode.NotModified )
        {
            if ( cache.TryGetValue<BlogPageModel>( KeyPage( permalink ), out var existing ) )
                return existing!;
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
            return null;

        // image rewriter relative to the resolved dir
        string ImageRewriter( string url )
        {
            if ( string.IsNullOrWhiteSpace( url ) )
                return url;
            url = TrimQuotes( url );

            if ( Uri.TryCreate( url, UriKind.Absolute, out _ ) )
                return url;
            if ( url.StartsWith( "data:", StringComparison.OrdinalIgnoreCase ) )
                return url;

            if ( url.StartsWith( "/" ) )
                return $"{RawBaseUrl}{url.TrimStart( '/' )}";

            var rel = url.TrimStart( '.', '/' );
            if ( !rel.StartsWith( "img/", StringComparison.OrdinalIgnoreCase ) )
                rel = $"img/{rel}";

            var postDir = relativeDir; // already "root/folder"
            return $"{RawBaseUrl}{postDir}/{rel}";
        }

        var sink = new BlogRuntimeSink( blogName: ExtractSlug( permalink ), rewriteImageUrl: ImageRewriter );
        var fragment = BlogAstWalker.Build( sink, markdown, ImageRewriter, out var fm );

        var page = new BlogPageModel
        {
            Permalink = permalink,
            Slug = ExtractSlug( permalink ),
            Title = string.IsNullOrWhiteSpace( fm.Title ) ? ExtractSlug( permalink ) : fm.Title,
            Summary = fm.Description,
            PostedOn = fm.PostedOn,
            Category = fm.Category,
            AuthorName = string.IsNullOrWhiteSpace( fm.AuthorName ) ? null : fm.AuthorName,
            AuthorImage = string.IsNullOrWhiteSpace( fm.AuthorImage ) ? null : ImageRewriter( fm.AuthorImage ),
            ReadTime = fm.ReadTime,
            ImageUrl = string.IsNullOrWhiteSpace( fm.ImageUrl ) ? null : ImageRewriter( fm.ImageUrl ),
            Content = fragment,
            Root = relativeDir.Split( '/' )[0],
        };

        cache.Set( KeyPage( permalink ), page, opt.PostCache );
        return page;
    }

    public Task InvalidateAsync( string key = null )
    {
        if ( string.IsNullOrWhiteSpace( key ) )
        {
            cache.Remove( KeyList );
            cache.Remove( KeyMap );
        }
        else
        {
            // key can be a permalink or a folder; drop both variants just in case
            cache.Remove( KeyPage( NormalizePermalink( key ) ?? key ) );
        }
        return Task.CompletedTask;
    }

    // ===== helpers =====

    private async Task<(FrontMatter fm, string markdown)?> TryReadFrontMatterAsync( string root, string folder, CancellationToken ct )
    {
        var mdPath = $"{root.TrimEnd( '/' )}/{folder}/post.md";
        var rawUrl = RawBaseUrl + mdPath;

        using var req = new HttpRequestMessage( HttpMethod.Get, rawUrl );
        // pull just the first 16KB (front matter is at the top)
        req.Headers.TryAddWithoutValidation( "Range", "bytes=0-16383" );

        using var res = await http.SendAsync( req, ct );
        if ( res.StatusCode != System.Net.HttpStatusCode.PartialContent &&
             res.StatusCode != System.Net.HttpStatusCode.OK )
            return null;

        var text = await res.Content.ReadAsStringAsync( ct );

        // Extract only the YAML front matter to avoid needing full body
        // But MarkdownFrontMatter.Parse already handles full strings; partial is fine.
        var fm = MarkdownFrontMatter.Parse( text );
        if ( fm is null || string.IsNullOrWhiteSpace( fm.Title ) )
        {
            // Fallback: if the range wasn’t enough (very large front matter), fetch full once
            using var res2 = await http.GetAsync( rawUrl, ct );
            if ( !res2.IsSuccessStatusCode )
                return null;
            text = await res2.Content.ReadAsStringAsync( ct );
            fm = MarkdownFrontMatter.Parse( text );
        }

        return (fm, text);
    }

    private async Task<string?> ResolveFolderByPermalink( string permalink, CancellationToken ct )
    {
        // Try all maps (blog, news, all) to be safe:
        if ( cache.TryGetValue<Dictionary<string, string>>( KeyMap( null ), out var all ) && all.TryGetValue( permalink, out var dir ) )
            return dir;

        foreach ( var r in opt.ContentRoot ?? Array.Empty<string>() )
            if ( cache.TryGetValue<Dictionary<string, string>>( KeyMap( r ), out var map ) && map.TryGetValue( permalink, out dir ) )
                return dir;

        // Rebuild per-root maps if needed (lightweight due to Range)
        foreach ( var r in opt.ContentRoot ?? Array.Empty<string>() )
            _ = await GetListAsync( r, ct );

        if ( cache.TryGetValue<Dictionary<string, string>>( KeyMap( null ), out all ) && all.TryGetValue( permalink, out dir ) )
            return dir;

        foreach ( var r in opt.ContentRoot ?? Array.Empty<string>() )
            if ( cache.TryGetValue<Dictionary<string, string>>( KeyMap( r ), out var map ) && map.TryGetValue( permalink, out dir ) )
                return dir;

        return null;
    }

    private static string TrimQuotes( string s )
    {
        s = s.Trim();
        if ( s.Length >= 2 )
        {
            var a = s[0];
            var b = s[^1];
            if ( ( a == '"' && b == '"' ) || ( a == '\'' && b == '\'' ) )
                s = s[1..^1].Trim();
        }
        return s;
    }

    private static string NormalizePermalink( string p )
    {
        if ( string.IsNullOrWhiteSpace( p ) )
            return null;

        // trim and unify slashes, but DO NOT add or remove the "blog" segment
        var s = p.Trim().Replace( '\\', '/' );

        // ensure a single leading slash
        if ( !s.StartsWith( "/" ) )
            s = "/" + s;

        // collapse duplicate slashes (e.g., "/blog//how-to" -> "/blog/how-to")
        while ( s.Contains( "//", StringComparison.Ordinal ) )
            s = s.Replace( "//", "/", StringComparison.Ordinal );

        // remove trailing slash (but keep "/" for root)
        if ( s.Length > 1 && s.EndsWith( '/' ) )
            s = s.TrimEnd( '/' );

        return s;
    }

    private static string ExtractSlug( string permalink )
        => permalink.Trim( '/' ).Split( '/', StringSplitOptions.RemoveEmptyEntries ).LastOrDefault() ?? "";
}