#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public abstract class BaseAddon : BaseComponent
    {
        #region Members

        private AddonType addonType = AddonType.Body;

        #endregion

        #region Methods

        protected override void RegisterClasses()
        {
            ClassMapper
                .Add( () => ClassProvider.Addon( AddonType ) );

            base.RegisterClasses();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Defines the location and behaviour of addon container.
        /// </summary>
        [Parameter]
        protected AddonType AddonType
        {
            get => addonType;
            set
            {
                addonType = value;

                ClassMapper.Dirty();
            }
        }

        [Parameter] protected RenderFragment ChildContent { get; set; }

        #endregion
    }
}
