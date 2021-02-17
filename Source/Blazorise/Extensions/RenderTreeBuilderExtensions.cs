﻿#region Using directives
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
#endregion

namespace Blazorise.Extensions
{
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
            if ( target != Blazorise.Target.None )
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
            builder.AddAttribute( GetSequence( line ), $"tabindex", value );

            return builder;
        }

        public static RenderTreeBuilder AriaPressed( this RenderTreeBuilder builder, object value, [CallerLineNumber] int line = 0 )
        {
            return Aria( builder, "pressed", value );
        }

        public static RenderTreeBuilder AriaHidden( this RenderTreeBuilder builder, object value, [CallerLineNumber] int line = 0 )
        {
            return Aria( builder, "hidden", value );
        }

        public static RenderTreeBuilder AriaLabel( this RenderTreeBuilder builder, object value, [CallerLineNumber] int line = 0 )
        {
            return Aria( builder, "label", value );
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

        public static RenderTreeBuilder OnClick( this RenderTreeBuilder builder, object receiver, EventCallback callback, [CallerLineNumber] int line = 0 )
        {
            builder.AddAttribute( GetSequence( line ), "onclick", EventCallback.Factory.Create<Microsoft.AspNetCore.Components.Web.MouseEventArgs>( receiver, callback ) );

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
}