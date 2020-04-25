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

        public override string TextEdit( bool plaintext ) => plaintext ? "ant-form-text" : "ant-input";

        public override string TextEditSize( Size size ) => $"{TextEdit( false )}-{ToSize( size )}";

        public override string TextEditColor( Color color ) => $"ant-form-text-{ToColor( color )}";

        public override string TextEditValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

        #endregion

        #region MemoEdit

        public override string MemoEdit() => "ant-input";

        public override string MemoEditValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

        #endregion

        #region Select

        public override string Select() => "ant-select-selection-search-input";

        public override string SelectMultiple() => null;

        public override string SelectSize( Size size ) => $"{Select()}-{ToSize( size )}";

        public override string SelectValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

        #endregion

        #region DateEdit

        public override string DateEdit() => "ant-input";

        public override string DateEditSize( Size size ) => $"{DateEdit()}-{ToSize( size )}";

        public override string DateEditValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

        #endregion

        #region TimeEdit

        public override string TimeEdit() => "ant-input";

        public override string TimeEditSize( Size size ) => $"{TimeEdit()}-{ToSize( size )}";

        public override string TimeEditValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

        #endregion

        #region ColorEdit

        public override string ColorEdit() => "ant-input";

        #endregion

        #region Check

        public override string Check() => "ant-checkbox-input";

        public override string CheckInline() => null;

        public override string CheckCursor( Cursor cursor ) => null;

        public override string CheckValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

        #endregion

        #region RadioGroup

        public override string RadioGroup( bool buttons ) => "ant-radio-group ant-radio-group-outline";

        public override string RadioGroupInline() => null;

        #endregion

        #region Radio

        public override string Radio( bool button ) => button ? "ant-radio-button-input" : "ant-radio-input";

        public override string RadioInline() => null;

        #endregion

        #region Switch

        public override string Switch() => "ant-switch";

        public override string SwitchChecked( bool @checked ) => @checked ? "ant-switch-checked" : null;

        public override string SwitchCursor( Cursor cursor ) => $"{Switch()}-{ToCursor( cursor )}";

        public override string SwitchValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

        #endregion

        #region FileEdit

        public override string FileEdit() => null;

        public override string FileEditValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

        #endregion

        #region Slider

        public override string Slider() => "ant-slider";

        public override string SliderColor( Color color ) => $"ant-slider-{ToColor( color )}";

        #endregion

        #region Label

        public override string Label() => null;

        public override string LabelType( LabelType labelType ) => null;

        public override string LabelCursor( Cursor cursor ) => null;

        #endregion

        #region Help

        public override string Help() => "ant-form-item-explain";

        #endregion

        #region Validation

        public override string ValidationSuccess() => "valid-feedback";

        public override string ValidationSuccessTooltip() => "valid-tooltip";

        public override string ValidationError() => "ant-form-item-explain";

        public override string ValidationErrorTooltip() => "invalid-tooltip";

        public override string ValidationNone() => "ant-form-item-explain";

        #endregion

        #region Fields

        public override string Fields() => "ant-row ant-form-row";

        public override string FieldsBody() => null;

        public override string FieldsColumn() => $"{Column()}";

        #endregion

        #region Field

        public override string Field() => "ant-form-item";

        public override string FieldHorizontal() => "ant-row";

        public override string FieldColumn() => "ant-col";

        public override string FieldJustifyContent( JustifyContent justifyContent ) => ToJustifyContent( justifyContent );

        public override string FieldValidation( ValidationStatus validationStatus )
        {
            switch ( validationStatus )
            {
                case ValidationStatus.Error:
                    return "ant-form-item-has-feedback ant-form-item-has-error";
                case ValidationStatus.Success:
                    return "ant-form-item-has-feedback ant-form-item-has-success";
                default:
                    return null;
            }
        }

        #endregion

        #region FieldLabel

        public override string FieldLabel() => null;

        public override string FieldLabelHorizontal() => "ant-form-item-label";

        #endregion

        #region FieldBody

        public override string FieldBody() => null;

        #endregion

        #region FieldHelp

        public override string FieldHelp() => "ant-form-item-explain";

        #endregion

        #region Control

        public override string ControlCheck() => UseCustomInputStyles ? "custom-control custom-checkbox" : "form-check";

        public override string ControlRadio() => UseCustomInputStyles ? "custom-control custom-radio" : "form-check";

        public override string ControlSwitch() => UseCustomInputStyles ? "custom-control custom-switch" : "form-check";

        public override string ControlFile() => UseCustomInputStyles ? "custom-control custom-file" : "form-group";

        public override string ControlText() => null;

        #endregion

        #region Addons

        public override string Addons() => "ant-input-group-wrapper";

        public override string AddonsHasButton( bool hasButton ) => hasButton ? "ant-input-search ant-input-search-enter-button" : null;

        public override string Addon( AddonType addonType )
        {
            switch ( addonType )
            {
                case AddonType.Start:
                    return "ant-input-group-addon";
                case AddonType.End:
                    return "ant-input-group-addon";
                default:
                    return null;
            }
        }

        public override string AddonLabel() => null;

        //public override string AddonContainer() => null;

        #endregion

        #region Inline

        public override string Inline() => "ant-form-inline";

        #endregion

        #region Button

        public override string Button() => "ant-btn";

        public override string ButtonColor( Color color ) => $"{Button()}-{ToColor( color )}";

        public override string ButtonOutline( Color color ) => color != Blazorise.Color.None ? $"{Button()}-outline-{ToColor( color )}" : $"{Button()}-outline";

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

        public override string ButtonActive() => "ant-btn-active";

        public override string ButtonLoading() => "ant-btn-loading";

        #endregion

        #region Buttons

        //public override string Buttons() => "btn-group";

        public override string ButtonsAddons() => "ant-btn-group";

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

        public override string CloseButton() => "ant-close";

        #endregion

        #region Dropdown

        public override string Dropdown() => "ant-dropdown-group ant-dropdown-button"; // ant-dropdown-group is custom class

        public override string DropdownGroup() => null;

        public override string DropdownShow() => null;

        public override string DropdownRight() => null;

        public override string DropdownItem() => "ant-dropdown-menu-item";

        public override string DropdownItemActive() => Active();

        public override string DropdownDivider() => "ant-dropdown-menu-divider";

        public override string DropdownMenu() => "ant-dropdown";

        //public override string DropdownMenuBody() => null;

        public override string DropdownMenuVisible( bool visible ) => visible ? null : "ant-dropdown-hidden";

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

        public override string Tabs() => "ant-tabs ant-tabs-top ant-tabs-line";

        public override string TabsCards() => "ant-tabs-card";

        public override string TabsPills() => "ant-tabs-pills";

        public override string TabsFullWidth() => "ant-tabs-fill";

        public override string TabsJustified() => "ant-tabs-justified";

        public override string TabsVertical() => null;

        public override string TabItem() => "ant-tabs-tab";

        public override string TabItemActive( bool active ) => active ? "ant-tabs-tab-active" : null;

        public override string TabItemDisabled( bool disabled ) => disabled ? "ant-tabs-tab-disabled" : null;

        public override string TabLink() => null;

        public override string TabLinkActive( bool active ) => null;

        public override string TabLinkDisabled( bool disabled ) => null;

        public override string TabsContent() => "ant-tabs-content ant-tabs-content-animated";

        public override string TabPanel() => "ant-tabs-tabpane";

        public override string TabPanelActive( bool active ) => active ? "ant-tabs-tabpane-active" : "ant-tabs-tabpane-inactive";

        #endregion

        #region Jumbotron

        public override string Jumbotron() => "ant-hero";

        public override string JumbotronBackground( Background background ) => $"ant-hero-{ToBackground( background )}";

        public override string JumbotronTitle( JumbotronTitleSize jumbotronTitleSize ) => $"ant-display-{ToJumbotronTitleSize( jumbotronTitleSize )}";

        public override string JumbotronSubtitle() => "ant-hero-subtitle";

        #endregion

        #region Card

        public override string CardGroup() => "ant-card-group";

        public override string Card() => "ant-card ant-card-bordered";

        public override string CardWhiteText() => "ant-text-white";

        public override string CardBackground( Background background ) => BackgroundColor( background );

        public override string CardActions() => "ant-card-actions";

        public override string CardBody() => "ant-card-body";

        public override string CardFooter() => "ant-card-foot";

        public override string CardHeader() => "ant-card-head";

        public override string CardImage() => "ant-card-cover";

        public override string CardTitle( bool insideHeader ) => insideHeader ? "ant-card-head-title" : "ant-card-meta-title";

        public override string CardTitleSize( bool insideHeader, int? size ) => null;

        public override string CardSubtitle( bool insideHeader ) => insideHeader ? "ant-card-head-subtitle" : "ant-card-meta-description";

        public override string CardSubtitleSize( bool insideHeader, int size ) => null;

        public override string CardText() => "ant-card-text";

        public override string CardLink() => "ant-card-extra";

        #endregion

        #region ListGroup

        public override string ListGroup() => "ant-list ant-list-split ant-list-bordered";

        public override string ListGroupFlush() => "ant-list-flush";

        public override string ListGroupItem() => "ant-list-item ant-list-item-no-flex";

        public override string ListGroupItemActive() => Active();

        public override string ListGroupItemDisabled() => Disabled();

        #endregion

        #region Container

        public override string Container() => "ant-container";

        public override string ContainerFluid() => "ant-container-fluid";

        #endregion

        #region Bar

        public override string Bar() => "ant-menu ant-menu-root ant-menu-horizontal";

        public override string BarBackground( Background background ) => BackgroundColor( background );

        public override string BarAlignment( Alignment alignment ) => $"ant-menu-{ToAlignment( alignment )}";

        public override string BarThemeContrast( ThemeContrast themeContrast ) => $"ant-menu-{ToThemeContrast( themeContrast )}";

        public override string BarBreakpoint( Breakpoint breakpoint ) => $"ant-menu-expand-{ToBreakpoint( breakpoint )}";

        public override string BarItem() => "ant-menu-item ant-menu-item-only-child";

        public override string BarItemActive() => "ant-menu-item-selected";

        public override string BarItemDisabled() => "ant-menu-item-disabled";

        public override string BarItemHasDropdown() => null;

        public override string BarItemHasDropdownShow() => null;

        public override string BarLink() => "ant-menu-link";

        public override string BarLinkDisabled() => Disabled();

        //public override string BarCollapse() => "navbar-collapse";

        public override string BarBrand() => "ant-menu-item";

        public override string BarToggler() => null;

        public override string BarTogglerCollapsed( bool isShow ) => null;

        public override string BarMenu() => null;

        public override string BarMenuShow() => null;

        public override string BarStart() => "ant-menu-start";

        public override string BarEnd() => "ant-menu-end";

        public override string BarDropdown() => "ant-menu-submenu ant-menu-submenu-horizontal";

        public override string BarDropdownShow() => "ant-menu-submenu-open";

        public override string BarDropdownToggle() => "ant-menu-submenu-title";

        public override string BarDropdownItem() => "ant-menu-item ant-menu-item-only-child";

        public override string BarTogglerIcon() => "navbar-toggler-icon";

        public override string BarDropdownMenu() => "ant-menu ant-menu-sub ant-menu-vertical";

        public override string BarDropdownMenuVisible( bool visible ) => visible ? null : "ant-menu-hidden";

        public override string BarDropdownMenuRight() => null;

        #endregion

        #region Accordion

        public override string Accordion() => "ant-collapse";

        #endregion

        #region Collapse

        public override string Collapse() => "ant-collapse-item";

        public override string CollapseActive( bool active ) => active ? "ant-collapse-item-active" : null;

        public override string CollapseHeader() => "ant-collapse-header";

        public override string CollapseBody() => "ant-collapse-content";

        public override string CollapseBodyActive( bool active ) => active ? "ant-collapse-content-active" : "ant-collapse-content-inactive";

        public override string CollapseBodyContent() => "ant-collapse-content-box";

        #endregion

        #region Row

        public override string Row() => "ant-row";

        #endregion

        #region Column

        public override string Column() => "ant-col";

        public override string Column( ColumnWidth columnWidth, Breakpoint breakpoint, bool offset )
        {
            var sb = new StringBuilder( Column() );

            if ( breakpoint != Blazorise.Breakpoint.None )
                sb.Append( $"-{ToBreakpoint( breakpoint )}" );

            if ( offset )
                sb.Append( "-offset" );

            sb.Append( $"-{ToColumnWidth( columnWidth )}" );

            return sb.ToString();
        }

        public override string Column( ColumnWidth columnWidth, IEnumerable<(Breakpoint breakpoint, bool offset)> rules ) =>
            string.Join( " ", rules.Select( r => Column( columnWidth, r.breakpoint, r.offset ) ) );

        #endregion

        #region Display

        public override string Display( DisplayType displayType, Breakpoint breakpoint, DisplayDirection direction )
        {
            var baseClass = breakpoint != Breakpoint.None
                ? $"ant-display-{ToBreakpoint( breakpoint )}-{ToDisplayType( displayType )}"
                : $"ant-display-{ToDisplayType( displayType )}";

            if ( direction != DisplayDirection.None )
                return $"{baseClass} ant-flex-{ToDisplayDirection( direction )}";

            return baseClass;
        }

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

        public override string ProgressBar() => "ant-progress-bg b-ant-progress-text";

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

        public override string TableStriped() => "ant-table-striped";

        public override string TableHoverable() => "ant-table-hover";

        public override string TableBordered() => "ant-table-bordered";

        public override string TableNarrow() => "ant-table-small";

        public override string TableBorderless() => "ant-table-borderless";

        public override string TableHeader() => "ant-table-thead";

        public override string TableHeaderThemeContrast( ThemeContrast themeContrast ) => $"ant-table-thead-{ToThemeContrast( themeContrast )}";

        public override string TableHeaderCell() => null;

        public override string TableFooter() => null;

        public override string TableBody() => "ant-table-tbody";

        public override string TableRow() => "ant-table-row";

        public override string TableRowColor( Color color ) => $"ant-table-{ToColor( color )}";

        public override string TableRowBackground( Background background ) => BackgroundColor( background );

        public override string TableRowTextColor( TextColor textColor ) => $"ant-text-{ToTextColor( textColor )}";

        public override string TableRowHoverCursor() => "ant-table-row-selectable";

        public override string TableRowIsSelected() => "selected";

        public override string TableRowHeader() => "ant-table-cell ant-table-row-header";

        public override string TableRowCell() => "ant-table-cell";

        public override string TableRowCellColor( Color color ) => $"ant-table-{ToColor( color )}";

        public override string TableRowCellBackground( Background background ) => BackgroundColor( background );

        public override string TableRowCellTextColor( TextColor textColor ) => $"ant-text-{ToTextColor( textColor )}";

        public override string TableRowCellTextAlignment( TextAlignment textAlignment ) => $"ant-text-{ToTextAlignment( textAlignment )}";

        public override string TableResponsive() => "ant-table-responsive";

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

        public override string TextColor( TextColor textColor ) => $"ant-typography-{ToTextColor( textColor )}";

        public override string TextAlignment( TextAlignment textAlignment ) => $"ant-typography-{ToTextAlignment( textAlignment )}";

        public override string TextTransform( TextTransform textTransform ) => $"ant-typography-{ToTextTransform( textTransform )}";

        public override string TextWeight( TextWeight textWeight ) => $"font-weight-{ToTextWeight( textWeight )}";

        public override string TextItalic() => "font-italic";

        #endregion

        #region Heading

        public override string HeadingSize( HeadingSize headingSize ) => "ant-typography";

        public override string HeadingTextColor( TextColor textColor ) => $"ant-typography-{ToTextColor( textColor )}";

        #endregion

        #region DisplayHeading

        public override string DisplayHeadingSize( DisplayHeadingSize displayHeadingSize ) => $"ant-display-{ToDisplayHeadingSize( displayHeadingSize )}";

        #endregion

        #region Paragraph

        public override string Paragraph() => "ant-typography";

        public override string ParagraphColor( TextColor textColor ) => $"ant-typography-{ToTextColor( textColor )}";

        #endregion

        #region Figure

        public override string Figure() => "ant-figure";

        public override string FigureSize( FigureSize figureSize ) => $"ant-figure-is-{ToFigureSize( figureSize )}";

        public override string FigureImage() => "ant-figure-img ant-figure-img-fluid";

        public override string FigureImageRounded() => "ant-figure-rounded";

        public override string FigureCaption() => "ant-figure-caption";

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
