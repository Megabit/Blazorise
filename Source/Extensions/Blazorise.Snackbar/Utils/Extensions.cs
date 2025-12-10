namespace Blazorise.Snackbar.Utils;

static class Extensions
{
    public static string GetName( this SnackbarLocation snackbarLocation )
    {
        switch ( snackbarLocation )
        {
            case SnackbarLocation.Top:
                return "top";
            case SnackbarLocation.TopStart:
                return "top-start";
            case SnackbarLocation.TopEnd:
                return "top-end";
            case SnackbarLocation.BottomStart:
                return "bottom-start";
            case SnackbarLocation.BottomEnd:
                return "bottom-end";
            case SnackbarLocation.Default:
            case SnackbarLocation.Bottom:
                return "bottom";
            default:
                return null;
        }
    }

    public static string GetName( this SnackbarStackLocation snackbarStackLocation )
    {
        switch ( snackbarStackLocation )
        {
            case SnackbarStackLocation.Top:
                return "top";
            case SnackbarStackLocation.TopStart:
                return "top-start";
            case SnackbarStackLocation.TopEnd:
                return "top-end";
            case SnackbarStackLocation.BottomStart:
                return "bottom-start";
            case SnackbarStackLocation.BottomEnd:
                return "bottom-end";
            case SnackbarStackLocation.Default:
            case SnackbarStackLocation.Bottom:
                return "bottom";
            default:
                return null;
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
            case SnackbarColor.Default:
            default:
                return null;
        }
    }
}