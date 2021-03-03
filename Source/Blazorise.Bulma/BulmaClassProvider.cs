#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace Blazorise.Bulma
{
    class BulmaClassProvider : ClassProvider
    {
        #region TextEdit

        public override string TextEdit( bool plaintext ) => plaintext ? "input is-static" : "input";

        public override string TextEditSize( Size size ) => $"is-{ToSize( size )}";

        public override string TextEditColor( Color color ) => $"is-{ToColor( color )}";

        public override string TextEditValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

        #endregion

        #region MemoEdit

        public override string MemoEdit() => "textarea";

        public override string MemoEditValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

        #endregion

        #region Select

        public override string Select() => "select is-fullwidth";

        public override string SelectMultiple() => "is-multiple";

        public override string SelectSize( Size size ) => $"is-{ToSize( size )}";

        public override string SelectValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

        #endregion

        #region NumericEdit

        public override string NumericEdit( bool plaintext ) => plaintext ? "input is-static" : "input";

        public override string NumericEditSize( Size size ) => $"is-{ToSize( size )}";

        public override string NumericEditColor( Color color ) => $"is-{ToColor( color )}";

        public override string NumericEditValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

        #endregion

        #region DateEdit

        public override string DateEdit( bool plaintext ) => plaintext ? "input is-static" : "input";

        public override string DateEditSize( Size size ) => $"is-{ToSize( size )}";

        public override string DateEditColor( Color color ) => $"is-{ToColor( color )}";

        public override string DateEditValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

        #endregion

        #region TimeEdit

        public override string TimeEdit( bool plaintext ) => plaintext ? "input is-static" : "input";

        public override string TimeEditSize( Size size ) => $"is-{ToSize( size )}";

        public override string TimeEditColor( Color color ) => $"is-{ToColor( color )}";

        public override string TimeEditValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

        #endregion

        #region ColorEdit

        public override string ColorEdit() => "input";

        #endregion

        #region Check

        public override string Check() => "is-checkradio";

        public override string CheckSize( Size size ) => $"is-{ToSize( size )}";

        public override string CheckInline() => "is-inline";

        public override string CheckCursor( Cursor cursor ) => $"{Check()}-{ToCursor( cursor )}";

        public override string CheckValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

        #endregion

        #region RadioGroup

        public override string RadioGroup( bool buttons, Orientation orientation )
            => $"{( buttons ? "buttons has-addons" : "control" )}{( orientation == Orientation.Horizontal ? null : " are-vertical" )}";

        public override string RadioGroupValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

        #endregion

        #region Radio

        public override string Radio( bool button ) => "is-checkradio";

        public override string RadioSize( bool button, Size size ) => $"is-{ToSize( size )}";

        public override string RadioInline( bool inline ) => inline ? "is-inline" : null;

        public override string RadioCursor( Cursor cursor ) => $"is-checkradio-{ToCursor( cursor )}";

        public override string RadioValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

        #endregion

        #region Switch

        public override string Switch() => "switch";

        public override string SwitchColor( Color color ) => $"is-{ToColor( color )}";

        public override string SwitchSize( Size size ) => $"is-{ToSize( size )}";

        public override string SwitchChecked( bool @checked ) => null;

        public override string SwitchCursor( Cursor cursor ) => $"{Switch()}-{ToCursor( cursor )}";

        public override string SwitchValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

        #endregion

        #region FileEdit

        public override string FileEdit() => "file-input";

        public override string FileEditValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

        #endregion

        #region Slider

        public override string Slider() => "slider";

        public override string SliderColor( Color color ) => $"is-{ToColor( color )}";

        #endregion

        #region Label

        public override string Label() => "label";

        public override string LabelType( LabelType labelType )
        {
            switch ( labelType )
            {
                case Blazorise.LabelType.File:
                    return "file-label";
                case Blazorise.LabelType.Check:
                case Blazorise.LabelType.Radio:
                case Blazorise.LabelType.Switch:
                case Blazorise.LabelType.None:
                default:
                    return null;
            }
        }

        public override string LabelCursor( Cursor cursor ) => $"label-{ToCursor( cursor )}";

        #endregion

        #region Help

        public override string Help() => "help";

        #region Validation

        public override string ValidationSuccess() => "help is-success";

        public override string ValidationSuccessTooltip() => "help is-success"; // TODO

        public override string ValidationError() => "help is-danger";

        public override string ValidationErrorTooltip() => "help is-danger"; // TODO

        public override string ValidationNone() => "help";

        public override string ValidationSummary() => "has-text-danger";

        public override string ValidationSummaryError() => "has-text-danger";

        #endregion

        #endregion

        #region Fields

        public override string Fields() => "columns is-multiline";

        public override string FieldsBody() => "field-body";

        public override string FieldsColumn() => "column";

        //public override string FieldsColumnSize( ColumnSize columnSize ) => $"is-{ColumnSize( columnSize )}";

        #endregion

        #region Field

        public override string Field() => "field";

        public override string FieldHorizontal() => "is-horizontal";

        public override string FieldColumn() => "column";

        public override string FieldJustifyContent( JustifyContent justifyContent ) => ToJustifyContent( justifyContent );

        public override string FieldValidation( ValidationStatus validationStatus ) => null;

        #endregion

        #region FieldLabel

        public override string FieldLabel() => "field-label";

        public override string FieldLabelHorizontal() => "is-normal";

        #endregion

        #region FieldBody

        public override string FieldBody() => "field-body";

        #endregion

        #region FieldHelp

        public override string FieldHelp() => "help";

        #endregion

        #region Control

        public override string ControlCheck() => "control";

        public override string ControlRadio() => "control";

        public override string ControlSwitch() => "control";

        public override string ControlFile() => "file has-name is-fullwidth";

        public override string ControlText() => "control";

        #endregion

        #region Addons

        public override string Addons() => "field has-addons";

        public override string AddonsHasButton( bool hasButton ) => null;

        public override string Addon( AddonType addonType )
        {
            switch ( addonType )
            {
                case AddonType.Start:
                case AddonType.End:
                    return "control";
                default:
                    return "control is-expanded";
            }
        }

        public override string AddonLabel() => "button is-static";

        //public override string AddonContainer() => "control";

        #endregion

        #region Inline

        public override string Inline() => "field is-horizontal";

        #endregion

        #region Button

        public override string Button() => "button";

        public override string ButtonColor( Color color ) => $"is-{ToColor( color )}";

        public override string ButtonOutline( Color color ) => $"is-{ToColor( color )} is-outlined";

        public override string ButtonSize( Size size ) => $"is-{ToSize( size )}";

        public override string ButtonBlock() => $"is-fullwidth";

        public override string ButtonActive() => "is-active";

        public override string ButtonLoading() => "is-loading";

        #endregion

        #region Buttons

        //public override string Buttons() => "buttons has-addons";

        public override string ButtonsAddons() => "field has-addons";

        public override string ButtonsToolbar() => "field is-grouped";

        public override string ButtonsSize( Size size ) => $"are-{ToSize( size )}";

        public override string ButtonsOrientation( Orientation orientation ) => orientation == Orientation.Vertical ? "buttons" : null;

        #endregion

        #region CloseButton

        public override string CloseButton() => "delete";

        #endregion

        #region Dropdown

        public override string Dropdown() => "dropdown";

        public override string DropdownGroup() => "field has-addons";

        public override string DropdownShow() => Active();

        public override string DropdownRight() => "is-right";

        public override string DropdownItem() => "dropdown-item";

        public override string DropdownItemActive( bool active ) => active ? Active() : null;

        public override string DropdownItemDisabled( bool disabled ) => disabled ? Disabled() : null;

        public override string DropdownDivider() => "dropdown-divider";

        public override string DropdownMenu() => "dropdown-menu";

        //public override string DropdownMenuBody() => "dropdown-content";

        public override string DropdownMenuVisible( bool visible ) => null;

        public override string DropdownMenuRight() => null;

        public override string DropdownToggle() => "button dropdown-trigger";

        public override string DropdownToggleColor( Color color ) => $"is-{ToColor( color )}";

        public override string DropdownToggleOutline( Color color ) => $"is-{ToColor( color )} is-outlined";

        public override string DropdownToggleSize( Size size ) => $"is-{ToSize( size )}";

        public override string DropdownToggleSplit() => null;

        public override string DropdownToggleIcon( bool visible ) => null;

        public override string DropdownDirection( Direction direction )
        {
            switch ( direction )
            {
                case Direction.Up:
                    return "is-up";
                case Direction.Right:
                    return "is-right";
                case Direction.Left:
                    return "is-left";
                case Direction.Down:
                case Direction.None:
                default:
                    return null;
            }
        }

        public override string DropdownTableResponsive() => null;

        #endregion

        #region Tabs

        public override string Tabs( bool pills ) => pills ? "tabs is-toggle" : "tabs";

        public override string TabsCards() => null;

        public override string TabsFullWidth() => "is-fullwidth";

        public override string TabsJustified() => "is-justified";

        public override string TabsVertical() => "is-vertical"; // this is custom class, bulma natively does not have vertical tabs

        public override string TabItem() => null;

        public override string TabItemActive( bool active ) => active ? Active() : null;

        public override string TabItemDisabled( bool disabled ) => null;

        public override string TabLinkDisabled( bool disabled ) => disabled ? Disabled() : null;

        public override string TabLink() => null;

        public override string TabLinkActive( bool active ) => null;

        public override string TabsContent() => "tab-content";

        public override string TabPanel() => "tab-pane";

        public override string TabPanelActive( bool active ) => active ? Active() : null;

        #endregion

        #region Steps

        public override string Steps() => "steps";

        public override string StepItem() => "step-item";

        public override string StepItemActive( bool active ) => active ? Active() : null;

        public override string StepItemCompleted( bool completed ) => completed ? "is-completed" : null;

        public override string StepItemColor( Color color ) => $"is-{ToColor( color )}";

        public override string StepItemMarker() => "step-marker";

        public override string StepItemDescription() => "step-details";

        public override string StepsContent() => "steps-content";

        public override string StepPanel() => "step-content";

        public override string StepPanelActive( bool active ) => active ? "is-active" : null;

        #endregion

        #region Carousel

        public override string Carousel() => "carousel";

        public override string CarouselSlides() => "carousel-items";

        public override string CarouselSlide() => "carousel-item";

        public override string CarouselSlideActive( bool active ) => active ? null : "carousel-item-hidden";

        public override string CarouselIndicators() => "carousel-indicator is-inside is-bottom";

        public override string CarouselIndicator() => "indicator-item";

        public override string CarouselIndicatorActive( bool active ) => active ? Active() : null;

        public override string CarouselFade( bool fade ) => null;

        public override string CarouselCaption() => null;

        #endregion

        #region Jumbotron

        public override string Jumbotron() => "hero";

        public override string JumbotronBackground( Background background ) => $"is-{ToBackground( background )}";

        public override string JumbotronTitle( JumbotronTitleSize jumbotronTitleSize ) => $"title is-{ToJumbotronTitleSize( jumbotronTitleSize )}";

        public override string JumbotronSubtitle() => "subtitle";

        #endregion

        #region Card

        public override string CardDeck() => "card-deck";

        public override string CardGroup() => "card-group";

        public override string Card() => "card";

        public override string CardWhiteText() => "has-text-white";

        public override string CardBackground( Background background ) => BackgroundColor( background );

        public override string CardActions() => "card-actions";

        public override string CardBody() => "card-content";

        public override string CardFooter() => "card-footer";

        public override string CardHeader() => "card-header";

        public override string CardImage() => "card-image";

        public override string CardTitle( bool insideHeader ) => insideHeader ? "card-header-title" : "title";

        public override string CardTitleSize( bool insideHeader, int? size ) => size != null ? $"is-{size}" : null;

        public override string CardSubtitle( bool insideHeader ) => insideHeader ? "card-header-subtitle" : "subtitle";

        public override string CardSubtitleSize( bool insideHeader, int size ) => $"is-{size}";

        public override string CardText() => "card-text";

        public override string CardLink() => null;

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

        public override string BarThemeContrast( ThemeContrast themeContrast ) => $"b-bar-{ToThemeContrast( themeContrast )}";

        public override string BarBreakpoint( Breakpoint breakpoint ) => $"navbar-expand-{ToBreakpoint( breakpoint )}";

        public override string BarMode( BarMode mode ) => $"b-bar-{ToBarMode( mode )}";

        public override string BarItem( BarMode mode ) => mode == Blazorise.BarMode.Horizontal ? "navbar-item" : "b-bar-item";

        public override string BarItemActive( BarMode mode ) => Active();

        public override string BarItemDisabled( BarMode mode ) => Disabled();

        public override string BarItemHasDropdown( BarMode mode ) => mode == Blazorise.BarMode.Horizontal ? "has-dropdown" : null;

        public override string BarItemHasDropdownShow( BarMode mode ) => mode == Blazorise.BarMode.Horizontal ? Active() : Show();

        public override string BarLink( BarMode mode ) => mode == Blazorise.BarMode.Horizontal ? "navbar-item" : "b-bar-link";

        public override string BarLinkDisabled( BarMode mode ) => Disabled();

        //public override string BarCollapse() => "navbar-menu";

        public override string BarBrand( BarMode mode ) => mode == Blazorise.BarMode.Horizontal ? "navbar-brand" : "b-bar-brand";

        public override string BarToggler( BarMode mode, BarTogglerMode togglerMode ) => mode == Blazorise.BarMode.Horizontal ? "navbar-burger" :
            togglerMode == BarTogglerMode.Popout ? "b-bar-toggler-popout" : "b-bar-toggler-inline";

        public override string BarTogglerCollapsed( BarMode mode, BarTogglerMode togglerMode, bool isShow ) => isShow || mode != Blazorise.BarMode.Horizontal ? null : Active();

        public override string BarMenu( BarMode mode ) => mode == Blazorise.BarMode.Horizontal ? "navbar-menu" : "b-bar-menu";

        public override string BarMenuShow( BarMode mode ) => Active();

        public override string BarStart( BarMode mode ) => mode == Blazorise.BarMode.Horizontal ? "navbar-start" : "b-bar-start";

        public override string BarEnd( BarMode mode ) => mode == Blazorise.BarMode.Horizontal ? "navbar-end" : "b-bar-end";

        //public override string BarHasDropdown() => "has-dropdown";

        public override string BarDropdown( BarMode mode ) => mode == Blazorise.BarMode.Horizontal ? null : "b-bar-dropdown";

        public override string BarDropdownShow( BarMode mode ) => null;

        public override string BarDropdownToggle( BarMode mode ) => mode == Blazorise.BarMode.Horizontal ? "navbar-link" : "b-bar-link b-bar-dropdown-toggle";

        public override string BarDropdownItem( BarMode mode ) => mode == Blazorise.BarMode.Horizontal ? "navbar-item" : "b-bar-dropdown-item";

        public override string BarTogglerIcon( BarMode mode ) => null;

        public override string BarDropdownMenu( BarMode mode ) => mode == Blazorise.BarMode.Horizontal ? "navbar-dropdown" : "b-bar-dropdown-menu";

        public override string BarDropdownMenuVisible( BarMode mode, bool visible ) => visible ? Show() : null;

        public override string BarDropdownMenuRight( BarMode mode ) => mode == Blazorise.BarMode.Horizontal ? "is-right" : null;

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

        public override string CollapseBodyContent() => "card-content";

        #endregion

        #region Row

        public override string Row() => "columns";

        #endregion

        #region Column

        public override string Column( bool hasSizes ) => hasSizes ? null : "column";

        public override string Column( ColumnWidth columnWidth, Breakpoint breakpoint, bool offset )
        {
            var baseClass = offset ? "offset-" : null;

            if ( breakpoint != Blazorise.Breakpoint.None )
            {
                if ( columnWidth == Blazorise.ColumnWidth.None )
                    return $"column is-{baseClass}{ToBreakpoint( breakpoint )}";

                return $"column is-{ToColumnWidth( columnWidth )}-{baseClass}{ToBreakpoint( breakpoint )}";
            }

            return $"column is-{baseClass}{ToColumnWidth( columnWidth )}";
        }

        public override string Column( ColumnWidth columnWidth, IEnumerable<(Breakpoint breakpoint, bool offset)> rules ) =>
              string.Join( " ", rules.Select( r => Column( columnWidth, r.breakpoint, r.offset ) ) );

        #endregion

        #region Display

        public override string Display( DisplayType displayType, Breakpoint breakpoint, DisplayDirection direction )
        {
            var baseClass = breakpoint != Breakpoint.None
                ? $"is-{ToDisplayType( displayType )}-{ToBreakpoint( breakpoint )}"
                : $"is-{ToDisplayType( displayType )}";

            if ( direction != DisplayDirection.None )
                return $"{baseClass} is-flex-direction-{ToDisplayDirection( direction )}";

            return baseClass;
        }

        #endregion

        #region Alert

        public override string Alert() => "notification";

        public override string AlertColor( Color color ) => $"is-{ToColor( color )}";

        public override string AlertDismisable() => null;

        public override string AlertFade() => Fade();

        public override string AlertShow() => Show();

        public override string AlertHasMessage() => null;

        public override string AlertHasDescription() => null;

        public override string AlertMessage() => null;

        public override string AlertDescription() => null;

        #endregion

        #region Modal

        public override string Modal() => "modal";

        public override string ModalFade() => null;

        public override string ModalVisible( bool visible ) => visible ? Active() : null;

        public override string ModalBackdrop() => "modal-background";

        public override string ModalBackdropFade() => Fade();

        public override string ModalBackdropVisible( bool visible ) => visible ? Show() : null;

        public override string ModalContent( bool dialog ) => dialog ? "modal-card" : "modal-content";

        public override string ModalContentSize( ModalSize modalSize ) => $"modal-{ToModalSize( modalSize )}";

        public override string ModalContentCentered() => null;

        public override string ModalBody() => "modal-card-body";

        public override string ModalHeader() => "modal-card-head";

        public override string ModalFooter() => "modal-card-foot";

        public override string ModalTitle() => "modal-card-title";

        #endregion

        #region Pagination

        public override string Pagination() => "pagination-list";

        public override string PaginationSize( Size size ) => $"is-{ToSize( size )}";

        public override string PaginationItem() => null;

        public override string PaginationItemActive() => null;

        public override string PaginationItemDisabled() => null;

        public override string PaginationLink() => "pagination-link";

        public override string PaginationLinkActive() => "is-current";

        public override string PaginationLinkDisabled() => "is-disabled";

        #endregion

        #region Progress

        public override string Progress() => "progress";

        public override string ProgressSize( Size size ) => $"is-{ToSize( size )}";

        public override string ProgressBar() => "progress";

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

        public override string BackgroundColor( Background color ) => $"has-background-{ToBackground( color )}";

        #endregion

        #region Title

        public override string Title() => "title";

        public override string TitleSize( int size ) => $"is-{size}";

        #endregion

        #region Table

        public override string Table() => "table";

        public override string TableFullWidth() => "is-fullwidth";

        public override string TableStriped() => "is-striped";

        public override string TableHoverable() => "is-hoverable";

        public override string TableBordered() => "is-bordered";

        public override string TableNarrow() => "is-narrow";

        public override string TableBorderless() => "is-borderless";

        public override string TableHeader() => null;

        public override string TableHeaderThemeContrast( ThemeContrast themeContrast ) => $"has-background-{ToThemeContrast( themeContrast )}";

        public override string TableHeaderCell() => null;

        public override string TableHeaderCellTextAlignment( TextAlignment textAlignment ) => $"has-text-{ToTextAlignment( textAlignment )}";

        public override string TableFooter() => null;

        public override string TableBody() => null;

        public override string TableRow() => null;

        public override string TableRowColor( Color color ) => $"has-background-{ToColor( color )}";

        public override string TableRowBackground( Background background ) => BackgroundColor( background );

        public override string TableRowTextColor( TextColor textColor ) => $"has-text-{ToTextColor( textColor )}";

        public override string TableRowHoverCursor() => "table-row-selectable";

        public override string TableRowIsSelected() => "is-selected";

        public override string TableRowHeader() => null;

        public override string TableRowCell() => null;

        public override string TableRowCellColor( Color color ) => $"has-background-{ToColor( color )}";

        public override string TableRowCellBackground( Background background ) => BackgroundColor( background );

        public override string TableRowCellTextColor( TextColor textColor ) => $"has-text-{ToTextColor( textColor )}";

        public override string TableRowCellTextAlignment( TextAlignment textAlignment ) => $"has-text-{ToTextAlignment( textAlignment )}";

        public override string TableResponsive() => "table-container";

        #endregion

        #region Badge

        public override string Badge() => "tag";

        public override string BadgeColor( Color color ) => $"is-{ToColor( color )}";

        public override string BadgePill() => null;

        public override string BadgeClose() => "delete is-small";

        #endregion

        #region Media

        public override string Media() => "media";

        public override string MediaLeft() => "media-left";

        public override string MediaRight() => "media-right";

        public override string MediaBody() => "media-content";

        #endregion

        #region Text

        public override string TextColor( TextColor textColor ) => $"has-text-{ToTextColor( textColor )}";

        public override string TextAlignment( TextAlignment textAlignment ) => $"has-text-{ToTextAlignment( textAlignment )}";

        public override string TextTransform( TextTransform textTransform ) => $"is-{ToTextTransform( textTransform )}";

        public override string TextWeight( TextWeight textWeight ) => $"has-text-weight-{ToTextWeight( textWeight )}";

        public override string TextItalic() => "is-italic";

        #endregion

        #region Heading

        public override string HeadingSize( HeadingSize headingSize ) => $"title is-{ToHeadingSize( headingSize )}";

        #endregion

        #region DisplayHeading

        public override string DisplayHeadingSize( DisplayHeadingSize displayHeadingSize ) => $"title is-{ToDisplayHeadingSize( displayHeadingSize )}";

        #endregion

        #region Paragraph

        public override string Paragraph() => null;

        public override string ParagraphColor( TextColor textColor ) => $"has-text-{ToTextColor( textColor )}";

        #endregion

        #region Figure

        public override string Figure() => "image";

        public override string FigureSize( FigureSize figureSize ) => $"is-{ToFigureSize( figureSize )}";

        public override string FigureImage() => "figure-img";

        public override string FigureImageRounded() => "is-rounded";

        public override string FigureCaption() => "figure-caption";

        #endregion

        #region Breadcrumb

        public override string Breadcrumb() => "breadcrumb";

        public override string BreadcrumbItem() => null;

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

        public override string Show() => "is-block";

        public override string Fade() => "fade";

        public override string Active() => "is-active";

        public override string Disabled() => "is-disabled";

        public override string Collapsed() => "collapsed";

        #endregion

        #region Layout

        // TODO: Bulma by default doesn't have spacing utilities. Try to fix this!
        public override string Spacing( Spacing spacing, SpacingSize spacingSize, Side side, Breakpoint breakpoint )
        {
            if ( breakpoint != Blazorise.Breakpoint.None )
                return $"is-{ToSpacing( spacing )}{ToSide( side )}-{ToBreakpoint( breakpoint )}-{ToSpacingSize( spacingSize )}";

            return $"is-{ToSpacing( spacing )}{ToSide( side )}-{ToSpacingSize( spacingSize )}";
        }

        public override string Spacing( Spacing spacing, SpacingSize spacingSize, IEnumerable<(Side side, Breakpoint breakpoint)> rules ) => string.Join( " ", rules.Select( x => Spacing( spacing, spacingSize, x.side, x.breakpoint ) ) );

        #endregion

        #region Flex

        public override string FlexAlignment( Alignment alignment ) => $"justify-content-{ToAlignment( alignment )}";

        #endregion

        #region Enums

        public override string ToSize( Size size )
        {
            switch ( size )
            {
                case Blazorise.Size.ExtraSmall:
                    return "extra-small";
                case Blazorise.Size.Small:
                    return "small";
                case Blazorise.Size.Medium:
                    return "medium";
                case Blazorise.Size.Large:
                    return "large";
                case Blazorise.Size.ExtraLarge:
                    return "extra-large";
                default:
                    return null;
            }
        }

        public override string ToBreakpoint( Breakpoint breakpoint )
        {
            switch ( breakpoint )
            {
                case Blazorise.Breakpoint.Mobile:
                    return "mobile";
                case Blazorise.Breakpoint.Tablet:
                    return "tablet";
                case Blazorise.Breakpoint.Desktop:
                    return "desktop";
                case Blazorise.Breakpoint.Widescreen:
                    return "widescreen";
                case Blazorise.Breakpoint.FullHD:
                    return "fullhd";
                default:
                    return null;
            }
        }

        public override string ToBackground( Background color )
        {
            switch ( color )
            {
                case Blazorise.Background.Primary:
                    return "primary";
                case Blazorise.Background.Secondary:
                    return "light";
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

        public override string ToTextColor( TextColor textColor )
        {
            switch ( textColor )
            {
                case Blazorise.TextColor.Primary:
                    return "primary";
                //case Blazorise.TextColor.Secondary:
                //    return "secondary";
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
                //case Blazorise.TextColor.Body:
                //    return "body";
                //case Blazorise.TextColor.Muted:
                //    return "muted";
                case Blazorise.TextColor.White:
                    return "white";
                //case Blazorise.TextColor.Black50:
                //    return "black-50";
                //case Blazorise.TextColor.White50:
                //    return "white-50";
                default:
                    return null;
            }
        }

        public override string ToFloat( Float @float )
        {
            switch ( @float )
            {
                case Blazorise.Float.Left:
                    return "is-pulled-left";
                case Blazorise.Float.Right:
                    return "is-pulled-right	";
                default:
                    return null;
            }
        }

        public override string ToColumnWidth( ColumnWidth columnWidth )
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
                default:
                    return null;
            }
        }

        public override string ToScreenreader( Screenreader screenreader )
        {
            switch ( screenreader )
            {
                case Blazorise.Screenreader.Only:
                    return "is-sr-only";
                case Blazorise.Screenreader.OnlyFocusable:
                    return "is-sr-only-focusable";
                default:
                    return null;
            }
        }

        public override string ToValidationStatus( ValidationStatus validationStatus )
        {
            switch ( validationStatus )
            {
                case Blazorise.ValidationStatus.Success:
                    return "is-success";
                case Blazorise.ValidationStatus.Error:
                    return "is-danger";
                default:
                    return null;
            }
        }

        public override string ToTextAlignment( TextAlignment textAlignment )
        {
            switch ( textAlignment )
            {
                case Blazorise.TextAlignment.Left:
                    return "left";
                case Blazorise.TextAlignment.Center:
                    return "centered";
                case Blazorise.TextAlignment.Right:
                    return "right";
                case Blazorise.TextAlignment.Justified:
                    return "justified";
                default:
                    return null;
            }
        }

        #endregion

        public override bool UseCustomInputStyles { get; set; } = false;

        public override string Provider => "Bulma";
    }
}
