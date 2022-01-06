#region Using directives
using Blazorise.Utilities;
#endregion

namespace Blazorise.Bootstrap5
{
    public partial class NumericPicker<TValue> : Blazorise.NumericPicker<TValue>
    {
        #region Constructors

        public NumericPicker()
        {
            NumericWrapperClassBuilder = new( BuildDialogClasses );
        }

        #endregion

        #region Methods

        protected internal override void DirtyClasses()
        {
            NumericWrapperClassBuilder.Dirty();

            base.DirtyClasses();
        }

        private void BuildDialogClasses( ClassBuilder builder )
        {
            builder.Append( "b-numeric" );
            builder.Append( ClassProvider.NumericPickerValidation( ParentValidation?.Status ?? ValidationStatus.None ), ParentValidation?.Status != ValidationStatus.None );
        }

        #endregion

        #region Properties

        protected string WrapperElementId { get; set; }

        protected ClassBuilder NumericWrapperClassBuilder { get; private set; }

        /// <summary>
        /// Gets numeric container class-names.
        /// </summary>
        protected string NumericWrapperClassNames => NumericWrapperClassBuilder.Class;

        #endregion
    }
}
