#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace Blazorise.AntDesign
{
    public class AntDesignClassProvider : ClassProvider
    {
        #region TextEdit

        public override string TextEdit( bool plaintext ) => plaintext ? "form-control-plaintext" : "ant-input";

        public override string TextEditSize( Size size ) => $"{TextEdit( false )}-{ToSize( size )}";

        public override string TextEditColor( Color color ) => $"text-{ToColor( color )}";

        public override string TextEditValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

        #endregion

        #region MemoEdit

        public override string MemoEdit() => "ant-input";

        public override string MemoEditValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

        #endregion

        #region Select

        public override string Select() => "ant-select";

        public override string SelectSize( Size size ) => $"{Select()}-{ToSize( size )}";

        public override string SelectValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

        #endregion

        #region DateEdit

        public override string DateEdit() => "ant-input";

        public override string DateEditSize( Size size ) => $"{DateEdit()}-{ToSize( size )}";

        public override string DateEditValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

        #endregion

        #region CheckEdit

        public override string CheckEdit() => UseCustomInputStyles ? "custom-control-input" : "form-check-input";

        public override string CheckEditInline() => UseCustomInputStyles ? "custom-control-inline" : "form-check-inline";

        public override string CheckEditCursor( Cursor cursor ) => $"{CheckEdit()}-{ToCursor( cursor )}";

        public override string CheckEditValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

        #endregion

        #region RadioEdit

        public override string RadioEdit() => UseCustomInputStyles ? "custom-control-input" : "form-check-input";

        public override string RadioInline() => UseCustomInputStyles ? "custom-control-inline" : "form-check-inline";

        #endregion

        #region Switch

        public override string Switch() => UseCustomInputStyles ? "custom-control-input" : "form-check-input";

        public override string SwitchCursor( Cursor cursor ) => $"{Switch()}-{ToCursor( cursor )}";

        public override string SwitchValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

        #endregion

        #region FileEdit

        public override string FileEdit() => UseCustomInputStyles ? "custom-file-input" : "form-control-file";

        public override string FileEditValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

        #endregion

        #region Slider

        public override string Slider() => "form-control-range";

        public override string SliderColor( Color color ) => $"form-control-range-{ToColor( color )}";

        #endregion

        #region Label

        public override string Label() => null;

        public override string LabelCursor( Cursor cursor ) => UseCustomInputStyles ? $"custom-control-label-{ToCursor( cursor )}" : $"form-check-label-{ToCursor( cursor )}";

        public override string LabelCheck() => UseCustomInputStyles ? "custom-control-label" : "form-check-label";

        public override string LabelSwitch() => UseCustomInputStyles ? "custom-control-label" : "form-check-label";

        public override string LabelFile() => UseCustomInputStyles ? "custom-file-label" : null;

        #endregion

        #region Help

        public override string Help() => "form-text text-muted";

        #endregion

        #region Validation

        public override string ValidationSuccess() => "valid-feedback";

        public override string ValidationSuccessTooltip() => "valid-tooltip";

        public override string ValidationError() => "invalid-feedback";

        public override string ValidationErrorTooltip() => "invalid-tooltip";

        public override string ValidationNone() => "form-text text-muted";

        #endregion

        #region Fields

        public override string Fields() => "form-row";

        public override string FieldsBody() => null;

        public override string FieldsColumn() => $"{Col()}";

        #endregion

        #region Field

        public override string Field() => "form-group";

        public override string FieldHorizontal() => "row";

        public override string FieldColumn() => $"{Col()}";

        public override string FieldJustifyContent( JustifyContent justifyContent ) => ToJustifyContent( justifyContent );

        #endregion

        #region FieldLabel

        public override string FieldLabel() => null;

        public override string FieldLabelHorizontal() => "col-form-label";

        #endregion

        #region FieldBody

        public override string FieldBody() => null;

        #endregion

        #region FieldHelp

        public override string FieldHelp() => "form-text text-muted";

        #endregion

        #region Control

        public override string ControlCheck() => UseCustomInputStyles ? "custom-control custom-checkbox" : "form-check";

        public override string ControlRadio() => UseCustomInputStyles ? "custom-control custom-radio" : "form-check";

        public override string ControlSwitch() => UseCustomInputStyles ? "custom-control custom-switch" : "form-check";

        public override string ControlFile() => UseCustomInputStyles ? "custom-control custom-file" : "form-group";

        public override string ControlText() => null;

        #endregion

        #region Addons

        public override string Addons() => "input-group";

        public override string Addon( AddonType addonType )
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

        public override string AddonLabel() => "input-group-text";

        //public override string AddonContainer() => null;

        #endregion

        #region Inline

        public override string Inline() => "form-inline";

        #endregion

        #region Button

        public override string Button() => "ant-btn";

        public override string ButtonColor( Color color ) => $"{Button()}-{ToColor( color )}";

        public override string ButtonOutline( Color color ) => color != Blazorise.Color.None ? $"{Button()}-dashed-{ToColor( color )}" : $"{Button()}-dashed";

        public override string ButtonSize( ButtonSize buttonSize )
        {
            switch ( buttonSize )
            {
                case Blazorise.ButtonSize.Small:
                    return "ant-btn-sm";
                case Blazorise.ButtonSize.Large:
                    return "ant-btn-lg";
                default:
                    return null;
            }
        }

        public override string ButtonBlock() => $"{Button()}-block";

        public override string ButtonActive() => "active";

        public override string ButtonLoading() => null;

        #endregion

        #region Buttons

        //public override string Buttons() => "btn-group";

        public override string ButtonsAddons() => "btn-group";

        public override string ButtonsToolbar() => "btn-toolbar";

        public override string ButtonsSize( ButtonsSize buttonsSize )
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

        public override string ButtonsVertical() => "btn-group-vertical";

        #endregion

        #region CloseButton

        public override string CloseButton() => "close";

        #endregion

        #region Dropdown

        public override string Dropdown() => null;

        public override string DropdownGroup() => "btn-group";

        public override string DropdownShow() => null;

        public override string DropdownRight() => null;

        public override string DropdownItem() => "ant-dropdown-menu-item";

        public override string DropdownItemActive() => Active();

        public override string DropdownDivider() => "ant-dropdown-menu-divider";

        public override string DropdownMenu() => "ant-dropdown-menu";

        //public override string DropdownMenuBody() => null;

        public override string DropdownMenuShow() => "ant-dropdown-open";

        public override string DropdownMenuRight() => "dropdown-menu-right";

        public override string DropdownToggle() => "ant-btn ant-dropdown-trigger";

        public override string DropdownToggleColor( Color color ) => $"{Button()}-{ToColor( color )}";

        public override string DropdownToggleOutline( Color color ) => color != Blazorise.Color.None ? $"{Button()}-outline-{ToColor( color )}" : $"{Button()}-outline";

        public override string DropdownToggleSize( ButtonSize buttonSize )
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

        public override string DropdownToggleSplit() => "dropdown-toggle-split";

        public override string DropdownDirection( Direction direction )
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

        public override string Tabs() => "nav nav-tabs";

        public override string TabsCards() => "card-header-tabs";

        public override string TabsPills() => "nav-pills";

        public override string TabsFullWidth() => "nav-fill";

        public override string TabsJustified() => "nav-justified";

        public override string TabsVertical() => "flex-column";

        public override string TabItem() => "nav-item";

        public override string TabItemActive() => null;

        public override string TabLink() => "nav-link";

        public override string TabLinkActive() => $"{Active()} {Show()}";

        public override string TabsContent() => "tab-content";

        public override string TabPanel() => "tab-pane";

        public override string TabPanelActive() => $"{Active()} {Show()}";

        #endregion

        #region Card

        public override string CardGroup() => "card-group";

        public override string Card() => "ant-card ant-card-bordered";

        public override string CardWhiteText() => "text-white";

        public override string CardBackground( Background background ) => BackgroundColor( background );

        public override string CardActions() => "card-actions";

        public override string CardBody() => "ant-card-body";

        public override string CardFooter() => "ant-card-foot";

        public override string CardHeader() => "ant-card-head";

        public override string CardImage() => "ant-card-cover";

        public override string CardTitle() => "ant-card-head-title";

        public override string CardSubtitle() => "ant-card-head-subtitle";

        public override string CardSubtitleSize( int size ) => null;

        public override string CardText() => "ant-card-text";

        public override string CardLink() => "ant-card-extra";

        #endregion

        #region ListGroup

        public override string ListGroup() => "list-group";

        public override string ListGroupFlush() => "list-group-flush";

        public override string ListGroupItem() => "list-group-item";

        public override string ListGroupItemActive() => Active();

        public override string ListGroupItemDisabled() => Disabled();

        #endregion

        #region Container

        public override string Container() => "container";

        public override string ContainerFluid() => "container-fluid";

        #endregion

        #region Panel

        public override string Panel() => null;

        #endregion

        #region Nav

        public override string Nav() => "nav";

        public override string NavItem() => "nav-item";

        public override string NavLink() => "nav-link";

        public override string NavTabs() => "nav-tabs";

        public override string NavCards() => "nav-cards";

        public override string NavPills() => "nav-pills";

        public override string NavFill( NavFillType fillType ) => fillType == NavFillType.Justified ? "nav-justified" : "nav-fill";

        public override string NavVertical() => "flex-column";

        #endregion

        #region Navbar

        public override string Bar() => "ant-menu";

        public override string BarThemeContrast( ThemeContrast themeContrast ) => $"navbar-{ToThemeContrast( themeContrast )}";

        public override string BarBreakpoint( Breakpoint breakpoint ) => $"navbar-expand-{ToBreakpoint( breakpoint )}";

        public override string BarItem() => "ant-menu-item";

        public override string BarItemActive() => Active();

        public override string BarItemDisabled() => Disabled();

        public override string BarItemHasDropdown() => "dropdown";

        public override string BarItemHasDropdownShow() => Show();

        public override string BarLink() => "ant-menu-link";

        public override string BarLinkDisabled() => Disabled();

        //public override string BarCollapse() => "navbar-collapse";

        public override string BarBrand() => "logo";

        public override string BarToggler() => "ant-menu-toggler";

        public override string BarTogglerCollapsed( bool isShow ) => isShow ? null : "collapsed";

        public override string BarMenu() => "collapse navbar-collapse";

        public override string BarMenuShow() => Show();

        public override string BarStart() => "navbar-nav mr-auto";

        public override string BarEnd() => "navbar-nav";

        //public override string BarHasDropdown() => "dropdown";

        public override string BarDropdown() => null;

        public override string BarDropdownShow() => null;

        public override string BarDropdownToggle() => "nav-link dropdown-toggle";

        public override string BarDropdownItem() => "dropdown-item";

        public override string BarTogglerIcon() => "navbar-toggler-icon";

        public override string BarDropdownMenu() => "dropdown-menu";

        public override string BarDropdownMenuShow() => Show();

        public override string BarDropdownMenuRight() => "dropdown-menu-right";

        #endregion

        #region Accordion

        public override string Accordion() => "accordion";

        #endregion

        #region Collapse

        public override string Collapse() => "collapse";

        public override string CollapseShow() => Show();

        #endregion

        #region Row

        public override string Row() => "ant-row";

        #endregion

        #region Col

        public override string Col() => "ant-col";

        public override string Col( ColumnWidth columnWidth, Breakpoint breakpoint, bool offset )
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

        public override string Col( ColumnWidth columnWidth, IEnumerable<(Breakpoint breakpoint, bool offset)> rules ) =>
            string.Join( " ", rules.Select( r => Col( columnWidth, r.breakpoint, r.offset ) ) );

        #endregion

        #region Alert

        public override string Alert() => "ant-alert ant-alert-no-icon";

        public override string AlertColor( Color color ) => $"ant-alert-{ToColor( color )}";

        public override string AlertDismisable() => "ant-alert-closable";

        public override string AlertFade() => Fade();

        public override string AlertShow() => Show();

        public override string AlertHasMessage() => null;

        public override string AlertHasDescription() => "ant-alert-with-description";

        public override string AlertMessage() => "ant-alert-message";

        public override string AlertDescription() => "ant-alert-description";

        #endregion

        #region Modal

        public override string Modal() => "ant-modal-root";

        public override string ModalFade() => null;

        public override string ModalVisible( bool visible ) => null;

        public override string ModalBackdrop() => "ant-modal-mask";

        public override string ModalBackdropFade() => null;

        public override string ModalBackdropVisible( bool visible ) => visible ? null : "ant-modal-mask-hidden";

        public override string ModalContent( bool dialog ) => "ant-modal-content";

        public override string ModalContentSize( ModalSize modalSize ) => null;

        public override string ModalContentCentered() => "ant-modal-dialog-centered";

        public override string ModalBody() => "ant-modal-body";

        public override string ModalHeader() => "ant-modal-header";

        public override string ModalFooter() => "ant-modal-footer";

        public override string ModalTitle() => "ant-modal-title";

        #endregion

        #region Pagination

        public override string Pagination() => "ant-pagination";

        public override string PaginationSize( Size size ) => $"{Pagination()}-{ToSize( size )}";

        public override string PaginationItem() => "ant-pagination-item";

        public override string PaginationItemActive() => "ant-pagination-item-active";

        public override string PaginationItemDisabled() => Disabled();

        public override string PaginationLink() => "ant-pagination-link";

        public override string PaginationLinkActive() => null;

        public override string PaginationLinkDisabled() => null;

        #endregion

        #region Progress

        public override string Progress() => "ant-progress ant-progress-line";

        public override string ProgressSize( Size size ) => $"progress-{ToSize( size )}";

        public override string ProgressBar() => "ant-progress-bg";

        public override string ProgressBarColor( Background background ) => BackgroundColor( background );

        public override string ProgressBarStriped() => "progress-bar-striped";

        public override string ProgressBarAnimated() => "progress-bar-animated";

        public override string ProgressBarWidth( int width ) => null;

        #endregion

        #region Chart

        public override string Chart() => null;

        #endregion

        #region Colors

        public override string BackgroundColor( Background color ) => $"bg-{ToBackground( color )}";

        #endregion

        #region Title

        public override string Title() => null;

        public override string TitleSize( int size ) => $"h{size}";

        #endregion

        #region Table

        public override string Table() => "ant-table";

        public override string TableFullWidth() => null;

        public override string TableStriped() => "table-striped";

        public override string TableHoverable() => "table-hover";

        public override string TableBordered() => "table-bordered";

        public override string TableNarrow() => "table-sm";

        public override string TableBorderless() => "table-borderless";

        public override string TableHeader() => "ant-table-thead";

        public override string TableHeaderThemeContrast( ThemeContrast themeContrast ) => $"thead-{ToThemeContrast( themeContrast )}";

        public override string TableHeaderCell() => null;

        public override string TableFooter() => null;

        public override string TableBody() => "ant-table-tbody";

        public override string TableRow() => null;

        public override string TableRowColor( Color color ) => $"table-{ToColor( color )}";

        public override string TableRowBackground( Background background ) => BackgroundColor( background );

        public override string TableRowTextColor( TextColor textColor ) => $"text-{ToTextColor( textColor )}";

        public override string TableRowHoverCursor() => "table-row-selectable";

        public override string TableRowIsSelected() => "selected";

        public override string TableRowHeader() => null;

        public override string TableRowCell() => null;

        public override string TableRowCellColor( Color color ) => $"table-{ToColor( color )}";

        public override string TableRowCellBackground( Background background ) => BackgroundColor( background );

        public override string TableRowCellTextColor( TextColor textColor ) => $"text-{ToTextColor( textColor )}";

        #endregion

        #region Badge

        public override string Badge() => "ant-tag";

        public override string BadgeColor( Color color ) => $"{Badge()}-{ToColor( color )}";

        public override string BadgePill() => $"{Badge()}-pill";

        #endregion

        #region Media

        public override string Media() => "media";

        public override string MediaLeft() => "media-left";

        public override string MediaRight() => "media-right";

        public override string MediaBody() => "media-body";

        #endregion

        #region Text

        public override string TextColor( TextColor textColor ) => $"text-{ToTextColor( textColor )}";

        public override string TextAlignment( TextAlignment textAlignment ) => $"text-{ToTextAlignment( textAlignment )}";

        public override string TextTransform( TextTransform textTransform ) => $"text-{ToTextTransform( textTransform )}";

        public override string TextWeight( TextWeight textWeight ) => $"font-weight-{ToTextWeight( textWeight )}";

        public override string TextItalic() => "font-italic";

        #endregion

        #region Heading

        public override string HeadingSize( HeadingSize headingSize ) => $"h{ToHeadingSize( headingSize )}";

        public override string HeadingTextColor( TextColor textColor ) => $"text-{ToTextColor( textColor )}";

        #endregion

        #region DisplayHeading

        public override string DisplayHeadingSize( DisplayHeadingSize displayHeadingSize ) => $"display-{ToDisplayHeadingSize( displayHeadingSize )}";

        #endregion

        #region Paragraph

        public override string Paragraph() => null;

        public override string ParagraphColor( TextColor textColor ) => $"text-{ToTextColor( textColor )}";

        #endregion

        #region Figure

        public override string Figure() => "figure";

        public override string FigureSize( FigureSize figureSize ) => $"figure-is-{ToFigureSize( figureSize )}";

        public override string FigureImage() => "figure-img";

        public override string FigureImageRounded() => "rounded";

        public override string FigureCaption() => "figure-caption";

        #endregion

        #region Breadcrumb

        public override string Breadcrumb() => "ant-breadcrumb";

        public override string BreadcrumbItem() => null;

        public override string BreadcrumbItemActive() => null;

        public override string BreadcrumbLink() => "ant-breadcrumb-link";

        #endregion

        #region Tooltip

        public override string Tooltip() => "b-tooltip";

        public override string TooltipPlacement( Placement placement ) => $"b-tooltip-{ToPlacement( placement )}";

        public override string TooltipMultiline() => "b-tooltip-multiline";

        public override string TooltipAlwaysActive() => "b-tooltip-active";

        public override string TooltipFade() => "b-tooltip-fade";

        public override string TooltipInline() => "b-tooltip-inline";

        #endregion

        #region Divider

        public override string Divider() => "divider";

        public override string DividerType( DividerType dividerType ) => $"{Divider()}-{ToDividerType( dividerType )}";

        #endregion

        #region States

        public override string Show() => "show";

        public override string Fade() => "fade";

        public override string Active() => "active";

        public override string Disabled() => "disabled";

        public override string Collapsed() => "collapsed";

        #endregion

        #region Layout

        public override string Spacing( Spacing spacing, SpacingSize spacingSize, Side side, Breakpoint breakpoint )
        {
            if ( breakpoint != Blazorise.Breakpoint.None )
                return $"{ToSpacing( spacing )}{ToSide( side )}-{ToBreakpoint( breakpoint )}-{ToSpacingSize( spacingSize )}";

            return $"{ToSpacing( spacing )}{ToSide( side )}-{ToSpacingSize( spacingSize )}";
        }

        public override string Spacing( Spacing spacing, SpacingSize spacingSize, IEnumerable<(Side side, Breakpoint breakpoint)> rules ) => string.Join( " ", rules.Select( x => Spacing( spacing, spacingSize, x.side, x.breakpoint ) ) );

        #endregion

        #region Flex

        public override string FlexAlignment( Alignment alignment ) => $"justify-content-{ToAlignment( alignment )}";

        #endregion

        #region Enums

        public override string ToColumnWidth( ColumnWidth columnWidth )
        {
            switch ( columnWidth )
            {
                case Blazorise.ColumnWidth.Is1:
                    return "2";
                case Blazorise.ColumnWidth.Is2:
                    return "4";
                case Blazorise.ColumnWidth.Is3:
                case Blazorise.ColumnWidth.Quarter:
                    return "6";
                case Blazorise.ColumnWidth.Is4:
                case Blazorise.ColumnWidth.Third:
                    return "8";
                case Blazorise.ColumnWidth.Is5:
                    return "10";
                case Blazorise.ColumnWidth.Is6:
                case Blazorise.ColumnWidth.Half:
                    return "12";
                case Blazorise.ColumnWidth.Is7:
                    return "14";
                case Blazorise.ColumnWidth.Is8:
                    return "16";
                case Blazorise.ColumnWidth.Is9:
                    return "18";
                case Blazorise.ColumnWidth.Is10:
                    return "20";
                case Blazorise.ColumnWidth.Is11:
                    return "22";
                case Blazorise.ColumnWidth.Is12:
                case Blazorise.ColumnWidth.Full:
                    return "24";
                case Blazorise.ColumnWidth.Auto:
                    return "auto";
                default:
                    return null;
            }
        }

        #endregion

        public override bool UseCustomInputStyles { get; set; } = true;

        public override string Provider => "AntDesign";
    }
}
