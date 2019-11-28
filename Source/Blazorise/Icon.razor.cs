#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public abstract class BaseIcon : BaseComponent
    {
        #region Members

        private object name;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( IconProvider.Icon( Name ) );

            if ( !IconProvider.IconNameAsContent && Name != null && Name is IconName )
                builder.Append( IconProvider.Get( (IconName)Name ) );

            if ( !IconProvider.IconNameAsContent && Name != null && Name is string )
                builder.Append( IconProvider.Get( (string)Name ) );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        [Inject]
        protected IIconProvider IconProvider { get; set; }

        [Parameter]
        public object Name
        {
            get => name;
            set
            {
                name = value;

                DirtyClasses();
            }
        }

        #endregion
    }
}
