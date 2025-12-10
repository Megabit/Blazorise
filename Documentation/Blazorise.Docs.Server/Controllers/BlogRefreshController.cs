using System;
using System.Threading;
using System.Threading.Tasks;
using Blazorise.Docs.BlogRuntime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Blazorise.Docs.Server.Controllers;

[ApiController]
[Route( "api/blog" )]
public sealed class BlogRefreshController : ControllerBase
{
    private readonly GithubBlogProvider provider;
    private readonly BlogOptions opt;

    public BlogRefreshController( IBlogProvider blog, IOptions<BlogOptions> options )
    {
        provider = (GithubBlogProvider)blog;
        opt = options.Value;
    }

    [HttpPost( "refresh" )]
    public async Task<IActionResult> Refresh( [FromHeader( Name = "X-Refresh-Secret" )] string secret, CancellationToken ct )
    {
        if ( string.IsNullOrWhiteSpace( opt.RuntimeRefreshSecret ) )
            return BadRequest( "Refresh secret not configured." );

        if ( !string.Equals( secret, opt.RuntimeRefreshSecret, StringComparison.Ordinal ) )
            return Unauthorized();

        await provider.RefreshAsync( ct );

        return Ok( new { ok = true } );
    }
}