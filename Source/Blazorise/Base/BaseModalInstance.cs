#region Using directives
using System.Collections.Generic;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Shared attributes for <see cref="ModalProvider"/> modal instances.
    /// </summary>
    public abstract class BaseModalInstance
    {
        /// <summary>
        /// All the custom attribute that are not part of Blazorise component.
        /// </summary>
        public virtual Dictionary<string, object> Attributes { get; set; }

        /// <summary>
        /// Gets or sets the component shadow box.
        /// </summary>
        public virtual Shadow Shadow { get; set; }

        /// <summary>
        /// Gets or sets the component background color.
        /// </summary>
        public virtual Background Background { get; set; }

        /// <summary>
        /// Changes the vertical alignment of inline, inline-block, inline-table, and table cell elements.
        /// </summary>
        public virtual VerticalAlignment VerticalAlignment { get; set; }

        /// <summary>
        /// Determines how the text will behave when it is larger than a parent container.
        /// </summary>
        public virtual TextOverflow TextOverflow { get; set; }

        /// <summary>
        /// Gets or sets the text weight.
        /// </summary>
        public virtual TextWeight TextWeight { get; set; }

        /// <summary>
        /// Gets or sets the text transformation.
        /// </summary>
        public virtual TextTransform TextTransform { get; set; }

        /// <summary>
        /// Gets or sets the text alignment.
        /// </summary>
        public virtual TextAlignment TextAlignment { get; set; }

        /// <summary>
        /// Gets or sets the text color.
        /// </summary>
        public virtual TextColor TextColor { get; set; }

        /// <summary>
        /// Changes the character casing of a element.
        /// </summary>
        public virtual CharacterCasing Casing { get; set; }

        /// <summary>
        /// The overflow property controls what happens to content that is too big to fit into an area.
        /// </summary>
        public virtual IFluentOverflow Overflow { get; set; }

        /// <summary>
        /// The position property specifies the type of positioning method used for an element (static, relative, fixed, absolute or sticky).
        /// </summary>
        public virtual IFluentPosition Position { get; set; }

        /// <summary>
        /// Specifies flexbox properties of an element.
        /// </summary>
        public virtual IFluentFlex Flex { get; set; }

        /// <summary>
        /// Specifies the border of an element.
        /// </summary>
        public virtual IFluentBorder Border { get; set; }

        /// <summary>
        /// Specifies the display behavior of an element.
        /// </summary>
        public virtual IFluentDisplay Display { get; set; }

        /// <summary>
        /// Defines the element padding spacing.
        /// </summary>
        public virtual IFluentSpacing Padding { get; set; }

        /// <summary>
        /// Defines the element margin spacing.
        /// </summary>
        public virtual IFluentSpacing Margin { get; set; }
        /// <summary>
        /// Defined the sizing for the element height attribute(s).
        /// </summary>
        public virtual IFluentSizing Height { get; set; }

        /// <summary>
        /// Defined the sizing for the element width attribute(s).
        /// </summary>
        public virtual IFluentSizing Width { get; set; }

        /// <summary>
        /// Controls the visibility, without modifying the display, of elements with visibility utilities.
        /// </summary>
        public virtual Visibility Visibility { get; set; }

        /// <summary>
        /// Fixes an element's floating children.
        /// </summary>
        public virtual bool Clearfix { get; set; }

        /// <summary>
        /// Floats an element to the defined side.
        /// </summary>
        public virtual Float Float { get; set; }

        /// <summary>
        /// Custom html style.
        /// </summary>
        public virtual string Style { get; set; }

        /// <summary>
        /// Custom css classname.
        /// </summary>
        public virtual string Class { get; set; }

        /// <summary>
        /// Centers the modal vertically.
        /// </summary>
        /// <remarks>
        /// Only considered if UseModalStructure is set.
        /// </remarks>
        public virtual bool Centered { get; set; }

        /// <summary>
        /// Scrolls the modal content independent of the page itself.
        /// </summary>
        /// <remarks>
        /// Only considered if <see cref="ModalInstanceOptions.UseModalStructure"/> is set.
        /// </remarks>
        public virtual bool Scrollable { get; set; }

        /// <summary>
        /// Changes the size of the modal.
        /// </summary>
        /// <remarks>
        /// Only considered if <see cref="ModalInstanceOptions.UseModalStructure"/> is set.
        /// </remarks>
        public virtual ModalSize Size { get; set; }
    }

}
