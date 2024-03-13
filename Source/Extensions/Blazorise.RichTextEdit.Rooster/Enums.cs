namespace Blazorise.RichTextEdit.Rooster;

/// <summary>
/// The enum used for controlling the capitalization of text.
/// </summary>
public record Capitalization( string Value )
{
    /// <summary>
    /// Transforms the first character of each word to uppercase
    /// </summary>
    public static Capitalization CapitalizeEachWord { get; } = new( "capitalize" );
    /// <summary>
    /// Transforms all characters to lowercase
    /// </summary>
    public static Capitalization Lowercase { get; } = new( "lowercase" );
    /// <summary>
    /// Transforms the first character after punctuation mark followed by space to uppercase and the rest of characters to lowercase.
    /// </summary>
    public static Capitalization Sentence { get; } = new( "sentence" );
    /// <summary>
    /// Transforms all characters to uppercase
    /// </summary>
    public static Capitalization Uppercase { get; } = new( "uppercase" );
}

/// <summary>
/// Represents the strategy to clear the format of the current editor selection
/// </summary>
public enum ClearFormatMode
{
    /// <summary>
    /// Detect Inline or Block format based on the current editor selector.
    /// </summary>
    AutoDetect = 2,
    /// <summary>
    /// BLock format. Remove text and structure format of the block.
    /// </summary>
    Block = 1,
    /// <summary>
    /// Inline format. Remove text format.
    /// </summary>
    Inline = 0
}

/// <summary>
/// The enum used for increase or decrease font size
/// </summary>
public enum FontSizeChange
{
    /// <summary>
    /// Decrease font size
    /// </summary>
    Decrease = 1,
    /// <summary>
    /// Increase font size
    /// </summary>
    Increase = 0
}

/// <summary>
/// Enum used to control the different types of bullet list.
/// </summary>
public enum BulletListType
{
    /// <summary>
    /// Bullet type circle
    /// </summary>
    Circle = 9,
    /// <summary>
    /// Bullet triggered by -
    /// </summary>
    Dash = 2,
    /// <summary>
    /// Bullet triggered by *
    /// </summary>
    Disc = 1,
    /// <summary>
    /// Bullet triggered by -->
    /// </summary>
    DoubleLongArrow = 8,
    /// <summary>
    /// Bullet triggered by —
    /// </summary>
    Hyphen = 7,
    /// <summary>
    /// Bullet triggered by ->
    /// </summary>
    LongArrow = 5,
    /// <summary>
    /// Maximum value of the enum
    /// </summary>
    Max = 9,
    /// <summary>
    /// Minimum value of the enum
    /// </summary>
    Min = 1,
    /// <summary>
    /// Bullet triggered by >
    /// </summary>
    ShortArrow = 4,
    /// <summary>
    /// Bullet triggered by --
    /// </summary>
    Square = 3,
    /// <summary>
    /// Bullet triggered by =>
    /// </summary>
    UnfilledArrow = 6
}

/// <summary>
/// The enum used for increase or decrease indentation of a block
/// </summary>
public enum Indentation
{
    /// <summary>
    /// Decrease indentation
    /// </summary>
    Decrease = 1,
    /// <summary>
    /// Increase indentation
    /// </summary>
    Increase = 0
}