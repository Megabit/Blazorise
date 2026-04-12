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
        { IconName.AlignJustify, "align-left" },
        { IconName.AngleDown, "down" },
        { IconName.AngleLeft, "left" },
        { IconName.AngleRight, "right" },
        { IconName.AngleUp, "up" },
        { IconName.Archive, "inbox" },
        { IconName.ArrowAltCircleDown, "down-circle" },
        { IconName.Baby, "smile" },
        { IconName.BabyCarriage, "car" },
        { IconName.Backspace, "rollback" },
        { IconName.BalanceScale, "audit" },
        { IconName.Ban, "stop" },
        { IconName.BandAid, "medicine-box" },
        { IconName.Bars, "menu" },
        { IconName.BatteryFull, "thunderbolt" },
        { IconName.BellSlash, "muted" },
        { IconName.Biking, "man" },
        { IconName.BirthdayCake, "gift" },
        { IconName.Bolt, "thunderbolt" },
        { IconName.BookReader, "read" },
        { IconName.Bookmark, "pushpin" },
        { IconName.BorderAll, "border-outer" },
        { IconName.BorderNone, "border" },
        { IconName.BorderStyle, "border-inner" },
        { IconName.Briefcase, "solution" },
        { IconName.Brush, "format-painter" },
        { IconName.Building, "bank" },
        { IconName.Bus, "car" },
        { IconName.CalendarCheck, "schedule" },
        { IconName.CalendarDay, "calendar" },
        { IconName.CalendarTimes, "calendar" },
        { IconName.CalendarWeek, "calendar" },
        { IconName.CameraRetro, "camera" },
        { IconName.CaretSquareRight, "right-square" },
        { IconName.Cart, "shopping-cart" },
        { IconName.CartMinus, "shopping-cart" },
        { IconName.CartPlus, "shopping-cart" },
        { IconName.Chair, "rest" },
        { IconName.ChartArea, "area-chart" },
        { IconName.ChartBar, "bar-chart" },
        { IconName.ChartLine, "line-chart" },
        { IconName.ChartPie, "pie-chart" },
        { IconName.ChartScatter, "dot-chart" },
        { IconName.CheckDouble, "check" },
        { IconName.ChevronDoubleDown, "down" },
        { IconName.ChevronDoubleLeft, "double-left" },
        { IconName.ChevronDoubleRight, "double-right" },
        { IconName.ChevronDoubleUp, "up" },
        { IconName.ChevronDown, "down" },
        { IconName.ChevronLeft, "left" },
        { IconName.ChevronRight, "right" },
        { IconName.ChevronUp, "up" },
        { IconName.Circle, "loading" },
        { IconName.City, "apartment" },
        { IconName.Clear, "clear" },
        { IconName.ClinicMedical, "medicine-box" },
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
        { IconName.Crop, "scan" },
        { IconName.Cut, "scissor" },
        { IconName.Delete, "delete" },
        { IconName.Desktop, "desktop" },
        { IconName.Dice, "number" },
        { IconName.Directions, "branches" },
        { IconName.Dog, "github" },
        { IconName.DollarSign, "dollar" },
        { IconName.DotCircle, "aim" },
        { IconName.Dumbbell, "trophy" },
        { IconName.Edit, "edit" },
        { IconName.Eject, "upload" },
        { IconName.Ethernet, "cluster" },
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
        { IconName.Film, "video-camera" },
        { IconName.Fingerprint, "security-scan" },
        { IconName.Flask, "experiment" },
        { IconName.FolderPlus, "folder-add" },
        { IconName.FrownOpen, "frown" },
        { IconName.Gamepad, "play-square" },
        { IconName.GasPump, "dashboard" },
        { IconName.Gavel, "audit" },
        { IconName.Globe, "global" },
        { IconName.Grin, "smile" },
        { IconName.GripLines, "drag" },
        { IconName.GripVertical, "drag" },
        { IconName.HandPaper, "stop" },
        { IconName.HandsHelping, "team" },
        { IconName.Headphones, "customer-service" },
        { IconName.Headset, "customer-service" },
        { IconName.Highlighter, "highlight" },
        { IconName.History, "history" },
        { IconName.Hospital, "medicine-box" },
        { IconName.HotTub, "rest" },
        { IconName.Hotel, "home" },
        { IconName.IdCard, "idcard" },
        { IconName.Image, "picture" },
        { IconName.Images, "picture" },
        { IconName.Indent, "menu-fold" },
        { IconName.Infinity, "number" },
        { IconName.Info, "info" },
        { IconName.InfoCircle, "info-circle" },
        { IconName.Keyboard, "number" },
        { IconName.Language, "translation" },
        { IconName.LaptopCode, "laptop" },
        { IconName.Laugh, "smile" },
        { IconName.LayerGroup, "block" },
        { IconName.Lightbulb, "bulb" },
        { IconName.List, "unordered-list" },
        { IconName.ListOl, "ordered-list" },
        { IconName.ListUl, "unordered-list" },
        { IconName.LocationArrow, "aim" },
        { IconName.LockOpen, "unlock" },
        { IconName.Mail, "mail" },
        { IconName.MailOpen, "mail" },
        { IconName.Map, "environment" },
        { IconName.MapMarker, "environment" },
        { IconName.MapMarkerAlt, "environment" },
        { IconName.Memory, "hdd" },
        { IconName.Microphone, "audio" },
        { IconName.MicrophoneSlash, "audio-muted" },
        { IconName.MinusCircle, "minus-circle" },
        { IconName.MinusSquare, "minus-square" },
        { IconName.MoneyBillAlt, "money-collect" },
        { IconName.MoreHorizontal, "ellipsis" },
        { IconName.MoreVertical, "more" },
        { IconName.Motorcycle, "car" },
        { IconName.Mouse, "one-to-one" },
        { IconName.Music, "sound" },
        { IconName.PaintBrush, "format-painter" },
        { IconName.PaintRoller, "bg-colors" },
        { IconName.Palette, "bg-colors" },
        { IconName.PaperPlane, "send" },
        { IconName.Paperclip, "paper-clip" },
        { IconName.Parking, "car" },
        { IconName.Paste, "copy" },
        { IconName.PauseCircle, "pause-circle" },
        { IconName.Pen, "form" },
        { IconName.PhoneAlt, "phone" },
        { IconName.PizzaSlice, "rest" },
        { IconName.Plane, "send" },
        { IconName.PlaneArrival, "download" },
        { IconName.PlaneDeparture, "upload" },
        { IconName.Play, "caret-right" },
        { IconName.PlayCircle, "play-circle" },
        { IconName.Plug, "disconnect" },
        { IconName.PlusCircle, "plus-circle" },
        { IconName.PlusSquare, "plus-square" },
        { IconName.Poll, "fund" },
        { IconName.Portrait, "user" },
        { IconName.Print, "printer" },
        { IconName.PuzzlePiece, "block" },
        { IconName.QuestionCircle, "question-circle" },
        { IconName.QuoteRight, "message" },
        { IconName.Random, "swap" },
        { IconName.Receipt, "profile" },
        { IconName.Redo, "redo" },
        { IconName.Remove, "minus" },
        { IconName.RemoveFormat, "clear" },
        { IconName.Reply, "rollback" },
        { IconName.ReplyAll, "rollback" },
        { IconName.Restroom, "team" },
        { IconName.Rss, "wifi" },
        { IconName.RulerHorizontal, "column-width" },
        { IconName.Running, "man" },
        { IconName.Satellite, "rocket" },
        { IconName.Save, "save" },
        { IconName.School, "read" },
        { IconName.Screenshot, "scan" },
        { IconName.SdCard, "hdd" },
        { IconName.SearchMinus, "zoom-out" },
        { IconName.SearchPlus, "zoom-in" },
        { IconName.Seedling, "skin" },
        { IconName.Server, "cloud-server" },
        { IconName.Settings, "setting" },
        { IconName.Shapes, "block" },
        { IconName.Share, "share-alt" },
        { IconName.ShareAlt, "share-alt" },
        { IconName.Shield, "safety" },
        { IconName.ShieldAlt, "safety" },
        { IconName.Ship, "truck" },
        { IconName.ShoppingBag, "shopping" },
        { IconName.ShoppingBasket, "shopping" },
        { IconName.ShoppingCart, "shopping-cart" },
        { IconName.ShuttleVan, "truck" },
        { IconName.SimCard, "mobile" },
        { IconName.SliderHorizontal, "sliders" },
        { IconName.Smartphone, "mobile" },
        { IconName.Smoking, "fire" },
        { IconName.SmokingBan, "stop" },
        { IconName.Sms, "message" },
        { IconName.Snowflake, "sun" },
        { IconName.Sort, "sort-ascending" },
        { IconName.SortAlphaDown, "sort-descending" },
        { IconName.SortAlphaUp, "sort-ascending" },
        { IconName.SortAmountDownAlt, "sort-descending" },
        { IconName.SortDown, "sort-descending" },
        { IconName.SortUp, "sort-ascending" },
        { IconName.Spa, "rest" },
        { IconName.SpellCheck, "check" },
        { IconName.Square, "border" },
        { IconName.StarHalf, "star" },
        { IconName.StepBackward, "step-backward" },
        { IconName.StepForward, "step-forward" },
        { IconName.StickyNote, "file-text" },
        { IconName.Store, "shop" },
        { IconName.StoreAlt, "shop" },
        { IconName.Stream, "partition" },
        { IconName.StreetView, "environment" },
        { IconName.Subscript, "font-size" },
        { IconName.Subway, "car" },
        { IconName.Suitcase, "solution" },
        { IconName.Superscript, "font-size" },
        { IconName.SwimmingPool, "rest" },
        { IconName.SyncAlt, "sync" },
        { IconName.Taxi, "car" },
        { IconName.TextHeight, "column-height" },
        { IconName.ThumbsDown, "dislike" },
        { IconName.ThumbsUp, "like" },
        { IconName.Ticket, "tags" },
        { IconName.TicketAlt, "tags" },
        { IconName.Times, "close" },
        { IconName.TimesCircle, "close-circle" },
        { IconName.Tint, "bg-colors" },
        { IconName.TintSlash, "clear" },
        { IconName.TrafficLight, "dashboard" },
        { IconName.Train, "car" },
        { IconName.Tram, "car" },
        { IconName.Tree, "branches" },
        { IconName.Tv, "monitor" },
        { IconName.UmbrellaBeach, "insurance" },
        { IconName.Undo, "undo" },
        { IconName.UserCheck, "user-switch" },
        { IconName.UserCircle, "user" },
        { IconName.UserFriends, "team" },
        { IconName.UserPlus, "user-add" },
        { IconName.UserTie, "solution" },
        { IconName.Users, "team" },
        { IconName.Utensils, "rest" },
        { IconName.Video, "video-camera" },
        { IconName.VideoSlash, "video-camera" },
        { IconName.Voicemail, "audio" },
        { IconName.VolumeDown, "sound" },
        { IconName.VolumeMute, "sound" },
        { IconName.VolumeOff, "sound" },
        { IconName.VolumeUp, "sound" },
        { IconName.Walking, "man" },
        { IconName.Wheelchair, "user" },
        { IconName.Wifi, "wifi" },
        { IconName.WineBottle, "rest" },
        { IconName.Wrench, "tool" },
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

    public override IconStyle DefaultIconStyle => IconStyle.Regular;

    public override bool IconNameAsContent => false;

    #endregion
}