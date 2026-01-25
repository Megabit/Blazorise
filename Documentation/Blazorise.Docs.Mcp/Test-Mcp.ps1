param(
    [string]$BaseUrl = "http://localhost:5000",
    [string]$RoutePrefix = "/docs",
    [int]$TimeoutSeconds = 20,
    [string]$ProtocolVersion = "2024-11-05",
    [string]$ExampleCode = "ButtonExample",
    [switch]$ListDocsPages
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

try
{
    Add-Type -AssemblyName System.Net.Http
}
catch
{
    throw "System.Net.Http assembly is not available: $($_.Exception.Message)"
}

[string] $normalizedBase = $BaseUrl.TrimEnd( "/" )
[System.Uri] $baseUri = [System.Uri]::new( $normalizedBase )
[System.Uri] $sseUri = [System.Uri]::new( $baseUri, "/mcp/sse" )
[System.Uri] $messageUri = [System.Uri]::new( $baseUri, "/mcp/message" )

[System.Net.Http.HttpClient] $client = [System.Net.Http.HttpClient]::new()
$client.Timeout = [System.Threading.Timeout]::InfiniteTimeSpan

[System.Collections.Generic.HashSet[int]] $pendingIds = New-Object "System.Collections.Generic.HashSet[int]"
$pendingIds.Add( 1 ) | Out-Null
$pendingIds.Add( 2 ) | Out-Null
if ( ![string]::IsNullOrWhiteSpace( $ExampleCode ) )
{
    $pendingIds.Add( 4 ) | Out-Null
}
if ( $ListDocsPages )
{
    $pendingIds.Add( 3 ) | Out-Null
}

[bool] $sent = $false
[bool] $initialized = $false
[bool] $initializeRequested = $false
[string] $sessionId = $null
[bool] $done = $false
[System.DateTime] $deadline = [System.DateTime]::UtcNow.AddSeconds( $TimeoutSeconds )

[System.Net.Http.HttpResponseMessage] $sseResponse = $null
[System.IO.Stream] $stream = $null
[System.IO.StreamReader] $reader = $null

try
{
    $sseResponse = $client.GetAsync(
        $sseUri,
        [System.Net.Http.HttpCompletionOption]::ResponseHeadersRead
    ).GetAwaiter().GetResult()
}
catch
{
    throw "SSE connection failed: $($_.Exception.Message)"
}

if ( !$sseResponse.IsSuccessStatusCode )
{
    throw "SSE connection failed: $($sseResponse.StatusCode)"
}

$stream = $sseResponse.Content.ReadAsStreamAsync().GetAwaiter().GetResult()

if ( $null -eq $stream )
{
    throw "SSE response stream was not available."
}

$reader = [System.IO.StreamReader]::new( $stream )

Write-Host "Connected to $sseUri"

function Get-PayloadPropertyValue
{
    param(
        [object]$Payload,
        [string]$Name
    )

    if ( $null -eq $Payload )
    {
        return $null
    }

    if ( $Payload -is [System.Array] )
    {
        foreach ( $item in $Payload )
        {
            [object] $value = Get-PayloadPropertyValue -Payload $item -Name $Name
            if ( $null -ne $value )
            {
                return $value
            }
        }

        return $null
    }

    [System.Management.Automation.PSPropertyInfo] $property = $Payload.PSObject.Properties[$Name]

    if ( $null -eq $property )
    {
        return $null
    }

    return $property.Value
}

function Get-JsonRpcMessages
{
    param(
        [object]$Payload
    )

    if ( $null -eq $Payload )
    {
        return @()
    }

    if ( $Payload -is [System.Array] )
    {
        return $Payload
    }

    return @($Payload)
}

function Process-JsonRpcPayload
{
    param(
        [object]$Payload,
        [string]$Raw,
        [string]$SourceLabel
    )

    [object[]] $messages = Get-JsonRpcMessages -Payload $Payload
    if ( $messages.Count -eq 0 )
    {
        return
    }

    foreach ( $message in $messages )
    {
        [object] $jsonRpcValue = Get-PayloadPropertyValue -Payload $message -Name "jsonrpc"
        if ( !$jsonRpcValue )
        {
            continue
        }

        if ( ![string]::IsNullOrWhiteSpace( $SourceLabel ) )
        {
            if ( ![string]::IsNullOrWhiteSpace( $Raw ) )
            {
                Write-Host "${SourceLabel}: $Raw"
            }
            else
            {
                [string] $serialized = $message | ConvertTo-Json -Depth 6
                Write-Host "${SourceLabel}: $serialized"
            }
        }

        [object] $idValue = Get-PayloadPropertyValue -Payload $message -Name "id"
        if ( $idValue )
        {
            [int] $responseId = [int]$idValue
            if ( $pendingIds.Contains( $responseId ) )
            {
                        $pendingIds.Remove( $responseId ) | Out-Null
                    }

                    if ( $responseId -eq 1 -and !$initialized )
                    {
                        Send-JsonRpcNotification -Method "initialized" -Params @{}
                        Send-JsonRpcRequest -Id 2 -Method "tools/list" -Params @{}
                        if ( $ListDocsPages )
                        {
                            Send-JsonRpcRequest -Id 3 -Method "tools/call" -Params @{
                                name = "list_docs_pages"
                                arguments = @{
                                    startsWith = $RoutePrefix
                                }
                            }
                        }
                        if ( ![string]::IsNullOrWhiteSpace( $ExampleCode ) )
                        {
                            Send-JsonRpcRequest -Id 4 -Method "tools/call" -Params @{
                                name = "get_example_code"
                                arguments = @{
                                    code = $ExampleCode
                                }
                            }
                        }
                        $initialized = $true
                        $sent = $true
                    }
                }

        if ( $pendingIds.Count -eq 0 )
        {
            $done = $true
        }
    }
}

function Send-JsonRpcRequest
{
    param(
        [int]$Id,
        [string]$Method,
        [object]$Params
    )

    [string] $payload = @{
        jsonrpc = "2.0"
        id = $Id
        method = $Method
        params = $Params
    } | ConvertTo-Json -Depth 6

    [System.Net.Http.HttpRequestMessage] $request = [System.Net.Http.HttpRequestMessage]::new(
        [System.Net.Http.HttpMethod]::Post,
        $messageUri
    )
    $request.Headers.Add( "mcp-session-id", $sessionId )
    $request.Content = [System.Net.Http.StringContent]::new(
        $payload,
        [System.Text.Encoding]::UTF8,
        "application/json"
    )

    [System.Net.Http.HttpResponseMessage] $response = $client.SendAsync( $request ).GetAwaiter().GetResult()

    Write-Host "POST $Method returned $($response.StatusCode)"

    [string] $responseBody = $response.Content.ReadAsStringAsync().GetAwaiter().GetResult()
    if ( ![string]::IsNullOrWhiteSpace( $responseBody ) )
    {
        [object] $payload = $null
        try
        {
            $payload = $responseBody | ConvertFrom-Json
        }
        catch
        {
            Write-Host "HTTP response: $responseBody"
            return
        }

        Process-JsonRpcPayload -Payload $payload -Raw $responseBody -SourceLabel "HTTP"
    }
}

function Send-JsonRpcNotification
{
    param(
        [string]$Method,
        [object]$Params
    )

    [string] $payload = @{
        jsonrpc = "2.0"
        method = $Method
        params = $Params
    } | ConvertTo-Json -Depth 6

    [System.Net.Http.HttpRequestMessage] $request = [System.Net.Http.HttpRequestMessage]::new(
        [System.Net.Http.HttpMethod]::Post,
        $messageUri
    )
    $request.Headers.Add( "mcp-session-id", $sessionId )
    $request.Content = [System.Net.Http.StringContent]::new(
        $payload,
        [System.Text.Encoding]::UTF8,
        "application/json"
    )

    [System.Net.Http.HttpResponseMessage] $response = $client.SendAsync( $request ).GetAwaiter().GetResult()
    Write-Host "POST $Method returned $($response.StatusCode)"

    [string] $responseBody = $response.Content.ReadAsStringAsync().GetAwaiter().GetResult()
    if ( ![string]::IsNullOrWhiteSpace( $responseBody ) )
    {
        [object] $payload = $null
        try
        {
            $payload = $responseBody | ConvertFrom-Json
        }
        catch
        {
            Write-Host "HTTP response: $responseBody"
            return
        }

        Process-JsonRpcPayload -Payload $payload -Raw $responseBody -SourceLabel "HTTP"
    }
}

[string] $eventData = $null
[System.Threading.Tasks.Task[string]] $readTask = $null

while ( !$done )
{
    if ( $TimeoutSeconds -gt 0 -and [System.DateTime]::UtcNow -gt $deadline )
    {
        break
    }

    if ( $null -eq $readTask )
    {
        $readTask = $reader.ReadLineAsync()
    }

    if ( !$readTask.Wait( 1000 ) )
    {
        continue
    }

    if ( $readTask.IsFaulted )
    {
        throw $readTask.Exception.GetBaseException().Message
    }

    if ( $readTask.IsCanceled )
    {
        break
    }

    [string] $line = $readTask.Result
    $readTask = $null

    if ( $null -eq $line )
    {
        break
    }

    if ( $line.Length -eq 0 )
    {
        if ( ![string]::IsNullOrWhiteSpace( $eventData ) )
        {
            [object] $payload = $null

            try
            {
                $payload = $eventData | ConvertFrom-Json
            }
            catch
            {
                Write-Host "SSE: $eventData"
                $eventData = $null
                continue
            }

            [object] $sessionIdValue = Get-PayloadPropertyValue -Payload $payload -Name "sessionId"
            if ( $sessionIdValue -and !$sessionId )
            {
                $sessionId = [string]$sessionIdValue
                Write-Host "SessionId: $sessionId"
            }

            [object] $endpointValue = Get-PayloadPropertyValue -Payload $payload -Name "endpoint"
            if ( $endpointValue )
            {
                $messageUri = [System.Uri]::new( $baseUri, [string]$endpointValue )
                Write-Host "Message endpoint: $messageUri"
            }

            if ( !$initializeRequested -and $sessionId )
            {
                Send-JsonRpcRequest -Id 1 -Method "initialize" -Params @{
                    protocolVersion = $ProtocolVersion
                    capabilities = @{}
                    clientInfo = @{
                        name = "Blazorise.Mcp.Test"
                        version = "0.1"
                    }
                }
                $initializeRequested = $true
            }

            Process-JsonRpcPayload -Payload $payload -Raw $eventData -SourceLabel "SSE"

            $eventData = $null
        }

        continue
    }

    if ( $line.StartsWith( "data:" ) )
    {
        [string] $dataLine = $line.Substring( 5 ).Trim()

        if ( [string]::IsNullOrWhiteSpace( $eventData ) )
        {
            $eventData = $dataLine
        }
        else
        {
            $eventData = $eventData + "`n" + $dataLine
        }
    }
}

if ( !$sessionId )
{
    Write-Host "No sessionId received."
    exit 1
}

if ( $pendingIds.Count -gt 0 )
{
    Write-Host "Timed out waiting for MCP responses."
    exit 1
}
