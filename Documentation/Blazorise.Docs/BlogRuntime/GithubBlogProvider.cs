#region Using directives
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
#endregion

namespace Blazorise.Docs.BlogRuntime;

public interface IBlogProvider
{
    Task<IReadOnlyList<BlogIndexItem>> GetListAsync( string root, int? take = null, int skip = 0, CancellationToken ct = default );
    Task<BlogPageModel> GetByPermalinkAsync( string permalink, CancellationToken ct = default );
    Task PreheatAsync( CancellationToken ct = default );
    Task RefreshAsync( CancellationToken ct = default );
}

public sealed class GithubBlogProvider : IBlogProvider
{
    #region Members

    private readonly HttpClient http;
    private readonly BlogOptions opt;
    private readonly SemaphoreSlim loadLock = new( 1, 1 );

    // immutable swap on refresh
    private volatile BundleState state = BundleState.Empty;

    // parsed-page cache (permalink -> model), invalidated on refresh
    private ConcurrentDictionary<string, BlogPageModel> pageCache = new( StringComparer.OrdinalIgnoreCase );

    private string RawBaseUrl =>
        $"https://raw.githubusercontent.com/{opt.Owner}/{opt.Repo}/{opt.Branch}/";

    private string RuntimeBaseUrl =>
        string.IsNullOrWhiteSpace( opt.RuntimeBaseUrlOverride )
            ? $"{RawBaseUrl}{opt.RuntimeFolder.TrimEnd( '/' )}/"
            : opt.RuntimeBaseUrlOverride!.TrimEnd( '/' ) + "/";

    #endregion

    #region Constructors

    public GithubBlogProvider( HttpClient http, IOptions<BlogOptions> options )
    {
        this.http = http;
        this.opt = options.Value;

        http.DefaultRequestHeaders.UserAgent.ParseAdd( "BlazoriseDocsBundle/1.0" );

        if ( !string.IsNullOrWhiteSpace( opt.GitHubToken ) )
            http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue( "Bearer", opt.GitHubToken );
    }

    #endregion

    #region Methods

    public async Task PreheatAsync( CancellationToken ct = default )
       => await EnsureLoadedAsync( force: false, ct );

    public async Task RefreshAsync( CancellationToken ct = default )
        => await EnsureLoadedAsync( force: true, ct );

    public async Task<IReadOnlyList<BlogIndexItem>> GetListAsync( string root, int? take = null, int skip = 0, CancellationToken ct = default )
    {
        await EnsureLoadedAsync( force: false, ct );

        var s = state; // local
        IEnumerable<BlogIndexItem> q = s.Items;

        if ( !string.IsNullOrWhiteSpace( root ) )
        {
            q = q
                .Where( i => string.Equals( i.Root, root, StringComparison.OrdinalIgnoreCase ) )
                .OrderByDescending( x => x.PostedOn );
        }

        if ( skip > 0 )
            q = q.Skip( skip );
        if ( take is > 0 )
            q = q.Take( take.Value );

        return q.ToList();
    }

    public async Task<BlogPageModel> GetByPermalinkAsync( string permalink, CancellationToken ct = default )
    {
        await EnsureLoadedAsync( force: false, ct );
        permalink = NormalizePermalink( permalink ) ?? "/";

        // parsed-page cache first
        if ( pageCache.TryGetValue( permalink, out var cached ) )
            return cached;

        var s = state;
        if ( !s.PermalinkToDir.TryGetValue( permalink, out var relativeDir ) )
            return null;

        if ( !s.MarkdownByDir.TryGetValue( relativeDir, out var markdown ) )
            return null;

        // build image rewriter
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

            // relativeDir is "blog/2024-xx-slug"
            return $"{RawBaseUrl}{relativeDir}/{rel}";
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

        pageCache[permalink] = page;
        return page;
    }

    private async Task EnsureLoadedAsync( bool force, CancellationToken ct )
    {
        if ( state.IsLoaded && !force )
            return;

        await loadLock.WaitAsync( ct );
        try
        {
            if ( state.IsLoaded && !force )
                return;

            // read version
            var version = await GetJsonAsync<VersionDoc>( $"{RuntimeBaseUrl}version.json", ct );
            var newCommit = version?.commit?.Trim() ?? "";

            // if we already have same commit and not forcing, keep
            if ( state.IsLoaded && !force && string.Equals( state.Commit, newCommit, StringComparison.OrdinalIgnoreCase ) )
                return;

            // load index
            var index = await GetJsonAsync<IndexDoc>( $"{RuntimeBaseUrl}index.json", ct ) ?? new();

            // load posts.zip into memory
            using var stream = await http.GetStreamAsync( $"{RuntimeBaseUrl}posts.zip", ct );
            using var ms = new MemoryStream();
            await stream.CopyToAsync( ms, ct );
            ms.Position = 0;

            var mdByDir = new Dictionary<string, string>( StringComparer.OrdinalIgnoreCase );
            using ( var zip = new ZipArchive( ms, ZipArchiveMode.Read, leaveOpen: true ) )
            {
                foreach ( var entry in zip.Entries )
                {
                    if ( !entry.FullName.EndsWith( "/post.md", StringComparison.OrdinalIgnoreCase ) )
                        continue;

                    using var er = new StreamReader( entry.Open() );
                    var text = await er.ReadToEndAsync();
                    var dir = entry.FullName[..^( "/post.md".Length )].TrimEnd( '/' );
                    mdByDir[dir] = text;
                }
            }

            // map to runtime models
            var items = ( index.items ?? new() ).Select( i => new BlogIndexItem
            {
                Permalink = NormalizePermalink( i.permalink ) ?? "/",
                Slug = i.slug,
                Title = string.IsNullOrWhiteSpace( i.title ) ? i.slug : i.title,
                Summary = string.IsNullOrWhiteSpace( i.summary ) ? null : i.summary,
                PostedOn = string.IsNullOrWhiteSpace( i.postedOn ) ? null : i.postedOn,
                Category = string.IsNullOrWhiteSpace( i.category ) ? null : i.category,
                ImageUrl = string.IsNullOrWhiteSpace( i.imageUrl ) ? null : RewriteUrlForDir( i.dir?.Trim(), i.imageUrl ),
                AuthorName = string.IsNullOrWhiteSpace( i.authorName ) ? null : i.authorName,
                AuthorImage = string.IsNullOrWhiteSpace( i.authorImage ) ? null : RewriteUrlForDir( i.dir?.Trim(), i.authorImage ),
                ReadTime = string.IsNullOrWhiteSpace( i.readTime ) ? null : i.readTime,
                Root = i.root,
                Pinned = i.pinned,
            } ).ToList();

            // permalink -> dir
            var p2d = new Dictionary<string, string>( StringComparer.OrdinalIgnoreCase );
            foreach ( var i in index.items ?? new() )
                if ( !string.IsNullOrWhiteSpace( i.permalink ) && !string.IsNullOrWhiteSpace( i.dir ) )
                    p2d[NormalizePermalink( i.permalink )!] = i.dir;

            // swap state atomically, reset page cache
            state = new BundleState
            {
                Commit = newCommit,
                Items = items,
                PermalinkToDir = p2d,
                MarkdownByDir = mdByDir
            };
            pageCache = new( StringComparer.OrdinalIgnoreCase );
        }
        finally
        {
            loadLock.Release();
        }
    }

    private string RewriteUrlForDir( string relativeDir, string url )
    {
        if ( string.IsNullOrWhiteSpace( url ) )
            return url;

        url = TrimQuotes( url );

        // Absolute URLs and data URIs stay as-is
        if ( Uri.TryCreate( url, UriKind.Absolute, out _ ) )
            return url;
        if ( url.StartsWith( "data:", StringComparison.OrdinalIgnoreCase ) )
            return url;

        // Leading slash means repo-root relative (keep prior behavior)
        if ( url.StartsWith( "/" ) )
            return $"{RawBaseUrl}{url.TrimStart( '/' )}";

        // Otherwise treat as post-local asset; ensure "img/" prefix
        var rel = url.TrimStart( '.', '/' );
        if ( !rel.StartsWith( "img/", StringComparison.OrdinalIgnoreCase ) )
            rel = $"img/{rel}";

        // relativeDir example: "blog/2023-04-18-v094-4"
        return $"{RawBaseUrl}{relativeDir.TrimEnd( '/' )}/{rel}";
    }

    private async Task<T> GetJsonAsync<T>( string url, CancellationToken ct )
    {
        using var req = new HttpRequestMessage( HttpMethod.Get, url );
        using var res = await http.SendAsync( req, HttpCompletionOption.ResponseHeadersRead, ct );
        res.EnsureSuccessStatusCode();
        await using var s = await res.Content.ReadAsStreamAsync( ct );
        return await JsonSerializer.DeserializeAsync<T>( s, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }, ct );
    }

    // ================= helpers & DTOs =================

    private static string NormalizePermalink( string p )
    {
        if ( string.IsNullOrWhiteSpace( p ) )
            return null;
        var s = p.Trim().Replace( '\\', '/' );
        if ( !s.StartsWith( '/' ) )
            s = "/" + s;
        while ( s.Contains( "//", StringComparison.Ordinal ) )
            s = s.Replace( "//", "/" );
        if ( s.Length > 1 && s.EndsWith( '/' ) )
            s = s.TrimEnd( '/' );
        return s;
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

    private static string ExtractSlug( string permalink )
        => permalink.Trim( '/' ).Split( '/', StringSplitOptions.RemoveEmptyEntries ).LastOrDefault() ?? "";

    private sealed record VersionDoc
    {
        public string commit { get; init; } = "";
        public string generatedAt { get; init; } = "";
    }

    private sealed record IndexDoc
    {
        public int version { get; init; }
        public string generatedAt { get; init; } = "";
        public string commit { get; init; } = "";
        public int count { get; init; }
        public List<Item> items { get; init; } = new();
        public sealed record Item
        {
            public string root { get; init; } = "";
            public string dir { get; init; } = "";
            public string slug { get; init; } = "";
            public string permalink { get; init; } = "";
            public string title { get; init; } = "";
            public string summary { get; init; } = "";
            public string postedOn { get; init; } = "";
            public string category { get; init; } = "";
            public string imageUrl { get; init; } = "";
            public string authorName { get; init; } = "";
            public string authorImage { get; init; } = "";
            public string readTime { get; init; } = "";
            public bool pinned { get; init; }
        }
    }

    private sealed class BundleState
    {
        public static readonly BundleState Empty = new()
        {
            Commit = "",
            Items = new List<BlogIndexItem>(),
            PermalinkToDir = new( StringComparer.OrdinalIgnoreCase ),
            MarkdownByDir = new( StringComparer.OrdinalIgnoreCase )
        };

        public required string Commit { get; init; }
        public required IReadOnlyList<BlogIndexItem> Items { get; init; }
        public required Dictionary<string, string> PermalinkToDir { get; init; }
        public required Dictionary<string, string> MarkdownByDir { get; init; }
        public bool IsLoaded => !string.IsNullOrWhiteSpace( Commit ) && Items.Count > 0 && MarkdownByDir.Count > 0;
    }

    #endregion
}