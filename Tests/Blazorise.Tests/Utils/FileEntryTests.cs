#region Using directives
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Blazorise.Tests.TestServices;
using Xunit;
#endregion

namespace Blazorise.Tests.Utils;

public class FileEntryTests
{
    [Theory]
    [InlineData( 0 )]
    [InlineData( 5 )]
    public async Task RestoredBytesCanBeReadWithoutAnOwnerOrBrowserHandle( int length )
    {
        var original = new FileEntry
        {
            Name = "cached.bin",
            Size = 999,
            Data = new byte[length],
            Status = FileEntryStatus.Ready,
        };

        var json = JsonSerializer.Serialize( original );
        var restored = JsonSerializer.Deserialize<FileEntry>( json );

        Assert.Null( restored.Owner );
        Assert.Equal( length, restored.Size );

        await using var result = new MemoryStream();
        await restored.WriteToStreamAsync( result );

        Assert.Equal( original.Data, result.ToArray() );

        await using var source = restored.OpenReadStream();
        await using var secondResult = new MemoryStream();
        var buffer = new byte[3];
        var bytesRead = 0;

        while ( ( bytesRead = await source.ReadAsync( buffer.AsMemory() ) ) > 0 )
        {
            await secondResult.WriteAsync( buffer.AsMemory( 0, bytesRead ) );
        }

        Assert.Equal( original.Data, secondResult.ToArray() );
        Assert.False( source.CanWrite );
        Assert.Equal( FileEntryStatus.Ready, restored.Status );
    }

    [Fact]
    public void SuppliedBytesDetermineTheSizeLimitRegardlessOfMetadata()
    {
        var file = new FileEntry
        {
            Size = 1,
            Data = new byte[512001],
        };

        Assert.Throws<IOException>( () => file.OpenReadStream() );
        Assert.Throws<IOException>( () => file.OpenReadStream( 100 ) );

        using var stream = file.OpenReadStream( file.Size );

        Assert.Equal( file.Data.LongLength, stream.Length );
    }

    [Fact]
    public async Task CancelStopsAnOpenCachedStreamAndNewReadsRemainPossible()
    {
        var file = new FileEntry { Data = new byte[] { 1, 2, 3 } };
        await using var stream = file.OpenReadStream();
        var buffer = new byte[1];

        Assert.Equal( 1, await stream.ReadAsync( buffer.AsMemory() ) );

        await file.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>( async () => await stream.ReadAsync( buffer.AsMemory() ) );

        await using var result = new MemoryStream();
        await file.WriteToStreamAsync( result );

        Assert.Equal( file.Data, result.ToArray() );
    }

    [Fact]
    public async Task CachedCopiesRespectTheCallersCancellationToken()
    {
        var file = new FileEntry { Data = new byte[] { 1, 2, 3 } };
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>( () => file.WriteToStreamAsync( Stream.Null, cancellation.Token ) );
    }

    [Fact]
    public void ReleasingCachedBytesPreservesTheFileDetails()
    {
        var file = new FileEntry
        {
            Name = "cached.bin",
            Data = new byte[] { 1, 2, 3 },
            Status = FileEntryStatus.Uploaded,
        };

        file.Data = null;

        Assert.Equal( 3, file.Size );
        Assert.Equal( "cached.bin", file.Name );
        Assert.Equal( FileEntryStatus.Uploaded, file.Status );
        Assert.Throws<InvalidOperationException>( () => file.OpenReadStream() );
    }

    [Fact]
    public async Task LargeNonSeekableSourcesOpenOnDemandAndReadOnlyTheRequestedPart()
    {
        var length = (long)int.MaxValue + 1024;
        var source = new GeneratedReadStream( length );
        var opens = 0;
        var file = new FileEntry
        {
            Size = length,
            StreamFactory = _ =>
            {
                opens++;
                return Task.FromResult<Stream>( source );
            },
        };

        await using ( var stream = file.OpenReadStream( length ) )
        {
            Assert.Equal( 0, opens );
            Assert.Equal( length, stream.Length );

            var buffer = new byte[4096];

            Assert.Equal( buffer.Length, await stream.ReadAsync( buffer.AsMemory() ) );
            Assert.Equal( buffer.Length, source.BytesRead );
            Assert.Equal( buffer.Length, source.MaxRequestedSize );
            Assert.Equal( 1, opens );
            Assert.Null( file.Data );
        }

        Assert.True( source.IsDisposedAsynchronously );
    }

    [Fact]
    public async Task CopiesNonSeekableContentWithBoundedBuffersAndReopensItForAnotherAttempt()
    {
        var length = 8L * 1024 * 1024;
        var sources = new List<GeneratedReadStream>();
        var file = new FileEntry
        {
            Size = length,
            StreamFactory = _ =>
            {
                var source = new GeneratedReadStream( length, maxReadSize: 8192 );
                sources.Add( source );
                return Task.FromResult<Stream>( source );
            },
        };

        await file.WriteToStreamAsync( Stream.Null );
        await file.WriteToStreamAsync( Stream.Null );

        Assert.Equal( 2, sources.Count );
        Assert.All( sources, source =>
        {
            Assert.Equal( length, source.BytesRead );
            Assert.InRange( source.MaxRequestedSize, 1, 81920 );
            Assert.True( source.IsDisposedAsynchronously );
        } );
        Assert.Null( file.Data );
    }

    [Theory]
    [InlineData( 0, 1, typeof( InvalidDataException ) )]
    [InlineData( 2, 3, typeof( InvalidDataException ) )]
    [InlineData( 3, 2, typeof( EndOfStreamException ) )]
    public async Task RejectsContentWhoseActualLengthDiffersFromTheDeclaredSize( int declaredSize, int actualSize, Type expectedExceptionType )
    {
        var source = new GeneratedReadStream( actualSize );
        var file = new FileEntry
        {
            Size = declaredSize,
            StreamFactory = _ => Task.FromResult<Stream>( source ),
        };

        await Assert.ThrowsAsync( expectedExceptionType, () => file.WriteToStreamAsync( Stream.Null ) );

        Assert.True( source.IsDisposedAsynchronously );
    }

    [Fact]
    public void SizeLimitIsCheckedBeforeOpeningTheFactory()
    {
        var opens = 0;
        var file = new FileEntry
        {
            Size = 100,
            StreamFactory = _ =>
            {
                opens++;
                return Task.FromResult<Stream>( new GeneratedReadStream( 100 ) );
            },
        };

        Assert.Throws<IOException>( () => file.OpenReadStream( 99 ) );
        Assert.Equal( 0, opens );
    }

    [Fact]
    public async Task CancelStopsOpeningTheSource()
    {
        var factoryStarted = new TaskCompletionSource( TaskCreationOptions.RunContinuationsAsynchronously );
        var factoryToken = CancellationToken.None;
        var file = new FileEntry
        {
            Size = 1,
            StreamFactory = async cancellationToken =>
            {
                factoryToken = cancellationToken;
                factoryStarted.TrySetResult();
                await Task.Delay( Timeout.Infinite, cancellationToken );
                return new GeneratedReadStream( 1 );
            },
        };

        await using var stream = file.OpenReadStream();
        var readTask = stream.ReadAsync( new byte[1].AsMemory() ).AsTask();
        await factoryStarted.Task;

        await file.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>( () => readTask );
        Assert.True( factoryToken.IsCancellationRequested );
    }

    [Fact]
    public async Task DisposalAlsoReleasesASourceThatFinishesOpeningLater()
    {
        var sourceReady = new TaskCompletionSource<Stream>( TaskCreationOptions.RunContinuationsAsynchronously );
        var source = new GeneratedReadStream( 1 );
        var file = new FileEntry
        {
            Size = 1,
            StreamFactory = _ => sourceReady.Task,
        };

        var stream = file.OpenReadStream();
        var readTask = stream.ReadAsync( new byte[1].AsMemory() ).AsTask();
        await stream.DisposeAsync();
        sourceReady.SetResult( source );

        await Assert.ThrowsAnyAsync<OperationCanceledException>( () => readTask );
        await source.Disposed.WaitAsync( TimeSpan.FromSeconds( 5 ) );
        Assert.True( source.IsDisposedAsynchronously );
    }

    [Fact]
    public async Task RestoredMetadataCanBeReconnectedToItsStreamSource()
    {
        var original = new FileEntry
        {
            Name = "cached.bin",
            Size = 3,
            StreamFactory = _ => Task.FromResult<Stream>( new GeneratedReadStream( 3 ) ),
        };

        var restored = JsonSerializer.Deserialize<FileEntry>( JsonSerializer.Serialize( original ) );

        Assert.Null( restored.StreamFactory );
        Assert.Null( restored.Data );
        Assert.Equal( 3, restored.Size );

        restored.StreamFactory = original.StreamFactory;
        await using var result = new MemoryStream();
        await restored.WriteToStreamAsync( result );

        Assert.Equal( new byte[] { 42, 42, 42 }, result.ToArray() );
    }

    [Fact]
    public void RejectsAmbiguousContentSources()
    {
        var file = new FileEntry
        {
            Data = new byte[] { 1 },
            StreamFactory = _ => Task.FromResult<Stream>( new GeneratedReadStream( 1 ) ),
        };

        Assert.Throws<InvalidOperationException>( () => file.OpenReadStream() );
    }
}