#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Blazorise.AntDesign
{
    public partial class Tabs : Blazorise.Tabs
    {
        #region Members

        #endregion

        #region Methods

        protected override void OnAfterRender( bool firstRender )
        {
            if ( firstRender )
            {
                // All child tab items and panels must be loaded before we can be sure that we can refresh state.
                // This is needed because selected tab will not work unless we know all properly initialized.
                if ( TabItems.Count > 0 && TabItems.Count == TabPanels.Count )
                {
                    StateHasChanged();
                }
            }

            base.OnAfterRender( firstRender );
        }

        #endregion

        #region Properties

        string TabsBarClassNames
            => $"ant-tabs-bar {ItemsPositionClassNames} {( IsCards && TabPosition == TabPosition.Top || TabPosition == TabPosition.Bottom ? "ant-tabs-card-bar" : "" )}";

        protected string ItemsPositionClassNames
        {
            get
            {
                switch ( TabPosition )
                {
                    case TabPosition.Left:
                        return "ant-tabs-left-bar";
                    case TabPosition.Right:
                        return "ant-tabs-right-bar";
                    case TabPosition.Bottom:
                        return "ant-tabs-bottom-bar";
                    case TabPosition.Top:
                    default:
                        return "ant-tabs-top-bar";
                }
            }
        }

        protected string ContentPositionClassNames
        {
            get
            {
                switch ( TabPosition )
                {
                    case TabPosition.Left:
                        return "ant-tabs-left-content";
                    case TabPosition.Right:
                        return "ant-tabs-right-content";
                    case TabPosition.Bottom:
                        return "ant-tabs-bottom-content";
                    case TabPosition.Top:
                    default:
                        return "ant-tabs-top-content";
                }
            }
        }

        protected string StyleOfSelectedTab
        {
            get
            {
                var negativeIndex = IndexOfSelectedTab > 0
                    ? IndexOfSelectedTab * -100
                    : 0;

                var margin = TabPosition == TabPosition.Left || TabPosition == TabPosition.Right
                    ? "margin-top"
                    : "margin-left";

                return $"{margin}: {negativeIndex}%;";
            }
        }

        #endregion
    }
}
