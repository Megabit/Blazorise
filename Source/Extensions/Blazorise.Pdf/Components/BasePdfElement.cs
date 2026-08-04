#region Using directives
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Pdf;

/// <summary>
/// Base class for declarative PDF elements.
/// </summary>
public abstract class BasePdfElement : ComponentBase, IDisposable
{
    #region Members

    private IList<PdfElementDefinition> registeredElements;

    private PdfPageContext pageContext;

    private PdfTableCellContext tableCellContext;

    #endregion

    #region Methods

    /// <inheritdoc />
    public override Task SetParametersAsync( ParameterView parameters )
    {
        bool definitionChanged = IsDefinitionChanged( parameters );
        Task task = base.SetParametersAsync( parameters );

        if ( definitionChanged )
            UpdateDefinition( Definition );

        return task;
    }

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        UpdateDefinition( Definition );
        RegisterDefinition();
    }

    /// <summary>
    /// Determines whether parameters that affect the element definition have changed.
    /// </summary>
    protected virtual bool IsDefinitionChanged( ParameterView parameters )
    {
        return parameters.IsParameterChanged( X )
            || parameters.IsParameterChanged( Y )
            || parameters.IsParameterChanged( Width )
            || parameters.IsParameterChanged( Height )
            || parameters.IsParameterChanged( BorderColor )
            || parameters.IsParameterChanged( BorderWidth );
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

    private void RegisterDefinition()
    {
        IList<PdfElementDefinition> elements = tableCellContext?.Elements ?? pageContext?.Elements;

        if ( ReferenceEquals( registeredElements, elements ) )
            return;

        registeredElements?.Remove( Definition );
        registeredElements = elements;

        if ( registeredElements is not null && !registeredElements.Contains( Definition ) )
            registeredElements.Add( Definition );
    }

    /// <inheritdoc />
    public void Dispose()
    {
        registeredElements?.Remove( Definition );
        registeredElements = null;
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
    public PdfElementDefinition Definition { get; } = new();

    /// <summary>
    /// Provides the current PDF page that receives this element definition.
    /// </summary>
    [CascadingParameter]
    protected PdfPageContext PageContext
    {
        get => pageContext;
        set
        {
            if ( ReferenceEquals( pageContext, value ) )
                return;

            pageContext = value;
            RegisterDefinition();
        }
    }

    /// <summary>
    /// Provides the current PDF table cell that receives this element definition.
    /// </summary>
    [CascadingParameter]
    protected PdfTableCellContext TableCellContext
    {
        get => tableCellContext;
        set
        {
            if ( ReferenceEquals( tableCellContext, value ) )
                return;

            tableCellContext = value;
            RegisterDefinition();
        }
    }

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