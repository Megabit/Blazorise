#region Using directives
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Blazor.Components;
#endregion

namespace Blazorise
{
    public class BaseComponent : BlazorComponent
    {
        #region Members

        protected string elementId = Utils.IDGenerator.Instance.Generate;

        //protected HashSet<string> Classes { get; set; }

        protected StringBuilder sb = new StringBuilder();

        #endregion

        #region Methods

        //protected void Push( string classname )
        //{
        //    if ( classname == null )
        //        return;

        //    if ( Classes == null )
        //        Classes = new HashSet<string>();

        //    if ( Classes.Contains( classname ) )
        //        return;

        //    Classes.Add( classname );
        //}

        protected virtual string Build( params string[] classnames )
        {
            var sb = new StringBuilder();

            foreach ( var cn in classnames )
            {
                Append( sb, cn );
            }

            #region Layout classes

            //var margin = Margin != Side.None && MarginSize != null ? ClassProvider.Margin( Margin, MarginSize ?? 0 ) : null;
            //var padding = Padding != Side.None && PaddingSize != null ? ClassProvider.Padding( Padding, PaddingSize ?? 0 ) : null;
            var margin = Margin?.Build( ClassProvider );
            var padding = Padding?.Build( ClassProvider );
            var @float = Float != Float.None ? ClassProvider.Float( Float ) : null;

            Append( sb, margin );
            Append( sb, padding );
            Append( sb, @float );

            #endregion

            //#region Child classes

            //if ( Classes != null )
            //{
            //    foreach ( var cn in Classes )
            //    {
            //        if ( cn != null )
            //        {
            //            if ( sb.Length > 0 )
            //                sb.Append( " " );

            //            sb.Append( cn );
            //        }
            //    }
            //}

            //#endregion

            #region Custom class

            if ( Class != null )
            {
                if ( sb.Length > 0 )
                    sb.Append( " " );

                sb.Append( Class );
            }

            #endregion

            return sb.ToString();
        }

        protected void Append( StringBuilder sb, string value )
        {
            if ( value == null )
                return;

            if ( sb.Length > 0 )
                sb.Append( " " );

            sb.Append( value );
        }

        #endregion

        #region Properties

        [Inject] protected IClassProvider ClassProvider { get; set; }

        /// <summary>
        /// Float an element to the left or right.
        /// </summary>
        [Parameter] Float Float { get; set; } = Float.None;

        /// <summary>
        /// Gets or sets the custom css classname.
        /// </summary>
        [Parameter] protected string Class { get; set; }

        /// <summary>
        /// Defines the margin spacing.
        /// </summary>
        [Parameter] FluentSpacing Margin { get; set; }

        /// <summary>
        /// Defines the padding spacing.
        /// </summary>
        [Parameter] FluentSpacing Padding { get; set; }

        #endregion
    }
}
