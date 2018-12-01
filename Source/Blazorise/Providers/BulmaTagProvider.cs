#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Blazorise.Providers
{
    class BulmaTagProvider : ITagProvider
    {
        #region Addon

        public bool AddonInsideContainer( AddonType addonType ) => true;

        #endregion

        #region Bar

        public string BarItem() => "div";

        public string BarLink() => "a";

        public string BarDropdown() => "div";

        public int BarTogglerSpanCount => 3;

        #endregion

        #region Button

        public bool ButtonInsideContainerWhenAddon => true;

        #endregion

        #region CardSubtitle

        public string CardSubtitle( int size ) => "p";

        #endregion

        #region Check

        public bool CheckInsideLabel => true;

        #endregion

        #region CloseButton

        public bool UseSpanForCloseButtonIcon => false;

        #endregion

        #region Dropdown

        public string DropdownDivider() => "hr";

        public bool DropdownMenuHasBody => true;

        #endregion

        #region File

        public bool FileInsideLabel => true;

        #endregion

        #region Fields

        public bool FieldsLabelIsInside => true;

        public bool FieldsHasBody => true;

        #endregion

        #region Modal

        public bool ModalUseStyleForShow => false;

        #endregion

        #region ModalContent

        public bool ModalContentInsideContainer => false;

        #endregion

        #region Select

        public bool SelectInsideContainer => true;

        #endregion

        #region Tabs

        public bool TabsClassOnContainer => true;

        public bool TabsListInsideContainer => true;

        #endregion

        [Obsolete]
        public string DrawerMenuLink() => "a";
    }
}
