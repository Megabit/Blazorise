#region Using directives
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Pdf;

/// <summary>
/// Base class for declarative PDF elements.
/// </summary>
public abstract class BasePdfElement : ComponentBase, IDisposable
{
    #region Methods

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        if ( Definition is null )
        {
            IList<PdfElementDefinition> elements = TableCellContext?.Elements ?? PageContext?.Elements;

            if ( elements is null )
                return;

            Definition = new();
            UpdateDefinition( Definition );
            elements.Add( Definition );
            return;
        }

        UpdateDefinition( Definition );
    }

    /// <summary>
    /// Updates an existing element definition from the current parameters.
    /// </summary>
    /// <param name="definition">Element definition to update.</param>
    protected virtual void UpdateDefinition( PdfElementDefinition definition )
    {
        definition.Type = ElementType;
        definition.X = X;
        definition.Y = Y;
        definition.Width = Width;
        definition.Height = Height;
        definition.Text = Text;
        definition.Wrap = Wrap;
        definition.Source = Source;

        definition.Font ??= new();
        definition.Font.Family = FontFamily;
        definition.Font.Size = FontSize;
        definition.Font.Color = TextColor;
        definition.Font.Alignment = TextAlignment;
        definition.Font.VerticalAlignment = VerticalAlignment;
        definition.Font.Bold = Bold;
        definition.Font.Italic = Italic;

        definition.Border ??= new();
        definition.Border.Color = BorderColor;
        definition.Border.Width = BorderWidth;

        definition.Appearance ??= new();
        definition.Appearance.BackgroundColor = BackgroundColor;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        ( TableCellContext?.Elements ?? PageContext?.Elements )?.Remove( Definition );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Element type.
    /// </summary>
    protected abstract PdfElementType ElementType { get; }

    /// <summary>
    /// Gets the generated element definition.
    /// </summary>
    public PdfElementDefinition Definition { get; private set; }

    /// <summary>
    /// Provides the current PDF page that receives this element definition.
    /// </summary>
    [CascadingParameter] protected PdfPageContext PageContext { get; set; }

    /// <summary>
    /// Provides the current PDF table cell that receives this element definition.
    /// </summary>
    [CascadingParameter] protected PdfTableCellContext TableCellContext { get; set; }

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