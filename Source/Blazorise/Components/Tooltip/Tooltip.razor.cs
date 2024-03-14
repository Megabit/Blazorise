#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Modules;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Tooltips display informative text when users hover over, focus on, or tap an element.
/// </summary>
public partial class Tooltip : BaseComponent, IAsyncDisposable
{
    #region Members

    private TooltipPlacement placement = TooltipPlacement.Top;

    private bool multiline;

    private bool alwaysActive;

    private bool showArrow = true;

    private bool inline;

    private bool fade;

    private int fadeDuration = 300;

    private TooltipTrigger trigger = TooltipTrigger.MouseEnterFocus;

    private bool autodetectInline;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.Tooltip() );
        builder.Append( ClassProvider.TooltipPlacement( Placement ) );
        builder.Append( ClassProvider.TooltipMultiline(), Multiline );
        builder.Append( ClassProvider.TooltipAlwaysActive(), AlwaysActive );
        builder.Append( ClassProvider.TooltipInline(), Inline );
        builder.Append( ClassProvider.TooltipFade(), Fade );

        base.BuildClasses( builder );
    }

    /// <inheritdoc/>
    public override Task SetParametersAsync( ParameterView parameters )
    {
        if ( parameters.TryGetValue<string>( nameof( Text ), out var paramText ) && Text != paramText )
        {
            ExecuteAfterRender( async () => await JSModule.UpdateContent( ElementRef, ElementId, paramText ) );
        }

        // autodetect inline mode only if Inline parameter is not explicitly defined
        autodetectInline = !parameters.TryGetValue<bool>( nameof( Inline ), out var _ );

        return base.SetParametersAsync( parameters );
    }

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        // try to detect if inline is needed
        ExecuteAfterRender( async () =>
        {
            await JSModule.Initialize( ElementRef, ElementId, new
            {
                Text,
                Placement = ClassProvider.ToTooltipPlacement( Placement ),
                Multiline,
                AlwaysActive,
                ShowArrow,
                Fade,
                FadeDuration,
                Trigger = ToTippyTrigger( Trigger ),
                TriggerTargetId,
                MaxWidth = Theme?.TooltipOptions?.MaxWidth,
                AutodetectInline = autodetectInline,
                ZIndex,
                Interactive,
                AppendTo,
            } );
        } );

        base.OnInitialized();
    }

    /// <inheritdoc/>
    protected override async ValueTask DisposeAsync( bool disposing )
    {
        if ( disposing && Rendered )
        {
            await JSModule.SafeDestroy( ElementRef, ElementId );
        }

        await base.DisposeAsync( disposing );
    }

    private static string ToTippyTrigger( TooltipTrigger trigger )
    {
        return trigger switch
        {
            TooltipTrigger.Click => "click",
            TooltipTrigger.Focus => "focusin",
            TooltipTrigger.MouseEnterClick => "mouseenter click",
            _ => "mouseenter focus",
        };
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    protected override bool ShouldAutoGenerateId => true;

    /// <summary>
    /// Gets or sets the <see cref="IJSTooltipModule"/> instance.
    /// </summary>
    [Inject] public IJSTooltipModule JSModule { get; set; }

    /// <summary>
    /// Gets or sets a regular tooltip's content. 
    /// </summary>
    [Parameter] public string Text { get; set; }

    /// <summary>
    /// Gets or sets the tooltip location relative to its component.
    /// </summary>
    [Parameter]
    public TooltipPlacement Placement
    {
        get => placement;
        set
        {
            placement = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Force the multiline display.
    /// </summary>
    [Parameter]
    public bool Multiline
    {
        get => multiline;
        set
        {
            multiline = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Always show tooltip, instead of just when hovering over the element.
    /// </summary>
    [Parameter]
    public bool AlwaysActive
    {
        get => alwaysActive;
        set
        {
            alwaysActive = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Gets or sets the tooltip arrow visibility.
    /// </summary>
    [Parameter]
    public bool ShowArrow
    {
        get => showArrow;
        set
        {
            showArrow = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Force inline block instead of trying to detect the element block.
    /// </summary>
    [Parameter]
    public bool Inline
    {
        get => inline;
        set
        {
            inline = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Makes the tooltip fade transition.
    /// </summary>
    [Parameter]
    public bool Fade
    {
        get => fade;
        set
        {
            fade = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Duration in ms of the fade transition animation.
    /// </summary>
    [Parameter]
    public int FadeDuration
    {
        get => fadeDuration;
        set
        {
            fadeDuration = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Determines the events that cause the tooltip to show.
    /// </summary>
    [Parameter]
    public TooltipTrigger Trigger
    {
        get => trigger;
        set
        {
            trigger = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Which element the trigger event listeners are applied to (instead of the reference element).
    /// </summary>
    [Parameter] public string TriggerTargetId { get; set; }

    /// <summary>
    /// Specifies the z-index CSS on the root popper node.
    /// </summary>
    [Parameter] public int? ZIndex { get; set; } = 9999;

    /// <summary>
    /// Determines if the tooltip has interactive content inside of it, so that it can be hovered over and clicked inside without hiding.
    /// </summary>
    [Parameter] public bool Interactive { get; set; }

    /// <summary>
    /// The element to append the tooltip to. If <see cref="Interactive"/> = true, the default behavior is appendTo: "parent".
    /// </summary>
    [Parameter] public string AppendTo { get; set; }

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="Tooltip"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Cascaded theme settings.
    /// </summary>
    [CascadingParameter] public Theme Theme { get; set; }

    #endregion
}