#region Using directives
using System;
using System.Globalization;
using System.Threading.Tasks;
using Blazorise.Gantt.Extensions;
using Blazorise.Localization;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Gantt.Components;

/// <summary>
/// Internal toolbar renderer for <see cref="Gantt{TItem}"/>.
/// </summary>
/// <typeparam name="TItem">The item type.</typeparam>
public partial class _GanttToolbar<TItem> : BaseComponent, IDisposable
{
    #region Members

    private bool dropdownColumnChooserVisible;

    #endregion

    #region Methods

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        LocalizerService.LocalizationChanged += OnLocalizationChanged;

        base.OnInitialized();
    }

    /// <inheritdoc />
    protected override void Dispose( bool disposing )
    {
        if ( disposing )
        {
            LocalizerService.LocalizationChanged -= OnLocalizationChanged;
        }
    }

    /// <inheritdoc />
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( "b-gantt-toolbar" );

        base.BuildClasses( builder );
    }

    private async void OnLocalizationChanged( object sender, EventArgs e )
    {
        await InvokeAsync( StateHasChanged );
    }

    private Task OnPreviousClick() => PreviousClicked.InvokeAsync();

    private Task OnNextClick() => NextClicked.InvokeAsync();

    private Task OnTodayClick() => TodayClicked.InvokeAsync();

    private Task OnDayViewClick() => DayViewClicked.InvokeAsync();

    private Task OnWeekViewClick() => WeekViewClicked.InvokeAsync();

    private Task OnMonthViewClick() => MonthViewClicked.InvokeAsync();

    private Task OnYearViewClick() => YearViewClicked.InvokeAsync();

    private Task OnAddTaskClick() => AddTaskClicked.InvokeAsync();

    private Task OnSearchTextValueChanged( string value ) => SearchTextChanged.InvokeAsync( value );

    private Task OnShowTitleColumnChanged( bool value ) => ShowTitleColumnChanged.InvokeAsync( value );

    private Task OnShowStartColumnChanged( bool value ) => ShowStartColumnChanged.InvokeAsync( value );

    private Task OnShowEndColumnChanged( bool value ) => ShowEndColumnChanged.InvokeAsync( value );

    #endregion

    #region Properties

    private string RangeText
    {
        get
        {
            if ( SelectedView == GanttView.Week )
            {
                var start = SelectedDate.StartOfWeek( FirstDayOfWeek );
                var end = start.AddDays( 6 );

                return $"{start.ToString( "MMM dd", CultureInfo.InvariantCulture )} - {end.ToString( "MMM dd, yyyy", CultureInfo.InvariantCulture )}";
            }

            if ( SelectedView == GanttView.Month )
            {
                return SelectedDate.ToString( "MMMM yyyy", CultureInfo.InvariantCulture );
            }

            if ( SelectedView == GanttView.Year )
            {
                return SelectedDate.ToString( "yyyy", CultureInfo.InvariantCulture );
            }

            return SelectedDate.ToString( "MMMM dd, yyyy", CultureInfo.InvariantCulture );
        }
    }

    [Inject] protected ITextLocalizer<Gantt<TItem>> Localizer { get; set; }

    [Inject] protected ITextLocalizerService LocalizerService { get; set; }

    [CascadingParameter] public Gantt<TItem> Gantt { get; set; }

    [Parameter] public DateOnly SelectedDate { get; set; }

    [Parameter] public GanttView SelectedView { get; set; }

    [Parameter] public DayOfWeek FirstDayOfWeek { get; set; }

    [Parameter] public string SearchText { get; set; }

    [Parameter] public bool ShowTitleColumn { get; set; }

    [Parameter] public bool ShowStartColumn { get; set; }

    [Parameter] public bool ShowEndColumn { get; set; }

    [Parameter] public bool ShowDayViewButton { get; set; }

    [Parameter] public bool ShowWeekViewButton { get; set; }

    [Parameter] public bool ShowMonthViewButton { get; set; }

    [Parameter] public bool ShowYearViewButton { get; set; }

    [Parameter] public EventCallback PreviousClicked { get; set; }

    [Parameter] public EventCallback NextClicked { get; set; }

    [Parameter] public EventCallback TodayClicked { get; set; }

    [Parameter] public EventCallback DayViewClicked { get; set; }

    [Parameter] public EventCallback WeekViewClicked { get; set; }

    [Parameter] public EventCallback MonthViewClicked { get; set; }

    [Parameter] public EventCallback YearViewClicked { get; set; }

    [Parameter] public EventCallback AddTaskClicked { get; set; }

    [Parameter] public EventCallback<string> SearchTextChanged { get; set; }

    [Parameter] public EventCallback<bool> ShowTitleColumnChanged { get; set; }

    [Parameter] public EventCallback<bool> ShowStartColumnChanged { get; set; }

    [Parameter] public EventCallback<bool> ShowEndColumnChanged { get; set; }

    #endregion
}