#region Using directives
using System;
using Blazorise;
using Blazorise.Extensions;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
#endregion

namespace Blazorise.Reporting.Internal;

internal static class ReportDesignerPropertiesRenderer
{
    #region Methods

    internal static void Render( RenderTreeBuilder builder, ReportDesignerPropertiesContext context )
    {
        var hasSelection = context.SelectionManager.ReportSelected
            || context.SelectionManager.SelectedSectionIndex is not null
            || !string.IsNullOrWhiteSpace( context.SelectionManager.SelectedElementKey );

        RenderReportProperties( builder, context );
        RenderSelectedSectionProperties( builder, context );
        RenderSelectedElementProperties( builder, context );
        RenderSelectedElementTools( builder, context );

        if ( hasSelection )
            return;

        builder.OpenComponent<Paragraph>();
        builder.Attribute( "TextColor", TextColor.Secondary );
        builder.Attribute( "ChildContent", (RenderFragment)( paragraphBuilder => paragraphBuilder.Content( "Select a band or report element to edit its properties." ) ) );
        builder.CloseComponent();
    }

    private static void RenderReportProperties( RenderTreeBuilder builder, ReportDesignerPropertiesContext context )
    {
        if ( !context.SelectionManager.ReportSelected )
            return;

        builder.OpenComponent<Div>();
        builder.Attribute( "Margin", Margin.Is3.FromBottom );
        builder.Attribute( "ChildContent", (RenderFragment)( childBuilder =>
        {
            childBuilder.OpenElement( "h6" );
            childBuilder.Content( "Report properties" );
            childBuilder.CloseElement();

            ReportPropertyPanelRenderer.RenderCheckbox( childBuilder, context.EventReceiver, "Snap to grid", context.SnapToGrid, context.SnapToGridChanged );
            ReportPropertyPanelRenderer.RenderNumberInput( childBuilder, context.EventReceiver, "Page width", context.Definition.Page.Width, value => context.UpdateReportPage( page => page.Width = Math.Max( 1, value ) ) );
            ReportPropertyPanelRenderer.RenderNumberInput( childBuilder, context.EventReceiver, "Page height", context.Definition.Page.Height, value => context.UpdateReportPage( page => page.Height = Math.Max( 1, value ) ) );
        } ) );
        builder.CloseComponent();
    }

    private static void RenderSelectedElementTools( RenderTreeBuilder builder, ReportDesignerPropertiesContext context )
    {
        var selected = context.SelectionManager.FindSelectedElement( context.Definition );

        if ( selected is null )
            return;

        builder.OpenComponent<Div>();
        builder.Attribute( "Margin", Margin.Is3.FromTop );
        builder.Attribute( "ChildContent", (RenderFragment)( childBuilder =>
        {
            childBuilder.OpenElement( "h6" );
            childBuilder.Content( "Selected element" );
            childBuilder.CloseElement();

            childBuilder.OpenComponent<Div>();
            childBuilder.Attribute( "Flex", Flex.Wrap );
            childBuilder.Attribute( "Gap", Gap.Is2 );
            childBuilder.Attribute( "ChildContent", (RenderFragment)( toolsBuilder =>
            {
                ReportPropertyPanelRenderer.RenderButton( toolsBuilder, context.EventReceiver, "Left", () => context.MoveSelectedElement( -8, 0, 0, 0 ) );
                ReportPropertyPanelRenderer.RenderButton( toolsBuilder, context.EventReceiver, "Up", () => context.MoveSelectedElement( 0, -8, 0, 0 ) );
                ReportPropertyPanelRenderer.RenderButton( toolsBuilder, context.EventReceiver, "Down", () => context.MoveSelectedElement( 0, 8, 0, 0 ) );
                ReportPropertyPanelRenderer.RenderButton( toolsBuilder, context.EventReceiver, "Right", () => context.MoveSelectedElement( 8, 0, 0, 0 ) );
                ReportPropertyPanelRenderer.RenderButton( toolsBuilder, context.EventReceiver, "Wider", () => context.MoveSelectedElement( 0, 0, 16, 0 ) );
                ReportPropertyPanelRenderer.RenderButton( toolsBuilder, context.EventReceiver, "Taller", () => context.MoveSelectedElement( 0, 0, 0, 8 ) );
            } ) );
            childBuilder.CloseComponent();
        } ) );
        builder.CloseComponent();
    }

    private static void RenderSelectedSectionProperties( RenderTreeBuilder builder, ReportDesignerPropertiesContext context )
    {
        var selected = context.SelectionManager.FindSelectedSection( context.Definition );

        if ( selected is null || !string.IsNullOrWhiteSpace( context.SelectionManager.SelectedElementKey ) )
            return;

        builder.OpenComponent<Div>();
        builder.Attribute( "Margin", Margin.Is3.FromBottom );
        builder.Attribute( "ChildContent", (RenderFragment)( childBuilder =>
        {
            childBuilder.OpenElement( "h6" );
            childBuilder.Content( "Band properties" );
            childBuilder.CloseElement();

            ReportPropertyPanelRenderer.RenderGroup( childBuilder, "Status", groupBuilder =>
            {
                ReportPropertyPanelRenderer.RenderCheckbox( groupBuilder, context.EventReceiver, "Suppress", selected.Suppressed, eventArgs => _ = context.UpdateSelectedSectionSuppression( eventArgs.Value is bool value && value ) );
            } );

            if ( selected.Suppressed )
                return;

            ReportPropertyPanelRenderer.RenderGroup( childBuilder, "General", groupBuilder =>
            {
                ReportPropertyPanelRenderer.RenderInput( groupBuilder, context.EventReceiver, "Name", selected.Name, value => context.UpdateSelectedSection( section => section.Name = value ) );
                ReportPropertyPanelRenderer.RenderInput( groupBuilder, context.EventReceiver, "Data source", selected.DataSource, value => context.UpdateSelectedSection( section => section.DataSource = value ) );
            } );

            ReportPropertyPanelRenderer.RenderGroup( childBuilder, "Layout", groupBuilder =>
            {
                ReportPropertyPanelRenderer.RenderNumberInput( groupBuilder, context.EventReceiver, "Height", selected.Height, value => context.UpdateSelectedSection( section => section.Height = Math.Max( 8, value ) ) );
            } );

            ReportPropertyPanelRenderer.RenderGroup( childBuilder, "Advanced", groupBuilder =>
            {
                ReportPropertyPanelRenderer.RenderInput( groupBuilder, context.EventReceiver, "Custom CSS", selected.Style, value => context.UpdateSelectedSection( section => section.Style = value ) );
            } );

            childBuilder.OpenComponent<Div>();
            childBuilder.Attribute( "Flex", Flex.Wrap );
            childBuilder.Attribute( "Gap", Gap.Is2 );
            childBuilder.Attribute( "Margin", Margin.Is3.FromTop );
            childBuilder.Attribute( "ChildContent", (RenderFragment)( toolsBuilder =>
            {
                ReportPropertyPanelRenderer.RenderButton( toolsBuilder, context.EventReceiver, "Insert before", () => context.InsertSection( false ) );
                ReportPropertyPanelRenderer.RenderButton( toolsBuilder, context.EventReceiver, "Insert after", () => context.InsertSection( true ) );

                if ( ReportDefinitionHelper.CanDeleteSection( selected ) )
                {
                    ReportPropertyPanelRenderer.RenderButton( toolsBuilder, context.EventReceiver, "Delete band", context.DeleteSelectedSection );
                }
            } ) );
            childBuilder.CloseComponent();
        } ) );
        builder.CloseComponent();
    }

    private static void RenderSelectedElementProperties( RenderTreeBuilder builder, ReportDesignerPropertiesContext context )
    {
        var selected = context.SelectionManager.FindSelectedElement( context.Definition );

        if ( selected is null || context.SelectionManager.IsSelectedElementSuppressed( context.Definition ) )
            return;

        builder.OpenComponent<Div>();
        builder.Attribute( "Margin", Margin.Is3.FromTop );
        builder.Attribute( "ChildContent", (RenderFragment)( childBuilder =>
        {
            childBuilder.OpenElement( "h6" );
            childBuilder.Content( "Element properties" );
            childBuilder.CloseElement();

            ReportPropertyPanelRenderer.RenderGroup( childBuilder, "General", groupBuilder =>
            {
                ReportPropertyPanelRenderer.RenderInput( groupBuilder, context.EventReceiver, "Name", selected.Name, value => context.UpdateSelectedElement( element => element.Name = value ) );
            } );

            RenderElementContentProperties( childBuilder, context, selected );
            RenderElementLayoutProperties( childBuilder, context, selected );
            RenderElementTextProperties( childBuilder, context, selected );
            RenderElementAppearanceProperties( childBuilder, context, selected );

            ReportPropertyPanelRenderer.RenderGroup( childBuilder, "Advanced", groupBuilder =>
            {
                ReportPropertyPanelRenderer.RenderInput( groupBuilder, context.EventReceiver, "CSS classes", selected.Class, value => context.UpdateSelectedElement( element => element.Class = value ) );
                ReportPropertyPanelRenderer.RenderInput( groupBuilder, context.EventReceiver, "Custom CSS", selected.Style, value => context.UpdateSelectedElement( element => element.Style = value ) );
            } );
        } ) );
        builder.CloseComponent();
    }

    private static void RenderElementContentProperties( RenderTreeBuilder builder, ReportDesignerPropertiesContext context, ReportElementDefinition selected )
    {
        if ( selected.Type != ReportElementType.Text
            && selected.Type != ReportElementType.Field
            && selected.Type != ReportElementType.Image )
            return;

        ReportPropertyPanelRenderer.RenderGroup( builder, selected.Type == ReportElementType.Field ? "Data" : "Content", groupBuilder =>
        {
            switch ( selected.Type )
            {
                case ReportElementType.Text:
                    ReportPropertyPanelRenderer.RenderInput( groupBuilder, context.EventReceiver, "Text", selected.Text, value => context.UpdateSelectedElement( element => element.Text = value ) );
                    break;
                case ReportElementType.Field:
                    ReportPropertyPanelRenderer.RenderInput( groupBuilder, context.EventReceiver, "Expression", ReportDefinitionHelper.FormatFieldExpression( selected ), valueChanged: null, readOnly: true );
                    ReportPropertyPanelRenderer.RenderInput( groupBuilder, context.EventReceiver, "Format", selected.Format, value => context.UpdateSelectedElement( element => element.Format = value ) );
                    break;
                case ReportElementType.Image:
                    ReportPropertyPanelRenderer.RenderInput( groupBuilder, context.EventReceiver, "Source", selected.Source, value => context.UpdateSelectedElement( element => element.Source = value ) );
                    ReportPropertyPanelRenderer.RenderInput( groupBuilder, context.EventReceiver, "Alt text", selected.Text, value => context.UpdateSelectedElement( element => element.Text = value ) );
                    break;
            }
        } );
    }

    private static void RenderElementLayoutProperties( RenderTreeBuilder builder, ReportDesignerPropertiesContext context, ReportElementDefinition selected )
    {
        ReportPropertyPanelRenderer.RenderGroup( builder, "Position and size", groupBuilder =>
        {
            ReportPropertyPanelRenderer.RenderNumberInput( groupBuilder, context.EventReceiver, "X", selected.X, value => context.UpdateSelectedElement( element => element.X = value ) );
            ReportPropertyPanelRenderer.RenderNumberInput( groupBuilder, context.EventReceiver, "Y", selected.Y, value => context.UpdateSelectedElement( element => element.Y = value ) );
            ReportPropertyPanelRenderer.RenderNumberInput( groupBuilder, context.EventReceiver, "Width", selected.Width, value => context.UpdateSelectedElement( element => element.Width = value ) );
            ReportPropertyPanelRenderer.RenderNumberInput( groupBuilder, context.EventReceiver, "Height", selected.Height, value => context.UpdateSelectedElement( element => element.Height = value ) );
        } );
    }

    private static void RenderElementTextProperties( RenderTreeBuilder builder, ReportDesignerPropertiesContext context, ReportElementDefinition selected )
    {
        if ( selected.Type is ReportElementType.Image or ReportElementType.Line or ReportElementType.Rectangle or ReportElementType.PageBreak )
            return;

        var font = ReportElementDefinitionHelper.EnsureFont( selected );

        ReportPropertyPanelRenderer.RenderGroup( builder, "Text", groupBuilder =>
        {
            ReportPropertyPanelRenderer.RenderInput( groupBuilder, context.EventReceiver, "Font family", font.Family, value => context.UpdateSelectedElement( element => ReportElementDefinitionHelper.EnsureFont( element ).Family = value ) );
            ReportPropertyPanelRenderer.RenderNullableNumberInput( groupBuilder, context.EventReceiver, "Font size", font.Size, value => context.UpdateSelectedElement( element => ReportElementDefinitionHelper.EnsureFont( element ).Size = ReportElementDefinitionHelper.NormalizeNullablePositiveNumber( value ) ) );
            ReportPropertyPanelRenderer.RenderColorInput( groupBuilder, context.EventReceiver, "Font color", font.Color, value => context.UpdateSelectedElement( element => ReportElementDefinitionHelper.EnsureFont( element ).Color = value ) );
            ReportPropertyPanelRenderer.RenderSelectInput( groupBuilder, context.EventReceiver, "Alignment", font.Alignment, value => context.UpdateSelectedElement( element => ReportElementDefinitionHelper.EnsureFont( element ).Alignment = value ) );
            ReportPropertyPanelRenderer.RenderCheckbox( groupBuilder, context.EventReceiver, "Bold", font.Bold, eventArgs => _ = context.UpdateSelectedElement( element => ReportElementDefinitionHelper.EnsureFont( element ).Bold = eventArgs.Value is bool value && value ) );
            ReportPropertyPanelRenderer.RenderCheckbox( groupBuilder, context.EventReceiver, "Italic", font.Italic, eventArgs => _ = context.UpdateSelectedElement( element => ReportElementDefinitionHelper.EnsureFont( element ).Italic = eventArgs.Value is bool value && value ) );
            ReportPropertyPanelRenderer.RenderCheckbox( groupBuilder, context.EventReceiver, "Underline", font.Underline, eventArgs => _ = context.UpdateSelectedElement( element => ReportElementDefinitionHelper.EnsureFont( element ).Underline = eventArgs.Value is bool value && value ) );
        } );
    }

    private static void RenderElementAppearanceProperties( RenderTreeBuilder builder, ReportDesignerPropertiesContext context, ReportElementDefinition selected )
    {
        var appearance = ReportElementDefinitionHelper.EnsureAppearance( selected );
        var border = ReportElementDefinitionHelper.EnsureBorder( selected );

        ReportPropertyPanelRenderer.RenderGroup( builder, "Appearance", groupBuilder =>
        {
            ReportPropertyPanelRenderer.RenderColorInput( groupBuilder, context.EventReceiver, "Fill color", appearance.BackgroundColor, value => context.UpdateSelectedElement( element => ReportElementDefinitionHelper.EnsureAppearance( element ).BackgroundColor = value ) );
            ReportPropertyPanelRenderer.RenderColorInput( groupBuilder, context.EventReceiver, "Border color", border.Color, value => context.UpdateSelectedElement( element => ReportElementDefinitionHelper.EnsureBorder( element ).Color = value ) );
            ReportPropertyPanelRenderer.RenderNullableNumberInput( groupBuilder, context.EventReceiver, "Border width", border.Width, value => context.UpdateSelectedElement( element => ReportElementDefinitionHelper.EnsureBorder( element ).Width = ReportElementDefinitionHelper.NormalizeNullablePositiveNumber( value ) ) );
            ReportPropertyPanelRenderer.RenderNullableNumberInput( groupBuilder, context.EventReceiver, "Corner radius", border.Radius, value => context.UpdateSelectedElement( element => ReportElementDefinitionHelper.EnsureBorder( element ).Radius = ReportElementDefinitionHelper.NormalizeNullablePositiveNumber( value ) ) );
            ReportPropertyPanelRenderer.RenderNullableNumberInput( groupBuilder, context.EventReceiver, "Opacity", appearance.Opacity, value => context.UpdateSelectedElement( element => ReportElementDefinitionHelper.EnsureAppearance( element ).Opacity = ReportElementDefinitionHelper.NormalizeOpacity( value ) ) );
        } );
    }

    #endregion
}