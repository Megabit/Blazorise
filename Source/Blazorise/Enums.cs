﻿#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Defines the button type and behaviour.
    /// </summary>
    public enum ButtonType
    {
        /// <summary>
        /// The button is a clickable button.
        /// </summary>
        Button,

        /// <summary>
        /// The button is a submit button (submits form-data).
        /// </summary>
        Submit,

        /// <summary>
        /// The button is a reset button (resets the form-data to its initial values).
        /// </summary>
        Reset,
    }

    /// <summary>
    /// Predefined set of contextual colors.
    /// </summary>
    public enum Color
    {
        /// <summary>
        /// No color will be applied to an element.
        /// </summary>
        None,

        /// <summary>
        /// Primary color.
        /// </summary>
        Primary,

        /// <summary>
        /// Secondary color.
        /// </summary>
        Secondary,

        /// <summary>
        /// Success color.
        /// </summary>
        Success,

        /// <summary>
        /// Danger color.
        /// </summary>
        Danger,

        /// <summary>
        /// Warning color.
        /// </summary>
        Warning,

        /// <summary>
        /// Info color.
        /// </summary>
        Info,

        /// <summary>
        /// Light color.
        /// </summary>
        Light,

        /// <summary>
        /// Dark color.
        /// </summary>
        Dark,

        /// <summary>
        /// Link color.
        /// </summary>
        Link,
    }

    /// <summary>
    /// Predefined set of contextual text colors.
    /// </summary>
    public enum TextColor
    {
        /// <summary>
        /// No color will be applied to an element.
        /// </summary>
        None,

        /// <summary>
        /// Primary color.
        /// </summary>
        Primary,

        /// <summary>
        /// Secondary color.
        /// </summary>
        Secondary,

        /// <summary>
        /// Success color.
        /// </summary>
        Success,

        /// <summary>
        /// Danger color.
        /// </summary>
        Danger,

        /// <summary>
        /// Warning color.
        /// </summary>
        Warning,

        /// <summary>
        /// Info color.
        /// </summary>
        Info,

        /// <summary>
        /// Light color.
        /// </summary>
        Light,

        /// <summary>
        /// Dark color.
        /// </summary>
        Dark,

        /// <summary>
        /// Body color.
        /// </summary>
        Body,

        /// <summary>
        /// Muted color.
        /// </summary>
        Muted,

        /// <summary>
        /// White color.
        /// </summary>
        White,

        /// <summary>
        /// Black text with 50% opacity on white background.
        /// </summary>
        Black50,

        /// <summary>
        /// White text with 50% opacity on black background.
        /// </summary>
        White50,
    }

    /// <summary>
    /// Predefined set of contextual background colors.
    /// </summary>
    public enum Background
    {
        /// <summary>
        /// No color will be applied to an element.
        /// </summary>
        None,

        /// <summary>
        /// Primary color.
        /// </summary>
        Primary,

        /// <summary>
        /// Secondary color.
        /// </summary>
        Secondary,

        /// <summary>
        /// Success color.
        /// </summary>
        Success,

        /// <summary>
        /// Danger color.
        /// </summary>
        Danger,

        /// <summary>
        /// Warning color.
        /// </summary>
        Warning,

        /// <summary>
        /// Info color.
        /// </summary>
        Info,

        /// <summary>
        /// Light color.
        /// </summary>
        Light,

        /// <summary>
        /// Dark color.
        /// </summary>
        Dark,

        /// <summary>
        /// White color.
        /// </summary>
        White,

        /// <summary>
        /// Transparent color.
        /// </summary>
        Transparent,
    }

    /// <summary>
    /// Adjusts the theme contrast.
    /// </summary>
    public enum Theme
    {
        /// <summary>
        /// Undefined.
        /// </summary>
        None,

        /// <summary>
        /// Adjusts the theme for a light colors.
        /// </summary>
        Light,

        /// <summary>
        /// Adjusts the theme for a dark colors.
        /// </summary>
        Dark,
    }

    /// <summary>
    /// Defines an element size.
    /// </summary>
    public enum Size
    {
        /// <summary>
        /// Don't resize an element.
        /// </summary>
        None,

        /// <summary>
        /// Makes an element extra small size.
        /// </summary>
        ExtraSmall,

        /// <summary>
        /// Makes an element small size.
        /// </summary>
        Small,

        /// <summary>
        /// Makes an element medium size.
        /// </summary>
        Medium,

        /// <summary>
        /// Makes an element large.
        /// </summary>
        Large,

        /// <summary>
        /// Makes an element extra large.
        /// </summary>
        ExtraLarge,
    }

    /// <summary>
    /// Defines the media breakpoint.
    /// </summary>
    public enum Breakpoint
    {
        /// <summary>
        /// Breakpoint is undefined.
        /// </summary>
        None,

        /// <summary>
        /// Valid on all devices. (extra small)
        /// </summary>
        Mobile,

        /// <summary>
        /// Breakpoint on tablets (small).
        /// </summary>
        Tablet,

        /// <summary>
        ///  Breakpoint on desktop (medium).
        /// </summary>
        Desktop,

        /// <summary>
        /// Breakpoint on widescreen (large).
        /// </summary>
        Widescreen,

        /// <summary>
        /// Breakpoint on large desktops (extra large).
        /// </summary>
        FullHD,
    }

    /// <summary>
    /// Defines number of columns to occupy in the grid.
    /// </summary>
    public enum ColumnWidth
    {
        /// <summary>
        /// No sizing.
        /// </summary>
        None,

        /// <summary>
        /// One column width.
        /// </summary>
        Is1,

        /// <summary>
        /// Two columns width.
        /// </summary>
        Is2,

        /// <summary>
        /// Three columns width.
        /// </summary>
        Is3,

        /// <summary>
        /// Four columns width.
        /// </summary>
        Is4,

        /// <summary>
        /// Five columns width.
        /// </summary>
        Is5,

        /// <summary>
        /// Six columns width.
        /// </summary>
        Is6,

        /// <summary>
        /// Seven columns width.
        /// </summary>
        Is7,

        /// <summary>
        /// Eight columns width.
        /// </summary>
        Is8,

        /// <summary>
        /// Nine columns width.
        /// </summary>
        Is9,

        /// <summary>
        /// Ten columns width.
        /// </summary>
        Is10,

        /// <summary>
        /// Eleven columns width.
        /// </summary>
        Is11,

        /// <summary>
        /// Twelve columns width.
        /// </summary>
        Is12,

        /// <summary>
        /// Twelve columns width.
        /// </summary>
        Full,

        /// <summary>
        /// Six columns width.
        /// </summary>
        Half,

        /// <summary>
        /// Four columns width.
        /// </summary>
        Third,

        /// <summary>
        /// Three columns width.
        /// </summary>
        Quarter,

        /// <summary>
        /// Fill all available space.
        /// </summary>
        Auto,
    }

    /// <summary>
    /// Direction of an dropdown menu.
    /// </summary>
    public enum Direction
    {
        /// <summary>
        /// Same as <see cref="Down"/>.
        /// </summary>
        None,

        /// <summary>
        /// Trigger dropdown menus bellow an element (default behaviour).
        /// </summary>
        Down,

        /// <summary>
        /// Trigger dropdown menus above an element.
        /// </summary>
        Up,

        /// <summary>
        /// Trigger dropdown menus to the right of an element.
        /// </summary>
        Right,

        /// <summary>
        /// Trigger dropdown menus to the left of an element.
        /// </summary>
        Left,
    }

    /// <summary>
    /// Defines the side on which to apply the spacing.
    /// </summary>
    public enum Side
    {
        /// <summary>
        /// No side.
        /// </summary>
        None,

        /// <summary>
        /// Top side.
        /// </summary>
        Top,

        /// <summary>
        /// Bottom side.
        /// </summary>
        Bottom,

        /// <summary>
        /// Left side.
        /// </summary>
        Left,

        /// <summary>
        /// Right side.
        /// </summary>
        Right,

        /// <summary>
        /// Left and right side.
        /// </summary>
        X,

        /// <summary>
        /// Top and bottom side.
        /// </summary>
        Y,

        /// <summary>
        /// All 4 sides of the element.
        /// </summary>
        All,
    }

    /// <summary>
    /// Floats an element to the left or right, or disable floating.
    /// </summary>
    public enum Float
    {
        /// <summary>
        /// Don't float on all viewport sizes.
        /// </summary>
        None,

        /// <summary>
        /// Float left on all viewport sizes.
        /// </summary>
        Left,

        /// <summary>
        /// Float right on all viewport sizes.
        /// </summary>
        Right,
    }

    /// <summary>
    /// Defines the spacing property.
    /// </summary>
    public enum Spacing
    {
        /// <summary>
        /// No spacing will be used.
        /// </summary>
        None,

        /// <summary>
        /// Use the margin spacing.
        /// </summary>
        Margin,

        /// <summary>
        /// Use the padding spacing.
        /// </summary>
        Padding,
    }

    public enum SpacingSize
    {
        /// <summary>
        /// For classes that eliminate the margin or padding by setting it to 0.
        /// </summary>
        Is0,

        /// <summary>
        /// (by default) for classes that set the margin or padding to $spacer * .25
        /// </summary>
        Is1,

        /// <summary>
        /// (by default) for classes that set the margin or padding to $spacer * .5
        /// </summary>
        Is2,

        /// <summary>
        /// (by default) for classes that set the margin or padding to $spacer
        /// </summary>
        Is3,

        /// <summary>
        /// (by default) for classes that set the margin or padding to $spacer * 1.5
        /// </summary>
        Is4,

        /// <summary>
        /// (by default) for classes that set the margin or padding to $spacer * 3
        /// </summary>
        Is5,

        /// <summary>
        /// For classes that set the margin to auto.
        /// </summary>
        IsAuto,
    }

    /// <summary>
    /// Defines the behaviour of the text input.
    /// </summary>
    public enum TextRole
    {
        /// <summary>
        /// Defines a default text input field.
        /// </summary>
        Text,

        /// <summary>
        /// Used for input fields that should contain an e-mail address.
        /// </summary>
        Email,

        /// <summary>
        /// Defines a password field.
        /// </summary>
        Password,

        /// <summary>
        /// Used for input fields that should contain a URL address.
        /// </summary>
        Url,
    }

    /// <summary>
    /// Specifies what kind of input mechanism would be most helpful for users entering content.
    /// </summary>
    public enum TextInputMode
    {
        /// <summary>
        /// The user agent should not display a virtual keyboard. This keyword is useful for content that renders its own keyboard control.
        /// </summary>
        None,

        /// <summary>
        /// The user agent should display a virtual keyboard capable of text input in the user's locale.
        /// </summary>
        Text,

        /// <summary>
        /// The user agent should display a virtual keyboard capable of telephone number input. This should including keys for the digits 0 to 9, the "#" character, and the "*" character. In some locales, this can also include alphabetic mnemonic labels (e.g., in the US, the key labeled "2" is historically also labeled with the letters A, B, and C).
        /// </summary>
        Tel,

        /// <summary>
        /// The user agent should display a virtual keyboard capable of text input in the user's locale, with keys for aiding in the input of URLs, such as that for the "/" and "." characters and for quick input of strings commonly found in domain names such as "www." or ".com".
        /// </summary>
        Url,

        /// <summary>
        /// The user agent should display a virtual keyboard capable of text input in the user's locale, with keys for aiding in the input of e-mail addresses, such as that for the "@" character and the "." character.
        /// </summary>
        Email,

        /// <summary>
        /// The user agent should display a virtual keyboard capable of numeric input. This keyword is useful for PIN entry.
        /// </summary>
        Numeric,

        /// <summary>
        /// The user agent should display a virtual keyboard capable of fractional numeric input. Numeric keys and the format separator for the locale should be shown.
        /// </summary>
        Decimal,

        /// <summary>
        /// The user agent should display a virtual keyboard optimized for search.
        /// </summary>
        Search,
    }

    public enum NavFillType
    {
        /// <summary>
        /// Same as <see cref="Default"/>.
        /// </summary>
        None,

        /// <summary>
        /// Fill all available space.
        /// </summary>
        Default,

        /// <summary>
        /// Fill all available space but with all of the elements having the same width.
        /// </summary>
        Justified,
    }

    /// <summary>
    /// Defines an element visibility behaviour.
    /// </summary>
    public enum Visibility
    {
        /// <summary>
        /// None.
        /// </summary>
        Default,

        /// <summary>
        /// Element will always be visible.
        /// </summary>
        Always,

        /// <summary>
        /// Element will always bi hidden.
        /// </summary>
        Never,
    }


    /// <summary>
    /// Defines the alignment of an element.
    /// </summary>
    public enum Alignment
    {
        /// <summary>
        /// No alignment will be applied.
        /// </summary>
        None,

        /// <summary>
        /// Aligns an element to the left.
        /// </summary>
        Start,

        /// <summary>
        /// Aligns an element on the center.
        /// </summary>
        Center,

        /// <summary>
        /// Aligns an element to the right.
        /// </summary>
        End,
    }

    /// <summary>
    /// Defines the text alignment.
    /// </summary>
    public enum TextAlignment
    {
        /// <summary>
        /// No alignment will be applied.
        /// </summary>
        None,

        /// <summary>
        /// Aligns the text to the left.
        /// </summary>
        Left,

        /// <summary>
        /// Centers the text.
        /// </summary>
        Center,

        /// <summary>
        /// Aligns the text to the right.
        /// </summary>
        Right,

        /// <summary>
        /// Stretches the lines so that each line has equal width.
        /// </summary>
        Justified
    }

    public enum TextTransform
    {
        /// <summary>
        /// No capitalization. The text renders as it is. This is default
        /// </summary>
        None,

        /// <summary>
        /// Transforms all characters to lowercase.
        /// </summary>
        Lowercase,

        /// <summary>
        /// Transforms all characters to uppercase.
        /// </summary>
        Uppercase,

        /// <summary>
        /// Transforms the first character of each word to uppercase
        /// </summary>
        Capitalize,
    }

    public enum TextWeight
    {
        /// <summary>
        /// No weight will be applied.
        /// </summary>
        None,

        /// <summary>
        /// Defines normal characters. This is default.
        /// </summary>
        Normal,

        /// <summary>
        /// 	Defines thick characters.
        /// </summary>
        Bold,

        /// <summary>
        /// Defines lighter characters.
        /// </summary>
        Light,
    }

    /// <summary>
    /// Modifies the URL matching behavior for a link.
    /// </summary>
    public enum Match
    {
        /// <summary>
        /// Specifies that the link should be active when it matches any prefix
        /// of the current URL.
        /// </summary>
        Prefix,

        /// <summary>
        /// Specifies that the link should be active when it matches the entire
        /// current URL.
        /// </summary>
        All,
    }

    /// <summary>
    /// Addon element type.
    /// </summary>
    public enum AddonType
    {
        /// <summary>
        /// Main addon of a field.
        /// </summary>
        Body,

        /// <summary>
        /// Addon will placed at the start of a field.
        /// </summary>
        Start,

        /// <summary>
        /// Addon will placed at the end of a field.
        /// </summary>
        End,
    }

    /// <summary>
    /// Custom input roles.
    /// </summary>
    public enum ControlRole
    {
        None,
        Check,
        Radio,
        File,
        Text,
    }

    /// <summary>
    /// Changes the size of the modal.
    /// </summary>
    public enum ModalSize
    {
        /// <summary>
        /// No sizing applied.
        /// </summary>
        None,

        /// <summary>
        /// Default modal size.
        /// </summary>
        Default,

        /// <summary>
        /// Small modal.
        /// </summary>
        Small,

        /// <summary>
        /// Large modal.
        /// </summary>
        Large,

        /// <summary>
        /// Extra large modal.
        /// </summary>
        ExtraLarge,
    }

    /// <summary>
    /// Buttons group behaviour.
    /// </summary>
    public enum ButtonsRole
    {
        /// <summary>
        /// Display buttons as addons.
        /// </summary>
        Addons,

        /// <summary>
        /// Display buttons as toolbar buttons.
        /// </summary>
        Toolbar,
    }

    /// <summary>
    /// Element orientation.
    /// </summary>
    public enum Orientation
    {
        /// <summary>
        /// Horizontal orientation.
        /// </summary>
        Horizontal,

        /// <summary>
        /// Vertical orientation.
        /// </summary>
        Vertical,
    }

    /// <summary>
    /// Aligns the flexible container's items when the items do not use all available space on the main-axis (horizontally).
    /// </summary>
    public enum JustifyContent
    {
        /// <summary>
        /// Sets this property to its default value.
        /// </summary>
        None,

        /// <summary>
        /// Items are positioned at the beginning of the container.
        /// </summary>
        Start,

        /// <summary>
        /// Items are positioned at the end of the container.
        /// </summary>
        End,

        /// <summary>
        /// Items are positioned at the center of the container.
        /// </summary>
        Center,

        /// <summary>
        /// Items are positioned with space between the lines.
        /// </summary>
        Between,

        /// <summary>
        /// Items are positioned with space before, between, and after the lines.
        /// </summary>
        Around,
    }

    /// <summary>
    /// Screen reader visibility.
    /// </summary>
    public enum Screenreader
    {
        /// <summary>
        /// Default.
        /// </summary>
        Always,

        /// <summary>
        /// Hide an element to all devices except screen readers.
        /// </summary>
        Only,

        /// <summary>
        /// Show the element again when it’s focused.
        /// </summary>
        OnlyFocusable,
    }

    /// <summary>
    /// Defines the heading size.
    /// </summary>
    public enum HeadingSize
    {
        /// <summary>
        /// Main title.
        /// </summary>
        Is1,

        Is2,

        Is3,

        Is4,

        Is5,

        Is6,
    }

    /// <summary>
    /// Defines the validation results.
    /// </summary>
    public enum ValidationStatus
    {
        /// <summary>
        /// No validation.
        /// </summary>
        None,

        /// <summary>
        /// Validation has passed the check.
        /// </summary>
        Success,

        /// <summary>
        /// Validation has failed.
        /// </summary>
        Error,
    }

    /// <summary>
    /// Defines the validation execution mode.
    /// </summary>
    public enum ValidationMode
    {
        /// <summary>
        /// Validation will execute on every input change.
        /// </summary>
        Auto,

        /// <summary>
        /// Validation will run only when explicitly called. 
        /// </summary>
        Manual,
    }

    /// <summary>
    /// Defines a button size.
    /// </summary>
    public enum ButtonSize
    {
        /// <summary>
        /// No sizing will be applied to the button.
        /// </summary>
        None,

        /// <summary>
        /// Makes a button to appear smaller.
        /// </summary>
        Small,

        /// <summary>
        /// Makes a button to appear larger.
        /// </summary>
        Large,
    }

    /// <summary>
    /// Defines sizes for button group.
    /// </summary>
    public enum ButtonsSize
    {
        /// <summary>
        /// No sizing will be applied to the buttons.
        /// </summary>
        None,

        /// <summary>
        /// Makes a buttons to appear smaller.
        /// </summary>
        Small,

        /// <summary>
        /// Makes a buttons to appear larger.
        /// </summary>
        Large,
    }

    /// <summary>
    /// Defines the mouse cursor.
    /// </summary>
    public enum Cursor
    {
        /// <summary>
        /// Default behaviour, nothing will be changed.
        /// </summary>
        Default,

        /// <summary>
        /// The cursor is a pointer and indicates a link.
        /// </summary>
        Pointer,
    }

    public enum IconName
    {
        New,
        Edit,
        Save,
        Cancel,
        Delete,
        Clear,
        Search,
        ClearSearch,
        Phone,
        Smartphone,
        Mail,
        Person,
        Lock,
        MoreHorizontal,
        MoreVertical,
        ExpandMore,
        ExpandLess,
        SliderHorizontal,
        SliderVertical,
        Dashboard,
    }

    public enum FormatType
    {
        None = 0,
        Numeric = 1,
        DateTime = 2,
        Custom = 3,
    }

    public enum MaskType
    {
        None,
        Numeric = 1,
        DateTime = 2,
        Regex = 3,
    }

    public enum MouseButton
    {
        Left = 0,
        Middle = 1,
        Right = 2,
    }
}
