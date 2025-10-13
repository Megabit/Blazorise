#region Using directives
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise.Extensions;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
public static class RenderTreeBuilderExtensions
{
    public static RenderTreeBuilder OpenElement( this RenderTreeBuilder builder, string name, [CallerLineNumber] int line = 0 )
    {
        builder.OpenElement( GetSequence( line ), name );

        return builder;
    }

    public static RenderTreeBuilder OpenComponent( this RenderTreeBuilder builder, Type componentType, [CallerLineNumber] int line = 0 )
    {
        builder.OpenComponent( GetSequence( line ), componentType );

        return builder;
    }

    public static RenderTreeBuilder OpenComponent<TComponent>( this RenderTreeBuilder builder, [CallerLineNumber] int line = 0 )
        where TComponent : IComponent
    {
        builder.OpenComponent<TComponent>( GetSequence( line ) );

        return builder;
    }

    public static RenderTreeBuilder Id( this RenderTreeBuilder builder, object value, [CallerLineNumber] int line = 0 )
    {
        builder.AddAttribute( GetSequence( line ), "id", value );

        return builder;
    }

    public static RenderTreeBuilder Key( this RenderTreeBuilder builder, object value )
    {
        builder.SetKey( value );

        return builder;
    }

    public static RenderTreeBuilder Type( this RenderTreeBuilder builder, string value, [CallerLineNumber] int line = 0 )
    {
        builder.AddAttribute( GetSequence( line ), "type", value );

        return builder;
    }

    public static RenderTreeBuilder Role( this RenderTreeBuilder builder, string value, [CallerLineNumber] int line = 0 )
    {
        builder.AddAttribute( GetSequence( line ), "role", value );

        return builder;
    }

    public static RenderTreeBuilder Class( this RenderTreeBuilder builder, string value, [CallerLineNumber] int line = 0 )
    {
        if ( !string.IsNullOrEmpty( value ) )
        {
            builder.AddAttribute( GetSequence( line ), "class", value );
        }

        return builder;
    }

    public static RenderTreeBuilder Style( this RenderTreeBuilder builder, string value, [CallerLineNumber] int line = 0 )
    {
        if ( !string.IsNullOrEmpty( value ) )
        {
            builder.AddAttribute( GetSequence( line ), "style", value );
        }

        return builder;
    }

    public static RenderTreeBuilder Href( this RenderTreeBuilder builder, string value, [CallerLineNumber] int line = 0 )
    {
        builder.AddAttribute( GetSequence( line ), "href", value );

        return builder;
    }

    public static RenderTreeBuilder Width( this RenderTreeBuilder builder, string value, [CallerLineNumber] int line = 0 )
    {
        builder.AddAttribute( GetSequence( line ), "width", value );

        return builder;
    }

    public static RenderTreeBuilder Height( this RenderTreeBuilder builder, string value, [CallerLineNumber] int line = 0 )
    {
        builder.AddAttribute( GetSequence( line ), "height", value );

        return builder;
    }

    public static RenderTreeBuilder Fill( this RenderTreeBuilder builder, string value, [CallerLineNumber] int line = 0 )
    {
        builder.AddAttribute( GetSequence( line ), "fill", value );

        return builder;
    }

    public static RenderTreeBuilder Target( this RenderTreeBuilder builder, Target target, [CallerLineNumber] int line = 0 )
    {
        if ( target != Blazorise.Target.Default )
        {
            builder.AddAttribute( GetSequence( line ), "target", target.ToTargetString() );
        }

        return builder;
    }

    public static RenderTreeBuilder Disabled( this RenderTreeBuilder builder, bool value, [CallerLineNumber] int line = 0 )
    {
        if ( value )
            builder.AddAttribute( GetSequence( line ), "disabled", true );

        return builder;
    }

    public static RenderTreeBuilder Readonly( this RenderTreeBuilder builder, bool value, [CallerLineNumber] int line = 0 )
    {
        if ( value )
            builder.AddAttribute( GetSequence( line ), "readonly", "true" );

        return builder;
    }

    public static RenderTreeBuilder Aria( this RenderTreeBuilder builder, string name, object value, [CallerLineNumber] int line = 0 )
    {
        builder.AddAttribute( GetSequence( line ), $"aria-{name}", value );

        return builder;
    }

    public static RenderTreeBuilder TabIndex( this RenderTreeBuilder builder, int? value, [CallerLineNumber] int line = 0 )
    {
        builder.AddAttribute( GetSequence( line ), "tabindex", value );

        return builder;
    }

    public static RenderTreeBuilder Draggable( this RenderTreeBuilder builder, object value, [CallerLineNumber] int line = 0 )
    {
        builder.AddAttribute( GetSequence( line ), "draggable", value );

        return builder;
    }

    public static RenderTreeBuilder Scope( this RenderTreeBuilder builder, string value, [CallerLineNumber] int line = 0 )
    {
        builder.AddAttribute( GetSequence( line ), "scope", value );

        return builder;
    }

    public static RenderTreeBuilder ColSpan( this RenderTreeBuilder builder, int? value, [CallerLineNumber] int line = 0 )
    {
        builder.AddAttribute( GetSequence( line ), "colspan", value );

        return builder;
    }

    public static RenderTreeBuilder RowSpan( this RenderTreeBuilder builder, int? value, [CallerLineNumber] int line = 0 )
    {
        builder.AddAttribute( GetSequence( line ), "rowspan", value );

        return builder;
    }

    public static RenderTreeBuilder AriaPressed( this RenderTreeBuilder builder, object value, [CallerLineNumber] int line = 0 )
    {
        return Aria( builder, "pressed", value, line );
    }

    public static RenderTreeBuilder AriaHidden( this RenderTreeBuilder builder, object value, [CallerLineNumber] int line = 0 )
    {
        return Aria( builder, "hidden", value, line );
    }

    public static RenderTreeBuilder AriaLabel( this RenderTreeBuilder builder, object value, [CallerLineNumber] int line = 0 )
    {
        return Aria( builder, "label", value, line );
    }

    public static RenderTreeBuilder AriaDisabled( this RenderTreeBuilder builder, object value, [CallerLineNumber] int line = 0 )
    {
        return Aria( builder, "disabled", value, line );
    }

    public static RenderTreeBuilder AriaExpanded( this RenderTreeBuilder builder, object value, [CallerLineNumber] int line = 0 )
    {
        return Aria( builder, "expanded", value, line );
    }

    public static RenderTreeBuilder Data( this RenderTreeBuilder builder, string name, object value, [CallerLineNumber] int line = 0 )
    {
        builder.AddAttribute( GetSequence( line ), $"data-{name}", value );

        return builder;
    }

    public static RenderTreeBuilder Attribute( this RenderTreeBuilder builder, string name, object value, [CallerLineNumber] int line = 0 )
    {
        builder.AddAttribute( GetSequence( line ), name, value );

        return builder;
    }

    public static RenderTreeBuilder Attribute( this RenderTreeBuilder builder, string name, MulticastDelegate value, [CallerLineNumber] int line = 0 )
    {
        builder.AddAttribute( GetSequence( line ), name, value );

        return builder;
    }

    public static RenderTreeBuilder Attribute( this RenderTreeBuilder builder, string name, bool value, [CallerLineNumber] int line = 0 )
    {
        builder.AddAttribute( GetSequence( line ), name, value );

        return builder;
    }

    public static RenderTreeBuilder Attribute( this RenderTreeBuilder builder, string name, string value, [CallerLineNumber] int line = 0 )
    {
        builder.AddAttribute( GetSequence( line ), name, value );

        return builder;
    }

    public static RenderTreeBuilder Attribute( this RenderTreeBuilder builder, string name, EventCallback value, [CallerLineNumber] int line = 0 )
    {
        builder.AddAttribute( GetSequence( line ), name, value );

        return builder;
    }

    public static RenderTreeBuilder Attribute<TArgument>( this RenderTreeBuilder builder, string name, EventCallback<TArgument> value, [CallerLineNumber] int line = 0 )
    {
        builder.AddAttribute( GetSequence( line ), name, value );

        return builder;
    }

    public static RenderTreeBuilder OnClick<T>( this RenderTreeBuilder builder, object receiver, EventCallback<T> callback, [CallerLineNumber] int line = 0 )
    {
        builder.AddAttribute( GetSequence( line ), "onclick", EventCallback.Factory.Create<T>( receiver, callback ) );

        return builder;
    }

    public static RenderTreeBuilder OnClickPreventDefault( this RenderTreeBuilder builder, bool preventDefault, [CallerLineNumber] int line = 0 )
    {
        builder.AddEventPreventDefaultAttribute( GetSequence( line ), "onclick", preventDefault );

        return builder;
    }

    public static RenderTreeBuilder OnClickStopPropagation( this RenderTreeBuilder builder, bool value, [CallerLineNumber] int line = 0 )
    {
        builder.AddEventStopPropagationAttribute( GetSequence( line ), "onclick", value );

        return builder;
    }

    public static RenderTreeBuilder OnMouseEnter<T>( this RenderTreeBuilder builder, object receiver, EventCallback<T> callback, [CallerLineNumber] int line = 0 )
    {
        builder.AddAttribute( GetSequence( line ), "onmouseenter", EventCallback.Factory.Create<T>( receiver, callback ) );

        return builder;
    }

    public static RenderTreeBuilder OnMouseLeave<T>( this RenderTreeBuilder builder, object receiver, EventCallback<T> callback, [CallerLineNumber] int line = 0 )
    {
        builder.AddAttribute( GetSequence( line ), "onmouseleave", EventCallback.Factory.Create<T>( receiver, callback ) );

        return builder;
    }

    public static RenderTreeBuilder OnMouseDown<T>( this RenderTreeBuilder builder, object receiver, EventCallback<T> callback, [CallerLineNumber] int line = 0 )
    {
        builder.AddAttribute( GetSequence( line ), "onmousedown", EventCallback.Factory.Create<T>( receiver, callback ) );

        return builder;
    }

    public static RenderTreeBuilder OnMouseMove<T>( this RenderTreeBuilder builder, object receiver, EventCallback<T> callback, [CallerLineNumber] int line = 0 )
    {
        builder.AddAttribute( GetSequence( line ), "onmousemove", EventCallback.Factory.Create<T>( receiver, callback ) );

        return builder;
    }

    public static RenderTreeBuilder OnMouseUp<T>( this RenderTreeBuilder builder, object receiver, EventCallback<T> callback, [CallerLineNumber] int line = 0 )
    {
        builder.AddAttribute( GetSequence( line ), "onmouseup", EventCallback.Factory.Create<T>( receiver, callback ) );

        return builder;
    }

    public static RenderTreeBuilder OnMouseOver<T>( this RenderTreeBuilder builder, object receiver, EventCallback<T> callback, [CallerLineNumber] int line = 0 )
    {
        builder.AddAttribute( GetSequence( line ), "onmouseover", EventCallback.Factory.Create<T>( receiver, callback ) );

        return builder;
    }

    public static RenderTreeBuilder OnMouseOverStopPropagation( this RenderTreeBuilder builder, bool value, [CallerLineNumber] int line = 0 )
    {
        builder.AddEventStopPropagationAttribute( GetSequence( line ), "onmouseover", value );

        return builder;
    }

    public static RenderTreeBuilder OnMouseOverPreventDefault( this RenderTreeBuilder builder, bool value, [CallerLineNumber] int line = 0 )
    {
        builder.AddEventPreventDefaultAttribute( GetSequence( line ), "onmouseover", value );

        return builder;
    }

    public static RenderTreeBuilder OnMouseLeaveStopPropagation( this RenderTreeBuilder builder, bool value, [CallerLineNumber] int line = 0 )
    {
        builder.AddEventStopPropagationAttribute( GetSequence( line ), "onmouseleave", value );

        return builder;
    }

    public static RenderTreeBuilder OnMouseLeavePreventDefault( this RenderTreeBuilder builder, bool value, [CallerLineNumber] int line = 0 )
    {
        builder.AddEventPreventDefaultAttribute( GetSequence( line ), "onmouseleave", value );

        return builder;
    }

    public static RenderTreeBuilder OnDragStartStopPropagation( this RenderTreeBuilder builder, bool value, [CallerLineNumber] int line = 0 )
    {
        builder.AddEventStopPropagationAttribute( GetSequence( line ), "ondragstart", value );

        return builder;
    }

    public static RenderTreeBuilder OnDragStartPreventDefault( this RenderTreeBuilder builder, bool value, [CallerLineNumber] int line = 0 )
    {
        builder.AddEventPreventDefaultAttribute( GetSequence( line ), "ondragstart", value );

        return builder;
    }

    public static RenderTreeBuilder OnDragEnterStopPropagation( this RenderTreeBuilder builder, bool value, [CallerLineNumber] int line = 0 )
    {
        builder.AddEventStopPropagationAttribute( GetSequence( line ), "ondragenter", value );

        return builder;
    }

    public static RenderTreeBuilder OnDragEnterPreventDefault( this RenderTreeBuilder builder, bool value, [CallerLineNumber] int line = 0 )
    {
        builder.AddEventPreventDefaultAttribute( GetSequence( line ), "ondragenter", value );

        return builder;
    }

    public static RenderTreeBuilder OnDragEndStopPropagation( this RenderTreeBuilder builder, bool value, [CallerLineNumber] int line = 0 )
    {
        builder.AddEventStopPropagationAttribute( GetSequence( line ), "ondragend", value );

        return builder;
    }

    public static RenderTreeBuilder OnDragEndPreventDefault( this RenderTreeBuilder builder, bool value, [CallerLineNumber] int line = 0 )
    {
        builder.AddEventPreventDefaultAttribute( GetSequence( line ), "ondragend", value );

        return builder;
    }

    public static RenderTreeBuilder OnDragLeaveStopPropagation( this RenderTreeBuilder builder, bool value, [CallerLineNumber] int line = 0 )
    {
        builder.AddEventStopPropagationAttribute( GetSequence( line ), "ondragleave", value );

        return builder;
    }

    public static RenderTreeBuilder OnDragLeavePreventDefault( this RenderTreeBuilder builder, bool value, [CallerLineNumber] int line = 0 )
    {
        builder.AddEventPreventDefaultAttribute( GetSequence( line ), "ondragleave", value );

        return builder;
    }

    public static RenderTreeBuilder OnDropStopPropagation( this RenderTreeBuilder builder, bool value, [CallerLineNumber] int line = 0 )
    {
        builder.AddEventStopPropagationAttribute( GetSequence( line ), "ondrop", value );

        return builder;
    }

    public static RenderTreeBuilder OnDropPreventDefault( this RenderTreeBuilder builder, bool value, [CallerLineNumber] int line = 0 )
    {
        builder.AddEventPreventDefaultAttribute( GetSequence( line ), "ondrop", value );

        return builder;
    }

    public static RenderTreeBuilder OnContextMenuStopPropagation( this RenderTreeBuilder builder, bool value, [CallerLineNumber] int line = 0 )
    {
        builder.AddEventStopPropagationAttribute( GetSequence( line ), "oncontextmenu", value );

        return builder;
    }

    public static RenderTreeBuilder OnContextMenuPreventDefault( this RenderTreeBuilder builder, bool value, [CallerLineNumber] int line = 0 )
    {
        builder.AddEventPreventDefaultAttribute( GetSequence( line ), "oncontextmenu", value );

        return builder;
    }

    public static RenderTreeBuilder OnDragEnter<T>( this RenderTreeBuilder builder, object receiver, EventCallback<T> callback, [CallerLineNumber] int line = 0 )
    {
        builder.AddAttribute( GetSequence( line ), "ondragenter", EventCallback.Factory.Create<T>( receiver, callback ) );

        return builder;
    }

    public static RenderTreeBuilder OnDragStart<T>( this RenderTreeBuilder builder, object receiver, EventCallback<T> callback, [CallerLineNumber] int line = 0 )
    {
        builder.AddAttribute( GetSequence( line ), "ondragstart", EventCallback.Factory.Create<T>( receiver, callback ) );

        return builder;
    }

    public static RenderTreeBuilder OnDragEnd<T>( this RenderTreeBuilder builder, object receiver, EventCallback<T> callback, [CallerLineNumber] int line = 0 )
    {
        builder.AddAttribute( GetSequence( line ), "ondragend", EventCallback.Factory.Create<T>( receiver, callback ) );

        return builder;
    }

    public static RenderTreeBuilder OnDragLeave<T>( this RenderTreeBuilder builder, object receiver, EventCallback<T> callback, [CallerLineNumber] int line = 0 )
    {
        builder.AddAttribute( GetSequence( line ), "ondragleave", EventCallback.Factory.Create<T>( receiver, callback ) );

        return builder;
    }

    public static RenderTreeBuilder OnDrop<T>( this RenderTreeBuilder builder, object receiver, EventCallback<T> callback, [CallerLineNumber] int line = 0 )
    {
        builder.AddAttribute( GetSequence( line ), "ondrop", EventCallback.Factory.Create<T>( receiver, callback ) );

        return builder;
    }

    public static RenderTreeBuilder OnContextMenu<T>( this RenderTreeBuilder builder, object receiver, EventCallback<T> callback, [CallerLineNumber] int line = 0 )
    {
        builder.AddAttribute( GetSequence( line ), "oncontextmenu", EventCallback.Factory.Create<T>( receiver, callback ) );

        return builder;
    }

    public static RenderTreeBuilder Content( this RenderTreeBuilder builder, RenderFragment fragment, [CallerLineNumber] int line = 0 )
    {
        builder.AddContent( GetSequence( line ), fragment );

        return builder;
    }

    public static RenderTreeBuilder Attributes( this RenderTreeBuilder builder, Dictionary<string, object> attributes, [CallerLineNumber] int line = 0 )
    {
        builder.AddMultipleAttributes( GetSequence( line ), attributes );

        return builder;
    }

    public static RenderTreeBuilder ComponentReferenceCapture( this RenderTreeBuilder builder, Action<object> action, [CallerLineNumber] int line = 0 )
    {
        builder.AddComponentReferenceCapture( GetSequence( line ), action );

        return builder;
    }

    public static RenderTreeBuilder ElementReferenceCapture( this RenderTreeBuilder builder, Action<ElementReference> action, [CallerLineNumber] int line = 0 )
    {
        builder.AddElementReferenceCapture( GetSequence( line ), action );

        return builder;
    }

    private static int GetSequence( int line )
    {
        // TODO
        return line;
    }
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member