#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Localization;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.DataGrid.Internal;

/// <summary>
/// Supports data grid modal behavior in DataGrid components.
/// </summary>
public partial class _DataGridModal<TItem> : BaseAfterRenderComponent, IDisposable
{
    #region Members

    /// <summary>
    /// Reference to the modal that hosts the edit form.
    /// </summary>
    protected Modal modalRef;

    /// <summary>
    /// Creates the edit model used by the data grid modal.
    /// </summary>
    protected EventCallbackFactory callbackFactory = new();

    /// <summary>
    /// Validation component for the modal edit form.
    /// </summary>
    protected Validations validations;

    /// <summary>
    /// Controls popup visible behavior for the data grid modal.
    /// </summary>
    protected bool popupVisible;

    /// <summary>
    /// Indicates whether the data grid modal is invalid.
    /// </summary>
    protected bool isInvalid;

    /// <summary>
    /// Callback invoked for member.
    /// </summary>
    protected EventCallback Cancel
        => EventCallback.Factory.Create( this, ParentDataGrid.CancelInternal );

    #endregion

    #region Methods

    /// <inheritdoc/>
    public override async Task SetParametersAsync( ParameterView parameters )
    {
        if ( parameters.TryGetValue( nameof( PopupVisible ), out bool popupVisibleParam ) && PopupVisible != popupVisibleParam && popupVisibleParam )
        {
            await OpenModal();
        }

        await base.SetParametersAsync( parameters );
    }

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        LocalizerService.LocalizationChanged += OnLocalizationChanged;

        base.OnInitialized();
    }

    /// <inheritdoc/>
    protected override void Dispose( bool disposing )
    {
        if ( disposing )
        {
            LocalizerService.LocalizationChanged -= OnLocalizationChanged;
        }

        base.Dispose( disposing );
    }

    private async void OnLocalizationChanged( object sender, EventArgs e )
    {
        await InvokeAsync( StateHasChanged );
    }

    /// <summary>
    /// Responds to validation status changes in the edit form.
    /// </summary>
    protected void ValidationsStatusChanged( ValidationsStatusChangedEventArgs args )
    {
        isInvalid = args.Status == ValidationStatus.Error;

        InvokeAsync( StateHasChanged );
    }

    /// <summary>
    /// Validates every field in the modal edit form.
    /// </summary>
    internal protected Task<bool> ValidateAll()
    {
        return validations.ValidateAll();
    }

    /// <summary>
    /// Submits the edited item and closes the modal after a successful save.
    /// </summary>
    internal protected async Task Save()
    {
        await ParentDataGrid.SaveInternal();

        if ( ParentDataGrid.EditState == DataGridEditState.None )
            await CloseModal();
    }

    /// <summary>
    /// Controls member behavior for the data grid modal.
    /// </summary>
    protected bool CanSaveFromOnScreenKeyboard
        => ( ParentDataGrid.CommandColumn is null || ParentDataGrid.CommandColumn.SaveCommandAllowed )
            && ParentDataGrid.SubmitFormOnEnter;

    /// <summary>
    /// Opens the DataGrid edit modal.
    /// </summary>
    protected async Task OpenModal()
    {
        if ( validations != null )
            await validations.ClearAll();

        ExecuteAfterRender( () => modalRef.Show() );
    }

    /// <summary>
    /// Closes the DataGrid edit modal.
    /// </summary>
    protected Task CloseModal()
        => modalRef.Hide();

    #endregion

    #region Properties

    /// <summary>
    /// Supplies localized text for modal actions.
    /// </summary>
    [Inject] protected ITextLocalizerService LocalizerService { get; set; }

    /// <summary>
    /// Supplies translated labels for modal editing controls.
    /// </summary>
    [Inject] protected ITextLocalizer<DataGrid<TItem>> Localizer { get; set; }

    /// <summary>
    /// Identifies the item being edited in the modal.
    /// </summary>
    [Parameter] public TItem EditItem { get; set; }

    /// <summary>
    /// Holds the model used to evaluate validation rules.
    /// </summary>
    [Parameter] public TItem ValidationItem { get; set; }

    /// <summary>
    /// Customizes the title rendered above the edit form.
    /// </summary>
    [Parameter] public RenderFragment<PopupTitleContext<TItem>> TitleTemplate { get; set; }

    /// <summary>
    /// Provides the columns available to the modal editor.
    /// </summary>
    [Parameter] public IEnumerable<DataGridColumn<TItem>> Columns { get; set; }

    /// <summary>
    /// Filters editable columns and arranges them by edit order.
    /// </summary>
    protected IEnumerable<DataGridColumn<TItem>> OrderedEditableColumns
    {
        get
        {
            return Columns
                .Where( column => !column.ExcludeFromEdit && column.CellValueIsEditable )
                .OrderBy( column => column.EditOrder ?? column.DisplayOrder );
        }
    }

    /// <summary>
    /// Editable cell values keyed by column field name.
    /// </summary>
    [Parameter] public IReadOnlyDictionary<string, CellEditContext<TItem>> EditItemCellValues { get; set; }

    /// <summary>
    /// Indicates whether the edit modal should be open.
    /// </summary>
    [Parameter] public bool PopupVisible { get; set; }

    /// <summary>
    /// Selects the size used by the edit modal.
    /// </summary>
    [Parameter] public ModalSize PopupSize { get; set; }

    /// <summary>
    /// Handles attempts to close the edit modal.
    /// </summary>
    [Parameter] public Func<ModalClosingEventArgs, Task> PopupClosing { get; set; }

    /// <summary>
    /// Tracks the current modal editing operation.
    /// </summary>
    [Parameter] public DataGridEditState EditState { get; set; }

    /// <summary>
    /// Specifies the parent <see cref="DataGrid{TItem}"/> of the this component.
    /// </summary>
    [CascadingParameter] public DataGrid<TItem> ParentDataGrid { get; set; }

    #endregion
}