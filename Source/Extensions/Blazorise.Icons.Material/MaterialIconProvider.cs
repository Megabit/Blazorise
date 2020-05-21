#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Providers;
#endregion

namespace Blazorise.Icons.Material
{
    class MaterialIconProvider : BaseIconProvider
    {
        #region Members

        private static Dictionary<IconName, string> names = new Dictionary<IconName, string>
        {
            { IconName.Add, "add" },
            { IconName.Adjust, "adjust" },
            { IconName.AlignCenter, "format_align_center" },
            { IconName.AlignJustify, "format_align_justify" },
            { IconName.AlignLeft, "format_align_left" },
            { IconName.AlignRight, "format_align_right" },
            { IconName.AngleLeft, "keyboard_arrow_left" },
            { IconName.AngleRight, "keyboard_arrow_right" },
            { IconName.Archive, "archive" },
            { IconName.Backspace, "backspace" },
            { IconName.Backward, "fast_rewind" },
            { IconName.Ban, "block" },
            { IconName.BandAid, "healing" },
            { IconName.Bars, "menu" },
            { IconName.BatteryFull, "battery_full" },
            { IconName.Bold, "format_bold" },
            { IconName.Book, "book" },
            { IconName.Bookmark, "bookmark" },
            { IconName.BorderAll, "border_all" },
            { IconName.BorderStyle, "border_style" },
            { IconName.Brush, "brush" },
            { IconName.Camera, "camera" },
            { IconName.Check, "check" },
            { IconName.CheckCircle, "check_circle" },
            { IconName.ChevronLeft, "chevron_left" },
            { IconName.ChevronRight, "chevron_right" },
            { IconName.Clear, "clear" },
            { IconName.Cloud, "cloud" },
            { IconName.Code, "code" },
            { IconName.Comment, "comment" },
            { IconName.Copyright, "copyright" },
            { IconName.CreditCard, "credit_card" },
            { IconName.Crop, "crop" },
            { IconName.Dashboard, "dashboard" },
            { IconName.Delete, "delete" },
            { IconName.Directions, "directions" },
            { IconName.Edit, "edit" },
            { IconName.Eject, "eject" },
            { IconName.ExpandLess, "expand_less" },
            { IconName.ExpandMore, "expand_more" },
            { IconName.FastForward, "fast_forward" },
            { IconName.FileDownload, "file_download" },
            { IconName.FileUpload, "file_upload" },
            { IconName.Filter, "filter" },
            { IconName.Fingerprint, "fingerprint" },
            { IconName.Flag, "flag" },
            { IconName.Folder, "folder" },
            { IconName.FolderOpen, "folder_open" },
            { IconName.Forward, "forward" },
            { IconName.Gamepad, "gamepad" },
            { IconName.Gavel, "gavel" },
            { IconName.Headset, "headset" },
            { IconName.History, "history" },
            { IconName.Home, "home" },
            { IconName.Hotel, "hotel" },
            { IconName.HotTub, "hot_tub" },
            { IconName.Image, "image" },
            { IconName.Inbox, "inbox" },
            { IconName.Info, "info" },
            { IconName.Italic, "format_italic" },
            { IconName.Key, "vpn_key" },
            { IconName.Keyboard, "keyboard" },
            { IconName.Language, "language" },
            { IconName.Laptop, "laptop" },
            { IconName.Link, "link" },
            { IconName.List, "list" },
            { IconName.Lock, "lock" },
            { IconName.LockOpen, "lock_open" },
            { IconName.Mail, "mail" },
            { IconName.Map, "map" },
            { IconName.Memory, "memory" },
            { IconName.MoreHorizontal, "more_horiz" },
            { IconName.MoreVertical, "more_vert" },
            { IconName.Motorcycle, "motorcycle" },
            { IconName.Mouse, "mouse" },
            { IconName.Music, "music_note" },
            { IconName.Palette, "palette" },
            { IconName.Pause, "pause" },
            { IconName.Phone, "phone" },
            { IconName.Poll, "poll" },
            { IconName.Portrait, "portrait" },
            { IconName.Print, "print" },
            { IconName.Receipt, "receipt" },
            { IconName.Redo, "redo" },
            { IconName.Remove, "remove" },
            { IconName.Reply, "reply" },
            { IconName.ReplyAll, "reply_all" },
            { IconName.Satellite, "satellite" },
            { IconName.Save, "save" },
            { IconName.School, "school" },
            { IconName.SdCard, "sd_card" },
            { IconName.Search, "search" },
            { IconName.Share, "share" },
            { IconName.ShoppingBasket, "shopping_basket" },
            { IconName.ShoppingCart, "shopping_cart" },
            { IconName.SimCard, "sim_card" },
            { IconName.SliderHorizontal, "tune" },
            { IconName.Smartphone, "smartphone" },
            { IconName.Sms, "sms" },
            { IconName.Sort, "sort" },
            { IconName.SortDown, "arrow_drop_down" },
            { IconName.SortUp, "arrow_drop_up" },
            { IconName.Spa, "spa" },
            { IconName.Star, "star" },
            { IconName.StarHalf, "star_half" },
            { IconName.Stop, "stop" },
            { IconName.Store, "store" },
            { IconName.Subway, "subway" },
            { IconName.Sync, "sync" },
            { IconName.Tablet, "tablet" },
            { IconName.Tint, "invert_colors" },
            { IconName.TintSlash, "invert_colors_off" },
            { IconName.Train, "train" },
            { IconName.Tram, "tram" },
            { IconName.Tv, "tv" },
            { IconName.Underline, "format_underlined" },
            { IconName.Undo, "undo" },
            { IconName.Voicemail, "voicemail" },
            { IconName.VolumeDown, "volume_down" },
            { IconName.VolumeMute, "volume_mute" },
            { IconName.VolumeOff, "volume_off" },
            { IconName.VolumeUp, "volume_up" },
            { IconName.Wifi, "wifi" },
        };

        private static Dictionary<IconStyle, string> styles = new Dictionary<IconStyle, string>
        {
            { IconStyle.Solid, "material-icons" },
            { IconStyle.Regular, "material-icons-outlined" },
            { IconStyle.Light, "material-icons-sharp" }, // TODO: probably not correct
            { IconStyle.DuoTone, "material-icons-two-tone" },
        };

        #endregion

        #region Constructors

        #endregion

        #region Methods

        public override string GetIconName( IconName iconName )
        {
            names.TryGetValue( iconName, out var value );

            return value;
        }

        public override void SetIconName( IconName name, string newName )
        {
            names[name] = newName;
        }

        public override string GetStyleName( IconStyle iconStyle )
        {
            if ( styles.TryGetValue( iconStyle, out var style ) )
                return style;

            return null;
        }

        protected override bool ContainsStyleName( string iconName )
        {
            return iconName.Split( ' ' ).Any( x => styles.Values.Contains( x ) || new string[] { "material-icons-round" }.Contains( x ) );
        }

        #endregion

        #region Properties

        public override bool IconNameAsContent => true;

        #endregion
    }
}
