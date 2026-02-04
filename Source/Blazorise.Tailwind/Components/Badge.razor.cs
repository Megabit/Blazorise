using Blazorise.Utilities;

namespace Blazorise.Tailwind.Components
{
    public partial class Badge
    {
        #region Constructors

        public Badge()
        {
            CloseButtonClassBuilder = new ClassBuilder( BuildCloseButtonClasses, builder => builder.Append( Classes?.Close ) );
        }

        #endregion

        #region Methods

        protected internal override void DirtyClasses()
        {
            CloseButtonClassBuilder.Dirty();

            base.DirtyClasses();
        }

        private void BuildCloseButtonClasses( ClassBuilder builder )
        {
            builder.Append( "inline-flex items-center p-0.5 ml-2 rounded-sm" );

            builder.Append( Color?.Name switch
            {
                "primary" => "bg-blue-100 text-blue-500 focus:ring-2 focus:ring-blue-400 hover:bg-blue-200 dark:bg-blue-200 dark:text-blue-600 dark:hover:bg-blue-300",
                "secondary" => "bg-secondary-100 text-secondary-500 focus:ring-2 focus:ring-secondary-400 hover:bg-secondary-200 dark:bg-secondary-200 dark:text-secondary-600 dark:hover:bg-secondary-300",
                "success" => "bg-green-100 text-green-500 focus:ring-2 focus:ring-green-400 hover:bg-green-200 dark:bg-green-200 dark:text-green-600 dark:hover:bg-green-300",
                "danger" => "bg-red-100 text-red-500 focus:ring-2 focus:ring-red-400 hover:bg-red-200 dark:bg-red-200 dark:text-red-600 dark:hover:bg-red-300",
                "warning" => "bg-yellow-100 text-yellow-500 focus:ring-2 focus:ring-yellow-400 hover:bg-yellow-200 dark:bg-yellow-200 dark:text-yellow-600 dark:hover:bg-yellow-300",
                "info" => "bg-info-100 text-info-500 focus:ring-2 focus:ring-info-400 hover:bg-info-200 dark:bg-info-200 dark:text-info-600 dark:hover:bg-info-300",
                "light" => "bg-light-100 text-light-500 focus:ring-2 focus:ring-light-400 hover:bg-light-200 dark:bg-light-200 dark:text-light-600 dark:hover:bg-light-300",
                "dark" => "bg-dark-800 text-dark-100 focus:ring-2 focus:ring-dark-400 hover:bg-dark-700 dark:bg-dark-800 dark:text-dark-400 dark:hover:bg-dark-700",
                "link" => "text-primary-600 dark:text-primary-500 hover:underline",
                _ => "text-gray-400 bg-transparent hover:text-gray-900 ml-auto dark:hover:text-white",
            } );
        }

        #endregion

        #region Properties

        protected ClassBuilder CloseButtonClassBuilder { get; private set; }

        protected string CloseButtonClassNames => CloseButtonClassBuilder.Class;

        #endregion
    }
}