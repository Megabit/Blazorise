#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Tailwind.Components;

public partial class CloseButton
{
    #region Methods

    protected override void BuildClasses( ClassBuilder builder )
    {
        if ( ParentAlert is not null )
        {
            builder.Append( "absolute top-0 right-0 ml-auto mx-1.5 my-1.5 p-1.5 h-8 w-8 rounded-lg" );

            builder.Append( ParentAlert.Color?.Name switch
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
        else if ( ParentBadge is not null )
        {
            builder.Append( "inline-flex items-center p-0.5 ml-2 rounded-sm" );

            builder.Append( ParentBadge.Color?.Name switch
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
        else if ( ParentModal is not null )
        {
            builder.Append( "items-center float-right text-gray-400 bg-transparent hover:text-gray-900 p-1.5 ml-auto dark:hover:text-white rounded-lg" );
        }
        else if ( ParentToast is not null )
        {
            builder.Append( "ms-auto -mx-1.5 -my-1.5 bg-white items-center justify-center flex-shrink-0 text-gray-400 hover:text-gray-900 rounded-lg focus:ring-2 focus:ring-gray-300 p-1.5 hover:bg-gray-100 inline-flex h-8 w-8 dark:text-gray-500 dark:hover:text-white dark:bg-gray-800 dark:hover:bg-gray-700" );
        }
        else
        {
            builder.Append( "items-center text-gray-400 bg-transparent hover:text-gray-900 rounded-lg p-0.5 ml-2 dark:hover:text-white rounded-lg" );
        }

        base.BuildClasses( builder );
    }

    #endregion

    #region Properties

    string SvgSize => ParentBadge is not null
        ? "w-3.5 h-3.5"
        : "w-5 h-5";

    /// <summary>
    /// Cascaded <see cref="Badge"/> component in which this <see cref="CloseButton"/> is placed.
    /// </summary>
    [CascadingParameter] protected Badge ParentBadge { get; set; }

    #endregion
}
