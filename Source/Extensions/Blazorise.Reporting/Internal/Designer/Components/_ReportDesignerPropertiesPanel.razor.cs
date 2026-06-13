#region Using directives
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting.Internal;

/// <summary>
/// Renders the report designer properties editor.
/// </summary>
public partial class _ReportDesignerPropertiesPanel
{
    #region Members

    private _ReportDesignerDataSourceDialog dataSourceDialogRef;

    private static readonly (string Value, string Text)[] TextColorOptions =
    [
        ( string.Empty, "Default" ),
        ( "primary", "Primary" ),
        ( "secondary", "Secondary" ),
        ( "success", "Success" ),
        ( "danger", "Danger" ),
        ( "warning", "Warning" ),
        ( "info", "Info" ),
        ( "light", "Light" ),
        ( "dark", "Dark" ),
        ( "body", "Body" ),
        ( "muted", "Muted" ),
        ( "white", "White" ),
        ( "black-50", "Black 50" ),
        ( "white-50", "White 50" ),
    ];

    private static readonly (string Value, string Text)[] BackgroundOptions =
    [
        ( string.Empty, "Default" ),
        ( "primary", "Primary" ),
        ( "secondary", "Secondary" ),
        ( "success", "Success" ),
        ( "danger", "Danger" ),
        ( "warning", "Warning" ),
        ( "info", "Info" ),
        ( "light", "Light" ),
        ( "dark", "Dark" ),
        ( "white", "White" ),
        ( "transparent", "Transparent" ),
        ( "body", "Body" ),
    ];

    private static readonly (ReportPageSize Value, string Text)[] PageSizeOptions =
    [
        ( ReportPageSize.Custom, "Custom" ),
        ( ReportPageSize.A3, "A3" ),
        ( ReportPageSize.A4, "A4" ),
        ( ReportPageSize.A5, "A5" ),
        ( ReportPageSize.Letter, "Letter" ),
        ( ReportPageSize.Legal, "Legal" ),
    ];

    private static readonly (ReportOrientation Value, string Text)[] PageOrientationOptions =
    [
        ( ReportOrientation.Portrait, "Portrait" ),
        ( ReportOrientation.Landscape, "Landscape" ),
    ];

    #endregion

    #region Methods

    private bool HasSelection => ReportSelected || SelectedSection is not null || SelectedElement is not null;

    private ReportAppearanceDefinition EnsureSelectedSectionAppearance()
    {
        return EnsureSectionAppearance( SelectedSection );
    }

    private ReportBorderDefinition EnsureSelectedSectionBorder()
    {
        return EnsureSectionBorder( SelectedSection );
    }

    private static ReportAppearanceDefinition EnsureSectionAppearance( ReportSectionDefinition section )
    {
        return section.Appearance ??= new();
    }

    private static ReportBorderDefinition EnsureSectionBorder( ReportSectionDefinition section )
    {
        return section.Border ??= new();
    }

    private static ReportPageMarginsDefinition EnsurePageMargins( ReportPageDefinition page )
    {
        return page.Margins ??= new();
    }

    private Task OnPageSizeChanged( ReportPageSize value )
    {
        return UpdateReportPage( page => ReportPageDefinitionHelper.ApplySize( page, value ) );
    }

    private Task OnPageOrientationChanged( ReportOrientation value )
    {
        return UpdateReportPage( page => ReportPageDefinitionHelper.ApplyOrientation( page, value ) );
    }

    private Task OnPageWidthChanged( double value )
    {
        return UpdateReportPage( page =>
        {
            page.Size = ReportPageSize.Custom;
            page.Width = Math.Max( 1, value );
        } );
    }

    private Task OnPageHeightChanged( double value )
    {
        return UpdateReportPage( page =>
        {
            page.Size = ReportPageSize.Custom;
            page.Height = Math.Max( 1, value );
        } );
    }

    private Task UpdateSelectedElementTextColor( string value )
    {
        return UpdateSelectedElement( element =>
        {
            var font = ReportElementDefinitionHelper.EnsureFont( element );

            font.TextColor = string.IsNullOrWhiteSpace( value ) ? null : value;
            font.Color = null;
        } );
    }

    private Task UpdateSelectedElementFontColor( string value )
    {
        return UpdateSelectedElement( element =>
        {
            var font = ReportElementDefinitionHelper.EnsureFont( element );

            font.Color = string.IsNullOrWhiteSpace( value ) ? null : value;
            font.TextColor = null;
        } );
    }

    private Task UpdateSelectedElementBackground( string value )
    {
        return UpdateSelectedElement( element =>
        {
            var appearance = ReportElementDefinitionHelper.EnsureAppearance( element );

            appearance.Background = string.IsNullOrWhiteSpace( value ) ? null : value;
            appearance.BackgroundColor = null;
        } );
    }

    private Task UpdateSelectedElementBackgroundColor( string value )
    {
        return UpdateSelectedElement( element =>
        {
            var appearance = ReportElementDefinitionHelper.EnsureAppearance( element );

            appearance.BackgroundColor = string.IsNullOrWhiteSpace( value ) ? null : value;
            appearance.Background = null;
        } );
    }

    private void OnSnapToGridChanged( bool value )
    {
        SnapToGridChanged?.Invoke( value );
    }

    private string GetSelectedSectionDataSourceDisplayName()
    {
        if ( string.IsNullOrWhiteSpace( SelectedSection?.DataSource ) )
            return null;

        ReportDesignerDataSourceOption dataSource = ReportDataSourceExplorer.ResolveBindableDataSources( Definition ).FirstOrDefault( option =>
            string.Equals( option.Value, SelectedSection.DataSource, StringComparison.OrdinalIgnoreCase )
            || string.Equals( option.DisplayName, SelectedSection.DataSource, StringComparison.OrdinalIgnoreCase ) );

        return dataSource?.DisplayName ?? SelectedSection.DataSource;
    }

    private Task OpenDataSourceDialogAsync()
    {
        return dataSourceDialogRef?.ShowAsync( SelectedSection?.DataSource ) ?? Task.CompletedTask;
    }

    private Task UpdateSelectedSectionDataSource( string value )
    {
        return UpdateSelectedSection( section => section.DataSource = string.IsNullOrWhiteSpace( value ) ? null : value );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Report definition whose page settings are edited.
    /// </summary>
    [Parameter] public ReportDefinition Definition { get; set; }

    /// <summary>
    /// Indicates that the root report node is selected.
    /// </summary>
    [Parameter] public bool ReportSelected { get; set; }

    /// <summary>
    /// Selected band definition, when a band is selected.
    /// </summary>
    [Parameter] public ReportSectionDefinition SelectedSection { get; set; }

    /// <summary>
    /// Selected element definition, when an element is selected.
    /// </summary>
    [Parameter] public ReportElementDefinition SelectedElement { get; set; }

    /// <summary>
    /// Indicates that the selected element belongs to a suppressed band.
    /// </summary>
    [Parameter] public bool SelectedElementSuppressed { get; set; }

    /// <summary>
    /// Indicates that designer movement snaps to the grid.
    /// </summary>
    [Parameter] public bool SnapToGrid { get; set; }

    /// <summary>
    /// Raised when snap-to-grid is toggled.
    /// </summary>
    [Parameter] public Action<bool> SnapToGridChanged { get; set; }

    /// <summary>
    /// Updates the report page definition.
    /// </summary>
    [Parameter] public Func<Action<ReportPageDefinition>, Task> UpdateReportPage { get; set; }

    /// <summary>
    /// Updates the selected band definition.
    /// </summary>
    [Parameter] public Func<Action<ReportSectionDefinition>, Task> UpdateSelectedSection { get; set; }

    /// <summary>
    /// Updates whether the selected band is suppressed.
    /// </summary>
    [Parameter] public Func<bool, Task> UpdateSelectedSectionSuppression { get; set; }

    /// <summary>
    /// Calculates the smallest height that can contain the band elements.
    /// </summary>
    [Parameter] public Func<ReportSectionDefinition, double> GetMinimumSectionHeight { get; set; }

    /// <summary>
    /// Indicates that a band can be inserted before or after the selected band.
    /// </summary>
    [Parameter] public bool CanInsertSection { get; set; }

    /// <summary>
    /// Inserts a band before or after the selected band.
    /// </summary>
    [Parameter] public Func<bool, Task> InsertSection { get; set; }

    /// <summary>
    /// Indicates that a group can be inserted around the selected detail band.
    /// </summary>
    [Parameter] public bool CanInsertGroup { get; set; }

    /// <summary>
    /// Opens the group insertion workflow for the selected detail band.
    /// </summary>
    [Parameter] public Func<Task> InsertGroup { get; set; }

    /// <summary>
    /// Deletes the currently selected band.
    /// </summary>
    [Parameter] public Func<Task> DeleteSelectedSection { get; set; }

    /// <summary>
    /// Updates the selected element definition.
    /// </summary>
    [Parameter] public Func<Action<ReportElementDefinition>, Task> UpdateSelectedElement { get; set; }

    /// <summary>
    /// Moves or resizes the selected element by a delta.
    /// </summary>
    [Parameter] public Func<double, double, double, double, Task> MoveSelectedElement { get; set; }

    #endregion
}