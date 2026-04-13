#region Using directives
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Blazorise.Providers;
#endregion

namespace Blazorise.Icons.Lucide;

class LucideIconProvider : BaseIconProvider
{
    #region Members

    private static readonly Dictionary<IconName, string> names = new()
    {
        { IconName.Add, "plus" },
        { IconName.Adjust, "sliders-horizontal" },
        { IconName.Alert, "bell" },
        { IconName.AlertOff, "bell-off" },
        { IconName.AlignCenter, "text-align-center" },
        { IconName.AlignJustify, "text-align-justify" },
        { IconName.AlignLeft, "text-align-start" },
        { IconName.AlignRight, "text-align-end" },
        { IconName.AngleDown, "chevron-down" },
        { IconName.AngleLeft, "chevron-left" },
        { IconName.AngleRight, "chevron-right" },
        { IconName.AngleUp, "chevron-up" },
        { IconName.Apple, "apple" },
        { IconName.Archive, "archive" },
        { IconName.ArrowAltCircleDown, "circle-arrow-down" },
        { IconName.ArrowDown, "arrow-down" },
        { IconName.ArrowLeft, "arrow-left" },
        { IconName.ArrowRight, "arrow-right" },
        { IconName.ArrowUp, "arrow-up" },
        { IconName.Baby, "baby" },
        { IconName.BabyCarriage, "baby" },
        { IconName.Backspace, "delete" },
        { IconName.Backward, "skip-back" },
        { IconName.BalanceScale, "scale" },
        { IconName.Ban, "ban" },
        { IconName.BandAid, "bandage" },
        { IconName.Bars, "menu" },
        { IconName.BatteryFull, "battery-full" },
        { IconName.Bell, "bell" },
        { IconName.BellSlash, "bell-off" },
        { IconName.Biking, "bike" },
        { IconName.BirthdayCake, "cake" },
        { IconName.Bold, "bold" },
        { IconName.Bolt, "zap" },
        { IconName.Book, "book" },
        { IconName.Bookmark, "bookmark" },
        { IconName.BookReader, "book-open" },
        { IconName.BorderAll, "square" },
        { IconName.BorderNone, "square-dashed" },
        { IconName.BorderStyle, "square-dashed-bottom" },
        { IconName.Briefcase, "briefcase" },
        { IconName.Brush, "brush" },
        { IconName.Bug, "bug" },
        { IconName.Building, "building" },
        { IconName.Bus, "bus" },
        { IconName.Calendar, "calendar" },
        { IconName.CalendarCheck, "calendar-check" },
        { IconName.CalendarDay, "calendar-days" },
        { IconName.CalendarTimes, "calendar-x" },
        { IconName.CalendarWeek, "calendar-range" },
        { IconName.Camera, "camera" },
        { IconName.CameraRetro, "camera" },
        { IconName.Car, "car" },
        { IconName.CaretSquareRight, "square-chevron-right" },
        { IconName.Cart, "shopping-cart" },
        { IconName.CartMinus, "shopping-cart" },
        { IconName.CartPlus, "shopping-cart" },
        { IconName.Chair, "armchair" },
        { IconName.ChartArea, "chart-area" },
        { IconName.ChartBar, "chart-bar" },
        { IconName.ChartLine, "chart-line" },
        { IconName.ChartPie, "chart-pie" },
        { IconName.ChartScatter, "chart-scatter" },
        { IconName.Check, "check" },
        { IconName.CheckCircle, "circle-check" },
        { IconName.CheckDouble, "list-checks" },
        { IconName.CheckSquare, "square-check" },
        { IconName.ChevronDoubleDown, "chevrons-down" },
        { IconName.ChevronDoubleLeft, "chevrons-left" },
        { IconName.ChevronDoubleRight, "chevrons-right" },
        { IconName.ChevronDoubleUp, "chevrons-up" },
        { IconName.ChevronDown, "chevron-down" },
        { IconName.ChevronLeft, "chevron-left" },
        { IconName.ChevronRight, "chevron-right" },
        { IconName.ChevronUp, "chevron-up" },
        { IconName.Circle, "circle" },
        { IconName.City, "building-2" },
        { IconName.Clear, "eraser" },
        { IconName.ClinicMedical, "briefcase-medical" },
        { IconName.Clipboard, "clipboard" },
        { IconName.Clock, "clock" },
        { IconName.ClosedCaptioning, "closed-caption" },
        { IconName.Cloud, "cloud" },
        { IconName.CloudDownloadAlt, "cloud-download" },
        { IconName.CloudUploadAlt, "cloud-upload" },
        { IconName.Cocktail, "martini" },
        { IconName.Code, "code" },
        { IconName.Coffee, "coffee" },
        { IconName.Comment, "message-circle" },
        { IconName.CommentAlt, "message-square" },
        { IconName.Comments, "messages-square" },
        { IconName.CompactDisc, "disc" },
        { IconName.Compass, "compass" },
        { IconName.Compress, "minimize" },
        { IconName.Copy, "copy" },
        { IconName.Copyright, "copyright" },
        { IconName.CreditCard, "credit-card" },
        { IconName.Crop, "crop" },
        { IconName.Cut, "scissors" },
        { IconName.Dashboard, "layout-dashboard" },
        { IconName.Delete, "trash" },
        { IconName.Desktop, "monitor" },
        { IconName.Dice, "dices" },
        { IconName.Directions, "navigation" },
        { IconName.Dog, "dog" },
        { IconName.DollarSign, "dollar-sign" },
        { IconName.DotCircle, "circle-dot" },
        { IconName.Download, "download" },
        { IconName.Dumbbell, "dumbbell" },
        { IconName.Edit, "square-pen" },
        { IconName.Eject, "upload" },
        { IconName.Ethernet, "ethernet-port" },
        { IconName.EuroSign, "euro" },
        { IconName.Exclamation, "circle-alert" },
        { IconName.ExclamationCircle, "circle-alert" },
        { IconName.ExclamationTriangle, "triangle-alert" },
        { IconName.Expand, "expand" },
        { IconName.ExpandArrowsAlt, "fullscreen" },
        { IconName.ExpandLess, "chevron-up" },
        { IconName.ExpandMore, "chevron-down" },
        { IconName.ExternalLinkSquareAlt, "external-link" },
        { IconName.Eye, "eye" },
        { IconName.EyeSlash, "eye-off" },
        { IconName.FastBackward, "rewind" },
        { IconName.FastForward, "fast-forward" },
        { IconName.File, "file" },
        { IconName.FileAlt, "file-text" },
        { IconName.FileDownload, "file-down" },
        { IconName.FilePdf, "file-text" },
        { IconName.FileUpload, "file-up" },
        { IconName.Film, "film" },
        { IconName.Filter, "funnel" },
        { IconName.Fingerprint, "fingerprint-pattern" },
        { IconName.Fire, "flame" },
        { IconName.Flag, "flag" },
        { IconName.Flask, "flask-conical" },
        { IconName.Folder, "folder" },
        { IconName.FolderOpen, "folder-open" },
        { IconName.FolderPlus, "folder-plus" },
        { IconName.Forward, "forward" },
        { IconName.Frown, "frown" },
        { IconName.FrownOpen, "frown" },
        { IconName.Gamepad, "gamepad-2" },
        { IconName.GasPump, "fuel" },
        { IconName.Gavel, "gavel" },
        { IconName.Gift, "gift" },
        { IconName.Globe, "globe" },
        { IconName.Grin, "laugh" },
        { IconName.GripLines, "grip-horizontal" },
        { IconName.GripVertical, "grip-vertical" },
        { IconName.HandPaper, "hand" },
        { IconName.HandsHelping, "hand-helping" },
        { IconName.Headphones, "headphones" },
        { IconName.Headset, "headset" },
        { IconName.Heart, "heart" },
        { IconName.Highlighter, "highlighter" },
        { IconName.History, "history" },
        { IconName.Home, "house" },
        { IconName.Hospital, "hospital" },
        { IconName.Hotel, "hotel" },
        { IconName.HotTub, "bath" },
        { IconName.Hourglass, "hourglass" },
        { IconName.IdCard, "id-card" },
        { IconName.Image, "image" },
        { IconName.Images, "images" },
        { IconName.Inbox, "inbox" },
        { IconName.Indent, "list-indent-increase" },
        { IconName.Infinity, "infinity" },
        { IconName.Info, "info" },
        { IconName.InfoCircle, "badge-info" },
        { IconName.Italic, "italic" },
        { IconName.Key, "key" },
        { IconName.Keyboard, "keyboard" },
        { IconName.Language, "languages" },
        { IconName.Laptop, "laptop" },
        { IconName.LaptopCode, "laptop-minimal" },
        { IconName.Laugh, "laugh" },
        { IconName.LayerGroup, "layers" },
        { IconName.Lightbulb, "lightbulb" },
        { IconName.Link, "link" },
        { IconName.List, "list" },
        { IconName.ListOl, "list-ordered" },
        { IconName.ListUl, "list" },
        { IconName.LocationArrow, "navigation" },
        { IconName.Lock, "lock" },
        { IconName.LockOpen, "lock-open" },
        { IconName.Mail, "mail" },
        { IconName.MailOpen, "mail-open" },
        { IconName.Map, "map" },
        { IconName.MapMarker, "map-pin" },
        { IconName.MapMarkerAlt, "map-pinned" },
        { IconName.Memory, "memory-stick" },
        { IconName.Microphone, "mic" },
        { IconName.MicrophoneSlash, "mic-off" },
        { IconName.Minus, "minus" },
        { IconName.MinusCircle, "circle-minus" },
        { IconName.MinusSquare, "square-minus" },
        { IconName.MoneyBillAlt, "banknote" },
        { IconName.Moon, "moon" },
        { IconName.MoreHorizontal, "ellipsis" },
        { IconName.MoreVertical, "ellipsis-vertical" },
        { IconName.Motorcycle, "motorbike" },
        { IconName.Mouse, "mouse" },
        { IconName.Music, "music" },
        { IconName.PaintBrush, "paintbrush" },
        { IconName.PaintRoller, "paint-roller" },
        { IconName.Palette, "palette" },
        { IconName.Paperclip, "paperclip" },
        { IconName.PaperPlane, "send" },
        { IconName.Parking, "square-parking" },
        { IconName.Paste, "clipboard-paste" },
        { IconName.Pause, "pause" },
        { IconName.PauseCircle, "circle-pause" },
        { IconName.Pen, "pen" },
        { IconName.Phone, "phone" },
        { IconName.PhoneAlt, "phone-call" },
        { IconName.PizzaSlice, "pizza" },
        { IconName.Plane, "plane" },
        { IconName.PlaneArrival, "plane-landing" },
        { IconName.PlaneDeparture, "plane-takeoff" },
        { IconName.Play, "play" },
        { IconName.PlayCircle, "circle-play" },
        { IconName.Plug, "plug" },
        { IconName.Plus, "plus" },
        { IconName.PlusCircle, "circle-plus" },
        { IconName.PlusSquare, "square-plus" },
        { IconName.Poll, "chart-no-axes-column" },
        { IconName.Portrait, "user" },
        { IconName.Print, "printer" },
        { IconName.PuzzlePiece, "puzzle" },
        { IconName.QuestionCircle, "circle-question-mark" },
        { IconName.QuoteRight, "quote" },
        { IconName.Random, "shuffle" },
        { IconName.Receipt, "receipt" },
        { IconName.Redo, "redo" },
        { IconName.Remove, "minus" },
        { IconName.RemoveFormat, "remove-formatting" },
        { IconName.Reply, "reply" },
        { IconName.ReplyAll, "reply-all" },
        { IconName.Restroom, "toilet" },
        { IconName.Rss, "rss" },
        { IconName.RulerHorizontal, "ruler" },
        { IconName.Running, "person-standing" },
        { IconName.Satellite, "satellite" },
        { IconName.Save, "save" },
        { IconName.School, "school" },
        { IconName.Screenshot, "scan" },
        { IconName.SdCard, "card-sim" },
        { IconName.Search, "search" },
        { IconName.SearchMinus, "zoom-out" },
        { IconName.SearchPlus, "zoom-in" },
        { IconName.Seedling, "sprout" },
        { IconName.Send, "send" },
        { IconName.Server, "server" },
        { IconName.Settings, "settings" },
        { IconName.Shapes, "shapes" },
        { IconName.Share, "share" },
        { IconName.ShareAlt, "share-2" },
        { IconName.Shield, "shield" },
        { IconName.ShieldAlt, "shield-check" },
        { IconName.Ship, "ship" },
        { IconName.ShoppingBag, "shopping-bag" },
        { IconName.ShoppingBasket, "shopping-basket" },
        { IconName.ShoppingCart, "shopping-cart" },
        { IconName.ShuttleVan, "bus-front" },
        { IconName.SimCard, "card-sim" },
        { IconName.SliderHorizontal, "sliders-horizontal" },
        { IconName.Smartphone, "smartphone" },
        { IconName.Smile, "smile" },
        { IconName.Smoking, "cigarette" },
        { IconName.SmokingBan, "cigarette-off" },
        { IconName.Sms, "message-square-text" },
        { IconName.Snowflake, "snowflake" },
        { IconName.Sort, "arrow-down-up" },
        { IconName.SortAlphaDown, "arrow-down-a-z" },
        { IconName.SortAlphaUp, "arrow-up-a-z" },
        { IconName.SortAmountDownAlt, "arrow-down-wide-narrow" },
        { IconName.SortDown, "arrow-down-wide-narrow" },
        { IconName.SortUp, "arrow-up-narrow-wide" },
        { IconName.Spa, "flower" },
        { IconName.SpellCheck, "spell-check" },
        { IconName.Square, "square" },
        { IconName.Star, "star" },
        { IconName.StarHalf, "star-half" },
        { IconName.StepBackward, "step-back" },
        { IconName.StepForward, "step-forward" },
        { IconName.StickyNote, "sticky-note" },
        { IconName.Stop, "square-stop" },
        { IconName.Store, "store" },
        { IconName.StoreAlt, "store" },
        { IconName.Stream, "list-collapse" },
        { IconName.StreetView, "view" },
        { IconName.Strikethrough, "strikethrough" },
        { IconName.Subscript, "subscript" },
        { IconName.Subway, "train-front" },
        { IconName.Suitcase, "briefcase-business" },
        { IconName.Sun, "sun" },
        { IconName.Superscript, "superscript" },
        { IconName.SwimmingPool, "waves" },
        { IconName.Sync, "refresh-cw" },
        { IconName.SyncAlt, "refresh-ccw" },
        { IconName.Table, "table" },
        { IconName.Tablet, "tablet" },
        { IconName.Tag, "tag" },
        { IconName.Taxi, "car-taxi-front" },
        { IconName.TextHeight, "text-cursor-input" },
        { IconName.ThumbsDown, "thumbs-down" },
        { IconName.ThumbsUp, "thumbs-up" },
        { IconName.Ticket, "ticket" },
        { IconName.TicketAlt, "tickets" },
        { IconName.Times, "x" },
        { IconName.TimesCircle, "circle-x" },
        { IconName.Tint, "droplet" },
        { IconName.TintSlash, "droplet-off" },
        { IconName.TrafficLight, "traffic-cone" },
        { IconName.Train, "train-front" },
        { IconName.Tram, "tram-front" },
        { IconName.Tree, "tree-pine" },
        { IconName.Truck, "truck" },
        { IconName.Tv, "tv" },
        { IconName.UmbrellaBeach, "umbrella" },
        { IconName.Underline, "underline" },
        { IconName.Undo, "undo" },
        { IconName.Unlock, "lock-open" },
        { IconName.User, "user" },
        { IconName.UserCheck, "user-check" },
        { IconName.UserCircle, "circle-user" },
        { IconName.UserFriends, "users" },
        { IconName.UserPlus, "user-plus" },
        { IconName.Users, "users" },
        { IconName.UserTie, "user-star" },
        { IconName.Utensils, "utensils" },
        { IconName.Video, "video" },
        { IconName.VideoSlash, "video-off" },
        { IconName.Voicemail, "voicemail" },
        { IconName.VolumeDown, "volume-1" },
        { IconName.VolumeMute, "volume-x" },
        { IconName.VolumeOff, "volume-off" },
        { IconName.VolumeUp, "volume-2" },
        { IconName.Walking, "footprints" },
        { IconName.Wallet, "wallet" },
        { IconName.Wheelchair, "accessibility" },
        { IconName.Wifi, "wifi" },
        { IconName.WineBottle, "bottle-wine" },
        { IconName.Wrench, "wrench" },
        { IconName.Zoom, "search" },
        { IconName.ZoomIn, "zoom-in" },
        { IconName.ZoomOut, "zoom-out" },
    };

    private static readonly string[] styleNames = new[]
    {
        "lucide",
    };

    #endregion

    #region Methods

    public override string IconSize( IconSize iconSize )
    {
        return iconSize switch
        {
            Blazorise.IconSize.ExtraSmall => "lucide-xs",
            Blazorise.IconSize.Small => "lucide-sm",
            Blazorise.IconSize.Large => "lucide-lg",
            Blazorise.IconSize.x2 => "lucide-2x",
            Blazorise.IconSize.x3 => "lucide-3x",
            Blazorise.IconSize.x4 => "lucide-4x",
            Blazorise.IconSize.x5 => "lucide-5x",
            Blazorise.IconSize.x6 => "lucide-6x",
            Blazorise.IconSize.x7 => "lucide-7x",
            Blazorise.IconSize.x8 => "lucide-8x",
            Blazorise.IconSize.x9 => "lucide-9x",
            Blazorise.IconSize.x10 => "lucide-10x",
            _ => null,
        };
    }

    public override string GetIconName( IconName iconName, IconStyle iconStyle )
    {
        if ( !names.TryGetValue( iconName, out string name ) )
            name = ToKebabCase( iconName.ToString() );

        return LucideIconSvg.ResolveIconName( name );
    }

    public override void SetIconName( IconName name, string newName )
    {
        names[name] = NormalizeBaseName( newName );
    }

    public override string GetStyleName( IconStyle iconStyle ) => "lucide";

    public string GetSvg( object name, IconStyle iconStyle )
    {
        string iconName = ResolveIconName( name, iconStyle );

        return LucideIconSvg.Get( iconName ) ?? string.Empty;
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
            string customName => LucideIconSvg.ResolveIconName( NormalizeBaseName( customName ) ),
            _ => null,
        };
    }

    private static string NormalizeBaseName( string name )
    {
        if ( string.IsNullOrWhiteSpace( name ) )
            return null;

        string[] tokens = name.Split( ' ' );
        string iconName = tokens.FirstOrDefault( x => x.StartsWith( "lucide-" ) && x != "lucide" && !x.EndsWith( "x" ) );

        if ( iconName is not null )
            name = iconName;

        return name.StartsWith( "lucide-" )
            ? name["lucide-".Length..]
            : name;
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