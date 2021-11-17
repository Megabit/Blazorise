#region Using directives
using System.Collections.Generic;
using static Blazorise.RichTextEdit.DynamicReferenceType;
#endregion

namespace Blazorise.RichTextEdit
{
    /// <summary>
    /// Blazorise RichTextEdit extension options
    /// </summary>
    public sealed class RichTextEditOptions
    {
        private List<DynamicReference> dynamicReferences;

        /// <summary>
        /// Load the QuillJs snow theme related resources
        /// </summary>
        public bool UseShowTheme { get; set; } = true;

        /// <summary>
        /// Load the QuillJs bubble theme related resources
        /// </summary>
        public bool UseBubbleTheme { get; set; }

        /// <summary>
        /// The QuillJs version to load
        /// </summary>
        public string QuillJsVersion { get; set; } = "1.3.7";

        /// <summary>
        /// Load the RichTextEdit scripts and stylesheets on demand.
        /// </summary>
        public bool DynamicallyLoadReferences { get; set; } = true;

        /// <summary>
        /// Dynamic references to be loaded when initializing the RichTextEdit component.
        /// </summary>
        public List<DynamicReference> DynamicReferences
        {
            get => dynamicReferences ?? GetDefaultReferences();
            set => dynamicReferences = value;
        }

        private List<DynamicReference> GetDefaultReferences()
        {
            List<DynamicReference> references = new()
            {
                new( $@"https://cdn.quilljs.com/{QuillJsVersion}/quill.js", Script ),
                new( @"_content/Blazorise.RichTextEdit/blazorise.richtextedit.bundle.scp.css", Stylesheet )
            };

            if ( UseBubbleTheme )
            {
                references.Add( new( $@"https://cdn.quilljs.com/{QuillJsVersion}/quill.bubble.css", Stylesheet ) );
            }

            if ( UseShowTheme )
            {
                references.Add( new( $@"https://cdn.quilljs.com/{QuillJsVersion}/quill.snow.css", Stylesheet ) );
            }

            return references;
        }
    }
}