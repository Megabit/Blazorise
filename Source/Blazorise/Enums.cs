#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Blazorise
{
    public enum IconProvider
    {
        /// <summary>
        /// https://fontawesome.com/
        /// </summary>
        FontAwesome,

        /// <summary>
        /// https://material.io/tools/icons/?style=baseline
        /// </summary>
        Material,
    }

    public enum ButtonType
    {
        Button,
        Submit,
        Reset,
    }

    public enum Color
    {
        None,
        Active,
        Primary,
        Secondary,
        Success,
        Danger,
        Warning,
        Info,
        Light,
        Dark,
        Link,
    }

    public enum TextColor
    {
        None,
        Primary,
        Secondary,
        Success,
        Danger,
        Warning,
        Info,
        Light,
        Dark,
        Body,
        Muted,
        White,
        Black50,
        White50,
    }

    public enum Background
    {
        None,
        Primary,
        Secondary,
        Success,
        Danger,
        Warning,
        Info,
        Light,
        Dark,
        White,
        Transparent,
    }

    public enum Theme
    {
        None,
        Light,
        Dark,
    }

    public enum Size
    {
        None,
        ExtraSmall,
        Small,
        Medium,
        Large,
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
        None,
        Is1,
        Is2,
        Is3,
        Is4,
        Is5,
        Is6,
        Is7,
        Is8,
        Is9,
        Is10,
        Is11,
        Is12,
        Full,
        Half,
        Third,
        Quarter,
        Auto,
    }

    public enum DropdownDirection
    {
        Down,
        Up,
        Right,
        Left,
    }

    public enum Side
    {
        None,
        Top,
        Bottom,
        Left,
        Right,
        X,
        Y,
        All,
    }

    public enum Float
    {
        None,
        Left,
        Right,
    }

    public enum Spacing
    {
        None,
        Margin,
        Padding,
    }

    public enum SpacingSize
    {
        Is0,
        Is1,
        Is2,
        Is3,
        Is4,
        Is5,
        IsAuto,
    }

    /// <summary>
    /// Defines the behaviour of the text edit.
    /// </summary>
    public enum TextRole
    {
        Text,
        Number,
        Email,
        Password,
        Url,
    }

    public enum DrawerType
    {
        Default,
        Permanent,
        Persistent,
        Temporary,
    }

    public enum NavFillType
    {
        None,
        Default,
        Justified,
    }

    public enum Visibility
    {
        Default,
        Always,
        Never,
    }

    public enum Alignment
    {
        None,
        Near,
        Center,
        Far,
    }

    public enum TextAlignment
    {
        None,
        Near,
        Center,
        Far,
        Justified
    }

    public enum TextTransform
    {
        None,
        Lowercase,
        Uppercase,
        Capitalize,
    }

    public enum TextWeight
    {
        None,
        Normal,
        Bold,
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

    public enum AddonType
    {
        Body,
        Start,
        End,
    }

    public enum ControlRole
    {
        None,
        Check,
        Radio,
        File,
        Text,
    }

    public enum ModalSize
    {
        None,
        Default,
        Small,
        Large,
    }

    public enum ButtonsRole
    {
        Addons,
        Toolbar,
    }

    public enum Orientation
    {
        Horizontal,
        Vertical,
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

    public enum JustifyContent
    {
        None,
        Start,
        End,
        Center,
        Between,
        Around,
    }
}
