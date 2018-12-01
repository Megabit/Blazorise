//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace Blazorise
//{
//    public interface ITagProvider
//    {
//        #region Addon

//        bool AddonInsideContainer( AddonType addonType );

//        #endregion

//        #region Bar

//        string BarItem();

//        string BarLink();

//        string BarDropdown();

//        int BarTogglerSpanCount { get; }

//        #endregion

//        #region Button

//        bool ButtonInsideContainerWhenAddon { get; }

//        #endregion

//        #region CardSubtitle

//        string CardSubtitle( int size );

//        #endregion

//        #region Check

//        bool CheckInsideLabel { get; }

//        #endregion

//        #region CloseButton

//        bool UseSpanForCloseButtonIcon { get; }

//        #endregion

//        #region Dropdown

//        string DropdownDivider();

//        bool DropdownMenuHasBody { get; }

//        #endregion

//        #region File

//        bool FileInsideLabel { get; }

//        #endregion

//        #region Fields

//        bool FieldsLabelIsInside { get; }

//        bool FieldsHasBody { get; }

//        #endregion

//        #region Modal

//        bool ModalUseStyleForShow { get; }

//        #endregion

//        #region ModalContent

//        bool ModalContentInsideContainer { get; }

//        #endregion

//        #region Select

//        bool SelectInsideContainer { get; }

//        #endregion

//        #region Tabs

//        bool TabsClassOnContainer { get; }

//        bool TabsListInsideContainer { get; }

//        #endregion

//        [Obsolete]
//        string DrawerMenuLink();
//    }
//}
