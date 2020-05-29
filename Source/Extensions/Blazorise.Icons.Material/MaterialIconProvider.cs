#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using Blazorise.Providers;
#endregion

namespace Blazorise.Icons.Material
{
    class MaterialIconProvider : BaseIconProvider
    {
        #region Members

        private static Dictionary<IconName, string> names = new Dictionary<IconName, string>
        {
            { IconName.New, "add" },
            { IconName.Edit, "create" },
            { IconName.Save, "save" },
            { IconName.Cancel, "cancel" },
            { IconName.Delete, "delete" },
            { IconName.Clear, "clear" },
            { IconName.Search, "search" },
            { IconName.ClearSearch, "clear" },
            { IconName.Phone, "phone" },
            { IconName.Smartphone, "smartphone" },
            { IconName.Mail, "mail" },
            { IconName.Person, "person" },
            { IconName.Lock, "lock" },
            { IconName.MoreHorizontal, "more_horiz" },
            { IconName.MoreVertical, "more_vert" },
            { IconName.ExpandMore, "expand_more" },
            { IconName.ExpandLess, "expand_less" },
            { IconName.SliderHorizontal, null },
            { IconName.SliderVertical, null},
            { IconName.Dashboard, "dashboard" },
            { IconName.Tint, "invert_colors" },
            { IconName.Palette, "palette" },
            { IconName.SortUp, "arrow_drop_up" },
            { IconName.SortDown, "arrow_drop_down" },
            { IconName.Bold, "format_bold" },
            { IconName.Italic, "format_italic" },
            { IconName.Underline, "format_underline" },
            { IconName.StrikeThrough, "format_strikethrough" },
            { IconName.Quote, "format_quote" },
            { IconName.Code, "code" },
            { IconName.Heading, "title" },
            { IconName.Indent, "format_indent_increase" },
            { IconName.FontSize, "format_size" },
            { IconName.AlignLeft, "format_align_left" },
            { IconName.AlignRight, "format_align_right" },
            { IconName.AlignCenter, "format_align_center" },
            { IconName.RemoveFormat, "format_clear" },
            { IconName.SuperScript, "superscript" },
            { IconName.SubScript, "subscript" },
            { IconName.Image, "insert_photo" },
            { IconName.Link, "link" },
            { IconName.UnorderedList, "format_list_bulleted" },
            { IconName.OrderedList, "format_list_numbered" },
            { IconName.Help, "help" },
        };

        private static Dictionary<IconStyle, string> styles = new Dictionary<IconStyle, string>
        {
            { IconStyle.Solid, "material-icons" },
            { IconStyle.Regular, "material-icons-outlined" },
            { IconStyle.Light, "material-icons-sharp" }, // TODO: probably not correct
            { IconStyle.DuoTone, "material-icons-two-tone" },
        };

        #endregion

        #region Constructors

        #endregion

        #region Methods

        public override string GetIconName( IconName iconName )
        {
            names.TryGetValue( iconName, out var value );

            return value;
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
            return iconName.Split( ' ' ).Any( x => styles.Values.Contains( x ) || new string[] { "material-icons-round" }.Contains( x ) );
        }

        #endregion

        #region Properties

        public override bool IconNameAsContent => true;

        #endregion
    }
}
