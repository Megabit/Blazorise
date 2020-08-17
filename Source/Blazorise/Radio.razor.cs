#region Using directives
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Utils;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Radio buttons allow the user to select one option from a set.
    /// </summary>
    /// <typeparam name="TValue">Checked value type.</typeparam>
    public partial class Radio<TValue> : BaseCheckComponent<bool>
    {
        #region Members

        private Color color = Color.None;

        private string group;

        #endregion

        #region Methods

        public Radio()
        {
            LabelButtonClassBuilder = new ClassBuilder( BuildLabelButtonClasses );
        }

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.Radio( AsButton ) );
            builder.Append( ClassProvider.RadioSize( AsButton, Size ), Size != Size.None );

            base.BuildClasses( builder );
        }

        private void BuildLabelButtonClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.Button() );
            builder.Append( ClassProvider.ButtonColor( color ), color != Color.None );
        }

        protected override void OnInitialized()
        {
            if ( ParentRadioGroup != null )
            {
                Checked = EqualityComparer<TValue>.Default.Equals( ParentRadioGroup.CheckedValue, Value );

                // TODO: possibly memory leak in Blazor server-side with prerendering mode!
                ParentRadioGroup.RadioCheckedChanged += OnRadioChanged;

                // Parent group name have higher priority!
                if ( string.IsNullOrEmpty( Group ) )
                {
                    Group = ParentRadioGroup.Name;
                }

                color = ParentRadioGroup.Color;
            }

            base.OnInitialized();
        }

        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                if ( ParentRadioGroup != null )
                {
                    ParentRadioGroup.RadioCheckedChanged -= OnRadioChanged;
                }
            }

            base.Dispose( disposing );
        }

        protected override Task OnChangeHandler( ChangeEventArgs e )
        {
            if ( ParentRadioGroup != null )
                return ParentRadioGroup.NotifyRadioChanged( this );

            // Radio should always be inside of RadioGroup or otherwise it's "checked" state will not
            // be activated like it should be. I will leave this just in case that users want to use it
            // but I will need to state in the documentation that it's generally not supported.
            return CurrentValueHandler( e?.Value?.ToString() );
        }

        protected override Task<ParseValue<bool>> ParseValueFromStringAsync( string value )
        {
            return base.ParseValueFromStringAsync( value );
        }

        private async void OnRadioChanged( object sender, RadioCheckedChangedEventArgs<TValue> e )
        {
            await CurrentValueHandler( e?.Value?.ToString() );

            // Some providers like AntDesign need additional changes on classes or styles.
            DirtyClasses();
            DirtyStyles();
        }

        #endregion

        #region Properties

        protected ClassBuilder LabelButtonClassBuilder { get; private set; }

        /// <summary>
        /// Gets the link button class-names.
        /// </summary>
        protected string LabelButtonClassNames => LabelButtonClassBuilder.Class;

        protected override string TrueValueName => Value?.ToString();

        protected bool ParentIsRadioGroup => ParentRadioGroup != null;

        protected bool AsButton => ParentRadioGroup?.Buttons == true;

        /// <summary>
        /// Sets the radio group name.
        /// </summary>
        [Parameter]
        public string Group
        {
            get => group;
            set
            {
                group = value;

                DirtyClasses();
            }
        }

        [Parameter] public TValue Value { get; set; }

        [CascadingParameter] protected RadioGroup<TValue> ParentRadioGroup { get; set; }

        #endregion
    }
}
