#region Using directives
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Blazorise.Extensions;
#endregion

namespace Blazorise.Bulma.Providers;

public class BulmaClassProvider : ClassProvider
{
    #region TextInput

    public override string TextInput( bool plaintext ) => plaintext ? "input is-static" : "input";

    public override string TextInputSize( Size size ) => size != Size.Default ? $"is-{ToSize( size )}" : null;

    public override string TextInputColor( Color color ) => color.IsNotNullOrDefault() ? $"is-{ToColor( color )}" : null;

    public override string TextInputValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

    #endregion

    #region MemoInput

    public override string MemoInput( bool plaintext ) => plaintext ? "textarea is-static" : "textarea";

    public override string MemoInputSize( Size size ) => size != Size.Default ? $"is-{ToSize( size )}" : null;

    public override string MemoInputValidation( ValidationStatus validationStatus ) => validationStatus != ValidationStatus.None ? ToValidationStatus( validationStatus ) : null;

    #endregion

    #region Select

    public override string Select() => "select is-fullwidth";

    public override string SelectMultiple( bool multiple ) => multiple ? "is-multiple" : null;

    public override string SelectSize( Size size ) => size != Size.Default ? $"is-{ToSize( size )}" : null;

    public override string SelectValidation( ValidationStatus validationStatus ) => validationStatus != ValidationStatus.None ? ToValidationStatus( validationStatus ) : null;

    #endregion

    #region NumericInput

    public override string NumericInput( bool plaintext ) => plaintext ? "input is-static" : "input";

    public override string NumericInputSize( Size size ) => size != Size.Default ? $"is-{ToSize( size )}" : null;

    public override string NumericInputColor( Color color ) => color.IsNotNullOrDefault() ? $"is-{ToColor( color )}" : null;

    public override string NumericInputValidation( ValidationStatus validationStatus ) => validationStatus != ValidationStatus.None ? ToValidationStatus( validationStatus ) : null;

    #endregion

    #region DateInput

    public override string DateInput( bool plaintext ) => plaintext ? "input is-static" : "input";

    public override string DateInputSize( Size size ) => size != Size.Default ? $"is-{ToSize( size )}" : null;

    public override string DateInputColor( Color color ) => color.IsNotNullOrDefault() ? $"is-{ToColor( color )}" : null;

    public override string DateInputValidation( ValidationStatus validationStatus ) => validationStatus != ValidationStatus.None ? ToValidationStatus( validationStatus ) : null;

    #endregion

    #region TimeInput

    public override string TimeInput( bool plaintext ) => plaintext ? "input is-static" : "input";

    public override string TimeInputSize( Size size ) => size != Size.Default ? $"is-{ToSize( size )}" : null;

    public override string TimeInputColor( Color color ) => color.IsNotNullOrDefault() ? $"is-{ToColor( color )}" : null;

    public override string TimeInputValidation( ValidationStatus validationStatus ) => validationStatus != ValidationStatus.None ? ToValidationStatus( validationStatus ) : null;

    #endregion

    #region ColorInput

    public override string ColorInput() => "input";

    public override string ColorInputSize( Size size ) => size != Size.Default ? $"is-{ToSize( size )}" : null;

    #endregion

    #region DatePicker

    public override string DatePicker( bool plaintext ) => plaintext ? "input is-static" : "input";

    public override string DatePickerSize( Size size ) => size != Size.Default ? $"is-{ToSize( size )}" : null;

    public override string DatePickerColor( Color color ) => color.IsNotNullOrDefault() ? $"is-{ToColor( color )}" : null;

    public override string DatePickerValidation( ValidationStatus validationStatus ) => validationStatus != ValidationStatus.None ? ToValidationStatus( validationStatus ) : null;

    #endregion

    #region TimePicker

    public override string TimePicker( bool plaintext ) => plaintext ? "input is-static" : "input";

    public override string TimePickerSize( Size size ) => size != Size.Default ? $"is-{ToSize( size )}" : null;

    public override string TimePickerColor( Color color ) => color.IsNotNullOrDefault() ? $"is-{ToColor( color )}" : null;

    public override string TimePickerValidation( ValidationStatus validationStatus ) => validationStatus != ValidationStatus.None ? ToValidationStatus( validationStatus ) : null;

    #endregion

    #region ColorPicker

    public override string ColorPicker() => "input b-input-color-picker";

    public override string ColorPickerSize( Size size ) => size != Size.Default ? $"is-{ToSize( size )}" : null;

    #endregion

    #region NumericPicker

    public override string NumericPicker( bool plaintext ) => plaintext ? "input is-static" : "input";

    public override string NumericPickerSize( Size size ) => size != Size.Default ? $"is-{ToSize( size )}" : null;

    public override string NumericPickerColor( Color color ) => color.IsNotNullOrDefault() ? $"is-{ToColor( color )}" : null;

    public override string NumericPickerValidation( ValidationStatus validationStatus ) => validationStatus != ValidationStatus.None ? ToValidationStatus( validationStatus ) : null;

    #endregion

    #region InputMask

    public override string InputMask( bool plaintext ) => plaintext ? "input is-static" : "input";

    public override string InputMaskSize( Size size ) => size != Size.Default ? $"is-{ToSize( size )}" : null;

    public override string InputMaskColor( Color color ) => color.IsNotNullOrDefault() ? $"is-{ToColor( color )}" : null;

    public override string InputMaskValidation( ValidationStatus validationStatus ) => validationStatus != ValidationStatus.None ? ToValidationStatus( validationStatus ) : null;

    #endregion

    #region Check

    public override string Check() => "is-checkradio";

    public override string CheckSize( Size size ) => size != Size.Default ? $"is-{ToSize( size )}" : null;

    public override string CheckInline( bool inline ) => inline ? "is-inline" : null;

    public override string CheckCursor( Cursor cursor ) => cursor != Cursor.Default ? $"{Check()}-{ToCursor( cursor )}" : null;

    public override string CheckValidation( ValidationStatus validationStatus ) => validationStatus != ValidationStatus.None ? ToValidationStatus( validationStatus ) : null;

    #endregion

    #region RadioGroup

    public override string RadioGroup( bool buttons, Orientation orientation )
        => $"{( buttons ? "buttons has-addons" : "control" )}{( orientation == Orientation.Horizontal ? null : " are-vertical" )}";

    public override string RadioGroupSize( bool buttons, Orientation orientation, Size size ) => size != Size.Default ? $"are-{ToSize( size )}" : null;

    public override string RadioGroupValidation( ValidationStatus validationStatus ) => validationStatus != ValidationStatus.None ? ToValidationStatus( validationStatus ) : null;

    #endregion

    #region Radio

    public override string Radio( bool button ) => "is-checkradio";

    public override string RadioSize( bool button, Size size ) => size != Size.Default ? $"is-{ToSize( size )}" : null;

    public override string RadioInline( bool inline ) => inline ? "is-inline" : null;

    public override string RadioCursor( Cursor cursor ) => cursor != Cursor.Default ? $"is-checkradio-{ToCursor( cursor )}" : null;

    public override string RadioValidation( ValidationStatus validationStatus ) => validationStatus != ValidationStatus.None ? ToValidationStatus( validationStatus ) : null;

    #endregion

    #region Switch

    public override string Switch() => "switch";

    public override string SwitchColor( Color color ) => color.IsNotNullOrDefault() ? $"is-{ToColor( color )}" : null;

    public override string SwitchSize( Size size ) => size != Size.Default ? $"is-{ToSize( size )}" : null;

    public override string SwitchChecked( bool @checked ) => null;

    public override string SwitchCursor( Cursor cursor ) => cursor != Cursor.Default ? $"{Switch()}-{ToCursor( cursor )}" : null;

    public override string SwitchValidation( ValidationStatus validationStatus ) => validationStatus != ValidationStatus.None ? ToValidationStatus( validationStatus ) : null;

    #endregion

    #region FileInput

    public override string FileInput() => "file-input";

    public override string FileInputSize( Size size ) => size != Size.Default ? $"is-{ToSize( size )}" : null;

    public override string FileInputValidation( ValidationStatus validationStatus ) => validationStatus != ValidationStatus.None ? ToValidationStatus( validationStatus ) : null;

    #endregion

    #region Slider

    public override string Slider() => "slider";

    public override string SliderColor( Color color ) => $"is-{ToColor( color )}";

    public override string SliderValidation( ValidationStatus validationStatus ) => validationStatus != ValidationStatus.None ? ToValidationStatus( validationStatus ) : null;

    #endregion

    #region Rating

    public override string Rating() => "rating";

    public override string RatingDisabled( bool disabled ) => disabled ? "rating-disabled" : null;

    public override string RatingReadonly( bool @readonly ) => @readonly ? "rating-readonly" : null;

    public override string RatingItem() => "rating-item";

    public override string RatingItemColor( Color color ) => color.IsNotNullOrDefault() ? $"is-{ToColor( color )}" : null;

    public override string RatingItemSelected( bool selected ) => null;

    public override string RatingItemHovered( bool hover ) => hover ? "is-hover" : null;

    #endregion

    #region Label

    public override string LabelType( LabelType labelType ) => labelType switch
    {
        Blazorise.LabelType.File => "file-label",
        _ => "label",
    };

    public override string LabelCursor( Cursor cursor ) => cursor != Cursor.Default ? $"label-{ToCursor( cursor )}" : null;

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

    #endregion

    #region Field

    public override string Field() => "field";

    public override string FieldHorizontal( bool horizontal ) => horizontal ? "is-horizontal" : null;

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

    public override string FieldLabelScreenreader( Screenreader screenreader ) => screenreader != Screenreader.Always ? ToScreenreader( screenreader ) : null;

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

    public override string ControlCheck( ControlRole role ) => role == ControlRole.Check ? "control" : null;

    public override string ControlRadio( ControlRole role ) => role == ControlRole.Radio ? "control" : null;

    public override string ControlSwitch( ControlRole role ) => role == ControlRole.Switch ? "control" : null;

    public override string ControlFile( ControlRole role ) => role == ControlRole.File ? "file has-name is-fullwidth" : null;

    public override string ControlText( ControlRole role ) => role == ControlRole.Text ? "control" : null;

    public override string ControlInline( ControlRole role, bool inline ) => ( role == ControlRole.Check || role == ControlRole.Radio || role == ControlRole.Switch ) && inline ? "is-inline" : null;

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

    #endregion

    #region Inline

    public override string Inline() => "field is-horizontal";

    #endregion

    #region Button

    public override string Button( bool outline ) => "button";

    public override string ButtonColor( Color color, bool outline ) => outline
        ? color.IsNotNullOrDefault() ? $"is-{ToColor( color )} is-outlined" : $"is-{ToColor( color )} is-outlined"
        : color.IsNotNullOrDefault() ? $"is-{ToColor( color )}" : null;

    public override string ButtonSize( Size size, bool outline ) => size == Size.Default ? null : $"is-{ToSize( size )}";

    public override string ButtonBlock( bool outline, bool block ) => block ? "is-fullwidth" : null;

    public override string ButtonActive( bool outline, bool active ) => active ? "is-active" : null;

    public override string ButtonDisabled( bool outline, bool disabled ) => disabled ? "is-disabled" : null;

    public override string ButtonLoading( bool outline, bool loading ) => loading ? "is-loading" : null;

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

    public override string ButtonsSize( Size size ) => size != Size.Default ? $"are-{ToSize( size )}" : null;

    #endregion

    #region CloseButton

    public override string CloseButton() => "delete";

    #endregion

    #region Dropdown

    public override string Dropdown( bool isDropdownSubmenu ) => "dropdown";

    public override string DropdownDisabled( bool disabled ) => disabled ? "is-disabled" : null;

    public override string DropdownGroup( bool group ) => group ? "field has-addons" : null;

    public override string DropdownObserverShow() => Show();

    public override string DropdownShow( bool show ) => show ? Active() : null;

    public override string DropdownRight( bool rightAligned ) => rightAligned ? "is-right" : null;

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

    public override string DropdownMenuScrollable( bool scrollable ) => scrollable ? "dropdown-menu-scrollable" : null;

    public override string DropdownMenuVisible( bool visible ) => null;

    public override string DropdownMenuEnd( bool endAligned ) => null;

    public override string DropdownToggle( bool isDropdownSubmenu, bool outline ) => isDropdownSubmenu ? "dropdown-item" : "button dropdown-trigger";

    public override string DropdownToggleSelector( bool isDropdownSubmenu ) => isDropdownSubmenu ? "dropdown-item" : "button dropdown-trigger";

    public override string DropdownToggleColor( Color color, bool outline ) => outline
        ? color.IsNotNullOrDefault() ? $"is-{ToColor( color )}" : $"is-outlined"
        : color.IsNotNullOrDefault() ? $"is-{ToColor( color )}" : null;

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

    public override string TabsCards( bool cards ) => null;

    public override string TabsFullWidth( bool fullWidth ) => fullWidth ? "is-fullwidth" : null;

    public override string TabsJustified( bool justified ) => justified ? "is-justified" : null;

    public override string TabsVertical( bool vertical ) => vertical ? "is-vertical" : null; // this is custom class, bulma natively does not have vertical tabs

    public override string TabItem( TabPosition tabPosition ) => null;

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

    public override string StepItemColor( Color color ) => color.IsNotNullOrDefault() ? $"is-{ToColor( color )}" : null;

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

    public override string JumbotronBackground( Background background ) => background.IsNotNullOrDefault() ? $"is-{ToBackground( background )}" : null;

    public override string JumbotronTitle( JumbotronTitleSize jumbotronTitleSize ) => $"title is-{ToJumbotronTitleSize( jumbotronTitleSize )}";

    public override string JumbotronSubtitle() => "subtitle";

    #endregion

    #region Card

    public override string CardDeck() => "card-deck";

    public override string CardGroup() => "card-group";

    public override string Card() => "card";

    public override string CardWhiteText( bool whiteText ) => whiteText ? "has-text-white" : null;

    public override string CardActions() => "card-actions";

    public override string CardBody() => "card-content";

    public override string CardFooter() => "card-footer";

    public override string CardHeader() => "card-header";

    public override string CardImage() => "card-image";

    public override string CardTitle( bool insideHeader ) => insideHeader ? "card-header-title" : "title";

    public override string CardTitleSize( bool insideHeader, HeadingSize? size ) => size is null ? null : $"is-{ToHeadingSize( size.Value )}";

    public override string CardSubtitle( bool insideHeader ) => insideHeader ? "card-header-subtitle" : "subtitle";

    public override string CardSubtitleSize( bool insideHeader, HeadingSize? size ) => size is null ? null : $"is-{ToHeadingSize( size.Value )}";

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

    public override string ListGroupItemSelectable( bool selectable ) => selectable ? "list-group-item-action" : null;

    public override string ListGroupItemActive( bool active ) => active ? Active() : null;

    public override string ListGroupItemDisabled( bool disabled ) => disabled ? Disabled() : null;

    public override string ListGroupItemColor( Color color, bool selectable, bool active ) => $"is-{ToColor( color )}";

    #endregion

    #region Container

    public override string Container( Breakpoint breakpoint )
        => breakpoint > Breakpoint.Desktop ? $"container is-{ToBreakpoint( breakpoint )}" : "container";

    public override string ContainerFluid() => "container-fluid";

    #endregion

    #region Bar

    public override string Bar( BarMode mode ) => "navbar";

    public override string BarInitial( BarMode mode, bool initial ) => mode != Blazorise.BarMode.Horizontal && initial ? "b-bar-initial" : null;

    public override string BarAlignment( BarMode mode, Alignment alignment ) => alignment != Alignment.Default ? $"justify-content-{ToAlignment( alignment )}" : null;

    public override string BarThemeContrast( BarMode mode, ThemeContrast themeContrast ) => themeContrast != ThemeContrast.None ? $"b-bar-{ToThemeContrast( themeContrast )}" : null;

    public override string BarBreakpoint( BarMode mode, Breakpoint breakpoint ) => breakpoint != Breakpoint.None ? $"navbar-expand-{ToBreakpoint( breakpoint )}" : null;

    public override string BarMode( BarMode mode ) => $"b-bar-{ToBarMode( mode )}";

    public override string BarItem( BarMode mode, bool hasDropdown ) => mode == Blazorise.BarMode.Horizontal
        ? hasDropdown
            ? "nav-item dropdown"
            : "nav-item dropdown"
        : "b-bar-item";

    public override string BarItemActive( BarMode mode, bool active ) => active ? Active() : null;

    public override string BarItemDisabled( BarMode mode, bool disabled ) => disabled ? Disabled() : null;

    public override string BarItemHasDropdown( BarMode mode, bool hasDropdown ) => mode == Blazorise.BarMode.Horizontal && hasDropdown ? "has-dropdown" : null;

    public override string BarLink( BarMode mode ) => mode == Blazorise.BarMode.Horizontal ? "navbar-item" : "b-bar-link";

    public override string BarLinkDisabled( BarMode mode, bool disabled ) => disabled ? Disabled() : null;

    public override string BarBrand( BarMode mode ) => mode == Blazorise.BarMode.Horizontal ? "navbar-brand" : "b-bar-brand";

    public override string BarToggler( BarMode mode, BarTogglerMode togglerMode ) => mode == Blazorise.BarMode.Horizontal ? "navbar-burger" :
        togglerMode == BarTogglerMode.Popout ? "b-bar-toggler-popout" : "b-bar-toggler-inline navbar-burger";

    public override string BarTogglerCollapsed( BarMode mode, BarTogglerMode togglerMode, bool isShow )
        => isShow || mode != Blazorise.BarMode.Horizontal ? null : Active();

    public override string BarMenu( BarMode mode ) => mode == Blazorise.BarMode.Horizontal ? "navbar-menu" : "b-bar-menu";

    public override string BarMenuShow( BarMode mode, bool show ) => show ? Active() : null;

    public override string BarStart( BarMode mode ) => mode == Blazorise.BarMode.Horizontal ? "navbar-start" : "b-bar-start";

    public override string BarEnd( BarMode mode ) => mode == Blazorise.BarMode.Horizontal ? "navbar-end" : "b-bar-end";

    public override string BarDropdown( BarMode mode, bool isBarDropDownSubmenu ) => mode == Blazorise.BarMode.Horizontal && isBarDropDownSubmenu
        ? "dropdown"
        : "b-bar-dropdown";

    public override string BarDropdownShow( BarMode mode, bool show ) => show ? Active() : null;

    public override string BarDropdownToggle( BarMode mode, bool isBarDropDownSubmenu ) => mode == Blazorise.BarMode.Horizontal
        ? isBarDropDownSubmenu
            ? "dropdown-item"
            : "navbar-link b-bar-dropdown-toggle"
        : "b-bar-link b-bar-dropdown-toggle";

    public override string BarDropdownToggleDisabled( BarMode mode, bool isBarDropDownSubmenu, bool disabled )
        => mode == Blazorise.BarMode.Horizontal && disabled ? "navbar-link-disabled" : null;

    public override string BarDropdownItem( BarMode mode ) => mode == Blazorise.BarMode.Horizontal ? "navbar-item" : "b-bar-dropdown-item";

    public override string BarDropdownItemDisabled( BarMode mode, bool disabled ) => null;

    public override string BarDropdownDivider( BarMode mode ) => "navbar-divider";

    public override string BarTogglerIcon( BarMode mode ) => null;

    public override string BarDropdownMenu( BarMode mode ) => mode == Blazorise.BarMode.Horizontal ? "navbar-dropdown" : "b-bar-dropdown-menu";

    public override string BarDropdownMenuVisible( BarMode mode, bool visible ) => visible ? Show() : null;

    public override string BarDropdownMenuRight( BarMode mode, bool rightAligned ) => rightAligned && mode == Blazorise.BarMode.Horizontal ? "is-right" : null;

    public override string BarDropdownMenuContainer( BarMode mode ) => mode == Blazorise.BarMode.Horizontal ? null : "b-bar-dropdown-menu-container";

    public override string BarCollapsed( BarMode mode, bool visible ) => null;

    public override string BarLabel( BarMode mode ) => "b-bar-label";

    #endregion

    #region Accordion

    public override string Accordion() => "accordion";

    public override string AccordionToggle() => "accordion-toggle";

    public override string AccordionToggleCollapsed( bool collapsed ) => collapsed ? null : "accordion-toggle-collapsed";

    public override string AccordionItem() => "card";

    public override string AccordionItemActive( bool active ) => null;

    public override string AccordionHeader() => "card-header";

    public override string AccordionBody() => "collapse";

    public override string AccordionBodyActive( bool active ) => active ? Show() : null;

    public override string AccordionBodyContent( bool firstInAccordion, bool lastInAccordion ) => "card-content";

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

    public override string Row() => "columns is-multiline";

    public override string RowColumns( RowColumnsSize rowColumnsSize, RowColumnsDefinition rowColumnsDefinition )
    {
        if ( rowColumnsDefinition.Breakpoint != Breakpoint.None && rowColumnsDefinition.Breakpoint != Breakpoint.Mobile )
            return $"are-columns-{ToBreakpoint( rowColumnsDefinition.Breakpoint )}-{ToRowColumnsSize( rowColumnsSize )}";

        return $"are-columns-{ToRowColumnsSize( rowColumnsSize )}";
    }

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

    public override string AlertColor( Color color ) => color.IsNotNullOrDefault() ? $"is-{ToColor( color )}" : null;

    public override string AlertDismisable( bool dismissable ) => null;

    public override string AlertFade( bool dismissable ) => dismissable ? Fade() : null;

    public override string AlertShow( bool dismissable, bool visible ) => dismissable && visible ? Show() : null;

    public override string AlertHasMessage( bool hasMessage ) => null;

    public override string AlertHasDescription( bool hasDescription ) => null;

    public override string AlertMessage() => null;

    public override string AlertDescription() => null;

    #endregion

    #region Modal

    public override string Modal() => "modal";

    public override string ModalFade( bool showing, bool hiding ) => showing || hiding ? Fade() : null;

    public override string ModalVisible( bool visible ) => visible ? Active() : null;

    public override string ModalSize( ModalSize modalSize ) => null;

    public override string ModalCentered( bool centered ) => null;

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

    public override string OffcanvasBackdropFade( bool showing, bool hiding ) => null;

    public override string OffcanvasBackdropVisible( bool visible ) => visible ? Active() : null;

    #endregion

    #region Toast

    public override string Toast() => "toast";

    public override string ToastAnimated( bool animated ) => null;

    public override string ToastFade( bool visible, bool showing, bool hiding ) => showing
        ? "toast-showing"
        : hiding
            ? "toast-hiding"
            : null;

    public override string ToastVisible( bool visible ) => visible
        ? "toast-show"
        : "toast-hide";

    public override string ToastHeader() => "toast-header";

    public override string ToastBody() => "toast-body";

    public override string Toaster() => "toast-container";

    public override string ToasterPlacement( ToasterPlacement placement ) => placement switch
    {
        Blazorise.ToasterPlacement.Top => "toast-container-top",
        Blazorise.ToasterPlacement.TopStart => "toast-container-top-left",
        Blazorise.ToasterPlacement.TopEnd => "toast-container-top-right",
        Blazorise.ToasterPlacement.Bottom => "toast-container-bottom",
        Blazorise.ToasterPlacement.BottomStart => "toast-container-bottom-left",
        Blazorise.ToasterPlacement.BottomEnd => "toast-container-bottom-right",
        _ => null,
    };

    public override string ToasterPlacementStrategy( ToasterPlacementStrategy placementStrategy ) => placementStrategy switch
    {
        Blazorise.ToasterPlacementStrategy.Fixed => "toast-container-fixed",
        Blazorise.ToasterPlacementStrategy.Absolute => "toast-container-absolute",
        _ => null,
    };

    #endregion

    #region Pagination

    public override string Pagination() => "pagination-list";

    public override string PaginationSize( Size size ) => size != Size.Default ? $"is-{ToSize( size )}" : null;

    public override string PaginationAlignment( Alignment alignment ) => alignment != Alignment.Default ? $"justify-content-{ToAlignment( alignment )}" : null;

    public override string PaginationBackgroundColor( Background background ) => background.IsNotNullOrDefault() ? $"has-background-{ToBackground( background )}" : null;

    public override string PaginationItem() => null;

    public override string PaginationItemActive( bool active ) => active ? Active() : null;

    public override string PaginationItemDisabled( bool disabled ) => null;

    public override string PaginationLink() => "pagination-link";

    public override string PaginationLinkSize( Size size ) => null;

    public override string PaginationLinkActive( bool active ) => active ? "is-current" : null;

    public override string PaginationLinkDisabled( bool disabled ) => disabled ? "is-disabled" : null;

    #endregion

    #region Progress

    public override string Progress() => "progress";

    public override string ProgressSize( Size size ) => size != Size.Default ? $"is-{ToSize( size )}" : null;

    public override string ProgressColor( Color color ) => color.IsNotNullOrDefault() ? $"is-{ToColor( color )}" : null;

    public override string ProgressStriped( bool stripped ) => stripped ? "progress-striped" : null;

    public override string ProgressAnimated( bool animated ) => animated ? "progress-animated" : null;

    public override string ProgressIndeterminate( bool indeterminate ) => indeterminate ? "progress-indeterminate" : null;

    public override string ProgressWidth( int width ) => null;

    public override string ProgressBar() => "progress-bar";

    public override string ProgressBarSize( Size size ) => $"is-{ToSize( size )}";

    public override string ProgressBarColor( Color color ) => color.IsNotNullOrDefault() ? $"is-{ToColor( color )}" : null;

    public override string ProgressBarStriped( bool striped ) => striped ? "progress-striped" : null;

    public override string ProgressBarAnimated( bool animated ) => animated ? "progress-animated" : null;

    public override string ProgressBarIndeterminate( bool indeterminate ) => indeterminate ? "progress-indeterminate" : null;

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

    public override string TableFullWidth( bool fullWidth ) => fullWidth ? "is-fullwidth" : null;

    public override string TableStriped( bool striped ) => striped ? "is-striped" : null;

    public override string TableHoverable( bool hoverable ) => hoverable ? "is-hoverable" : null;

    public override string TableBordered( bool bordered ) => bordered ? "is-bordered" : null;

    public override string TableNarrow( bool narrow ) => narrow ? "is-narrow" : null;

    public override string TableBorderless( bool borderless ) => borderless ? "is-borderless" : null;

    public override string TableHeader() => null;

    public override string TableHeaderThemeContrast( ThemeContrast themeContrast ) => themeContrast != ThemeContrast.None ? $"table-thead-theme has-background-{ToThemeContrast( themeContrast )}" : null;

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

    public override string TableRowColor( Color color ) => color.IsNotNullOrDefault() ? $"has-background-{ToColor( color )}" : null;

    public override string TableRowHoverCursor( Cursor cursor ) => cursor != Cursor.Default ? "table-row-selectable" : null;

    public override string TableRowIsSelected( bool selected ) => selected ? "is-selected" : null;

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

    public override string TableRowCellColor( Color color ) => color.IsNotNullOrDefault() ? $"has-background-{ToColor( color )}" : null;

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

    public override string TableResponsiveMode( TableResponsiveMode responsiveMode ) => responsiveMode == Blazorise.TableResponsiveMode.Mobile ? "is-table-mobile" : null;

    public override string TableCaption() => "table-caption";

    public override string TableCaptionSide( TableCaptionSide side ) => side != Blazorise.TableCaptionSide.Default ? $"table-caption-{ToTableCaptionSide( side )}" : null;

    #endregion

    #region Badge

    public override string Badge() => "tag";

    public override string BadgeColor( Color color, bool subtle )
        => color.IsNotNullOrDefault() ? $"is-{ToColor( color )}{( subtle ? "-subtle" : string.Empty )}" : null;

    public override string BadgePill( bool pill ) => null;

    public override string BadgeClose() => "delete is-small";

    public override string BadgeCloseColor( Color color, bool subtle )
            => color.IsNotNullOrDefault() ? $"is-{ToColor( color )}{( subtle ? "-subtle" : string.Empty )}" : null;

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

    public override string TextDecoration( TextDecoration textDecoration ) => $"has-text-decoration-{ToTextDecoration( textDecoration )}";

    public override string TextWeight( TextWeight textWeight ) => $"has-text-weight-{ToTextWeight( textWeight )}";

    public override string TextOverflow( TextOverflow textOverflow ) => $"has-text-{ToTextOverflow( textOverflow )}";

    public override string TextSize( TextSizeType textSizeType, TextSizeDefinition textSizeDefinition )
    {
        if ( textSizeType == TextSizeType.Default )
            return null;

        if ( textSizeDefinition.Breakpoint != Breakpoint.None && textSizeDefinition.Breakpoint != Breakpoint.Mobile )
            return $"is-size-{ToTextSizeType( textSizeType )}-{ToBreakpoint( textSizeDefinition.Breakpoint )}";

        return $"is-size-{ToTextSizeType( textSizeType )}";
    }

    public override string TextItalic( bool italic ) => italic ? "is-italic" : null;

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

    public override string FigureSize( FigureSize figureSize ) => figureSize != Blazorise.FigureSize.Default ? $"is-{ToFigureSize( figureSize )}" : null;

    public override string FigureImage() => "figure-img";

    public override string FigureImageRounded( bool rounded ) => rounded ? "is-rounded" : null;

    public override string FigureCaption() => "figure-caption";

    #endregion

    #region Image

    public override string Image() => null;

    public override string ImageFluid( bool fluid ) => fluid ? "is-fullwidth" : null;

    #endregion

    #region Breadcrumb

    public override string Breadcrumb() => "breadcrumb";

    public override string BreadcrumbItem() => null;

    public override string BreadcrumbItemActive( bool active ) => active ? Active() : null;

    public override string BreadcrumbLink() => null;

    #endregion

    #region Tooltip

    public override string Tooltip() => "b-tooltip";

    public override string TooltipPlacement( TooltipPlacement tooltipPlacement ) => $"b-tooltip-{ToTooltipPlacement( tooltipPlacement )}";

    public override string TooltipMultiline( bool multiline ) => multiline ? "b-tooltip-multiline" : null;

    public override string TooltipAlwaysActive( bool alwaysActive ) => alwaysActive ? "b-tooltip-active" : null;

    public override string TooltipFade( bool fade ) => fade ? "b-tooltip-fade" : null;

    public override string TooltipInline( bool inline ) => inline ? "b-tooltip-inline" : null;

    #endregion

    #region Skeleton

    public override string Skeleton() => "skeleton-lines";

    public override string SkeletonAnimation( SkeletonAnimation animation ) => animation != Blazorise.SkeletonAnimation.Default ? $"skeleton-lines-{ToSkeletonAnimation( animation )}" : null;

    public override string SkeletonItem() => "skeleton-line";

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

    public override string Spacing( Spacing spacing, SpacingSize spacingSize, Side side, Breakpoint breakpoint )
    {
        if ( breakpoint != Blazorise.Breakpoint.None && breakpoint != Blazorise.Breakpoint.Mobile )
            return $"is-{ToSpacing( spacing )}{ToSide( side )}-{ToBreakpoint( breakpoint )}-{ToSpacingSize( spacingSize )}";

        return $"{ToSpacing( spacing )}{ToSide( side )}-{ToSpacingSize( spacingSize )}";
    }

    public override string Spacing( Spacing spacing, SpacingSize spacingSize, IEnumerable<(Side side, Breakpoint breakpoint)> rules ) => string.Join( " ", rules.Select( x => Spacing( spacing, spacingSize, x.side, x.breakpoint ) ) );

    #endregion

    #region Gap

    public override string Gap( GapSize gapSize, GapSide gapSide )
    {
        return gapSide switch
        {
            GapSide.X => $"is-column-gap-{ToGapSize( gapSize )}",
            GapSide.Y => $"is-row-gap-{ToGapSize( gapSize )}",
            _ => $"is-gap-{ToGapSize( gapSize )}",
        };
    }

    public override string Gap( GapSize gapSize, IEnumerable<GapSide> rules )
        => string.Join( " ", rules.Select( x => Gap( gapSize, x ) ) );

    #endregion

    #region Gutter

    public override string Gutter( GutterSize gutterSize, GutterSide gutterSide, Breakpoint breakpoint )
    {
        var sb = new StringBuilder( "is-gutter" );

        if ( gutterSide != GutterSide.None && gutterSide != GutterSide.All )
            sb.Append( '-' ).Append( ToGutterSide( gutterSide ) );

        if ( breakpoint != Breakpoint.None && breakpoint != Breakpoint.Mobile )
            sb.Append( '-' ).Append( ToBreakpoint( breakpoint ) );

        sb.Append( '-' ).Append( ToGutterSize( gutterSize ) );

        return sb.ToString();
    }

    public override string Gutter( GutterSize gutterSize, IEnumerable<(GutterSide, Breakpoint)> rules )
        => string.Join( " ", rules.Select( x => Gutter( gutterSize, x.Item1, x.Item2 ) ) );

    #endregion

    #region Borders

    public override string Border( BorderSize borderSize, BorderDefinition borderDefinition )
    {
        var sb = new StringBuilder( "has-border" );

        if ( borderDefinition.Side != BorderSide.All )
            sb.Append( '-' ).Append( ToBorderSide( borderDefinition.Side ) );

        if ( borderSize != BorderSize.Default )
            sb.Append( '-' ).Append( ToBorderSize( borderSize ) );

        if ( borderDefinition.Color != BorderColor.None )
        {
            sb.Append( " has-border-" ).Append( ToBorderColor( borderDefinition.Color ) );

            if ( borderDefinition.Subtle )
                sb.Append( "-subtle" );
        }

        return sb.ToString();
    }

    public override string Border( BorderSize borderSize, IEnumerable<BorderDefinition> rules )
        => string.Join( " ", rules.Select( x => Border( borderSize, x ) ) );

    public override string BorderRadius( BorderRadius borderRadius )
        => ToBorderRadius( borderRadius );

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

        if ( flexDefinition.Basis && flexDefinition.BasisSize != FlexBasisSize.Default )
            sb.Append( "is-flex-basis-" ).Append( breakpoint ).Append( ToBasisSize( flexDefinition.BasisSize ) );

        if ( flexDefinition.Wrap != FlexWrap.Default )
            sb.Append( "is-flex-wrap-" ).Append( breakpoint ).Append( ToWrap( flexDefinition.Wrap ) );

        if ( flexDefinition.Order != FlexOrder.Default )
            sb.Append( "is-flex-order-" ).Append( breakpoint ).Append( ToOrder( flexDefinition.Order ) );

        if ( flexDefinition.Fill )
            sb.Append( "is-flex-" ).Append( breakpoint ).Append( "fill" );

        return sb.ToString();
    }

    public override string Flex( FlexRule flexRule )
    {
        var sb = new StringBuilder();

        if ( flexRule.FlexType != FlexType.Default )
        {
            if ( flexRule.Breakpoint > Breakpoint.Mobile )
                sb.Append( $"is-{ToFlexType( flexRule.FlexType )}-{ToBreakpoint( flexRule.Breakpoint )}" );
            else
                sb.Append( $"is-{ToFlexType( flexRule.FlexType )}" );

            sb.Append( ' ' );
        }

        sb.Append( string.Join( ' ', flexRule.Definitions.Where( x => x.Condition ?? true ).Select( x => Flex( x ) ) ) );

        return sb.ToString();
    }

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
        ? $"is-overflow-x-{ToOverflowType( overflowType )} is-overflow-y-{ToOverflowType( secondOverflowType )}"
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

    #region ObjectFit

    public override string ObjectFit( ObjectFitType objectFitType, ObjectFitDefinition objectFitDefinition )
    {
        if ( objectFitType == ObjectFitType.Default )
            return null;

        if ( objectFitDefinition.Breakpoint != Breakpoint.None && objectFitDefinition.Breakpoint != Breakpoint.Mobile )
            return $"is-object-fit-{ToBreakpoint( objectFitDefinition.Breakpoint )}-{ToObjectFitType( objectFitType )}";

        return $"is-object-fit-{ToObjectFitType( objectFitType )}";
    }

    #endregion

    #region Elements

    public override string UnorderedList() => "is-unordered-list";

    public override string UnorderedListUnstyled( bool unstyled ) => unstyled ? "is-unordered-list-unstyled" : null;

    public override string OrderedList() => "is-ordered-list";

    public override string OrderedListUnstyled( bool unstyled ) => unstyled ? "is-ordered-list-unstyled" : null;

    public override string OrderedListType( OrderedListType orderedListType ) => orderedListType != Blazorise.OrderedListType.Default ? $"is-ordered-list-{ToOrderedListType( orderedListType )}" : null;

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
            Blazorise.Breakpoint.QuadHD or Blazorise.Breakpoint.ExtraExtraLarge => "quadhd",
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
            Blazorise.BorderRadius.Rounded => "has-radius-normal",
            Blazorise.BorderRadius.RoundedTop => "has-rounded-border-top",
            Blazorise.BorderRadius.RoundedEnd => "has-rounded-border-right",
            Blazorise.BorderRadius.RoundedBottom => "has-rounded-border-bottom",
            Blazorise.BorderRadius.RoundedStart => "has-rounded-border-left",
            Blazorise.BorderRadius.RoundedCircle => "has-rounded-border-circle",
            Blazorise.BorderRadius.RoundedPill => "has-radius-rounded",
            Blazorise.BorderRadius.RoundedZero => "is-radiusless",
            _ => null,
        };
    }

    #endregion

    public override bool UseCustomInputStyles { get; set; } = false;

    public override string Provider => "Bulma";
}