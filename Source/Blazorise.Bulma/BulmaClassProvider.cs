#region Using directives
using System.Collections.Generic;
using System.Linq;
using System.Text;
#endregion

namespace Blazorise.Bulma;

public class BulmaClassProvider : ClassProvider
{
    #region TextEdit

    public override string TextEdit( bool plaintext ) => plaintext ? "input is-static" : "input";

    public override string TextEditSize( Size size ) => size != Size.Default ? $"is-{ToSize( size )}" : null;

    public override string TextEditColor( Color color ) => color != Color.Default ? $"is-{ToColor( color )}" : null;

    public override string TextEditValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

    #endregion

    #region MemoEdit

    public override string MemoEdit( bool plaintext ) => plaintext ? "textarea is-static" : "textarea";

    public override string MemoEditSize( Size size ) => size != Size.Default ? $"is-{ToSize( size )}" : null;

    public override string MemoEditValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

    #endregion

    #region Select

    public override string Select() => "select is-fullwidth";

    public override string SelectMultiple() => "is-multiple";

    public override string SelectSize( Size size ) => size != Size.Default ? $"is-{ToSize( size )}" : null;

    public override string SelectValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

    #endregion

    #region NumericEdit

    public override string NumericEdit( bool plaintext ) => plaintext ? "input is-static" : "input";

    public override string NumericEditSize( Size size ) => size != Size.Default ? $"is-{ToSize( size )}" : null;

    public override string NumericEditColor( Color color ) => color != Color.Default ? $"is-{ToColor( color )}" : null;

    public override string NumericEditValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

    #endregion

    #region DateEdit

    public override string DateEdit( bool plaintext ) => plaintext ? "input is-static" : "input";

    public override string DateEditSize( Size size ) => size != Size.Default ? $"is-{ToSize( size )}" : null;

    public override string DateEditColor( Color color ) => color != Color.Default ? $"is-{ToColor( color )}" : null;

    public override string DateEditValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

    #endregion

    #region TimeEdit

    public override string TimeEdit( bool plaintext ) => plaintext ? "input is-static" : "input";

    public override string TimeEditSize( Size size ) => size != Size.Default ? $"is-{ToSize( size )}" : null;

    public override string TimeEditColor( Color color ) => color != Color.Default ? $"is-{ToColor( color )}" : null;

    public override string TimeEditValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

    #endregion

    #region ColorEdit

    public override string ColorEdit() => "input";

    public override string ColorEditSize( Size size ) => size != Size.Default ? $"is-{ToSize( size )}" : null;

    #endregion

    #region DatePicker

    public override string DatePicker( bool plaintext ) => plaintext ? "input is-static" : "input";

    public override string DatePickerSize( Size size ) => size != Size.Default ? $"is-{ToSize( size )}" : null;

    public override string DatePickerColor( Color color ) => color != Color.Default ? $"is-{ToColor( color )}" : null;

    public override string DatePickerValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

    #endregion

    #region TimePicker

    public override string TimePicker( bool plaintext ) => plaintext ? "input is-static" : "input";

    public override string TimePickerSize( Size size ) => size != Size.Default ? $"is-{ToSize( size )}" : null;

    public override string TimePickerColor( Color color ) => color != Color.Default ? $"is-{ToColor( color )}" : null;

    public override string TimePickerValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

    #endregion

    #region ColorPicker

    public override string ColorPicker() => "input b-input-color-picker";

    public override string ColorPickerSize( Size size ) => size != Size.Default ? $"is-{ToSize( size )}" : null;

    #endregion

    #region NumericPicker

    public override string NumericPicker( bool plaintext ) => plaintext ? "input is-static" : "input";

    public override string NumericPickerSize( Size size ) => size != Size.Default ? $"is-{ToSize( size )}" : null;

    public override string NumericPickerColor( Color color ) => color != Color.Default ? $"is-{ToColor( color )}" : null;

    public override string NumericPickerValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

    #endregion

    #region InputMask

    public override string InputMask( bool plaintext ) => plaintext ? "input is-static" : "input";

    public override string InputMaskSize( Size size ) => size != Size.Default ? $"is-{ToSize( size )}" : null;

    public override string InputMaskColor( Color color ) => color != Color.Default ? $"is-{ToColor( color )}" : null;

    public override string InputMaskValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

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

    public override string RadioGroupSize( bool buttons, Orientation orientation, Size size ) => $"are-{ToSize( size )}";

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

    public override string FileEditSize( Size size ) => size != Size.Default ? $"is-{ToSize( size )}" : null;

    public override string FileEditValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

    #endregion

    #region Slider

    public override string Slider() => "slider";

    public override string SliderColor( Color color ) => $"is-{ToColor( color )}";

    public override string SliderValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

    #endregion

    #region Rating

    public override string Rating() => "rating";

    public override string RatingDisabled( bool disabled ) => disabled ? "rating-disabled" : null;

    public override string RatingReadonly( bool @readonly ) => @readonly ? "rating-readonly" : null;

    public override string RatingItem() => "rating-item";

    public override string RatingItemColor( Color color ) => $"is-{ToColor( color )}";

    public override string RatingItemSelected( bool selected ) => null;

    public override string RatingItemHovered( bool hover ) => hover ? "is-hover" : null;

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

    public override string FieldSize( Size size ) => null;

    public override string FieldJustifyContent( JustifyContent justifyContent ) => ToJustifyContent( justifyContent );

    public override string FieldValidation( ValidationStatus validationStatus ) => null;

    #endregion

    #region FieldLabel

    public override string FieldLabel( bool horizontal ) => horizontal ? "field-label is-normal" : "field-label";

    public override string FieldLabelRequiredIndicator( bool requiredIndicator )
        => requiredIndicator
            ? "field-label-required"
            : null;

    #endregion

    #region FieldBody

    public override string FieldBody() => "field-body";

    #endregion

    #region FieldHelp

    public override string FieldHelp() => "help";

    #endregion

    #region FocusTrap

    public override string FocusTrap() => "focus-trap";

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

    public override string AddonsSize( Size size ) => size != Size.Default ? $"is-{ToSize( size )}" : null;

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

    public override string AddonSize( Size size ) => null;

    public override string AddonLabel() => "button is-static";

    //public override string AddonContainer() => "control";

    #endregion

    #region Inline

    public override string Inline() => "field is-horizontal";

    #endregion

    #region Button

    public override string Button( bool outline ) => "button";

    public override string ButtonColor( Color color, bool outline ) => outline
        ? color != Color.Default ? $"is-{ToColor( color )} is-outlined" : $"is-{ToColor( color )} is-outlined"
        : color != Color.Default ? $"is-{ToColor( color )}" : null;

    public override string ButtonSize( Size size, bool outline ) => size == Size.Default ? null : $"is-{ToSize( size )}";

    public override string ButtonBlock( bool outline ) => "is-fullwidth";

    public override string ButtonActive( bool outline ) => "is-active";

    public override string ButtonDisabled( bool outline ) => "is-disabled";

    public override string ButtonLoading( bool outline ) => "is-loading";

    public override string ButtonStretchedLink( bool stretched ) => stretched ? "is-link-stretched" : null;

    #endregion

    #region Buttons

    public override string Buttons( ButtonsRole role, Orientation orientation )
    {
        if ( role == ButtonsRole.Toolbar )
            return "field is-grouped";

        if ( orientation == Orientation.Vertical )
            return "field has-addons buttons";

        return "field has-addons";
    }

    public override string ButtonsSize( Size size ) => $"are-{ToSize( size )}";

    #endregion

    #region CloseButton

    public override string CloseButton() => "delete";

    #endregion

    #region Dropdown

    public override string Dropdown( bool isDropdownSubmenu ) => "dropdown";

    public override string DropdownDisabled() => "is-disabled";

    public override string DropdownGroup() => "field has-addons";

    public override string DropdownObserverShow() => DropdownShow();

    public override string DropdownShow() => Active();

    public override string DropdownRight() => "is-right";

    public override string DropdownItem() => "dropdown-item";

    public override string DropdownItemActive( bool active ) => active ? Active() : null;

    public override string DropdownItemDisabled( bool disabled ) => disabled ? Disabled() : null;

    public override string DropdownDivider() => "dropdown-divider";

    public override string DropdownHeader() => "dropdown-header";

    public override string DropdownMenu() => "dropdown-menu";

    public override string DropdownMenuPositionStrategy( DropdownPositionStrategy dropdownPositionStrategy )
        => $"dropdown-menu-position-strategy {( dropdownPositionStrategy == DropdownPositionStrategy.Fixed ? "dropdown-menu-position-strategy-fixed" : "dropdown-menu-position-strategy-absolute" )}";

    public override string DropdownFixedHeaderVisible( bool visible )
        => visible ? "dropdown-table-fixed-header-visible" : null;

    public override string DropdownMenuSelector() => "dropdown-menu";

    public override string DropdownMenuScrollable() => "dropdown-menu-scrollable";

    //public override string DropdownMenuBody() => "dropdown-content";

    public override string DropdownMenuVisible( bool visible ) => null;

    public override string DropdownMenuRight() => null;

    public override string DropdownToggle( bool isDropdownSubmenu, bool outline ) => isDropdownSubmenu ? "dropdown-item" : "button dropdown-trigger";

    public override string DropdownToggleSelector( bool isDropdownSubmenu ) => isDropdownSubmenu ? "dropdown-item" : "button dropdown-trigger";

    public override string DropdownToggleColor( Color color, bool outline ) => outline
        ? color != Color.Default ? $"is-{ToColor( color )}" : $"is-outlined"
        : color != Color.Default ? $"is-{ToColor( color )}" : null;

    public override string DropdownToggleSize( Size size, bool outline )
        => size != Size.Default ? $"is-{ToSize( size )}" : null;

    public override string DropdownToggleSplit( bool split ) => null;

    public override string DropdownToggleIcon( bool visible ) => null;

    public override string DropdownDirection( Direction direction ) => direction switch
    {
        Direction.Up => "is-up",
        Direction.End => "is-right",
        Direction.Start => "is-left",
        _ => null,
    };

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

    public override string TabLink( TabPosition tabPosition ) => null;

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

    public override string StepItemMarkerColor( Color color, bool active ) => null;

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

    public override string CarouselSlideIndex( int activeSlideIndex, int slideindex, int totalSlides ) => null;

    public override string CarouselSlideSlidingLeft( bool left ) => null;

    public override string CarouselSlideSlidingRight( bool right ) => null;

    public override string CarouselSlideSlidingPrev( bool previous ) => null;

    public override string CarouselSlideSlidingNext( bool next ) => null;

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

    public override string CardLinkUnstyled( bool unstyled ) => unstyled ? "is-link-unstyled" : null;

    public override string CardLinkActive( bool active ) => LinkActive( active );

    #endregion

    #region ListGroup

    public override string ListGroup() => "list-group";

    public override string ListGroupFlush( bool flush ) => flush ? "list-group-flush" : null;

    public override string ListGroupScrollable( bool scrollable ) => scrollable ? "list-group-scrollable" : null;

    public override string ListGroupItem() => "list-group-item";

    public override string ListGroupItemSelectable() => "list-group-item-action";

    public override string ListGroupItemActive() => Active();

    public override string ListGroupItemDisabled() => Disabled();

    public override string ListGroupItemColor( Color color, bool selectable, bool active ) => $"is-{ToColor( color )}";

    #endregion

    #region Container

    public override string Container( Breakpoint breakpoint )
        => breakpoint > Breakpoint.Desktop ? $"container is-{ToBreakpoint( breakpoint )}" : "container";

    public override string ContainerFluid() => "container-fluid";

    #endregion

    #region Bar

    public override string Bar() => "navbar";

    public override string BarInitial( bool initial ) => initial ? "b-bar-initial" : null;

    public override string BarAlignment( Alignment alignment ) => FlexAlignment( alignment );

    public override string BarThemeContrast( ThemeContrast themeContrast ) => $"b-bar-{ToThemeContrast( themeContrast )}";

    public override string BarBreakpoint( Breakpoint breakpoint ) => $"navbar-expand-{ToBreakpoint( breakpoint )}";

    public override string BarMode( BarMode mode ) => $"b-bar-{ToBarMode( mode )}";

    public override string BarItem( BarMode mode, bool hasDropdown ) => mode == Blazorise.BarMode.Horizontal
        ? hasDropdown
            ? "nav-item dropdown"
            : "nav-item dropdown"
        : "b-bar-item";

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

    public override string BarTogglerCollapsed( BarMode mode, BarTogglerMode togglerMode, bool isShow )
        => isShow || mode != Blazorise.BarMode.Horizontal ? null : Active();

    public override string BarMenu( BarMode mode ) => mode == Blazorise.BarMode.Horizontal ? "navbar-menu" : "b-bar-menu";

    public override string BarMenuShow( BarMode mode ) => Active();

    public override string BarStart( BarMode mode ) => mode == Blazorise.BarMode.Horizontal ? "navbar-start" : "b-bar-start";

    public override string BarEnd( BarMode mode ) => mode == Blazorise.BarMode.Horizontal ? "navbar-end" : "b-bar-end";

    //public override string BarHasDropdown() => "has-dropdown";

    public override string BarDropdown( BarMode mode, bool isBarDropDownSubmenu ) => mode == Blazorise.BarMode.Horizontal && isBarDropDownSubmenu
        ? "dropdown"
        : "b-bar-dropdown";

    public override string BarDropdownShow( BarMode mode ) => Active();

    public override string BarDropdownToggle( BarMode mode, bool isBarDropDownSubmenu ) => mode == Blazorise.BarMode.Horizontal
        ? isBarDropDownSubmenu
            ? "dropdown-item"
            : "navbar-link b-bar-dropdown-toggle"
        : "b-bar-link b-bar-dropdown-toggle";

    public override string BarDropdownToggleDisabled( BarMode mode, bool isBarDropDownSubmenu, bool disabled )
        => mode == Blazorise.BarMode.Horizontal && disabled ? "navbar-link-disabled" : null;

    public override string BarDropdownItem( BarMode mode ) => mode == Blazorise.BarMode.Horizontal ? "navbar-item" : "b-bar-dropdown-item";

    public override string BarDropdownDivider( BarMode mode ) => "navbar-divider";

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

    #region AccordionToggle

    public override string AccordionToggle() => "button";

    public override string AccordionToggleCollapsed( bool collapsed ) => null;

    #endregion

    #region Collapse

    public override string Collapse( bool accordion ) => "card";

    public override string CollapseActive( bool accordion, bool active ) => null;

    public override string CollapseHeader( bool accordion ) => "card-header";

    public override string CollapseBody( bool accordion ) => "collapse";

    public override string CollapseBodyActive( bool accordion, bool active ) => active ? Show() : null;

    public override string CollapseBodyContent( bool accordion, bool firstInAccordion, bool lastInAccordion ) => "card-content";

    #endregion

    #region Row

    public override string Row() => "columns is-multiline";

    public override string RowColumns( RowColumnsSize rowColumnsSize, RowColumnsDefinition rowColumnsDefinition )
    {
        if ( rowColumnsDefinition.Breakpoint != Breakpoint.None && rowColumnsDefinition.Breakpoint != Breakpoint.Mobile )
            return $"are-columns-{ToBreakpoint( rowColumnsDefinition.Breakpoint )}-{ToRowColumnsSize( rowColumnsSize )}";

        return $"are-columns-{ToRowColumnsSize( rowColumnsSize )}";
    }

    public override string RowNoGutters( bool noGutters ) => noGutters ? "is-gapless" : null;

    #endregion

    #region Column

    public override string Column( bool grid, bool hasSizes ) => hasSizes ? null : "column";

    public override string Column( bool grid, ColumnWidth columnWidth, Breakpoint breakpoint, bool offset )
    {
        if ( grid )
        {
            if ( breakpoint != Blazorise.Breakpoint.None )
            {
                return $"is-grid-column-{ToColumnWidth( columnWidth )}-{ToBreakpoint( breakpoint )}";
            }

            return $"is-grid-column-{ToColumnWidth( columnWidth )}";
        }

        var baseClass = offset ? "offset-" : null;

        if ( breakpoint != Blazorise.Breakpoint.None )
        {
            if ( columnWidth == Blazorise.ColumnWidth.Default )
                return $"column is-{baseClass}{ToBreakpoint( breakpoint )}";

            return $"column is-{ToColumnWidth( columnWidth )}-{baseClass}{ToBreakpoint( breakpoint )}";
        }

        return $"column is-{baseClass}{ToColumnWidth( columnWidth )}";
    }

    public override string Column( bool grid, IEnumerable<ColumnDefinition> columnDefinitions )
       => string.Join( ' ', columnDefinitions.Select( x => Column( grid, x.ColumnWidth, x.Breakpoint, x.Offset ) ) );

    #endregion

    #region Grid

    public override string Grid() => "grid";

    public override string GridRows( GridRowsSize gridRows, GridRowsDefinition gridRowsDefinition )
    {
        if ( gridRowsDefinition.Breakpoint != Breakpoint.None && gridRowsDefinition.Breakpoint != Breakpoint.Mobile )
            return $"are-grid-rows-{ToGridRowsSize( gridRows )}-{ToBreakpoint( gridRowsDefinition.Breakpoint )}";

        return $"are-grid-rows-{ToGridRowsSize( gridRows )}";
    }

    public override string GridColumns( GridColumnsSize gridColumns, GridColumnsDefinition gridColumnsDefinition )
    {
        if ( gridColumnsDefinition.Breakpoint != Breakpoint.None && gridColumnsDefinition.Breakpoint != Breakpoint.Mobile )
            return $"are-grid-columns-{ToGridColumnsSize( gridColumns )}-{ToBreakpoint( gridColumnsDefinition.Breakpoint )}";

        return $"are-grid-columns-{ToGridColumnsSize( gridColumns )}";
    }

    #endregion

    #region Display

    public override string Display( DisplayType displayType, DisplayDefinition displayDefinition )
    {
        var baseClass = displayDefinition.Breakpoint != Breakpoint.None
            ? $"is-{ToDisplayType( displayType )}-{ToBreakpoint( displayDefinition.Breakpoint )}"
            : $"is-{ToDisplayType( displayType )}";

        if ( displayDefinition.Direction != DisplayDirection.Default )
            return $"{baseClass} is-flex-direction-{ToDisplayDirection( displayDefinition.Direction )}";

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

    public override string ModalFade() => Fade();

    public override string ModalFade( bool animation ) => animation ? Fade() : null;

    public override string ModalVisible( bool visible ) => visible ? Active() : null;

    public override string ModalSize( ModalSize modalSize ) => null;

    public override string ModalBackdrop() => "modal-background";

    public override string ModalBackdropFade() => Fade();

    public override string ModalBackdropVisible( bool visible ) => visible ? Show() : null;

    public override string ModalContent( bool dialog ) => dialog ? "modal-card" : "modal-content";

    public override string ModalContentSize( ModalSize modalSize )
    {
        if ( modalSize == Blazorise.ModalSize.Default )
            return null;

        if ( modalSize == Blazorise.ModalSize.Fullscreen )
            return "modal-fullscreen";

        return $"modal-{ToModalSize( modalSize )}";
    }

    public override string ModalContentCentered( bool centered ) => null;

    public override string ModalContentScrollable( bool scrollable ) => null;

    public override string ModalBody() => "modal-card-body";

    public override string ModalHeader() => "modal-card-head";

    public override string ModalFooter() => "modal-card-foot";

    public override string ModalTitle() => "modal-card-title";

    #endregion

    #region Offcanvas

    public override string Offcanvas() => "offcanvas";

    public override string OffcanvasPlacement( Placement placement, bool visible )
    {
        return placement switch
        {
            Placement.Start => "is-start",
            Placement.End => "is-end",
            Placement.Top => "is-top",
            Placement.Bottom => "is-bottom",
            _ => "",
        };
    }

    public override string OffcanvasFade( bool showing, bool hiding ) => showing
        ? "is-showing"
        : hiding
            ? "is-hiding"
            : null;

    public override string OffcanvasVisible( bool visible ) => visible ? Active() : null;

    public override string OffcanvasHeader() => "offcanvas-header";

    public override string OffcanvasFooter() => "offcanvas-footer";

    public override string OffcanvasBody() => "offcanvas-body";

    public override string OffcanvasBackdrop() => "offcanvas-backdrop";

    public override string OffcanvasBackdropFade() => null;

    public override string OffcanvasBackdropVisible( bool visible ) => visible ? Active() : null;

    #endregion

    #region Pagination

    public override string Pagination() => "pagination-list";

    public override string PaginationSize( Size size ) => $"is-{ToSize( size )}";

    public override string PaginationItem() => null;

    public override string PaginationItemActive() => null;

    public override string PaginationItemDisabled() => null;

    public override string PaginationLink() => "pagination-link";

    public override string PaginationLinkSize( Size size ) => null;

    public override string PaginationLinkActive( bool active ) => active ? "is-current" : null;

    public override string PaginationLinkDisabled( bool disabled ) => disabled ? "is-disabled" : null;

    #endregion

    #region Progress

    public override string Progress() => "progress";

    public override string ProgressSize( Size size ) => $"is-{ToSize( size )}";

    public override string ProgressColor( Color color ) => $"is-{ToColor( color )}";

    public override string ProgressStriped() => "progress-striped";

    public override string ProgressAnimated() => "progress-animated";

    public override string ProgressIndeterminate() => "progress-indeterminate";

    public override string ProgressWidth( int width ) => null;

    public override string ProgressBar() => "progress-bar";

    public override string ProgressBarSize( Size size ) => $"is-{ToSize( size )}";

    public override string ProgressBarColor( Color color ) => $"is-{ToColor( color )}";

    public override string ProgressBarStriped() => "progress-striped";

    public override string ProgressBarAnimated() => "progress-animated";

    public override string ProgressBarIndeterminate() => "progress-indeterminate";

    public override string ProgressBarWidth( int width ) => null;

    #endregion

    #region Chart

    public override string Chart() => null;

    #endregion

    #region Colors

    public override string BackgroundColor( Background background ) => $"has-background-{ToBackground( background )}";

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

    public override string TableHeaderThemeContrast( ThemeContrast themeContrast ) => $"table-thead-theme has-background-{ToThemeContrast( themeContrast )}";

    public override string TableHeaderCell() => null;

    public override string TableHeaderCellCursor( Cursor cursor ) => cursor != Cursor.Default ? $"is-cursor-{ToCursor( cursor )}" : null;

    public override string TableHeaderCellFixed( TableColumnFixedPosition fixedPosition )
    {
        return fixedPosition switch
        {
            TableColumnFixedPosition.Start => "is-header-cell-fixed-start",
            TableColumnFixedPosition.End => "is-header-cell-fixed-end",
            _ => null,
        };
    }

    public override string TableFooter() => null;

    public override string TableBody() => null;

    public override string TableRow( bool striped, bool hoverable ) => null;

    public override string TableRowColor( Color color ) => $"has-background-{ToColor( color )}";

    public override string TableRowHoverCursor() => "table-row-selectable";

    public override string TableRowIsSelected() => "is-selected";

    public override string TableRowHeader() => null;

    public override string TableRowHeaderFixed( TableColumnFixedPosition fixedPosition )
    {
        return fixedPosition switch
        {
            TableColumnFixedPosition.Start => "is-row-header-fixed-start",
            TableColumnFixedPosition.End => "is-row-header-fixed-end",
            _ => null,
        };
    }

    public override string TableRowCell() => null;

    public override string TableRowCellColor( Color color ) => $"has-background-{ToColor( color )}";

    public override string TableRowCellFixed( TableColumnFixedPosition fixedPosition )
    {
        return fixedPosition switch
        {
            TableColumnFixedPosition.Start => "is-row-cell-fixed-start",
            TableColumnFixedPosition.End => "is-row-cell-fixed-end",
            _ => null,
        };
    }

    public override string TableRowGroup( bool expanded ) => "table-group";

    public override string TableRowGroupCell() => "table-group-cell";

    public override string TableRowGroupIndentCell() => "table-group-indentcell";

    public override string TableResponsive( bool responsive ) => responsive ? "table-container" : null;

    public override string TableFixedHeader( bool fixedHeader ) => fixedHeader ? "table-container-fixed-header" : null;

    public override string TableFixedColumns( bool fixedColumns ) => fixedColumns ? "table-container-fixed-columns" : null;

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

    public override string TextOverflow( TextOverflow textOverflow ) => $"has-text-{ToTextOverflow( textOverflow )}";

    public override string TextSize( TextSize textSize ) => $"is-size-{ToTextSize( textSize )}";

    public override string TextItalic() => "is-italic";

    #endregion

    #region Code

    public override string Code() => null;

    #endregion

    #region Heading

    public override string HeadingSize( HeadingSize headingSize ) => $"title is-{ToHeadingSize( headingSize )}";

    #endregion

    #region DisplayHeading

    public override string DisplayHeadingSize( DisplayHeadingSize displayHeadingSize ) => $"title is-{ToDisplayHeadingSize( displayHeadingSize )}";

    #endregion

    #region Lead

    public override string Lead() => "lead";

    #endregion

    #region Paragraph

    public override string Paragraph() => null;

    public override string ParagraphColor( TextColor textColor ) => $"has-text-{ToTextColor( textColor )}";

    #endregion

    #region Blockquote

    public override string Blockquote() => "blockquote";

    public override string BlockquoteFooter() => "blockquote-footer";

    #endregion

    #region Figure

    public override string Figure() => "image";

    public override string FigureSize( FigureSize figureSize ) => $"is-{ToFigureSize( figureSize )}";

    public override string FigureImage() => "figure-img";

    public override string FigureImageRounded() => "is-rounded";

    public override string FigureCaption() => "figure-caption";

    #endregion

    #region Image

    public override string Image() => null;

    public override string ImageFluid( bool fluid ) => fluid ? "is-fullwidth" : null;

    #endregion

    #region Breadcrumb

    public override string Breadcrumb() => "breadcrumb";

    public override string BreadcrumbItem() => null;

    public override string BreadcrumbItemActive() => Active();

    public override string BreadcrumbLink() => null;

    #endregion

    #region Tooltip

    public override string Tooltip() => "b-tooltip";

    public override string TooltipPlacement( TooltipPlacement tooltipPlacement ) => $"b-tooltip-{ToTooltipPlacement( tooltipPlacement )}";

    public override string TooltipMultiline() => "b-tooltip-multiline";

    public override string TooltipAlwaysActive() => "b-tooltip-active";

    public override string TooltipFade() => "b-tooltip-fade";

    public override string TooltipInline() => "b-tooltip-inline";

    #endregion

    #region Divider

    public override string Divider() => "divider";

    public override string DividerType( DividerType dividerType ) => $"{Divider()}-{ToDividerType( dividerType )}";

    #endregion

    #region Link

    public override string Link() => null;

    public override string LinkActive( bool active ) => active ? Active() : null;

    public override string LinkUnstyled( bool unstyled ) => unstyled ? "is-link-unstyled" : null;

    public override string LinkStretched( bool stretched ) => stretched ? "is-link-stretched" : null;

    public override string LinkDisabled( bool disabled ) => disabled ? "is-link-disabled" : null;

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

    #region Gap

    public override string Gap( GapSize gapSize, GapSide gapSide )
    {
        return $"is-gap-{ToGapSize( gapSize )}";
    }

    public override string Gap( GapSize gapSize, IEnumerable<GapSide> rules )
        => string.Join( " ", rules.Select( x => Gap( gapSize, x ) ) );

    #endregion

    #region Borders

    public override string Border( BorderSize borderSize, BorderSide borderSide, BorderColor borderColor )
    {
        var sb = new StringBuilder( "has-border" );

        if ( borderSide != BorderSide.All )
            sb.Append( '-' ).Append( ToBorderSide( borderSide ) );

        if ( borderSize != BorderSize.Default )
            sb.Append( '-' ).Append( ToBorderSize( borderSize ) );

        if ( borderColor != BorderColor.None )
            sb.Append( " has-border-" ).Append( ToBorderColor( borderColor ) );

        return sb.ToString();
    }

    public override string Border( BorderSize borderSize, IEnumerable<(BorderSide borderSide, BorderColor borderColor)> rules )
        => string.Join( " ", rules.Select( x => Border( borderSize, x.borderSide, x.borderColor ) ) );

    public override string BorderRadius( BorderRadius borderRadius )
        => $"has-{ToBorderRadius( borderRadius )}";

    #endregion

    #region Flex

    public override string Flex( FlexType flexType )
    {
        return flexType != FlexType.Default
            ? $"is-{ToFlexType( flexType )}"
            : null;
    }

    public override string Flex( FlexDefinition flexDefinition )
    {
        var sb = new StringBuilder();

        var breakpoint = flexDefinition.Breakpoint != Breakpoint.None && flexDefinition.Breakpoint != Breakpoint.Mobile
            ? $"{ToBreakpoint( flexDefinition.Breakpoint )}-"
            : null;

        if ( flexDefinition.Direction != FlexDirection.Default )
            sb.Append( "is-flex-direction-" ).Append( breakpoint ).Append( ToDirection( flexDefinition.Direction ) );

        if ( flexDefinition.JustifyContent != FlexJustifyContent.Default )
            sb.Append( "is-justify-content-" ).Append( breakpoint ).Append( ToJustifyContent( flexDefinition.JustifyContent ) );

        if ( flexDefinition.AlignItems != FlexAlignItems.Default )
            sb.Append( "is-align-items-" ).Append( breakpoint ).Append( ToAlignItems( flexDefinition.AlignItems ) );

        if ( flexDefinition.AlignSelf != FlexAlignSelf.Default )
            sb.Append( "is-align-self-" ).Append( breakpoint ).Append( ToAlignSelf( flexDefinition.AlignSelf ) );

        if ( flexDefinition.AlignContent != FlexAlignContent.Default )
            sb.Append( "is-align-content-" ).Append( breakpoint ).Append( ToAlignContent( flexDefinition.AlignContent ) );

        if ( flexDefinition.GrowShrink != FlexGrowShrink.Default && flexDefinition.GrowShrinkSize != FlexGrowShrinkSize.Default )
            sb.Append( "is-flex-" ).Append( breakpoint ).Append( ToGrowShrink( flexDefinition.GrowShrink ) ).Append( "-" ).Append( ToGrowShrinkSize( flexDefinition.GrowShrinkSize ) );

        if ( flexDefinition.Wrap != FlexWrap.Default )
            sb.Append( "is-flex-wrap-" ).Append( breakpoint ).Append( ToWrap( flexDefinition.Wrap ) );

        if ( flexDefinition.Order != FlexOrder.Default )
            sb.Append( "is-flex-order-" ).Append( breakpoint ).Append( ToOrder( flexDefinition.Order ) );

        if ( flexDefinition.Fill )
            sb.Append( "is-flex-" ).Append( breakpoint ).Append( "fill" );

        return sb.ToString();
    }

    public override string Flex( FlexType flexType, IEnumerable<FlexDefinition> flexDefinitions )
    {
        var sb = new StringBuilder();

        if ( flexType != FlexType.Default )
            sb.Append( $"is-{ToFlexType( flexType )}" ).Append( ' ' );

        sb.Append( string.Join( ' ', flexDefinitions.Select( x => Flex( x ) ) ) );

        return sb.ToString();
    }

    public override string FlexAlignment( Alignment alignment ) => $"justify-content-{ToAlignment( alignment )}";

    #endregion

    #region Sizing

    public override string Sizing( SizingType sizingType, SizingSize sizingSize, SizingDefinition sizingDefinition )
    {
        var sb = new StringBuilder( "is-" );

        if ( sizingDefinition.IsMin && sizingDefinition.IsViewport )
            sb.Append( "min-viewport-" );
        else if ( sizingDefinition.IsMax )
            sb.Append( "max-" );
        else if ( sizingDefinition.IsViewport )
            sb.Append( "viewport-" );

        sb.Append( sizingType == SizingType.Width
            ? "width"
            : "height" );

        if ( sizingDefinition.Breakpoint != Breakpoint.None && sizingDefinition.Breakpoint != Breakpoint.Mobile )
            sb.Append( $"-{ToBreakpoint( sizingDefinition.Breakpoint )}" );

        sb.Append( $"-{ToSizingSize( sizingSize )}" );

        return sb.ToString();
    }

    public override string Sizing( SizingType sizingType, SizingSize sizingSize, IEnumerable<SizingDefinition> rules )
        => string.Join( " ", rules.Select( x => Sizing( sizingType, sizingSize, x ) ) );

    #endregion

    #region Float

    public override string Float( Float @float ) => $"is-pulled-{ToFloat( @float )}";

    public override string Clearfix() => "is-clearfix";

    #endregion

    #region Visibility

    public override string Visibility( Visibility visibility )
    {
        return visibility switch
        {
            Blazorise.Visibility.Visible => "is-visible",
            Blazorise.Visibility.Invisible => "is-invisible",
            _ => null,
        };
    }

    #endregion

    #region VerticalAlignment

    public override string VerticalAlignment( VerticalAlignment verticalAlignment )
        => $"is-vertical-align-{ToVerticalAlignment( verticalAlignment )}";

    #endregion

    #region Shadow

    public override string Shadow( Shadow shadow )
    {
        if ( shadow == Blazorise.Shadow.Default )
            return "has-shadow";

        return $"has-shadow-{ToShadow( shadow )}";
    }

    #endregion

    #region Overflow

    public override string Overflow( OverflowType overflowType, OverflowType secondOverflowType ) => secondOverflowType != OverflowType.Default
        ? $"is-overflow-{ToOverflowType( overflowType )}-{ToOverflowType( secondOverflowType )}"
        : $"is-overflow-{ToOverflowType( overflowType )}";

    #endregion

    #region Position

    public override string Position( PositionType positionType, PositionEdgeType edgeType, int edgeOffset, PositionTranslateType translateType )
    {
        return $"is-{ToPositionEdgeType( edgeType )}-{edgeOffset}";
    }

    public override string Position( PositionType positionType, IEnumerable<(PositionEdgeType edgeType, int edgeOffset)> edges, PositionTranslateType translateType )
    {
        var sb = new StringBuilder( $"is-position-{ToPositionType( positionType )}" );

        if ( edges != null && edges.Count() > 0 )
            sb.Append( ' ' ).Append( string.Join( " ", edges.Select( x => Position( positionType, x.edgeType, x.edgeOffset, translateType ) ) ) );

        if ( translateType != PositionTranslateType.None )
            sb.Append( " is-translate-" ).Append( ToPositionTranslateType( translateType ) );

        return sb.ToString();
    }

    #endregion

    #region Elements

    public override string UnorderedList() => "is-unordered-list";

    public override string UnorderedListUnstyled( bool unstyled ) => unstyled ? "is-unordered-list-unstyled" : null;

    public override string OrderedList() => "is-ordered-list";

    public override string OrderedListUnstyled( bool unstyled ) => unstyled ? "is-ordered-list-unstyled" : null;

    public override string OrderedListType( OrderedListType orderedListType ) => $"is-ordered-list-{ToOrderedListType( orderedListType )}";

    public override string DescriptionList() => null;

    public override string DescriptionListTerm() => null;

    public override string DescriptionListDefinition() => null;

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
        return breakpoint switch
        {
            Blazorise.Breakpoint.Mobile or Blazorise.Breakpoint.ExtraSmall => "mobile",
            Blazorise.Breakpoint.Tablet or Blazorise.Breakpoint.Small => "tablet",
            Blazorise.Breakpoint.Desktop or Blazorise.Breakpoint.Medium => "desktop",
            Blazorise.Breakpoint.Widescreen or Blazorise.Breakpoint.Large => "widescreen",
            Blazorise.Breakpoint.FullHD or Blazorise.Breakpoint.ExtraLarge => "fullhd",
            Blazorise.Breakpoint.Full2K or Blazorise.Breakpoint.ExtraExtraLarge => "full24",
            _ => null,
        };
    }

    public override string ToDisplayType( DisplayType displayType )
    {
        return displayType switch
        {
            Blazorise.DisplayType.None => "hidden",
            Blazorise.DisplayType.Block => "block",
            Blazorise.DisplayType.Inline => "inline",
            Blazorise.DisplayType.InlineBlock => "inline-block",
            Blazorise.DisplayType.Flex => "flex",
            Blazorise.DisplayType.InlineFlex => "inline-flex",
            Blazorise.DisplayType.Table => "table",
            Blazorise.DisplayType.TableRow => "table-row",
            Blazorise.DisplayType.TableCell => "table-cell",
            _ => null,
        };
    }

    public override string ToBackground( Background background )
    {
        var name = background?.Name;

        if ( name == "secondary" )
            return "light";

        return name;
    }

    public override string ToTextColor( TextColor textColor )
    {
        var name = textColor?.Name;

        if ( name == "secondary" )
            return "light";

        return name;
    }

    public override string ToColumnWidth( ColumnWidth columnWidth )
    {
        return columnWidth switch
        {
            Blazorise.ColumnWidth.Is1 => "1",
            Blazorise.ColumnWidth.Is2 => "2",
            Blazorise.ColumnWidth.Is3 or Blazorise.ColumnWidth.Quarter => "3",
            Blazorise.ColumnWidth.Is4 or Blazorise.ColumnWidth.Third => "4",
            Blazorise.ColumnWidth.Is5 => "5",
            Blazorise.ColumnWidth.Is6 or Blazorise.ColumnWidth.Half => "6",
            Blazorise.ColumnWidth.Is7 => "7",
            Blazorise.ColumnWidth.Is8 => "8",
            Blazorise.ColumnWidth.Is9 => "9",
            Blazorise.ColumnWidth.Is10 => "10",
            Blazorise.ColumnWidth.Is11 => "11",
            Blazorise.ColumnWidth.Is12 or Blazorise.ColumnWidth.Full => "12",
            Blazorise.ColumnWidth.Auto => "narrow",
            _ => null,
        };
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

    public override string ToTextAlignment( TextAlignment textAlignment ) => textAlignment switch
    {
        Blazorise.TextAlignment.Start => "left",
        Blazorise.TextAlignment.Center => "centered",
        Blazorise.TextAlignment.End => "right",
        Blazorise.TextAlignment.Justified => "justified",
        _ => null,
    };

    public override string ToJustifyContent( FlexJustifyContent justifyContent ) => justifyContent switch
    {
        Blazorise.FlexJustifyContent.Start => "flex-start",
        Blazorise.FlexJustifyContent.End => "flex-end",
        Blazorise.FlexJustifyContent.Center => "center",
        Blazorise.FlexJustifyContent.Between => "space-between",
        Blazorise.FlexJustifyContent.Around => "space-around",
        _ => null,
    };

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

    public override string ToShadow( Shadow shadow )
    {
        return shadow switch
        {
            Blazorise.Shadow.Remove => "none",
            Blazorise.Shadow.Small => "small",
            Blazorise.Shadow.Large => "large",
            _ => null,
        };
    }

    public override string ToBorderRadius( BorderRadius borderRadius )
    {
        return borderRadius switch
        {
            Blazorise.BorderRadius.Rounded => "rounded-border",
            Blazorise.BorderRadius.RoundedTop => "rounded-border-top",
            Blazorise.BorderRadius.RoundedEnd => "rounded-border-right",
            Blazorise.BorderRadius.RoundedBottom => "rounded-border-bottom",
            Blazorise.BorderRadius.RoundedStart => "rounded-border-left",
            Blazorise.BorderRadius.RoundedCircle => "rounded-border-circle",
            Blazorise.BorderRadius.RoundedPill => "rounded-border-pill",
            Blazorise.BorderRadius.RoundedZero => "rounded-border-0",
            _ => null,
        };
    }

    #endregion

    public override bool UseCustomInputStyles { get; set; } = false;

    public override string Provider => "Bulma";
}