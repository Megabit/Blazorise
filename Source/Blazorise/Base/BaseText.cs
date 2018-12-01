#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Blazor;
using Microsoft.AspNetCore.Blazor.Components;
#endregion

namespace Blazorise.Base
{
    public abstract class BaseText : BaseInputComponent
    {
        #region Members

        private Color color;

        #endregion

        #region Methods

        protected override void RegisterClasses()
        {
            ClassMapper
                .Add( () => ClassProvider.Text( IsPlaintext ) )
                .If( () => ClassProvider.TextColor( Color ), () => Color != Color.None )
                .If( () => ClassProvider.TextSize( Size ), () => Size != Size.None );

            base.RegisterClasses();
        }

        protected void HandleTextChange( UIChangeEventArgs e )
        {
            Text = e?.Value?.ToString();
            TextChanged?.Invoke( Text );
        }

        #endregion

        #region Properties

        protected override bool NeedSizableBlock => ParentIsHorizontal && ParentAddons == null;

        protected string Type => Role.ToTextRoleString();

        /// <summary>
        /// Sets the role of the input text.
        /// </summary>
        [Parameter] protected TextRole Role { get; set; } = TextRole.Text;

        /// <summary>
        /// Sets the placeholder for the empty text.
        /// </summary>
        [Parameter] protected string Placeholder { get; set; }

        /// <summary>
        /// Gets or sets the text inside the input field.
        /// </summary>
        [Parameter] protected string Text { get; set; }

        /// <summary>
        /// Occurs after text has changed.
        /// </summary>
        [Parameter] protected Action<string> TextChanged { get; set; }

        /// <summary>
        /// Specifies the maximum number of characters allowed in the input element.
        /// </summary>
        [Parameter] protected int? MaxLength { get; set; }

        /// <summary>
        /// Sets the class to remove the default form field styling and preserve the correct margin and padding.
        /// </summary>
        [Parameter] protected bool IsPlaintext { get; set; }

        [Parameter]
        protected Color Color
        {
            get => color;
            set
            {
                color = value;

                Dirty();
                ClassMapper.Dirty();
            }
        }

        [CascadingParameter] protected BaseAddons ParentAddons { get; set; }

        #endregion
    }
}
