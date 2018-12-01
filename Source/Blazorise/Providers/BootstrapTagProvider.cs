#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Blazorise.Providers
{
    class BootstrapTagProvider : ITagProvider
    {
        #region Addon

        public bool AddonInsideContainer( AddonType addonType ) => addonType != AddonType.Body;

        #endregion

        #region Bar

        public string BarItem() => "div";

        public string BarLink() => "a";

        public string BarDropdown() => "div";

        public int BarTogglerSpanCount => 1;

        #endregion

        #region Button

        public bool ButtonInsideContainerWhenAddon => false;

        #endregion

        #region CardSubtitle

        public string CardSubtitle( int size ) => $"h{size}";

        #endregion

        #region Check

        public bool CheckInsideLabel => false;

        #endregion

        #region CloseButton

        public bool UseSpanForCloseButtonIcon => true;

        #endregion

        #region Dropdown

        public string DropdownDivider() => "div";

        public bool DropdownMenuHasBody => false;

        #endregion

        #region File

        public bool FileInsideLabel => false;

        #endregion

        #region Fields

        public bool FieldsLabelIsInside => false;

        public bool FieldsHasBody => false;

        #endregion

        #region Modal

        public bool ModalUseStyleForShow => true;

        #endregion

        #region ModalContent

        public bool ModalContentInsideContainer => true;

        #endregion

        #region Select

        public bool SelectInsideContainer => false;

        #endregion

        #region Tabs

        public bool TabsClassOnContainer => false;

        public bool TabsListInsideContainer => false;

        #endregion

        [Obsolete]
        public string DrawerMenuLink() => "a";
    }
}
