using System;
using System.Threading;
using System.Threading.Tasks;
using Blazorise.Docs.Options;
using Blazorise.Docs.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Blazorise.Docs.Server.Controllers;

[ApiController]
[Route( "api/jobs" )]
public sealed class JobsRefreshController : ControllerBase
{
    private readonly IJobsService jobsService;
    private readonly JobsOptions options;

    public JobsRefreshController( IJobsService jobsService, IOptions<JobsOptions> options )
    {
        this.jobsService = jobsService;
        this.options = options.Value ?? new JobsOptions();
    }

    /// <summary>
    /// Refreshes the jobs feed cache.
    /// </summary>
    /// <param name="secret">The refresh secret passed in the request header.</param>
    /// <param name="ct">A token to cancel the operation.</param>
    /// <returns>An action result.</returns>
    [HttpPost( "refresh" )]
    public async Task<IActionResult> Refresh( [FromHeader( Name = "X-Refresh-Secret" )] string secret, CancellationToken ct )
    {
        if ( string.IsNullOrWhiteSpace( options.RefreshSecret ) )
            return BadRequest( "Refresh secret not configured." );

        if ( !string.Equals( secret, options.RefreshSecret, StringComparison.Ordinal ) )
            return Unauthorized();

        await jobsService.RefreshAsync( ct );

        return Ok( new { ok = true } );
    }
}