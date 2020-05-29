using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace Blazorise.RichTextEdit
{
    public partial class RichTextEditToolbarSelectItem
    {
        #region Members        
        /// <summary>The option attributes</summary>
        private Dictionary<string, object> attributes;
        #endregion

        #region Properties        
        /// <summary> Gets or sets a value indicating whether this option is selected, eg the default value.</summary>
        [Parameter] public bool IsSelected { get; set; }

        /// <summary> Gets or sets the value of this option</summary>
        [Parameter] public string Value { get; set; }

        /// <summary> Gets or sets the child content</summary>
        [Parameter] public RenderFragment ChildContent { get; set; }
        #endregion

        #region Methods        
        /// <summary>
        /// Method invoked when the component is ready to start, having received its
        /// initial parameters from its parent in the render tree.
        /// </summary>
        protected override void OnInitialized()
        {
            attributes = new Dictionary<string, object>();

            if ( IsSelected )
            {
                attributes["selected"] = true;
            }

            if ( !string.IsNullOrWhiteSpace( Value ) )
            {
                attributes["value"] = Value;
            }
        }
        #endregion
    }
}
