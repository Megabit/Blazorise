#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Blazorise.Providers
{
    class BootstrapClassProvider : IClassProvider
    {
        #region Text

        public virtual string Text( bool plaintext ) => plaintext ? "form-control-plaintext" : "form-control";

        public virtual string TextSize( Size size ) => $"{Text( false )}-{Size( size )}";

        public virtual string TextColor( Color color ) => $"text-{Color( color )}";

        #endregion

        #region Select

        public virtual string Select() => Custom ? "custom-select" : "form-control";

        public virtual string SelectSize( Size size ) => $"{Select()}-{Size( size )}";

        #endregion

        #region Date

        public string Date() => "form-control";

        public string DateSize( Size size ) => $"{Date()}-{Size( size )}";

        #endregion

        #region Check

        public string Check() => Custom ? "custom-control-input" : "form-check-input";

        public string CheckInline() => Custom ? "custom-control-inline" : "form-check-inline";

        #endregion

        #region Radio

        public virtual string Radio() => Custom ? "custom-control-input" : "form-check-input";

        public virtual string RadioInline() => Custom ? "custom-control-inline" : "form-check-inline";

        #endregion

        #region File

        public virtual string File() => Custom ? "custom-file-input" : "form-control-file";

        #endregion

        #region Label

        public string Label() => null;

        public string LabelCheck() => Custom ? "custom-control-label" : "form-check-label";

        public string LabelFile() => Custom ? "custom-file-label" : null;

        #endregion

        #region Help

        public string Help() => "form-text text-muted";

        #endregion

        #region Fields

        public virtual string Fields() => "form-row";

        public virtual string FieldsBody() => null;

        public virtual string FieldsColumn() => $"{Col()}";

        public virtual string FieldsColumnSize( ColumnSize columnSize )
        {
            var colClass = Col();
            var columnSizeClass = ColumnSize( columnSize );

            return columnSizeClass != null ? $"{colClass}-{columnSizeClass}" : colClass;
        }

        #endregion

        #region Field

        public virtual string Field( /*bool hasCustom*/ ) =>/* Custom && hasCustom ? "custom-control" :*/ "form-group";

        public virtual string FieldColumn() => $"{Col()}";

        public virtual string FieldColumnSize( ColumnSize columnSize )
        {
            var colClass = Col();
            var columnSizeClass = ColumnSize( columnSize );

            return columnSizeClass != null ? $"{colClass}-{columnSizeClass}" : colClass;
        }

        public virtual string FieldCheck() => Custom ? "custom-checkbox" : "form-check";

        public virtual string FieldRadio() => Custom ? "custom-radio" : "form-check";

        public virtual string FieldFile() => Custom ? "custom-file" : "form-group";

        #endregion

        #region Control

        public virtual string ControlCheck() => Custom ? "custom-control custom-checkbox" : "form-check";

        public virtual string ControlRadio() => Custom ? "custom-control custom-radio" : "form-check";

        public virtual string ControlFile() => Custom ? "custom-control custom-file" : "form-group";

        #endregion

        #region Addons

        public virtual string Addons() => "input-group";

        public virtual string Addon( AddonType addonType )
        {
            switch ( addonType )
            {
                case AddonType.Start:
                    return "input-group-prepend";
                case AddonType.End:
                    return "input-group-append";
                default:
                    return null;
            }
        }

        public virtual string AddonLabel() => "input-group-text";

        public virtual string AddonContainer() => null;

        #endregion

        #region Inline

        public virtual string Inline() => "form-inline";

        #endregion

        #region Button

        public virtual string Button() => "btn";

        public virtual string ButtonColor( Color color ) => $"{Button()}-{Color( color )}";

        public virtual string ButtonOutline( Color color ) => color != Blazorise.Color.None ? $"{Button()}-outline-{Color( color )}" : $"{Button()}-outline";

        public virtual string ButtonSize( Size size ) => $"{Button()}-{Size( size )}";

        public virtual string ButtonBlock() => $"{Button()}-block";

        public virtual string ButtonActive() => "active";

        #endregion

        #region Buttons

        //public virtual string Buttons() => "btn-group";

        public virtual string ButtonsAddons() => "btn-group";

        public virtual string ButtonsToolbar() => "btn-toolbar";

        public virtual string ButtonsSize( Size size ) => $"{ButtonsAddons()}-{Size( size )}";

        public virtual string ButtonsVertical() => "btn-group-vertical";

        #endregion

        #region CloseButton

        public virtual string CloseButton() => "close";

        #endregion

        #region Dropdown

        public virtual string Dropdown() => "dropdown";

        public virtual string DropdownGroup() => "btn-group";

        public virtual string DropdownShow() => Show();

        public virtual string DropdownRight() => null;

        public virtual string DropdownItem() => "dropdown-item";

        public virtual string DropdownDivider() => "dropdown-divider";

        public virtual string DropdownMenu() => "dropdown-menu";

        public virtual string DropdownMenuBody() => null;

        public virtual string DropdownMenuShow() => Show();

        public virtual string DropdownMenuRight() => "dropdown-menu-right";

        public virtual string DropdownToggle() => "dropdown-toggle";

        public virtual string DropdownToggleSplit() => "dropdown-toggle-split";

        #endregion

        #region Tab

        public virtual string Tabs() => "nav nav-tabs";

        public virtual string TabsCards() => "card-header-tabs";

        public virtual string TabsPills() => "nav-pills";

        public virtual string TabsFullWidth() => /*fillType == NavFillType.Justified ? "nav-justified" :*/ "nav-fill";

        public virtual string TabsVertical() => "flex-column";

        public virtual string TabItem() => "nav-item";

        public virtual string TabItemActive() => null;

        public virtual string TabLink() => "nav-link";

        public virtual string TabLinkActive() => $"{Active()} {Show()}";

        public virtual string TabContent() => "tab-content";

        public virtual string TabPanel() => "tab-pane";

        public virtual string TabPanelActive() => $"{Active()} {Show()}";

        #endregion

        #region Card

        public virtual string Card() => "card";

        public virtual string CardActions() => "card-actions";

        public virtual string CardBody() => "card-body";

        public virtual string CardFooter() => "card-footer";

        public virtual string CardHeader() => "card-header";

        public virtual string CardTitle() => "card-title";

        public virtual string CardSubtitle() => "card-subtitle";

        public virtual string CardSubtitleSize( int size ) => null;

        public virtual string CardText() => "card-text";

        #endregion

        #region Container

        public virtual string Container() => "container";

        public virtual string ContainerFluid() => "container-fluid";

        #endregion

        #region Panel

        public virtual string Panel() => null;

        #endregion

        #region Nav

        public virtual string Nav() => "nav";

        public virtual string NavItem() => "nav-item";

        public virtual string NavLink() => "nav-link";

        public virtual string NavTabs() => "nav-tabs";

        public virtual string NavCards() => "nav-cards";

        public virtual string NavPills() => "nav-pills";

        public virtual string NavFill( NavFillType fillType ) => fillType == NavFillType.Justified ? "nav-justified" : "nav-fill";

        public virtual string NavVertical() => "flex-column";

        #endregion

        #region Navbar

        public virtual string Bar() => "navbar";

        public virtual string BarShade( Theme theme ) => $"navbar-{Theme( theme )}";

        public virtual string BarBreakpoint( Breakpoint breakpoint ) => $"navbar-expand-{Breakpoint( breakpoint )}";

        public virtual string BarItem() => "nav-item";

        public virtual string BarItemActive() => Active();

        public virtual string BarItemDisabled() => Disabled();

        public virtual string BarItemDropdown() => "dropdown";

        public virtual string BarItemDropdownShow() => Show();

        public virtual string BarLink() => "nav-link";

        public virtual string BarLinkDisabled() => Disabled();

        //public virtual string BarCollapse() => "navbar-collapse";

        public virtual string BarBrand() => "navbar-brand";

        public virtual string BarToggler() => "navbar-toggler";

        public virtual string BarMenu() => "collapse navbar-collapse";

        public virtual string BarMenuShow() => Show();

        public virtual string BarStart() => "navbar-nav mr-auto";

        public virtual string BarEnd() => "navbar-nav";

        //public virtual string BarHasDropdown() => "dropdown";

        public virtual string BarDropdown() => "dropdown-menu";

        public virtual string BarDropdownShow() => Show();

        public virtual string BarDropdownToggler() => "nav-link dropdown-toggle";

        public virtual string BarDropdownItem() => "dropdown-item";

        public virtual string BarTogglerIcon() => "navbar-toggler-icon";

        #endregion

        #region Collapse

        public virtual string Collapse() => "collapse";

        public virtual string CollapseShow() => Show();

        #endregion

        #region Drawer

        public virtual string Drawer() => "navdrawer";

        public virtual string Drawer( DrawerType drawerType, Size breakpoint ) => breakpoint != Blazorise.Size.None ? $"{Drawer()}-{DrawerType( drawerType )}-{Size( breakpoint )}" : $"{Drawer()}-{DrawerType( drawerType )}";

        public virtual string DrawerContent() => "navdrawer-content";

        public virtual string DrawerHeader() => "navdrawer-header";

        public virtual string DrawerBrand() => "navbar-brand";

        public virtual string DrawerToggler() => "navbar-toggler";

        public virtual string DrawerLabel() => "navdrawer-subheader";

        public virtual string DrawerDivider() => "navdrawer-divider";

        public virtual string DrawerMenu() => "navdrawer-nav";

        public virtual string DrawerMenuItem() => "nav-item";

        public virtual string DrawerMenuLink() => "nav-link";

        #endregion

        #region Row

        public virtual string Row() => "row";

        #endregion

        #region Col

        public virtual string Col() => "col";

        public virtual string Col( Size size, ColumnSize columnSize ) => $"{Col()}-{Size( size )}-{ColumnSize( columnSize )}";

        public virtual string Col( Size size, int span ) => $"{Col()}-{Size( size )}-{span}";

        #endregion

        #region Snackbar

        public virtual string Snackbar() => "snackbar";

        public virtual string SnackbarShow() => Show();

        public virtual string SnackbarMultiline() => "snackbar-multi-line";

        public virtual string SnackbarBody() => "snackbar-body";

        public virtual string SnackbarAction() => "snackbar-btn";

        #endregion

        #region Alert

        public virtual string Alert() => "alert";

        public virtual string AlertColor( Color color ) => $"{Alert()}-{Color( color )}";

        public virtual string AlertDismisable() => "alert-dismissible";

        //public virtual string AlertShow( bool show ) => $"alert-dismissible {Fade()} {( show ? Show() : null )}";

        #endregion

        #region Modal

        public virtual string Modal() => "modal";

        public virtual string ModalFade() => $"{Fade()}";

        public virtual string ModalShow() => $"{Show()}";

        public virtual string ModalBackdrop() => "modal-backdrop";

        public virtual string ModalContent( bool isForm ) => "modal-content";

        public virtual string ModalBody() => "modal-body";

        public virtual string ModalHeader() => "modal-header";

        public virtual string ModalFooter() => "modal-footer";

        public virtual string ModalHeaderTitle() => "modal-title";

        #endregion

        #region Pagination

        public virtual string Pagination() => "pagination";

        public virtual string PaginationSize( Size size ) => $"{Pagination()}-{Size( size )}";

        public virtual string PaginationItem() => "page-item";

        public virtual string PaginationLink() => "page-link";

        #endregion

        #region Progress

        public virtual string Progress() => "progress";

        public virtual string ProgressBar() => "progress-bar";

        public virtual string ProgressBarStriped() => "progress-bar-striped";

        public virtual string ProgressBarAnimated() => "progress-bar-animated";

        public virtual string ProgressBarWidth( int width ) => $"w-{width}";

        #endregion

        #region Chart

        public virtual string Chart() => null;

        #endregion

        #region Colors

        public virtual string BackgroundColor( Background color ) => $"bg-{Color( color )}";

        #endregion

        #region Title

        public virtual string Title() => null;

        public virtual string TitleSize( int size ) => $"h{size}";

        #endregion

        #region Table

        public virtual string Table() => "table";

        public virtual string TableFullWidth() => null;

        public virtual string TableStriped() => "table-striped";

        public virtual string TableHoverable() => "table-hover";

        public virtual string TableBordered() => "table-bordered";

        public virtual string TableHeader() => null;

        public virtual string TableHeaderCell() => null;

        public virtual string TableBody() => null;

        public virtual string TableRow() => null;

        public virtual string TableRowHeader() => null;

        public virtual string TableRowCell() => null;

        #endregion

        #region Badge

        public virtual string Badge() => "badge";

        public virtual string BadgeColor( Color color ) => $"{Badge()}-{Color( color )}";

        public virtual string BadgePill() => $"{Badge()}-pill";

        #endregion

        #region Media

        public virtual string Media() => "media";

        public virtual string MediaLeft() => "media-left";

        public virtual string MediaRight() => "media-right";

        public virtual string MediaBody() => "media-body";

        #endregion

        #region SimpleText

        public virtual string SimpleTextColor( TextColor textColor ) => $"text-{TextColor( textColor )}";

        public virtual string SimpleTextAlignment( TextAlignment textAlignment ) => $"text-{TextAlignment( textAlignment )}";

        public virtual string SimpleTextTransform( TextTransform textTransform ) => $"text-{TextTransform( textTransform )}";

        public virtual string SimpleTextWeight( TextWeight textWeight ) => $"font-weight-{TextWeight( textWeight )}";

        public virtual string SimpleTextItalic() => "font-italic";

        #endregion

        #region Figure

        public virtual string Figure() => "figure";

        #endregion

        #region States

        public virtual string Show() => "show";

        public virtual string Fade() => "fade";

        public virtual string Active() => "active";

        public virtual string Disabled() => "disabled";

        public virtual string Collapsed() => "collapsed";

        #endregion

        #region Layout

        public virtual string Spacing( Spacing spacing, Side side, int size )
        {
            var sp = spacing != Blazorise.Spacing.None ? Spacing( spacing ) : null;
            var sd = spacing != Blazorise.Spacing.None && side != Blazorise.Side.None ? Side( side ) : null;
            var sz = spacing != Blazorise.Spacing.None || side != Blazorise.Side.None ? $"-{size}" : null;

            return $"{sp}{sd}{sz}";
        }

        public virtual string Spacing( Spacing spacing, Side side, int size, Breakpoint breakpoint )
        {
            if ( breakpoint != Blazorise.Breakpoint.None )
                return $"{Spacing( spacing )}{Side( side )}-{Breakpoint( breakpoint )}-{size}";

            return $"{Spacing( spacing )}{Side( side )}-{size}";
        }

        public virtual string Margin( Side side, int size, Breakpoint breakpoint )
        {
            if ( breakpoint != Blazorise.Breakpoint.None )
                return $"{Spacing( Blazorise.Spacing.Margin )}{Side( side )}-{Breakpoint( breakpoint )}-{size}";

            return $"{Spacing( Blazorise.Spacing.Margin )}{Side( side )}-{size}";
        }

        public virtual string Padding( Side side, int size, Breakpoint breakpoint )
        {
            if ( breakpoint != Blazorise.Breakpoint.None )
                return $"{Spacing( Blazorise.Spacing.Padding )}{Side( side )}-{Breakpoint( breakpoint )}-{size}";

            return $"{Spacing( Blazorise.Spacing.Padding )}{Side( side )}-{size}";
        }

        #endregion

        #region Flex

        public virtual string FlexAlignment( Alignment alignment ) => $"justify-content-{Alignment( alignment )}";

        #endregion

        #region Enums

        public string Size( Size size )
        {
            switch ( size )
            {
                case Blazorise.Size.ExtraSmall:
                    return "xs";
                case Blazorise.Size.Small:
                    return "sm";
                case Blazorise.Size.Medium:
                    return "md";
                case Blazorise.Size.Large:
                    return "lg";
                case Blazorise.Size.ExtraLarge:
                    return "xl";
                default:
                    return null;
            }
        }

        public string Breakpoint( Breakpoint breakpoint )
        {
            switch ( breakpoint )
            {
                case Blazorise.Breakpoint.Small:
                    return "sm";
                case Blazorise.Breakpoint.Medium:
                    return "md";
                case Blazorise.Breakpoint.Large:
                    return "lg";
                case Blazorise.Breakpoint.ExtraLarge:
                    return "xl";
                default:
                    return null;
            }
        }

        //public string Span( Span span )
        //{
        //    switch ( span )
        //    {
        //        case Blazorise.Span.Auto:
        //            return "auto";
        //        case Blazorise.Span._1:
        //            return "1";
        //        case Blazorise.Span._2:
        //            return "2";
        //        case Blazorise.Span._3:
        //            return "3";
        //        case Blazorise.Span._4:
        //            return "4";
        //        case Blazorise.Span._5:
        //            return "5";
        //        case Blazorise.Span._6:
        //            return "6";
        //        case Blazorise.Span._7:
        //            return "7";
        //        case Blazorise.Span._8:
        //            return "8";
        //        case Blazorise.Span._9:
        //            return "9";
        //        case Blazorise.Span._10:
        //            return "10";
        //        case Blazorise.Span._11:
        //            return "11";
        //        case Blazorise.Span._12:
        //            return "12";
        //        default:
        //            return null;
        //    }
        //}

        public string Color( Color color )
        {
            switch ( color )
            {
                case Blazorise.Color.Active:
                    return "active";
                case Blazorise.Color.Primary:
                    return "primary";
                case Blazorise.Color.Secondary:
                    return "secondary";
                case Blazorise.Color.Success:
                    return "success";
                case Blazorise.Color.Danger:
                    return "danger";
                case Blazorise.Color.Warning:
                    return "warning";
                case Blazorise.Color.Info:
                    return "info";
                case Blazorise.Color.Light:
                    return "light";
                case Blazorise.Color.Dark:
                    return "dark";
                case Blazorise.Color.Link:
                    return "link";
                default:
                    return null;
            }
        }

        public string Color( Background color )
        {
            switch ( color )
            {
                case Blazorise.Background.Primary:
                    return "primary";
                case Blazorise.Background.Secondary:
                    return "secondary";
                case Blazorise.Background.Success:
                    return "success";
                case Blazorise.Background.Danger:
                    return "danger";
                case Blazorise.Background.Warning:
                    return "warning";
                case Blazorise.Background.Info:
                    return "info";
                case Blazorise.Background.Light:
                    return "light";
                case Blazorise.Background.Dark:
                    return "dark";
                case Blazorise.Background.White:
                    return "white";
                case Blazorise.Background.Transparent:
                    return "transparent";
                default:
                    return null;
            }
        }

        public string TextColor( TextColor textColor )
        {
            switch ( textColor )
            {
                case Blazorise.TextColor.Primary:
                    return "primary";
                case Blazorise.TextColor.Secondary:
                    return "secondary";
                case Blazorise.TextColor.Success:
                    return "success";
                case Blazorise.TextColor.Danger:
                    return "danger";
                case Blazorise.TextColor.Warning:
                    return "warning";
                case Blazorise.TextColor.Info:
                    return "info";
                case Blazorise.TextColor.Light:
                    return "light";
                case Blazorise.TextColor.Dark:
                    return "dark";
                case Blazorise.TextColor.Body:
                    return "body";
                case Blazorise.TextColor.Muted:
                    return "muted";
                case Blazorise.TextColor.White:
                    return "white";
                case Blazorise.TextColor.Black50:
                    return "black-50";
                case Blazorise.TextColor.White50:
                    return "white-50";
                default:
                    return null;
            }
        }

        public string Theme( Theme theme )
        {
            switch ( theme )
            {
                case Blazorise.Theme.Light:
                    return "light";
                case Blazorise.Theme.Dark:
                    return "dark";
                default:
                    return null;
            }
        }

        //public virtual string Visibility( Visibility visibility )
        //{
        //    switch ( visibility )
        //    {
        //        case Blazorise.Visibility.Always:
        //            return "d-block";
        //        case Blazorise.Visibility.Never:
        //            return "d-none";
        //        default:
        //            return null;
        //    }
        //}

        public virtual string DropdownDirection( DropdownDirection dropdownDirection )
        {
            switch ( dropdownDirection )
            {
                case Blazorise.DropdownDirection.Up:
                    return "dropup";
                case Blazorise.DropdownDirection.Right:
                    return "dropright";
                case Blazorise.DropdownDirection.Left:
                    return "dropleft";
                case Blazorise.DropdownDirection.Down:
                default:
                    return null;
            }
        }

        public virtual string Float( Float @float )
        {
            switch ( @float )
            {
                case Blazorise.Float.Left:
                    return "float-left";
                case Blazorise.Float.Right:
                    return "float-right";
                default:
                    return null;
            }
        }

        public virtual string Spacing( Spacing spacing )
        {
            switch ( spacing )
            {
                case Blazorise.Spacing.Margin:
                    return "m";
                case Blazorise.Spacing.Padding:
                    return "p";
                default:
                    return null;
            }
        }

        public virtual string Side( Side side )
        {
            switch ( side )
            {
                case Blazorise.Side.Top:
                    return "t";
                case Blazorise.Side.Bottom:
                    return "b";
                case Blazorise.Side.Left:
                    return "l";
                case Blazorise.Side.Right:
                    return "r";
                case Blazorise.Side.X:
                    return "x";
                case Blazorise.Side.Y:
                    return "y";
                default:
                    return null;
            }
        }

        public virtual string Alignment( Alignment alignment )
        {
            switch ( alignment )
            {
                case Blazorise.Alignment.Near:
                    return "start";
                case Blazorise.Alignment.Center:
                    return "center";
                case Blazorise.Alignment.Far:
                    return "end";
                default:
                    return null;
            }
        }

        public virtual string TextAlignment( TextAlignment textAlignment )
        {
            switch ( textAlignment )
            {
                case Blazorise.TextAlignment.Near:
                    return "left";
                case Blazorise.TextAlignment.Center:
                    return "center";
                case Blazorise.TextAlignment.Far:
                    return "right";
                case Blazorise.TextAlignment.Justified:
                    return "justify";
                default:
                    return null;
            }
        }

        public virtual string TextTransform( TextTransform textTransform )
        {
            switch ( textTransform )
            {
                case Blazorise.TextTransform.Lowercase:
                    return "lowercase";
                case Blazorise.TextTransform.Uppercase:
                    return "uppercase";
                case Blazorise.TextTransform.Capitalize:
                    return "capitalize";
                default:
                    return null;
            }
        }

        public virtual string TextWeight( TextWeight textWeight )
        {
            switch ( textWeight )
            {
                case Blazorise.TextWeight.Normal:
                    return "normal";
                case Blazorise.TextWeight.Bold:
                    return "bold";
                case Blazorise.TextWeight.Light:
                    return "light";
                default:
                    return null;
            }
        }

        public virtual string DrawerType( DrawerType drawerType )
        {
            switch ( drawerType )
            {
                case Blazorise.DrawerType.Permanent:
                    return "permanent";
                case Blazorise.DrawerType.Persistent:
                    return "persistent";
                case Blazorise.DrawerType.Temporary:
                    return "temporary";
                default:
                    return "default";
            }
        }

        public virtual string ColumnSize( ColumnSize columnSize )
        {
            switch ( columnSize )
            {
                case Blazorise.ColumnSize.Is1:
                    return "1";
                case Blazorise.ColumnSize.Is2:
                    return "2";
                case Blazorise.ColumnSize.Is3:
                case Blazorise.ColumnSize.Quarter:
                    return "3";
                case Blazorise.ColumnSize.Is4:
                case Blazorise.ColumnSize.Third:
                    return "4";
                case Blazorise.ColumnSize.Is5:
                    return "5";
                case Blazorise.ColumnSize.Is6:
                case Blazorise.ColumnSize.Half:
                    return "6";
                case Blazorise.ColumnSize.Is7:
                    return "7";
                case Blazorise.ColumnSize.Is8:
                    return "8";
                case Blazorise.ColumnSize.Is9:
                    return "9";
                case Blazorise.ColumnSize.Is10:
                    return "10";
                case Blazorise.ColumnSize.Is11:
                    return "11";
                case Blazorise.ColumnSize.Is12:
                case Blazorise.ColumnSize.Full:
                    return "12";
                default:
                    return null;
            }
        }

        public virtual string ModalSize( ModalSize modalSize )
        {
            switch ( modalSize )
            {
                case Blazorise.ModalSize.Small:
                    return "modal-sm";
                case Blazorise.ModalSize.Large:
                    return "modal-lg";
                case Blazorise.ModalSize.Default:
                default:
                    return null;
            }
        }

        #endregion

        public bool Custom { get; set; } = true;

        public virtual FrameworkProvider Provider { get { return FrameworkProvider.Bootstrap; } }
    }
}
