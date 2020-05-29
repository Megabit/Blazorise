using Microsoft.AspNetCore.Components;

namespace Blazorise.RichTextEdit
{
    public partial class RichTextEditToolbarGroup
    {
        #region Members        
        /// <summary>The class provider.</summary>
        [Inject] private IClassProvider classProvider { get; set; }

        /// <summary>The constructed class names</summary>
        private string classNames;
        #endregion

        #region Properties        
        /// <summary>Gets or sets the child content.</summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        /// <summary>Gets or sets the float position.</summary>
        [Parameter] public Float Float { get; set; }
        #endregion

        #region Methods                
        /// <summary>
        /// Method invoked when the component has received parameters from its parent in
        /// the render tree, and the incoming values have been assigned to properties.
        /// </summary>
        protected override void OnParametersSet()
        {
            classNames = "ql-formats";

            if ( Float != Float.None )
            {
                classNames += " " + classProvider.ToFloat( Float );
            }
        }
        #endregion
    }
}
