#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace Blazorise.Bootstrap
{
    public class BootstrapClassProvider : ClassProvider
    {
        #region TextEdit

        public override string TextEdit( bool plaintext ) => plaintext ? "form-control-plaintext" : "form-control";

        public override string TextEditSize( Size size ) => $"form-control-{ToSize( size )}";

        public override string TextEditColor( Color color ) => $"text-{ToColor( color )}";

        public override string TextEditValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

        #endregion

        #region MemoEdit

        public override string MemoEdit() => "form-control";

        public override string MemoEditValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

        #endregion

        #region Select

        public override string Select() => UseCustomInputStyles ? "custom-select" : "form-control";

        public override string SelectMultiple() => null;

        public override string SelectSize( Size size ) => $"{Select()}-{ToSize( size )}";

        public override string SelectValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

        #endregion

        #region NumericEdit

        public override string NumericEdit( bool plaintext ) => plaintext ? "form-control-plaintext" : "form-control";

        public override string NumericEditSize( Size size ) => $"form-control-{ToSize( size )}";

        public override string NumericEditColor( Color color ) => $"text-{ToColor( color )}";

        public override string NumericEditValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

        #endregion

        #region DateEdit

        public override string DateEdit( bool plaintext ) => plaintext ? "form-control-plaintext" : "form-control";

        public override string DateEditSize( Size size ) => $"form-control-{ToSize( size )}";

        public override string DateEditColor( Color color ) => $"text-{ToColor( color )}";

        public override string DateEditValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

        #endregion

        #region TimeEdit

        public override string TimeEdit( bool plaintext ) => plaintext ? "form-control-plaintext" : "form-control";

        public override string TimeEditSize( Size size ) => $"form-control-{ToSize( size )}";

        public override string TimeEditColor( Color color ) => $"text-{ToColor( color )}";

        public override string TimeEditValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

        #endregion

        #region ColorEdit

        public override string ColorEdit() => "form-control";

        #endregion

        #region Check

        public override string Check() => UseCustomInputStyles ? "custom-control-input" : "form-check-input";

        public override string CheckSize( Size size ) => $"{Check()}-{ToSize( size )}";

        public override string CheckInline() => UseCustomInputStyles ? "custom-control-inline" : "form-check-inline";

        public override string CheckCursor( Cursor cursor ) => $"{Check()}-{ToCursor( cursor )}";

        public override string CheckValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

        #endregion

        #region RadioGroup

        public override string RadioGroup( bool buttons, Orientation orientation ) => buttons
            ? orientation == Orientation.Horizontal ? "btn-group btn-group-toggle" : "btn-group-vertical btn-group-toggle"
            : null;

        public override string RadioGroupValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

        #endregion

        #region Radio

        public override string Radio( bool button ) => button ? null : UseCustomInputStyles ? "custom-control-input" : "form-check-input";

        public override string RadioSize( bool button, Size size ) => $"{Radio( button )}-{ToSize( size )}";

        public override string RadioInline( bool inline ) => inline
            ? UseCustomInputStyles ? "custom-control-inline" : "form-check-inline"
            : null;

        public override string RadioCursor( Cursor cursor ) => $"{( UseCustomInputStyles ? "custom-control-input" : "form-check-input" )}-{ToCursor( cursor )}";

        public override string RadioValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

        #endregion

        #region Switch

        public override string Switch() => UseCustomInputStyles ? "custom-control-input" : "form-check-input";

        public override string SwitchColor( Color color ) => $"{Switch()}-{ToColor( color )}";

        public override string SwitchSize( Size size ) => $"custom-control-input-{ToSize( size )}";

        public override string SwitchChecked( bool @checked ) => null;

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

        public override string LabelType( LabelType labelType )
        {
            switch ( labelType )
            {
                case Blazorise.LabelType.Check:
                case Blazorise.LabelType.Radio:
                case Blazorise.LabelType.Switch:
                    return UseCustomInputStyles ? "custom-control-label" : "form-check-label";
                case Blazorise.LabelType.File:
                    return UseCustomInputStyles ? "custom-file-label" : null;
                case Blazorise.LabelType.None:
                default:
                    return null;
            }
        }

        public override string LabelCursor( Cursor cursor ) => UseCustomInputStyles ? $"custom-control-label-{ToCursor( cursor )}" : $"form-check-label-{ToCursor( cursor )}";

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

        public override string ValidationSummary() => "text-danger";

        public override string ValidationSummaryError() => "text-danger";

        #endregion

        #region Fields

        public override string Fields() => "form-row";

        public override string FieldsBody() => null;

        public override string FieldsColumn() => "col";

        #endregion

        #region Field

        public override string Field() => "form-group";

        public override string FieldHorizontal() => "row";

        public override string FieldColumn() => "col";

        public override string FieldJustifyContent( JustifyContent justifyContent ) => ToJustifyContent( justifyContent );

        public override string FieldValidation( ValidationStatus validationStatus ) => null;

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

        public override string AddonsHasButton( bool hasButton ) => null;

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

        public override string Button() => "btn";

        public override string ButtonColor( Color color ) => $"{Button()}-{ToColor( color )}";

        public override string ButtonOutline( Color color ) => color != Blazorise.Color.None ? $"{Button()}-outline-{ToColor( color )}" : $"{Button()}-outline";

        public override string ButtonSize( Size size ) => $"{Button()}-{ToSize( size )}";

        public override string ButtonBlock() => $"{Button()}-block";

        public override string ButtonActive() => "active";

        public override string ButtonLoading() => null;

        #endregion

        #region Buttons

        //public override string Buttons() => "btn-group";

        public override string ButtonsAddons() => "btn-group";

        public override string ButtonsToolbar() => "btn-toolbar";

        public override string ButtonsSize( Size size ) => $"{ButtonsAddons()}-{ToSize( size )}";

        public override string ButtonsOrientation( Orientation orientation ) => orientation == Orientation.Vertical ? "btn-group-vertical" : null;

        #endregion

        #region CloseButton

        public override string CloseButton() => "close";

        #endregion

        #region Dropdown

        public override string Dropdown() => "dropdown";

        public override string DropdownGroup() => "btn-group";

        public override string DropdownShow() => Show();

        public override string DropdownRight() => null;

        public override string DropdownItem() => "dropdown-item";

        public override string DropdownItemActive( bool active ) => active ? Active() : null;

        public override string DropdownItemDisabled( bool disabled ) => disabled ? Disabled() : null;

        public override string DropdownDivider() => "dropdown-divider";

        public override string DropdownMenu() => "dropdown-menu";

        //public override string DropdownMenuBody() => null;

        public override string DropdownMenuVisible( bool visible ) => visible ? Show() : null;

        public override string DropdownMenuRight() => "dropdown-menu-right";

        public override string DropdownToggle() => "btn dropdown-toggle";

        public override string DropdownToggleColor( Color color ) => $"{Button()}-{ToColor( color )}";

        public override string DropdownToggleOutline( Color color ) => color != Blazorise.Color.None ? $"{Button()}-outline-{ToColor( color )}" : $"{Button()}-outline";

        public override string DropdownToggleSize( Size size ) => $"{Button()}-{ToSize( size )}";

        public override string DropdownToggleSplit() => "dropdown-toggle-split";

        public override string DropdownToggleIcon( bool visible ) => visible ? null : "dropdown-toggle-hidden";

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

        public override string DropdownTableResponsive() => "position-static";

        #endregion

        #region Tabs

        public override string Tabs( bool pills ) => pills ? "nav nav-pills" : "nav nav-tabs";

        public override string TabsCards() => "card-header-tabs";

        public override string TabsFullWidth() => "nav-fill";

        public override string TabsJustified() => "nav-justified";

        public override string TabsVertical() => "flex-column";

        public override string TabItem() => "nav-item";

        public override string TabItemActive( bool active ) => null;

        public override string TabItemDisabled( bool disabled ) => null;

        public override string TabLink() => "nav-link";

        public override string TabLinkActive( bool active ) => active ? $"{Active()} {Show()}" : null;

        public override string TabLinkDisabled( bool disabled ) => disabled ? "disabled" : null;

        public override string TabsContent() => "tab-content";

        public override string TabPanel() => "tab-pane";

        public override string TabPanelActive( bool active ) => active ? $"{Active()} {Show()}" : null;

        #endregion

        #region Steps

        public override string Steps() => "steps";

        public override string StepItem() => "step";

        public override string StepItemActive( bool active ) => active ? "step-active" : null;

        public override string StepItemCompleted( bool completed ) => completed ? "step-completed" : null;

        public override string StepItemColor( Color color ) => $"{StepItem()}-{ToColor( color )}";

        public override string StepItemMarker() => "step-circle";

        public override string StepItemDescription() => "step-text";

        public override string StepsContent() => "steps-content";

        public override string StepPanel() => "step-panel";

        public override string StepPanelActive( bool active ) => active ? "active" : null;

        #endregion

        #region Carousel

        public override string Carousel() => "carousel slide";

        public override string CarouselSlides() => "carousel-inner";

        public override string CarouselSlide() => "carousel-item";

        public override string CarouselSlideActive( bool active ) => active ? Active() : null;

        public override string CarouselIndicators() => "carousel-indicators";

        public override string CarouselIndicator() => null;

        public override string CarouselIndicatorActive( bool active ) => active ? Active() : null;

        public override string CarouselFade( bool fade ) => fade ? "carousel-fade" : null;

        public override string CarouselCaption() => "carousel-caption";

        #endregion

        #region Jumbotron

        public override string Jumbotron() => "jumbotron";

        public override string JumbotronBackground( Background background ) => $"jumbotron-{ToBackground( background )}";

        public override string JumbotronTitle( JumbotronTitleSize jumbotronTitleSize ) => $"display-{ToJumbotronTitleSize( jumbotronTitleSize )}";

        public override string JumbotronSubtitle() => "lead";

        #endregion

        #region Card

        public override string CardDeck() => "card-deck";

        public override string CardGroup() => "card-group";

        public override string Card() => "card";

        public override string CardWhiteText() => "text-white";

        public override string CardBackground( Background background ) => BackgroundColor( background );

        public override string CardActions() => "card-actions";

        public override string CardBody() => "card-body";

        public override string CardFooter() => "card-footer";

        public override string CardHeader() => "card-header";

        public override string CardImage() => "card-img-top";

        public override string CardTitle( bool insideHeader ) => "card-title";

        public override string CardTitleSize( bool insideHeader, int? size ) => null;

        public override string CardSubtitle( bool insideHeader ) => "card-subtitle";

        public override string CardSubtitleSize( bool insideHeader, int size ) => null;

        public override string CardText() => "card-text";

        public override string CardLink() => "card-link";

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

        #region Bar

        public override string Bar() => "navbar";

        public override string BarBackground( Background background ) => BackgroundColor( background );

        public override string BarAlignment( Alignment alignment ) => FlexAlignment( alignment );

        public override string BarThemeContrast( ThemeContrast themeContrast ) => $"navbar-{ToThemeContrast( themeContrast )} b-bar-{ToThemeContrast( themeContrast )}";

        public override string BarBreakpoint( Breakpoint breakpoint ) => breakpoint != Breakpoint.None && breakpoint != Breakpoint.Mobile ? $"navbar-expand-{ToBreakpoint( breakpoint )}" : null;

        public override string BarMode( BarMode mode ) => $"b-bar-{ToBarMode( mode )}";

        public override string BarItem( BarMode mode ) => mode == Blazorise.BarMode.Horizontal ? "nav-item" : "b-bar-item";

        public override string BarItemActive( BarMode mode ) => Active();

        public override string BarItemDisabled( BarMode mode ) => Disabled();

        public override string BarItemHasDropdown( BarMode mode ) => null;

        public override string BarItemHasDropdownShow( BarMode mode ) => null;

        public override string BarLink( BarMode mode ) => mode == Blazorise.BarMode.Horizontal ? "nav-link" : "b-bar-link";

        public override string BarLinkDisabled( BarMode mode ) => Disabled();

        public override string BarBrand( BarMode mode ) => mode == Blazorise.BarMode.Horizontal ? "navbar-brand" : "b-bar-brand";

        public override string BarToggler( BarMode mode, BarTogglerMode togglerMode ) => mode == Blazorise.BarMode.Horizontal ? "navbar-toggler" :
            togglerMode == BarTogglerMode.Popout ? "b-bar-toggler-popout" : "b-bar-toggler-inline";

        public override string BarTogglerCollapsed( BarMode mode, BarTogglerMode togglerMode, bool isShow ) => isShow || mode != Blazorise.BarMode.Horizontal ? null : "collapsed";

        public override string BarMenu( BarMode mode ) => mode == Blazorise.BarMode.Horizontal ? "collapse navbar-collapse" : "b-bar-menu";

        public override string BarMenuShow( BarMode mode ) => Show();

        public override string BarStart( BarMode mode ) => mode == Blazorise.BarMode.Horizontal ? "navbar-nav mr-auto" : "b-bar-start";

        public override string BarEnd( BarMode mode ) => mode == Blazorise.BarMode.Horizontal ? "navbar-nav ml-auto" : "b-bar-end";

        public override string BarDropdown( BarMode mode ) => mode == Blazorise.BarMode.Horizontal ? "dropdown" : "b-bar-dropdown";

        public override string BarDropdownShow( BarMode mode ) => Show();

        public override string BarDropdownToggle( BarMode mode ) => mode == Blazorise.BarMode.Horizontal ? "nav-link dropdown-toggle" : "b-bar-link b-bar-dropdown-toggle";

        public override string BarDropdownItem( BarMode mode ) => mode == Blazorise.BarMode.Horizontal ? "dropdown-item" : "b-bar-dropdown-item";

        public override string BarTogglerIcon( BarMode mode ) => "navbar-toggler-icon";

        public override string BarDropdownMenu( BarMode mode ) => mode == Blazorise.BarMode.Horizontal ? "dropdown-menu" : "b-bar-dropdown-menu";

        public override string BarDropdownMenuVisible( BarMode mode, bool visible ) => visible ? Show() : null;

        public override string BarDropdownMenuRight( BarMode mode ) => mode == Blazorise.BarMode.Horizontal ? "dropdown-menu-right" : "b-bar-right";

        public override string BarDropdownMenuContainer( BarMode mode ) => mode == Blazorise.BarMode.Horizontal ? null : "b-bar-dropdown-menu-container";

        public override string BarCollapsed( BarMode mode ) => null;

        public override string BarLabel() => "b-bar-label";

        #endregion

        #region Accordion

        public override string Accordion() => "accordion";

        #endregion

        #region Collapse

        public override string Collapse() => "card";

        public override string CollapseActive( bool active ) => null;

        public override string CollapseHeader() => "card-header";

        public override string CollapseBody() => "collapse";

        public override string CollapseBodyActive( bool active ) => active ? Show() : null;

        public override string CollapseBodyContent() => "card-body";

        #endregion

        #region Row

        public override string Row() => "row";

        #endregion

        #region Column

        public override string Column( bool hasSizes ) => hasSizes ? null : "col";

        public override string Column( ColumnWidth columnWidth, Breakpoint breakpoint, bool offset )
        {
            var baseClass = offset ? "offset" : "col";

            if ( breakpoint != Blazorise.Breakpoint.None && breakpoint != Blazorise.Breakpoint.Mobile )
            {
                return $"{baseClass}-{ToBreakpoint( breakpoint )}-{ToColumnWidth( columnWidth )}";
            }

            return $"{baseClass}-{ToColumnWidth( columnWidth )}";
        }

        public override string Column( ColumnWidth columnWidth, IEnumerable<(Breakpoint breakpoint, bool offset)> rules )
            => string.Join( " ", rules.Select( r => Column( columnWidth, r.breakpoint, r.offset ) ) );

        #endregion

        #region Display

        public override string Display( DisplayType displayType, Breakpoint breakpoint, DisplayDirection direction )
        {
            var baseClass = breakpoint != Breakpoint.None && breakpoint != Blazorise.Breakpoint.Mobile
                ? $"d-{ToBreakpoint( breakpoint )}-{ToDisplayType( displayType )}"
                : $"d-{ToDisplayType( displayType )}";

            if ( direction != DisplayDirection.None )
                return $"{baseClass} flex-{ToDisplayDirection( direction )}";

            return baseClass;
        }

        #endregion

        #region Alert

        public override string Alert() => "alert";

        public override string AlertColor( Color color ) => $"{Alert()}-{ToColor( color )}";

        public override string AlertDismisable() => "alert-dismissible";

        public override string AlertFade() => Fade();

        public override string AlertShow() => Show();

        public override string AlertHasMessage() => null;

        public override string AlertHasDescription() => null;

        public override string AlertMessage() => null;

        public override string AlertDescription() => null;

        #endregion

        #region Modal

        public override string Modal() => "modal";

        public override string ModalFade() => Fade();

        public override string ModalVisible( bool visible ) => visible ? Show() : null;

        public override string ModalBackdrop() => "modal-backdrop";

        public override string ModalBackdropFade() => Fade();

        public override string ModalBackdropVisible( bool visible ) => visible ? Show() : null;

        public override string ModalContent( bool dialog ) => "modal-content";

        public override string ModalContentSize( ModalSize modalSize ) => $"modal-{ToModalSize( modalSize )}";

        public override string ModalContentCentered() => "modal-dialog-centered";

        public override string ModalBody() => "modal-body";

        public override string ModalHeader() => "modal-header";

        public override string ModalFooter() => "modal-footer";

        public override string ModalTitle() => "modal-title";

        #endregion

        #region Pagination

        public override string Pagination() => "pagination";

        public override string PaginationSize( Size size ) => $"{Pagination()}-{ToSize( size )}";

        public override string PaginationItem() => "page-item";

        public override string PaginationItemActive() => Active();

        public override string PaginationItemDisabled() => Disabled();

        public override string PaginationLink() => "page-link";

        public override string PaginationLinkActive() => null;

        public override string PaginationLinkDisabled() => null;

        #endregion

        #region Progress

        public override string Progress() => "progress";

        public override string ProgressSize( Size size ) => $"progress-{ToSize( size )}";

        public override string ProgressBar() => "progress-bar";

        public override string ProgressBarSize( Size size ) => null;

        public override string ProgressBarColor( Background background ) => BackgroundColor( background );

        public override string ProgressBarStriped() => "progress-bar-striped";

        public override string ProgressBarAnimated() => "progress-bar-animated";

        public override string ProgressBarWidth( int width ) => $"w-{width}";

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

        public override string Table() => "table";

        public override string TableFullWidth() => null;

        public override string TableStriped() => "table-striped";

        public override string TableHoverable() => "table-hover";

        public override string TableBordered() => "table-bordered";

        public override string TableNarrow() => "table-sm";

        public override string TableBorderless() => "table-borderless";

        public override string TableHeader() => null;

        public override string TableHeaderThemeContrast( ThemeContrast themeContrast ) => $"thead-{ToThemeContrast( themeContrast )}";

        public override string TableHeaderCell() => null;

        public override string TableHeaderCellTextAlignment( TextAlignment textAlignment ) => $"text-{ToTextAlignment( textAlignment )}";

        public override string TableFooter() => null;

        public override string TableBody() => null;

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

        public override string TableRowCellTextAlignment( TextAlignment textAlignment ) => $"text-{ToTextAlignment( textAlignment )}";

        public override string TableResponsive() => "table-responsive";

        #endregion

        #region Badge

        public override string Badge() => "badge";

        public override string BadgeColor( Color color ) => $"{Badge()}-{ToColor( color )}";

        public override string BadgePill() => $"{Badge()}-pill";

        public override string BadgeClose() => "badge-close";

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

        public override string FigureImage() => "figure-img img-fluid";

        public override string FigureImageRounded() => "rounded";

        public override string FigureCaption() => "figure-caption";

        #endregion

        #region Breadcrumb

        public override string Breadcrumb() => "breadcrumb";

        public override string BreadcrumbItem() => "breadcrumb-item";

        public override string BreadcrumbItemActive() => Active();

        public override string BreadcrumbLink() => null;

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
            if ( breakpoint != Blazorise.Breakpoint.None && breakpoint != Breakpoint.Mobile )
                return $"{ToSpacing( spacing )}{ToSide( side )}-{ToBreakpoint( breakpoint )}-{ToSpacingSize( spacingSize )}";

            return $"{ToSpacing( spacing )}{ToSide( side )}-{ToSpacingSize( spacingSize )}";
        }

        public override string Spacing( Spacing spacing, SpacingSize spacingSize, IEnumerable<(Side side, Breakpoint breakpoint)> rules ) => string.Join( " ", rules.Select( x => Spacing( spacing, spacingSize, x.side, x.breakpoint ) ) );

        #endregion

        #region Flex

        public override string FlexAlignment( Alignment alignment ) => $"justify-content-{ToAlignment( alignment )}";

        #endregion       

        public override bool UseCustomInputStyles { get; set; } = true;

        public override string Provider => "Bootstrap";
    }
}
