#region Using directives
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Blazorise.Extensions;
#endregion

namespace Blazorise.FluentUI2;

public class FluentUI2ClassProvider : ClassProvider
{
    #region TextEdit

    public override string TextEdit( bool plaintext ) => plaintext ? "fui-Input__input-plaintext" : "fui-Input__input";

    public override string TextEditSize( Size size ) => size != Size.Default ? $"fui-Input__input-{ToSize( size )}" : null;

    public override string TextEditColor( Color color ) => color.IsNotNullOrDefault() ? $"fui-TextColor-{ToColor( color )}" : null;

    public override string TextEditValidation( ValidationStatus validationStatus ) => validationStatus == ValidationStatus.None ? null : $"fui-Input__input-{ToValidationStatus( validationStatus )}";

    #endregion

    #region MemoEdit

    public override string MemoEdit( bool plaintext ) => plaintext ? "fui-Textarea__input-plaintext" : "fui-Textarea__input";

    public override string MemoEditSize( Size size ) => size != Size.Default ? $"fui-Textarea__input-{ToSize( size )}" : null;

    public override string MemoEditValidation( ValidationStatus validationStatus ) => validationStatus != ValidationStatus.None ? $"fui-Textarea__input-{ToValidationStatus( validationStatus )}" : null;

    #endregion

    #region Select

    public override string Select() => "fui-Select__select";

    public override string SelectMultiple( bool multiple ) => null;

    public override string SelectSize( Size size ) => size != Size.Default ? $"{Select()}-{ToSize( size )}" : null;

    public override string SelectValidation( ValidationStatus validationStatus ) => validationStatus != ValidationStatus.None ? $"fui-Select__select-{ToValidationStatus( validationStatus )}" : null;

    #endregion

    #region NumericEdit

    public override string NumericEdit( bool plaintext ) => plaintext ? "fui-NumericInput fui-Input__input-plaintext" : "fui-NumericInput fui-Input__input";

    public override string NumericEditSize( Size size ) => size != Size.Default ? $"fui-Input__input-{ToSize( size )}" : null;

    public override string NumericEditColor( Color color ) => color.IsNotNullOrDefault() ? $"fui-TextColor-{ToColor( color )}" : null;

    public override string NumericEditValidation( ValidationStatus validationStatus ) => validationStatus != ValidationStatus.None ? $"fui-Input__input-{ToValidationStatus( validationStatus )}" : null;

    #endregion

    #region DateEdit

    public override string DateEdit( bool plaintext ) => plaintext ? "fui-DateInput fui-Input__input-plaintext" : "fui-DateInput fui-Input__input";

    public override string DateEditSize( Size size ) => size != Size.Default ? $"fui-Input__input-{ToSize( size )}" : null;

    public override string DateEditColor( Color color ) => color.IsNotNullOrDefault() ? $"fui-TextColor-{ToColor( color )}" : null;

    public override string DateEditValidation( ValidationStatus validationStatus ) => validationStatus != ValidationStatus.None ? $"fui-Input__input-{ToValidationStatus( validationStatus )}" : null;

    #endregion

    #region TimeEdit

    public override string TimeEdit( bool plaintext ) => plaintext ? "fui-TimeInput fui-Input__input-plaintext" : "fui-TimeInput fui-Input__input";

    public override string TimeEditSize( Size size ) => size != Size.Default ? $"fui-Input__input-{ToSize( size )}" : null;

    public override string TimeEditColor( Color color ) => color.IsNotNullOrDefault() ? $"fui-TextColor-{ToColor( color )}" : null;

    public override string TimeEditValidation( ValidationStatus validationStatus ) => validationStatus != ValidationStatus.None ? $"fui-Input__input-{ToValidationStatus( validationStatus )}" : null;

    #endregion

    #region ColorEdit

    public override string ColorEdit() => "fui-ColorInput fui-Input__input";

    public override string ColorEditSize( Size size ) => size != Size.Default ? $"fui-Input__input-{ToSize( size )}" : null;

    #endregion

    #region DatePicker

    public override string DatePicker( bool plaintext ) => plaintext ? "fui-DatePicker fui-Input__input-plaintext" : "fui-DatePicker fui-Input__input";

    public override string DatePickerSize( Size size ) => size != Size.Default ? $"fui-Input__input-{ToSize( size )}" : null;

    public override string DatePickerColor( Color color ) => color.IsNotNullOrDefault() ? $"fui-TextColor-{ToColor( color )}" : null;

    public override string DatePickerValidation( ValidationStatus validationStatus ) => validationStatus != ValidationStatus.None ? $"fui-Input__input-{ToValidationStatus( validationStatus )}" : null;

    #endregion

    #region TimePicker

    public override string TimePicker( bool plaintext ) => plaintext ? "fui-TimePicker fui-Input__input-plaintext" : "fui-TimePicker fui-Input__input";

    public override string TimePickerSize( Size size ) => size != Size.Default ? $"fui-Input__input-{ToSize( size )}" : null;

    public override string TimePickerColor( Color color ) => color.IsNotNullOrDefault() ? $"fui-TextColor-{ToColor( color )}" : null;

    public override string TimePickerValidation( ValidationStatus validationStatus ) => validationStatus != ValidationStatus.None ? $"fui-Input__input-{ToValidationStatus( validationStatus )}" : null;

    #endregion

    #region ColorPicker

    public override string ColorPicker() => "fui-ColorPicker fui-Input__input b-input-color-picker";

    public override string ColorPickerSize( Size size ) => size != Size.Default ? $"fui-Input__input-{ToSize( size )}" : null;

    #endregion

    #region NumericPicker

    public override string NumericPicker( bool plaintext ) => plaintext ? "fui-NumericPicker fui-SpinButton__input-plaintext" : "fui-NumericInput fui-SpinButton__input";

    public override string NumericPickerSize( Size size ) => size != Size.Default ? $"fui-SpinButton__input-{ToSize( size )}" : null;

    public override string NumericPickerColor( Color color ) => color.IsNotNullOrDefault() ? $"fui-TextColor-{ToColor( color )}" : null;

    public override string NumericPickerValidation( ValidationStatus validationStatus ) => validationStatus != ValidationStatus.None ? $"fui-SpinButton-{ToValidationStatus( validationStatus )}" : null;

    #endregion

    #region InputMask

    public override string InputMask( bool plaintext ) => plaintext ? "fui-InputMask fui-Input__input-plaintext" : "fui-InputMask fui-Input__input";

    public override string InputMaskSize( Size size ) => size != Size.Default ? $"fui-Input__input-{ToSize( size )}" : null;

    public override string InputMaskColor( Color color ) => color.IsNotNullOrDefault() ? $"fui-TextColor-{ToColor( color )}" : null;

    public override string InputMaskValidation( ValidationStatus validationStatus ) => validationStatus != ValidationStatus.None ? $"fui-Input__input-{ToValidationStatus( validationStatus )}" : null;

    #endregion

    #region Check

    public override string Check() => "fui-Checkbox__input";

    public override string CheckSize( Size size ) => size != Size.Default ? $"{Check()}-{ToSize( size )}" : null;

    public override string CheckInline( bool inline ) => inline ? "fui-Checkbox__input-inline" : null;

    public override string CheckCursor( Cursor cursor ) => cursor != Cursor.Default ? $"{Check()}-{ToCursor( cursor )}" : null;

    public override string CheckValidation( ValidationStatus validationStatus ) => validationStatus != ValidationStatus.None ? $"fui-Input__input-{ToValidationStatus( validationStatus )}" : null;

    #endregion

    #region RadioGroup

    public override string RadioGroup( bool buttons, Orientation orientation ) => buttons
        ? orientation == Orientation.Horizontal ? "fui-RadioGroup__horizontal-buttons" : "fui-RadioGroup__buttons"
        : orientation == Orientation.Horizontal ? "fui-RadioGroup__horizontal" : "fui-RadioGroup";

    public override string RadioGroupSize( bool buttons, Orientation orientation, Size size )
    {
        if ( size == Size.Default )
            return null;

        return buttons
            ? orientation == Orientation.Horizontal ? $"fui-RadioGroup__horizontal-buttons-{ToSize( size )}" : $"fui-RadioGroup__buttons-{ToSize( size )}"
            : orientation == Orientation.Horizontal ? $"fui-RadioGroup__horizontal-{ToSize( size )}" : $"fui-RadioGroup-{ToSize( size )}";
    }

    public override string RadioGroupValidation( ValidationStatus validationStatus ) => validationStatus != ValidationStatus.None ? $"fui-RadioGroup-{ToValidationStatus( validationStatus )}" : null;

    #endregion

    #region Radio

    public override string Radio( bool button ) => "fui-Radio__input";

    public override string RadioSize( bool button, Size size ) => size != Size.Default ? $"fui-Radio__input-{ToSize( size )}" : null;

    public override string RadioInline( bool inline ) => inline
        ? "fui-Radio__input-inline"
        : null;

    public override string RadioCursor( Cursor cursor ) => cursor != Cursor.Default ? $"fui-Radio__input-{ToCursor( cursor )}" : null;

    public override string RadioValidation( ValidationStatus validationStatus ) => validationStatus != ValidationStatus.None ? $"fui-Radio__input-{ToValidationStatus( validationStatus )}" : null;

    #endregion

    #region Switch

    public override string Switch() => "fui-Switch__input";

    public override string SwitchColor( Color color ) => color.IsNotNullOrDefault() ? $"{Switch()}-{ToColor( color )}" : null;

    public override string SwitchSize( Size size ) => size != Size.Default ? $"fui-Switch__input-{ToSize( size )}" : null;

    public override string SwitchChecked( bool @checked ) => null;

    public override string SwitchCursor( Cursor cursor ) => cursor != Cursor.Default ? $"{Switch()}-{ToCursor( cursor )}" : null;

    public override string SwitchValidation( ValidationStatus validationStatus ) => validationStatus != ValidationStatus.None ? $"fui-Switch__input-{ToValidationStatus( validationStatus )}" : null;

    #endregion

    #region FileEdit

    public override string FileEdit() => "fui-Input__input";

    public override string FileEditSize( Size size ) => size != Size.Default ? $"{FileEdit()}-{ToSize( size )}" : null;

    public override string FileEditValidation( ValidationStatus validationStatus ) => validationStatus != ValidationStatus.None ? $"fui-Input__input-{ToValidationStatus( validationStatus )}" : null;

    #endregion

    #region Slider

    public override string Slider() => "fui-Slider__input";

    public override string SliderColor( Color color ) => $"fui-Slider__input-{ToColor( color )}";

    public override string SliderValidation( ValidationStatus validationStatus ) => validationStatus != ValidationStatus.None ? $"fui-Slider__input-{ToValidationStatus( validationStatus )}" : null;

    #endregion

    #region Rating

    public override string Rating() => "fui-Rating";

    public override string RatingDisabled( bool disabled ) => disabled ? "fui-Rating-disabled" : null;

    public override string RatingReadonly( bool @readonly ) => @readonly ? "fui-Rating-readonly" : null;

    public override string RatingItem() => "fui-RatingItem";

    public override string RatingItemColor( Color color ) => color.IsNotNullOrDefault() ? $"fui-RatingItem-{ToColor( color )}" : null;

    public override string RatingItemSelected( bool selected ) => selected ? "fui-RatingItem-selected" : null;

    public override string RatingItemHovered( bool hover ) => hover ? "fui-RatingItem-hover" : null;

    #endregion

    #region Label

    public override string LabelType( LabelType labelType ) => labelType switch
    {
        Blazorise.LabelType.Check or Blazorise.LabelType.Radio or Blazorise.LabelType.Switch => "fui-Checkbox__label",
        Blazorise.LabelType.File => "fui-File__label",
        _ => "fui-Label",
    };

    public override string LabelCursor( Cursor cursor ) => cursor != Cursor.Default ? $"fui-Label-{ToCursor( cursor )}" : null;

    #endregion

    #region Help

    public override string Help() => "fui-Field__hint fui-TextColor-muted";

    #endregion

    #region Validation

    public override string ValidationSuccess() => "fui-Field__validationMessage";

    public override string ValidationSuccessTooltip() => "fui-Field__validationTooltip";

    public override string ValidationError() => "fui-Field__validationMessage";

    public override string ValidationErrorTooltip() => "fui-Field__validationTooltip";

    public override string ValidationNone() => "fui-Field__hint fui-TextColor-muted";

    public override string ValidationSummary() => "fui-TextColor-danger";

    public override string ValidationSummaryError() => "fui-TextColor-danger";

    #endregion

    #region Fields

    public override string Fields() => "fui-Fields";

    public override string FieldsBody() => null;

    public override string FieldsColumn() => "fui-Column";

    #endregion

    #region Field

    public override string Field() => "fui-Field";

    public override string FieldHorizontal( bool horizontal ) => horizontal ? "fui-FieldHorizontal" : null;

    public override string FieldColumn() => "fui-Column";

    public override string FieldSize( Size size ) => null;

    public override string FieldJustifyContent( JustifyContent justifyContent ) => ToJustifyContent( justifyContent );

    public override string FieldValidation( ValidationStatus validationStatus ) => null;

    #endregion

    #region FieldLabel

    public override string FieldLabel( bool horizontal ) => horizontal ? "fui-Label fui-LabelHorizontal" : "fui-Label";

    public override string FieldLabelRequiredIndicator( bool requiredIndicator )
        => requiredIndicator
            ? "fui-Label__required"
            : null;

    public override string FieldLabelScreenreader( Screenreader screenreader ) => screenreader != Screenreader.Always ? ToScreenreader( screenreader ) : null;

    #endregion

    #region FieldBody

    public override string FieldBody() => null;

    #endregion

    #region FieldHelp

    public override string FieldHelp() => "fui-Field__hint";

    #endregion

    #region FocusTrap

    public override string FocusTrap() => "fui-FocusTrap";

    #endregion

    #region Control

    public override string ControlCheck( ControlRole role ) => null;

    public override string ControlRadio( ControlRole role ) => null;

    public override string ControlSwitch( ControlRole role ) => null;

    public override string ControlFile( ControlRole role ) => null;

    public override string ControlText( ControlRole role ) => null;

    public override string ControlInline( ControlRole role, bool inline ) => ( role == ControlRole.Check || role == ControlRole.Radio || role == ControlRole.Switch ) && inline ? "fui-Checkbox__input-inline" : null;

    #endregion

    #region Addons

    public override string Addons() => "fui-Input";

    public override string AddonsSize( Size size ) => size != Size.Default ? $"fui-Input-{ToSize( size )}" : null;

    public override string AddonsHasButton( bool hasButton ) => null;

    public override string Addon( AddonType addonType )
    {
        return addonType switch
        {
            AddonType.Start => "fui-Input__contentBefore",
            AddonType.End => "fui-Input__contentAfter",
            _ => null,
        };
    }

    public override string AddonSize( Size size ) => null;

    public override string AddonLabel() => "fui-Text";

    #endregion

    #region Inline

    public override string Inline() => "fui-Fields-inline";

    #endregion

    #region Button

    public override string Button( bool outline ) => "fui-Button";

    public override string ButtonColor( Color color, bool outline ) => outline
        ? color.IsNotNullOrDefault() ? $"{Button( outline )}Outline-{ToColor( color )}" : $"{Button( outline )}Outline"
        : color.IsNotNullOrDefault() ? $"{Button( outline )}-{ToColor( color )}" : null;

    public override string ButtonSize( Size size, bool outline ) => size == Size.Default ? null : $"fui-Button-{ToSize( size )}";

    public override string ButtonBlock( bool outline, bool block ) => block ? $"{Button( outline )}-block" : null;

    public override string ButtonActive( bool outline, bool active ) => active ? "fui-Button-active" : null;

    public override string ButtonDisabled( bool outline, bool disabled ) => disabled ? "fui-Button-disabled" : null;

    public override string ButtonLoading( bool outline, bool loading ) => loading ? "fui-Button-loading" : null;

    public override string ButtonStretchedLink( bool stretched ) => stretched ? "fui-Button-link-stretched" : null;

    #endregion

    #region Buttons

    public override string Buttons( ButtonsRole role, Orientation orientation )
    {
        if ( role == ButtonsRole.Toolbar )
            return "fui-ButtonGroup-toolbar";

        if ( orientation == Orientation.Vertical )
            return "fui-ButtonGroup-vertical";

        return "fui-ButtonGroup";
    }

    public override string ButtonsSize( Size size ) => size != Size.Default ? $"fui-ButtonGroup-{ToSize( size )}" : null;

    #endregion

    #region CloseButton

    public override string CloseButton() => "fui-CloseButton";

    #endregion

    #region Dropdown

    public override string Dropdown( bool isDropdownSubmenu ) => "fui-Menu";

    public override string DropdownDisabled( bool disabled ) => disabled ? "fui-MenuButton-disabled" : null;

    public override string DropdownGroup( bool group ) => group ? "fui-ButtonGroup" : null;

    public override string DropdownObserverShow() => "fui-MenuButton-show";

    public override string DropdownShow( bool show ) => show ? "fui-MenuButton-show" : null;

    public override string DropdownRight( bool rightAligned ) => null;

    public override string DropdownItem() => "fui-MenuItem";

    public override string DropdownItemActive( bool active ) => active ? "fui-MenuItem-active" : null;

    public override string DropdownItemDisabled( bool disabled ) => disabled ? "fui-MenuItem-disabled" : null;

    public override string DropdownDivider() => "fui-MenuDivider";

    public override string DropdownHeader() => "fui-MenuGroup";

    public override string DropdownMenu() => "fui-MenuPopover";

    public override string DropdownMenuPositionStrategy( DropdownPositionStrategy dropdownPositionStrategy )
        => $"fui-MenuPopover-position-strategy {( dropdownPositionStrategy == DropdownPositionStrategy.Fixed ? "fui-MenuPopover-position-strategy-fixed" : "fui-MenuPopover-position-strategy-absolute" )}";

    public override string DropdownFixedHeaderVisible( bool visible )
        => visible ? "dropdown-table-fixed-header-visible" : null;

    public override string DropdownMenuSelector() => "fui-MenuPopover";

    public override string DropdownMenuScrollable( bool scrollable ) => scrollable ? "fui-MenuPopover-scrollable" : null;

    public override string DropdownMenuVisible( bool visible ) => visible ? "fui-MenuPopover-show" : null;

    public override string DropdownMenuRight( bool rightAligned ) => rightAligned ? "fui-MenuPopover-right" : null;

    public override string DropdownToggle( bool isDropdownSubmenu, bool outline ) => isDropdownSubmenu
        ? "fui-MenuItem"
        : "fui-Button fui-MenuButton";

    public override string DropdownToggleSelector( bool isDropdownSubmenu ) => isDropdownSubmenu
        ? "fui-MenuItem"
        : "fui-Button fui-MenuButton";

    public override string DropdownToggleColor( Color color, bool outline ) => outline
        ? color.IsNotNullOrDefault() ? $"fui-ButtonOutline-{ToColor( color )}" : $"fui-ButtonOutline"
        : color.IsNotNullOrDefault() ? $"fui-Button-{ToColor( color )}" : null;

    public override string DropdownToggleSize( Size size, bool outline )
        => size != Size.Default ? $"fui-MenuButton-{ToSize( size )}" : null;

    public override string DropdownToggleSplit( bool split ) => split ? "fui-SplitButton__menuButton" : null;

    public override string DropdownToggleIcon( bool visible ) => visible ? null : "fui-MenuButton-toggle-hidden";

    public override string DropdownDirection( Direction direction ) => direction switch
    {
        Direction.Up => "fui-Menu-up",
        Direction.End => "fui-Menu-end",
        Direction.Start => "fui-Menu-start",
        _ => "fui-Menu-down",
    };

    #endregion

    //#region Dropdown

    //public override string Dropdown( bool isDropdownSubmenu ) => "fui-Dropdown";

    //public override string DropdownDisabled() => "fui-Dropdown-disabled";

    //public override string DropdownGroup() => "btn-group";

    //public override string DropdownObserverShow() => DropdownShow();

    //public override string DropdownShow() => "fui-Dropdown-show";

    //public override string DropdownRight() => null;

    //public override string DropdownItem() => "fui-Option";

    //public override string DropdownItemActive( bool active ) => active ? "fui-Option-active" : null;

    //public override string DropdownItemDisabled( bool disabled ) => disabled ? "fui-Option-disabled" : null;

    //public override string DropdownDivider() => "fui-Option__divider";

    //public override string DropdownHeader() => "fui-OptionGroup__label";

    //public override string DropdownMenu() => "fui-Listbox fui-Dropdown__listbox";

    //public override string DropdownMenuPositionStrategy( DropdownPositionStrategy dropdownPositionStrategy )
    //    => $"fui-Dropdown__listbox-position-strategy {( dropdownPositionStrategy == DropdownPositionStrategy.Fixed ? "fui-Dropdown__listbox-position-strategy-fixed" : "fui-Dropdown__listbox-position-strategy-absolute" )}";

    //public override string DropdownFixedHeaderVisible( bool visible )
    //    => visible ? "dropdown-table-fixed-header-visible" : null;

    //public override string DropdownMenuSelector() => "dropdown-menu";

    //public override string DropdownMenuScrollable() => "dropdown-menu-scrollable";

    ////public override string DropdownMenuBody() => null;

    //public override string DropdownMenuVisible( bool visible ) => visible ? "fui-Dropdown__listbox-show" : null;

    //public override string DropdownMenuRight() => "dropdown-menu-right";

    //public override string DropdownToggle( bool isDropdownSubmenu, bool outline ) => isDropdownSubmenu ? "dropdown-item dropdown-toggle" : "fui-Dropdown__button";

    //public override string DropdownToggleSelector( bool isDropdownSubmenu ) => isDropdownSubmenu ? "dropdown-item dropdown-toggle" : "fui-Dropdown__button";

    //public override string DropdownToggleColor( Color color, bool outline ) => outline
    //    ? color.IsNotNullOrDefault() ? $"btn-outline-{ToColor( color )}" : $"btn-outline"
    //    : color.IsNotNullOrDefault() ? $"btn-{ToColor( color )}" : null;

    //public override string DropdownToggleSize( Size size, bool outline )
    //    => size != Size.Default ? $"btn-{ToSize( size )}" : null;

    //public override string DropdownToggleSplit( bool split ) => split ? "dropdown-toggle-split" : null;

    //public override string DropdownToggleIcon( bool visible ) => visible ? null : "dropdown-toggle-hidden";

    //public override string DropdownDirection( Direction direction ) => direction switch
    //{
    //    Direction.Up => "dropup",
    //    Direction.End => "dropright",
    //    Direction.Start => "dropleft",
    //    _ => null,
    //};

    //#endregion

    #region Tabs

    public override string Tabs( bool pills ) => "fui-TabList";

    public override string TabsCards( bool cards ) => null;

    public override string TabsFullWidth( bool fullWidth ) => fullWidth ? "fui-TabList-fill" : null;

    public override string TabsJustified( bool justified ) => justified ? "fui-TabList-justified" : null;

    public override string TabsVertical( bool vertical ) => null;

    public override string TabItem( TabPosition tabPosition )
    {
        return tabPosition switch
        {
            TabPosition.Start => "fui-Tab fui-Tab-start",
            TabPosition.End => "fui-Tab fui-Tab-end",
            TabPosition.Bottom => "fui-Tab fui-Tab-bottom",
            _ => "fui-Tab fui-Tab-top",
        };
    }

    public override string TabItemActive( bool active ) => active ? "fui-Tab-active" : null;

    public override string TabItemDisabled( bool disabled ) => disabled ? "fui-Tab-disabled" : null;

    public override string TabLink( TabPosition tabPosition ) => "fui-Tab__content";

    public override string TabLinkActive( bool active ) => active ? "fui-Tab__content-active" : null;

    public override string TabLinkDisabled( bool disabled ) => disabled ? "fui-Tab__content-disabled" : null;

    public override string TabsContent() => "fui-TabPanels";

    public override string TabPanel() => "fui-TabPanel";

    public override string TabPanelActive( bool active ) => active ? "fui-TabPanel-active" : null;

    #endregion

    #region Steps

    public override string Steps() => "fui-Steps";

    public override string StepItem() => "fui-Step";

    public override string StepItemActive( bool active ) => active ? "fui-Step-active" : null;

    public override string StepItemCompleted( bool completed ) => completed ? "fui-Step-completed" : null;

    public override string StepItemColor( Color color ) => color.IsNotNullOrDefault() ? $"{StepItem()}-{ToColor( color )}" : null;

    public override string StepItemMarker() => "fui-Step__circle";

    public override string StepItemMarkerColor( Color color, bool active ) => null;

    public override string StepItemDescription() => "fui-Step__text";

    public override string StepsContent() => "fui-StepPanels";

    public override string StepPanel() => "fui-StepPanel";

    public override string StepPanelActive( bool active ) => active ? "fui-StepPanel-active" : null;

    #endregion

    #region Carousel

    public override string Carousel() => "fui-Carousel";

    public override string CarouselSlides() => "fui-Carousel__itemscontainer";

    public override string CarouselSlide() => "fui-Carousel__item";

    public override string CarouselSlideActive( bool active ) => active ? "fui-Carousel__item-active" : null;

    public override string CarouselSlideIndex( int activeSlideIndex, int slideindex, int totalSlides ) => null;

    public override string CarouselSlideSlidingLeft( bool left ) => left ? "fui-Carousel__item-left" : null;

    public override string CarouselSlideSlidingRight( bool right ) => right ? "fui-Carousel__item-right" : null;

    public override string CarouselSlideSlidingPrev( bool previous ) => previous ? "fui-Carousel__item-prev" : null;

    public override string CarouselSlideSlidingNext( bool next ) => next ? "fui-Carousel__item-next" : null;

    public override string CarouselIndicators() => "fui-Carousel__navigation";

    public override string CarouselIndicator() => "fui-Carousel__navigationitem";

    public override string CarouselIndicatorActive( bool active ) => active ? "fui-Carousel__navigationitem-active" : null;

    public override string CarouselFade( bool fade ) => fade ? "fui-Carousel-fade" : null;

    public override string CarouselCaption() => "fui-Carousel__caption";

    #endregion

    #region Jumbotron

    public override string Jumbotron() => "fui-Jumbotron";

    public override string JumbotronBackground( Background background ) => background.IsNotNullOrDefault() ? $"fui-Jumbotron-{ToBackground( background )}" : null;

    public override string JumbotronTitle( JumbotronTitleSize jumbotronTitleSize ) => $"fui-Jumbotron__title-{ToJumbotronTitleSize( jumbotronTitleSize )}";

    public override string JumbotronSubtitle() => "fui-Jumbotron__subtitle";

    #endregion

    #region Card

    public override string CardDeck() => "fui-CardDeck";

    public override string CardGroup() => "fui-CardGroup";

    public override string Card() => "fui-Card";

    public override string CardWhiteText( bool whiteText ) => whiteText ? "fui-TextColor-white" : null;

    public override string CardActions() => "fui-CardActions";

    public override string CardBody() => "fui-CardBody";

    public override string CardFooter() => "fui-CardFooter";

    public override string CardHeader() => "fui-CardHeader";

    public override string CardImage() => "fui-CardPreview";

    public override string CardTitle( bool insideHeader ) => "fui-CardHeader__header";

    public override string CardTitleSize( bool insideHeader, int? size ) => null;

    public override string CardSubtitle( bool insideHeader ) => "card-subtitle";

    public override string CardSubtitleSize( bool insideHeader, int size ) => null;

    public override string CardText() => "fui-CardText";

    public override string CardLink() => "fuiCardLink";

    public override string CardLinkUnstyled( bool unstyled ) => LinkUnstyled( unstyled );

    public override string CardLinkActive( bool active ) => LinkActive( active );

    #endregion

    #region ListGroup

    public override string ListGroup() => "fui-ListGroup";

    public override string ListGroupFlush( bool flush ) => flush ? "fui-ListGroup-flush" : null;

    public override string ListGroupScrollable( bool scrollable ) => scrollable ? "fui-ListGroup-scrollable" : null;

    public override string ListGroupItem() => "fui-ListGroupItem";

    public override string ListGroupItemSelectable( bool selectable ) => selectable ? "fui-ListGroupItem-action" : null;

    public override string ListGroupItemActive( bool active ) => active ? "fui-ListGroupItem-active" : null;

    public override string ListGroupItemDisabled( bool disabled ) => disabled ? "fui-ListGroupItem-disabled" : null;

    public override string ListGroupItemColor( Color color, bool selectable, bool active ) => color.IsNotNullOrDefault() ? $"{ListGroupItem()}-{base.ToColor( color )}" : null;

    #endregion

    #region Container

    public override string Container( Breakpoint breakpoint )
        => breakpoint != Breakpoint.None && breakpoint != Breakpoint.Mobile ? $"fui-Container-{ToBreakpoint( breakpoint )}" : "fui-Container";

    public override string ContainerFluid() => "fui-Container-fluid";

    #endregion

    #region Bar

    public override string Bar( BarMode mode ) => mode == Blazorise.BarMode.Horizontal ? "fui-NavigationBar" : "navbar";

    public override string BarInitial( BarMode mode, bool initial ) => mode != Blazorise.BarMode.Horizontal && initial ? "b-bar-initial" : null;

    public override string BarAlignment( BarMode mode, Alignment alignment ) => alignment != Alignment.Default ? $"fui-JustifyContent-{ToAlignment( alignment )}" : null;

    public override string BarThemeContrast( BarMode mode, ThemeContrast themeContrast )
    {
        if ( themeContrast == ThemeContrast.None )
            return null;

        return mode == Blazorise.BarMode.Horizontal
            ? $"fui-NavigationBar-{ToThemeContrast( themeContrast )} b-bar-{ToThemeContrast( themeContrast )}"
            : $"navbar-{ToThemeContrast( themeContrast )} b-bar-{ToThemeContrast( themeContrast )}";
    }

    public override string BarBreakpoint( BarMode mode, Breakpoint breakpoint )
    {
        if ( breakpoint == Breakpoint.None )
            return null;

        return mode == Blazorise.BarMode.Horizontal
            ? breakpoint != Breakpoint.None && breakpoint != Breakpoint.Mobile ? $"fui-NavigationBar-expand-{ToBreakpoint( breakpoint )}" : null
            : breakpoint != Breakpoint.None && breakpoint != Breakpoint.Mobile ? $"navbar-expand-{ToBreakpoint( breakpoint )}" : null;
    }

    public override string BarMode( BarMode mode ) => mode == Blazorise.BarMode.Horizontal
        ? $"fui-NavigationBar-{ToBarMode( mode )}"
        : $"b-bar-{ToBarMode( mode )}";

    public override string BarItem( BarMode mode, bool hasDropdown ) => mode == Blazorise.BarMode.Horizontal
        ? hasDropdown
            ? "fui-NavigationBar__dropdown"
            : "fui-NavigationBar__item"
        : "b-bar-item";

    public override string BarItemActive( BarMode mode, bool active ) => active ? "fui-NavigationBar__item-active" : null;

    public override string BarItemDisabled( BarMode mode, bool disabled ) => disabled ? "fui-NavigationBar__item-disabled" : null;

    public override string BarItemHasDropdown( BarMode mode, bool hasDropdown ) => null;

    public override string BarLink( BarMode mode ) => mode == Blazorise.BarMode.Horizontal
        ? "fui-NavigationBar__link"
        : "b-bar-link";

    public override string BarLinkDisabled( BarMode mode, bool disabled ) => disabled
        ? mode == Blazorise.BarMode.Horizontal
            ? "fui-NavigationBar__link-disabled"
            : Disabled()
        : null;

    public override string BarBrand( BarMode mode ) => mode == Blazorise.BarMode.Horizontal
        ? "fui-NavigationBar__brand"
        : "b-bar-brand";

    public override string BarToggler( BarMode mode, BarTogglerMode togglerMode ) => mode == Blazorise.BarMode.Horizontal
        ? "fui-NavigationBar__toggler"
        : togglerMode == BarTogglerMode.Popout
            ? "b-bar-toggler-popout"
            : "b-bar-toggler-inline";

    public override string BarTogglerCollapsed( BarMode mode, BarTogglerMode togglerMode, bool isShow ) =>
        isShow || mode != Blazorise.BarMode.Horizontal ? null : "collapsed";

    public override string BarMenu( BarMode mode ) => mode == Blazorise.BarMode.Horizontal
        ? "fui-NavigationBar__menu"
        : "b-bar-menu";

    public override string BarMenuShow( BarMode mode, bool show ) => show
        ? mode == Blazorise.BarMode.Horizontal
            ? "fui-NavigationBar__menu-show"
            : Show()
        : null;

    public override string BarStart( BarMode mode ) => mode == Blazorise.BarMode.Horizontal
        ? "fui-NavigationBar__start"
        : "b-bar-start";

    public override string BarEnd( BarMode mode ) => mode == Blazorise.BarMode.Horizontal
        ? "fui-NavigationBar__end"
        : "b-bar-end";

    public override string BarDropdown( BarMode mode, bool isBarDropDownSubmenu ) => mode == Blazorise.BarMode.Horizontal
        ? isBarDropDownSubmenu ? "fui-NavigationBar__subdropdown" : "fui-NavigationBar__dropdown"
        : "b-bar-dropdown";

    public override string BarDropdownShow( BarMode mode, bool show ) => show
        ? mode == Blazorise.BarMode.Horizontal
            ? "fui-NavigationBar__dropdown-show"
            : Show()
        : null;

    public override string BarDropdownToggle( BarMode mode, bool isBarDropDownSubmenu ) => mode == Blazorise.BarMode.Horizontal
        ? isBarDropDownSubmenu
            ? "fui-NavigationBar__dropdown-item"
            : "fui-NavigationBar__link fui-NavigationBar__dropdown-toggle"
        : "b-bar-link b-bar-dropdown-toggle";

    public override string BarDropdownToggleDisabled( BarMode mode, bool isBarDropDownSubmenu, bool disabled ) => mode == Blazorise.BarMode.Horizontal && disabled
        ? "disabled"
        : null;

    public override string BarDropdownItem( BarMode mode ) => mode == Blazorise.BarMode.Horizontal
        ? "fui-NavigationBar__dropdown-item"
        : "b-bar-dropdown-item";

    public override string BarTogglerIcon( BarMode mode ) => mode == Blazorise.BarMode.Horizontal
        ? "fui-NavigationBar__toggler-icon"
        : "navbar-toggler-icon";

    public override string BarDropdownDivider( BarMode mode ) => mode == Blazorise.BarMode.Horizontal
        ? "fui-NavigationBar__dropdown-divider"
        : "dropdown-divider";

    public override string BarDropdownMenu( BarMode mode ) => mode == Blazorise.BarMode.Horizontal
        ? "fui-NavigationBar__dropdown-menu"
        : "b-bar-dropdown-menu";

    public override string BarDropdownMenuVisible( BarMode mode, bool visible ) => mode == Blazorise.BarMode.Horizontal
        ? visible ? "fui-NavigationBar__dropdown-menu-show" : null
        : visible ? "dropdown-menu-show" : null;

    public override string BarDropdownMenuRight( BarMode mode, bool rightAligned ) => rightAligned
        ? mode == Blazorise.BarMode.Horizontal
            ? "fui-NavigationBar__dropdown-right"
            : "b-bar-right"
        : null;

    public override string BarDropdownMenuContainer( BarMode mode ) => mode == Blazorise.BarMode.Horizontal ?
        null :
        "b-bar-dropdown-menu-container";

    public override string BarCollapsed( BarMode mode, bool visible ) => null;

    public override string BarLabel( BarMode mode ) => "b-bar-label";

    #endregion

    #region Accordion

    public override string Accordion() => "fui-Accordion";

    public override string AccordionToggle() => "fui-AccordionHeader__button";

    public override string AccordionToggleCollapsed( bool collapsed ) => null;

    public override string AccordionItem() => "fui-AccordionItem";

    public override string AccordionItemActive( bool active ) => active ? "fui-AccordionItem-active" : null;

    public override string AccordionHeader() => "fui-AccordionHeader";

    public override string AccordionBody() => "fui-AccordionPanel";

    public override string AccordionBodyActive( bool active ) => active ? "fui-AccordionPanel-show" : null;

    public override string AccordionBodyContent( bool firstInAccordion, bool lastInAccordion ) => "fui-AccordionPanel__content";

    #endregion

    #region Collapse

    public override string Collapse( bool accordion ) => "fui-AccordionItem";

    public override string CollapseActive( bool accordion, bool active ) => active ? "fui-AccordionItem-active" : null;

    public override string CollapseHeader( bool accordion ) => "fui-AccordionHeader";

    public override string CollapseBody( bool accordion ) => "fui-AccordionPanel";

    public override string CollapseBodyActive( bool accordion, bool active ) => active ? "fui-AccordionPanel-show" : null;

    public override string CollapseBodyContent( bool accordion, bool firstInAccordion, bool lastInAccordion ) => "card-body";

    #endregion

    #region Row

    public override string Row() => "fui-Row";

    public override string RowColumns( RowColumnsSize rowColumnsSize, RowColumnsDefinition rowColumnsDefinition )
    {
        if ( rowColumnsDefinition.Breakpoint != Breakpoint.None && rowColumnsDefinition.Breakpoint != Breakpoint.Mobile )
            return $"fui-RowColumns-{ToBreakpoint( rowColumnsDefinition.Breakpoint )}-{ToRowColumnsSize( rowColumnsSize )}";

        return $"fui-RowColumns-{ToRowColumnsSize( rowColumnsSize )}";
    }

    public override string RowNoGutters( bool noGutters ) => noGutters ? "fui-NoGutters" : null;

    #endregion

    #region Column

    public override string Column( bool grid, bool hasSizes ) => hasSizes ? null : "fui-Column";

    public override string Column( bool grid, ColumnWidth columnWidth, Breakpoint breakpoint, bool offset )
    {
        var baseClass = offset
            ? grid ? "fui-Grid-start" : "fui-ColumnOffset"
            : grid ? "fui-GridColumn" : "fui-Column";

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

    public override string Grid() => "fui-Grid";

    public override string GridRows( GridRowsSize gridRows, GridRowsDefinition gridRowsDefinition )
    {
        if ( gridRowsDefinition.Breakpoint != Breakpoint.None && gridRowsDefinition.Breakpoint != Breakpoint.Mobile )
            return $"fui-GridRows-{ToBreakpoint( gridRowsDefinition.Breakpoint )}-{ToGridRowsSize( gridRows )}";

        return $"fui-GridRows-{ToGridRowsSize( gridRows )}";
    }

    public override string GridColumns( GridColumnsSize gridColumns, GridColumnsDefinition gridColumnsDefinition )
    {
        if ( gridColumnsDefinition.Breakpoint != Breakpoint.None && gridColumnsDefinition.Breakpoint != Breakpoint.Mobile )
            return $"fui-GridColumns-{ToBreakpoint( gridColumnsDefinition.Breakpoint )}-{ToGridColumnsSize( gridColumns )}";

        return $"fui-GridColumns-{ToGridColumnsSize( gridColumns )}";
    }

    #endregion

    #region Display

    public override string Display( DisplayType displayType, DisplayDefinition displayDefinition )
    {
        var baseClass = displayDefinition.Breakpoint != Breakpoint.None && displayDefinition.Breakpoint != Blazorise.Breakpoint.Mobile
            ? $"fui-Display-{ToDisplayType( displayType )}-{ToBreakpoint( displayDefinition.Breakpoint )}"
            : $"fui-Display-{ToDisplayType( displayType )}";

        if ( displayDefinition.Direction != DisplayDirection.Default )
            return $"{baseClass} fui-Flex-{ToDisplayDirection( displayDefinition.Direction )}";

        return baseClass;
    }

    #endregion

    #region Alert

    public override string Alert() => "fui-MessageBar";

    public override string AlertColor( Color color ) => color.IsNotNullOrDefault() ? $"fui-MessageBar-{ToColor( color )}" : null;

    public override string AlertDismisable( bool dismissable ) => dismissable ? "fui-MessageBar-closable" : null;

    public override string AlertFade( bool dismissable ) => dismissable ? "fui-MessageBar-fade" : null;

    public override string AlertShow( bool dismissable, bool visible ) => dismissable && visible ? "fui-MessageBar-show" : null;

    public override string AlertHasMessage( bool hasMessage ) => null;

    public override string AlertHasDescription( bool hasDescription ) => null;

    public override string AlertMessage() => "fui-MessageBar__message";

    public override string AlertDescription() => "fui-MessageBar__description";

    #endregion

    #region Modal

    public override string Modal() => "fui-DialogSurface";

    public override string ModalFade( bool showing, bool hiding ) => showing || hiding ? "fui-DialogSurface-fade" : null;

    public override string ModalVisible( bool visible ) => visible ? "fui-DialogSurface-show" : "fui-DialogSurface-hide";

    public override string ModalSize( ModalSize modalSize ) => modalSize == Blazorise.ModalSize.Default ? null : $"fui-DialogSurface-{ToModalSize( modalSize )}";

    public override string ModalCentered( bool centered ) => centered ? "fui-DialogSurface-centered" : null;

    public override string ModalBackdrop() => "fui-DialogSurface__backdrop";

    public override string ModalBackdropFade() => "fui-DialogSurface__backdrop-fade";

    public override string ModalBackdropVisible( bool visible ) => visible ? "fui-DialogSurface__backdrop-show" : null;

    public override string ModalContent( bool dialog ) => "fui-DialogBody";

    public override string ModalContentSize( ModalSize modalSize ) => modalSize == Blazorise.ModalSize.Default ? null : $"fui-DialogBody-{ToModalSize( modalSize )}";

    public override string ModalContentCentered( bool centered ) => "fui-DialogBody-centered";

    public override string ModalContentScrollable( bool scrollable ) => null;

    public override string ModalBody() => "fui-DialogContent";

    public override string ModalHeader() => "fui-DialogHeader";

    public override string ModalFooter() => "fui-DialogActions";

    public override string ModalTitle() => "fui-DialogTitle";

    #endregion

    #region Offcanvas

    public override string Offcanvas() => "fui-OverlayDrawer";

    public override string OffcanvasPlacement( Placement placement, bool visible )
    {
        return placement switch
        {
            Placement.Start => "fui-OverlayDrawer-start",
            Placement.End => "fui-OverlayDrawer-end",
            Placement.Top => "fui-OverlayDrawer-top",
            Placement.Bottom => "fui-OverlayDrawer-bottom",
            _ => null,
        };
    }

    public override string OffcanvasFade( bool showing, bool hiding ) => showing
        ? "fui-OverlayDrawer-showing"
        : hiding
            ? "fui-OverlayDrawer-hiding"
            : null;

    public override string OffcanvasVisible( bool visible ) => visible ? "fui-OverlayDrawer-show" : null;

    public override string OffcanvasHeader() => "fui-DrawerHeader";

    public override string OffcanvasFooter() => "fui-DrawerFooter";

    public override string OffcanvasBody() => "fui-DrawerBody";

    public override string OffcanvasBackdrop() => "fui-OverlayDrawer__backdrop";

    public override string OffcanvasBackdropFade( bool showing, bool hiding ) => showing
        ? "fui-OverlayDrawer__backdrop-showing"
        : hiding
            ? "fui-OverlayDrawer__backdrop-hiding"
            : null;

    public override string OffcanvasBackdropVisible( bool visible ) => visible ? "fui-OverlayDrawer__backdrop-show" : null;

    #endregion

    #region Toast

    public override string Toast() => "fui-Toast";

    public override string ToastAnimated( bool animated ) => null;

    public override string ToastFade( bool visible, bool showing, bool hiding ) => showing
        ? "fui-Toast-showing"
        : hiding
            ? "fui-Toast-hiding"
            : null;

    public override string ToastVisible( bool visible ) => visible ? "fui-Toast-show" : null;

    public override string ToastHeader() => "fui-ToastHeader";

    public override string ToastBody() => "fui-ToastBody";

    public override string Toaster() => "fui-Toaster";

    public override string ToasterPlacement( ToasterPlacement placement ) => placement switch
    {
        Blazorise.ToasterPlacement.Top => "fui-Toaster-top",
        Blazorise.ToasterPlacement.TopStart => "fui-Toaster-top-start",
        Blazorise.ToasterPlacement.TopEnd => "fui-Toaster-top-end",
        Blazorise.ToasterPlacement.Bottom => "fui-Toaster-bottom",
        Blazorise.ToasterPlacement.BottomStart => "fui-Toaster-bottom-start",
        Blazorise.ToasterPlacement.BottomEnd => "fui-Toaster-bottom-end",
        _ => null,
    };

    public override string ToasterPlacementStrategy( ToasterPlacementStrategy placementStrategy ) => placementStrategy switch
    {
        Blazorise.ToasterPlacementStrategy.Fixed => "fui-Toaster-fixed",
        Blazorise.ToasterPlacementStrategy.Absolute => "fui-Toaster-absolute",
        _ => null,
    };

    #endregion

    #region Pagination

    public override string Pagination() => "fui-Pagination";

    public override string PaginationSize( Size size ) => size != Size.Default ? $"{Pagination()}-{ToSize( size )}" : null;

    public override string PaginationAlignment( Alignment alignment ) => alignment != Alignment.Default ? $"fui-JustifyContent-{ToAlignment( alignment )}" : null;

    public override string PaginationBackgroundColor( Background background ) => background.IsNotNullOrDefault() ? $"fui-Background-{ToBackground( background )}" : null;

    public override string PaginationItem() => "fui-PaginationItem";

    public override string PaginationItemActive( bool active ) => active ? "fui-PaginationItem-active" : null;

    public override string PaginationItemDisabled( bool disabled ) => disabled ? "fui-PaginationItem-disabled" : null;

    public override string PaginationLink() => "fui-PaginationLink";

    public override string PaginationLinkSize( Size size ) => null;

    public override string PaginationLinkActive( bool active ) => active ? "fui-PaginationLink-active" : null;

    public override string PaginationLinkDisabled( bool disabled ) => disabled ? "fui-PaginationLink-disabled" : null;

    #endregion

    #region Progress

    public override string Progress() => "fui-ProgressBar";

    public override string ProgressSize( Size size ) => size != Size.Default ? $"fui-ProgressBar-{ToSize( size )}" : null;

    public override string ProgressColor( Color color ) => null;

    public override string ProgressStriped( bool stripped ) => null;

    public override string ProgressAnimated( bool animated ) => null;

    public override string ProgressIndeterminate( bool indeterminate ) => null;

    public override string ProgressWidth( int width ) => null;

    public override string ProgressBar() => "fui-ProgressBar__bar";

    public override string ProgressBarSize( Size size ) => size == Size.Default ? null : $"fui-ProgressBar__bar-{ToSize( size )}";

    public override string ProgressBarColor( Color color ) => color.IsNotNullOrDefault() ? $"fui-ProgressBar__bar-{ToColor( color )}" : null;

    public override string ProgressBarStriped( bool striped ) => striped ? "fui-ProgressBar__bar-striped" : null;

    public override string ProgressBarAnimated( bool animated ) => animated ? "fui-ProgressBar__bar-animated" : null;

    public override string ProgressBarIndeterminate( bool indeterminate ) => indeterminate ? "fui-ProgressBar__bar-indeterminate" : null;

    public override string ProgressBarWidth( int width ) => null;

    #endregion

    #region Chart

    public override string Chart() => null;

    #endregion

    #region Colors

    public override string BackgroundColor( Background background ) => $"fui-Background-{ToBackground( background )}";

    #endregion

    #region Table

    public override string Table() => "fui-Table";

    public override string TableFullWidth( bool fullWidth ) => fullWidth ? "fui-Table-fullwidth" : null;

    public override string TableStriped( bool striped ) => striped ? "fui-Table-striped" : null;

    public override string TableHoverable( bool hoverable ) => hoverable ? "fui-Table-hoverable" : null;

    public override string TableBordered( bool bordered ) => bordered ? "fui-Table-bordered" : null;

    public override string TableNarrow( bool narrow ) => narrow ? "fui-Table-narrow" : null;

    public override string TableBorderless( bool borderless ) => borderless ? "fui-Table-borderless" : null;

    public override string TableHeader() => "fui-TableHeader";

    public override string TableHeaderThemeContrast( ThemeContrast themeContrast ) => themeContrast != ThemeContrast.None ? $"fui-TableHeader__theme fui-TableHeader__theme-{ToThemeContrast( themeContrast )}" : null;

    public override string TableHeaderCell() => "fui-TableHeaderCell";

    public override string TableHeaderCellCursor( Cursor cursor ) => cursor != Cursor.Default ? $"fui-Cursor-{ToCursor( cursor )}" : null;

    public override string TableHeaderCellFixed( TableColumnFixedPosition fixedPosition )
    {
        return fixedPosition switch
        {
            TableColumnFixedPosition.Start => "fui-TableHeaderCell-fixed-start",
            TableColumnFixedPosition.End => "fui-TableHeaderCell-fixed-end",
            _ => null,
        };
    }

    public override string TableFooter() => "fui-TableFooter";

    public override string TableBody() => "fui-TableBody";

    public override string TableRow( bool striped, bool hoverable ) => "fui-TableRow";

    public override string TableRowColor( Color color ) => color.IsNotNullOrDefault() ? $"fui-TableRow-{ToColor( color )}" : null;

    public override string TableRowHoverCursor( Cursor cursor ) => cursor != Cursor.Default ? "fui-TableRow-selectable" : null;

    public override string TableRowIsSelected( bool selected ) => selected ? "fui-TableRow-selected" : null;

    public override string TableRowHeader() => "fui-TableRowHeader";

    public override string TableRowHeaderFixed( TableColumnFixedPosition fixedPosition )
    {
        return fixedPosition switch
        {
            TableColumnFixedPosition.Start => "fui-TableRowHeader-fixed-start",
            TableColumnFixedPosition.End => "fui-TableRowHeader-fixed-end",
            _ => null,
        };
    }

    public override string TableRowCell() => "fui-TableCell";

    public override string TableRowCellColor( Color color ) => color.IsNotNullOrDefault() ? $"fui-TableCell-{ToColor( color )}" : null;

    public override string TableRowCellFixed( TableColumnFixedPosition fixedPosition )
    {
        return fixedPosition switch
        {
            TableColumnFixedPosition.Start => "fui-TableCell-fixed-start",
            TableColumnFixedPosition.End => "fui-TableCell-fixed-end",
            _ => null,
        };
    }

    public override string TableRowGroup( bool expanded ) => "fui-TableGroup";

    public override string TableRowGroupCell() => "fui-TableGroupCell";

    public override string TableRowGroupIndentCell() => "fui-TableGroup-indentcell";

    public override string TableResponsive( bool responsive ) => responsive ? "fui-Table-responsive" : null;

    public override string TableFixedHeader( bool fixedHeader ) => fixedHeader ? "fui-Table-fixed-header" : null;

    public override string TableFixedColumns( bool fixedColumns ) => fixedColumns ? "fui-Table-fixed-columns" : null;

    public override string TableResponsiveMode( TableResponsiveMode responsiveMode ) => responsiveMode == Blazorise.TableResponsiveMode.Mobile ? "fui-Table-mobile" : null;

    #endregion

    #region Badge

    public override string Badge() => "fui-Badge";

    public override string BadgeColor( Color color ) => color.IsNotNullOrDefault() ? $"{Badge()}-{ToColor( color )}" : null;

    public override string BadgePill( bool pill ) => pill ? $"{Badge()}-pill" : null;

    public override string BadgeClose() => "fui-Badge__close";

    public override string BadgeCloseColor( Color color ) => color.IsNotNullOrDefault() ? $"{Badge()}-{ToColor( color )}" : null;

    #endregion

    #region Media

    public override string Media() => "fui-Media";

    public override string MediaLeft() => "fui-Media-left";

    public override string MediaRight() => "fui-Media-right";

    public override string MediaBody() => "fui-Media-body";

    #endregion

    #region Text

    public override string TextColor( TextColor textColor ) => $"fui-TextColor-{ToTextColor( textColor )}";

    public override string TextAlignment( TextAlignment textAlignment ) => $"fui-TextAlignment-{ToTextAlignment( textAlignment )}";

    public override string TextTransform( TextTransform textTransform ) => $"fui-TextTransform-{ToTextTransform( textTransform )}";

    public override string TextWeight( TextWeight textWeight ) => $"fui-TextWeight-{ToTextWeight( textWeight )}";

    public override string TextOverflow( TextOverflow textOverflow ) => $"fui-TextOverflow-{ToTextOverflow( textOverflow )}";

    public override string TextSize( TextSizeType textSizeType, TextSizeDefinition textSizeDefinition )
    {
        if ( textSizeType == TextSizeType.Default )
            return null;

        if ( textSizeDefinition.Breakpoint != Breakpoint.None && textSizeDefinition.Breakpoint != Breakpoint.Mobile )
            return $"fui-TextSize-{ToBreakpoint( textSizeDefinition.Breakpoint )}-{ToTextSizeType( textSizeType )}";

        return $"fui-TextSize-{ToTextSizeType( textSizeType )}";
    }

    public override string TextItalic( bool italic ) => italic ? "fui-Text-italic" : null;

    #endregion

    #region Code

    public override string Code() => null;

    #endregion

    #region Heading

    public override string HeadingSize( HeadingSize headingSize ) => $"fui-Heading-{ToHeadingSize( headingSize )}";

    #endregion

    #region DisplayHeading

    public override string DisplayHeadingSize( DisplayHeadingSize displayHeadingSize ) => $"fui-DisplayHeading-{ToDisplayHeadingSize( displayHeadingSize )}";

    #endregion

    #region Lead

    public override string Lead() => "fui-Lead";

    #endregion

    #region Paragraph

    public override string Paragraph() => "fui-Paragraph";

    public override string ParagraphColor( TextColor textColor ) => $"fui-Paragraph-{ToTextColor( textColor )}";

    #endregion

    #region Blockquote

    public override string Blockquote() => "fui-Blockquote";

    public override string BlockquoteFooter() => "fui-Blockquote__footer";

    #endregion

    #region Figure

    public override string Figure() => "fui-Figure";

    public override string FigureSize( FigureSize figureSize ) => figureSize != Blazorise.FigureSize.Default ? $"fui-Figure-{ToFigureSize( figureSize )}" : null;

    public override string FigureImage() => "fui-Figure__image";

    public override string FigureImageRounded( bool rounded ) => rounded ? "fui-Figure-rounded" : null;

    public override string FigureCaption() => "fui-Figure__caption";

    #endregion

    #region Image

    public override string Image() => "fui-Image";

    public override string ImageFluid( bool fluid ) => fluid ? "fui-Image-fluid" : null;

    #endregion

    #region Breadcrumb

    public override string Breadcrumb() => "fui-Breadcrumb__list";

    public override string BreadcrumbItem() => "fui-BreadcrumbItem";

    public override string BreadcrumbItemActive( bool active ) => active ? "fui-BreadcrumbItem-active" : null;

    public override string BreadcrumbLink() => "fui-BreadcrumbButton";

    #endregion

    #region Tooltip

    public override string Tooltip() => "fui-Tooltip";

    public override string TooltipPlacement( TooltipPlacement tooltipPlacement ) => $"fui-Tooltip-{ToTooltipPlacement( tooltipPlacement )}";

    public override string TooltipMultiline( bool multiline ) => multiline ? "fui-Tooltip-multiline" : null;

    public override string TooltipAlwaysActive( bool alwaysActive ) => alwaysActive ? "fui-Tooltip-active" : null;

    public override string TooltipFade( bool fade ) => fade ? "fui-Tooltip-fade" : null;

    public override string TooltipInline( bool inline ) => inline ? "fui-Tooltip-inline" : null;

    #endregion

    #region Divider

    public override string Divider() => "fui-Divider";

    public override string DividerType( DividerType dividerType ) => $"{Divider()}-{ToDividerType( dividerType )}";

    #endregion

    #region Link

    public override string Link() => "fui-Link";

    public override string LinkActive( bool active ) => active ? "fui-Link-active" : null;

    public override string LinkUnstyled( bool unstyled ) => unstyled ? "fui-Link-unstyled" : null;

    public override string LinkStretched( bool stretched ) => stretched ? "fui-Link-stretched" : null;

    public override string LinkDisabled( bool disabled ) => disabled ? "fui-Link-disabled" : null;

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
            return $"fui-{spacingString}{sideString}-{ToBreakpoint( breakpoint )}{spacingSizeString}";

        return $"fui-{spacingString}{sideString}{spacingSizeString}";
    }

    public override string Spacing( Spacing spacing, SpacingSize spacingSize, IEnumerable<(Side side, Breakpoint breakpoint)> rules ) => string.Join( " ", rules.Select( x => Spacing( spacing, spacingSize, x.side, x.breakpoint ) ) );

    #endregion

    #region Gap

    public override string Gap( GapSize gapSize, GapSide gapSide )
    {
        var side = gapSide != GapSide.None && gapSide != GapSide.All
            ? $"-{ToGapSide( gapSide )}-"
            : null;

        return $"fui-Gap{side}-{ToGapSize( gapSize )}";
    }

    public override string Gap( GapSize gapSize, IEnumerable<GapSide> rules )
        => string.Join( " ", rules.Select( x => Gap( gapSize, x ) ) );

    #endregion

    #region Borders

    public override string Border( BorderSize borderSize, BorderSide borderSide, BorderColor borderColor )
    {
        var sb = new StringBuilder( "fui-Border" );

        if ( borderColor != BorderColor.None )
            sb.Append( '-' ).Append( ToBorderColor( borderColor ) );

        if ( borderSide != BorderSide.All )
            sb.Append( '-' ).Append( ToBorderSide( borderSide ) );

        if ( borderSize != BorderSize.Default )
            sb.Append( '-' ).Append( ToBorderSize( borderSize ) );

        return sb.ToString();
    }

    public override string Border( BorderSize borderSize, IEnumerable<(BorderSide borderSide, BorderColor borderColor)> rules )
        => string.Join( " ", rules.Select( x => Border( borderSize, x.borderSide, x.borderColor ) ) );

    #endregion

    #region Flex

    public override string Flex( FlexType flexType )
    {
        return flexType != FlexType.Default
            ? $"fui-{ToFlexType( flexType )}"
            : null;
    }

    public override string Flex( FlexDefinition flexDefinition )
    {
        var sb = new StringBuilder();

        var breakpoint = flexDefinition.Breakpoint != Breakpoint.None && flexDefinition.Breakpoint != Breakpoint.Mobile
            ? $"{ToBreakpoint( flexDefinition.Breakpoint )}-"
            : null;

        if ( flexDefinition.Direction != FlexDirection.Default )
            sb.Append( "fui-Flex-" ).Append( breakpoint ).Append( ToDirection( flexDefinition.Direction ) );

        if ( flexDefinition.JustifyContent != FlexJustifyContent.Default )
            sb.Append( "fui-JustifyContent-" ).Append( breakpoint ).Append( ToJustifyContent( flexDefinition.JustifyContent ) );

        if ( flexDefinition.AlignItems != FlexAlignItems.Default )
            sb.Append( "fui-AlignItems-" ).Append( breakpoint ).Append( ToAlignItems( flexDefinition.AlignItems ) );

        if ( flexDefinition.AlignSelf != FlexAlignSelf.Default )
            sb.Append( "fui-AlignSelf-" ).Append( breakpoint ).Append( ToAlignSelf( flexDefinition.AlignSelf ) );

        if ( flexDefinition.AlignContent != FlexAlignContent.Default )
            sb.Append( "fui-AlignContent-" ).Append( breakpoint ).Append( ToAlignContent( flexDefinition.AlignContent ) );

        if ( flexDefinition.GrowShrink != FlexGrowShrink.Default && flexDefinition.GrowShrinkSize != FlexGrowShrinkSize.Default )
            sb.Append( "fui-Flex-" ).Append( breakpoint ).Append( ToGrowShrink( flexDefinition.GrowShrink ) ).Append( "-" ).Append( ToGrowShrinkSize( flexDefinition.GrowShrinkSize ) );

        if ( flexDefinition.Wrap != FlexWrap.Default )
            sb.Append( "fui-Flex-" ).Append( breakpoint ).Append( ToWrap( flexDefinition.Wrap ) );

        if ( flexDefinition.Order != FlexOrder.Default )
            sb.Append( "fui-FlexOrder-" ).Append( breakpoint ).Append( ToOrder( flexDefinition.Order ) );

        if ( flexDefinition.Fill )
            sb.Append( "fui-Flex-" ).Append( breakpoint ).Append( "fill" );

        return sb.ToString();
    }

    public override string Flex( FlexType flexType, IEnumerable<FlexDefinition> flexDefinitions )
    {
        var sb = new StringBuilder();

        if ( flexType != FlexType.Default )
            sb.Append( $"fui-{ToFlexType( flexType )}" ).Append( ' ' );

        sb.Append( string.Join( ' ', flexDefinitions.Select( x => Flex( x ) ) ) );

        return sb.ToString();
    }

    #endregion

    #region Sizing

    public override string Sizing( SizingType sizingType, SizingSize sizingSize, SizingDefinition sizingDefinition )
    {
        var sb = new StringBuilder( "fui-" );

        if ( sizingDefinition.IsMin && sizingDefinition.IsViewport )
            sb.Append( "MinViewport" );
        else if ( sizingDefinition.IsMax )
            sb.Append( "Max" );
        else if ( sizingDefinition.IsViewport )
            sb.Append( "Viewport" );

        sb.Append( sizingType == SizingType.Width
            ? "Width"
            : "Height" );

        if ( sizingDefinition.Breakpoint != Breakpoint.None )
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
            Blazorise.Visibility.Visible => "fui-Visible",
            Blazorise.Visibility.Invisible => "fui-Invisible",
            _ => null,
        };
    }

    #endregion

    #region VerticalAlignment

    public override string VerticalAlignment( VerticalAlignment verticalAlignment )
        => $"fui-Align-{ToVerticalAlignment( verticalAlignment )}";

    #endregion

    #region Shadow

    public override string Shadow( Shadow shadow )
    {
        if ( shadow == Blazorise.Shadow.Default )
            return "fui-Shadow";

        return $"fui-Shadow-{ToShadow( shadow )}";
    }

    #endregion

    #region Overflow

    public override string Overflow( OverflowType overflowType, OverflowType secondOverflowType ) => secondOverflowType != OverflowType.Default
        ? $"fui-Overflow-{ToOverflowType( overflowType )}-{ToOverflowType( secondOverflowType )}"
        : $"fui-Overflow-{ToOverflowType( overflowType )}";

    #endregion

    #region Position

    public override string Position( PositionType positionType, PositionEdgeType edgeType, int edgeOffset, PositionTranslateType translateType )
    {
        return $"fui-{ToPositionEdgeType( edgeType )}-{edgeOffset}";
    }

    public override string Position( PositionType positionType, IEnumerable<(PositionEdgeType edgeType, int edgeOffset)> edges, PositionTranslateType translateType )
    {
        var sb = new StringBuilder( $"fui-Position-{ToPositionType( positionType )}" );

        if ( edges != null && edges.Count() > 0 )
            sb.Append( ' ' ).Append( string.Join( " ", edges.Select( x => Position( positionType, x.edgeType, x.edgeOffset, translateType ) ) ) );

        if ( translateType != PositionTranslateType.None )
            sb.Append( " fui-Translate-" ).Append( ToPositionTranslateType( translateType ) );

        return sb.ToString();
    }

    #endregion

    #region ObjectFit

    public override string ObjectFit( ObjectFitType objectFitType, ObjectFitDefinition objectFitDefinition )
    {
        if ( objectFitType == ObjectFitType.Default )
            return null;

        if ( objectFitDefinition.Breakpoint != Breakpoint.None && objectFitDefinition.Breakpoint != Breakpoint.Mobile )
            return $"fui-ObjectFit-{ToBreakpoint( objectFitDefinition.Breakpoint )}-{ToObjectFitType( objectFitType )}";

        return $"fui-ObjectFit-{ToObjectFitType( objectFitType )}";
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

    public override string ToSize( Size size )
    {
        return size switch
        {
            Blazorise.Size.ExtraSmall => "xs",
            Blazorise.Size.Small => "sm",
            Blazorise.Size.Medium => "md",
            Blazorise.Size.Large => "lg",
            Blazorise.Size.ExtraLarge => "xl",
            _ => null,
        };
    }

    public override string ToSpacing( Spacing spacing )
    {
        return spacing switch
        {
            Blazorise.Spacing.Margin => "Margin",
            Blazorise.Spacing.Padding => "Padding",
            _ => null,
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

    public override string ToValidationStatus( ValidationStatus validationStatus )
    {
        return validationStatus switch
        {
            Blazorise.ValidationStatus.Success => "success",
            Blazorise.ValidationStatus.Error => "error",
            _ => null,
        };
    }

    public override string ToModalSize( ModalSize modalSize )
    {
        return modalSize switch
        {
            Blazorise.ModalSize.Small => "sm",
            Blazorise.ModalSize.Large => "lg",
            Blazorise.ModalSize.ExtraLarge => "xl",
            Blazorise.ModalSize.Fullscreen => "full",
            _ => null,
        };
    }

    public override string ToDisplayType( DisplayType displayType )
    {
        return displayType switch
        {
            Blazorise.DisplayType.None => "none",
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

    public override string ToFlexType( FlexType flexType )
    {
        return flexType switch
        {
            Blazorise.FlexType.Flex => "Flex",
            Blazorise.FlexType.InlineFlex => "InlineFlex",
            _ => null,
        };
    }

    public override string ToGrowShrink( FlexGrowShrink growShrink )
    {
        return growShrink switch
        {
            Blazorise.FlexGrowShrink.Grow => "grow",
            Blazorise.FlexGrowShrink.Shrink => "shrink",
            _ => null,
        };
    }

    public override string ToTextWeight( TextWeight textWeight )
    {
        return textWeight switch
        {
            Blazorise.TextWeight.Normal => "regular",
            Blazorise.TextWeight.SemiBold => "semibold",
            Blazorise.TextWeight.Bold => "bold",
            Blazorise.TextWeight.Light => "light",
            _ => null,
        };
    }

    public override string ToPositionEdgeType( PositionEdgeType positionEdgeType )
    {
        return positionEdgeType switch
        {
            Blazorise.PositionEdgeType.Top => "Top",
            Blazorise.PositionEdgeType.Start => "Left",
            Blazorise.PositionEdgeType.Bottom => "Bottom",
            Blazorise.PositionEdgeType.End => "Right",
            _ => null,
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

    public override string ToBorderRadius( BorderRadius borderRadius )
    {
        return borderRadius switch
        {
            Blazorise.BorderRadius.Rounded => "fui-Rounded",
            Blazorise.BorderRadius.RoundedTop => "fui-Rounded-top",
            Blazorise.BorderRadius.RoundedEnd => "fui-Rounded-end",
            Blazorise.BorderRadius.RoundedBottom => "fui-Rounded-bottom",
            Blazorise.BorderRadius.RoundedStart => "fui-Rounded-start",
            Blazorise.BorderRadius.RoundedCircle => "fui-Rounded-circle",
            Blazorise.BorderRadius.RoundedPill => "fui-Rounded-pill",
            Blazorise.BorderRadius.RoundedZero => "fui-Rounded-0",
            _ => null,
        };
    }

    #endregion

    public override bool UseCustomInputStyles { get; set; } = false;

    public override string Provider => "FluentUI2";
}