﻿#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace Blazorise.Bootstrap
{
    public class BootstrapClassProvider : IClassProvider
    {
        #region TextEdit

        public virtual string TextEdit( bool plaintext ) => plaintext ? "form-control-plaintext" : "form-control";

        public virtual string TextEditSize( Size size ) => $"{TextEdit( false )}-{ToSize( size )}";

        public virtual string TextEditColor( Color color ) => $"text-{ToColor( color )}";

        public virtual string TextEditValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

        #endregion

        #region MemoEdit

        public virtual string MemoEdit() => "form-control";

        public virtual string MemoEditValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

        #endregion

        #region SelectEdit

        public virtual string SelectEdit() => UseCustomInputStyles ? "custom-select" : "form-control";

        public virtual string SelectEditSize( Size size ) => $"{SelectEdit()}-{ToSize( size )}";

        public virtual string SelectEditValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

        #endregion

        #region DateEdit

        public virtual string DateEdit() => "form-control";

        public virtual string DateEditSize( Size size ) => $"{DateEdit()}-{ToSize( size )}";

        public virtual string DateEditValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

        #endregion

        #region CheckEdit

        public virtual string CheckEdit() => UseCustomInputStyles ? "custom-control-input" : "form-check-input";

        public virtual string CheckEditInline() => UseCustomInputStyles ? "custom-control-inline" : "form-check-inline";

        public virtual string CheckEditCursor( Cursor cursor ) => $"{CheckEdit()}-{ToCursor( cursor )}";

        public virtual string CheckEditValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

        #endregion

        #region RadioEdit

        public virtual string RadioEdit() => UseCustomInputStyles ? "custom-control-input" : "form-check-input";

        public virtual string RadioInline() => UseCustomInputStyles ? "custom-control-inline" : "form-check-inline";

        #endregion

        #region FileEdit

        public virtual string FileEdit() => UseCustomInputStyles ? "custom-file-input" : "form-control-file";

        public virtual string FileEditValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

        #endregion

        #region Label

        public virtual string Label() => null;

        public virtual string LabelCursor( Cursor cursor ) => UseCustomInputStyles ? $"custom-control-label-{ToCursor( cursor )}" : $"form-check-label-{ToCursor( cursor )}";

        public virtual string LabelCheck() => UseCustomInputStyles ? "custom-control-label" : "form-check-label";

        public virtual string LabelFile() => UseCustomInputStyles ? "custom-file-label" : null;

        #endregion

        #region Help

        public virtual string Help() => "form-text text-muted";

        #endregion

        #region Validation

        public virtual string ValidationSuccess() => "valid-feedback";

        public virtual string ValidationSuccessTooltip() => "valid-tooltip";

        public virtual string ValidationError() => "invalid-feedback";

        public virtual string ValidationErrorTooltip() => "invalid-tooltip";

        public virtual string ValidationNone() => "form-text text-muted";

        #endregion

        #region Fields

        public virtual string Fields() => "form-row";

        public virtual string FieldsBody() => null;

        public virtual string FieldsColumn() => $"{Col()}";

        #endregion

        #region Field

        public virtual string Field() => "form-group";

        public virtual string FieldHorizontal() => "row";

        public virtual string FieldColumn() => $"{Col()}";

        public virtual string FieldJustifyContent( JustifyContent justifyContent ) => ToJustifyContent( justifyContent );

        #endregion

        #region FieldLabel

        public virtual string FieldLabel() => null;

        public virtual string FieldLabelHorizontal() => "col-form-label";

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

        //public virtual string AddonContainer() => null;

        #endregion

        #region Inline

        public virtual string Inline() => "form-inline";

        #endregion

        #region Button

        public virtual string Button() => "btn";

        public virtual string ButtonColor( Color color ) => $"{Button()}-{ToColor( color )}";

        public virtual string ButtonOutline( Color color ) => color != Blazorise.Color.None ? $"{Button()}-outline-{ToColor( color )}" : $"{Button()}-outline";

        public virtual string ButtonSize( ButtonSize buttonSize )
        {
            switch ( buttonSize )
            {
                case Blazorise.ButtonSize.Small:
                    return "btn-sm";
                case Blazorise.ButtonSize.Large:
                    return "btn-lg";
                default:
                    return null;
            }
        }

        public virtual string ButtonBlock() => $"{Button()}-block";

        public virtual string ButtonActive() => "active";

        public virtual string ButtonLoading() => null;

        #endregion

        #region Buttons

        //public virtual string Buttons() => "btn-group";

        public virtual string ButtonsAddons() => "btn-group";

        public virtual string ButtonsToolbar() => "btn-toolbar";

        public virtual string ButtonsSize( ButtonsSize buttonsSize )
        {
            switch ( buttonsSize )
            {
                case Blazorise.ButtonsSize.Small:
                    return "btn-group-sm";
                case Blazorise.ButtonsSize.Large:
                    return "btn-group-lg";
                default:
                    return null;
            }
        }

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

        public virtual string DropdownItemActive() => Active();

        public virtual string DropdownDivider() => "dropdown-divider";

        public virtual string DropdownMenu() => "dropdown-menu";

        //public virtual string DropdownMenuBody() => null;

        public virtual string DropdownMenuShow() => Show();

        public virtual string DropdownMenuRight() => "dropdown-menu-right";

        public virtual string DropdownToggle() => "btn dropdown-toggle";

        public virtual string DropdownToggleColor( Color color ) => $"{Button()}-{ToColor( color )}";

        public virtual string DropdownToggleOutline( Color color ) => color != Blazorise.Color.None ? $"{Button()}-outline-{ToColor( color )}" : $"{Button()}-outline";

        public virtual string DropdownToggleSize( ButtonSize buttonSize )
        {
            switch ( buttonSize )
            {
                case Blazorise.ButtonSize.Small:
                    return "btn-sm";
                case Blazorise.ButtonSize.Large:
                    return "btn-lg";
                default:
                    return null;
            }
        }

        public virtual string DropdownToggleSplit() => "dropdown-toggle-split";

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

        public virtual string Tabs() => "nav nav-tabs";

        public virtual string TabsCards() => "card-header-tabs";

        public virtual string TabsPills() => "nav-pills";

        public virtual string TabsFullWidth() => "nav-fill";

        public virtual string TabsJustified() => "nav-justified";

        public virtual string TabsVertical() => "flex-column";

        public virtual string TabItem() => "nav-item";

        public virtual string TabItemActive() => null;

        public virtual string TabLink() => "nav-link";

        public virtual string TabLinkActive() => $"{Active()} {Show()}";

        public virtual string TabsContent() => "tab-content";

        public virtual string TabPanel() => "tab-pane";

        public virtual string TabPanelActive() => $"{Active()} {Show()}";

        #endregion

        #region Card

        public virtual string CardGroup() => "card-group";

        public virtual string Card() => "card";

        public virtual string CardWhiteText() => "text-white";

        public virtual string CardBackground( Background background ) => BackgroundColor( background );

        public virtual string CardActions() => "card-actions";

        public virtual string CardBody() => "card-body";

        public virtual string CardFooter() => "card-footer";

        public virtual string CardHeader() => "card-header";

        public virtual string CardImage() => "card-img-top";

        public virtual string CardTitle() => "card-title";

        public virtual string CardSubtitle() => "card-subtitle";

        public virtual string CardSubtitleSize( int size ) => null;

        public virtual string CardText() => "card-text";

        public virtual string CardLink() => "card-link";

        #endregion

        #region ListGroup

        public virtual string ListGroup() => "list-group";

        public virtual string ListGroupFlush() => "list-group-flush";

        public virtual string ListGroupItem() => "list-group-item";

        public virtual string ListGroupItemActive() => Active();

        public virtual string ListGroupItemDisabled() => Disabled();

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

        public virtual string BarThemeContrast( ThemeContrast themeContrast ) => $"navbar-{ToThemeContrast( themeContrast )}";

        public virtual string BarBreakpoint( Breakpoint breakpoint ) => $"navbar-expand-{ToBreakpoint( breakpoint )}";

        public virtual string BarItem() => "nav-item";

        public virtual string BarItemActive() => Active();

        public virtual string BarItemDisabled() => Disabled();

        public virtual string BarItemHasDropdown() => "dropdown";

        public virtual string BarItemHasDropdownShow() => Show();

        public virtual string BarLink() => "nav-link";

        public virtual string BarLinkDisabled() => Disabled();

        //public virtual string BarCollapse() => "navbar-collapse";

        public virtual string BarBrand() => "navbar-brand";

        public virtual string BarToggler() => "navbar-toggler";

        public virtual string BarTogglerCollapsed( bool isShow ) => isShow ? null : "collapsed";

        public virtual string BarMenu() => "collapse navbar-collapse";

        public virtual string BarMenuShow() => Show();

        public virtual string BarStart() => "navbar-nav mr-auto";

        public virtual string BarEnd() => "navbar-nav";

        //public virtual string BarHasDropdown() => "dropdown";

        public virtual string BarDropdown() => null;

        public virtual string BarDropdownShow() => null;

        public virtual string BarDropdownToggle() => "nav-link dropdown-toggle";

        public virtual string BarDropdownItem() => "dropdown-item";

        public virtual string BarTogglerIcon() => "navbar-toggler-icon";

        public virtual string BarDropdownMenu() => "dropdown-menu";

        public virtual string BarDropdownMenuShow() => Show();

        public virtual string BarDropdownMenuRight() => "dropdown-menu-right";

        #endregion

        #region Accordion

        public virtual string Accordion() => "accordion";

        #endregion

        #region Collapse

        public virtual string Collapse() => "collapse";

        public virtual string CollapseShow() => Show();

        #endregion

        #region Row

        public virtual string Row() => "row";

        #endregion

        #region Col

        public virtual string Col() => "col";

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

        public virtual string Alert() => "alert";

        public virtual string AlertColor( Color color ) => $"{Alert()}-{ToColor( color )}";

        public virtual string AlertDismisable() => "alert-dismissible";

        //public virtual string AlertShow( bool show ) => $"alert-dismissible {Fade()} {( show ? Show() : null )}";

        #endregion

        #region Modal

        public virtual string Modal() => "modal";

        public virtual string ModalFade() => $"{Fade()}";

        public virtual string ModalShow() => $"{Show()}";

        public virtual string ModalBackdrop() => "modal-backdrop";

        public virtual string ModalContent( bool isForm ) => "modal-content";

        public virtual string ModalContentCentered() => "modal-dialog-centered";

        public virtual string ModalBody() => "modal-body";

        public virtual string ModalHeader() => "modal-header";

        public virtual string ModalFooter() => "modal-footer";

        public virtual string ModalTitle() => "modal-title";

        #endregion

        #region Pagination

        public virtual string Pagination() => "pagination";

        public virtual string PaginationSize( Size size ) => $"{Pagination()}-{ToSize( size )}";

        public virtual string PaginationItem() => "page-item";

        public virtual string PaginationItemActive() => Active();

        public virtual string PaginationItemDisabled() => Disabled();

        public virtual string PaginationLink() => "page-link";

        public virtual string PaginationLinkActive() => null;

        public virtual string PaginationLinkDisabled() => null;

        #endregion

        #region Progress

        public virtual string Progress() => "progress";

        public virtual string ProgressSize( Size size ) => $"progress-{ToSize( size )}";

        public virtual string ProgressBar() => "progress-bar";

        public virtual string ProgressBarColor( Background background ) => BackgroundColor( background );

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

        public virtual string Title() => null;

        public virtual string TitleSize( int size ) => $"h{size}";

        #endregion

        #region Table

        public virtual string Table() => "table";

        public virtual string TableFullWidth() => null;

        public virtual string TableStriped() => "table-striped";

        public virtual string TableHoverable() => "table-hover";

        public virtual string TableBordered() => "table-bordered";

        public virtual string TableNarrow() => "table-sm";

        public virtual string TableBorderless() => "table-borderless";

        public virtual string TableHeader() => null;

        public virtual string TableHeaderThemeContrast( ThemeContrast themeContrast ) => $"thead-{ToThemeContrast( themeContrast )}";

        public virtual string TableHeaderCell() => null;

        public virtual string TableFooter() => null;

        public virtual string TableBody() => null;

        public virtual string TableRow() => null;

        public virtual string TableRowColor( Color color ) => $"table-{ToColor( color )}";

        public virtual string TableRowBackground( Background background ) => BackgroundColor( background );

        public virtual string TableRowTextColor( TextColor textColor ) => $"text-{ToTextColor( textColor )}";

        public virtual string TableRowIsSelected() => "selected";

        public virtual string TableRowHeader() => null;

        public virtual string TableRowCell() => null;

        public virtual string TableRowCellColor( Color color ) => $"table-{ToColor( color )}";

        public virtual string TableRowCellBackground( Background background ) => BackgroundColor( background );

        public virtual string TableRowCellTextColor( TextColor textColor ) => $"text-{ToTextColor( textColor )}";

        #endregion

        #region Badge

        public virtual string Badge() => "badge";

        public virtual string BadgeColor( Color color ) => $"{Badge()}-{ToColor( color )}";

        public virtual string BadgePill() => $"{Badge()}-pill";

        #endregion

        #region Media

        public virtual string Media() => "media";

        public virtual string MediaLeft() => "media-left";

        public virtual string MediaRight() => "media-right";

        public virtual string MediaBody() => "media-body";

        #endregion

        #region Text

        public virtual string TextColor( TextColor textColor ) => $"text-{ToTextColor( textColor )}";

        public virtual string TextAlignment( TextAlignment textAlignment ) => $"text-{ToTextAlignment( textAlignment )}";

        public virtual string TextTransform( TextTransform textTransform ) => $"text-{ToTextTransform( textTransform )}";

        public virtual string TextWeight( TextWeight textWeight ) => $"font-weight-{ToTextWeight( textWeight )}";

        public virtual string TextItalic() => "font-italic";

        #endregion

        #region Heading

        public virtual string HeadingSize( HeadingSize headingSize ) => $"h{ToHeadingSize( headingSize )}";

        public virtual string HeadingTextColor( TextColor textColor ) => $"text-{ToTextColor( textColor )}";

        #endregion

        #region DisplayHeading

        public virtual string DisplayHeadingSize( DisplayHeadingSize displayHeadingSize ) => $"display-{ToDisplayHeadingSize( displayHeadingSize )}";

        #endregion

        #region Paragraph

        public virtual string Paragraph() => null;

        public virtual string ParagraphColor( TextColor textColor ) => $"text-{ToTextColor( textColor )}";

        #endregion

        #region Figure

        public virtual string Figure() => "figure";

        public virtual string FigureSize( FigureSize figureSize ) => $"figure-is-{ToFigureSize( figureSize )}";

        public virtual string FigureImage() => "figure-img";

        public virtual string FigureImageRounded() => "rounded";

        public virtual string FigureCaption() => "figure-caption";

        #endregion

        #region Breadcrumb

        public virtual string Breadcrumb() => "breadcrumb";

        public virtual string BreadcrumbItem() => "breadcrumb-item";

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

        public virtual string ToBackground( Background color )
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
                    return "1";
                case Blazorise.SpacingSize.Is2:
                    return "2";
                case Blazorise.SpacingSize.Is3:
                    return "3";
                case Blazorise.SpacingSize.Is4:
                    return "4";
                case Blazorise.SpacingSize.Is5:
                    return "5";
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

        public virtual string ToFigureSize( FigureSize figureSize )
        {
            switch ( figureSize )
            {
                case Blazorise.FigureSize.Is16x16:
                    return "16x16";
                case Blazorise.FigureSize.Is24x24:
                    return "24x24";
                case Blazorise.FigureSize.Is32x32:
                    return "32x32";
                case Blazorise.FigureSize.Is48x48:
                    return "48x48";
                case Blazorise.FigureSize.Is64x64:
                    return "64x64";
                case Blazorise.FigureSize.Is96x96:
                    return "96x96";
                case Blazorise.FigureSize.Is128x128:
                    return "128x128";
                case Blazorise.FigureSize.Is256x256:
                    return "256x256";
                case Blazorise.FigureSize.Is512x512:
                    return "512x512";
                default:
                    return null;
            }
        }

        #endregion

        public virtual bool UseCustomInputStyles { get; set; } = true;

        public virtual string Provider => "Bootstrap";
    }
}
