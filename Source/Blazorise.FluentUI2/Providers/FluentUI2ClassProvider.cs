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

    public override string TextEditColor( Color color ) => color != Color.Default ? $"text-{ToColor( color )}" : null;

    public override string TextEditValidation( ValidationStatus validationStatus ) => validationStatus == ValidationStatus.None ? null : $"fui-Input__input-{ToValidationStatus( validationStatus )}";

    #endregion

    #region MemoEdit

    public override string MemoEdit( bool plaintext ) => plaintext ? "fui-Textarea__input-plaintext" : "fui-Textarea__input";

    public override string MemoEditSize( Size size ) => size != Size.Default ? $"fui-Textarea__input-{ToSize( size )}" : null;

    public override string MemoEditValidation( ValidationStatus validationStatus ) => validationStatus == ValidationStatus.None ? null : $"fui-Textarea__input-{ToValidationStatus( validationStatus )}";

    #endregion

    #region Select

    public override string Select() => "fui-Select__select";

    public override string SelectMultiple() => null;

    public override string SelectSize( Size size ) => size != Size.Default ? $"{Select()}-{ToSize( size )}" : null;

    public override string SelectValidation( ValidationStatus validationStatus ) => validationStatus == ValidationStatus.None ? null : $"fui-Select__select-{ToValidationStatus( validationStatus )}";

    #endregion

    #region NumericEdit

    public override string NumericEdit( bool plaintext ) => plaintext ? "fui-NumericInput fui-Input__input-plaintext" : "fui-NumericInput fui-Input__input";

    public override string NumericEditSize( Size size ) => size != Size.Default ? $"fui-Input__input-{ToSize( size )}" : null;

    public override string NumericEditColor( Color color ) => color != Color.Default ? $"text-{ToColor( color )}" : null;

    public override string NumericEditValidation( ValidationStatus validationStatus ) => validationStatus == ValidationStatus.None ? null : $"fui-Input__input-{ToValidationStatus( validationStatus )}";

    #endregion

    #region DateEdit

    public override string DateEdit( bool plaintext ) => plaintext ? "fui-DateInput fui-Input__input-plaintext" : "fui-DateInput fui-Input__input";

    public override string DateEditSize( Size size ) => size != Size.Default ? $"fui-Input__input-{ToSize( size )}" : null;

    public override string DateEditColor( Color color ) => color != Color.Default ? $"text-{ToColor( color )}" : null;

    public override string DateEditValidation( ValidationStatus validationStatus ) => validationStatus == ValidationStatus.None ? null : $"fui-Input__input-{ToValidationStatus( validationStatus )}";

    #endregion

    #region TimeEdit

    public override string TimeEdit( bool plaintext ) => plaintext ? "fui-TimeInput fui-Input__input-plaintext" : "fui-TimeInput fui-Input__input";

    public override string TimeEditSize( Size size ) => size != Size.Default ? $"fui-Input__input-{ToSize( size )}" : null;

    public override string TimeEditColor( Color color ) => color != Color.Default ? $"text-{ToColor( color )}" : null;

    public override string TimeEditValidation( ValidationStatus validationStatus ) => validationStatus == ValidationStatus.None ? null : $"fui-Input__input-{ToValidationStatus( validationStatus )}";

    #endregion

    #region ColorEdit

    public override string ColorEdit() => "fui-ColorInput fui-Input__input";

    public override string ColorEditSize( Size size ) => size != Size.Default ? $"fui-Input__input-{ToSize( size )}" : null;

    #endregion

    #region DatePicker

    public override string DatePicker( bool plaintext ) => plaintext ? "fui-DatePicker fui-Input__input-plaintext" : "fui-DatePicker fui-Input__input";

    public override string DatePickerSize( Size size ) => size != Size.Default ? $"fui-Input__input-{ToSize( size )}" : null;

    public override string DatePickerColor( Color color ) => color != Color.Default ? $"text-{ToColor( color )}" : null;

    public override string DatePickerValidation( ValidationStatus validationStatus ) => validationStatus == ValidationStatus.None ? null : $"fui-Input__input-{ToValidationStatus( validationStatus )}";

    #endregion

    #region TimePicker

    public override string TimePicker( bool plaintext ) => plaintext ? "fui-TimePicker fui-Input__input-plaintext" : "fui-TimePicker fui-Input__input";

    public override string TimePickerSize( Size size ) => size != Size.Default ? $"fui-Input__input-{ToSize( size )}" : null;

    public override string TimePickerColor( Color color ) => color != Color.Default ? $"text-{ToColor( color )}" : null;

    public override string TimePickerValidation( ValidationStatus validationStatus ) => validationStatus == ValidationStatus.None ? null : $"fui-Input__input-{ToValidationStatus( validationStatus )}";

    #endregion

    #region ColorPicker

    public override string ColorPicker() => "fui-ColorPicker fui-Input__input b-input-color-picker";

    public override string ColorPickerSize( Size size ) => size != Size.Default ? $"fui-Input__input-{ToSize( size )}" : null;

    #endregion

    #region NumericPicker

    public override string NumericPicker( bool plaintext ) => plaintext ? "fui-NumericPicker fui-Input__input-plaintext" : "fui-NumericInput fui-Input__input";

    public override string NumericPickerSize( Size size ) => size != Size.Default ? $"fui-Input__input-{ToSize( size )}" : null;

    public override string NumericPickerColor( Color color ) => color != Color.Default ? $"text-{ToColor( color )}" : null;

    public override string NumericPickerValidation( ValidationStatus validationStatus ) => validationStatus == ValidationStatus.None ? null : $"fui-Input__input-{ToValidationStatus( validationStatus )}";

    #endregion

    #region InputMask

    public override string InputMask( bool plaintext ) => plaintext ? "fui-InputMask fui-Input__input-plaintext" : "fui-InputMask fui-Input__input";

    public override string InputMaskSize( Size size ) => size != Size.Default ? $"fui-Input__input-{ToSize( size )}" : null;

    public override string InputMaskColor( Color color ) => color != Color.Default ? $"text-{ToColor( color )}" : null;

    public override string InputMaskValidation( ValidationStatus validationStatus ) => validationStatus == ValidationStatus.None ? null : $"fui-Input__input-{ToValidationStatus( validationStatus )}";

    #endregion

    #region Check

    public override string Check() => "fui-Checkbox__input";

    public override string CheckSize( Size size ) => $"{Check()}-{ToSize( size )}";

    public override string CheckInline() => "fui-Checkbox__input-inline";

    public override string CheckCursor( Cursor cursor ) => $"{Check()}-{ToCursor( cursor )}";

    public override string CheckValidation( ValidationStatus validationStatus ) => validationStatus == ValidationStatus.None ? null : $"fui-Input__input-{ToValidationStatus( validationStatus )}";

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

    public override string RadioGroupValidation( ValidationStatus validationStatus ) => validationStatus == ValidationStatus.None ? null : $"fui-RadioGroup-{ToValidationStatus( validationStatus )}";

    #endregion

    #region Radio

    public override string Radio( bool button ) => "fui-Radio__input";

    public override string RadioSize( bool button, Size size ) => $"fui-Radio__input-{ToSize( size )}";

    public override string RadioInline( bool inline ) => inline
        ? "fui-Radio__input-inline"
        : null;

    public override string RadioCursor( Cursor cursor ) => $"fui-Radio__input-{ToCursor( cursor )}";

    public override string RadioValidation( ValidationStatus validationStatus ) => validationStatus == ValidationStatus.None ? null : $"fui-Radio__input-{ToValidationStatus( validationStatus )}";

    #endregion

    #region Switch

    public override string Switch() => "fui-Switch__input";

    public override string SwitchColor( Color color ) => $"{Switch()}-{ToColor( color )}";

    public override string SwitchSize( Size size ) => $"fui-Switch__input-{ToSize( size )}";

    public override string SwitchChecked( bool @checked ) => null;

    public override string SwitchCursor( Cursor cursor ) => $"{Switch()}-{ToCursor( cursor )}";

    public override string SwitchValidation( ValidationStatus validationStatus ) => validationStatus == ValidationStatus.None ? null : $"fui-Switch__input-{ToValidationStatus( validationStatus )}";

    #endregion

    #region FileEdit

    public override string FileEdit() => "fui-Input__input";

    public override string FileEditSize( Size size ) => size != Size.Default ? $"{FileEdit()}-{ToSize( size )}" : null;

    public override string FileEditValidation( ValidationStatus validationStatus ) => validationStatus == ValidationStatus.None ? null : $"fui-Input__input-{ToValidationStatus( validationStatus )}";

    #endregion

    #region Slider

    public override string Slider() => "fui-Slider__input";

    public override string SliderColor( Color color ) => $"fui-Slider__input-{ToColor( color )}";

    public override string SliderValidation( ValidationStatus validationStatus ) => validationStatus == ValidationStatus.None ? null : $"fui-Slider__input-{ToValidationStatus( validationStatus )}";

    #endregion

    #region Rating

    public override string Rating() => "fui-Rating";

    public override string RatingDisabled( bool disabled ) => disabled ? "fui-Rating-disabled" : null;

    public override string RatingReadonly( bool @readonly ) => @readonly ? "fui-Rating-readonly" : null;

    public override string RatingItem() => "fui-RatingItem";

    public override string RatingItemColor( Color color ) => $"fui-RatingItem-{ToColor( color )}";

    public override string RatingItemSelected( bool selected ) => selected ? "fui-RatingItem-selected" : null;

    public override string RatingItemHovered( bool hover ) => hover ? "fui-RatingItem-hover" : null;

    #endregion

    #region Label

    public override string Label() => null;

    public override string LabelType( LabelType labelType )
    {
        return labelType switch
        {
            Blazorise.LabelType.Check or Blazorise.LabelType.Radio or Blazorise.LabelType.Switch => UseCustomInputStyles ? "custom-control-label" : "form-check-label",
            Blazorise.LabelType.File => UseCustomInputStyles ? "custom-file-label" : null,
            _ => null,
        };
    }

    public override string LabelCursor( Cursor cursor ) => UseCustomInputStyles ? $"custom-control-label-{ToCursor( cursor )}" : $"form-check-label-{ToCursor( cursor )}";

    #endregion

    #region Help

    public override string Help() => "form-text text-muted";

    #endregion

    #region Validation

    public override string ValidationSuccess() => "fui-Field__validationMessage";

    public override string ValidationSuccessTooltip() => "valid-tooltip";

    public override string ValidationError() => "fui-Field__validationMessage";

    public override string ValidationErrorTooltip() => "invalid-tooltip";

    public override string ValidationNone() => "form-text text-muted";

    public override string ValidationSummary() => "text-danger";

    public override string ValidationSummaryError() => "text-danger";

    #endregion

    #region Fields

    public override string Fields() => "fui-Fields";

    public override string FieldsBody() => null;

    public override string FieldsColumn() => "fui-Column";

    #endregion

    #region Field

    public override string Field() => "fui-Field";

    public override string FieldHorizontal() => "fui-FieldHorizontal";

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

    public override string ControlCheck() => UseCustomInputStyles ? "custom-control custom-checkbox" : "form-check";

    public override string ControlRadio() => UseCustomInputStyles ? "custom-control custom-radio" : "form-check";

    public override string ControlSwitch() => UseCustomInputStyles ? "custom-control custom-switch" : "form-check";

    public override string ControlFile() => UseCustomInputStyles ? "custom-file" : "form-group";

    public override string ControlText() => null;

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

    //public override string AddonContainer() => null;

    #endregion

    #region Inline

    public override string Inline() => "fui-Fields-inline";

    #endregion

    #region Button

    public override string Button( bool outline ) => "fui-Button";

    public override string ButtonColor( Color color, bool outline ) => outline
        ? color != Color.Default ? $"{Button( outline )}Outline-{ToColor( color )}" : $"{Button( outline )}Outline"
        : color != Color.Default ? $"{Button( outline )}-{ToColor( color )}" : null;

    public override string ButtonSize( Size size, bool outline ) => size == Size.Default ? null : $"fui-Button-{ToSize( size )}";

    public override string ButtonBlock( bool outline ) => $"{Button( outline )}-block";

    public override string ButtonActive( bool outline ) => "active";

    public override string ButtonDisabled( bool outline ) => "disabled";

    public override string ButtonLoading( bool outline ) => "loading";

    public override string ButtonStretchedLink( bool stretched ) => stretched ? "link-stretched" : null;

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

    public override string ButtonsSize( Size size ) => $"fui-ButtonGroup-{ToSize( size )}";

    #endregion

    #region CloseButton

    public override string CloseButton() => "fui-CloseButton";

    #endregion

    #region Dropdown

    public override string Dropdown( bool isDropdownSubmenu ) => "fui-Menu";

    public override string DropdownDisabled() => "fui-MenuButton-disabled";

    public override string DropdownGroup() => "fui-ButtonGroup";

    public override string DropdownObserverShow() => DropdownShow();

    public override string DropdownShow() => "fui-MenuButton-show";

    public override string DropdownRight() => null;

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

    public override string DropdownMenuSelector() => "dropdown-menu";

    public override string DropdownMenuScrollable() => "dropdown-menu-scrollable";

    //public override string DropdownMenuBody() => null;

    public override string DropdownMenuVisible( bool visible ) => visible ? "fui-MenuPopover-show" : null;

    public override string DropdownMenuRight() => "dropdown-menu-right";

    public override string DropdownToggle( bool isDropdownSubmenu, bool outline ) => isDropdownSubmenu
        ? "fui-MenuItem"
        : "fui-Button fui-MenuButton";

    public override string DropdownToggleSelector( bool isDropdownSubmenu ) => isDropdownSubmenu
        ? "fui-MenuItem"
        : "fui-Button fui-MenuButton";

    public override string DropdownToggleColor( Color color, bool outline ) => outline
        ? color != Color.Default ? $"fui-ButtonOutline-{ToColor( color )}" : $"fui-ButtonOutline"
        : color != Color.Default ? $"fui-Button-{ToColor( color )}" : null;

    public override string DropdownToggleSize( Size size, bool outline )
        => size != Size.Default ? $"btn-{ToSize( size )}" : null;

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
    //    ? color != Color.Default ? $"btn-outline-{ToColor( color )}" : $"btn-outline"
    //    : color != Color.Default ? $"btn-{ToColor( color )}" : null;

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

    public override string TabsCards() => null;

    public override string TabsFullWidth() => "fui-TabList-fill";

    public override string TabsJustified() => "fui-TabList-justified";

    public override string TabsVertical() => null;

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

    public override string StepItemColor( Color color ) => $"{StepItem()}-{ToColor( color )}";

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

    public override string Jumbotron() => "jumbotron";

    public override string JumbotronBackground( Background background ) => $"jumbotron-{ToBackground( background )}";

    public override string JumbotronTitle( JumbotronTitleSize jumbotronTitleSize ) => $"display-{ToJumbotronTitleSize( jumbotronTitleSize )}";

    public override string JumbotronSubtitle() => "lead";

    #endregion

    #region Card

    public override string CardDeck() => "fui-CardDeck";

    public override string CardGroup() => "fui-CardGroup";

    public override string Card() => "fui-Card";

    public override string CardWhiteText() => "fui-Text-white";

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

    public override string ListGroupItemSelectable() => "fui-ListGroupItem-action";

    public override string ListGroupItemActive( bool active ) => active ? "fui-ListGroupItem-active" : null;

    public override string ListGroupItemDisabled( bool disabled ) => disabled ? "fui-ListGroupItem-disabled" : null;

    public override string ListGroupItemColor( Color color, bool selectable, bool active ) => color.IsNullOrDefault() ? null : $"{ListGroupItem()}-{base.ToColor( color )}";

    #endregion

    #region Container

    public override string Container( Breakpoint breakpoint )
        => breakpoint != Breakpoint.None && breakpoint != Breakpoint.Mobile ? $"container-{ToBreakpoint( breakpoint )}" : "container";

    public override string ContainerFluid() => "container-fluid";

    #endregion

    #region Bar

    public override string Bar() => "navbar";

    public override string BarInitial( bool initial ) => initial ? "b-bar-initial" : null;

    public override string BarAlignment( Alignment alignment ) => FlexAlignment( alignment );

    public override string BarThemeContrast( ThemeContrast themeContrast ) => $"navbar-{ToThemeContrast( themeContrast )} b-bar-{ToThemeContrast( themeContrast )}";

    public override string BarBreakpoint( Breakpoint breakpoint ) => breakpoint != Breakpoint.None && breakpoint != Breakpoint.Mobile ? $"navbar-expand-{ToBreakpoint( breakpoint )}" : null;

    public override string BarMode( BarMode mode ) => $"b-bar-{ToBarMode( mode )}";

    public override string BarItem( BarMode mode, bool hasDropdown ) => mode == Blazorise.BarMode.Horizontal
        ? hasDropdown
            ? "nav-item dropdown"
            : "nav-item"
        : "b-bar-item";

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

    public override string BarDropdown( BarMode mode, bool isBarDropDownSubmenu ) => mode == Blazorise.BarMode.Horizontal
        ? "dropdown"
        : "b-bar-dropdown";

    public override string BarDropdownShow( BarMode mode ) => Show();

    public override string BarDropdownToggle( BarMode mode, bool isBarDropDownSubmenu ) => mode == Blazorise.BarMode.Horizontal
        ? isBarDropDownSubmenu
            ? "dropdown-item"
            : "nav-link dropdown-toggle"
        : "b-bar-link b-bar-dropdown-toggle";

    public override string BarDropdownToggleDisabled( BarMode mode, bool isBarDropDownSubmenu, bool disabled )
        => mode == Blazorise.BarMode.Horizontal && disabled ? "disabled" : null;

    public override string BarDropdownItem( BarMode mode ) => mode == Blazorise.BarMode.Horizontal ? "dropdown-item" : "b-bar-dropdown-item";

    public override string BarTogglerIcon( BarMode mode ) => "navbar-toggler-icon";

    public override string BarDropdownDivider( BarMode mode ) => "dropdown-divider";

    public override string BarDropdownMenu( BarMode mode ) => mode == Blazorise.BarMode.Horizontal ? "dropdown-menu" : "b-bar-dropdown-menu";

    public override string BarDropdownMenuVisible( BarMode mode, bool visible ) => visible ? Show() : null;

    public override string BarDropdownMenuRight( BarMode mode ) => mode == Blazorise.BarMode.Horizontal ? "dropdown-menu-right" : "b-bar-right";

    public override string BarDropdownMenuContainer( BarMode mode ) => mode == Blazorise.BarMode.Horizontal ? null : "b-bar-dropdown-menu-container";

    public override string BarCollapsed( BarMode mode ) => null;

    public override string BarLabel() => "b-bar-label";

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

    public override string AlertColor( Color color ) => $"fui-MessageBar-{ToColor( color )}";

    public override string AlertDismisable() => "fui-MessageBar-closable";

    public override string AlertFade() => "fui-MessageBar-fade";

    public override string AlertShow() => "fui-MessageBar-show";

    public override string AlertHasMessage() => null;

    public override string AlertHasDescription() => null;

    public override string AlertMessage() => "fui-MessageBar__message";

    public override string AlertDescription() => "fui-MessageBar__description";

    #endregion

    #region Modal

    public override string Modal() => "fui-DialogSurface";

    public override string ModalFade() => "fui-DialogSurface-fade";

    public override string ModalFade( bool animation ) => animation ? "fui-DialogSurface-fade" : null;

    public override string ModalVisible( bool visible ) => visible ? "fui-DialogSurface-show" : "fui-DialogSurface-hide";

    public override string ModalSize( ModalSize modalSize ) => modalSize == Blazorise.ModalSize.Default ? null : $"fui-DialogSurface-{ToModalSize( modalSize )}";

    public override string ModalBackdrop() => "fui-DialogSurface__backdrop";

    public override string ModalBackdropFade() => "fui-DialogSurface__backdrop-fade";

    public override string ModalBackdropVisible( bool visible ) => visible ? "fui-DialogSurface__backdrop-show" : null;

    public override string ModalContent( bool dialog ) => "fui-DialogBody";

    public override string ModalContentSize( ModalSize modalSize ) => null;

    public override string ModalContentCentered( bool centered ) => null;

    public override string ModalContentScrollable( bool scrollable ) => null;

    public override string ModalBody() => "fui-DialogContent";

    public override string ModalHeader() => "fui-DialogTitle";

    public override string ModalFooter() => "fui-DialogActions";

    public override string ModalTitle() => "modal-title";

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

    #region Pagination

    public override string Pagination() => "fui-Pagination";

    public override string PaginationSize( Size size ) => $"{Pagination()}-{ToSize( size )}";

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

    public override string ProgressSize( Size size ) => size == Size.Default ? null : $"fui-ProgressBar-{ToSize( size )}";

    public override string ProgressColor( Color color ) => null;

    public override string ProgressStriped() => null;

    public override string ProgressAnimated() => null;

    public override string ProgressIndeterminate() => null;

    public override string ProgressWidth( int width ) => null;

    public override string ProgressBar() => "fui-ProgressBar__bar";

    public override string ProgressBarSize( Size size ) => size == Size.Default ? null : $"fui-ProgressBar__bar-{ToSize( size )}";

    public override string ProgressBarColor( Color color ) => $"fui-ProgressBar__bar-{ToColor( color )}";

    public override string ProgressBarStriped() => "fui-ProgressBar__bar-striped";

    public override string ProgressBarAnimated() => "fui-ProgressBar__bar-animated";

    public override string ProgressBarIndeterminate() => "fui-ProgressBar__bar-indeterminate";

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

    public override string TableFullWidth() => "fui-Table-fullwidth";

    public override string TableStriped() => "fui-Table-striped";

    public override string TableHoverable() => "fui-Table-hoverable";

    public override string TableBordered() => "fui-Table-bordered";

    public override string TableNarrow() => "fui-Table-narrow";

    public override string TableBorderless() => "fui-Table-borderless";

    public override string TableHeader() => "fui-TableHeader";

    public override string TableHeaderThemeContrast( ThemeContrast themeContrast ) => $"fui-TableHeader__theme fui-TableHeader__theme-{ToThemeContrast( themeContrast )}";

    public override string TableHeaderCell() => "fui-TableHeaderCell";

    public override string TableHeaderCellCursor( Cursor cursor ) => cursor != Cursor.Default ? $"cursor-{ToCursor( cursor )}" : null;

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

    public override string TableRowColor( Color color ) => $"fui-TableRow-{ToColor( color )}";

    public override string TableRowHoverCursor() => "fui-TableRow-selectable";

    public override string TableRowIsSelected() => "fui-TableRow-selected";

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

    public override string TableRowCellColor( Color color ) => $"fui-TableCell-{ToColor( color )}";

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

    #endregion

    #region Badge

    public override string Badge() => "fui-Badge";

    public override string BadgeColor( Color color ) => $"{Badge()}-{ToColor( color )}";

    public override string BadgePill() => $"{Badge()}-pill";

    public override string BadgeClose() => "fui-Badge__close";

    #endregion

    #region Media

    public override string Media() => "media";

    public override string MediaLeft() => "media-left";

    public override string MediaRight() => "media-right";

    public override string MediaBody() => "media-body";

    #endregion

    #region Text

    public override string TextColor( TextColor textColor ) => $"fui-TextColor-{ToTextColor( textColor )}";

    public override string TextAlignment( TextAlignment textAlignment ) => $"fui-TextAlignment-{ToTextAlignment( textAlignment )}";

    public override string TextTransform( TextTransform textTransform ) => $"fui-TextTransform-{ToTextTransform( textTransform )}";

    public override string TextWeight( TextWeight textWeight ) => $"fui-TextWeight-{ToTextWeight( textWeight )}";

    public override string TextOverflow( TextOverflow textOverflow ) => $"fui-TextOverflow-{ToTextOverflow( textOverflow )}";

    public override string TextSize( TextSize textSize ) => $"fui-TextSize-{ToTextSize( textSize )}";

    public override string TextItalic() => "fui-Text-italic";

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

    public override string FigureSize( FigureSize figureSize ) => $"fui-Figure-{ToFigureSize( figureSize )}";

    public override string FigureImage() => "fui-Figure__image";

    public override string FigureImageRounded() => "fui-Figure-rounded";

    public override string FigureCaption() => "fui-Figure__caption";

    #endregion

    #region Image

    public override string Image() => "fui-Image";

    public override string ImageFluid( bool fluid ) => fluid ? "fui-Image-fluid" : null;

    #endregion

    #region Breadcrumb

    public override string Breadcrumb() => "fui-Breadcrumb__list";

    public override string BreadcrumbItem() => "fui-BreadcrumbItem";

    public override string BreadcrumbItemActive() => "fui-BreadcrumbItem-active";

    public override string BreadcrumbLink() => "fui-BreadcrumbButton";

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
        var sb = new StringBuilder( "border" );

        if ( borderSide != BorderSide.All )
            sb.Append( '-' ).Append( ToBorderSide( borderSide ) );

        if ( borderSize != BorderSize.Default )
            sb.Append( '-' ).Append( ToBorderSize( borderSize ) );

        if ( borderColor != BorderColor.None )
            sb.Append( " border-" ).Append( ToBorderColor( borderColor ) );

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

    public override string FlexAlignment( Alignment alignment ) => $"fui-JustifyContent-{ToAlignment( alignment )}";

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
        ? $"fui-Overflow-{ToOverflowType( overflowType )}-{ToOverflowType( secondOverflowType )}"
        : $"fui-Overflow-{ToOverflowType( overflowType )}";

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

    #region Elements

    public override string UnorderedList() => "unordered-list";

    public override string UnorderedListUnstyled( bool unstyled ) => unstyled ? "list-unstyled" : null;

    public override string OrderedList() => "ordered-list";

    public override string OrderedListUnstyled( bool unstyled ) => unstyled ? "list-unstyled" : null;

    public override string OrderedListType( OrderedListType orderedListType ) => $"ordered-list-{ToOrderedListType( orderedListType )}";

    public override string DescriptionList() => null;

    public override string DescriptionListTerm() => null;

    public override string DescriptionListDefinition() => null;

    #endregion

    #region Enums

    public override string ToBreakpoint( Breakpoint breakpoint )
    {
        return breakpoint switch
        {
            Blazorise.Breakpoint.Mobile or Blazorise.Breakpoint.ExtraSmall => "xs",
            Blazorise.Breakpoint.Tablet or Blazorise.Breakpoint.Small => "sm",
            Blazorise.Breakpoint.Desktop or Blazorise.Breakpoint.Medium => "md",
            Blazorise.Breakpoint.Widescreen or Blazorise.Breakpoint.Large => "lg",
            Blazorise.Breakpoint.FullHD or Blazorise.Breakpoint.ExtraLarge => "xl",
            Blazorise.Breakpoint.Full2K or Blazorise.Breakpoint.ExtraExtraLarge => "xxl",
            _ => null,
        };
    }

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

    #endregion

    public override bool UseCustomInputStyles { get; set; } = false;

    public override string Provider => "FluentUI2";
}