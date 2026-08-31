#region Using directives
using System;
using System.Collections.Generic;
using Blazorise.Charts.Svg;
using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
#endregion

namespace Blazorise.Tests.Components;

public class SvgChartAnimationComponentTest : BunitContext
{
    public SvgChartAnimationComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider();
        JSInterop.AddBlazoriseUtilities();

        var module = JSInterop.SetupModule( "./_content/Blazorise.Charts.Svg/svgChart.js" );
        module.SetupVoid( "runAnimations", _ => true ).SetVoidResult();
        module.SetupVoid( "destroyAnimations", _ => true ).SetVoidResult();
    }

    [Fact]
    public void DeclarativeAnimation_AnimatesInitialRender()
    {
        List<RevenueSample> items =
        [
            new() { Month = "Jan", Revenue = 68 },
            new() { Month = "Feb", Revenue = 74 },
            new() { Month = "Mar", Revenue = 91 }
        ];
        IRenderedComponent<SvgColumnChart<RevenueSample>> component = Render<SvgColumnChart<RevenueSample>>( parameters => parameters
            .Add( chart => chart.Items, items )
            .AddChildContent( builder =>
            {
                builder.OpenComponent<SvgChartAnimation>( 0 );
                builder.AddAttribute( 1, nameof( SvgChartAnimation.Enabled ), true );
                builder.AddAttribute( 2, nameof( SvgChartAnimation.Duration ), TimeSpan.FromMilliseconds( 500 ) );
                builder.CloseComponent();

                builder.OpenComponent<SvgChartCategoryAxis<RevenueSample>>( 3 );
                builder.AddAttribute( 4, nameof( SvgChartCategoryAxis<RevenueSample>.Value ), (Func<RevenueSample, object>)( item => item.Month ) );
                builder.CloseComponent();

                builder.OpenComponent<SvgChartValueAxis>( 5 );
                builder.AddAttribute( 6, nameof( SvgChartValueAxis.BeginAtZero ), true );
                builder.CloseComponent();

                builder.OpenComponent<SvgColumnSeries<RevenueSample>>( 7 );
                builder.AddAttribute( 8, nameof( SvgColumnSeries<RevenueSample>.Name ), "Revenue" );
                builder.AddAttribute( 9, nameof( SvgColumnSeries<RevenueSample>.Value ), (Func<RevenueSample, double?>)( item => item.Revenue ) );
                builder.CloseComponent();
            } ) );

        component.WaitForAssertion( () =>
            Assert.NotEmpty( component.FindAll( "[data-svg-chart-animation-initial='true']" ) ) );
    }

    private sealed class RevenueSample
    {
        public string Month { get; set; }

        public double Revenue { get; set; }
    }
}