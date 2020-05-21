#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Providers;
#endregion

namespace Blazorise.Icons.FontAwesome
{
    class FontAwesomeIconProvider : BaseIconProvider
    {
        #region Members

        private static Dictionary<IconName, string> names = new Dictionary<IconName, string>
        {
            { IconName.Add, "fa-plus" },
            { IconName.Adjust, "fa-adjust" },
            { IconName.AlignCenter, "fa-align-center" },
            { IconName.AlignJustify, "fa-align-justify" },
            { IconName.AlignLeft, "fa-align-left" },
            { IconName.AlignRight, "fa-align-right" },
            { IconName.AngleLeft, "fa-angle-left" },
            { IconName.AngleRight, "fa-angle-right" },
            { IconName.Archive, "fa-archive" },
            { IconName.Backspace, "fa-backspace" },
            { IconName.Backward, "fa-backward" },
            { IconName.Ban, "fa-ban" },
            { IconName.BandAid, "fa-band-aid" },
            { IconName.Bars, "fa-bars" },
            { IconName.BatteryFull, "fa-battery-full" },
            { IconName.Bold, "fa-bold" },
            { IconName.Book, "fa-book" },
            { IconName.Bookmark, "fa-bookmark" },
            { IconName.BorderAll, "fa-border-all" },
            { IconName.BorderStyle, "fa-border-style" },
            { IconName.Brush, "fa-brush" },
            { IconName.Camera, "fa-camera" },
            { IconName.Check, "fa-check" },
            { IconName.CheckCircle, "fa-check-circle" },
            { IconName.ChevronLeft, "fa-chevron-left" },
            { IconName.ChevronRight, "fa-chevron-right" },
            { IconName.Clear, "fa-eraser" },
            { IconName.Cloud, "fa-cloud" },
            { IconName.Code, "fa-code" },
            { IconName.Comment, "fa-comment" },
            { IconName.Copyright, "fa-copyright" },
            { IconName.CreditCard, "fa-credit-card" },
            { IconName.Crop, "fa-crop" },
            { IconName.Dashboard, "fa-columns" },
            { IconName.Delete, "fa-trash" },
            { IconName.Directions, "fa-directions" },
            { IconName.Edit, "fa-edit" },
            { IconName.Eject, "fa-eject" },
            { IconName.ExpandLess, "fa-chevron-up" },
            { IconName.ExpandMore, "fa-chevron-down" },
            { IconName.FastForward, "fa-fast-forward" },
            { IconName.FileDownload, "fa-file-download" },
            { IconName.FileUpload, "fa-file-upload" },
            { IconName.Filter, "fa-filter" },
            { IconName.Fingerprint, "fa-fingerprint" },
            { IconName.Flag, "fa-flag" },
            { IconName.Folder, "fa-folder" },
            { IconName.FolderOpen, "fa-folder-open" },
            { IconName.Forward, "fa-forward" },
            { IconName.Gamepad, "fa-gamepad" },
            { IconName.Gavel, "fa-gavel" },
            { IconName.Headset, "fa-headset" },
            { IconName.History, "fa-history" },
            { IconName.Home, "fa-home" },
            { IconName.Hotel, "fa-hotel" },
            { IconName.HotTub, "fa-hot-tub" },
            { IconName.Image, "fa-image" },
            { IconName.Inbox, "fa-inbox" },
            { IconName.Info, "fa-info" },
            { IconName.Italic, "fa-italic" },
            { IconName.Key, "fa-key" },
            { IconName.Keyboard, "fa-keyboard" },
            { IconName.Language, "fa-language" },
            { IconName.Laptop, "fa-laptop" },
            { IconName.Link, "fa-link" },
            { IconName.List, "fa-list" },
            { IconName.Lock, "fa-lock" },
            { IconName.LockOpen, "fa-lock-open" },
            { IconName.Mail, "fa-envelope" },
            { IconName.Map, "fa-map" },
            { IconName.Memory, "fa-memory" },
            { IconName.MoreHorizontal, "fa-ellipsis-h" },
            { IconName.MoreVertical, "fa-ellipsis-v" },
            { IconName.Motorcycle, "fa-motorcycle" },
            { IconName.Mouse, "fa-mouse" },
            { IconName.Music, "fa-music" },
            { IconName.Palette, "fa-palette" },
            { IconName.Pause, "fa-pause" },
            { IconName.Phone, "fa-phone" },
            { IconName.Poll, "fa-poll" },
            { IconName.Portrait, "fa-portrait" },
            { IconName.Print, "fa-print" },
            { IconName.Receipt, "fa-receipt" },
            { IconName.Redo, "fa-redo" },
            { IconName.Remove, "fa-minus" },
            { IconName.Reply, "fa-reply" },
            { IconName.ReplyAll, "fa-reply-all" },
            { IconName.Satellite, "fa-satellite" },
            { IconName.Save, "fa-save" },
            { IconName.School, "fa-school" },
            { IconName.SdCard, "fa-sd-card" },
            { IconName.Search, "fa-search" },
            { IconName.Share, "fa-share" },
            { IconName.ShoppingBasket, "fa-shopping-basket" },
            { IconName.ShoppingCart, "fa-shopping-cart" },
            { IconName.SimCard, "fa-sim-card" },
            { IconName.SliderHorizontal, "fa-sliders-h" },
            { IconName.Smartphone, "fa-mobile" },
            { IconName.Sms, "fa-sms" },
            { IconName.Sort, "fa-sort" },
            { IconName.SortDown, "fa-sort-down" },
            { IconName.SortUp, "fa-sort-up" },
            { IconName.Spa, "fa-spa" },
            { IconName.Star, "fa-star" },
            { IconName.StarHalf, "fa-star-half" },
            { IconName.Stop, "fa-stop" },
            { IconName.Store, "fa-store" },
            { IconName.Subway, "fa-subway" },
            { IconName.Sync, "fa-sync" },
            { IconName.Tablet, "fa-tablet" },
            { IconName.Tint, "fa-tint" },
            { IconName.TintSlash, "fa-tint-slash" },
            { IconName.Train, "fa-train" },
            { IconName.Tram, "fa-tram" },
            { IconName.Tv, "fa-tv" },
            { IconName.Underline, "fa-underline" },
            { IconName.Undo, "fa-undo" },
            { IconName.Voicemail, "fa-voicemail" },
            { IconName.VolumeDown, "fa-volume-down" },
            { IconName.VolumeMute, "fa-volume-mute" },
            { IconName.VolumeOff, "fa-volume-off" },
            { IconName.VolumeUp, "fa-volume-up" },
            { IconName.Wifi, "fa-wifi" },
        };

        private static Dictionary<IconStyle, string> styles = new Dictionary<IconStyle, string>
        {
            { IconStyle.Solid, "fas" },
            { IconStyle.Regular, "far" },
            { IconStyle.Light, "fal" },
            { IconStyle.DuoTone, "fad" },
        };

        #endregion

        #region Constructors

        #endregion

        #region Methods

        public override string GetIconName( IconName iconName )
        {
            names.TryGetValue( iconName, out var name );

            return name;
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
            return iconName.Split( ' ' ).Any( x => styles.Values.Contains( x ) || new string[] { "fab" }.Contains( x ) );
        }

        #endregion

        #region Properties

        public override bool IconNameAsContent => false;

        #endregion
    }
}
