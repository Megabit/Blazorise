#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        };

        #endregion

        #region Constructors

        #endregion

        #region Methods

        public override string Icon() => "fas";

        public override string Get( IconName iconName )
        {
            names.TryGetValue( iconName, out var name );

            return name;
        }

        public override void Set( IconName name, string newName )
        {
            names[name] = newName;
        }

        #endregion

        #region Properties

        public override bool IconNameAsContent => false;

        #endregion
    }
}
