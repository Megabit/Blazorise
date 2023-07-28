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
                return "top-left";
            case SnackbarLocation.TopEnd:
                return "top-right";
            case SnackbarLocation.Start:
            case SnackbarLocation.BottomStart:
                return "bottom-left";
            case SnackbarLocation.End:
            case SnackbarLocation.BottomEnd:
                return "bottom-right";
            case SnackbarLocation.Default:
            case SnackbarLocation.Bottom:
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
                return "top-left";
            case SnackbarStackLocation.TopEnd:
                return "top-right";
            case SnackbarStackLocation.Start:
            case SnackbarStackLocation.BottomStart:
                return "bottom-left";
            case SnackbarStackLocation.End:
            case SnackbarStackLocation.BottomEnd:
                return "bottom-right";
            case SnackbarStackLocation.Center:
            case SnackbarStackLocation.Bottom:
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