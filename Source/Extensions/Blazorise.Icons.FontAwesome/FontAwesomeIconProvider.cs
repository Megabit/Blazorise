#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using Blazorise.Providers;
#endregion

namespace Blazorise.Icons.FontAwesome
{
    class FontAwesomeIconProvider : BaseIconProvider
    {
        #region Members

        private static Dictionary<IconName, string> names = new Dictionary<IconName, string>
        {
            { IconName.New, "fa-plus" },
            { IconName.Edit, "fa-edit" },
            { IconName.Save, "fa-save" },
            { IconName.Cancel, "fa-ban" },
            { IconName.Delete, "fa-trash" },
            { IconName.Clear, "fa-eraser" },
            { IconName.Search, "fa-search" },
            { IconName.ClearSearch, "fa-minus-circle" },
            { IconName.Phone, "fa-phone" },
            { IconName.Smartphone, "fa-mobile" },
            { IconName.Mail, "fa-envelope" },
            { IconName.Person, "fa-user" },
            { IconName.Lock, "fa-lock" },
            { IconName.MoreHorizontal, "fa-ellipsis-h" },
            { IconName.MoreVertical, "fa-ellipsis-v" },
            { IconName.ExpandMore, "fa-chevron-down" },
            { IconName.ExpandLess, "fa-chevron-up" },
            { IconName.SliderHorizontal, "fa-sliders-h" },
            { IconName.SliderVertical, "fa-sliders-v" },
            { IconName.Dashboard, "fa-columns" }, // find better
            { IconName.Tint, "fa-tint" },
            { IconName.Palette, "fa-palette" },
            { IconName.SortUp, "fa-sort-up" },
            { IconName.SortDown, "fa-sort-down" },
            { IconName.Bold, "fa-bold" },
            { IconName.Italic, "fa-italic" },
            { IconName.Underline, "fa-underline" },
            { IconName.StrikeThrough, "fa-strikethrough" },
            { IconName.Quote, "fa-quote-right" },
            { IconName.Code, "fa-code" },
            { IconName.Heading, "fa-heading" },
            { IconName.Indent, "fa-indent" },
            { IconName.FontSize, "fa-text-height" },
            { IconName.AlignLeft, "fa-align-left" },
            { IconName.AlignRight, "fa-align-right" },
            { IconName.AlignCenter, "fa-align-center" },
            { IconName.RemoveFormat, "fa-remove-format" },
            { IconName.SuperScript, "fa-superscript" },
            { IconName.SubScript, "fa-subscript" },
            { IconName.Image, "fa-image" },
            { IconName.Link, "fa-link" },
            { IconName.UnorderedList, "fa-list-ul" },
            { IconName.OrderedList, "fa-list-ol" },
            { IconName.Help, "fa-question" },
        };

        private static Dictionary<IconStyle, string> styles = new Dictionary<IconStyle, string>
        {
            { IconStyle.Solid, "fas" },
            { IconStyle.Regular, "far" },
            { IconStyle.Light, "fal" },
            { IconStyle.DuoTone, "fad" },
        };

        #endregion

        #region Constructors

        #endregion

        #region Methods

        public override string GetIconName( IconName iconName )
        {
            names.TryGetValue( iconName, out var name );

            return name;
        }

        public override void SetIconName( IconName name, string newName )
        {
            names[name] = newName;
        }

        public override string GetStyleName( IconStyle iconStyle )
        {
            if ( styles.TryGetValue( iconStyle, out var style ) )
                return style;

            return null;
        }

        protected override bool ContainsStyleName( string iconName )
        {
            return iconName.Split( ' ' ).Any( x => styles.Values.Contains( x ) || new string[] { "fab" }.Contains( x ) );
        }

        #endregion

        #region Properties

        public override bool IconNameAsContent => false;

        #endregion
    }
}
