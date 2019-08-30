#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace Blazorise.Frolic
{
    public class FrolicClassProvider : IClassProvider
    {
        #region TextEdit

        public virtual string TextEdit( bool plaintext ) => "e-control";

        public virtual string TextEditSize( Size size ) => $"{TextEdit( false )}-{ToSize( size )}";

        public virtual string TextEditColor( Color color ) => $"text-{ToColor( color )}";

        public virtual string TextEditValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

        #endregion

        #region Memo

        public virtual string MemoEdit() => "e-control";

        public virtual string MemoEditValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

        #endregion

        #region SelectEdit

        public virtual string SelectEdit() => "e-select";

        public virtual string SelectEditSize( Size size ) => $"{SelectEdit()}-{ToSize( size )}";

        public virtual string SelectEditValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

        #endregion

        #region DateEdit

        public virtual string DateEdit() => "e-control";

        public virtual string DateEditSize( Size size ) => $"{DateEdit()}-{ToSize( size )}";

        public virtual string DateEditValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

        #endregion

        #region CheckEdit

        public virtual string CheckEdit() => null;

        public virtual string CheckEditInline() => null;

        public virtual string CheckEditCursor( Cursor cursor ) => $"e-check-{ToCursor( cursor )}";

        public virtual string CheckEditValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

        #endregion

        #region RadioEdit

        public virtual string RadioEdit() => null;

        public virtual string RadioInline() => null;

        #endregion

        #region FileEdit

        public virtual string FileEdit() => "e-control";

        public virtual string FileEditValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

        #endregion

        #region Label

        public virtual string Label() => "e-label";

        public virtual string LabelCursor( Cursor cursor ) => $"e-label-{ToCursor( cursor )}";

        public virtual string LabelCheck() => null;

        public virtual string LabelFile() => "e-label";

        #endregion

        #region Help

        public virtual string Help() => "e-form-info";

        #endregion

        #region Validation

        public string ValidationSuccess() => "e-form-info text-success";

        public string ValidationSuccessTooltip() => "e-form-info text-success";

        public string ValidationError() => "e-form-info text-danger";

        public string ValidationErrorTooltip() => "e-form-info text-danger";

        #endregion

        #region Fields

        public virtual string Fields() => "e-cols";

        public virtual string FieldsBody() => null;

        public virtual string FieldsColumn() => $"{Col()}";

        #endregion

        #region Field

        public virtual string Field() => "e-form-group";

        public virtual string FieldHorizontal() => "e-cols no-gap";

        public virtual string FieldColumn() => $"{Col()}";

        public virtual string FieldJustifyContent( JustifyContent justifyContent ) => ToJustifyContent( justifyContent );

        #endregion

        #region FieldLabel

        public virtual string FieldLabel() => "e-label";

        public virtual string FieldLabelHorizontal() => "e-form-group";

        #endregion

        #region FieldBody

        public virtual string FieldBody() => null;

        #endregion

        #region FieldHelp

        public virtual string FieldHelp() => "form-text text-muted";

        #endregion

        #region Control

        public virtual string ControlCheck() => UseCustomInputStyles ? "custom-control custom-checkbox" : "form-check";

        public virtual string ControlRadio() => UseCustomInputStyles ? "custom-control custom-radio" : "form-check";

        public virtual string ControlFile() => UseCustomInputStyles ? "custom-control custom-file" : "form-group";

        public virtual string ControlText() => null;

        #endregion

        #region Addons

        public virtual string Addons() => "e-form-group unified";

        public virtual string Addon( AddonType addonType ) => "e-control-helper";

        public virtual string AddonLabel() => null;

        public virtual string AddonContainer() => null;

        #endregion

        #region Inline

        public virtual string Inline() => "form-inline";

        #endregion

        #region Button

        public virtual string Button() => "e-btn";

        public virtual string ButtonColor( Color color ) => ToColor( color );

        public virtual string ButtonOutline( Color color ) => color != Blazorise.Color.None ? $"outlined {ToColor( color )}" : $"outlined";

        public virtual string ButtonSize( ButtonSize buttonSize )
        {
            switch ( buttonSize )
            {
                case Blazorise.ButtonSize.Small:
                    return "small";
                case Blazorise.ButtonSize.Large:
                    return "plus";
                default:
                    return null;
            }
        }

        public virtual string ButtonBlock() => $"fullwidth";

        public virtual string ButtonActive() => "active";

        public virtual string ButtonLoading() => "anime";

        #endregion

        #region Buttons

        //public virtual string Buttons() => "btn-group";

        public virtual string ButtonsAddons() => "e-buttons unified";

        public virtual string ButtonsToolbar() => "e-toolbar";

        public virtual string ButtonsSize( ButtonsSize buttonsSize )
        {
            switch ( buttonsSize )
            {
                case Blazorise.ButtonsSize.Small:
                    return "small";
                case Blazorise.ButtonsSize.Large:
                    return "plus";
                default:
                    return null;
            }
        }

        public virtual string ButtonsVertical() => "btn-group-vertical";

        #endregion

        #region CloseButton

        public virtual string CloseButton() => "e-delete";

        #endregion

        #region Dropdown

        public virtual string Dropdown() => "e-dropdown";

        public virtual string DropdownGroup() => "btn-group";

        public virtual string DropdownShow() => Show();

        public virtual string DropdownRight() => "drop-right";

        public virtual string DropdownItem() => "drop-item";

        public virtual string DropdownItemActive() => Active();

        public virtual string DropdownDivider() => "dropdown-divider";

        public virtual string DropdownMenu() => "drop-items";

        public virtual string DropdownMenuBody() => null;

        public virtual string DropdownMenuShow() => Show();

        public virtual string DropdownMenuRight() => null;

        public virtual string DropdownToggle() => "button dropdown";

        public virtual string DropdownToggleColor( Color color ) => ToColor( color );

        public virtual string DropdownToggleOutline( Color color ) => color != Blazorise.Color.None ? $"outlined {ToColor( color )}" : $"outlined";

        public virtual string DropdownToggleSize( ButtonSize buttonSize )
        {
            switch ( buttonSize )
            {
                case Blazorise.ButtonSize.Small:
                    return "small";
                case Blazorise.ButtonSize.Large:
                    return "plus";
                default:
                    return null;
            }
        }

        public virtual string DropdownToggleSplit() => "button split";

        public virtual string DropdownDirection( Direction direction )
        {
            switch ( direction )
            {
                case Direction.Up:
                    return "dropup";
                case Direction.Right:
                    return "dropright";
                case Direction.Left:
                    return "dropleft";
                case Direction.Down:
                case Direction.None:
                default:
                    return null;
            }
        }

        #endregion

        #region Tab

        public virtual string Tabs() => "e-tabs";

        public virtual string TabsCards() => "card-header-tabs";

        public virtual string TabsPills() => "nav-pills";

        public virtual string TabsFullWidth() => "nav-fill";

        public virtual string TabsJustified() => "nav-justified";

        public virtual string TabsVertical() => "vertical";

        public virtual string TabItem() => null;

        public virtual string TabItemActive() => Active();

        public virtual string TabLink() => null;

        public virtual string TabLinkActive() => null;

        public virtual string TabsContent() => "e-tabs-content";

        public virtual string TabPanel() => "e-tabs-panel";

        public virtual string TabPanelActive() => Active();

        #endregion

        #region Card

        public virtual string CardGroup() => "e-cards unified";

        public virtual string Card() => "e-card";

        public virtual string CardWhiteText() => "text-white";

        public virtual string CardBackground( Background background ) => ToBackground( background );

        public virtual string CardActions() => "card-actions";

        public virtual string CardBody() => "card-body";

        public virtual string CardFooter() => "card-body";

        public virtual string CardHeader() => "card-body";

        public virtual string CardImage() => null;

        public virtual string CardTitle() => "card-title";

        public virtual string CardSubtitle() => "card-subtitle";

        public virtual string CardSubtitleSize( int size ) => null;

        public virtual string CardText() => "card-text";

        public virtual string CardLink() => "card-link";

        #endregion

        #region ListGroup

        public virtual string ListGroup() => "e-list";

        public virtual string ListGroupFlush() => "no-border";

        public virtual string ListGroupItem() => "e-list-item";

        public virtual string ListGroupItemActive() => Active();

        public virtual string ListGroupItemDisabled() => Disabled();

        #endregion

        #region Container

        public virtual string Container() => "e-container";

        public virtual string ContainerFluid() => "e-container-fluid";

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

        public virtual string Bar() => "e-nav";

        public virtual string BarThemeContrast( ThemeContrast themeContrast ) => $"navbar-{ToThemeContrast( themeContrast )}";

        public virtual string BarBreakpoint( Breakpoint breakpoint ) => $"navbar-expand-{ToBreakpoint( breakpoint )}";

        public virtual string BarItem() => null;

        public virtual string BarItemActive() => null;

        public virtual string BarItemDisabled() => Disabled();

        public virtual string BarItemHasDropdown() => "dropdown";

        public virtual string BarItemHasDropdownShow() => Show();

        public virtual string BarLink() => "e-menu-item";

        public virtual string BarLinkDisabled() => Disabled();

        //public virtual string BarCollapse() => "navbar-collapse";

        public virtual string BarBrand() => "navbar-brand";

        public virtual string BarToggler() => "e-btn no-shadow no-desktop";

        public virtual string BarTogglerCollapsed( bool isShow ) => isShow ? Show() : null;

        public virtual string BarMenu() => "e-menu";

        public virtual string BarMenuShow() => Show();

        public virtual string BarStart() => "e-distribution";

        public virtual string BarEnd() => "e-distribution";

        //public virtual string BarHasDropdown() => "dropdown";

        public virtual string BarDropdown() => "e-dropdown";

        public virtual string BarDropdownShow() => null;

        public virtual string BarDropdownToggle() => null;

        public virtual string BarDropdownItem() => "drop-item";

        public virtual string BarTogglerIcon() => "navbar-toggler-icon";

        public virtual string BarDropdownMenu() => "drop-items";

        public virtual string BarDropdownMenuShow() => Show();

        public virtual string BarDropdownMenuRight() => "drop-items-right";

        #endregion

        #region Accordion

        public virtual string Accordion() => "accordion";

        #endregion

        #region Collapse

        public virtual string Collapse() => "collapse";

        public virtual string CollapseShow() => Show();

        #endregion

        #region Row

        public virtual string Row() => "e-cols";

        #endregion

        #region Col

        public virtual string Col() => "e-col";

        public virtual string Col( ColumnWidth columnWidth, IEnumerable<(Breakpoint breakpoint, bool offset)> rules ) =>
            string.Join( " ", rules.Select( r => Col( columnWidth, r.breakpoint, r.offset ) ) );

        private string Col( ColumnWidth columnWidth, Breakpoint breakpoint, bool offset )
        {
            var baseClass = offset ? "offset" : Col();

            if ( breakpoint != Blazorise.Breakpoint.None )
            {
                if ( columnWidth == Blazorise.ColumnWidth.None )
                    return $"{baseClass}-{ToBreakpoint( breakpoint )}";

                return $"{baseClass}-{ToBreakpoint( breakpoint )}-{ToColumnWidth( columnWidth )}";
            }

            //if ( columnWidth == Blazorise.ColumnWidth.Auto )
            //    return $"{baseClass}";

            return $"{baseClass}-{ToColumnWidth( columnWidth )}";
        }

        #endregion

        #region Alert

        public virtual string Alert() => "e-alert";

        public virtual string AlertColor( Color color ) => ToColor( color );

        public virtual string AlertDismisable() => null;

        //public virtual string AlertShow( bool show ) => $"alert-dismissible {Fade()} {( show ? Show() : null )}";

        #endregion

        #region Modal

        public virtual string Modal() => "e-modal";

        public virtual string ModalFade() => "e-modal-e";

        public virtual string ModalShow() => "launch";

        public virtual string ModalBackdrop() => "e-modal-backdrop";

        public virtual string ModalContent( bool isForm ) => "e-modal-content";

        public virtual string ModalContentCentered() => "modal-dialog-centered";

        public virtual string ModalBody() => "e-modal-body";

        public virtual string ModalHeader() => "e-modal-header";

        public virtual string ModalFooter() => "e-modal-footer";

        public virtual string ModalTitle() => "e-modal-title";

        #endregion

        #region Pagination

        public virtual string Pagination() => "e-pagination";

        public virtual string PaginationSize( Size size ) => $"{Pagination()}-{ToSize( size )}";

        public virtual string PaginationItem() => "e-page-item";

        public virtual string PaginationItemActive() => "on-page";

        public virtual string PaginationItemDisabled() => Disabled();

        public virtual string PaginationLink() => null;

        public virtual string PaginationLinkActive() => null;

        public virtual string PaginationLinkDisabled() => null;

        #endregion

        #region Progress

        public virtual string Progress() => null;

        public virtual string ProgressSize( Size size ) => null;

        public virtual string ProgressBar() => "e-progress";

        public virtual string ProgressBarColor( Background background ) => ToBackground( background );

        public virtual string ProgressBarStriped() => "progress-bar-striped";

        public virtual string ProgressBarAnimated() => "progress-bar-animated";

        public virtual string ProgressBarWidth( int width ) => $"w-{width}";

        #endregion

        #region Chart

        public virtual string Chart() => null;

        #endregion

        #region Colors

        public virtual string BackgroundColor( Background color ) => $"bg-{ToBackground( color )}";

        #endregion

        #region Title

        public virtual string Title() => "e-title";

        public virtual string TitleSize( int size ) => $"h{size}";

        #endregion

        #region Table

        public virtual string Table() => "e-table";

        public virtual string TableFullWidth() => null;

        public virtual string TableStriped() => "striped";

        public virtual string TableHoverable() => "hovered";

        public virtual string TableBordered() => "bordered";

        public virtual string TableNarrow() => "narrowed";

        public virtual string TableBorderless() => "borderless";

        public virtual string TableHeader() => "e-thead";

        public virtual string TableHeaderThemeContrast( ThemeContrast themeContrast ) => ToThemeContrast( themeContrast );

        public virtual string TableHeaderCell() => null;

        public virtual string TableFooter() => null;

        public virtual string TableBody() => null;

        public virtual string TableRow() => "e-row";

        public virtual string TableRowColor( Color color ) => ToColor( color );

        public virtual string TableRowBackground( Background background ) => ToBackground( background );

        public virtual string TableRowTextColor( TextColor textColor ) => $"text-{ToTextColor( textColor )}";

        public virtual string TableRowIsSelected() => "selected";

        public virtual string TableRowHeader() => null;

        public virtual string TableRowCell() => null;

        public virtual string TableRowCellColor( Color color ) => ToColor( color );

        public virtual string TableRowCellBackground( Background background ) => ToBackground( background );

        public virtual string TableRowCellTextColor( TextColor textColor ) => $"text-{ToTextColor( textColor )}";

        #endregion

        #region Badge

        public virtual string Badge() => "e-tag";

        public virtual string BadgeColor( Color color ) => $"{Badge()} {ToColor( color )}";

        public virtual string BadgePill() => $"{Badge()} rounded";

        #endregion

        #region Media

        public virtual string Media() => "e-media";

        public virtual string MediaLeft() => "media-left";

        public virtual string MediaRight() => "media-right";

        public virtual string MediaBody() => "e-media-body";

        #endregion

        #region Text

        public virtual string TextColor( TextColor textColor ) => $"text-{ToTextColor( textColor )}";

        public virtual string TextAlignment( TextAlignment textAlignment ) => $"text-{ToTextAlignment( textAlignment )}";

        public virtual string TextTransform( TextTransform textTransform ) => $"text-{ToTextTransform( textTransform )}";

        public virtual string TextWeight( TextWeight textWeight ) => $"font-weight-{ToTextWeight( textWeight )}";

        public virtual string TextItalic() => "font-italic";

        #endregion

        #region Heading

        public virtual string HeadingSize( HeadingSize headingSize ) => null;

        public virtual string HeadingTextColor( TextColor textColor ) => $"text-{ToTextColor( textColor )}";

        #endregion

        #region DisplayHeading

        public virtual string DisplayHeadingSize( DisplayHeadingSize displayHeadingSize ) => $"size-{ToDisplayHeadingSize( displayHeadingSize )}";

        #endregion

        #region Paragraph

        public virtual string Paragraph() => null;

        public virtual string ParagraphColor( TextColor textColor ) => $"text-{ToTextColor( textColor )}";

        #endregion

        #region Figure

        public virtual string Figure() => "figure";

        #endregion

        #region Breadcrumb

        public virtual string Breadcrumb() => "e-breadcrumb";

        public virtual string BreadcrumbItem() => null;

        public virtual string BreadcrumbItemActive() => Active();

        public virtual string BreadcrumbLink() => null;

        #endregion

        #region Tooltip

        public virtual string Tooltip() => "b-tooltip";

        public virtual string TooltipPlacement( Placement placement ) => $"b-tooltip-{ToPlacement( placement )}";

        #endregion

        #region States

        public virtual string Show() => "show";

        public virtual string Fade() => "fade";

        public virtual string Active() => "active";

        public virtual string Disabled() => "disabled";

        public virtual string Collapsed() => "collapsed";

        #endregion

        #region Layout

        public virtual string Spacing( Spacing spacing, SpacingSize spacingSize, Side side, Breakpoint breakpoint )
        {
            if ( breakpoint != Blazorise.Breakpoint.None )
                return $"{ToSpacing( spacing )}{ToSide( side )}-{ToBreakpoint( breakpoint )}-{ToSpacingSize( spacingSize )}";

            return $"{ToSpacing( spacing )}{ToSide( side )}-{ToSpacingSize( spacingSize )}";
        }

        public virtual string Spacing( Spacing spacing, SpacingSize spacingSize, IEnumerable<(Side side, Breakpoint breakpoint)> rules ) => string.Join( " ", rules.Select( x => Spacing( spacing, spacingSize, x.side, x.breakpoint ) ) );

        #endregion

        #region Flex

        public virtual string FlexAlignment( Alignment alignment ) => $"justify-content-{ToAlignment( alignment )}";

        #endregion

        #region Enums

        public virtual string ToSize( Size size )
        {
            switch ( size )
            {
                case Blazorise.Size.ExtraSmall:
                    return "tiny";
                case Blazorise.Size.Small:
                    return "small";
                case Blazorise.Size.Medium:
                    return null;
                case Blazorise.Size.Large:
                case Blazorise.Size.ExtraLarge:
                    return "large";
                default:
                    return null;
            }
        }

        public virtual string ToBreakpoint( Breakpoint breakpoint )
        {
            switch ( breakpoint )
            {
                case Blazorise.Breakpoint.Mobile:
                    return "xs";
                case Blazorise.Breakpoint.Tablet:
                    return "sm";
                case Blazorise.Breakpoint.Desktop:
                    return "md";
                case Blazorise.Breakpoint.Widescreen:
                    return "lg";
                case Blazorise.Breakpoint.FullHD:
                    return "xl";
                default:
                    return null;
            }
        }

        public virtual string ToColor( Color color )
        {
            switch ( color )
            {
                case Blazorise.Color.Primary:
                    return "primary";
                case Blazorise.Color.Secondary:
                    return "gray";
                case Blazorise.Color.Success:
                    return "success";
                case Blazorise.Color.Danger:
                    return "danger";
                case Blazorise.Color.Warning:
                    return "warning";
                case Blazorise.Color.Info:
                    return "sky";
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

        public virtual string ToBackground( Background color )
        {
            switch ( color )
            {
                case Blazorise.Background.Primary:
                    return "primary";
                case Blazorise.Background.Secondary:
                    return "gray";
                case Blazorise.Background.Success:
                    return "success";
                case Blazorise.Background.Danger:
                    return "danger";
                case Blazorise.Background.Warning:
                    return "warning";
                case Blazorise.Background.Info:
                    return "sky";
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

        public virtual string ToTextColor( TextColor textColor )
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

        public virtual string ToThemeContrast( ThemeContrast themeContrast )
        {
            switch ( themeContrast )
            {
                case Blazorise.ThemeContrast.Light:
                    return "light";
                case Blazorise.ThemeContrast.Dark:
                    return "dark";
                default:
                    return null;
            }
        }

        public virtual string ToFloat( Float @float )
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

        public virtual string ToSpacing( Spacing spacing )
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

        public virtual string ToSide( Side side )
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

        public virtual string ToAlignment( Alignment alignment )
        {
            switch ( alignment )
            {
                case Blazorise.Alignment.Start:
                    return "start";
                case Blazorise.Alignment.Center:
                    return "center";
                case Blazorise.Alignment.End:
                    return "end";
                default:
                    return null;
            }
        }

        public virtual string ToTextAlignment( TextAlignment textAlignment )
        {
            switch ( textAlignment )
            {
                case Blazorise.TextAlignment.Left:
                    return "left";
                case Blazorise.TextAlignment.Center:
                    return "center";
                case Blazorise.TextAlignment.Right:
                    return "right";
                case Blazorise.TextAlignment.Justified:
                    return "justify";
                default:
                    return null;
            }
        }

        public virtual string ToTextTransform( TextTransform textTransform )
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

        public virtual string ToTextWeight( TextWeight textWeight )
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

        public virtual string ToColumnWidth( ColumnWidth columnWidth )
        {
            switch ( columnWidth )
            {
                case Blazorise.ColumnWidth.Is1:
                    return "1";
                case Blazorise.ColumnWidth.Is2:
                    return "2";
                case Blazorise.ColumnWidth.Is3:
                case Blazorise.ColumnWidth.Quarter:
                    return "3";
                case Blazorise.ColumnWidth.Is4:
                case Blazorise.ColumnWidth.Third:
                    return "4";
                case Blazorise.ColumnWidth.Is5:
                    return "5";
                case Blazorise.ColumnWidth.Is6:
                case Blazorise.ColumnWidth.Half:
                    return "6";
                case Blazorise.ColumnWidth.Is7:
                    return "7";
                case Blazorise.ColumnWidth.Is8:
                    return "8";
                case Blazorise.ColumnWidth.Is9:
                    return "9";
                case Blazorise.ColumnWidth.Is10:
                    return "10";
                case Blazorise.ColumnWidth.Is11:
                    return "11";
                case Blazorise.ColumnWidth.Is12:
                case Blazorise.ColumnWidth.Full:
                    return "12";
                case Blazorise.ColumnWidth.Auto:
                    return "auto";
                default:
                    return null;
            }
        }

        public virtual string ToModalSize( ModalSize modalSize )
        {
            switch ( modalSize )
            {
                case Blazorise.ModalSize.Small:
                    return "modal-sm";
                case Blazorise.ModalSize.Large:
                    return "modal-lg";
                case Blazorise.ModalSize.ExtraLarge:
                    return "modal-xl";
                case Blazorise.ModalSize.Default:
                default:
                    return null;
            }
        }

        public virtual string ToSpacingSize( SpacingSize spacingSize )
        {
            switch ( spacingSize )
            {
                case Blazorise.SpacingSize.Is0:
                    return "0";
                case Blazorise.SpacingSize.Is1:
                    return "5";
                case Blazorise.SpacingSize.Is2:
                    return "4";
                case Blazorise.SpacingSize.Is3:
                    return "3";
                case Blazorise.SpacingSize.Is4:
                    return "3";
                case Blazorise.SpacingSize.Is5:
                    return "1";
                case Blazorise.SpacingSize.IsAuto:
                    return "auto";
                default:
                    return null;
            }
        }

        public virtual string ToJustifyContent( JustifyContent justifyContent )
        {
            switch ( justifyContent )
            {
                case Blazorise.JustifyContent.Start:
                    return "justify-content-start";
                case Blazorise.JustifyContent.End:
                    return "justify-content-end";
                case Blazorise.JustifyContent.Center:
                    return "justify-content-center";
                case Blazorise.JustifyContent.Between:
                    return "justify-content-between";
                case Blazorise.JustifyContent.Around:
                    return "justify-content-around";
                default:
                    return null;
            }
        }

        public virtual string ToScreenreader( Screenreader screenreader )
        {
            switch ( screenreader )
            {
                case Blazorise.Screenreader.Only:
                    return "sr-only";
                case Blazorise.Screenreader.OnlyFocusable:
                    return "sr-only-focusable";
                default:
                    return null;
            }
        }

        public virtual string ToHeadingSize( HeadingSize headingSize )
        {
            switch ( headingSize )
            {
                case Blazorise.HeadingSize.Is1:
                    return "1";
                case Blazorise.HeadingSize.Is2:
                    return "2";
                case Blazorise.HeadingSize.Is3:
                    return "3";
                case Blazorise.HeadingSize.Is4:
                    return "4";
                case Blazorise.HeadingSize.Is5:
                    return "5";
                case Blazorise.HeadingSize.Is6:
                    return "6";
                default:
                    return null;
            }
        }

        public virtual string ToDisplayHeadingSize( DisplayHeadingSize displayHeadingSize )
        {
            switch ( displayHeadingSize )
            {
                case Blazorise.DisplayHeadingSize.Is1:
                    return "1";
                case Blazorise.DisplayHeadingSize.Is2:
                    return "2";
                case Blazorise.DisplayHeadingSize.Is3:
                    return "3";
                case Blazorise.DisplayHeadingSize.Is4:
                    return "4";
                default:
                    return null;
            }
        }

        public string ToPlacement( Placement placement )
        {
            switch ( placement )
            {
                case Blazorise.Placement.Bottom:
                    return "bottom";
                case Blazorise.Placement.Left:
                    return "left";
                case Blazorise.Placement.Right:
                    return "right";
                case Blazorise.Placement.Top:
                default:
                    return "top";
            }
        }

        public virtual string ToValidationStatus( ValidationStatus validationStatus )
        {
            switch ( validationStatus )
            {
                case Blazorise.ValidationStatus.Success:
                    return "is-valid";
                case Blazorise.ValidationStatus.Error:
                    return "is-invalid";
                default:
                    return null;
            }
        }

        public virtual string ToCursor( Cursor cursor )
        {
            switch ( cursor )
            {
                case Blazorise.Cursor.Pointer:
                    return "pointer";
                default:
                    return null;
            }
        }

        #endregion

        public virtual bool UseCustomInputStyles { get; set; } = true;

        public virtual string Provider => "eFrolic";
    }
}
