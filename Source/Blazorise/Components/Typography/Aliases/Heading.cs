namespace Blazorise;

/// <summary>
/// Represents a first-level heading element, equivalent to an HTML <c>&lt;h1&gt;</c> tag.
/// </summary>
/// <remarks>
/// This component is an alias for <see cref="Heading"/> with <see cref="Heading.Size"/> preset to <see cref="HeadingSize.Is1"/>.
/// Use it to provide semantic and visual emphasis for the highest-level heading on a page.
/// </remarks>
public class Heading1 : Heading
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Heading1"/> class
    /// with the <see cref="Heading.Size"/> set to <see cref="HeadingSize.Is1"/>.
    /// </summary>
    public Heading1()
    {
        Size = HeadingSize.Is1;
    }
}

/// <summary>
/// Represents a second-level heading element, equivalent to an HTML <c>&lt;h2&gt;</c> tag.
/// </summary>
/// <remarks>
/// This component is an alias for <see cref="Heading"/> with <see cref="Heading.Size"/> preset to <see cref="HeadingSize.Is2"/>.
/// It is typically used for section titles below the main heading.
/// </remarks>
public class Heading2 : Heading
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Heading2"/> class
    /// with the <see cref="Heading.Size"/> set to <see cref="HeadingSize.Is2"/>.
    /// </summary>
    public Heading2()
    {
        Size = HeadingSize.Is2;
    }
}

/// <summary>
/// Represents a third-level heading element, equivalent to an HTML <c>&lt;h3&gt;</c> tag.
/// </summary>
/// <remarks>
/// This component is an alias for <see cref="Heading"/> with <see cref="Heading.Size"/> preset to <see cref="HeadingSize.Is3"/>.
/// It is typically used for sub-section titles.
/// </remarks>
public class Heading3 : Heading
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Heading3"/> class
    /// with the <see cref="Heading.Size"/> set to <see cref="HeadingSize.Is3"/>.
    /// </summary>
    public Heading3()
    {
        Size = HeadingSize.Is3;
    }
}

/// <summary>
/// Represents a fourth-level heading element, equivalent to an HTML <c>&lt;h4&gt;</c> tag.
/// </summary>
/// <remarks>
/// This component is an alias for <see cref="Heading"/> with <see cref="Heading.Size"/> preset to <see cref="HeadingSize.Is4"/>.
/// It is often used for lower-level section headings.
/// </remarks>
public class Heading4 : Heading
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Heading4"/> class
    /// with the <see cref="Heading.Size"/> set to <see cref="HeadingSize.Is4"/>.
    /// </summary>
    public Heading4()
    {
        Size = HeadingSize.Is4;
    }
}

/// <summary>
/// Represents a fifth-level heading element, equivalent to an HTML <c>&lt;h5&gt;</c> tag.
/// </summary>
/// <remarks>
/// This component is an alias for <see cref="Heading"/> with <see cref="Heading.Size"/> preset to <see cref="HeadingSize.Is5"/>.
/// It is suitable for minor section titles or less prominent headings.
/// </remarks>
public class Heading5 : Heading
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Heading5"/> class
    /// with the <see cref="Heading.Size"/> set to <see cref="HeadingSize.Is5"/>.
    /// </summary>
    public Heading5()
    {
        Size = HeadingSize.Is5;
    }
}

/// <summary>
/// Represents a sixth-level heading element, equivalent to an HTML <c>&lt;h6&gt;</c> tag.
/// </summary>
/// <remarks>
/// This component is an alias for <see cref="Heading"/> with <see cref="Heading.Size"/> preset to <see cref="HeadingSize.Is6"/>.
/// It is typically used for the least prominent headings in a content hierarchy.
/// </remarks>
public class Heading6 : Heading
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Heading6"/> class
    /// with the <see cref="Heading.Size"/> set to <see cref="HeadingSize.Is6"/>.
    /// </summary>
    public Heading6()
    {
        Size = HeadingSize.Is6;
    }
}