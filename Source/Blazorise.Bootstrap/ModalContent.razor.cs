#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Blazorise.Bootstrap.BootstrapBase
{
    public abstract class BaseBootstrapModalContent : BaseModalContent
    {
        #region Members

        private string dialogClassNames;

        private bool dialogDirtyClasses = true;

        #endregion

        #region Methods

        protected override void DirtyClasses()
        {
            dialogDirtyClasses = true;

            base.DirtyClasses();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets dialog container classname.
        /// </summary>
        protected string DialogClassNames
        {
            get
            {
                if ( dialogDirtyClasses )
                {
                    var classBuilder = new ClassBuilder();

                    classBuilder.Append( $"modal-dialog {ClassProvider.ToModalSize( Size )}" );
                    classBuilder.Append( ClassProvider.ModalContentCentered(), IsCentered );

                    dialogClassNames = classBuilder.Value?.TrimEnd();

                    dialogDirtyClasses = false;
                }

                return dialogClassNames;
            }
        }

        #endregion
    }
}
