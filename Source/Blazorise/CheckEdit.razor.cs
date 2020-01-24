#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class CheckEdit : BaseInputComponent<bool?>
    {
        #region Members

        private string radioGroup;

        private bool isInline;

        private Cursor cursor;

        #endregion

        #region Methods

        protected override void OnInitialized()
        {
            if ( ParentValidation != null )
            {
                if ( CheckedExpression != null )
                    ParentValidation.InitializeInputExpression( CheckedExpression );
                else if ( NullableCheckedExpression != null )
                    ParentValidation.InitializeInputExpression( NullableCheckedExpression );
            }

            base.OnInitialized();
        }

        protected override void BuildClasses( ClassBuilder builder )
        {
            if ( RadioGroup != null )
                builder.Append( ClassProvider.RadioEdit() );
            else
                builder.Append( ClassProvider.CheckEdit() );

            builder.Append( ClassProvider.CheckEditCursor( Cursor ), Cursor != Cursor.Default );
            builder.Append( ClassProvider.CheckEditValidation( ParentValidation?.Status ?? ValidationStatus.None ), ParentValidation?.Status != ValidationStatus.None );

            base.BuildClasses( builder );
        }

        protected Task OnChangeHandler( ChangeEventArgs e )
        {
            return CurrentValueHandler( e?.Value?.ToString() );
        }

        protected override Task<ParseValue<bool?>> ParseValueFromStringAsync( string value )
        {
            var parsedValue = value?.ToLowerInvariant() == ( RadioGroup != null ? "on" : "true" );

            return Task.FromResult( new ParseValue<bool?>( true, parsedValue, null ) );
        }

        protected override Task OnInternalValueChanged( bool? value )
        {
            return Task.WhenAll(
                CheckedChanged.InvokeAsync( Checked ),
                NullableCheckedChanged.InvokeAsync( NullableChecked ) );
        }

        #endregion

        #region Properties

        protected ControlRole Role => RadioGroup != null ? ControlRole.Radio : ControlRole.Check;

        protected string Type => RadioGroup != null ? "radio" : "checkbox";

        protected override bool? InternalValue { get => Checked; set => Checked = value ?? false; }

        /// <summary>
        /// Gets or sets the checked flag.
        /// </summary>
        [Parameter] public bool Checked { get; set; }

        /// <summary>
        /// Gets or sets the nullable value for checked flag.
        /// </summary>
        [Parameter] public bool? NullableChecked { get; set; }

        /// <summary>
        /// Occurs when the check state is changed.
        /// </summary>
        [Parameter] public EventCallback<bool> CheckedChanged { get; set; }

        /// <summary>
        /// Occurs when the check state of nullable value is changed.
        /// </summary>
        [Obsolete( "This parameter is only temporary until the issue with generic componnets is fixed. see http://git.travelsoft.hr/Travelsoft/_git/Adriagate/pullrequest/59?_a=overview" )]
        [Parameter] public EventCallback<bool?> NullableCheckedChanged { get; set; }

        /// <summary>
        /// Gets or sets an expression that identifies the checked value.
        /// </summary>
        [Parameter] public Expression<Func<bool>> CheckedExpression { get; set; }

        /// <summary>
        /// Gets or sets an expression that identifies the nullable checked value.
        /// </summary>
        [Parameter] public Expression<Func<bool?>> NullableCheckedExpression { get; set; }

        /// <summary>
        /// Sets the radio group name.
        /// </summary>
        [Parameter]
        public string RadioGroup
        {
            get => radioGroup;
            set
            {
                radioGroup = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Group checkboxes or radios on the same horizontal row.
        /// </summary>
        [Parameter]
        public bool IsInline
        {
            get => isInline;
            set
            {
                isInline = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Defines the mouse cursor based on the behaviour by the current css framework.
        /// </summary>
        [Parameter]
        public Cursor Cursor
        {
            get => cursor;
            set
            {
                cursor = value;

                DirtyClasses();
            }
        }

        #endregion
    }
}
