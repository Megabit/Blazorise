#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class FileEdit : BaseInputComponent<string[]>
    {
        #region Members

        private bool multiple;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.FileEdit() );
            builder.Append( ClassProvider.FileEditValidation( ParentValidation?.Status ?? ValidationStatus.None ), ParentValidation?.Status != ValidationStatus.None );

            base.BuildClasses( builder );
        }

        protected Task OnChangeHandler( ChangeEventArgs e )
        {
            return CurrentValueHandler( e?.Value?.ToString() );
        }

        protected override Task OnInternalValueChanged( string[] value )
        {
            return PathChanged.InvokeAsync( value );
        }

        protected override async Task<ParseValue<string[]>> ParseValueFromStringAsync( string value )
        {
            if ( Multiple )
            {
                var multipleValues = await JSRunner.GetFilePaths( ElementRef );

                return new ParseValue<string[]>( true, multipleValues, null );
            }
            else
            {
                return new ParseValue<string[]>( true, new string[] { value?.ToString() }, null );
            }
        }

        #endregion

        #region Properties

        protected override string[] InternalValue
        {
            get => null;
            set
            {
                // TODO
            }
        }

        /// <summary>
        /// Enables the multiple file selection.
        /// </summary>
        [Parameter]
        public bool Multiple
        {
            get => multiple;
            set
            {
                multiple = value;

                DirtyClasses();
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
        [Parameter] public EventCallback<string[]> PathChanged { get; set; }

        #endregion
    }
}
