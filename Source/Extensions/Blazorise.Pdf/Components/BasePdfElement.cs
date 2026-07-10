#region Using directives
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Pdf;

/// <summary>
/// Base class for declarative PDF elements.
/// </summary>
public abstract class BasePdfElement : ComponentBase
{
    #region Members

    private IList<PdfElementDefinition> previousElements;

    private PdfElementDefinition previousDefinition;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the generated element definition.
    /// </summary>
    public PdfElementDefinition Definition { get; private set; }

    #endregion

    #region Methods

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        IList<PdfElementDefinition> elements = TableCellContext?.Elements ?? PageContext?.Elements;

        if ( elements is null )
            return;

        if ( previousElements is not null && previousDefinition is not null )
            previousElements.Remove( previousDefinition );

        Definition = CreateDefinition();
        elements.Add( Definition );

        previousDefinition = Definition;
        previousElements = elements;
    }

    /// <summary>
    /// Creates the element definition.
    /// </summary>
    /// <returns>The created element definition.</returns>
    protected virtual PdfElementDefinition CreateDefinition()
    {
        return new()
        {
            Type = ElementType,
            X = X,
            Y = Y,
            Width = Width,
            Height = Height,
            Text = Text,
            Wrap = Wrap,
            Source = Source,
            Font = new()
            {
                Family = FontFamily,
                Size = FontSize,
                Color = TextColor,
                Alignment = TextAlignment,
                VerticalAlignment = VerticalAlignment,
                Bold = Bold,
                Italic = Italic,
            },
            Border = new()
            {
                Color = BorderColor,
                Width = BorderWidth,
            },
            Appearance = new()
            {
                BackgroundColor = BackgroundColor,
            },
        };
    }

    #endregion

    #region Parameters

    /// <summary>
    /// Provides the current PDF page that receives this element definition.
    /// </summary>
    [CascadingParameter] protected PdfPageContext PageContext { get; set; }

    /// <summary>
    /// Provides the current PDF table cell that receives this element definition.
    /// </summary>
    [CascadingParameter] protected PdfTableCellContext TableCellContext { get; set; }

    /// <summary>
    /// Element type.
    /// </summary>
    protected abstract PdfElementType ElementType { get; }

    /// <summary>
    /// Horizontal element position.
    /// </summary>
    [Parameter] public double X { get; set; }

    /// <summary>
    /// Vertical element position.
    /// </summary>
    [Parameter] public double Y { get; set; }

    /// <summary>
    /// Element width.
    /// </summary>
    [Parameter] public double Width { get; set; }

    /// <summary>
    /// Element height.
    /// </summary>
    [Parameter] public double Height { get; set; }

    /// <summary>
    /// Text rendered by text-based elements.
    /// </summary>
    [Parameter] public string Text { get; set; }

    /// <summary>
    /// Indicates that text should wrap inside the element bounds.
    /// </summary>
    [Parameter] public bool Wrap { get; set; } = true;

    /// <summary>
    /// Image source used by image elements.
    /// </summary>
    [Parameter] public string Source { get; set; }

    /// <summary>
    /// Font family used by text-based elements. The built-in renderer maps the family to the closest PDF standard font (Helvetica, Times, or Courier).
    /// </summary>
    [Parameter] public string FontFamily { get; set; } = "Helvetica";

    /// <summary>
    /// Font size used by text-based elements.
    /// </summary>
    [Parameter] public double FontSize { get; set; } = 12;

    /// <summary>
    /// Text color in hexadecimal format.
    /// </summary>
    [Parameter] public string TextColor { get; set; } = "#000000";

    /// <summary>
    /// Text alignment inside the element bounds.
    /// </summary>
    /// <remarks>
    /// <see cref="TextAlignment.Default"/> and <see cref="TextAlignment.Start"/> align to the start.
    /// <see cref="TextAlignment.Justified"/> distributes words across wrapped non-final paragraph lines.
    /// </remarks>
    [Parameter] public TextAlignment TextAlignment { get; set; }

    /// <summary>
    /// Text vertical alignment inside the element bounds.
    /// </summary>
    /// <remarks>
    /// <see cref="VerticalAlignment.Default"/>, <see cref="VerticalAlignment.Baseline"/>,
    /// <see cref="VerticalAlignment.Top"/>, and <see cref="VerticalAlignment.TextTop"/> align to the top.
    /// <see cref="VerticalAlignment.Middle"/> centers the text, while <see cref="VerticalAlignment.Bottom"/>
    /// and <see cref="VerticalAlignment.TextBottom"/> align to the bottom.
    /// </remarks>
    [Parameter] public VerticalAlignment VerticalAlignment { get; set; }

    /// <summary>
    /// Makes text bold.
    /// </summary>
    [Parameter] public bool Bold { get; set; }

    /// <summary>
    /// Makes text italic.
    /// </summary>
    [Parameter] public bool Italic { get; set; }

    /// <summary>
    /// Border color in hexadecimal format.
    /// </summary>
    [Parameter] public string BorderColor { get; set; } = "#000000";

    /// <summary>
    /// Border width.
    /// </summary>
    [Parameter] public double BorderWidth { get; set; } = 1;

    /// <summary>
    /// Background color in hexadecimal format.
    /// </summary>
    [Parameter] public string BackgroundColor { get; set; }

    #endregion
}