#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise.Reporting.Internal;

internal static class ReportDesignerContextMenuRenderer
{
    #region Methods

    internal static void Render(
        RenderTreeBuilder builder,
        object eventReceiver,
        ReportDefinition definition,
        ReportContextMenuState contextMenu,
        Func<Task> insertBefore,
        Func<Task> insertAfter,
        Func<Task> toggleSuppression,
        Func<Task> deleteSection,
        Func<Task> deleteElement,
        Func<Task> close )
    {
        if ( contextMenu is null || !contextMenu.Visible )
            return;

        builder.OpenElement( "div" );
        builder.Class( "b-report-context-menu" );
        builder.Style( $"left:{contextMenu.ClientX}px;top:{contextMenu.ClientY}px;" );

        switch ( contextMenu.Target )
        {
            case ReportContextMenuTarget.Section when contextMenu.SectionIndex >= 0 && contextMenu.SectionIndex < definition.Sections.Count:
                var section = definition.Sections[contextMenu.SectionIndex];
                if ( !section.Suppressed )
                {
                    RenderButton( builder, eventReceiver, "Insert band before", insertBefore );
                    RenderButton( builder, eventReceiver, "Insert band after", insertAfter );
                    RenderButton( builder, eventReceiver, "Suppress", toggleSuppression );

                    if ( ReportDefinitionHelper.CanDeleteSection( section ) )
                    {
                        RenderButton( builder, eventReceiver, "Delete band", deleteSection );
                    }
                }
                else
                {
                    RenderButton( builder, eventReceiver, "Don't suppress", toggleSuppression );
                }
                break;
            case ReportContextMenuTarget.Element:
                RenderButton( builder, eventReceiver, "Delete element", deleteElement );
                break;
        }

        RenderButton( builder, eventReceiver, "Close", close );

        builder.CloseElement();
    }

    private static void RenderButton( RenderTreeBuilder builder, object eventReceiver, string text, Func<Task> clicked )
    {
        builder.OpenElement( "button" );
        builder.Type( "button" );
        builder.Attribute( "onclick", EventCallback.Factory.Create<MouseEventArgs>( eventReceiver, clicked ) );
        builder.Content( text );
        builder.CloseElement();
    }

    #endregion
}