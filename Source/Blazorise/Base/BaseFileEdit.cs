#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Base
{
    public abstract class BaseFileEdit : BaseInputComponent
    {
        #region Members

        private bool isMultiple;

        #endregion

        #region Methods

        protected override void RegisterClasses()
        {
            ClassMapper
                .Add( () => ClassProvider.File() );

            base.RegisterClasses();
        }

        protected async void PathChangedHandler( UIChangeEventArgs e )
        {
            if ( IsMultiple )
            {
                var files = await JSRunner.GetFilePaths( ElementRef );

                PathChanged?.Invoke( files );
            }
            else
            {
                PathChanged?.Invoke( new string[] { e?.Value?.ToString() } );
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Enables the multiple file selection.
        /// </summary>
        [Parameter]
        protected bool IsMultiple
        {
            get => isMultiple;
            set
            {
                isMultiple = value;

                ClassMapper.Dirty();
            }
        }

        /// <summary>
        /// Specifies the types of files that the input accepts.
        /// </summary>
        /// <see cref="https://www.w3schools.com/tags/att_input_accept.asp"/>
        [Parameter] protected string Filter { get; set; }

        /// <summary>
        /// Occurs when the file path is changed.
        /// </summary>
        [Parameter] private Action<string[]> PathChanged { get; set; }

        #endregion
    }
}
