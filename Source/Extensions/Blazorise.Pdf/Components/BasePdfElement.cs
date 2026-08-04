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
        definition.ClipContent = ElementClipContent;

        definition.Border ??= new();
        definition.Border.Color = BorderColor;
        definition.Border.Width = BorderWidth;

        definition.Appearance ??= new();
        definition.Appearance.BackgroundColor = ElementBackgroundColor;
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
    /// Indicates that content should be clipped to the element bounds.
    /// </summary>
    protected virtual bool ElementClipContent => true;

    /// <summary>
    /// Background color in hexadecimal format.
    /// </summary>
    protected virtual string ElementBackgroundColor => null;

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
    /// Border color in hexadecimal format.
    /// </summary>
    [Parameter] public string BorderColor { get; set; } = "#000000";

    /// <summary>
    /// Border width.
    /// </summary>
    [Parameter] public double BorderWidth { get; set; }

    #endregion
}