#region Using directives
using Blazorise;
using Blazorise.Pdf;
using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
#endregion

namespace Blazorise.Tests.Extensions.Pdf;

public class PdfDeclarativeComponentTest : BunitContext
{
    [Fact]
    public void Declarative_Components_Should_Update_And_Unregister_Dynamic_Definitions()
    {
        IRenderedComponent<PdfDocument> component = Render<PdfDocument>( parameters => parameters
            .Add( x => x.Title, "First" )
            .Add( x => x.ChildContent, CreateDocumentContent( true, "First", 40 ) ) );

        PdfDocumentDefinition definition = component.Instance.Definition;
        Assert.Equal( "First", definition.Title );
        Assert.Single( definition.Fonts );
        Assert.Single( definition.Pages );
        Assert.Equal( 2, definition.Pages[0].Elements.Count );

        FontFamily initialFont = definition.Fonts[0];
        PdfPageDefinition initialPage = definition.Pages[0];
        PdfElementDefinition initialText = initialPage.Elements[0];
        PdfElementDefinition initialTable = initialPage.Elements[1];
        PdfTableRowDefinition initialRow = Assert.Single( initialTable.Rows );
        PdfTableCellDefinition initialCell = Assert.Single( initialRow.Cells );
        PdfElementDefinition initialNestedText = Assert.Single( initialCell.Elements );

        component.Render( parameters => parameters
            .Add( x => x.Title, "Second" )
            .Add( x => x.ChildContent, CreateDocumentContent( true, "Second", 80 ) ) );

        Assert.Equal( "Second", definition.Title );
        Assert.Single( definition.Fonts );
        Assert.Same( initialFont, definition.Fonts[0] );
        Assert.Equal( "SecondFont", definition.Fonts[0].Name );
        Assert.Single( definition.Pages );
        Assert.Same( initialPage, definition.Pages[0] );
        Assert.Equal( 2, initialPage.Elements.Count );
        Assert.Same( initialText, initialPage.Elements[0] );
        Assert.Equal( "Second", initialText.Text );
        Assert.Equal( 80d, initialText.Width );
        Assert.Same( initialTable, initialPage.Elements[1] );
        Assert.Same( initialRow, Assert.Single( initialTable.Rows ) );
        Assert.Equal( 36d, initialRow.Height );
        Assert.Same( initialCell, Assert.Single( initialRow.Cells ) );
        Assert.Equal( 80d, initialCell.Width );
        Assert.Same( initialNestedText, Assert.Single( initialCell.Elements ) );
        Assert.Equal( "Nested Second", initialNestedText.Text );

        component.Render( parameters => parameters
            .Add( x => x.Title, "Empty" )
            .Add( x => x.ChildContent, CreateDocumentContent( false, null, 0 ) ) );

        Assert.Equal( "Empty", definition.Title );
        Assert.Empty( definition.Fonts );
        Assert.Empty( definition.Pages );
    }

    private static RenderFragment CreateDocumentContent( bool includeContent, string text, double width )
    {
        return builder =>
        {
            if ( !includeContent )
                return;

            builder.OpenComponent<PdfFont>( 0 );
            builder.SetKey( "font" );
            builder.AddAttribute( 1, nameof( PdfFont.Name ), $"{text}Font" );
            builder.AddAttribute( 2, nameof( PdfFont.Regular ), FontSource.FromBytes( [1] ) );
            builder.CloseComponent();

            builder.OpenComponent<PdfPage>( 3 );
            builder.SetKey( "page" );
            builder.AddAttribute( 4, nameof( PdfPage.Width ), 200d );
            builder.AddAttribute( 5, nameof( PdfPage.Height ), 300d );
            builder.AddAttribute( 6, nameof( PdfPage.ChildContent ), CreatePageContent( text, width ) );
            builder.CloseComponent();
        };
    }

    private static RenderFragment CreatePageContent( string text, double width )
    {
        return builder =>
        {
            builder.OpenComponent<PdfText>( 0 );
            builder.SetKey( "text" );
            builder.AddAttribute( 1, nameof( PdfText.Text ), text );
            builder.AddAttribute( 2, nameof( PdfText.Width ), width );
            builder.AddAttribute( 3, nameof( PdfText.Height ), 20d );
            builder.CloseComponent();

            builder.OpenComponent<PdfTable>( 4 );
            builder.SetKey( "table" );
            builder.AddAttribute( 5, nameof( PdfTable.Width ), 100d );
            builder.AddAttribute( 6, nameof( PdfTable.Height ), 60d );
            builder.AddAttribute( 7, nameof( PdfTable.ChildContent ), CreateTableContent( text, width ) );
            builder.CloseComponent();
        };
    }

    private static RenderFragment CreateTableContent( string text, double width )
    {
        return builder =>
        {
            builder.OpenComponent<PdfTableRow>( 0 );
            builder.SetKey( "row" );
            builder.AddAttribute( 1, nameof( PdfTableRow.Height ), 36d );
            builder.AddAttribute( 2, nameof( PdfTableRow.ChildContent ), CreateRowContent( text, width ) );
            builder.CloseComponent();
        };
    }

    private static RenderFragment CreateRowContent( string text, double width )
    {
        return builder =>
        {
            builder.OpenComponent<PdfTableCell>( 0 );
            builder.SetKey( "cell" );
            builder.AddAttribute( 1, nameof( PdfTableCell.Width ), width );
            builder.AddAttribute( 2, nameof( PdfTableCell.ChildContent ), CreateCellContent( text ) );
            builder.CloseComponent();
        };
    }

    private static RenderFragment CreateCellContent( string text )
    {
        return builder =>
        {
            builder.OpenComponent<PdfText>( 0 );
            builder.SetKey( "nested-text" );
            builder.AddAttribute( 1, nameof( PdfText.Text ), $"Nested {text}" );
            builder.AddAttribute( 2, nameof( PdfText.Width ), 40d );
            builder.AddAttribute( 3, nameof( PdfText.Height ), 20d );
            builder.CloseComponent();
        };
    }
}