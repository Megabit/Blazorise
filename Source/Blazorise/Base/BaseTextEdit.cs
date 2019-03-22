#region Using directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Base
{
    public abstract class BaseTextEdit : BaseTextInput<string>
    {
        #region Members

        #endregion

        #region Methods

        protected override void HandleValue( object value )
        {
            Text = value?.ToString();
            TextChanged.InvokeAsync( Text );
        }

        #endregion

        #region Properties

        protected string Type => Role.ToTextRoleString();

        /// <summary>
        /// Sets the role of the input text.
        /// </summary>
        [Parameter] protected TextRole Role { get; set; } = TextRole.Text;

        /// <summary>
        /// Gets or sets the text inside the input field.
        /// </summary>
        [Parameter] protected string Text { get => InternalValue; set => InternalValue = value; }

        /// <summary>
        /// Occurs after text has changed.
        /// </summary>
        [Parameter] protected EventCallback<string> TextChanged { get; set; }

        /// <summary>
        /// Specifies the maximum number of characters allowed in the input element.
        /// </summary>
        [Parameter] protected int? MaxLength { get; set; }

        #endregion
    }
}
