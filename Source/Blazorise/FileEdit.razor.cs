#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public abstract class BaseFileEdit : BaseInputComponent<string[]>
    {
        #region Members

        private bool isMultiple;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.FileEdit() );
            builder.Append( ClassProvider.FileEditValidation( ParentValidation?.Status ?? ValidationStatus.None ), ParentValidation?.Status != ValidationStatus.None );

            base.BuildClasses( builder );
        }

        protected async void PathChangedHandler( UIChangeEventArgs e )
        {
            if ( IsMultiple )
                InternalValue = await JSRunner.GetFilePaths( ElementRef );
            else
                InternalValue = new string[] { e?.Value?.ToString() };

            PathChanged?.Invoke( InternalValue );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Enables the multiple file selection.
        /// </summary>
        [Parameter]
        public bool IsMultiple
        {
            get => isMultiple;
            set
            {
                isMultiple = value;

                Dirty();
            }
        }

        /// <summary>
        /// Specifies the types of files that the input accepts.
        /// </summary>
        /// <see cref="https://www.w3schools.com/tags/att_input_accept.asp"/>
        [Parameter] public string Filter { get; set; }

        /// <summary>
        /// Occurs when the file path is changed.
        /// </summary>
        [Parameter] public Action<string[]> PathChanged { get; set; }

        #endregion
    }
}
