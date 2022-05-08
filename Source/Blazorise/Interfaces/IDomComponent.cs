#region Using directives
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Base interface for all DOM based components.
    /// </summary>
    public interface IDomComponent
    {
        /// <summary>
        /// Gets or sets the reference to the rendered element.
        /// </summary>
        ElementReference ElementRef { get; set; }

        /// <summary>
        /// Gets or sets the unique id of the element.
        /// </summary>
        /// <remarks>
        /// Note that this ID is not defined for the component but instead for the underlined element that it represents.
        /// eg: for the TextEdit the ID will be set on the input element.
        /// </remarks>
        string ElementId { get; set; }

        /// <summary>
        /// Custom css classname.
        /// </summary>
        string Class { get; set; }

        /// <summary>
        /// Custom html style.
        /// </summary>
        string Style { get; set; }

        /// <summary>
        /// Floats an element to the defined side.
        /// </summary>
        Float Float { get; set; }

        /// <summary>
        /// Fixes an element's floating children.
        /// </summary>
        bool Clearfix { get; set; }

        /// <summary>
        /// Controls the visibility, without modifying the display, of elements with visibility utilities.
        /// </summary>
        Visibility Visibility { get; set; }

        /// <summary>
        /// Defined the sizing for the element width attribute(s).
        /// </summary>
        IFluentSizing Width { get; set; }

        /// <summary>
        /// Defined the sizing for the element height attribute(s).
        /// </summary>
        IFluentSizing Height { get; set; }

        /// <summary>
        /// Defines the element margin spacing.
        /// </summary>
        IFluentSpacing Margin { get; set; }

        /// <summary>
        /// Defines the element padding spacing.
        /// </summary>
        IFluentSpacing Padding { get; set; }

        /// <summary>
        /// Specifies the display behavior of an element.
        /// </summary>
        IFluentDisplay Display { get; set; }

        /// <summary>
        /// Specifies the border of an element.
        /// </summary>
        IFluentBorder Border { get; set; }

        /// <summary>
        /// Specifies flexbox properties of an element.
        /// </summary>
        IFluentFlex Flex { get; set; }

        /// <summary>
        /// The position property specifies the type of positioning method used for an element (static, relative, fixed, absolute or sticky).
        /// </summary>
        IFluentPosition Position { get; set; }

        /// <summary>
        /// The overflow property controls what happens to content that is too big to fit into an area.
        /// </summary>
        IFluentOverflow Overflow { get; set; }

        /// <summary>
        /// Changes the character casing of a element.
        /// </summary>
        CharacterCasing Casing { get; set; }

        /// <summary>
        /// Gets or sets the text color.
        /// </summary>
        TextColor TextColor { get; set; }

        /// <summary>
        /// Gets or sets the text alignment.
        /// </summary>
        TextAlignment TextAlignment { get; set; }

        /// <summary>
        /// Gets or sets the text transformation.
        /// </summary>
        TextTransform TextTransform { get; set; }

        /// <summary>
        /// Gets or sets the text weight.
        /// </summary>
        TextWeight TextWeight { get; set; }

        /// <summary>
        /// Determines how the text will behave when it is larger than a parent container.
        /// </summary>
        TextOverflow TextOverflow { get; set; }

        /// <summary>
        /// Changes the vertical alignment of inline, inline-block, inline-table, and table cell elements.
        /// </summary>
        VerticalAlignment VerticalAlignment { get; set; }

        /// <summary>
        /// Gets or sets the component background color.
        /// </summary>
        Background Background { get; set; }

        /// <summary>
        /// Gets or sets the component shadow box.
        /// </summary>
        Shadow Shadow { get; set; }

        /// <summary>
        /// Captures all the custom attribute that are not part of Blazorise component.
        /// </summary>
        Dictionary<string, object> Attributes { get; set; }
    }
}
