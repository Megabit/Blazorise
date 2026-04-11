#region Using directives
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Blazorise.Providers;
#endregion

namespace Blazorise.Icons.AntDesign;

class AntDesignIconProvider : BaseIconProvider
{
    #region Members

    private static readonly Dictionary<IconName, string> names = new()
    {
        { IconName.Add, "plus" },
        { IconName.Adjust, "control" },
        { IconName.Alert, "bell" },
        { IconName.AlertOff, "bell" },
        { IconName.AngleDown, "down" },
        { IconName.AngleLeft, "left" },
        { IconName.AngleRight, "right" },
        { IconName.AngleUp, "up" },
        { IconName.ArrowAltCircleDown, "down-circle" },
        { IconName.Bars, "menu" },
        { IconName.BirthdayCake, "gift" },
        { IconName.Bolt, "thunderbolt" },
        { IconName.BorderNone, "border" },
        { IconName.CalendarTimes, "calendar" },
        { IconName.Cart, "shopping-cart" },
        { IconName.CartMinus, "shopping-cart" },
        { IconName.CartPlus, "shopping-cart" },
        { IconName.ChartArea, "area-chart" },
        { IconName.ChartBar, "bar-chart" },
        { IconName.ChartLine, "line-chart" },
        { IconName.ChartPie, "pie-chart" },
        { IconName.CheckDouble, "check" },
        { IconName.Clear, "clear" },
        { IconName.Clipboard, "copy" },
        { IconName.Clock, "clock-circle" },
        { IconName.ClosedCaptioning, "file-text" },
        { IconName.CloudDownloadAlt, "cloud-download" },
        { IconName.CloudUploadAlt, "cloud-upload" },
        { IconName.Cocktail, "coffee" },
        { IconName.CommentAlt, "message" },
        { IconName.Comments, "wechat" },
        { IconName.CompactDisc, "customer-service" },
        { IconName.Copyright, "copyright-circle" },
        { IconName.Cut, "scissor" },
        { IconName.Delete, "delete" },
        { IconName.Desktop, "desktop" },
        { IconName.DollarSign, "dollar" },
        { IconName.DotCircle, "aim" },
        { IconName.Edit, "edit" },
        { IconName.EuroSign, "euro" },
        { IconName.ExclamationCircle, "exclamation-circle" },
        { IconName.ExclamationTriangle, "warning" },
        { IconName.Expand, "fullscreen" },
        { IconName.ExpandArrowsAlt, "fullscreen" },
        { IconName.ExpandLess, "up" },
        { IconName.ExpandMore, "down" },
        { IconName.ExternalLinkSquareAlt, "export" },
        { IconName.EyeSlash, "eye-invisible" },
        { IconName.FastBackward, "fast-backward" },
        { IconName.FastForward, "fast-forward" },
        { IconName.FileAlt, "file-text" },
        { IconName.FileDownload, "file" },
        { IconName.FileUpload, "file" },
        { IconName.FolderPlus, "folder-add" },
        { IconName.FrownOpen, "frown" },
        { IconName.Gamepad, "play-square" },
        { IconName.Grin, "smile" },
        { IconName.GripLines, "drag" },
        { IconName.GripVertical, "drag" },
        { IconName.History, "history" },
        { IconName.IdCard, "idcard" },
        { IconName.Info, "info" },
        { IconName.InfoCircle, "info-circle" },
        { IconName.Language, "translation" },
        { IconName.LayerGroup, "block" },
        { IconName.Lightbulb, "bulb" },
        { IconName.LockOpen, "unlock" },
        { IconName.Mail, "mail" },
        { IconName.MailOpen, "mail" },
        { IconName.MapMarker, "environment" },
        { IconName.MapMarkerAlt, "environment" },
        { IconName.Microphone, "audio" },
        { IconName.MicrophoneSlash, "audio-muted" },
        { IconName.MinusCircle, "minus-circle" },
        { IconName.MinusSquare, "minus-square" },
        { IconName.MoneyBillAlt, "money-collect" },
        { IconName.MoreHorizontal, "ellipsis" },
        { IconName.MoreVertical, "more" },
        { IconName.Paperclip, "paper-clip" },
        { IconName.PaperPlane, "send" },
        { IconName.Paste, "copy" },
        { IconName.PauseCircle, "pause-circle" },
        { IconName.PhoneAlt, "phone" },
        { IconName.PlayCircle, "play-circle" },
        { IconName.PlusCircle, "plus-circle" },
        { IconName.PlusSquare, "plus-square" },
        { IconName.Poll, "fund" },
        { IconName.QuestionCircle, "question-circle" },
        { IconName.QuoteRight, "message" },
        { IconName.Random, "swap" },
        { IconName.Redo, "redo" },
        { IconName.Remove, "minus" },
        { IconName.Reply, "rollback" },
        { IconName.ReplyAll, "rollback" },
        { IconName.Rss, "rss" },
        { IconName.Save, "save" },
        { IconName.SearchMinus, "zoom-out" },
        { IconName.SearchPlus, "zoom-in" },
        { IconName.Settings, "setting" },
        { IconName.ShareAlt, "share-alt" },
        { IconName.ShieldAlt, "safety" },
        { IconName.ShoppingBag, "shopping" },
        { IconName.ShoppingBasket, "shopping" },
        { IconName.ShoppingCart, "shopping-cart" },
        { IconName.SliderHorizontal, "sliders" },
        { IconName.Sms, "message" },
        { IconName.Sort, "sort-ascending" },
        { IconName.SortAlphaDown, "sort-descending" },
        { IconName.SortAlphaUp, "sort-ascending" },
        { IconName.SortAmountDownAlt, "sort-descending" },
        { IconName.SortDown, "sort-descending" },
        { IconName.SortUp, "sort-ascending" },
        { IconName.StepBackward, "step-backward" },
        { IconName.StepForward, "step-forward" },
        { IconName.StickyNote, "file-text" },
        { IconName.Stream, "partition" },
        { IconName.SyncAlt, "sync" },
        { IconName.Times, "close" },
        { IconName.TimesCircle, "close-circle" },
        { IconName.Undo, "undo" },
        { IconName.UserFriends, "team" },
        { IconName.UserPlus, "user-add" },
        { IconName.Users, "team" },
        { IconName.VideoSlash, "video-camera" },
        { IconName.VolumeMute, "sound" },
        { IconName.VolumeOff, "sound" },
        { IconName.VolumeUp, "sound" },
        { IconName.Wifi, "wifi" },
        { IconName.Zoom, "search" },
        { IconName.ZoomIn, "zoom-in" },
        { IconName.ZoomOut, "zoom-out" },
    };

    private static readonly string[] styleNames = new[]
    {
        "anticon",
    };

    #endregion

    #region Methods

    public override string IconSize( IconSize iconSize )
    {
        return iconSize switch
        {
            Blazorise.IconSize.ExtraSmall => "anticon-xs",
            Blazorise.IconSize.Small => "anticon-sm",
            Blazorise.IconSize.Large => "anticon-lg",
            Blazorise.IconSize.x2 => "anticon-2x",
            Blazorise.IconSize.x3 => "anticon-3x",
            Blazorise.IconSize.x4 => "anticon-4x",
            Blazorise.IconSize.x5 => "anticon-5x",
            Blazorise.IconSize.x6 => "anticon-6x",
            Blazorise.IconSize.x7 => "anticon-7x",
            Blazorise.IconSize.x8 => "anticon-8x",
            Blazorise.IconSize.x9 => "anticon-9x",
            Blazorise.IconSize.x10 => "anticon-10x",
            _ => null,
        };
    }

    public override string GetIconName( IconName iconName, IconStyle iconStyle )
    {
        if ( !names.TryGetValue( iconName, out string name ) )
            name = ToKebabCase( iconName.ToString() );

        return GetStyledIconName( name, iconStyle );
    }

    public override void SetIconName( IconName name, string newName )
    {
        names[name] = NormalizeBaseName( newName );
    }

    public override string GetStyleName( IconStyle iconStyle ) => "anticon";

    public string GetSvg( object name, IconStyle iconStyle )
    {
        string iconName = ResolveIconName( name, iconStyle );

        return AntDesignIconSvg.Get( iconName ) ?? string.Empty;
    }

    protected override bool ContainsStyleName( string iconName )
    {
        return iconName.Split( ' ' ).Any( x => styleNames.Contains( x ) );
    }

    private string ResolveIconName( object name, IconStyle iconStyle )
    {
        return name switch
        {
            IconName iconName => GetIconName( iconName, iconStyle ),
            string customName => GetStyledIconName( NormalizeBaseName( customName ), iconStyle ),
            _ => null,
        };
    }

    private static string GetStyledIconName( string name, IconStyle iconStyle )
    {
        if ( name is null )
            return null;

        if ( HasThemeSuffix( name ) )
            return name.StartsWith( "anticon-" ) ? name : $"anticon-{name}";

        string[] themes = iconStyle switch
        {
            IconStyle.Solid => new[] { "filled", "outlined", "twotone" },
            IconStyle.DuoTone => new[] { "twotone", "outlined", "filled" },
            _ => new[] { "outlined", "filled", "twotone" },
        };

        return AntDesignIconSvg.ResolveIconName( name, themes );
    }

    private static string NormalizeBaseName( string name )
    {
        if ( string.IsNullOrWhiteSpace( name ) )
            return null;

        string[] tokens = name.Split( ' ' );
        string iconName = tokens.FirstOrDefault( x => x.StartsWith( "anticon-" ) && x != "anticon" && !x.EndsWith( "x" ) );

        if ( iconName is not null )
            name = iconName;

        return name.StartsWith( "anticon-" )
            ? name["anticon-".Length..]
            : name;
    }

    private static bool HasThemeSuffix( string name )
    {
        return name.EndsWith( "-outlined" )
               || name.EndsWith( "-filled" )
               || name.EndsWith( "-twotone" );
    }

    private static string ToKebabCase( string value )
    {
        StringBuilder builder = new();

        for ( int i = 0; i < value.Length; i++ )
        {
            char current = value[i];

            if ( char.IsUpper( current ) && i > 0 )
                builder.Append( '-' );

            builder.Append( char.ToLowerInvariant( current ) );
        }

        return builder.ToString();
    }

    #endregion

    #region Properties

    public override bool IconNameAsContent => false;

    #endregion
}