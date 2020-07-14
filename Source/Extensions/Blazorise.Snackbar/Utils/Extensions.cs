using System;
using System.Collections.Generic;
using System.Text;

namespace Blazorise.Snackbar.Utils
{
    static class Extensions
    {
        public static string GetName( this SnackbarLocation snackbarLocation )
        {
            switch ( snackbarLocation )
            {
                case SnackbarLocation.Left:
                    return "left";
                case SnackbarLocation.Right:
                    return "right";
                case SnackbarLocation.None:
                default:
                    return null;
            }
        }

        public static string GetName( this SnackbarStackLocation snackbarStackLocation )
        {
            switch ( snackbarStackLocation )
            {
                case SnackbarStackLocation.Left:
                    return "left";
                case SnackbarStackLocation.Right:
                    return "right";
                case SnackbarStackLocation.Center:
                default:
                    return "center";
            }
        }

        public static string GetName( this SnackbarColor snackbarColor )
        {
            switch ( snackbarColor )
            {
                case SnackbarColor.Primary:
                    return "primary";
                case SnackbarColor.Secondary:
                    return "secondary";
                case SnackbarColor.Success:
                    return "success";
                case SnackbarColor.Danger:
                    return "danger";
                case SnackbarColor.Warning:
                    return "warning";
                case SnackbarColor.Info:
                    return "info";
                case SnackbarColor.Light:
                    return "light";
                case SnackbarColor.Dark:
                    return "dark";
                case SnackbarColor.None:
                default:
                    return null;
            }
        }
    }
}
