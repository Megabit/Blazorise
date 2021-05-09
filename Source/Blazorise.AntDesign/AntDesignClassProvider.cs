﻿#region Using directives
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

        public override string TextEditSize( Size size ) => $"ant-input-{ToSize( size )}";

        public override string TextEditColor( Color color ) => $"ant-form-text-{ToColor( color )}";

        public override string TextEditValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

        #endregion

        #region MemoEdit

        public override string MemoEdit() => "ant-input";

        public override string MemoEditSize( Size size ) => $"ant-input-{ToSize( size )}";

        public override string MemoEditValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

        #endregion

        #region Select

        public override string Select() => "ant-select-selection-search-input";

        public override string SelectMultiple() => null;

        public override string SelectSize( Size size ) => $"ant-select-{ToSize( size )}";

        public override string SelectValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

        #endregion

        #region NumericEdit

        public override string NumericEdit( bool plaintext ) => plaintext ? "ant-form-text" : "ant-input";

        public override string NumericEditSize( Size size ) => $"ant-input-{ToSize( size )}";

        public override string NumericEditColor( Color color ) => $"ant-form-text-{ToColor( color )}";

        public override string NumericEditValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

        #endregion

        #region DateEdit

        public override string DateEdit( bool plaintext ) => plaintext ? "ant-form-text" : "ant-input";

        public override string DateEditSize( Size size ) => $"ant-input-{ToSize( size )}";

        public override string DateEditColor( Color color ) => $"ant-form-text-{ToColor( color )}";

        public override string DateEditValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

        #endregion

        #region TimeEdit

        public override string TimeEdit( bool plaintext ) => plaintext ? "ant-form-text" : "ant-input";

        public override string TimeEditSize( Size size ) => $"ant-input-{ToSize( size )}";

        public override string TimeEditColor( Color color ) => $"ant-form-text-{ToColor( color )}";

        public override string TimeEditValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

        #endregion

        #region ColorEdit

        public override string ColorEdit() => "ant-input";

        #endregion

        #region Check

        public override string Check() => "ant-checkbox-input";

        public override string CheckSize( Size size ) => $"{Check()}-{ToSize( size )}";

        public override string CheckInline() => null;

        public override string CheckCursor( Cursor cursor ) => null;

        public override string CheckValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

        #endregion

        #region RadioGroup

        public override string RadioGroup( bool buttons, Orientation orientation )
            => "ant-radio-group ant-radio-group-outline" + ( orientation == Orientation.Horizontal ? "" : " ant-radio-group-vertical" );

        public override string RadioGroupValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

        #endregion

        #region Radio

        public override string Radio( bool button ) => button ? "ant-radio-button-input" : "ant-radio-input";

        public override string RadioSize( bool button, Size size ) => $"{Radio( button )}-{ToSize( size )}";

        public override string RadioInline( bool inline ) => null;

        public override string RadioCursor( Cursor cursor ) => null;

        public override string RadioValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

        #endregion

        #region Switch

        public override string Switch() => "ant-switch";

        public override string SwitchColor( Color color ) => $"{Switch()}-{ToColor( color )}";

        public override string SwitchSize( Size size ) => $"{Switch()}-{ToSize( size )}";

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

        public override string SliderValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

        #endregion

        #region Rating

        public override string Rating() => "ant-rate";

        public override string RatingDisabled( bool disabled ) => disabled ? "ant-rate-disabled" : null;

        public override string RatingReadonly( bool @readonly ) => @readonly ? "ant-rate-readonly" : null;

        public override string RatingItem() => "ant-rate-star";

        public override string RatingItemColor( Color color ) => $"ant-rate-star-{ToColor( color )}";

        public override string RatingItemSelected( bool selected ) => selected ? "ant-rate-star-full" : "ant-rate-star-zero";

        public override string RatingItemHovered( bool hover ) => hover ? "ant-rate-star-focused" : null;

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

        public override string ValidationSummary() => "ant-typography-danger";

        public override string ValidationSummaryError() => "ant-typography-danger";

        #endregion

        #region Fields

        public override string Fields() => "ant-row ant-form-row";

        public override string FieldsBody() => null;

        public override string FieldsColumn() => "ant-col";

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

        public override string ButtonSize( Size size ) => $"{Button()}-{ToSize( size )}";

        public override string ButtonBlock() => $"{Button()}-block";

        public override string ButtonActive() => "ant-btn-active";

        public override string ButtonDisabled() => "ant-btn-disabled";

        public override string ButtonLoading() => "ant-btn-loading";

        #endregion

        #region Buttons

        public override string Buttons( ButtonsRole role, Orientation orientation )
        {
            if ( role == ButtonsRole.Toolbar )
                return "btn-toolbar";

            if ( orientation == Orientation.Vertical )
                return "ant-btn-group btn-group-vertical";

            return "ant-btn-group";
        }

        public override string ButtonsSize( Size size ) => $"ant-btn-group-{ToSize( size )}";

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

        public override string DropdownItemActive( bool active ) => active ? Active() : null;

        public override string DropdownItemDisabled( bool disabled ) => disabled ? "ant-dropdown-menu-item-disabled" : null;

        public override string DropdownDivider() => "ant-dropdown-menu-divider";

        public override string DropdownHeader() => "ant-dropdown-menu-header";

        public override string DropdownMenu() => "ant-dropdown";

        //public override string DropdownMenuBody() => null;

        public override string DropdownMenuVisible( bool visible ) => visible ? null : "ant-dropdown-hidden";

        public override string DropdownMenuRight() => "dropdown-menu-right";

        public override string DropdownToggle() => "ant-btn ant-dropdown-trigger";

        public override string DropdownToggleColor( Color color ) => $"{Button()}-{ToColor( color )}";

        public override string DropdownToggleOutline( Color color ) => color != Color.None ? $"{Button()}-outline-{ToColor( color )}" : $"{Button()}-outline";

        public override string DropdownToggleSize( Size size ) => $"{Button()}-{ToSize( size )}";

        public override string DropdownToggleSplit() => "dropdown-toggle-split";

        public override string DropdownToggleIcon( bool visible ) => null;

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

        public override string DropdownTableResponsive() => null;

        #endregion

        #region Tabs

        public override string Tabs( bool pills ) => pills ? "ant-tabs ant-tabs-top ant-tabs-line ant-tabs-pills" : "ant-tabs ant-tabs-top ant-tabs-line";

        public override string TabsCards() => "ant-tabs-card";

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

        #region Steps

        public override string Steps() => "ant-steps ant-steps-horizontal ant-steps-label-horizontal";

        public override string StepItem() => "ant-steps-item";

        public override string StepItemActive( bool active ) => active ? "ant-steps-item-process ant-steps-item-active" : "ant-steps-item-wait";

        public override string StepItemCompleted( bool completed ) => completed ? "ant-steps-item-finish" : null;

        public override string StepItemColor( Color color ) => $"ant-steps-item-{ToColor( color )}";

        public override string StepItemMarker() => "ant-steps-item-icon";

        public override string StepItemDescription() => "ant-steps-item-content";

        public override string StepsContent() => "ant-steps-content";

        public override string StepPanel() => "ant-steps-panel";

        public override string StepPanelActive( bool active ) => active ? "ant-steps-panel-active" : null;

        #endregion

        #region Carousel

        public override string Carousel() => "ant-carousel";

        public override string CarouselSlides() => "slick-list";

        public override string CarouselSlide() => "slick-slide";

        public override string CarouselSlideActive( bool active ) => active ? "slick-active slick-current" : null;

        public override string CarouselIndicators() => "slick-dots slick-dots-bottom";

        public override string CarouselIndicator() => null;

        public override string CarouselIndicatorActive( bool active ) => active ? "slick-active" : null;

        public override string CarouselFade( bool fade ) => null;

        public override string CarouselCaption() => null;

        #endregion

        #region Jumbotron

        public override string Jumbotron() => "ant-hero";

        public override string JumbotronBackground( Background background ) => $"ant-hero-{ToBackground( background )}";

        public override string JumbotronTitle( JumbotronTitleSize jumbotronTitleSize ) => $"ant-display-{ToJumbotronTitleSize( jumbotronTitleSize )}";

        public override string JumbotronSubtitle() => "ant-hero-subtitle";

        #endregion

        #region Card

        public override string CardDeck() => "ant-card-deck";

        public override string CardGroup() => "ant-card-group";

        public override string Card() => "ant-card ant-card-bordered";

        public override string CardWhiteText() => "ant-text-white";

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

        public override string Bar() => "ant-menu ant-menu-root";

        public override string BarAlignment( Alignment alignment ) => $"ant-menu-{ToAlignment( alignment )}";

        public override string BarThemeContrast( ThemeContrast themeContrast ) => $"ant-menu-{ToThemeContrast( themeContrast )}";

        public override string BarBreakpoint( Breakpoint breakpoint ) => $"ant-menu-expand-{ToBreakpoint( breakpoint )}";

        public override string BarMode( BarMode mode ) => $"ant-menu-{ToBarMode( mode )} {( mode == Blazorise.BarMode.VerticalSmall ? "ant-menu-inline-collapsed" : null )}";

        public override string BarItem( BarMode mode ) => mode == Blazorise.BarMode.Horizontal ? "ant-menu-item ant-menu-item-only-child" : "ant-menu-item";

        public override string BarItemActive( BarMode mode ) => "ant-menu-item-selected";

        public override string BarItemDisabled( BarMode mode ) => "ant-menu-item-disabled";

        public override string BarItemHasDropdown( BarMode mode ) => null;

        public override string BarItemHasDropdownShow( BarMode mode ) => null;

        public override string BarLink( BarMode mode ) => mode == Blazorise.BarMode.Horizontal ? "ant-menu-link" : null;

        public override string BarLinkDisabled( BarMode mode ) => Disabled();

        //public override string BarCollapse() => "navbar-collapse";

        public override string BarBrand( BarMode mode ) => mode == Blazorise.BarMode.Horizontal ? "ant-menu-item" : "ant-menu-item ant-bar-brand";

        public override string BarToggler( BarMode mode, BarTogglerMode togglerMode ) => mode == Blazorise.BarMode.Horizontal ? null :
            togglerMode == BarTogglerMode.Popout ? "ant-menu-toggler-popout" : "ant-menu-toggler-inline";

        public override string BarTogglerCollapsed( BarMode mode, BarTogglerMode togglerMode, bool isShow ) => null;

        public override string BarMenu( BarMode mode ) => null;

        public override string BarMenuShow( BarMode mode ) => null;

        public override string BarStart( BarMode mode ) => "ant-menu-start";

        public override string BarEnd( BarMode mode ) => "ant-menu-end";

        public override string BarDropdown( BarMode mode ) => $"ant-menu-submenu ant-menu-submenu-{ToBarMode( mode )}";

        public override string BarDropdownShow( BarMode mode ) => mode == Blazorise.BarMode.Horizontal ? "ant-menu-submenu-open" : "ant-menu-submenu-open";

        public override string BarDropdownToggle( BarMode mode ) => mode == Blazorise.BarMode.Horizontal ? "ant-menu-submenu-title" : "ant-menu-submenu-title";

        public override string BarDropdownItem( BarMode mode ) => "ant-menu-item ant-menu-item-only-child";

        public override string BarTogglerIcon( BarMode mode ) => mode == Blazorise.BarMode.Horizontal ? "navbar-toggler-icon" : "navbar-toggler-icon";

        public override string BarDropdownMenu( BarMode mode ) => mode == Blazorise.BarMode.Horizontal ? $"ant-menu ant-menu-sub ant-menu-vertical" : $"ant-menu ant-menu-sub ant-menu-{ToBarMode( mode )}";

        public override string BarDropdownMenuVisible( BarMode mode, bool visible ) => visible ? null : "ant-menu-hidden";

        public override string BarDropdownMenuRight( BarMode mode ) => null;

        public override string BarDropdownMenuContainer( BarMode mode ) => mode == Blazorise.BarMode.Horizontal ? null : "b-bar-dropdown-menu-container";

        public override string BarCollapsed( BarMode mode ) => $"ant-menu-{ToBarMode( mode )}-collapsed";

        public override string BarLabel() => "ant-menu-label";

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

        public override string RowNoGutters() => "ant-row-no-gutters";

        #endregion

        #region Column

        public override string Column( bool hasSizes ) => "ant-col";

        public override string Column( ColumnWidth columnWidth, Breakpoint breakpoint, bool offset )
        {
            // AntDesign requires for base ant-col class to be always defined.
            var sb = new StringBuilder( $"ant-col" );

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

        public override string Display( DisplayType displayType, DisplayDefinition displayDefinition )
        {
            var baseClass = displayDefinition.Breakpoint != Breakpoint.None
                ? $"ant-display-{ToBreakpoint( displayDefinition.Breakpoint )}-{ToDisplayType( displayType )}"
                : $"ant-display-{ToDisplayType( displayType )}";

            if ( displayDefinition.Direction != DisplayDirection.None )
                return $"{baseClass} ant-flex-{ToDisplayDirection( displayDefinition.Direction )}";

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

        public override string ProgressBarSize( Size size ) => null;

        public override string ProgressBarStriped() => "progress-bar-striped";

        public override string ProgressBarAnimated() => "progress-bar-animated";

        public override string ProgressBarWidth( int width ) => null;

        #endregion

        #region Chart

        public override string Chart() => null;

        #endregion

        #region Colors

        public override string BackgroundColor( Background background ) => $"bg-{ToBackground( background )}";

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

        public override string TableRowHoverCursor() => "ant-table-row-selectable";

        public override string TableRowIsSelected() => "selected";

        public override string TableRowHeader() => "ant-table-cell ant-table-row-header";

        public override string TableRowCell() => "ant-table-cell";

        public override string TableRowCellColor( Color color ) => $"ant-table-{ToColor( color )}";

        public override string TableResponsive() => "ant-table-responsive";

        public override string TableFixedHeader() => "ant-table-fixed-header";

        #endregion

        #region Badge

        public override string Badge() => "ant-tag";

        public override string BadgeColor( Color color ) => $"{Badge()}-{ToColor( color )}";

        public override string BadgePill() => $"{Badge()}-pill";

        public override string BadgeClose() => "anticon anticon-close";

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

        #region Image

        public override string Image() => "ant-image-img";

        public override string ImageFluid( bool fluid ) => fluid ? "ant-image-img-fluid" : null;

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

        #region Borders

        public override string Border( BorderSize borderSize, BorderSide borderSide, BorderColor borderColor )
        {
            var sb = new StringBuilder( "ant-border" );

            if ( borderSide != BorderSide.All )
                sb.Append( '-' ).Append( ToBorderSide( borderSide ) );

            if ( borderSize == BorderSize.Is0 )
                sb.Append( "-0" );

            if ( borderColor != BorderColor.None )
            {
                sb.Append( " ant-border-" ).Append( ToBorderColor( borderColor ) );
            }

            return sb.ToString();
        }

        public override string Border( BorderSize borderSize, IEnumerable<(BorderSide borderSide, BorderColor borderColor)> rules )
            => string.Join( " ", rules.Select( x => Border( borderSize, x.borderSide, x.borderColor ) ) );

        public override string BorderRadius( BorderRadius borderRadius )
            => $"ant-{ToBorderRadius( borderRadius )}";

        #endregion

        #region Flex

        public override string Flex( FlexType flexType )
        {
            return flexType != FlexType.None
                ? $"ant-display-{ToFlexType( flexType )}"
                : null;
        }

        public override string Flex( FlexDefinition flexDefinition )
        {
            var sb = new StringBuilder();

            var breakpoint = flexDefinition.Breakpoint != Breakpoint.None && flexDefinition.Breakpoint != Breakpoint.Mobile
                ? $"{ToBreakpoint( flexDefinition.Breakpoint )}-"
                : null;

            if ( flexDefinition.Direction != FlexDirection.None )
                sb.Append( "ant-flex-direction-" ).Append( breakpoint ).Append( ToDirection( flexDefinition.Direction ) );

            if ( flexDefinition.JustifyContent != FlexJustifyContent.None )
                sb.Append( "ant-justify-content-" ).Append( breakpoint ).Append( ToJustifyContent( flexDefinition.JustifyContent ) );

            if ( flexDefinition.AlignItems != FlexAlignItems.None )
                sb.Append( "ant-align-items-" ).Append( breakpoint ).Append( ToAlignItems( flexDefinition.AlignItems ) );

            if ( flexDefinition.AlignSelf != FlexAlignSelf.None )
                sb.Append( "ant-align-self-" ).Append( breakpoint ).Append( ToAlignSelf( flexDefinition.AlignSelf ) );

            if ( flexDefinition.AlignContent != FlexAlignContent.None )
                sb.Append( "ant-align-content-" ).Append( breakpoint ).Append( ToAlignContent( flexDefinition.AlignContent ) );

            if ( flexDefinition.GrowShrink != FlexGrowShrink.None && flexDefinition.GrowShrinkSize != FlexGrowShrinkSize.None )
                sb.Append( "ant-flex-" ).Append( breakpoint ).Append( ToGrowShrink( flexDefinition.GrowShrink ) ).Append( "-" ).Append( ToGrowShrinkSize( flexDefinition.GrowShrinkSize ) );

            if ( flexDefinition.Wrap != FlexWrap.None )
                sb.Append( "ant-flex-wrap-" ).Append( breakpoint ).Append( ToWrap( flexDefinition.Wrap ) );

            if ( flexDefinition.Order != FlexOrder.None )
                sb.Append( "ant-flex-order-" ).Append( breakpoint ).Append( ToOrder( flexDefinition.Order ) );

            if ( flexDefinition.Fill )
                sb.Append( "ant-flex-" ).Append( breakpoint ).Append( "fill" );

            return sb.ToString();
        }

        public override string Flex( FlexType flexType, IEnumerable<FlexDefinition> flexDefinitions )
        {
            var sb = new StringBuilder();

            if ( flexType != FlexType.None )
                sb.Append( $"ant-display-{ToFlexType( flexType )}" ).Append( ' ' );

            sb.Append( string.Join( ' ', flexDefinitions.Select( x => Flex( x ) ) ) );

            return sb.ToString();
        }

        public override string FlexAlignment( Alignment alignment ) => $"justify-content-{ToAlignment( alignment )}";

        #endregion

        #region Sizing

        public override string Sizing( SizingType sizingType, SizingSize sizingSize, SizingDefinition sizingDefinition )
        {
            var sb = new StringBuilder( "ant-" );

            if ( sizingDefinition.IsMin && sizingDefinition.IsViewport )
                sb.Append( "min-wiewport-" );
            else if ( sizingDefinition.IsMax )
                sb.Append( "max-" );
            else if ( sizingDefinition.IsViewport )
                sb.Append( "viewport-" );

            sb.Append( sizingType == SizingType.Width
                ? "width"
                : "height" );

            sb.Append( $"-{ToSizingSize( sizingSize )}" );

            return sb.ToString();
        }

        #endregion

        #region Visibility

        public override string Visibility( Visibility visibility )
        {
            return visibility switch
            {
                Blazorise.Visibility.Visible => "ant-visible",
                Blazorise.Visibility.Invisible => "ant-invisible",
                _ => null,
            };
        }

        #endregion

        #region VerticalAlignment

        public override string VerticalAlignment( VerticalAlignment verticalAlignment )
            => $"ant-vertical-align-{ToVerticalAlignment( verticalAlignment )}";

        #endregion

        #region Shadow

        public override string Shadow( Shadow shadow )
        {
            if ( shadow == Blazorise.Shadow.Default )
                return "ant-shadow";

            return $"ant-shadow-{ToShadow( shadow )}";
        }

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

        public override string ToBarMode( BarMode mode )
        {
            switch ( mode )
            {
                case Blazorise.BarMode.Horizontal:
                    return "horizontal";
                case Blazorise.BarMode.VerticalInline:
                    return "inline";
                case Blazorise.BarMode.VerticalSmall:
                case Blazorise.BarMode.VerticalPopout:
                    return "vertical";
                default:
                    return null;
            }
        }

        public override string ToJustifyContent( FlexJustifyContent justifyContent )
        {
            return justifyContent switch
            {
                Blazorise.FlexJustifyContent.Start => "flex-start",
                Blazorise.FlexJustifyContent.End => "flex-end",
                Blazorise.FlexJustifyContent.Center => "center",
                Blazorise.FlexJustifyContent.Between => "space-between",
                Blazorise.FlexJustifyContent.Around => "space-around",
                _ => null,
            };
        }

        public override string ToAlignItems( FlexAlignItems alignItems )
        {
            return alignItems switch
            {
                Blazorise.FlexAlignItems.Start => "flex-start",
                Blazorise.FlexAlignItems.End => "flex-end",
                Blazorise.FlexAlignItems.Center => "center",
                Blazorise.FlexAlignItems.Baseline => "baseline",
                Blazorise.FlexAlignItems.Stretch => "stretch",
                _ => null,
            };
        }

        public override string ToAlignSelf( FlexAlignSelf alignSelf )
        {
            return alignSelf switch
            {
                Blazorise.FlexAlignSelf.Auto => "auto",
                Blazorise.FlexAlignSelf.Start => "flex-start",
                Blazorise.FlexAlignSelf.End => "flex-end",
                Blazorise.FlexAlignSelf.Center => "center",
                Blazorise.FlexAlignSelf.Baseline => "baseline",
                Blazorise.FlexAlignSelf.Stretch => "stretch",
                _ => null,
            };
        }

        public override string ToAlignContent( FlexAlignContent alignContent )
        {
            return alignContent switch
            {
                Blazorise.FlexAlignContent.Start => "flex-start",
                Blazorise.FlexAlignContent.End => "flex-end",
                Blazorise.FlexAlignContent.Center => "center",
                Blazorise.FlexAlignContent.Between => "space-between",
                Blazorise.FlexAlignContent.Around => "space-around",
                Blazorise.FlexAlignContent.Stretch => "stretch",
                _ => null,
            };
        }

        #endregion

        public override bool UseCustomInputStyles { get; set; } = true;

        public override string Provider => "AntDesign";
    }
}
