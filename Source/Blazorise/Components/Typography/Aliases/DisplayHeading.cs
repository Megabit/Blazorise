namespace Blazorise
{
    /// <summary>
    /// Represents a large, prominent display heading, equivalent to the display-1 style.
    /// </summary>
    /// <remarks>
    /// This component is an alias for <see cref="DisplayHeading"/> with <see cref="DisplayHeading.Size"/> preset to <see cref="DisplayHeadingSize.Is1"/>.
    /// It is typically used for the most visually prominent headings, such as hero titles or page banners.
    /// </remarks>
    public class DisplayHeading1 : DisplayHeading
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayHeading1"/> class
        /// with the <see cref="DisplayHeading.Size"/> set to <see cref="DisplayHeadingSize.Is1"/>.
        /// </summary>
        public DisplayHeading1()
        {
            Size = DisplayHeadingSize.Is1;
        }
    }

    /// <summary>
    /// Represents a large display heading, equivalent to the display-2 style.
    /// </summary>
    /// <remarks>
    /// This component is an alias for <see cref="DisplayHeading"/> with <see cref="DisplayHeading.Size"/> preset to <see cref="DisplayHeadingSize.Is2"/>.
    /// It is commonly used for secondary hero text or major section titles.
    /// </remarks>
    public class DisplayHeading2 : DisplayHeading
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayHeading2"/> class
        /// with the <see cref="DisplayHeading.Size"/> set to <see cref="DisplayHeadingSize.Is2"/>.
        /// </summary>
        public DisplayHeading2()
        {
            Size = DisplayHeadingSize.Is2;
        }
    }

    /// <summary>
    /// Represents a medium display heading, equivalent to the display-3 style.
    /// </summary>
    /// <remarks>
    /// This component is an alias for <see cref="DisplayHeading"/> with <see cref="DisplayHeading.Size"/> preset to <see cref="DisplayHeadingSize.Is3"/>.
    /// It is useful for standout section titles that are smaller than hero headers but still highly visible.
    /// </remarks>
    public class DisplayHeading3 : DisplayHeading
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayHeading3"/> class
        /// with the <see cref="DisplayHeading.Size"/> set to <see cref="DisplayHeadingSize.Is3"/>.
        /// </summary>
        public DisplayHeading3()
        {
            Size = DisplayHeadingSize.Is3;
        }
    }

    /// <summary>
    /// Represents a smaller display heading, equivalent to the display-4 style.
    /// </summary>
    /// <remarks>
    /// This component is an alias for <see cref="DisplayHeading"/> with <see cref="DisplayHeading.Size"/> preset to <see cref="DisplayHeadingSize.Is4"/>.
    /// It is suitable for less prominent display headings or sub-headings within large text blocks.
    /// </remarks>
    public class DisplayHeading4 : DisplayHeading
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayHeading4"/> class
        /// with the <see cref="DisplayHeading.Size"/> set to <see cref="DisplayHeadingSize.Is4"/>.
        /// </summary>
        public DisplayHeading4()
        {
            Size = DisplayHeadingSize.Is4;
        }
    }
}