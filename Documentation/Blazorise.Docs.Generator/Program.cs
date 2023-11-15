using System.Text;
using System.Xml;
using System.Xml.Linq;

//Don't bother with making good/pretty code, this is just for quick dirty docs generation saving us some time.

//XML Doc example
//<member name="T:Blazorise.ButtonsRole">
//    <summary>
//    Buttons group behaviour.
//    </summary>
//</member>
//<member name="F:Blazorise.ButtonsRole.Addons">
//    <summary>
//    Display buttons as addons.
//    </summary>
//</member>
//<member name="F:Blazorise.ButtonsRole.Toolbar">
//    <summary>
//    Display buttons as toolbar buttons.
//    </summary>
//</member>
var blzSrcPath = @"..\..\..\..\..\Source\Blazorise";
var file = @$"{blzSrcPath}\obj\Debug\net8.0\Blazorise.xml";
var enumsFolder = @$"{blzSrcPath}\Enums";
string outputPath = @$"{blzSrcPath}\obj\Debug\net8.0\EnumsOutput.txt";

var enumsFolderDirectory = new DirectoryInfo( enumsFolder );
var enumFileNames = enumsFolderDirectory.GetFiles().Select( x => x.Name.Split( ".cs" )[0] );

XElement doc = XElement.Load( file );
var enums = doc.Descendants( "member" ).Where( x => enumFileNames
       .Any( y => x.FirstAttribute.Value.Equals( $"T:Blazorise.{y}" ) ||
                        x.FirstAttribute.Value.StartsWith( $"F:Blazorise.{y}." ) ) )
    .GroupBy( x => x.FirstAttribute.Value.Split( '.' )[1] ); //Don't think there will be a collision, improve groupby if there's a collision


//Our Docs Model
//<DocsPageSection>
//    <DocsPageSectionHeader Title="Breakpoint">
//        <Paragraph>Defines the media breakpoint.</Paragraph>
//        <UnorderedList>
//            <UnorderedListItem><Code>None</Code> Undefined.</UnorderedListItem>
//            <UnorderedListItem><Code>Mobile</Code> Valid on all devices. (extra small)</UnorderedListItem>
//            <UnorderedListItem><Code>Tablet</Code> Breakpoint on tablets (small).</UnorderedListItem>
//            <UnorderedListItem><Code>Desktop</Code> Breakpoint on desktop (medium).</UnorderedListItem>
//            <UnorderedListItem><Code>Widescreen</Code> Breakpoint on widescreen (large).</UnorderedListItem>
//            <UnorderedListItem><Code>FullHD</Code> Breakpoint on large desktops (extra large).</UnorderedListItem>
//        </UnorderedList>
//    </DocsPageSectionHeader>
//</DocsPageSection>

var sb = new StringBuilder();
var tabAsSpaces = "    ";
foreach ( var enumGroup in enums )
{
    sb.AppendLine( "<DocsPageSection>" );
    sb.AppendLine( $"{tabAsSpaces}<DocsPageSectionHeader Title=\"{enumGroup.Key}\">" );
    sb.AppendLine( $"{tabAsSpaces}{tabAsSpaces}<Paragraph>{enumGroup.First().Descendants().First().FirstNode.ToString().Trim()}</Paragraph>" );
    sb.AppendLine( $"{tabAsSpaces}{tabAsSpaces}<UnorderedList>" );

    foreach ( var enumChild in enumGroup.Skip( 1 ) )
    {
        var enumChildName = enumChild.FirstAttribute.Value.Replace( "F:Blazorise.", "" );
        sb.AppendLine( $"{tabAsSpaces}{tabAsSpaces}{tabAsSpaces}<UnorderedListItem><Code>{enumChildName}</Code> {enumChild.Descendants().First().FirstNode.ToString().Trim()}</UnorderedListItem>" );
    }

    sb.AppendLine( $"{tabAsSpaces}{tabAsSpaces}</UnorderedList>" );
    sb.AppendLine( $"{tabAsSpaces}</DocsPageSectionHeader>" );
    sb.AppendLine( "</DocsPageSection>" );
    sb.AppendLine();
}

var outputFileInfo = new FileInfo( outputPath );
using var sw = new System.IO.StreamWriter( outputFileInfo.FullName );
await sw.WriteLineAsync( sb.ToString() );
sw.Close();

Console.WriteLine( "Success!" );
Console.WriteLine( outputFileInfo.FullName );
Console.ReadKey();

