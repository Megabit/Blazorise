#region Using directives
using System.Globalization;
using System.Threading.Tasks;
using Blazorise.Modules;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise.AntDesign.Components;

public partial class Slider<TValue> : Blazorise.Slider<TValue>
{
    #region Members

    private bool mouseDown = false;

    private DomElement sliderElementInfo;

    private double sliderStart;

    private double sliderWidth;

    /// <summary>
    /// X coordinate of the first mouse down.
    /// </summary>
    private double startedAt;

    #endregion

    #region Methods

    protected override void OnInitialized()
    {
        Converters.TryChangeType<double>( Max, out var max );
        Converters.TryChangeType<double>( Value, out var value );

        Percentage = (int)( value / max * 100 );

        base.OnInitialized();
    }

    protected async Task OnSliderMouseDown( MouseEventArgs e )
    {
        // only the left button can trigger the change
        if ( e.Button != 0 )
            return;

        mouseDown = true;

        sliderElementInfo = await JSUtilitiesModule.GetElementInfo( ElementRef, ElementId );

        sliderStart = sliderElementInfo.BoundingClientRect.Left;
        sliderWidth = sliderElementInfo.BoundingClientRect.Width;

        startedAt = e.ClientX;

        Percentage = CalculatePercentage( e.ClientX );

        await CurrentValueHandler( CalculateValue( Percentage ) );
    }

    protected Task OnSliderMouseMove( MouseEventArgs e )
    {
        if ( !mouseDown )
            return Task.CompletedTask;

        Percentage = CalculatePercentage( e.ClientX );

        return CurrentValueHandler( CalculateValue( Percentage ) );
    }

    protected Task OnSliderMouseUp( MouseEventArgs e )
    {
        // I know this is not ideal. The right way would be to listen to document onmouseup event
        // but for now I will leave it like this.
        // TODO: add document.addEventListener('mouseup') and use instead of element mouseup!
        mouseDown = false;

        return Task.CompletedTask;
    }

    int CalculatePercentage( double clientX )
    {
        var start = clientX - startedAt + ( startedAt - sliderStart );

        var value = start / sliderWidth * 100d;

        if ( value < 0 )
            value = 0;

        if ( value > 100 )
            value = 100;

        return (int)value;
    }

    string CalculateValue( double percentage )
    {
        Converters.TryChangeType<double>( Min, out var min );
        Converters.TryChangeType<double>( Max, out var max );

        var range = max - min;

        var value = range * percentage / 100d;

        value += min;

        if ( value < min )
            value = min;

        if ( value > max )
            value = max;

        return value.ToString( CultureInfo.InvariantCulture );
    }

    #endregion

    #region Properties

    int Percentage { get; set; }

    string TrackStyle
        => $"left: 0%; right: auto; width: {Percentage}%;";

    string HandleStyle
        => $"left: {Percentage}%; right: auto; transform: translateX(-50%);";

    #endregion
}