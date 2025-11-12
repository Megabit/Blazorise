using System;

namespace Blazorise.Docs.Infrastructure;

public static class BuildStamp
{
    public static readonly string V = Guid.NewGuid().ToString( "N" );
}