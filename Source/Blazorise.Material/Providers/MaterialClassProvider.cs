using System.Collections.Generic;
using System.Linq;
using System.Text;
using Blazorise.Extensions;

namespace Blazorise.Material.Providers;

public class MaterialClassProvider : ClassProvider
{
    #region TextInput

    public override string TextInput( bool plaintext ) => plaintext ? "mui-input-plaintext" : "mui-input";

    public override string TextInputSize( Size size ) => size != Size.Default ? $"mui-input-{ToSize( size )}" : null;

    public override string TextInputColor( Color color ) => color.IsNotNullOrDefault() ? $"text-{ToColor( color )}" : null;

    public override string TextInputValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

    #endregion

    #region MemoInput

    public override string MemoInput( bool plaintext ) => plaintext ? "mui-input-plaintext" : "mui-input";

    public override string MemoInputSize( Size size ) => size != Size.Default ? $"mui-input-{ToSize( size )}" : null;

    public override string MemoInputValidation( ValidationStatus validationStatus ) => validationStatus != ValidationStatus.None ? ToValidationStatus( validationStatus ) : null;

    #endregion

    #region Select

    public override string Select() => "mui-select";

    public override string SelectMultiple( bool multiple ) => null;

    public override string SelectSize( Size size ) => size != Size.Default ? $"mui-select-{ToSize( size )}" : null;

    public override string SelectValidation( ValidationStatus validationStatus ) => validationStatus != ValidationStatus.None ? ToValidationStatus( validationStatus ) : null;

    #endregion

    #region NumericInput

    public override string NumericInput( bool plaintext ) => plaintext ? "mui-input-plaintext" : "mui-input";

    public override string NumericInputSize( Size size ) => size != Size.Default ? $"mui-input-{ToSize( size )}" : null;

    public override string NumericInputColor( Color color ) => color.IsNotNullOrDefault() ? $"text-{ToColor( color )}" : null;

    public override string NumericInputValidation( ValidationStatus validationStatus ) => validationStatus != ValidationStatus.None ? ToValidationStatus( validationStatus ) : null;

    #endregion

    #region DateInput

    public override string DateInput( bool plaintext ) => plaintext ? "mui-input-plaintext" : "mui-input";

    public override string DateInputSize( Size size ) => size != Size.Default ? $"mui-input-{ToSize( size )}" : null;

    public override string DateInputColor( Color color ) => color.IsNotNullOrDefault() ? $"text-{ToColor( color )}" : null;

    public override string DateInputValidation( ValidationStatus validationStatus ) => validationStatus != ValidationStatus.None ? ToValidationStatus( validationStatus ) : null;

    #endregion

    #region TimeInput

    public override string TimeInput( bool plaintext ) => plaintext ? "mui-input-plaintext" : "mui-input";

    public override string TimeInputSize( Size size ) => size != Size.Default ? $"mui-input-{ToSize( size )}" : null;

    public override string TimeInputColor( Color color ) => color.IsNotNullOrDefault() ? $"text-{ToColor( color )}" : null;

    public override string TimeInputValidation( ValidationStatus validationStatus ) => validationStatus != ValidationStatus.None ? ToValidationStatus( validationStatus ) : null;

    #endregion

    #region ColorInput

    public override string ColorInput() => "form-control";

    public override string ColorInputSize( Size size ) => size != Size.Default ? $"mui-input-{ToSize( size )}" : null;

    #endregion

    #region DatePicker

    public override string DatePicker( bool plaintext ) => plaintext ? "mui-input-plaintext" : "mui-input";

    public override string DatePickerSize( Size size ) => size != Size.Default ? $"mui-input-{ToSize( size )}" : null;

    public override string DatePickerColor( Color color ) => color.IsNotNullOrDefault() ? $"text-{ToColor( color )}" : null;

    public override string DatePickerValidation( ValidationStatus validationStatus ) => validationStatus != ValidationStatus.None ? ToValidationStatus( validationStatus ) : null;

    #endregion

    #region TimePicker

    public override string TimePicker( bool plaintext ) => plaintext ? "mui-input-plaintext" : "mui-input";

    public override string TimePickerSize( Size size ) => size != Size.Default ? $"mui-input-{ToSize( size )}" : null;

    public override string TimePickerColor( Color color ) => color.IsNotNullOrDefault() ? $"text-{ToColor( color )}" : null;

    public override string TimePickerValidation( ValidationStatus validationStatus ) => validationStatus != ValidationStatus.None ? ToValidationStatus( validationStatus ) : null;

    #endregion

    #region ColorPicker

    public override string ColorPicker() => "form-control b-input-color-picker";

    public override string ColorPickerSize( Size size ) => size != Size.Default ? $"mui-input-{ToSize( size )}" : null;

    #endregion

    #region NumericPicker

    public override string NumericPicker( bool plaintext ) => plaintext ? "mui-input-plaintext" : "mui-input";

    public override string NumericPickerSize( Size size ) => size != Size.Default ? $"mui-input-{ToSize( size )}" : null;

    public override string NumericPickerColor( Color color ) => color.IsNotNullOrDefault() ? $"text-{ToColor( color )}" : null;

    public override string NumericPickerValidation( ValidationStatus validationStatus ) => validationStatus != ValidationStatus.None ? ToValidationStatus( validationStatus ) : null;

    #endregion

    #region InputMask

    public override string InputMask( bool plaintext ) => plaintext ? "mui-input-plaintext" : "mui-input";

    public override string InputMaskSize( Size size ) => size != Size.Default ? $"mui-input-{ToSize( size )}" : null;

    public override string InputMaskColor( Color color ) => color.IsNotNullOrDefault() ? $"text-{ToColor( color )}" : null;

    public override string InputMaskValidation( ValidationStatus validationStatus ) => validationStatus != ValidationStatus.None ? ToValidationStatus( validationStatus ) : null;

    #endregion

    #region Check

    public override string Check() => "mui-check";

    public override string CheckSize( Size size ) => size != Size.Default ? $"mui-check-{ToSize( size )}" : null;

    public override string CheckInline( bool inline ) => inline ? "mui-check-inline" : null;

    public override string CheckCursor( Cursor cursor ) => cursor != Cursor.Default ? $"mui-check-{ToCursor( cursor )}" : null;

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

    public override string Radio( bool button ) => button ? "mui-radio-button" : "mui-radio";

    public override string RadioSize( bool button, Size size ) => size != Size.Default ? $"mui-radio-{ToSize( size )}" : null;

    public override string RadioInline( bool inline ) => inline ? "mui-radio-inline" : null;

    public override string RadioCursor( Cursor cursor ) => cursor != Cursor.Default ? $"mui-radio-{ToCursor( cursor )}" : null;

    public override string RadioValidation( ValidationStatus validationStatus ) => validationStatus != ValidationStatus.None ? ToValidationStatus( validationStatus ) : null;

    #endregion

    #region Switch

    public override string Switch() => "mui-switch";

    public override string SwitchColor( Color color ) => color.IsNotNullOrDefault() ? $"mui-switch-{ToColor( color )}" : null;

    public override string SwitchSize( Size size ) => size != Size.Default ? $"mui-switch-{ToSize( size )}" : null;

    public override string SwitchChecked( bool @checked ) => null;

    public override string SwitchCursor( Cursor cursor ) => cursor != Cursor.Default ? $"mui-switch-{ToCursor( cursor )}" : null;

    public override string SwitchValidation( ValidationStatus validationStatus ) => validationStatus != ValidationStatus.None ? ToValidationStatus( validationStatus ) : null;

    #endregion

    #region FileInput

    public override string FileInput() => "mui-input";

    public override string FileInputSize( Size size ) => size != Size.Default ? $"mui-input-{ToSize( size )}" : null;

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
            Blazorise.LabelType.Check => "mui-check-label",
            Blazorise.LabelType.Radio => "mui-radio-label",
            Blazorise.LabelType.Switch => "mui-switch-label",
            Blazorise.LabelType.File => "mui-file-label",
            _ => "mui-label",
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

    public override string Fields() => "mui-row";

    public override string FieldsBody() => null;

    public override string FieldsColumn() => "mui-column";

    #endregion

    #region Field

    public override string Field() => "mui-field";

    public override string FieldHorizontal( bool horizontal ) => horizontal ? "mui-field-horizontal" : null;

    public override string FieldColumn() => "mui-column";

    public override string FieldSize( Size size ) => null;

    public override string FieldJustifyContent( JustifyContent justifyContent ) => justifyContent != JustifyContent.Default ? $"mui-{ToJustifyContent( justifyContent )}" : null;

    public override string FieldValidation( ValidationStatus validationStatus ) => null;

    #endregion

    #region FieldLabel

    public override string FieldLabel( bool horizontal ) => horizontal ? "mui-field-label-horizontal" : "mui-field-label";

    public override string FieldLabelRequiredIndicator( bool requiredIndicator )
        => requiredIndicator
            ? "mui-field-required"
            : null;

    public override string FieldLabelScreenreader( Screenreader screenreader ) => screenreader != Screenreader.Always ? ToScreenreader( screenreader ) : null;

    #endregion

    #region FieldBody

    public override string FieldBody() => "mui-field-body";

    #endregion

    #region FieldHelp

    public override string FieldHelp() => "mui-field-helper";

    #endregion

    #region FocusTrap

    public override string FocusTrap() => "focus-trap";

    #endregion

    #region Control

    public override string ControlCheck( ControlRole role ) => null;

    public override string ControlRadio( ControlRole role ) => null;

    public override string ControlSwitch( ControlRole role ) => null;

    public override string ControlFile( ControlRole role ) => null;

    public override string ControlText( ControlRole role ) => null;

    public override string ControlInline( ControlRole role, bool inline ) => null;

    #endregion

    #region Addons

    public override string Addons() => "mui-addons";

    public override string AddonsSize( Size size ) => size != Size.Default ? $"mui-addons-{ToSize( size )}" : null;

    public override string AddonsHasButton( bool hasButton ) => hasButton ? "mui-addons-buttons" : null;

    public override string Addon( AddonType addonType )
    {

        return addonType switch
        {
            AddonType.Start => "mui-addon-start",
            AddonType.End => "mui-addon-end",
            _ => "mui-addon-body",
        };
    }

    public override string AddonSize( Size size ) => size != Size.Default ? $"mui-addon-{ToSize( size )}" : null;

    public override string AddonLabel() => "mui-addon-text";

    #endregion

    #region Inline

    public override string Inline() => "mui-fields-inline";

    #endregion

    #region Button

    public override string Button( bool outline ) => "mui-button";

    public override string ButtonColor( Color color, bool outline ) => outline
        ? color.IsNotNullOrDefault() ? $"{Button( outline )}-outline-{ToColor( color )}" : $"{Button( outline )}-outline"
        : color.IsNotNullOrDefault() ? $"{Button( outline )}-{ToColor( color )}" : null;

    public override string ButtonSize( Size size, bool outline ) => size == Size.Default ? null : $"mui-button-{ToSize( size )}";

    public override string ButtonBlock( bool outline, bool block ) => block ? $"mui-button-block" : null;

    public override string ButtonActive( bool outline, bool active ) => active ? "mui-button-active" : null;

    public override string ButtonDisabled( bool outline, bool disabled ) => disabled ? "mui-button-disabled" : null;

    public override string ButtonLoading( bool outline, bool loading ) => null;

    public override string ButtonStretchedLink( bool stretched ) => stretched ? "stretched-link" : null;

    #endregion

    #region Buttons

    public override string Buttons( ButtonsRole role, Orientation orientation )
    {
        if ( role == ButtonsRole.Toolbar )
            return "mui-buttons-toolbar";

        if ( orientation == Orientation.Vertical )
            return "mui-button-group-vertical";

        return "mui-button-group";
    }

    public override string ButtonsSize( Size size ) => size != Size.Default ? $"mui-button-group-{ToSize( size )}" : null;

    #endregion

    #region CloseButton

    public override string CloseButton() => "mui-button-close";

    #endregion

    #region Dropdown

    public override string Dropdown( bool isDropdownSubmenu ) => "mui-dropdown";

    public override string DropdownDisabled( bool disabled ) => disabled ? "mui-dropdown-disabled" : null;

    public override string DropdownGroup( bool group ) => group ? "mui-button-group" : null;

    public override string DropdownObserverShow() => "mui-dropdown-active";

    public override string DropdownShow( bool show ) => show ? "mui-dropdown-active" : null;

    public override string DropdownRight( bool rightAligned ) => null;

    public override string DropdownItem() => "mui-dropdown-item";

    public override string DropdownItemActive( bool active ) => active ? Active() : null;

    public override string DropdownItemDisabled( bool disabled ) => disabled ? Disabled() : null;

    public override string DropdownDivider() => "mui-dropdown-divider";

    public override string DropdownHeader() => "mui-dropdown-header";

    public override string DropdownMenu() => "mui-dropdown-menu";

    public override string DropdownMenuPositionStrategy( DropdownPositionStrategy dropdownPositionStrategy )
        => $"mui-dropdown-menu-position-strategy {( dropdownPositionStrategy == DropdownPositionStrategy.Fixed ? "mui-dropdown-menu-position-strategy-fixed" : "mui-dropdown-menu-position-strategy-absolute" )}";

    public override string DropdownFixedHeaderVisible( bool visible )
        => visible ? "mui-dropdown-table-fixed-header-visible" : null;

    public override string DropdownMenuSelector() => "mui-dropdown-menu";

    public override string DropdownMenuScrollable( bool scrollable ) => scrollable ? "mui-dropdown-menu-scrollable" : null;

    public override string DropdownMenuVisible( bool visible ) => visible ? "mui-dropdown-menu-active" : null;

    public override string DropdownMenuEnd( bool endAligned ) => endAligned ? "mui-dropdown-menu-end" : null;

    public override string DropdownToggle( bool isDropdownSubmenu, bool outline ) => isDropdownSubmenu ? "mui-dropdown-item mui-dropdown-toggle" : "mui-button mui-dropdown-toggle";

    public override string DropdownToggleSelector( bool isDropdownSubmenu ) => isDropdownSubmenu ? "mui-dropdown-item mui-dropdown-toggle" : "mui-button mui-dropdown-toggle";

    public override string DropdownToggleColor( Color color, bool outline ) => outline
        ? color.IsNotNullOrDefault() ? $"mui-button-outline-{ToColor( color )}" : $"mui-button-outline"
        : color.IsNotNullOrDefault() ? $"mui-button-{ToColor( color )}" : null;

    public override string DropdownToggleSize( Size size, bool outline )
        => size != Size.Default ? $"mui-button-{ToSize( size )}" : null;

    public override string DropdownToggleSplit( bool split ) => split ? "mui-dropdown-toggle-split" : null;

    public override string DropdownToggleIcon( bool visible ) => visible ? null : "mui-dropdown-toggle-hidden";

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

    public override string Tabs( bool pills ) => pills ? "mui-tabs mui-tabs-pills" : "mui-tabs";

    public override string TabsCards( bool cards ) => cards ? "mui-tabs-cards" : null;

    public override string TabsFullWidth( bool fullWidth ) => fullWidth ? "mui-tabs-fullwidth" : null;

    public override string TabsJustified( bool justified ) => justified ? "mui-tabs-justified" : null;

    public override string TabsVertical( bool vertical ) => vertical ? "mui-tabs-vertical" : null;

    public override string TabItem( TabPosition tabPosition )
    {
        return tabPosition switch
        {
            TabPosition.Start => "mui-tab-item mui-tab-item-start",
            TabPosition.End => "mui-tab-item mui-tab-item-end",
            TabPosition.Bottom => "mui-tab-item mui-tab-item-bottom",
            _ => "mui-tab-item mui-tab-item-top",
        };
    }

    public override string TabItemActive( bool active ) => active ? "mui-tab-item-active" : null;

    public override string TabItemDisabled( bool disabled ) => disabled ? "mui-tab-item-disabled" : null;

    public override string TabLink( TabPosition tabPosition ) => "mui-tab-link";

    public override string TabLinkActive( bool active ) => active ? "mui-tab-link-active" : null;

    public override string TabLinkDisabled( bool disabled ) => disabled ? "mui-tab-link-disabled" : null;

    public override string TabsContent() => "mui-tabs-content";

    public override string TabPanel() => "mui-tab-panel";

    public override string TabPanelActive( bool active ) => active ? "mui-tab-panel-active" : null;

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

    public override string CardDeck() => "mui-card-deck";

    public override string CardGroup() => "mui-card-group";

    public override string Card() => "mui-card";

    public override string CardWhiteText( bool whiteText ) => whiteText ? "text-white" : null;

    public override string CardActions() => "mui-card-actions";

    public override string CardBody() => "mui-card-body";

    public override string CardFooter() => "mui-card-footer";

    public override string CardHeader() => "mui-card-header";

    public override string CardImage() => "mui-card-img-top";

    public override string CardTitle( bool insideHeader ) => "mui-card-title";

    public override string CardTitleSize( bool insideHeader, HeadingSize? size ) => null;

    public override string CardSubtitle( bool insideHeader ) => "mui-card-subtitle";

    public override string CardSubtitleSize( bool insideHeader, HeadingSize? size ) => null;

    public override string CardText() => "mui-card-text";

    public override string CardLink() => "mui-card-link";

    public override string CardLinkUnstyled( bool unstyled ) => unstyled ? "link-unstyled" : null;

    public override string CardLinkActive( bool active ) => LinkActive( active );

    #endregion

    #region ListGroup

    public override string ListGroup() => "mui-list";

    public override string ListGroupFlush( bool flush ) => flush ? "mui-list-flush" : null;

    public override string ListGroupScrollable( bool scrollable ) => scrollable ? "mui-list-scrollable" : null;

    public override string ListGroupItem() => "mui-list-item";

    public override string ListGroupItemSelectable( bool selectable ) => selectable ? "mui-list-item-action" : null;

    public override string ListGroupItemActive( bool active ) => active ? Active() : null;

    public override string ListGroupItemDisabled( bool disabled ) => disabled ? Disabled() : null;

    public override string ListGroupItemColor( Color color, bool selectable, bool active )
        => color.IsNotNullOrDefault() ? $"{ListGroupItem()}-{ToColor( color )}" : null;

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

    public override string BarDropdownToggleIcon( bool isToggleIconVisible ) => isToggleIconVisible ? null : "b-bar-dropdown-toggle-hidden";

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

    public override string Collapse() => "mui-collapse";

    public override string CollapseActive( bool active ) => active ? "active" : null;

    public override string CollapseHeader() => "mui-collapse-header";

    public override string CollapseBody() => "mui-collapse-body";

    public override string CollapseBodyActive( bool active ) => active ? "active" : null;

    public override string CollapseBodyContent() => "mui-collapse-body-content";

    #endregion

    #region Row

    public override string Row() => "mui-row";

    public override string RowColumns( RowColumnsSize rowColumnsSize, RowColumnsDefinition rowColumnsDefinition )
    {
        if ( rowColumnsDefinition.Breakpoint != Breakpoint.None && rowColumnsDefinition.Breakpoint != Breakpoint.Mobile )
            return $"mui-row-columns-{ToBreakpoint( rowColumnsDefinition.Breakpoint )}-{ToRowColumnsSize( rowColumnsSize )}";

        return $"mui-row-columns-{ToRowColumnsSize( rowColumnsSize )}";
    }

    #endregion

    #region Column

    public override string Column( bool grid, bool hasSizes ) => hasSizes ? null : grid ? "mui-grid-column" : "mui-column";

    public override string Column( bool grid, ColumnWidth columnWidth, Breakpoint breakpoint, bool offset )
    {
        var baseClass = offset
            ? grid ? "mui-grid-start" : "mui-column-offset"
            : grid ? "mui-grid-column" : "mui-column";

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

    public override string Grid() => "mui-grid";

    public override string GridRows( GridRowsSize gridRows, GridRowsDefinition gridRowsDefinition )
    {
        if ( gridRowsDefinition.Breakpoint != Breakpoint.None && gridRowsDefinition.Breakpoint != Breakpoint.Mobile )
            return $"mui-grid-rows-{ToBreakpoint( gridRowsDefinition.Breakpoint )}-{ToGridRowsSize( gridRows )}";

        return $"mui-grid-rows-{ToGridRowsSize( gridRows )}";
    }

    public override string GridColumns( GridColumnsSize gridColumns, GridColumnsDefinition gridColumnsDefinition )
    {
        if ( gridColumnsDefinition.Breakpoint != Breakpoint.None && gridColumnsDefinition.Breakpoint != Breakpoint.Mobile )
            return $"mui-grid-columns-{ToBreakpoint( gridColumnsDefinition.Breakpoint )}-{ToGridColumnsSize( gridColumns )}";

        return $"mui-grid-columns-{ToGridColumnsSize( gridColumns )}";
    }

    #endregion

    #region Display

    public override string Display( DisplayType displayType, DisplayDefinition displayDefinition )
    {
        var baseClass = displayDefinition.Breakpoint != Breakpoint.None && displayDefinition.Breakpoint != Blazorise.Breakpoint.Mobile
            ? $"mui-display-{ToBreakpoint( displayDefinition.Breakpoint )}-{ToDisplayType( displayType )}"
            : $"mui-display-{ToDisplayType( displayType )}";

        if ( displayDefinition.Direction != DisplayDirection.Default )
            return $"{baseClass} mui-flex-{ToDisplayDirection( displayDefinition.Direction )}";

        return baseClass;
    }

    #endregion

    #region Alert

    public override string Alert() => "mui-alert";

    public override string AlertColor( Color color ) => color.IsNotNullOrDefault() ? $"{Alert()}-{ToColor( color )}" : null;

    public override string AlertDismisable( bool dismissable ) => dismissable ? "mui-alert-closable" : null;

    public override string AlertFade( bool dismissable ) => dismissable ? Fade() : null;

    public override string AlertShow( bool dismissable, bool visible ) => dismissable && visible ? Show() : null;

    public override string AlertHasMessage( bool hasMessage ) => null;

    public override string AlertHasDescription( bool hasDescription ) => null;

    public override string AlertMessage() => null;

    public override string AlertDescription() => null;

    #endregion

    #region Modal

    public override string Modal() => "mui-modal";

    public override string ModalFade( bool showing, bool hiding ) => showing
        ? "mui-modal-showing"
        : hiding
            ? "mui-modal-hiding"
            : null;

    public override string ModalVisible( bool visible ) => visible ? "mui-modal-visible" : null;

    public override string ModalSize( ModalSize modalSize ) => modalSize == Blazorise.ModalSize.Fullscreen ? "mui-modal-fullscreen" : null;

    public override string ModalCentered( bool centered ) => centered ? "mui-modal-centered" : null;

    public override string ModalBackdrop() => "mui-modal-backdrop";

    public override string ModalBackdropFade() => "mui-modal-backdrop-fade";

    public override string ModalBackdropVisible( bool visible ) => visible ? "mui-modal-backdrop-visible" : null;

    public override string ModalContent( bool dialog ) => "mui-modal-content";

    public override string ModalContentSize( ModalSize modalSize )
    {
        return modalSize switch
        {
            Blazorise.ModalSize.Small => "mui-modal-content-sm",
            Blazorise.ModalSize.Large => "mui-modal-content-lg",
            Blazorise.ModalSize.ExtraLarge => "mui-modal-content-xl",
            Blazorise.ModalSize.Fullscreen => "mui-modal-content-fullscreen",
            _ => null,
        };
    }

    public override string ModalContentCentered( bool centered ) => centered ? "mui-modal-content-centered" : null;

    public override string ModalContentScrollable( bool scrollable ) => scrollable ? "mui-modal-content-scrollable" : null;

    public override string ModalBody() => "mui-modal-body";

    public override string ModalHeader() => "mui-modal-header";

    public override string ModalFooter() => "mui-modal-footer";

    public override string ModalTitle() => "mui-modal-title";

    #endregion

    #region Offcanvas

    public override string Offcanvas() => "mui-offcanvas";

    public override string OffcanvasPlacement( Placement placement, bool visible )
    {
        return placement switch
        {
            Placement.Start => "mui-offcanvas-start",
            Placement.End => "mui-offcanvas-end",
            Placement.Top => "mui-offcanvas-top",
            Placement.Bottom => "mui-offcanvas-bottom",
            _ => null,
        };
    }

    public override string OffcanvasFade( bool showing, bool hiding ) => showing
        ? "mui-offcanvas-showing"
        : hiding
            ? "mui-offcanvas-hiding"
            : null;

    public override string OffcanvasVisible( bool visible ) => visible ? "mui-offcanvas-visible" : null;

    public override string OffcanvasHeader() => "mui-offcanvas-header";

    public override string OffcanvasFooter() => "mui-offcanvas-footer";

    public override string OffcanvasBody() => "mui-offcanvas-body";

    public override string OffcanvasBackdrop() => "mui-offcanvas-backdrop";

    public override string OffcanvasBackdropFade( bool showing, bool hiding ) => showing
        ? "mui-offcanvas-backdrop-showing"
        : hiding
            ? "mui-offcanvas-backdrop-hiding"
            : null;

    public override string OffcanvasBackdropVisible( bool visible ) => visible ? "mui-offcanvas-backdrop-visible" : null;

    #endregion

    #region Toast

    public override string Toast() => "mui-toast";

    public override string ToastAnimated( bool animated ) => animated ? "mui-toast-animated" : null;

    public override string ToastFade( bool visible, bool showing, bool hiding ) => showing
        ? "mui-toast-showing"
        : hiding
            ? "mui-toast-hiding"
            : null;

    public override string ToastVisible( bool visible ) => visible ? "mui-toast-visible" : null;

    public override string ToastHeader() => "mui-toast-header";

    public override string ToastBody() => "mui-toast-body";

    public override string Toaster() => "mui-toaster";

    public override string ToasterPlacement( ToasterPlacement placement ) => placement switch
    {
        Blazorise.ToasterPlacement.Top => "mui-toaster-top",
        Blazorise.ToasterPlacement.TopStart => "mui-toaster-top-start",
        Blazorise.ToasterPlacement.TopEnd => "mui-toaster-top-end",
        Blazorise.ToasterPlacement.Bottom => "mui-toaster-bottom",
        Blazorise.ToasterPlacement.BottomStart => "mui-toaster-bottom-start",
        Blazorise.ToasterPlacement.BottomEnd => "mui-toaster-bottom-end",
        _ => null,
    };

    public override string ToasterPlacementStrategy( ToasterPlacementStrategy placementStrategy ) => placementStrategy switch
    {
        Blazorise.ToasterPlacementStrategy.Fixed => "mui-toaster-fixed",
        Blazorise.ToasterPlacementStrategy.Absolute => "mui-toaster-absolute",
        _ => null,
    };

    #endregion

    #region Pagination

    public override string Pagination() => "mui-pagination";

    public override string PaginationSize( Size size ) => size != Size.Default ? $"{Pagination()}-{ToSize( size )}" : null;

    public override string PaginationAlignment( Alignment alignment ) => alignment != Alignment.Default ? $"mui-justify-content-{ToAlignment( alignment )}" : null;

    public override string PaginationBackgroundColor( Background background ) => background.IsNotNullOrDefault() ? $"bg-{ToBackground( background )}" : null;

    public override string PaginationItem() => "mui-pagination-item";

    public override string PaginationItemActive( bool active ) => active ? "mui-pagination-item-active" : null;

    public override string PaginationItemDisabled( bool disabled ) => disabled ? "mui-pagination-item-disabled" : null;

    public override string PaginationLink() => "mui-pagination-link";

    public override string PaginationLinkSize( Size size ) => null;

    public override string PaginationLinkActive( bool active ) => active ? "mui-pagination-link-active" : null;

    public override string PaginationLinkDisabled( bool disabled ) => disabled ? "mui-pagination-link-disabled" : null;

    #endregion

    #region Progress

    public override string Progress() => "mui-progress";

    public override string ProgressSize( Size size ) => size != Size.Default ? $"mui-progress-{ToSize( size )}" : null;

    public override string ProgressColor( Color color ) => null;

    public override string ProgressStriped( bool stripped ) => stripped ? "mui-progress-striped" : null;

    public override string ProgressAnimated( bool animated ) => animated ? "mui-progress-animated" : null;

    public override string ProgressIndeterminate( bool indeterminate ) => indeterminate ? "mui-progress-indeterminate" : null;

    public override string ProgressWidth( int width ) => null;

    public override string ProgressBar() => "mui-progress-bar";

    public override string ProgressBarSize( Size size ) => size != Size.Default ? $"mui-progress-bar-{ToSize( size )}" : null;

    public override string ProgressBarColor( Color color ) => color.IsNotNullOrDefault() ? $"mui-progress-bar-{ToColor( color )}" : null;

    public override string ProgressBarStriped( bool striped ) => striped ? "mui-progress-bar-striped" : null;

    public override string ProgressBarAnimated( bool animated ) => animated ? "mui-progress-bar-animated" : null;

    public override string ProgressBarIndeterminate( bool indeterminate ) => indeterminate ? "mui-progress-bar-indeterminate" : null;

    public override string ProgressBarWidth( int width ) => null;

    #endregion

    #region Chart

    public override string Chart() => null;

    #endregion

    #region Colors

    public override string BackgroundColor( Background background ) => $"bg-{ToBackground( background )}";

    #endregion

    #region Table

    public override string Table() => "mui-table";

    public override string TableFullWidth( bool fullWidth ) => fullWidth ? "mui-table-fullwidth" : null;

    public override string TableStriped( bool striped ) => striped ? "mui-table-striped" : null;

    public override string TableHoverable( bool hoverable ) => hoverable ? "mui-table-hoverable" : null;

    public override string TableBordered( bool bordered ) => bordered ? "mui-table-bordered" : null;

    public override string TableNarrow( bool narrow ) => narrow ? "mui-table-narrow" : null;

    public override string TableBorderless( bool borderless ) => borderless ? "mui-table-borderless" : null;

    public override string TableHeader() => "mui-table-header";

    public override string TableHeaderThemeContrast( ThemeContrast themeContrast ) => themeContrast != ThemeContrast.None ? $"mui-table-header-theme mui-table-header-theme-{ToThemeContrast( themeContrast )}" : null;

    public override string TableHeaderCell() => "mui-table-header-cell";

    public override string TableHeaderCellCursor( Cursor cursor ) => cursor != Cursor.Default ? $"cursor-{ToCursor( cursor )}" : null;

    public override string TableHeaderCellFixed( TableColumnFixedPosition fixedPosition )
    {
        return fixedPosition switch
        {
            TableColumnFixedPosition.Start => "mui-table-header-cell-fixed-start",
            TableColumnFixedPosition.End => "mui-table-header-cell-fixed-end",
            _ => null,
        };
    }

    public override string TableFooter() => "mui-table-footer";

    public override string TableBody() => "mui-table-body";

    public override string TableRow( bool striped, bool hoverable ) => "mui-table-row";

    public override string TableRowColor( Color color ) => color.IsNotNullOrDefault() ? $"mui-table-row-{ToColor( color )}" : null;

    public override string TableRowHoverCursor( Cursor cursor ) => cursor != Cursor.Default ? "mui-table-row-selectable" : null;

    public override string TableRowIsSelected( bool selected ) => selected ? "mui-table-row-selected" : null;

    public override string TableRowHeader() => "mui-table-row-header";

    public override string TableRowHeaderFixed( TableColumnFixedPosition fixedPosition )
    {
        return fixedPosition switch
        {
            TableColumnFixedPosition.Start => "mui-table-row-header-fixed-start",
            TableColumnFixedPosition.End => "mui-table-row-header-fixed-end",
            _ => null,
        };
    }

    public override string TableRowCell() => "mui-table-row-cell";

    public override string TableRowCellColor( Color color ) => color.IsNotNullOrDefault() ? $"mui-table-cell-{ToColor( color )}" : null;

    public override string TableRowCellFixed( TableColumnFixedPosition fixedPosition )
    {
        return fixedPosition switch
        {
            TableColumnFixedPosition.Start => "mui-table-row-cell-fixed-start",
            TableColumnFixedPosition.End => "mui-table-row-cell-fixed-end",
            _ => null,
        };
    }

    public override string TableRowGroup( bool expanded ) => "mui-table-group";

    public override string TableRowGroupCell() => "mui-table-group-cell";

    public override string TableRowGroupIndentCell() => "mui-table-group-indentcell";

    public override string TableResponsive( bool responsive ) => responsive ? "mui-table-responsive" : null;

    public override string TableFixedHeader( bool fixedHeader ) => fixedHeader ? "mui-table-fixed-header" : null;

    public override string TableFixedColumns( bool fixedColumns ) => fixedColumns ? "mui-table-fixed-columns" : null;

    public override string TableResponsiveMode( TableResponsiveMode responsiveMode ) => responsiveMode == Blazorise.TableResponsiveMode.Mobile ? "mui-table-mobile" : null;

    public override string TableCaption() => "mui-table-caption";

    public override string TableCaptionSide( TableCaptionSide side ) => side != Blazorise.TableCaptionSide.Default ? $"mui-table-caption-{ToTableCaptionSide( side )}" : null;

    #endregion

    #region Badge

    public override string Badge() => "mui-chip";

    public override string BadgeColor( Color color, bool subtle ) => color.IsNotNullOrDefault() ? $"mui-chip-{ToColor( color )}" : null;

    public override string BadgePill( bool pill ) => pill ? "mui-chip-rounded" : null;

    public override string BadgeClose() => "mui-chip-close";

    public override string BadgeCloseColor( Color color, bool subtle ) => color.IsNotNullOrDefault() ? $"mui-chip-close-{ToColor( color )}" : null;

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
        var spacingString = ToSpacing( spacing );
        var sideString = side == Side.None || side == Side.All ? null : $"-{ToSide( side )}";
        var spacingSizeString = $"-{ToSpacingSize( spacingSize )}";

        if ( breakpoint != Blazorise.Breakpoint.None && breakpoint != Breakpoint.Mobile )
            return $"mui-{spacingString}{sideString}-{ToBreakpoint( breakpoint )}{spacingSizeString}";

        return $"mui-{spacingString}{sideString}{spacingSizeString}";
    }

    public override string Spacing( Spacing spacing, SpacingSize spacingSize, IEnumerable<(Side side, Breakpoint breakpoint)> rules ) => string.Join( " ", rules.Select( x => Spacing( spacing, spacingSize, x.side, x.breakpoint ) ) );

    #endregion

    #region Gutter

    public override string Gutter( GutterSize gutterSize, GutterSide gutterSide, Breakpoint breakpoint )
    {
        var sb = new StringBuilder( "mui-gutter" );

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

    #region Gap

    public override string Gap( GapSize gapSize, GapSide gapSide )
    {
        var side = gapSide != GapSide.None && gapSide != GapSide.All
            ? $"{ToGapSide( gapSide )}-"
            : null;

        return $"mui-gap-{side}{ToGapSize( gapSize )}";
    }

    public override string Gap( GapSize gapSize, IEnumerable<GapSide> rules )
        => string.Join( " ", rules.Select( x => Gap( gapSize, x ) ) );

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
            ? $"mui-display-{ToFlexType( flexType )}"
            : null;
    }

    public override string Flex( FlexDefinition flexDefinition )
    {
        var sb = new StringBuilder();

        var breakpoint = flexDefinition.Breakpoint != Breakpoint.None && flexDefinition.Breakpoint != Breakpoint.Mobile
            ? $"{ToBreakpoint( flexDefinition.Breakpoint )}-"
            : null;

        if ( flexDefinition.Direction != FlexDirection.Default )
            sb.Append( "mui-flex-" ).Append( breakpoint ).Append( ToDirection( flexDefinition.Direction ) );

        if ( flexDefinition.JustifyContent != FlexJustifyContent.Default )
            sb.Append( "mui-justify-content-" ).Append( breakpoint ).Append( ToJustifyContent( flexDefinition.JustifyContent ) );

        if ( flexDefinition.AlignItems != FlexAlignItems.Default )
            sb.Append( "mui-align-items-" ).Append( breakpoint ).Append( ToAlignItems( flexDefinition.AlignItems ) );

        if ( flexDefinition.AlignSelf != FlexAlignSelf.Default )
            sb.Append( "mui-align-self-" ).Append( breakpoint ).Append( ToAlignSelf( flexDefinition.AlignSelf ) );

        if ( flexDefinition.AlignContent != FlexAlignContent.Default )
            sb.Append( "mui-align-content-" ).Append( breakpoint ).Append( ToAlignContent( flexDefinition.AlignContent ) );

        if ( flexDefinition.GrowShrink != FlexGrowShrink.Default && flexDefinition.GrowShrinkSize != FlexGrowShrinkSize.Default )
            sb.Append( "mui-flex-" ).Append( breakpoint ).Append( ToGrowShrink( flexDefinition.GrowShrink ) ).Append( "-" ).Append( ToGrowShrinkSize( flexDefinition.GrowShrinkSize ) );

        if ( flexDefinition.Basis && flexDefinition.BasisSize != FlexBasisSize.Default )
            sb.Append( "mui-flex-basis-" ).Append( breakpoint ).Append( ToBasisSize( flexDefinition.BasisSize ) );

        if ( flexDefinition.Wrap != FlexWrap.Default )
            sb.Append( "mui-flex-" ).Append( breakpoint ).Append( ToWrap( flexDefinition.Wrap ) );

        if ( flexDefinition.Order != FlexOrder.Default )
            sb.Append( "mui-order-" ).Append( breakpoint ).Append( ToOrder( flexDefinition.Order ) );

        if ( flexDefinition.Fill )
            sb.Append( "mui-flex-" ).Append( breakpoint ).Append( "fill" );

        return sb.ToString();
    }

    public override string Flex( FlexRule flexRule )
    {
        var sb = new StringBuilder();

        if ( flexRule.FlexType != FlexType.Default )
        {
            if ( flexRule.Breakpoint > Breakpoint.Mobile )
            {
                sb.Append( $"mui-display-{ToBreakpoint( flexRule.Breakpoint )}-{ToFlexType( flexRule.FlexType )}" );
            }
            else
            {
                sb.Append( $"mui-display-{ToFlexType( flexRule.FlexType )}" );
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

    public override string ToColor( Color color )
    {
        if ( color == Color.Danger )
            return "error";

        return base.ToColor( color );
    }

    public override string ToSpacing( Spacing spacing )
    {
        return spacing switch
        {
            Blazorise.Spacing.Margin => "margin",
            Blazorise.Spacing.Padding => "padding",
            _ => null,
        };
    }

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
            Blazorise.Side.Top => "top",
            Blazorise.Side.Bottom => "bottom",
            Blazorise.Side.Start => "start",
            Blazorise.Side.End => "end",
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

    public override bool UseCustomInputStyles { get; set; } = false;

    public override string Provider => "Material";
}