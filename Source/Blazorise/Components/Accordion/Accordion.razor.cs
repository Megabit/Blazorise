#region Using directives
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// An accordion is a vertically stacked list of headers that reveal or hide associated sections of content.
/// </summary>
public partial class Accordion : BaseComponent
{
    #region Members

    private List<AccordionItem> accordionItems;

    #endregion

    #region Methods

    /// <inheritdoc/>
    public override async Task SetParametersAsync( ParameterView parameters )
    {
        if ( parameters.IsParameterChanged( Animated ) || parameters.IsParameterChanged( AnimationDuration ) )
            DirtyStyles();

        await base.SetParametersAsync( parameters );
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.Accordion() );

        base.BuildClasses( builder );
    }

    /// <inheritdoc/>
    protected override void BuildStyles( StyleBuilder builder )
    {
        builder.Append( StyleProvider.AccordionAnimationDuration( EffectiveAnimationDuration ) );

        base.BuildStyles( builder );
    }

    /// <summary>
    /// Notifies the <see cref="Accordion"/> that the specified <see cref="AccordionItem"/> has been initialized.
    /// </summary>
    /// <param name="accordionItem">The <see cref="AccordionItem"/> that has been initialized.</param>
    internal void NotifyAccordionItemInitialized( AccordionItem accordionItem )
    {
        accordionItems ??= new();

        accordionItems.Add( accordionItem );
    }

    /// <summary>
    /// Notifies the <see cref="Accordion"/> that the specified <see cref="AccordionItem"/> has been removed.
    /// </summary>
    /// <param name="accordionItem">The <see cref="AccordionItem"/> that has been removed.</param>
    internal void NotifyAccordionItemRemoved( AccordionItem accordionItem )
    {
        if ( accordionItems is not null && accordionItems.Contains( accordionItem ) )
            accordionItems.Remove( accordionItem );
    }

    /// <summary>
    /// Determines if the specified <see cref="AccordionItem"/> is the first item in the <see cref="Accordion"/>.
    /// </summary>
    /// <param name="accordionItem">The <see cref="AccordionItem"/> to check.</param>
    /// <returns>
    /// A <see cref="bool"/> value indicating if the specified <see cref="AccordionItem"/> is the first item in the <see cref="Accordion"/>.
    /// </returns>
    internal bool IsFirstInAccordion( AccordionItem accordionItem )
    {
        if ( accordionItems is not null && accordionItems.IndexOf( accordionItem ) == 0 )
            return true;

        return false;
    }

    /// <summary>
    /// Determines if the specified <see cref="AccordionItem"/> is the last item in the <see cref="Accordion"/>.
    /// </summary>
    /// <param name="accordionItem">The <see cref="AccordionItem"/> to check.</param>
    /// <returns>
    /// A <see cref="bool"/> value indicating if the specified <see cref="AccordionItem"/> is the last item in the <see cref="Accordion"/>.
    /// </returns>
    internal bool IsLastInAccordion( AccordionItem accordionItem )
    {
        if ( accordionItems is not null && accordionItems.IndexOf( accordionItem ) == accordionItems.Count - 1 )
            return true;

        return false;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the duration override, or null to retain the provider's default timing.
    /// </summary>
    protected int? EffectiveAnimationDuration => !Animated ? 0 : AnimationDuration.HasValue ? Math.Max( 0, AnimationDuration.Value ) : null;

    /// <summary>
    /// Gets the animation duration serialized for markup.
    /// </summary>
    protected string AnimationDurationString => EffectiveAnimationDuration?.ToString( CultureInfo.InvariantCulture );

    /// <summary>
    /// Enables the transitions supplied by the CSS provider. Set to false for immediate changes.
    /// </summary>
    [Parameter] public bool Animated { get; set; } = true;

    /// <summary>
    /// Overrides the provider's animation duration, in milliseconds. Null preserves the provider's default.
    /// Zero or a negative value disables transitions.
    /// </summary>
    [Parameter] public int? AnimationDuration { get; set; }

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="Accordion"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}