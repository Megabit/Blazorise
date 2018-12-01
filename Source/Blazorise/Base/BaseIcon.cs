#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Blazor;
using Microsoft.AspNetCore.Blazor.Components;
#endregion

namespace Blazorise.Base
{
    public abstract class BaseIcon : BaseComponent
    {
        #region Members

        private object name;

        #endregion

        #region Methods

        protected override void RegisterClasses()
        {
            ClassMapper
                .Add( () => IconProvider.Icon() )
                .If( () => IconProvider.Get( (IconName)Name ), () => !IconProvider.IconNameAsContent && Name != null && Name is IconName )
                .If( () => IconProvider.Get( (string)Name ), () => !IconProvider.IconNameAsContent && Name != null && Name is string );

            base.RegisterClasses();
        }

        #endregion

        #region Properties

        [Inject]
        protected IIconProvider IconProvider { get; set; }

        [Parameter]
        protected object Name
        {
            get => name;
            set
            {
                name = value;

                ClassMapper.Dirty();
            }
        }

        #endregion
    }
}
