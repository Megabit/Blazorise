#region Using directives
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Blazorise.Extensions;
#endregion

namespace Blazorise.AntDesign.Providers;

public class AntDesignClassProvider : ClassProvider
{
    #region TextInput

    public override string TextInput( bool plaintext ) => plaintext ? "ant-form-text" : "ant-input ant-input-outlined";

    public override string TextInputSize( Size size ) => size != Size.Default ? $"ant-input-{ToSize( size )}" : null;

    public override string TextInputColor( Color color ) => color.IsNotNullOrDefault() ? $"ant-typography-{ToColor( color )}" : null;

    public override string TextInputValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

    #endregion

    #region MemoInput

    public override string MemoInput( bool plaintext ) => plaintext ? "ant-form-text" : "ant-input ant-input-outlined";

    public override string MemoInputSize( Size size ) => size != Size.Default ? $"ant-input-{ToSize( size )}" : null;

    public override string MemoInputValidation( ValidationStatus validationStatus ) => validationStatus != ValidationStatus.None ? ToValidationStatus( validationStatus, "ant-input" ) : null;

    #endregion

    #region Select

    public override string Select() => "ant-select ant-select-show-arrow ant-select-outlined";

    public override string SelectMultiple( bool multiple ) => multiple ? "ant-select-multiple" : "ant-select-single";

    public override string SelectSize( Size size ) => size != Size.Default ? $"ant-select-{ToSize( size )}" : null;

    public override string SelectValidation( ValidationStatus validationStatus ) => validationStatus != ValidationStatus.None ? ToValidationStatus( validationStatus, "ant-select" ) : null;

    #endregion

    #region NumericInput

    public override string NumericInput( bool plaintext ) => plaintext ? "ant-form-text" : "ant-input-number ant-input-number-outlined";

    public override string NumericInputSize( Size size ) => size != Size.Default ? $"ant-input-number-{ToSize( size )}" : null;

    public override string NumericInputColor( Color color ) => color.IsNotNullOrDefault() ? $"ant-typography-{ToColor( color )}" : null;

    public override string NumericInputValidation( ValidationStatus validationStatus ) => validationStatus != ValidationStatus.None ? ToValidationStatus( validationStatus, "ant-input-number" ) : null;

    #endregion

    #region DateInput

    public override string DateInput( bool plaintext ) => plaintext ? "ant-form-text" : "ant-picker ant-picker-outlined";

    public override string DateInputSize( Size size ) => size != Size.Default ? $"ant-picker-{ToSize( size )}" : null;

    public override string DateInputColor( Color color ) => color.IsNotNullOrDefault() ? $"ant-typography-{ToColor( color )}" : null;

    public override string DateInputValidation( ValidationStatus validationStatus ) => validationStatus != ValidationStatus.None ? ToValidationStatus( validationStatus, "ant-picker" ) : null;

    #endregion

    #region TimeInput

    public override string TimeInput( bool plaintext ) => plaintext ? "ant-form-text" : "ant-picker ant-picker-outlined";

    public override string TimeInputSize( Size size ) => size != Size.Default ? $"ant-picker-{ToSize( size )}" : null;

    public override string TimeInputColor( Color color ) => color.IsNotNullOrDefault() ? $"ant-typography-{ToColor( color )}" : null;

    public override string TimeInputValidation( ValidationStatus validationStatus ) => validationStatus != ValidationStatus.None ? ToValidationStatus( validationStatus, "ant-picker" ) : null;

    #endregion

    #region ColorInput

    public override string ColorInput() => "b-ant-color-input";

    public override string ColorInputSize( Size size ) => null;

    #endregion

    #region DatePicker

    public override string DatePicker( bool plaintext ) => plaintext ? "ant-form-text" : "ant-picker ant-picker-outlined";

    public override string DatePickerSize( Size size ) => size != Size.Default ? $"ant-picker-{ToSize( size )}" : null;

    public override string DatePickerColor( Color color ) => color.IsNotNullOrDefault() ? $"ant-typography-{ToColor( color )}" : null;

    public override string DatePickerValidation( ValidationStatus validationStatus ) => validationStatus != ValidationStatus.None ? ToValidationStatus( validationStatus, "ant-picker" ) : null;

    #endregion

    #region TimePicker

    public override string TimePicker( bool plaintext ) => plaintext ? "ant-form-text" : "ant-picker ant-picker-outlined";

    public override string TimePickerSize( Size size ) => size != Size.Default ? $"ant-picker-{ToSize( size )}" : null;

    public override string TimePickerColor( Color color ) => color.IsNotNullOrDefault() ? $"ant-typography-{ToColor( color )}" : null;

    public override string TimePickerValidation( ValidationStatus validationStatus ) => validationStatus != ValidationStatus.None ? ToValidationStatus( validationStatus, "ant-picker" ) : null;

    #endregion

    #region ColorPicker

    public override string ColorPicker() => "ant-input b-input-color-picker";

    public override string ColorPickerSize( Size size ) => size != Size.Default ? $"ant-input-{ToSize( size )}" : null;

    #endregion

    #region NumericPicker

    public override string NumericPicker( bool plaintext ) => plaintext ? "ant-form-text" : "ant-input";

    public override string NumericPickerSize( Size size ) => size != Size.Default ? $"ant-input-{ToSize( size )}" : null;

    public override string NumericPickerColor( Color color ) => color.IsNotNullOrDefault() ? $"ant-typography-{ToColor( color )}" : null;

    public override string NumericPickerValidation( ValidationStatus validationStatus ) => validationStatus != ValidationStatus.None ? ToValidationStatus( validationStatus, "ant-input-number" ) : null;

    #endregion

    #region InputMask

    public override string InputMask( bool plaintext ) => plaintext ? "ant-form-text" : "ant-input";

    public override string InputMaskSize( Size size ) => size != Size.Default ? $"ant-input-{ToSize( size )}" : null;

    public override string InputMaskColor( Color color ) => color.IsNotNullOrDefault() ? $"ant-typography-{ToColor( color )}" : null;

    public override string InputMaskValidation( ValidationStatus validationStatus ) => validationStatus != ValidationStatus.None ? ToValidationStatus( validationStatus ) : null;

    #endregion

    #region Check

    public override string Check() => "ant-checkbox-input";

    public override string CheckSize( Size size ) => size != Size.Default ? $"{Check()}-{ToSize( size )}" : null;

    public override string CheckInline( bool inline ) => null;

    public override string CheckCursor( Cursor cursor ) => null;

    public override string CheckValidation( ValidationStatus validationStatus ) => validationStatus != ValidationStatus.None ? ToValidationStatus( validationStatus ) : null;

    #endregion

    #region RadioGroup

    public override string RadioGroup( bool buttons, Orientation orientation )
        => "ant-radio-group"
            + ( buttons ? " ant-radio-group-outline" : "" )
            + ( orientation == Orientation.Horizontal ? "" : " ant-radio-group-vertical" );

    public override string RadioGroupSize( bool buttons, Orientation orientation, Size size ) => size switch
    {
        Size.Small => "ant-radio-group-small",
        Size.Large => "ant-radio-group-large",
        _ => null,
    };

    public override string RadioGroupValidation( ValidationStatus validationStatus ) => validationStatus != ValidationStatus.None ? ToValidationStatus( validationStatus ) : null;

    #endregion

    #region Radio

    public override string Radio( bool button ) => button ? "ant-radio-button-input" : "ant-radio-input";

    public override string RadioSize( bool button, Size size ) => button || size == Size.Default ? null : $"{Radio( button )}-{ToSize( size )}";

    public override string RadioInline( bool inline ) => null;

    public override string RadioCursor( Cursor cursor ) => null;

    public override string RadioValidation( ValidationStatus validationStatus ) => validationStatus != ValidationStatus.None ? ToValidationStatus( validationStatus ) : null;

    #endregion

    #region Switch

    public override string Switch() => "ant-switch";

    public override string SwitchColor( Color color ) => color.IsNotNullOrDefault() ? $"{Switch()}-{ToColor( color )}" : null;

    public override string SwitchSize( Size size ) => size != Size.Default ? $"{Switch()}-{ToSize( size )}" : null;

    public override string SwitchChecked( bool @checked ) => @checked ? "ant-switch-checked" : null;

    public override string SwitchCursor( Cursor cursor ) => cursor != Cursor.Default ? $"{Switch()}-{ToCursor( cursor )}" : null;

    public override string SwitchValidation( ValidationStatus validationStatus ) => validationStatus != ValidationStatus.None ? ToValidationStatus( validationStatus ) : null;

    #endregion

    #region FileInput

    public override string FileInput() => "ant-upload-input";

    public override string FileInputSize( Size size ) => null;

    public override string FileInputValidation( ValidationStatus validationStatus ) => validationStatus != ValidationStatus.None ? ToValidationStatus( validationStatus ) : null;

    #endregion

    #region Slider

    public override string Slider() => "ant-slider";

    public override string SliderColor( Color color ) => $"ant-slider-{ToColor( color )}";

    public override string SliderValidation( ValidationStatus validationStatus ) => validationStatus != ValidationStatus.None ? ToValidationStatus( validationStatus ) : null;

    #endregion

    #region Rating

    public override string Rating() => "ant-rate";

    public override string RatingDisabled( bool disabled ) => disabled ? "ant-rate-disabled" : null;

    public override string RatingReadonly( bool @readonly ) => @readonly ? "ant-rate-readonly" : null;

    public override string RatingItem() => "ant-rate-star";

    public override string RatingItemColor( Color color ) => color.IsNotNullOrDefault() ? $"ant-rate-star-{ToColor( color )}" : null;

    public override string RatingItemSelected( bool selected ) => selected ? "ant-rate-star-full" : null;

    public override string RatingItemHovered( bool hover ) => hover ? "ant-rate-star-focused" : null;

    #endregion

    #region Label

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

    public override string FieldHorizontal( bool horizontal ) => horizontal ? "ant-row" : null;

    public override string FieldColumn() => "ant-col";

    public override string FieldSize( Size size ) => null;

    public override string FieldJustifyContent( JustifyContent justifyContent ) => ToJustifyContent( justifyContent );

    public override string FieldValidation( ValidationStatus validationStatus ) => validationStatus switch
    {
        ValidationStatus.Error => "ant-form-item-has-feedback ant-form-item-has-error",
        ValidationStatus.Success => "ant-form-item-has-feedback ant-form-item-has-success",
        _ => null,
    };

    #endregion

    #region FieldLabel

    public override string FieldLabel( bool horizontal ) => horizontal ? "ant-form-item-label" : null;

    public override string FieldLabelRequiredIndicator( bool requiredIndicator )
        => requiredIndicator
            ? "ant-form-item-required"
            : null;

    public override string FieldLabelScreenreader( Screenreader screenreader ) => screenreader != Screenreader.Always ? ToScreenreader( screenreader ) : null;

    #endregion

    #region FieldBody

    public override string FieldBody() => null;

    #endregion

    #region FieldHelp

    public override string FieldHelp() => "ant-form-item-explain";

    #endregion

    #region FocusTrap

    public override string FocusTrap() => "ant-focus-trap";

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

    public override string Addons() => "ant-input-group-wrapper ant-input-group-wrapper-outlined b-ant-addons";

    public override string AddonsSize( Size size ) => size != Size.Default ? $"ant-input-group-wrapper-{ToSize( size )}" : null;

    public override string AddonsHasButton( bool hasButton ) => hasButton ? "b-ant-addons-has-button" : null;

    public override string AddonsValidation( ValidationStatus validationStatus )
        => validationStatus switch
        {
            ValidationStatus.Error => "ant-input-group-wrapper-status-error",
            ValidationStatus.Success => "ant-input-group-wrapper-status-success",
            _ => null,
        };

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

    public override string AddonSize( Size size ) => null;

    public override string AddonLabel() => null;

    #endregion

    #region Inline

    public override string Inline() => "ant-form-inline";

    #endregion

    #region Button

    public override string Button( bool outline ) => "ant-btn";

    public override string ButtonColor( Color color, bool outline ) => ToButtonColorClass( color, outline );

    public override string ButtonSize( Size size, bool outline ) => size == Size.Default ? null : $"{Button( outline )}-{ToSize( size )}";

    public override string ButtonBlock( bool outline, bool block ) => block ? $"{Button( outline )}-block" : null;

    public override string ButtonActive( bool outline, bool active ) => active ? Active() : null;

    public override string ButtonDisabled( bool outline, bool disabled ) => disabled ? "ant-btn-disabled" : null;

    public override string ButtonLoading( bool outline, bool loading ) => loading ? "ant-btn-loading" : null;

    public override string ButtonStretchedLink( bool stretched ) => stretched ? "ant-link-stretched" : null;

    #endregion

    #region Buttons

    public override string Buttons( ButtonsRole role, Orientation orientation )
    {
        if ( role == ButtonsRole.Toolbar )
            return null;

        if ( orientation == Orientation.Vertical )
            return "ant-btn-group ant-btn-group-vertical";

        return "ant-btn-group";
    }

    public override string ButtonsSize( Size size ) => size != Size.Default ? $"ant-btn-group-{ToSize( size )}" : null;

    #endregion

    #region CloseButton

    public override string CloseButton() => "ant-close";

    #endregion

    #region Dropdown

    public override string Dropdown( bool isDropdownSubmenu ) => isDropdownSubmenu ? "ant-dropdown-menu-submenu ant-dropdown-menu-submenu-vertical" : "b-ant-dropdown";

    public override string DropdownDisabled( bool disabled ) => disabled ? "b-ant-dropdown-disabled" : null;

    public override string DropdownGroup( bool group ) => group ? "ant-space-compact ant-dropdown-button b-ant-dropdown-group" : null;

    public override string DropdownObserverShow() => "ant-dropdown-hidden";

    public override string DropdownShow( bool show ) => null;

    public override string DropdownRight( bool rightAligned ) => null;

    public override string DropdownItem() => "ant-dropdown-menu-item";

    public override string DropdownItemActive( bool active ) => active ? "ant-dropdown-menu-item-selected" : null;

    public override string DropdownItemDisabled( bool disabled ) => disabled ? "ant-dropdown-menu-item-disabled" : null;

    public override string DropdownDivider() => "ant-dropdown-menu-item-divider";

    public override string DropdownHeader() => "ant-dropdown-menu-item-group-title";

    public override string DropdownMenu() => "b-ant-dropdown-overlay";

    public override string DropdownMenuPositionStrategy( DropdownPositionStrategy dropdownPositionStrategy )
        => $"b-ant-dropdown-position-strategy {( dropdownPositionStrategy == DropdownPositionStrategy.Fixed ? "b-ant-dropdown-position-strategy-fixed" : "b-ant-dropdown-position-strategy-absolute" )}";

    public override string DropdownFixedHeaderVisible( bool visible )
        => visible ? "b-ant-dropdown-table-fixed-header-visible" : null;

    public override string DropdownMenuSelector() => "b-ant-dropdown-overlay";

    public override string DropdownMenuScrollable( bool scrollable ) => scrollable ? "b-ant-dropdown-menu-scrollable" : null;

    public override string DropdownMenuVisible( bool visible ) => visible ? null : "ant-dropdown-hidden ant-dropdown-menu-hidden ant-dropdown-menu-submenu-hidden";

    public override string DropdownMenuEnd( bool endAligned ) => null;

    public override string DropdownToggle( bool isDropdownSubmenu, bool outline ) => isDropdownSubmenu ? "ant-dropdown-menu-submenu-title" : "ant-btn ant-dropdown-trigger";

    public override string DropdownToggleSelector( bool isDropdownSubmenu ) => isDropdownSubmenu ? "ant-dropdown-menu-submenu-title" : "ant-dropdown-trigger";

    public override string DropdownToggleColor( Color color, bool outline ) => ToButtonColorClass( color, outline );

    public override string DropdownToggleSize( Size size, bool outline )
        => size != Size.Default ? $"ant-btn-{ToSize( size )}" : null;

    public override string DropdownToggleSplit( bool split ) => split ? "ant-btn-icon-only b-ant-dropdown-toggle-split" : null;

    public override string DropdownToggleIcon( bool visible ) => null;

    public override string DropdownDirection( Direction direction ) => null;

    #endregion

    #region Tabs

    public override string Tabs( bool pills ) => pills ? "ant-tabs ant-tabs-top ant-tabs-pills" : "ant-tabs ant-tabs-top";

    public override string TabsCards( bool cards ) => cards ? "ant-tabs-card" : null;

    public override string TabsFullWidth( bool fullWidth ) => fullWidth ? "ant-tabs-fill" : null;

    public override string TabsJustified( bool justified ) => justified ? "ant-tabs-justified" : null;

    public override string TabsVertical( bool vertical ) => null;

    public override string TabItem( TabPosition tabPosition ) => "ant-tabs-tab";

    public override string TabItemActive( bool active ) => active ? "ant-tabs-tab-active" : null;

    public override string TabItemDisabled( bool disabled ) => disabled ? "ant-tabs-tab-disabled" : null;

    public override string TabLink( TabPosition tabPosition ) => null;

    public override string TabLinkActive( bool active ) => null;

    public override string TabLinkDisabled( bool disabled ) => null;

    public override string TabsContent() => "ant-tabs-content";

    public override string TabPanel() => "ant-tabs-tabpane";

    public override string TabPanelActive( bool active ) => active ? null : "ant-tabs-tabpane-hidden";

    #endregion

    #region Steps

    public override string Steps() => "ant-steps ant-steps-horizontal";

    public override string StepItem() => "ant-steps-item";

    public override string StepItemActive( bool active ) => active ? "ant-steps-item-process ant-steps-item-active" : "ant-steps-item-wait";

    public override string StepItemCompleted( bool completed ) => completed ? "ant-steps-item-finish" : null;

    public override string StepItemColor( Color color ) => color.IsNotNullOrDefault() ? $"ant-steps-item-{ToColor( color )}" : null;

    public override string StepItemMarker() => "ant-steps-item-icon";

    public override string StepItemMarkerColor( Color color, bool active ) => null;

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

    public override string CarouselSlideIndex( int activeSlideIndex, int slideindex, int totalSlides ) => null;

    public override string CarouselSlideSlidingLeft( bool left ) => null;

    public override string CarouselSlideSlidingRight( bool right ) => null;

    public override string CarouselSlideSlidingPrev( bool previous ) => null;

    public override string CarouselSlideSlidingNext( bool next ) => null;

    public override string CarouselIndicators() => "slick-dots slick-dots-bottom";

    public override string CarouselIndicator() => null;

    public override string CarouselIndicatorActive( bool active ) => active ? "slick-active" : null;

    public override string CarouselFade( bool fade ) => null;

    public override string CarouselCaption() => null;

    #endregion

    #region Jumbotron

    public override string Jumbotron() => "ant-hero";

    public override string JumbotronBackground( Background background ) => background.IsNotNullOrDefault() ? $"ant-hero-{ToBackground( background )}" : null;

    public override string JumbotronTitle( JumbotronTitleSize jumbotronTitleSize ) => $"ant-display-{ToJumbotronTitleSize( jumbotronTitleSize )}";

    public override string JumbotronSubtitle() => "ant-hero-subtitle";

    #endregion

    #region Card

    public override string CardDeck() => "ant-card-deck";

    public override string CardGroup() => "ant-card-group";

    public override string Card() => "ant-card ant-card-bordered";

    public override string CardWhiteText( bool whiteText ) => whiteText ? "ant-typography-light" : null;

    public override string CardActions() => "ant-card-actions";

    public override string CardBody() => "ant-card-body";

    public override string CardFooter() => "ant-card-body b-ant-card-footer";

    public override string CardHeader() => "ant-card-head";

    public override string CardImage() => "ant-card-cover";

    public override string CardTitle( bool insideHeader ) => insideHeader ? "ant-card-head-title" : "ant-card-meta-title";

    public override string CardTitleSize( bool insideHeader, HeadingSize? size ) => null;

    public override string CardSubtitle( bool insideHeader ) => insideHeader ? "b-ant-card-head-subtitle" : "ant-card-meta-description";

    public override string CardSubtitleSize( bool insideHeader, HeadingSize? size ) => null;

    public override string CardText() => "b-ant-card-text";

    public override string CardLink() => "ant-card-link";

    public override string CardLinkUnstyled( bool unstyled ) => unstyled ? "ant-link-unstyled" : null;

    public override string CardLinkActive( bool active ) => LinkActive( active );

    #endregion

    #region ListGroup

    public override string ListGroup() => "ant-list ant-list-split ant-list-bordered b-ant-list-group";

    public override string ListGroupFlush( bool flush ) => flush ? "b-ant-list-group-flush" : null;

    public override string ListGroupScrollable( bool scrollable ) => scrollable ? "b-ant-list-group-scrollable" : null;

    public override string ListGroupItem() => "ant-list-item b-ant-list-group-item";

    public override string ListGroupItemSelectable( bool selectable ) => selectable ? "b-ant-list-group-item-selectable" : null;

    public override string ListGroupItemActive( bool active ) => active ? "b-ant-list-group-item-active" : null;

    public override string ListGroupItemDisabled( bool disabled ) => disabled ? "b-ant-list-group-item-disabled" : null;

    public override string ListGroupItemColor( Color color, bool selectable, bool active ) => color == Color.Default ? null : $"b-ant-list-group-item-{ToColor( color )}";

    #endregion

    #region Container

    public override string Container( Breakpoint breakpoint )
        => breakpoint != Breakpoint.None && breakpoint != Breakpoint.Mobile ? $"ant-container-{ToBreakpoint( breakpoint )}" : "ant-container";

    public override string ContainerFluid() => "ant-container-fluid";

    #endregion

    #region Bar

    public override string Bar( BarMode mode ) => "ant-menu ant-menu-root";

    public override string BarInitial( BarMode mode, bool initial ) => mode != Blazorise.BarMode.Horizontal && initial ? "b-bar-initial" : null;

    public override string BarAlignment( BarMode mode, Alignment alignment ) => alignment switch
    {
        Alignment.Start => "ant-justify-content-flex-start",
        Alignment.Center => "ant-justify-content-center",
        Alignment.End => "ant-justify-content-flex-end",
        _ => null,
    };

    public override string BarThemeContrast( BarMode mode, ThemeContrast themeContrast ) => themeContrast != ThemeContrast.None ? $"ant-menu-{ToThemeContrast( themeContrast )}" : null;

    public override string BarBreakpoint( BarMode mode, Breakpoint breakpoint ) => null;

    public override string BarMode( BarMode mode ) => $"ant-menu-{ToBarMode( mode )} {( mode == Blazorise.BarMode.VerticalSmall ? "ant-menu-inline-collapsed" : null )}";

    public override string BarItem( BarMode mode, bool hasDropdown ) => mode == Blazorise.BarMode.Horizontal
        ? "ant-menu-item ant-menu-item-only-child"
        : "ant-menu-item";

    public override string BarItemActive( BarMode mode, bool active ) => active ? "ant-menu-item-selected" : null;

    public override string BarItemDisabled( BarMode mode, bool disabled ) => disabled ? "ant-menu-item-disabled" : null;

    public override string BarItemHasDropdown( BarMode mode, bool hasDropdown ) => null;

    public override string BarLink( BarMode mode ) => null;

    public override string BarLinkDisabled( BarMode mode, bool disabled ) => disabled ? Disabled() : null;

    public override string BarIcon( BarMode mode ) => "ant-menu-item-icon";

    public override string BarBrand( BarMode mode ) => null;

    public override string BarBrandToggler( BarMode mode ) => "b-bar-mobile-toggle";

    public override string BarToggler( BarMode mode, BarTogglerMode togglerMode ) => mode == Blazorise.BarMode.Horizontal ? null :
        "ant-menu-toggler-inline";

    public override string BarTogglerCollapsed( BarMode mode, BarTogglerMode togglerMode, bool isShow ) => null;

    public override string BarMenu( BarMode mode ) => null;

    public override string BarMenuShow( BarMode mode, bool show ) => null;

    public override string BarStart( BarMode mode ) => null;

    public override string BarEnd( BarMode mode ) => null;

    public override string BarDropdown( BarMode mode, bool isBarDropDownSubmenu )
    {
        var effectiveMode = mode == Blazorise.BarMode.Horizontal && isBarDropDownSubmenu
            ? "vertical"
            : ToBarMode( mode );

        return $"ant-menu-submenu ant-menu-submenu-{effectiveMode}";
    }

    public override string BarDropdownShow( BarMode mode, bool show ) => show ? "ant-menu-submenu-open" : null;

    public override string BarDropdownToggle( BarMode mode, bool isBarDropDownSubmenu ) => mode == Blazorise.BarMode.Horizontal ? "ant-menu-submenu-title" : "ant-menu-submenu-title";

    public override string BarDropdownToggleDisabled( BarMode mode, bool isBarDropDownSubmenu, bool disabled ) => mode == Blazorise.BarMode.Horizontal && disabled ? "ant-menu-submenu-disabled" : null;

    public override string BarDropdownToggleIcon( bool isToggleIconVisible ) => null;

    public override string BarDropdownToggleIconContainer( BarMode mode ) => "b-bar-dropdown-toggle-icon-container";

    public override string BarDropdownToggleIconLayer( BarMode mode ) => "b-bar-dropdown-toggle-icon-layer";

    public override string BarDropdownToggleIconLayerVisible( BarMode mode, bool visible ) => visible ? "b-bar-dropdown-toggle-icon-layer-visible" : null;

    public override string BarDropdownToggleIconLayerHiddenExpand( BarMode mode, bool hiddenExpand ) => hiddenExpand ? "b-bar-dropdown-toggle-icon-layer-hidden-expand" : null;

    public override string BarDropdownToggleIconLayerHiddenCollapse( BarMode mode, bool hiddenCollapse ) => hiddenCollapse ? "b-bar-dropdown-toggle-icon-layer-hidden-collapse" : null;

    public override string BarDropdownItem( BarMode mode ) => "ant-menu-item";

    public override string BarDropdownItemDisabled( BarMode mode, bool disabled ) => null;

    public override string BarDropdownDivider( BarMode mode ) => "ant-menu-item-divider";

    public override string BarTogglerIcon( BarMode mode ) => mode == Blazorise.BarMode.Horizontal ? "navbar-toggler-icon" : "navbar-toggler-icon";

    public override string BarDropdownMenu( BarMode mode ) => mode == Blazorise.BarMode.Horizontal ? "ant-menu ant-menu-sub ant-menu-vertical" : $"ant-menu ant-menu-sub ant-menu-{ToBarMode( mode )}";

    public override string BarDropdownMenuVisible( BarMode mode, bool visible ) => visible ? null : "ant-menu-hidden";

    public override string BarDropdownMenuRight( BarMode mode, bool rightAligned ) => null;

    public override string BarDropdownMenuContainer( BarMode mode ) => "ant-menu-submenu-popup";

    public override string BarCollapsed( BarMode mode, bool visible ) => visible ? null : $"ant-menu-{ToBarMode( mode )}-collapsed";

    public override string BarLabel( BarMode mode ) => "ant-menu-label";

    #endregion

    #region Accordion

    public override string Accordion() => "ant-collapse";

    public override string AccordionToggle() => "b-ant-accordion-toggle";

    public override string AccordionToggleCollapsed( bool collapsed ) => null;

    public override string AccordionItem() => "ant-collapse-item";

    public override string AccordionItemActive( bool active ) => active ? "ant-collapse-item-active" : null;

    public override string AccordionHeader() => "ant-collapse-header";

    public override string AccordionBody() => "ant-collapse-panel";

    public override string AccordionBodyActive( bool active ) => active ? null : "ant-collapse-panel-hidden";

    public override string AccordionBodyContent( bool firstInAccordion, bool lastInAccordion ) => "ant-collapse-body";

    #endregion

    #region Collapse

    public override string Collapse() => "ant-collapse-item";

    public override string CollapseActive( bool active ) => active ? "ant-collapse-item-active" : null;

    public override string CollapseHeader() => "ant-collapse-header";

    public override string CollapseBody() => "ant-collapse-panel";

    public override string CollapseBodyActive( bool active ) => active ? null : "ant-collapse-panel-hidden";

    public override string CollapseBodyContent() => "ant-collapse-body";

    #endregion

    #region Row

    public override string Row() => "ant-row";

    public override string RowColumns( RowColumnsSize rowColumnsSize, RowColumnsDefinition rowColumnsDefinition )
    {
        if ( rowColumnsDefinition.Breakpoint != Breakpoint.None && rowColumnsDefinition.Breakpoint != Breakpoint.Mobile )
            return $"ant-row-columns-{ToBreakpoint( rowColumnsDefinition.Breakpoint )}-{ToRowColumnsSize( rowColumnsSize )}";

        return $"ant-row-columns-{ToRowColumnsSize( rowColumnsSize )}";
    }

    #endregion

    #region Column

    public override string Column( bool grid, bool hasSizes ) => "ant-col";

    public override string Column( bool grid, ColumnWidth columnWidth, Breakpoint breakpoint, bool offset )
    {
        // AntDesign requires for base ant-col class to be always defined.
        var sb = new StringBuilder( grid ? "ant-grid-col" : "ant-col" );

        if ( breakpoint != Blazorise.Breakpoint.None )
            sb.Append( $"-{ToBreakpoint( breakpoint )}" );

        if ( offset )
            sb.Append( "-offset" );

        sb.Append( $"-{ToColumnWidth( columnWidth )}" );

        return sb.ToString();
    }

    public override string Column( bool grid, IEnumerable<ColumnDefinition> columnDefinitions )
        => string.Join( ' ', columnDefinitions.Select( x => Column( grid, x.ColumnWidth, x.Breakpoint, x.Offset ) ) );

    #endregion

    public override string Grid() => "ant-grid";

    public override string GridRows( GridRowsSize gridRows, GridRowsDefinition gridRowsDefinition )
    {
        if ( gridRowsDefinition.Breakpoint != Breakpoint.None && gridRowsDefinition.Breakpoint != Breakpoint.Mobile )
            return $"ant-grid-rows-{ToBreakpoint( gridRowsDefinition.Breakpoint )}-{ToGridRowsSize( gridRows )}";

        return $"ant-grid-rows-{ToGridRowsSize( gridRows )}";
    }

    public override string GridColumns( GridColumnsSize gridColumns, GridColumnsDefinition gridColumnsDefinition )
    {
        if ( gridColumnsDefinition.Breakpoint != Breakpoint.None && gridColumnsDefinition.Breakpoint != Breakpoint.Mobile )
            return $"ant-grid-cols-{ToBreakpoint( gridColumnsDefinition.Breakpoint )}-{ToGridColumnsSize( gridColumns )}";

        return $"ant-grid-cols-{ToGridColumnsSize( gridColumns )}";
    }

    #region Display

    public override string Display( DisplayType displayType, DisplayDefinition displayDefinition )
    {
        var baseClass = displayDefinition.Breakpoint != Breakpoint.None && displayDefinition.Breakpoint != Breakpoint.Mobile
            ? $"ant-display-{ToBreakpoint( displayDefinition.Breakpoint )}-{ToDisplayType( displayType )}"
            : $"ant-display-{ToDisplayType( displayType )}";

        if ( displayDefinition.Direction != DisplayDirection.Default )
            return $"{baseClass} ant-flex-{ToDisplayDirection( displayDefinition.Direction )}";

        return baseClass;
    }

    #endregion

    #region Alert

    public override string Alert() => "ant-alert";

    public override string AlertColor( Color color )
    {
        if ( color.IsNullOrDefault() )
            return null;

        if ( color == Color.Success || color == Color.Info || color == Color.Warning )
            return $"ant-alert-{ToColor( color )}";

        if ( color == Color.Danger )
            return "ant-alert-error";

        if ( color == Color.Primary )
            return "ant-alert-info";

        return $"ant-alert-{ToColor( color )}";
    }

    public override string AlertDismisable( bool dismissable ) => dismissable ? "b-ant-alert-dismissible" : null;

    public override string AlertFade( bool dismissable ) => dismissable ? Fade() : null;

    public override string AlertShow( bool dismissable, bool visible ) => dismissable && visible ? Show() : null;

    public override string AlertHasMessage( bool hasMessage ) => null;

    public override string AlertHasDescription( bool hasDescription ) => hasDescription ? "ant-alert-with-description" : null;

    public override string AlertMessage() => "ant-alert-title";

    public override string AlertDescription() => "ant-alert-description";

    #endregion

    #region Modal

    public override string Modal() => "ant-modal-root";

    public override string ModalFade( bool showing, bool hiding ) => showing || hiding ? Fade() : null;

    public override string ModalVisible( bool visible ) => null;

    public override string ModalSize( ModalSize modalSize ) => null;

    public override string ModalCentered( bool centered ) => null;

    public override string ModalBackdrop() => "ant-modal-mask";

    public override string ModalBackdropFade() => null;

    public override string ModalBackdropVisible( bool visible ) => null;

    public override string ModalContent( bool dialog ) => "ant-modal-content";

    public override string ModalContentSize( ModalSize modalSize ) => modalSize == Blazorise.ModalSize.Fullscreen
        ? "ant-modal-content-fullscreen"
        : null;

    public override string ModalContentCentered( bool centered ) => null;

    public override string ModalContentScrollable( bool scrollable ) => scrollable ? "ant-modal-content-scrollable" : null;

    public override string ModalBody() => "ant-modal-body";

    public override string ModalHeader() => "ant-modal-header";

    public override string ModalFooter() => "ant-modal-footer";

    public override string ModalTitle() => "ant-modal-title";

    #endregion

    #region Offcanvas

    public override string Offcanvas() => "ant-drawer";

    public override string OffcanvasPlacement( Placement placement, bool visible )
    {
        return placement switch
        {
            Placement.Start => "ant-drawer-left",
            Placement.End => "ant-drawer-right",
            Placement.Top => "ant-drawer-top",
            Placement.Bottom => "ant-drawer-bottom",
            _ => "",
        };
    }

    public override string OffcanvasFade( bool showing, bool hiding ) => showing
        ? "ant-showing"
        : hiding
            ? "ant-hiding"
            : null;

    public override string OffcanvasVisible( bool visible ) => visible ? "ant-drawer-open" : null;

    public override string OffcanvasHeader() => "ant-drawer-header";

    public override string OffcanvasFooter() => "ant-drawer-footer";

    public override string OffcanvasBody() => "ant-drawer-body";

    public override string OffcanvasBackdrop() => "ant-drawer-mask";

    public override string OffcanvasBackdropFade( bool showing, bool hiding ) => null;

    public override string OffcanvasBackdropVisible( bool visible ) => null;

    #endregion

    #region Toast

    public override string Toast() => "ant-notification-notice ant-notification-notice-closable";

    public override string ToastAnimated( bool animated ) => null;

    public override string ToastFade( bool visible, bool showing, bool hiding ) => showing
        ? "ant-notification-fade-enter ant-notification-fade-enter-active ant-notification-fade-appear ant-notification-fade-appear-prepare ant-notification-fade-appear-active"
        : hiding
            ? "ant-notification-fade-leave ant-notification-fade-leave-active"
            : null;

    public override string ToastVisible( bool visible ) => null;

    public override string ToastHeader() => "ant-notification-notice-title";

    public override string ToastBody() => "ant-notification-notice-description";

    public override string Toaster() => "ant-notification";

    public override string ToasterPlacement( ToasterPlacement placement ) => placement switch
    {
        Blazorise.ToasterPlacement.Top => "ant-notification-top",
        Blazorise.ToasterPlacement.TopStart => "ant-notification-topLeft",
        Blazorise.ToasterPlacement.TopEnd => "ant-notification-topRight",
        Blazorise.ToasterPlacement.Bottom => "ant-notification-bottom",
        Blazorise.ToasterPlacement.BottomStart => "ant-notification-bottomLeft",
        Blazorise.ToasterPlacement.BottomEnd => "ant-notification-bottomRight",
        _ => null,
    };

    public override string ToasterPlacementStrategy( ToasterPlacementStrategy placementStrategy ) => null;

    #endregion

    #region Pagination

    public override string Pagination() => "ant-pagination";

    public override string PaginationSize( Size size ) => size != Size.Default ? $"{Pagination()}-{ToSize( size )}" : null;

    public override string PaginationAlignment( Alignment alignment ) => alignment switch
    {
        Alignment.Start => "ant-pagination-start",
        Alignment.Center => "ant-pagination-center",
        Alignment.End => "ant-pagination-end",
        _ => null,
    };

    public override string PaginationBackgroundColor( Background background ) => background.IsNotNullOrDefault() ? $"bg-{ToBackground( background )}" : null;

    public override string PaginationItem() => "ant-pagination-item";

    public override string PaginationItemActive( bool active ) => active ? "ant-pagination-item-active" : null;

    public override string PaginationItemDisabled( bool disabled ) => disabled ? "ant-pagination-disabled" : null;

    public override string PaginationLink() => null;

    public override string PaginationLinkSize( Size size ) => null;

    public override string PaginationLinkActive( bool active ) => null;

    public override string PaginationLinkDisabled( bool disabled ) => null;

    #endregion

    #region Progress

    public override string Progress() => "ant-progress ant-progress-line";

    public override string ProgressSize( Size size ) => size != Size.Default ? $"progress-{ToSize( size )}" : null;

    public override string ProgressColor( Color color ) => null;

    public override string ProgressStriped( bool stripped ) => null;

    public override string ProgressAnimated( bool animated ) => null;

    public override string ProgressIndeterminate( bool indeterminate ) => indeterminate ? "ant-progress-indeterminate" : null;

    public override string ProgressWidth( int width ) => null;

    public override string ProgressBar() => "ant-progress-bg b-ant-progress-text";

    public override string ProgressBarSize( Size size ) => $"ant-progress-bg-{ToSize( size )}";

    public override string ProgressBarColor( Color color ) => color.IsNotNullOrDefault() ? $"bg-{ToColor( color )}" : null;

    public override string ProgressBarStriped( bool striped ) => striped ? "ant-progress-bar-striped" : null;

    public override string ProgressBarAnimated( bool animated ) => animated ? "ant-progress-bar-animated" : null;

    public override string ProgressBarIndeterminate( bool indeterminate ) => indeterminate ? "ant-progress-bar-indeterminate" : null;

    public override string ProgressBarWidth( int width ) => null;

    public override string PageProgress() => "b-page-progress";

    public override string PageProgressActive( bool visible ) => visible ? "b-page-progress-active" : null;

    public override string PageProgressIndicator() => "b-page-progress-indicator";

    public override string PageProgressIndicatorColor( Color color ) => color.IsNotNullOrDefault() ? $"b-page-progress-indicator-{ToColor( color )}" : null;

    public override string PageProgressIndicatorIndeterminate( bool indeterminate ) => indeterminate ? "b-page-progress-indicator-indeterminate" : null;

    #endregion

    #region Chart

    public override string Chart() => null;

    #endregion

    #region Colors

    public override string BackgroundColor( Background background ) => $"bg-{ToBackground( background )}";

    #endregion

    #region Table

    public override string Table() => "ant-table";

    public override string TableFullWidth( bool fullWidth ) => null;

    public override string TableStriped( bool striped ) => striped ? "ant-table-striped" : null;

    public override string TableHoverable( bool hoverable ) => hoverable ? "ant-table-hover" : null;

    public override string TableBordered( bool bordered ) => bordered ? "ant-table-bordered" : null;

    public override string TableNarrow( bool narrow ) => narrow ? "ant-table-small" : null;

    public override string TableBorderless( bool borderless ) => borderless ? "ant-table-borderless" : null;

    public override string TableHeader() => "ant-table-thead";

    public override string TableHeaderThemeContrast( ThemeContrast themeContrast ) => themeContrast != ThemeContrast.None ? $"ant-table-thead-theme ant-table-thead-{ToThemeContrast( themeContrast )}" : null;

    public override string TableHeaderCell() => null;

    public override string TableHeaderCellCursor( Cursor cursor ) => cursor != Cursor.Default ? $"ant-cursor-{ToCursor( cursor )}" : null;

    public override string TableHeaderCellFixed( TableColumnFixedPosition fixedPosition )
    {
        return fixedPosition switch
        {
            TableColumnFixedPosition.Start => "ant-table-cell-fix-start ant-header-cell-fixed-start",
            TableColumnFixedPosition.End => "ant-table-cell-fix-end ant-header-cell-fixed-end",
            _ => null,
        };
    }

    public override string TableFooter() => null;

    public override string TableBody() => "ant-table-tbody";

    public override string TableRow( bool striped, bool hoverable ) => "ant-table-row";

    public override string TableRowColor( Color color ) => color.IsNotNullOrDefault() ? $"ant-table-{ToColor( color )}" : null;

    public override string TableRowHoverCursor( Cursor cursor ) => cursor != Cursor.Default ? "ant-table-row-selectable" : null;

    public override string TableRowIsSelected( bool selected ) => selected ? "selected" : null;

    public override string TableRowHeader() => "ant-table-cell ant-table-row-header";

    public override string TableRowHeaderFixed( TableColumnFixedPosition fixedPosition )
    {
        return fixedPosition switch
        {
            TableColumnFixedPosition.Start => "ant-table-cell-fix-start ant-row-header-fixed-start",
            TableColumnFixedPosition.End => "ant-table-cell-fix-end ant-row-header-fixed-end",
            _ => null,
        };
    }

    public override string TableRowCell() => "ant-table-cell";

    public override string TableRowCellColor( Color color ) => color.IsNotNullOrDefault() ? $"ant-table-{ToColor( color )}" : null;

    public override string TableRowCellFixed( TableColumnFixedPosition fixedPosition )
    {
        return fixedPosition switch
        {
            TableColumnFixedPosition.Start => "ant-table-cell-fix-start ant-row-cell-fixed-start",
            TableColumnFixedPosition.End => "ant-table-cell-fix-end ant-row-cell-fixed-end",
            _ => null,
        };
    }

    public override string TableRowGroup( bool expanded ) => "ant-table-group";

    public override string TableRowGroupCell() => "ant-table-cell";

    public override string TableRowGroupIndentCell() => "ant-table-cell";

    public override string TableResponsive( bool responsive ) => responsive ? "ant-table-responsive" : null;

    public override string TableFixedHeader( bool fixedHeader ) => fixedHeader ? "ant-table-fixed-header" : null;

    public override string TableFixedColumns( bool fixedColumns ) => fixedColumns ? "ant-table-fixed-columns" : null;

    public override string TableResponsiveMode( TableResponsiveMode responsiveMode ) => responsiveMode == Blazorise.TableResponsiveMode.Mobile ? "ant-table-mobile" : null;

    public override string TableCaption() => "ant-table-caption";

    public override string TableCaptionSide( TableCaptionSide side ) => side != Blazorise.TableCaptionSide.Default ? $"ant-table-caption-{ToTableCaptionSide( side )}" : null;

    #endregion

    #region Badge

    public override string Badge() => "ant-tag";

    public override string BadgeColor( Color color, bool subtle )
    {
        if ( color.IsNullOrDefault() )
            return null;

        return $"{Badge()}-{ToBadgeColorClass( color )} {ToBadgeVariantClass( subtle )}";
    }

    public override string BadgePill( bool pill ) => pill ? $"{Badge()}-pill" : null;

    public override string BadgeClose() => "ant-tag-close-icon";

    public override string BadgeCloseColor( Color color, bool subtle ) => null;

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

    public override string TextDecoration( TextDecoration textDecoration ) => $"ant-typography-decoration-{ToTextDecoration( textDecoration )}";

    public override string TextWeight( TextWeight textWeight ) => $"ant-font-weight-{ToTextWeight( textWeight )}";

    public override string TextOverflow( TextOverflow textOverflow ) => $"ant-typography-{ToTextOverflow( textOverflow )}";

    public override string TextSize( TextSizeType textSizeType, TextSizeDefinition textSizeDefinition )
    {
        if ( textSizeType == TextSizeType.Default )
            return null;

        if ( textSizeDefinition.Breakpoint != Breakpoint.None && textSizeDefinition.Breakpoint != Breakpoint.Mobile )
            return $"ant-font-size-{ToBreakpoint( textSizeDefinition.Breakpoint )}-{ToTextSizeType( textSizeType )}";

        return $"ant-font-size-{ToTextSizeType( textSizeType )}";
    }

    public override string TextItalic( bool italic ) => italic ? "ant-typography-italic" : null;

    #endregion

    #region Code

    public override string Code() => null;

    #endregion

    #region Heading

    public override string HeadingSize( HeadingSize headingSize ) => "ant-typography";

    #endregion

    #region DisplayHeading

    public override string DisplayHeadingSize( DisplayHeadingSize displayHeadingSize ) => $"ant-display-{ToDisplayHeadingSize( displayHeadingSize )}";

    #endregion

    #region Lead

    public override string Lead() => "ant-lead";

    #endregion

    #region Paragraph

    public override string Paragraph() => "ant-typography";

    public override string ParagraphColor( TextColor textColor ) => $"ant-typography-{ToTextColor( textColor )}";

    #endregion

    #region Blockquote

    public override string Blockquote() => "ant-blockquote";

    public override string BlockquoteFooter() => "ant-blockquote-footer";

    #endregion

    #region Figure

    public override string Figure() => "ant-figure";

    public override string FigureSize( FigureSize figureSize ) => figureSize != Blazorise.FigureSize.Default ? $"ant-figure-is-{ToFigureSize( figureSize )}" : null;

    public override string FigureImage() => "ant-figure-img ant-figure-img-fluid";

    public override string FigureImageRounded( bool rounded ) => rounded ? "ant-figure-rounded" : null;

    public override string FigureCaption() => "ant-figure-caption";

    #endregion

    #region Image

    public override string Image() => "ant-image-img";

    public override string ImageFluid( bool fluid ) => fluid ? "ant-image-img-fluid" : null;

    #endregion

    #region Breadcrumb

    public override string Breadcrumb() => "ant-breadcrumb";

    public override string BreadcrumbItem() => null;

    public override string BreadcrumbItemActive( bool active ) => null;

    public override string BreadcrumbLink() => "ant-breadcrumb-link";

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

    public override string Skeleton() => "ant-skeleton ant-skeleton-element";

    public override string SkeletonAnimation( SkeletonAnimation animation ) => animation != Blazorise.SkeletonAnimation.Default ? $"ant-skeleton-active ant-skeleton-{ToSkeletonAnimation( animation )}" : null;

    public override string SkeletonItem() => "ant-skeleton-content";

    #endregion

    #region Divider

    public override string Divider() => "divider";

    public override string DividerType( DividerType dividerType ) => $"{Divider()}-{ToDividerType( dividerType )}";

    #endregion

    #region Link

    public override string Link() => null;

    public override string LinkActive( bool active ) => active ? Active() : null;

    public override string LinkUnstyled( bool unstyled ) => unstyled ? "ant-link-unstyled" : null;

    public override string LinkStretched( bool stretched ) => stretched ? "ant-link-stretched" : null;

    public override string LinkDisabled( bool disabled ) => disabled ? "ant-link-disabled" : null;

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

    #region Gap

    public override string Gap( GapSize gapSize, GapSide gapSide )
    {
        var nativeGapSize = ToNativeGapSize( gapSize );

        if ( nativeGapSize is not null )
        {
            return gapSide switch
            {
                GapSide.None or GapSide.All => $"ant-flex-gap-{nativeGapSize}",
                GapSide.X => $"ant-space-gap-col-{nativeGapSize}",
                GapSide.Y => $"ant-space-gap-row-{nativeGapSize}",
                _ => $"ant-flex-gap-{nativeGapSize}",
            };
        }

        var side = gapSide != GapSide.None && gapSide != GapSide.All
            ? $"{ToGapSide( gapSide )}-"
            : null;

        return $"ant-gap-{side}{ToGapSize( gapSize )}";
    }

    public override string Gap( GapSize gapSize, IEnumerable<GapSide> rules )
        => string.Join( " ", rules.Select( x => Gap( gapSize, x ) ) );

    #endregion

    #region Gutter

    public override string Gutter( GutterSize gutterSize, GutterSide gutterSide, Breakpoint breakpoint )
    {
        var sb = new StringBuilder( "ant-gutter" );

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
        var sb = new StringBuilder( "ant-border" );

        if ( borderDefinition.Side != BorderSide.All )
            sb.Append( '-' ).Append( ToBorderSide( borderDefinition.Side ) );

        if ( borderSize != BorderSize.Default )
            sb.Append( '-' ).Append( ToBorderSize( borderSize ) );

        if ( borderDefinition.Color != BorderColor.None )
        {
            sb.Append( " ant-border-" ).Append( ToBorderColor( borderDefinition.Color ) );

            if ( borderDefinition.Subtle )
                sb.Append( "-subtle" );
        }

        return sb.ToString();
    }

    public override string Border( BorderSize borderSize, IEnumerable<BorderDefinition> rules )
        => string.Join( " ", rules.Select( x => Border( borderSize, x ) ) );

    public override string BorderRadius( BorderRadius borderRadius )
        => $"ant-{ToBorderRadius( borderRadius )}";

    #endregion

    #region Flex

    public override string Flex( FlexType flexType )
    {
        return flexType != FlexType.Default
            ? $"ant-display-{ToFlexType( flexType )}"
            : null;
    }

    public override string Flex( FlexDefinition flexDefinition )
    {
        var sb = new StringBuilder();

        var breakpoint = flexDefinition.Breakpoint != Breakpoint.None && flexDefinition.Breakpoint != Breakpoint.Mobile
            ? $"{ToBreakpoint( flexDefinition.Breakpoint )}-"
            : null;

        if ( flexDefinition.Direction != FlexDirection.Default )
            sb.Append( "ant-flex-direction-" ).Append( breakpoint ).Append( ToDirection( flexDefinition.Direction ) );

        if ( flexDefinition.JustifyContent != FlexJustifyContent.Default )
            sb.Append( "ant-justify-content-" ).Append( breakpoint ).Append( ToJustifyContent( flexDefinition.JustifyContent ) );

        if ( flexDefinition.AlignItems != FlexAlignItems.Default )
            sb.Append( "ant-align-items-" ).Append( breakpoint ).Append( ToAlignItems( flexDefinition.AlignItems ) );

        if ( flexDefinition.AlignSelf != FlexAlignSelf.Default )
            sb.Append( "ant-align-self-" ).Append( breakpoint ).Append( ToAlignSelf( flexDefinition.AlignSelf ) );

        if ( flexDefinition.AlignContent != FlexAlignContent.Default )
            sb.Append( "ant-align-content-" ).Append( breakpoint ).Append( ToAlignContent( flexDefinition.AlignContent ) );

        if ( flexDefinition.GrowShrink != FlexGrowShrink.Default && flexDefinition.GrowShrinkSize != FlexGrowShrinkSize.Default )
            sb.Append( "ant-flex-" ).Append( breakpoint ).Append( ToGrowShrink( flexDefinition.GrowShrink ) ).Append( "-" ).Append( ToGrowShrinkSize( flexDefinition.GrowShrinkSize ) );

        if ( flexDefinition.Basis && flexDefinition.BasisSize != FlexBasisSize.Default )
            sb.Append( "ant-flex-basis-" ).Append( breakpoint ).Append( ToBasisSize( flexDefinition.BasisSize ) );

        if ( flexDefinition.Wrap != FlexWrap.Default )
            sb.Append( "ant-flex-wrap-" ).Append( breakpoint ).Append( ToWrap( flexDefinition.Wrap ) );

        if ( flexDefinition.Order != FlexOrder.Default )
            sb.Append( "ant-flex-order-" ).Append( breakpoint ).Append( ToOrder( flexDefinition.Order ) );

        if ( flexDefinition.Fill )
            sb.Append( "ant-flex-" ).Append( breakpoint ).Append( "fill" );

        return sb.ToString();
    }

    public override string Flex( FlexRule flexRule )
    {
        var sb = new StringBuilder();

        if ( flexRule.FlexType != FlexType.Default )
        {
            if ( flexRule.Breakpoint > Breakpoint.Mobile )
                sb.Append( $"ant-display-{ToBreakpoint( flexRule.Breakpoint )}-{ToFlexType( flexRule.FlexType )}" );
            else
                sb.Append( $"ant-display-{ToFlexType( flexRule.FlexType )}" );

            sb.Append( ' ' );
        }

        sb.Append( string.Join( ' ', flexRule.Definitions.Where( x => x.Condition ?? true ).Select( x => Flex( x ) ) ) );

        return sb.ToString();
    }

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

    #region Overflow

    public override string Overflow( OverflowType overflowType, OverflowType secondOverflowType ) => secondOverflowType != OverflowType.Default
        ? $"ant-overflow-{ToOverflowType( overflowType )}-{ToOverflowType( secondOverflowType )}"
        : $"ant-overflow-{ToOverflowType( overflowType )}";

    #endregion

    #region Position

    public override string Position( PositionType positionType, PositionEdgeType edgeType, int edgeOffset, PositionTranslateType translateType )
    {
        return $"ant-{ToPositionEdgeType( edgeType )}-{edgeOffset}";
    }

    public override string Position( PositionType positionType, IEnumerable<(PositionEdgeType edgeType, int edgeOffset)> edges, PositionTranslateType translateType )
    {
        var sb = new StringBuilder( $"ant-position-{ToPositionType( positionType )}" );

        if ( edges != null && edges.Count() > 0 )
            sb.Append( ' ' ).Append( string.Join( " ", edges.Select( x => Position( positionType, x.edgeType, x.edgeOffset, translateType ) ) ) );

        if ( translateType != PositionTranslateType.None )
            sb.Append( " ant-translate-" ).Append( ToPositionTranslateType( translateType ) );

        return sb.ToString();
    }

    #endregion

    #region ObjectFit

    public override string ObjectFit( ObjectFitType objectFitType, ObjectFitDefinition objectFitDefinition )
    {
        if ( objectFitType == ObjectFitType.Default )
            return null;

        if ( objectFitDefinition.Breakpoint != Breakpoint.None && objectFitDefinition.Breakpoint != Breakpoint.Mobile )
            return $"ant-object-fit-{ToBreakpoint( objectFitDefinition.Breakpoint )}-{ToObjectFitType( objectFitType )}";

        return $"ant-object-fit-{ToObjectFitType( objectFitType )}";
    }

    #endregion

    #region Elements

    public override string UnorderedList() => "ant-unordered-list";

    public override string UnorderedListUnstyled( bool unstyled ) => unstyled ? "ant-unordered-list-unstyled" : null;

    public override string OrderedList() => "ant-ordered-list";

    public override string OrderedListUnstyled( bool unstyled ) => unstyled ? "ant-ordered-list-unstyled" : null;

    public override string OrderedListType( OrderedListType orderedListType ) => orderedListType != Blazorise.OrderedListType.Default ? $"ant-ordered-list-{ToOrderedListType( orderedListType )}" : null;

    public override string DescriptionList() => null;

    public override string DescriptionListTerm() => null;

    public override string DescriptionListDefinition() => null;

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

    public override string ToValidationStatus( ValidationStatus validationStatus )
    {
        return validationStatus switch
        {
            Blazorise.ValidationStatus.Success => "ant-input-status-success",
            Blazorise.ValidationStatus.Error => "ant-input-status-error",
            _ => null,
        };
    }

    #endregion

    public override bool UseCustomInputStyles { get; set; } = true;

    public override string Provider => "AntDesign";

    private string ToButtonColorClass( Color color, bool outline )
    {
        var colorName = color.IsNotNullOrDefault()
            ? ToColor( color )
            : null;

        return colorName switch
        {
            null => "ant-btn-color-default ant-btn-variant-outlined",
            "primary" => $"ant-btn-color-primary ant-btn-variant-{( outline ? "outlined" : "solid" )}",
            "danger" => $"ant-btn-color-dangerous ant-btn-variant-{( outline ? "outlined" : "solid" )}",
            _ => outline
                ? $"{Button( outline )}-outline-{colorName} ant-btn-variant-outlined"
                : $"{Button( outline )}-{colorName} ant-btn-variant-solid",
        };
    }

    private string ToBadgeColorClass( Color color )
    {
        string colorName = color.IsNotNullOrDefault()
            ? ToColor( color )
            : null;

        return colorName switch
        {
            "danger" => "error",
            "info" => "processing",
            _ => colorName,
        };
    }

    private static string ToBadgeVariantClass( bool subtle ) => subtle ? "ant-tag-outlined" : "ant-tag-solid";

    private static string ToValidationStatus( ValidationStatus validationStatus, string prefix )
    {
        string suffix = validationStatus switch
        {
            Blazorise.ValidationStatus.Success => "success",
            Blazorise.ValidationStatus.Error => "error",
            _ => null,
        };

        return suffix is null
            ? null
            : $"{prefix}-status-{suffix}";
    }

    private static string ToNativeGapSize( GapSize gapSize )
    {
        return gapSize switch
        {
            GapSize.Is2 => "small",
            GapSize.Is3 => "middle",
            GapSize.Is4 => "large",
            _ => null,
        };
    }
}