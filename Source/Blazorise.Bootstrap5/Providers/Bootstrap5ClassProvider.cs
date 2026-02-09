#region Using directives
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Blazorise.Extensions;
#endregion

namespace Blazorise.Bootstrap5.Providers;

public class Bootstrap5ClassProvider : ClassProvider
{
    #region TextInput

    public override string TextInput( bool plaintext ) => plaintext ? "form-control-plaintext" : "form-control";

    public override string TextInputSize( Size size ) => size != Size.Default ? $"form-control-{ToSize( size )}" : null;

    public override string TextInputColor( Color color ) => color.IsNotNullOrDefault() ? $"text-{ToColor( color )}" : null;

    public override string TextInputValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

    #endregion

    #region MemoInput

    public override string MemoInput( bool plaintext ) => plaintext ? "form-control-plaintext" : "form-control";

    public override string MemoInputSize( Size size ) => size != Size.Default ? $"form-control-{ToSize( size )}" : null;

    public override string MemoInputValidation( ValidationStatus validationStatus ) => validationStatus != ValidationStatus.None ? ToValidationStatus( validationStatus ) : null;

    #endregion

    #region Select

    public override string Select() => "form-select";

    public override string SelectMultiple( bool multiple ) => null;

    public override string SelectSize( Size size ) => size != Size.Default ? $"{Select()}-{ToSize( size )}" : null;

    public override string SelectValidation( ValidationStatus validationStatus ) => validationStatus != ValidationStatus.None ? ToValidationStatus( validationStatus ) : null;

    #endregion

    #region NumericInput

    public override string NumericInput( bool plaintext ) => plaintext ? "form-control-plaintext" : "form-control";

    public override string NumericInputSize( Size size ) => size != Size.Default ? $"form-control-{ToSize( size )}" : null;

    public override string NumericInputColor( Color color ) => color.IsNotNullOrDefault() ? $"text-{ToColor( color )}" : null;

    public override string NumericInputValidation( ValidationStatus validationStatus ) => validationStatus != ValidationStatus.None ? ToValidationStatus( validationStatus ) : null;

    #endregion

    #region DateInput

    public override string DateInput( bool plaintext ) => plaintext ? "form-control-plaintext" : "form-control";

    public override string DateInputSize( Size size ) => size != Size.Default ? $"form-control-{ToSize( size )}" : null;

    public override string DateInputColor( Color color ) => color.IsNotNullOrDefault() ? $"text-{ToColor( color )}" : null;

    public override string DateInputValidation( ValidationStatus validationStatus ) => validationStatus != ValidationStatus.None ? ToValidationStatus( validationStatus ) : null;

    #endregion

    #region TimeInput

    public override string TimeInput( bool plaintext ) => plaintext ? "form-control-plaintext" : "form-control";

    public override string TimeInputSize( Size size ) => size != Size.Default ? $"form-control-{ToSize( size )}" : null;

    public override string TimeInputColor( Color color ) => color.IsNotNullOrDefault() ? $"text-{ToColor( color )}" : null;

    public override string TimeInputValidation( ValidationStatus validationStatus ) => validationStatus != ValidationStatus.None ? ToValidationStatus( validationStatus ) : null;

    #endregion

    #region ColorInput

    public override string ColorInput() => "form-control";

    public override string ColorInputSize( Size size ) => size != Size.Default ? $"form-control-{ToSize( size )}" : null;

    #endregion

    #region DatePicker

    public override string DatePicker( bool plaintext ) => plaintext ? "form-control-plaintext" : "form-control";

    public override string DatePickerSize( Size size ) => size != Size.Default ? $"form-control-{ToSize( size )}" : null;

    public override string DatePickerColor( Color color ) => color.IsNotNullOrDefault() ? $"text-{ToColor( color )}" : null;

    public override string DatePickerValidation( ValidationStatus validationStatus ) => validationStatus != ValidationStatus.None ? ToValidationStatus( validationStatus ) : null;

    #endregion

    #region TimePicker

    public override string TimePicker( bool plaintext ) => plaintext ? "form-control-plaintext" : "form-control";

    public override string TimePickerSize( Size size ) => size != Size.Default ? $"form-control-{ToSize( size )}" : null;

    public override string TimePickerColor( Color color ) => color.IsNotNullOrDefault() ? $"text-{ToColor( color )}" : null;

    public override string TimePickerValidation( ValidationStatus validationStatus ) => validationStatus != ValidationStatus.None ? ToValidationStatus( validationStatus ) : null;

    #endregion

    #region ColorPicker

    public override string ColorPicker() => "form-control b-input-color-picker";

    public override string ColorPickerSize( Size size ) => size != Size.Default ? $"form-control-{ToSize( size )}" : null;

    #endregion

    #region NumericPicker

    public override string NumericPicker( bool plaintext ) => plaintext ? "form-control-plaintext" : "form-control";

    public override string NumericPickerSize( Size size ) => size != Size.Default ? $"form-control-{ToSize( size )}" : null;

    public override string NumericPickerColor( Color color ) => color.IsNotNullOrDefault() ? $"text-{ToColor( color )}" : null;

    public override string NumericPickerValidation( ValidationStatus validationStatus ) => validationStatus != ValidationStatus.None ? ToValidationStatus( validationStatus ) : null;

    #endregion

    #region InputMask

    public override string InputMask( bool plaintext ) => plaintext ? "form-control-plaintext" : "form-control";

    public override string InputMaskSize( Size size ) => size != Size.Default ? $"form-control-{ToSize( size )}" : null;

    public override string InputMaskColor( Color color ) => color.IsNotNullOrDefault() ? $"text-{ToColor( color )}" : null;

    public override string InputMaskValidation( ValidationStatus validationStatus ) => validationStatus != ValidationStatus.None ? ToValidationStatus( validationStatus ) : null;

    #endregion

    #region Check

    public override string Check() => "form-check-input";

    public override string CheckSize( Size size ) => size != Size.Default ? $"{Check()}-{ToSize( size )}" : null;

    public override string CheckInline( bool inline ) => inline ? "form-check-inline" : null;

    public override string CheckCursor( Cursor cursor ) => cursor != Cursor.Default ? $"{Check()}-{ToCursor( cursor )}" : null;

    public override string CheckValidation( ValidationStatus validationStatus ) => validationStatus != ValidationStatus.None ? ToValidationStatus( validationStatus ) : null;

    #endregion

    #region RadioGroup

    public override string RadioGroup( bool buttons, Orientation orientation ) => buttons
        ? orientation == Orientation.Horizontal ? "btn-group btn-group-toggle" : "btn-group-vertical btn-group-toggle"
        : null;

    public override string RadioGroupSize( bool buttons, Orientation orientation, Size size )
    {
        if ( size == Size.Default )
            return null;

        return buttons
            ? orientation == Orientation.Horizontal ? $"btn-group-{ToSize( size )}" : $"btn-group-vertical-{ToSize( size )}"
            : null;
    }

    public override string RadioGroupValidation( ValidationStatus validationStatus ) => validationStatus != ValidationStatus.None ? ToValidationStatus( validationStatus ) : null;

    #endregion

    #region Radio

    public override string Radio( bool button ) => button ? "btn-check" : "form-check-input";

    public override string RadioSize( bool button, Size size ) => size != Size.Default ? $"{Radio( button )}-{ToSize( size )}" : null;

    public override string RadioInline( bool inline ) => inline
        ? UseCustomInputStyles ? "custom-control-inline" : "form-check-inline"
        : null;

    public override string RadioCursor( Cursor cursor ) => cursor != Cursor.Default ? $"{( UseCustomInputStyles ? "custom-control-input" : "form-check-input" )}-{ToCursor( cursor )}" : null;

    public override string RadioValidation( ValidationStatus validationStatus ) => validationStatus != ValidationStatus.None ? ToValidationStatus( validationStatus ) : null;

    #endregion

    #region Switch

    public override string Switch() => "form-check-input";

    public override string SwitchColor( Color color ) => color.IsNotNullOrDefault() ? $"{Switch()}-{ToColor( color )}" : null;

    public override string SwitchSize( Size size ) => size != Size.Default ? $"form-check-input-{ToSize( size )}" : null;

    public override string SwitchChecked( bool @checked ) => null;

    public override string SwitchCursor( Cursor cursor ) => cursor != Cursor.Default ? $"{Switch()}-{ToCursor( cursor )}" : null;

    public override string SwitchValidation( ValidationStatus validationStatus ) => validationStatus != ValidationStatus.None ? ToValidationStatus( validationStatus ) : null;

    #endregion

    #region FileInput

    public override string FileInput() => "form-control";

    public override string FileInputSize( Size size ) => size != Size.Default ? $"{FileInput()}-{ToSize( size )}" : null;

    public override string FileInputValidation( ValidationStatus validationStatus ) => validationStatus != ValidationStatus.None ? ToValidationStatus( validationStatus ) : null;

    #endregion

    #region Slider

    public override string Slider() => "form-range";

    public override string SliderColor( Color color ) => $"form-range-{ToColor( color )}";

    public override string SliderValidation( ValidationStatus validationStatus ) => validationStatus != ValidationStatus.None ? ToValidationStatus( validationStatus ) : null;

    #endregion

    #region Rating

    public override string Rating() => "rating";

    public override string RatingDisabled( bool disabled ) => disabled ? "rating-disabled" : null;

    public override string RatingReadonly( bool @readonly ) => @readonly ? "rating-readonly" : null;

    public override string RatingItem() => "rating-item";

    public override string RatingItemColor( Color color ) => color.IsNotNullOrDefault() ? $"rating-item-{ToColor( color )}" : null;

    public override string RatingItemSelected( bool selected ) => null;

    public override string RatingItemHovered( bool hover ) => hover ? "rating-item-hover" : null;

    #endregion

    #region Label

    public override string LabelType( LabelType labelType )
    {
        return labelType switch
        {
            Blazorise.LabelType.Check or Blazorise.LabelType.Radio or Blazorise.LabelType.Switch => "form-check-label",
            Blazorise.LabelType.File => "input-group-text",
            _ => "form-label",
        };
    }

    public override string LabelCursor( Cursor cursor ) => cursor != Cursor.Default ? $"form-check-label-{ToCursor( cursor )}" : null;

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

    public override string Fields() => "row";

    public override string FieldsBody() => null;

    public override string FieldsColumn() => "col";

    #endregion

    #region Field

    public override string Field() => "form-group";

    public override string FieldHorizontal( bool horizontal ) => horizontal ? "row" : null;

    public override string FieldColumn() => "col";

    public override string FieldSize( Size size ) => null;

    public override string FieldJustifyContent( JustifyContent justifyContent ) => ToJustifyContent( justifyContent );

    public override string FieldValidation( ValidationStatus validationStatus ) => null;

    #endregion

    #region FieldLabel

    public override string FieldLabel( bool horizontal ) => horizontal ? "col-form-label" : "form-label";

    public override string FieldLabelRequiredIndicator( bool requiredIndicator )
        => requiredIndicator
            ? "form-label-required"
            : null;

    public override string FieldLabelScreenreader( Screenreader screenreader ) => screenreader != Screenreader.Always ? ToScreenreader( screenreader ) : null;

    #endregion

    #region FieldBody

    public override string FieldBody() => null;

    #endregion

    #region FieldHelp

    public override string FieldHelp() => "form-text text-muted";

    #endregion

    #region FocusTrap

    public override string FocusTrap() => "focus-trap";

    #endregion

    #region Control

    public override string ControlCheck( ControlRole role ) => role == ControlRole.Check ? "form-check" : null;

    public override string ControlRadio( ControlRole role ) => role == ControlRole.Radio ? "form-check" : null;

    public override string ControlSwitch( ControlRole role ) => role == ControlRole.Switch ? "form-check form-switch" : null;

    public override string ControlFile( ControlRole role ) => role == ControlRole.File ? "input-group form-file" : null;

    public override string ControlText( ControlRole role ) => null;

    public override string ControlInline( ControlRole role, bool inline ) => ( role == ControlRole.Check || role == ControlRole.Radio || role == ControlRole.Switch ) && inline ? "form-check-inline" : null;

    #endregion

    #region Addons

    public override string Addons() => "input-group";

    public override string AddonsSize( Size size ) => size != Size.Default ? $"input-group-{ToSize( size )}" : null;

    public override string AddonsHasButton( bool hasButton ) => null;

    public override string Addon( AddonType addonType ) => null;

    public override string AddonSize( Size size ) => null;

    public override string AddonLabel() => "input-group-text";

    #endregion

    #region Inline

    public override string Inline() => "form-inline";

    #endregion

    #region Button

    public override string Button( bool outline ) => "btn";

    public override string ButtonColor( Color color, bool outline ) => outline
        ? color.IsNotNullOrDefault() ? $"{Button( outline )}-outline-{ToColor( color )}" : $"{Button( outline )}-outline"
        : color.IsNotNullOrDefault() ? $"{Button( outline )}-{ToColor( color )}" : null;

    public override string ButtonSize( Size size, bool outline ) => size == Size.Default ? null : $"{Button( outline )}-{ToSize( size )}";

    public override string ButtonBlock( bool outline, bool block ) => block ? $"{Button( outline )}-block" : null;

    public override string ButtonActive( bool outline, bool active ) => active ? "active" : null;

    public override string ButtonDisabled( bool outline, bool disabled ) => disabled ? "disabled" : null;

    public override string ButtonLoading( bool outline, bool loading ) => null;

    public override string ButtonStretchedLink( bool stretched ) => stretched ? "stretched-link" : null;

    #endregion

    #region Buttons

    public override string Buttons( ButtonsRole role, Orientation orientation )
    {
        if ( role == ButtonsRole.Toolbar )
            return "btn-toolbar";

        if ( orientation == Orientation.Vertical )
            return "btn-group-vertical";

        return "btn-group";
    }

    public override string ButtonsSize( Size size ) => size != Size.Default ? $"btn-group-{ToSize( size )}" : null;

    #endregion

    #region CloseButton

    public override string CloseButton() => "btn-close";

    #endregion

    #region Dropdown

    public override string Dropdown( bool isDropdownSubmenu ) => "dropdown";

    public override string DropdownDisabled( bool disabled ) => disabled ? "dropdown-disabled" : null;

    public override string DropdownGroup( bool group ) => group ? "btn-group" : null;

    public override string DropdownObserverShow() => Show();

    public override string DropdownShow( bool show ) => show ? Show() : null;

    public override string DropdownRight( bool rightAligned ) => null;

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

    public override string DropdownMenuVisible( bool visible ) => visible ? Show() : null;

    public override string DropdownMenuEnd( bool endAligned ) => endAligned ? "dropdown-menu-end" : null;

    public override string DropdownToggle( bool isDropdownSubmenu, bool outline ) => isDropdownSubmenu ? "dropdown-item dropdown-toggle" : "btn dropdown-toggle";

    public override string DropdownToggleSelector( bool isDropdownSubmenu ) => isDropdownSubmenu ? "dropdown-item dropdown-toggle" : "btn dropdown-toggle";

    public override string DropdownToggleColor( Color color, bool outline ) => outline
        ? color.IsNotNullOrDefault() ? $"btn-outline-{ToColor( color )}" : $"btn-outline"
        : color.IsNotNullOrDefault() ? $"btn-{ToColor( color )}" : null;

    public override string DropdownToggleSize( Size size, bool outline )
        => size != Size.Default ? $"btn-{ToSize( size )}" : null;

    public override string DropdownToggleSplit( bool split ) => split ? "dropdown-toggle-split" : null;

    public override string DropdownToggleIcon( bool visible ) => visible ? null : "dropdown-toggle-hidden";

    public override string DropdownDirection( Direction direction )
    {
        return direction switch
        {
            Direction.Up => "dropup",
            Direction.End => "dropend",
            Direction.Start => "dropstart",
            _ => null,
        };
    }

    #endregion

    #region Tabs

    public override string Tabs( bool pills ) => pills ? "nav nav-pills" : "nav nav-tabs";

    public override string TabsCards( bool cards ) => cards ? "card-header-tabs" : null;

    public override string TabsFullWidth( bool fullWidth ) => fullWidth ? "nav-fill" : null;

    public override string TabsJustified( bool justified ) => justified ? "nav-justified" : null;

    public override string TabsVertical( bool vertical ) => vertical ? "flex-column" : null;

    public override string TabItem( TabPosition tabPosition ) => "nav-item";

    public override string TabItemActive( bool active ) => null;

    public override string TabItemDisabled( bool disabled ) => null;

    public override string TabLink( TabPosition tabPosition ) => "nav-link";

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

    public override string StepItemColor( Color color ) => color.IsNotNullOrDefault() ? $"{StepItem()}-{ToColor( color )}" : null;

    public override string StepItemMarker() => "step-circle";

    public override string StepItemMarkerColor( Color color, bool active ) => null;

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

    public override string CarouselSlideIndex( int activeSlideIndex, int slideindex, int totalSlides ) => null;

    public override string CarouselSlideSlidingLeft( bool left ) => left ? "carousel-item-start" : null;

    public override string CarouselSlideSlidingRight( bool right ) => right ? "carousel-item-end" : null;

    public override string CarouselSlideSlidingPrev( bool previous ) => previous ? "carousel-item-prev" : null;

    public override string CarouselSlideSlidingNext( bool next ) => next ? "carousel-item-next" : null;

    public override string CarouselIndicators() => "carousel-indicators";

    public override string CarouselIndicator() => null;

    public override string CarouselIndicatorActive( bool active ) => active ? Active() : null;

    public override string CarouselFade( bool fade ) => fade ? "carousel-fade" : null;

    public override string CarouselCaption() => "carousel-caption";

    #endregion

    #region Jumbotron

    public override string Jumbotron() => "jumbotron";

    public override string JumbotronBackground( Background background ) => background.IsNotNullOrDefault() ? $"jumbotron-{ToBackground( background )}" : null;

    public override string JumbotronTitle( JumbotronTitleSize jumbotronTitleSize ) => $"display-{ToJumbotronTitleSize( jumbotronTitleSize )}";

    public override string JumbotronSubtitle() => "lead";

    #endregion

    #region Card

    public override string CardDeck() => "card-deck";

    public override string CardGroup() => "card-group";

    public override string Card() => "card";

    public override string CardWhiteText( bool whiteText ) => whiteText ? "text-white" : null;

    public override string CardActions() => "card-actions";

    public override string CardBody() => "card-body";

    public override string CardFooter() => "card-footer";

    public override string CardHeader() => "card-header";

    public override string CardImage() => "card-img-top";

    public override string CardTitle( bool insideHeader ) => "card-title";

    public override string CardTitleSize( bool insideHeader, HeadingSize? size ) => null;

    public override string CardSubtitle( bool insideHeader ) => "card-subtitle";

    public override string CardSubtitleSize( bool insideHeader, HeadingSize? size ) => null;

    public override string CardText() => "card-text";

    public override string CardLink() => "card-link";

    public override string CardLinkUnstyled( bool unstyled ) => unstyled ? "link-unstyled" : null;

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

    public override string ListGroupItemColor( Color color, bool selectable, bool active ) => $"{ListGroupItem()}-{ToColor( color )}";

    #endregion

    #region Container

    public override string Container( Breakpoint breakpoint )
        => breakpoint != Breakpoint.None && breakpoint != Breakpoint.Mobile ? $"container-{ToBreakpoint( breakpoint )}" : "container";

    public override string ContainerFluid() => "container-fluid";

    #endregion

    #region Bar

    public override string Bar( BarMode mode ) => "navbar";

    public override string BarInitial( BarMode mode, bool initial ) => mode != Blazorise.BarMode.Horizontal && initial ? "b-bar-initial" : null;

    public override string BarAlignment( BarMode mode, Alignment alignment ) => alignment != Alignment.Default ? $"justify-content-{ToAlignment( alignment )}" : null;

    public override string BarThemeContrast( BarMode mode, ThemeContrast themeContrast ) => themeContrast != ThemeContrast.None ? $"navbar-{ToThemeContrast( themeContrast )} b-bar-{ToThemeContrast( themeContrast )}" : null;

    public override string BarBreakpoint( BarMode mode, Breakpoint breakpoint ) => breakpoint != Breakpoint.None && breakpoint != Breakpoint.Mobile ? $"navbar-expand-{ToBreakpoint( breakpoint )}" : null;

    public override string BarMode( BarMode mode ) => $"b-bar-{ToBarMode( mode )}";

    public override string BarItem( BarMode mode, bool hasDropdown ) => mode == Blazorise.BarMode.Horizontal
        ? hasDropdown
            ? "nav-item dropdown"
            : "nav-item"
        : "b-bar-item";

    public override string BarItemActive( BarMode mode, bool active ) => active ? Active() : null;

    public override string BarItemDisabled( BarMode mode, bool disabled ) => disabled ? Disabled() : null;

    public override string BarItemHasDropdown( BarMode mode, bool hasDropdown ) => null;

    public override string BarLink( BarMode mode ) => mode == Blazorise.BarMode.Horizontal ? "nav-link" : "b-bar-link";

    public override string BarLinkDisabled( BarMode mode, bool disabled ) => disabled ? Disabled() : null;

    public override string BarBrand( BarMode mode ) => mode == Blazorise.BarMode.Horizontal ? "navbar-brand" : "b-bar-brand";

    public override string BarToggler( BarMode mode, BarTogglerMode togglerMode ) => mode == Blazorise.BarMode.Horizontal ? "navbar-toggler" :
        togglerMode == BarTogglerMode.Popout ? "b-bar-toggler-popout" : "b-bar-toggler-inline";

    public override string BarTogglerCollapsed( BarMode mode, BarTogglerMode togglerMode, bool isShow ) => isShow || mode != Blazorise.BarMode.Horizontal ? null : "collapsed";

    public override string BarMenu( BarMode mode ) => mode == Blazorise.BarMode.Horizontal ? "collapse navbar-collapse" : "b-bar-menu";

    public override string BarMenuShow( BarMode mode, bool show ) => show ? Show() : null;

    public override string BarStart( BarMode mode ) => mode == Blazorise.BarMode.Horizontal ? "navbar-nav me-auto" : "b-bar-start";

    public override string BarEnd( BarMode mode ) => mode == Blazorise.BarMode.Horizontal ? "navbar-nav ms-auto" : "b-bar-end";

    public override string BarDropdown( BarMode mode, bool isBarDropDownSubmenu ) => mode == Blazorise.BarMode.Horizontal ? "dropdown" : "b-bar-dropdown";

    public override string BarDropdownShow( BarMode mode, bool show ) => show ? Show() : null;

    public override string BarDropdownToggle( BarMode mode, bool isBarDropDownSubmenu ) => mode == Blazorise.BarMode.Horizontal
        ? isBarDropDownSubmenu
            ? "dropdown-item"
            : "nav-link dropdown-toggle"
        : "b-bar-link b-bar-dropdown-toggle";

    public override string BarDropdownToggleDisabled( BarMode mode, bool isBarDropDownSubmenu, bool disabled )
        => mode == Blazorise.BarMode.Horizontal && disabled ? "disabled" : null;

    public override string BarDropdownToggleIcon( bool isToggleIconVisible ) => isToggleIconVisible ? null : "dropdown-toggle-hidden";

    public override string BarDropdownItem( BarMode mode ) => mode == Blazorise.BarMode.Horizontal ? "dropdown-item" : "b-bar-dropdown-item";

    public override string BarDropdownItemDisabled( BarMode mode, bool disabled ) => null;

    public override string BarTogglerIcon( BarMode mode ) => "navbar-toggler-icon";

    public override string BarDropdownDivider( BarMode mode ) => "dropdown-divider";

    public override string BarDropdownMenu( BarMode mode ) => mode == Blazorise.BarMode.Horizontal ? "dropdown-menu" : "b-bar-dropdown-menu";

    public override string BarDropdownMenuVisible( BarMode mode, bool visible ) => visible ? Show() : null;

    public override string BarDropdownMenuRight( BarMode mode, bool rightAligned ) => rightAligned ? mode == Blazorise.BarMode.Horizontal ? "dropdown-menu-end" : "b-bar-right" : null;

    public override string BarDropdownMenuContainer( BarMode mode ) => mode == Blazorise.BarMode.Horizontal ? null : "b-bar-dropdown-menu-container";

    public override string BarCollapsed( BarMode mode, bool visible ) => null;

    public override string BarLabel( BarMode mode ) => "b-bar-label";

    #endregion

    #region Accordion

    public override string Accordion() => "accordion";

    public override string AccordionToggle() => "accordion-button";

    public override string AccordionToggleCollapsed( bool collapsed ) => collapsed ? null : "collapsed";

    public override string AccordionItem() => "accordion-item";

    public override string AccordionItemActive( bool active ) => null;

    public override string AccordionHeader() => "accordion-header";

    public override string AccordionBody() => "accordion-collapse collapse";

    public override string AccordionBodyActive( bool active ) => active ? Show() : null;

    public override string AccordionBodyContent( bool firstInAccordion, bool lastInAccordion ) => "accordion-body";

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

    public override string RowColumns( RowColumnsSize rowColumnsSize, RowColumnsDefinition rowColumnsDefinition )
    {
        if ( rowColumnsDefinition.Breakpoint != Breakpoint.None && rowColumnsDefinition.Breakpoint != Breakpoint.Mobile )
            return $"row-cols-{ToBreakpoint( rowColumnsDefinition.Breakpoint )}-{ToRowColumnsSize( rowColumnsSize )}";

        return $"row-cols-{ToRowColumnsSize( rowColumnsSize )}";
    }

    #endregion

    #region Column

    public override string Column( bool grid, bool hasSizes ) => hasSizes ? null : grid ? "g-col" : "col";

    public override string Column( bool grid, ColumnWidth columnWidth, Breakpoint breakpoint, bool offset )
    {
        var baseClass = offset
            ? grid ? "g-start" : "offset"
            : grid ? "g-col" : "col";

        if ( breakpoint != Blazorise.Breakpoint.None && breakpoint != Blazorise.Breakpoint.Mobile )
        {
            return $"{baseClass}-{ToBreakpoint( breakpoint )}-{ToColumnWidth( columnWidth )}";
        }

        return $"{baseClass}-{ToColumnWidth( columnWidth )}";
    }

    public override string Column( bool grid, IEnumerable<ColumnDefinition> columnDefinitions )
       => string.Join( ' ', columnDefinitions.Select( x => Column( grid, x.ColumnWidth, x.Breakpoint, x.Offset ) ) );

    #endregion

    #region Grid

    public override string Grid() => "grid";

    public override string GridRows( GridRowsSize gridRows, GridRowsDefinition gridRowsDefinition )
    {
        if ( gridRowsDefinition.Breakpoint != Breakpoint.None && gridRowsDefinition.Breakpoint != Breakpoint.Mobile )
            return $"g-rows-{ToBreakpoint( gridRowsDefinition.Breakpoint )}-{ToGridRowsSize( gridRows )}";

        return $"g-rows-{ToGridRowsSize( gridRows )}";
    }

    public override string GridColumns( GridColumnsSize gridColumns, GridColumnsDefinition gridColumnsDefinition )
    {
        if ( gridColumnsDefinition.Breakpoint != Breakpoint.None && gridColumnsDefinition.Breakpoint != Breakpoint.Mobile )
            return $"g-cols-{ToBreakpoint( gridColumnsDefinition.Breakpoint )}-{ToGridColumnsSize( gridColumns )}";

        return $"g-cols-{ToGridColumnsSize( gridColumns )}";
    }

    #endregion

    #region Display

    public override string Display( DisplayType displayType, DisplayDefinition displayDefinition )
    {
        var baseClass = displayDefinition.Breakpoint != Breakpoint.None && displayDefinition.Breakpoint != Blazorise.Breakpoint.Mobile
            ? $"d-{ToBreakpoint( displayDefinition.Breakpoint )}-{ToDisplayType( displayType )}"
            : $"d-{ToDisplayType( displayType )}";

        if ( displayDefinition.Direction != DisplayDirection.Default )
            return $"{baseClass} flex-{ToDisplayDirection( displayDefinition.Direction )}";

        return baseClass;
    }

    #endregion

    #region Alert

    public override string Alert() => "alert";

    public override string AlertColor( Color color ) => color.IsNotNullOrDefault() ? $"{Alert()}-{ToColor( color )}" : null;

    public override string AlertDismisable( bool dismissable ) => dismissable ? "alert-dismissible" : null;

    public override string AlertFade( bool dismissable ) => dismissable ? Fade() : null;

    public override string AlertShow( bool dismissable, bool visible ) => dismissable && visible ? Show() : null;

    public override string AlertHasMessage( bool hasMessage ) => null;

    public override string AlertHasDescription( bool hasDescription ) => null;

    public override string AlertMessage() => null;

    public override string AlertDescription() => null;

    #endregion

    #region Modal

    public override string Modal() => "modal";

    public override string ModalFade( bool showing, bool hiding ) => showing
        ? "showing"
        : hiding
            ? "hiding"
            : null;

    public override string ModalVisible( bool visible ) => visible ? Show() : null;

    public override string ModalSize( ModalSize modalSize ) => null;

    public override string ModalCentered( bool centered ) => null;

    public override string ModalBackdrop() => "modal-backdrop";

    public override string ModalBackdropFade() => Fade();

    public override string ModalBackdropVisible( bool visible ) => visible ? Show() : null;

    public override string ModalContent( bool dialog ) => "modal-content";

    public override string ModalContentSize( ModalSize modalSize ) => null;

    public override string ModalContentCentered( bool centered ) => null;

    public override string ModalContentScrollable( bool scrollable ) => null;

    public override string ModalBody() => "modal-body";

    public override string ModalHeader() => "modal-header";

    public override string ModalFooter() => "modal-footer";

    public override string ModalTitle() => "modal-title";

    #endregion

    #region Offcanvas

    public override string Offcanvas() => "offcanvas";

    public override string OffcanvasPlacement( Placement placement, bool visible )
    {
        return placement switch
        {
            Placement.Start => "offcanvas-start",
            Placement.End => "offcanvas-end",
            Placement.Top => "offcanvas-top",
            Placement.Bottom => "offcanvas-bottom",
            _ => "",
        };
    }

    public override string OffcanvasFade( bool showing, bool hiding ) => showing
        ? "showing"
        : hiding
            ? "hiding"
            : null;

    public override string OffcanvasVisible( bool visible ) => visible ? Show() : null;

    public override string OffcanvasHeader() => "offcanvas-header";

    public override string OffcanvasFooter() => "offcanvas-footer";

    public override string OffcanvasBody() => "offcanvas-body";

    public override string OffcanvasBackdrop() => "offcanvas-backdrop";

    public override string OffcanvasBackdropFade( bool showing, bool hiding ) => Fade();

    public override string OffcanvasBackdropVisible( bool visible ) => visible ? Show() : null;

    #endregion

    #region Toast

    public override string Toast() => "toast";

    public override string ToastAnimated( bool animated ) => animated ? "fade" : null;

    public override string ToastFade( bool visible, bool showing, bool hiding )
    {
        if ( showing || hiding )
            return "show showing";

        if ( visible )
            return "show";

        return "hide";
    }

    public override string ToastVisible( bool visible ) => null;

    public override string ToastHeader() => "toast-header";

    public override string ToastBody() => "toast-body";

    public override string Toaster() => "toast-container";

    public override string ToasterPlacement( ToasterPlacement placement ) => placement switch
    {
        Blazorise.ToasterPlacement.Top => "p-3 top-0 start-50 translate-middle-x",
        Blazorise.ToasterPlacement.TopStart => "p-3 top-0 start-0",
        Blazorise.ToasterPlacement.TopEnd => "p-3 top-0 end-0",
        Blazorise.ToasterPlacement.Bottom => "p-3 bottom-0 start-50 translate-middle-x",
        Blazorise.ToasterPlacement.BottomStart => "p-3 bottom-0 start-0",
        Blazorise.ToasterPlacement.BottomEnd => "p-3 bottom-0 end-0",
        _ => null,
    };

    public override string ToasterPlacementStrategy( ToasterPlacementStrategy placementStrategy ) => placementStrategy switch
    {
        Blazorise.ToasterPlacementStrategy.Fixed => "position-fixed",
        Blazorise.ToasterPlacementStrategy.Absolute => "position-absolute",
        _ => null,
    };

    #endregion

    #region Pagination

    public override string Pagination() => "pagination";

    public override string PaginationSize( Size size ) => size != Size.Default ? $"{Pagination()}-{ToSize( size )}" : null;

    public override string PaginationAlignment( Alignment alignment ) => alignment != Alignment.Default ? $"justify-content-{ToAlignment( alignment )}" : null;

    public override string PaginationBackgroundColor( Background background ) => background.IsNotNullOrDefault() ? $"bg-{ToBackground( background )}" : null;

    public override string PaginationItem() => "page-item";

    public override string PaginationItemActive( bool active ) => active ? Active() : null;

    public override string PaginationItemDisabled( bool disabled ) => disabled ? Disabled() : null;

    public override string PaginationLink() => "page-link";

    public override string PaginationLinkSize( Size size ) => null;

    public override string PaginationLinkActive( bool active ) => null;

    public override string PaginationLinkDisabled( bool disabled ) => null;

    #endregion

    #region Progress

    public override string Progress() => "progress";

    public override string ProgressSize( Size size ) => size != Size.Default ? $"progress-{ToSize( size )}" : null;

    public override string ProgressColor( Color color ) => null;

    public override string ProgressStriped( bool stripped ) => null;

    public override string ProgressAnimated( bool animated ) => null;

    public override string ProgressIndeterminate( bool indeterminate ) => indeterminate ? "progress-indeterminate" : null;

    public override string ProgressWidth( int width ) => null;

    public override string ProgressBar() => "progress-bar";

    public override string ProgressBarSize( Size size ) => null;

    public override string ProgressBarColor( Color color ) => color.IsNotNullOrDefault() ? $"bg-{ToColor( color )}" : null;

    public override string ProgressBarStriped( bool striped ) => striped ? "progress-bar-striped" : null;

    public override string ProgressBarAnimated( bool animated ) => animated ? "progress-bar-animated" : null;

    public override string ProgressBarIndeterminate( bool indeterminate ) => indeterminate ? "progress-bar-indeterminate" : null;

    public override string ProgressBarWidth( int width ) => null;

    #endregion

    #region Chart

    public override string Chart() => null;

    #endregion

    #region Colors

    public override string BackgroundColor( Background background ) => $"bg-{ToBackground( background )}";

    #endregion

    #region Table

    public override string Table() => "table";

    public override string TableFullWidth( bool fullWidth ) => null;

    public override string TableStriped( bool striped ) => striped ? "table-striped" : null;

    public override string TableHoverable( bool hoverable ) => hoverable ? "table-hover" : null;

    public override string TableBordered( bool bordered ) => bordered ? "table-bordered" : null;

    public override string TableNarrow( bool narrow ) => narrow ? "table-sm" : null;

    public override string TableBorderless( bool borderless ) => borderless ? "table-borderless" : null;

    public override string TableHeader() => null;

    public override string TableHeaderThemeContrast( ThemeContrast themeContrast ) => themeContrast != ThemeContrast.None ? $"table-thead-theme table-{ToThemeContrast( themeContrast )}" : null;

    public override string TableHeaderCell() => null;

    public override string TableHeaderCellCursor( Cursor cursor ) => cursor != Cursor.Default ? $"cursor-{ToCursor( cursor )}" : null;

    public override string TableHeaderCellFixed( TableColumnFixedPosition fixedPosition )
    {
        return fixedPosition switch
        {
            TableColumnFixedPosition.Start => "table-header-cell-fixed-start",
            TableColumnFixedPosition.End => "table-header-cell-fixed-end",
            _ => null,
        };
    }

    public override string TableFooter() => null;

    public override string TableBody() => null;

    public override string TableRow( bool striped, bool hoverable ) => null;

    public override string TableRowColor( Color color ) => color.IsNotNullOrDefault() ? $"table-{ToColor( color )}" : null;

    public override string TableRowHoverCursor( Cursor cursor ) => cursor != Cursor.Default ? "table-row-selectable" : null;

    public override string TableRowIsSelected( bool selected ) => selected ? "selected" : null;

    public override string TableRowHeader() => null;

    public override string TableRowHeaderFixed( TableColumnFixedPosition fixedPosition )
    {
        return fixedPosition switch
        {
            TableColumnFixedPosition.Start => "table-row-header-fixed-start",
            TableColumnFixedPosition.End => "table-row-header-fixed-end",
            _ => null,
        };
    }

    public override string TableRowCell() => null;

    public override string TableRowCellColor( Color color ) => color.IsNotNullOrDefault() ? $"table-{ToColor( color )}" : null;

    public override string TableRowCellFixed( TableColumnFixedPosition fixedPosition )
    {
        return fixedPosition switch
        {
            TableColumnFixedPosition.Start => "table-row-cell-fixed-start",
            TableColumnFixedPosition.End => "table-row-cell-fixed-end",
            _ => null,
        };
    }

    public override string TableRowGroup( bool expanded ) => "table-group";

    public override string TableRowGroupCell() => "table-group-cell";

    public override string TableRowGroupIndentCell() => "table-group-indentcell";

    public override string TableResponsive( bool responsive ) => responsive ? "table-responsive" : null;

    public override string TableFixedHeader( bool fixedHeader ) => fixedHeader ? "table-fixed-header" : null;

    public override string TableFixedColumns( bool fixedColumns ) => fixedColumns ? "table-fixed-columns" : null;

    public override string TableResponsiveMode( TableResponsiveMode responsiveMode ) => responsiveMode == Blazorise.TableResponsiveMode.Mobile ? "table-mobile" : null;

    public override string TableCaption() => "table-caption";

    public override string TableCaptionSide( TableCaptionSide side ) => side != Blazorise.TableCaptionSide.Default ? $"table-caption-{ToTableCaptionSide( side )}" : null;

    #endregion

    #region Badge

    public override string Badge() => "badge";

    public override string BadgeColor( Color color, bool subtle )
        => color.IsNotNullOrDefault() ? $"text-bg-{ToColor( color )}{( subtle ? "-subtle" : string.Empty )}" : null;

    public override string BadgePill( bool pill ) => pill ? "rounded-pill" : null;

    public override string BadgeClose() => "badge-close";

    public override string BadgeCloseColor( Color color, bool subtle )
        => color.IsNotNullOrDefault() ? $"text-bg-{ToColor( color )}{( subtle ? "-subtle" : string.Empty )}" : null;

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

    public override string TextDecoration( TextDecoration textDecoration ) => $"text-decoration-{ToTextDecoration( textDecoration )}";

    public override string TextWeight( TextWeight textWeight ) => $"fw-{ToTextWeight( textWeight )}";

    public override string TextOverflow( TextOverflow textOverflow ) => $"text-{ToTextOverflow( textOverflow )}";

    public override string TextSize( TextSizeType textSizeType, TextSizeDefinition textSizeDefinition )
    {
        if ( textSizeType == TextSizeType.Default )
            return null;

        if ( textSizeDefinition.Breakpoint != Breakpoint.None && textSizeDefinition.Breakpoint != Breakpoint.Mobile )
            return $"fs-{ToBreakpoint( textSizeDefinition.Breakpoint )}-{ToTextSizeType( textSizeType )}";

        return $"fs-{ToTextSizeType( textSizeType )}";
    }

    public override string TextItalic( bool italic ) => italic ? "fst-italic" : null;

    #endregion

    #region Code

    public override string Code() => null;

    #endregion

    #region Heading

    public override string HeadingSize( HeadingSize headingSize ) => $"h{ToHeadingSize( headingSize )}";

    #endregion

    #region DisplayHeading

    public override string DisplayHeadingSize( DisplayHeadingSize displayHeadingSize ) => $"display-{ToDisplayHeadingSize( displayHeadingSize )}";

    #endregion

    #region Lead

    public override string Lead() => "lead";

    #endregion

    #region Paragraph

    public override string Paragraph() => null;

    public override string ParagraphColor( TextColor textColor ) => $"text-{ToTextColor( textColor )}";

    #endregion

    #region Blockquote

    public override string Blockquote() => "blockquote";

    public override string BlockquoteFooter() => "blockquote-footer";

    #endregion

    #region Figure

    public override string Figure() => "figure";

    public override string FigureSize( FigureSize figureSize ) => figureSize != Blazorise.FigureSize.Default ? $"figure-is-{ToFigureSize( figureSize )}" : null;

    public override string FigureImage() => "figure-img img-fluid";

    public override string FigureImageRounded( bool rounded ) => rounded ? "rounded" : null;

    public override string FigureCaption() => "figure-caption";

    #endregion

    #region Image

    public override string Image() => null;

    public override string ImageFluid( bool fluid ) => fluid ? "img-fluid" : null;

    #endregion

    #region Breadcrumb

    public override string Breadcrumb() => "breadcrumb";

    public override string BreadcrumbItem() => "breadcrumb-item";

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

    public override string Skeleton() => null;

    public override string SkeletonAnimation( SkeletonAnimation animation ) => animation != Blazorise.SkeletonAnimation.Default ? $"placeholder-{ToSkeletonAnimation( animation )}" : null;

    public override string SkeletonItem() => "placeholder";

    #endregion

    #region Divider

    public override string Divider() => "divider";

    public override string DividerType( DividerType dividerType ) => $"{Divider()}-{ToDividerType( dividerType )}";

    #endregion

    #region Link

    public override string Link() => null;

    public override string LinkActive( bool active ) => active ? Active() : null;

    public override string LinkUnstyled( bool unstyled ) => unstyled ? "link-unstyled" : null;

    public override string LinkStretched( bool stretched ) => stretched ? "stretched-link" : null;

    public override string LinkDisabled( bool disabled ) => disabled ? "link-disabled" : null;

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

    #region Gap

    public override string Gap( GapSize gapSize, GapSide gapSide )
    {
        var side = gapSide != GapSide.None && gapSide != GapSide.All
            ? $"{ToGapSide( gapSide )}-"
            : null;

        return $"gap-{side}{ToGapSize( gapSize )}";
    }

    public override string Gap( GapSize gapSize, IEnumerable<GapSide> rules )
        => string.Join( " ", rules.Select( x => Gap( gapSize, x ) ) );

    #endregion

    #region Gutter

    public override string Gutter( GutterSize gutterSize, GutterSide gutterSide, Breakpoint breakpoint )
    {
        var sb = new StringBuilder( "g" );

        if ( gutterSide != GutterSide.None && gutterSide != GutterSide.All )
            sb.Append( ToGutterSide( gutterSide ) );

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
        var sb = new StringBuilder( "border" );

        if ( borderDefinition.Side != BorderSide.All )
            sb.Append( '-' ).Append( ToBorderSide( borderDefinition.Side ) );

        if ( borderSize != BorderSize.Default )
            sb.Append( '-' ).Append( ToBorderSize( borderSize ) );

        if ( borderDefinition.Color != BorderColor.None )
        {
            sb.Append( " border-" ).Append( ToBorderColor( borderDefinition.Color ) );

            if ( borderDefinition.Subtle )
                sb.Append( "-subtle" );
        }

        return sb.ToString();
    }

    public override string Border( BorderSize borderSize, IEnumerable<BorderDefinition> rules )
        => string.Join( " ", rules.Select( x => Border( borderSize, x ) ) );

    #endregion

    #region Flex

    public override string Flex( FlexType flexType )
    {
        return flexType != FlexType.Default
            ? $"d-{ToFlexType( flexType )}"
            : null;
    }

    public override string Flex( FlexDefinition flexDefinition )
    {
        var sb = new StringBuilder();

        var breakpoint = flexDefinition.Breakpoint != Breakpoint.None && flexDefinition.Breakpoint != Breakpoint.Mobile
            ? $"{ToBreakpoint( flexDefinition.Breakpoint )}-"
            : null;

        if ( flexDefinition.Direction != FlexDirection.Default )
            sb.Append( "flex-" ).Append( breakpoint ).Append( ToDirection( flexDefinition.Direction ) );

        if ( flexDefinition.JustifyContent != FlexJustifyContent.Default )
            sb.Append( "justify-content-" ).Append( breakpoint ).Append( ToJustifyContent( flexDefinition.JustifyContent ) );

        if ( flexDefinition.AlignItems != FlexAlignItems.Default )
            sb.Append( "align-items-" ).Append( breakpoint ).Append( ToAlignItems( flexDefinition.AlignItems ) );

        if ( flexDefinition.AlignSelf != FlexAlignSelf.Default )
            sb.Append( "align-self-" ).Append( breakpoint ).Append( ToAlignSelf( flexDefinition.AlignSelf ) );

        if ( flexDefinition.AlignContent != FlexAlignContent.Default )
            sb.Append( "align-content-" ).Append( breakpoint ).Append( ToAlignContent( flexDefinition.AlignContent ) );

        if ( flexDefinition.GrowShrink != FlexGrowShrink.Default && flexDefinition.GrowShrinkSize != FlexGrowShrinkSize.Default )
            sb.Append( "flex-" ).Append( breakpoint ).Append( ToGrowShrink( flexDefinition.GrowShrink ) ).Append( "-" ).Append( ToGrowShrinkSize( flexDefinition.GrowShrinkSize ) );

        if ( flexDefinition.Basis && flexDefinition.BasisSize != FlexBasisSize.Default )
            sb.Append( "flex-basis-" ).Append( breakpoint ).Append( ToBasisSize( flexDefinition.BasisSize ) );

        if ( flexDefinition.Wrap != FlexWrap.Default )
            sb.Append( "flex-" ).Append( breakpoint ).Append( ToWrap( flexDefinition.Wrap ) );

        if ( flexDefinition.Order != FlexOrder.Default )
            sb.Append( "order-" ).Append( breakpoint ).Append( ToOrder( flexDefinition.Order ) );

        if ( flexDefinition.Fill )
            sb.Append( "flex-" ).Append( breakpoint ).Append( "fill" );

        return sb.ToString();
    }

    public override string Flex( FlexRule flexRule )
    {
        var sb = new StringBuilder();

        if ( flexRule.FlexType != FlexType.Default )
        {
            if ( flexRule.Breakpoint > Breakpoint.Mobile )
            {
                sb.Append( $"d-{ToBreakpoint( flexRule.Breakpoint )}-{ToFlexType( flexRule.FlexType )}" );
            }
            else
            {
                sb.Append( $"d-{ToFlexType( flexRule.FlexType )}" );
            }

            sb.Append( ' ' );
        }

        sb.Append( string.Join( ' ', flexRule.Definitions.Where( x => x.Condition ?? true ).Select( x => Flex( x ) ) ) );

        return sb.ToString();
    }

    #endregion

    #region Sizing

    public override string Sizing( SizingType sizingType, SizingSize sizingSize, SizingDefinition sizingDefinition )
    {
        var sb = new StringBuilder();

        if ( sizingDefinition.IsMin && sizingDefinition.IsViewport )
            sb.Append( "min-v" );
        else if ( sizingDefinition.IsMax )
            sb.Append( "m" );
        else if ( sizingDefinition.IsViewport )
            sb.Append( "v" );

        sb.Append( sizingType == SizingType.Width
            ? "w"
            : "h" );

        if ( sizingDefinition.Breakpoint != Breakpoint.None && sizingDefinition.Breakpoint != Breakpoint.Mobile )
            sb.Append( $"-{ToBreakpoint( sizingDefinition.Breakpoint )}" );

        sb.Append( $"-{ToSizingSize( sizingSize )}" );

        return sb.ToString();
    }

    public override string Sizing( SizingType sizingType, SizingSize sizingSize, IEnumerable<SizingDefinition> rules )
        => string.Join( " ", rules.Select( x => Sizing( sizingType, sizingSize, x ) ) );

    #endregion

    #region Visibility

    public override string Visibility( Visibility visibility )
    {
        return visibility switch
        {
            Blazorise.Visibility.Visible => "visible",
            Blazorise.Visibility.Invisible => "invisible",
            _ => null,
        };
    }

    #endregion

    #region VerticalAlignment

    public override string VerticalAlignment( VerticalAlignment verticalAlignment )
        => $"align-{ToVerticalAlignment( verticalAlignment )}";

    #endregion

    #region Shadow

    public override string Shadow( Shadow shadow )
    {
        if ( shadow == Blazorise.Shadow.Default )
            return "shadow";

        return $"shadow-{ToShadow( shadow )}";
    }

    #endregion

    #region Overflow

    public override string Overflow( OverflowType overflowType, OverflowType secondOverflowType ) => secondOverflowType != OverflowType.Default
        ? $"overflow-{ToOverflowType( overflowType )}-{ToOverflowType( secondOverflowType )}"
        : $"overflow-{ToOverflowType( overflowType )}";

    #endregion

    #region Position

    public override string Position( PositionType positionType, PositionEdgeType edgeType, int edgeOffset, PositionTranslateType translateType )
    {
        return $"{ToPositionEdgeType( edgeType )}-{edgeOffset}";
    }

    public override string Position( PositionType positionType, IEnumerable<(PositionEdgeType edgeType, int edgeOffset)> edges, PositionTranslateType translateType )
    {
        var sb = new StringBuilder( $"position-{ToPositionType( positionType )}" );

        if ( edges != null && edges.Count() > 0 )
            sb.Append( ' ' ).Append( string.Join( " ", edges.Select( x => Position( positionType, x.edgeType, x.edgeOffset, translateType ) ) ) );

        if ( translateType != PositionTranslateType.None )
            sb.Append( " translate-" ).Append( ToPositionTranslateType( translateType ) );

        return sb.ToString();
    }

    #endregion

    #region ObjectFit

    public override string ObjectFit( ObjectFitType objectFitType, ObjectFitDefinition objectFitDefinition )
    {
        if ( objectFitType == ObjectFitType.Default )
            return null;

        if ( objectFitDefinition.Breakpoint != Breakpoint.None && objectFitDefinition.Breakpoint != Breakpoint.Mobile )
            return $"object-fit-{ToBreakpoint( objectFitDefinition.Breakpoint )}-{ToObjectFitType( objectFitType )}";

        return $"object-fit-{ToObjectFitType( objectFitType )}";
    }

    #endregion

    #region Elements

    public override string UnorderedList() => "unordered-list";

    public override string UnorderedListUnstyled( bool unstyled ) => unstyled ? "list-unstyled" : null;

    public override string OrderedList() => "ordered-list";

    public override string OrderedListUnstyled( bool unstyled ) => unstyled ? "list-unstyled" : null;

    public override string OrderedListType( OrderedListType orderedListType ) => orderedListType != Blazorise.OrderedListType.Default ? $"ordered-list-{ToOrderedListType( orderedListType )}" : null;

    public override string DescriptionList() => null;

    public override string DescriptionListTerm() => null;

    public override string DescriptionListDefinition() => null;

    #endregion

    #region Enums

    public override string ToFloat( Float @float )
    {
        return @float switch
        {
            Blazorise.Float.Start => "start",
            Blazorise.Float.End => "end",
            _ => null,
        };
    }

    public override string ToBorderRadius( BorderRadius borderRadius )
    {
        return borderRadius switch
        {
            Blazorise.BorderRadius.Rounded => "rounded",
            Blazorise.BorderRadius.RoundedTop => "rounded-top",
            Blazorise.BorderRadius.RoundedEnd => "rounded-end",
            Blazorise.BorderRadius.RoundedBottom => "rounded-bottom",
            Blazorise.BorderRadius.RoundedStart => "rounded-start",
            Blazorise.BorderRadius.RoundedCircle => "rounded-circle",
            Blazorise.BorderRadius.RoundedPill => "rounded-pill",
            Blazorise.BorderRadius.RoundedZero => "rounded-0",
            _ => null,
        };
    }

    public override string ToTextAlignment( TextAlignment textAlignment )
    {
        return textAlignment switch
        {
            Blazorise.TextAlignment.Start => "start",
            Blazorise.TextAlignment.Center => "center",
            Blazorise.TextAlignment.End => "end",
            Blazorise.TextAlignment.Justified => "justify",
            _ => null,
        };
    }

    public override string ToPlacement( Placement placement )
    {
        return placement switch
        {
            Blazorise.Placement.Bottom => "bottom",
            Blazorise.Placement.Start => "start",
            Blazorise.Placement.End => "end",
            _ => "top",
        };
    }

    public override string ToBorderSide( BorderSide borderSide )
    {
        return borderSide switch
        {
            Blazorise.BorderSide.Bottom => "bottom",
            Blazorise.BorderSide.Start => "start",
            Blazorise.BorderSide.End => "end",
            _ => "top",
        };
    }

    public override string ToSide( Side side )
    {
        return side switch
        {
            Blazorise.Side.Top => "t",
            Blazorise.Side.Bottom => "b",
            Blazorise.Side.Start => "s",
            Blazorise.Side.End => "e",
            Blazorise.Side.X => "x",
            Blazorise.Side.Y => "y",
            _ => null,
        };
    }

    public override string ToPositionEdgeType( PositionEdgeType positionEdgeType )
    {
        return positionEdgeType switch
        {
            Blazorise.PositionEdgeType.Top => "top",
            Blazorise.PositionEdgeType.Start => "start",
            Blazorise.PositionEdgeType.Bottom => "bottom",
            Blazorise.PositionEdgeType.End => "end",
            _ => null,
        };
    }

    public override string ToScreenreader( Screenreader screenreader )
    {
        return screenreader switch
        {
            Blazorise.Screenreader.Only => "visually-hidden",
            Blazorise.Screenreader.OnlyFocusable => "visually-hidden-focusable",
            _ => null,
        };
    }

    public override string ToSkeletonAnimation( SkeletonAnimation animation )
    {
        return animation switch
        {
            Blazorise.SkeletonAnimation.Wave => "wave",
            Blazorise.SkeletonAnimation.Pulse => "glow",
            _ => null
        };
    }

    #endregion

    public override bool UseCustomInputStyles { get; set; } = true;

    public override string Provider => "Bootstrap5";
}