#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            { IconName.Edit, "create" }, // wtf google!? create??
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
        };

        #endregion

        #region Constructors

        #endregion

        #region Methods

        public override string Icon() => "material-icons";

        public override string Get( IconName iconName )
        {
            names.TryGetValue( iconName, out var value );

            return value;
        }

        public override void Set( IconName name, string newName )
        {
            names[name] = newName;
        }

        #endregion

        #region Properties

        public override bool IconNameAsContent => true;

        #endregion
    }
}
