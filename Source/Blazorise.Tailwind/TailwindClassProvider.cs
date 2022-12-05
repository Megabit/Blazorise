#region Using directives
using System.Collections.Generic;
using System.Linq;
using System.Text;
#endregion

namespace Blazorise.Tailwind;

public class TailwindClassProvider : ClassProvider
{
    #region TextEdit

    public override string TextEdit( bool plaintext ) => plaintext
        ? "form-control-plaintext"
        : "form-control block w-full px-3 py-1.5 text-base font-normal text-gray-700 bg-white bg-clip-padding border border-solid border-gray-300 rounded transition ease-in-out m-0 focus:text-gray-700 focus:bg-white focus:border-blue-600 focus:outline-none";

    public override string TextEditSize( Size size ) => $"form-control-{ToSize( size )}";

    public override string TextEditColor( Color color ) => $"text-{ToColor( color )}";

    public override string TextEditValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

    #endregion

    #region MemoEdit

    public override string MemoEdit( bool plaintext ) => plaintext
        ? "form-control-plaintext"
        : "form-control block w-full px-3 py-1.5 text-base font-normal text-gray-700 bg-white bg-clip-padding border border-solid border-gray-300 rounded transition ease-in-out m-0 focus:text-gray-700 focus:bg-white focus:border-blue-600 focus:outline-none";

    public override string MemoEditSize( Size size ) => $"form-control-{ToSize( size )}";

    public override string MemoEditValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

    #endregion

    #region Select

    public override string Select() => "form-select appearance-none block w-full px-3 py-1.5 text-base font-normal text-gray-700 bg-white bg-clip-padding bg-no-repeat border border-solid border-gray-300 rounded transition ease-in-out m-0 focus:text-gray-700 focus:bg-white focus:border-blue-600 focus:outline-none";

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

    public override string DateEdit( bool plaintext ) => "form-control block w-full px-3 py-1.5 text-base font-normal text-gray-700 bg-white bg-clip-padding border border-solid border-gray-300 rounded transition ease-in-out m-0 focus:text-gray-700 focus:bg-white focus:border-blue-600 focus:outline-none";

    public override string DateEditSize( Size size ) => $"form-control-{ToSize( size )}";

    public override string DateEditColor( Color color ) => $"text-{ToColor( color )}";

    public override string DateEditValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

    #endregion

    #region TimeEdit

    public override string TimeEdit( bool plaintext ) => "form-control block w-full px-3 py-1.5 text-base font-normal text-gray-700 bg-white bg-clip-padding border border-solid border-gray-300 rounded transition ease-in-out m-0 focus:text-gray-700 focus:bg-white focus:border-blue-600 focus:outline-none";

    public override string TimeEditSize( Size size ) => $"form-control-{ToSize( size )}";

    public override string TimeEditColor( Color color ) => $"text-{ToColor( color )}";

    public override string TimeEditValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

    #endregion

    #region ColorEdit

    public override string ColorEdit() => "form-control";

    public override string ColorEditSize( Size size ) => $"form-control-{ToSize( size )}";

    #endregion

    #region DatePicker

    public override string DatePicker( bool plaintext ) => "form-control block w-full px-3 py-1.5 text-base font-normal text-gray-700 bg-white bg-clip-padding border border-solid border-gray-300 rounded transition ease-in-out m-0 focus:text-gray-700 focus:bg-white focus:border-blue-600 focus:outline-none";

    public override string DatePickerSize( Size size ) => $"form-control-{ToSize( size )}";

    public override string DatePickerColor( Color color ) => $"text-{ToColor( color )}";

    public override string DatePickerValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

    #endregion

    #region TimePicker

    public override string TimePicker( bool plaintext ) => "form-control block w-full px-3 py-1.5 text-base font-normal text-gray-700 bg-white bg-clip-padding border border-solid border-gray-300 rounded transition ease-in-out m-0 focus:text-gray-700 focus:bg-white focus:border-blue-600 focus:outline-none";

    public override string TimePickerSize( Size size ) => $"form-control-{ToSize( size )}";

    public override string TimePickerColor( Color color ) => $"text-{ToColor( color )}";

    public override string TimePickerValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

    #endregion

    #region ColorPicker

    public override string ColorPicker() => "form-control b-input-color-picker";

    public override string ColorPickerSize( Size size ) => $"form-control-{ToSize( size )}";

    #endregion

    #region NumericPicker

    public override string NumericPicker( bool plaintext ) => "form-control block w-full px-3 py-1.5 text-base font-normal text-gray-700 bg-white bg-clip-padding border border-solid border-gray-300 rounded transition ease-in-out m-0 focus:text-gray-700 focus:bg-white focus:border-blue-600 focus:outline-none";

    public override string NumericPickerSize( Size size ) => $"form-control-{ToSize( size )}";

    public override string NumericPickerColor( Color color ) => $"text-{ToColor( color )}";

    public override string NumericPickerValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

    #endregion

    #region InputMask

    public override string InputMask( bool plaintext ) => "form-control block w-full px-3 py-1.5 text-base font-normal text-gray-700 bg-white bg-clip-padding border border-solid border-gray-300 rounded transition ease-in-out m-0 focus:text-gray-700 focus:bg-white focus:border-blue-600 focus:outline-none";

    public override string InputMaskSize( Size size ) => $"form-control-{ToSize( size )}";

    public override string InputMaskColor( Color color ) => $"text-{ToColor( color )}";

    public override string InputMaskValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

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

    public override string RadioGroupSize( bool buttons, Orientation orientation, Size size ) => buttons
        ? orientation == Orientation.Horizontal ? $"btn-group-{ToSize( size )}" : $"btn-group-vertical-{ToSize( size )}"
        : null;

    public override string RadioGroupValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

    #endregion

    #region Radio

    public override string Radio( bool button ) => button ? null : "form-check-label inline-block text-gray-800";

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

    public override string FileEdit() => "form-control block w-full px-3 py-1.5 text-base font-normal text-gray-700 bg-white bg-clip-padding border border-solid border-gray-300 rounded transition ease-in-out m-0 focus:text-gray-700 focus:bg-white focus:border-blue-600 focus:outline-none";

    public override string FileEditSize( Size size ) => $"{FileEdit()}-{ToSize( size )}";

    public override string FileEditValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

    #endregion

    #region Slider

    public override string Slider() => "form-range appearance-none w-full h-6 p-0 bg-transparent focus:outline-none focus:ring-0 focus:shadow-none";

    public override string SliderColor( Color color ) => $"form-control-range-{ToColor( color )}";

    public override string SliderValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

    #endregion

    #region Rating

    public override string Rating() => "rating";

    public override string RatingDisabled( bool disabled ) => disabled ? "rating-disabled" : null;

    public override string RatingReadonly( bool @readonly ) => @readonly ? "rating-readonly" : null;

    public override string RatingItem() => "rating-item";

    public override string RatingItemColor( Color color ) => $"rating-item-{ToColor( color )}";

    public override string RatingItemSelected( bool selected ) => null;

    public override string RatingItemHovered( bool hover ) => hover ? "rating-item-hover" : null;

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

    public override string Field() => "mb-3";

    public override string FieldHorizontal() => "grid grid-cols-12";

    public override string FieldColumn() => "col-span-12";

    public override string FieldJustifyContent( JustifyContent justifyContent ) => ToJustifyContent( justifyContent );

    public override string FieldValidation( ValidationStatus validationStatus ) => null;

    #endregion

    #region FieldLabel

    public override string FieldLabel( bool horizontal ) => horizontal ? "col-form-label" : "form-label inline-block mb-2 text-gray-700";

    #endregion

    #region FieldBody

    public override string FieldBody() => null;

    #endregion

    #region FieldHelp

    public override string FieldHelp() => "block mt-1 text-xs text-gray-600";

    #endregion

    #region FocusTrap

    public override string FocusTrap() => "focus-trap";

    #endregion

    #region Control

    public override string ControlCheck() => UseCustomInputStyles ? "custom-control custom-checkbox" : "form-check";

    public override string ControlRadio() => UseCustomInputStyles ? "custom-control custom-radio" : "form-check";

    public override string ControlSwitch() => UseCustomInputStyles ? "custom-control custom-switch" : "form-check";

    public override string ControlFile() => UseCustomInputStyles ? "custom-file" : "form-group";

    public override string ControlText() => null;

    #endregion

    #region Addons

    public override string Addons() => "input-group";

    public override string AddonsSize( Size size ) => $"input-group-{ToSize( size )}";

    public override string AddonsHasButton( bool hasButton ) => null;

    public override string Addon( AddonType addonType )
    {
        return addonType switch
        {
            AddonType.Start => "input-group-prepend",
            AddonType.End => "input-group-append",
            _ => null,
        };
    }

    public override string AddonLabel() => "input-group-text";

    //public override string AddonContainer() => null;

    #endregion

    #region Inline

    public override string Inline() => "form-inline";

    #endregion

    #region Button

    public override string Button( bool outline ) => outline
        ? "inline-block px-6 py-2 border-2 mb-2"
        : "inline-block px-6 py-2.5 transition duration-150 ease-in-out mb-2";

    public override string ButtonColor( Color color, bool outline )// => $"bg-{ToColor( color )} hover:bg-{ToColor( color )}-500";
    {
        var name = color?.Name;

        return name switch
        {
            "primary" => "bg-blue-600 text-white font-medium text-xs leading-tight uppercase rounded shadow-md hover:bg-blue-700 hover:shadow-lg focus:bg-blue-700 focus:shadow-lg focus:outline-none focus:ring-0 active:bg-blue-800 active:shadow-lg",
            "secondary" => "bg-purple-600 text-white font-medium text-xs leading-tight uppercase rounded shadow-md hover:bg-purple-700 hover:shadow-lg focus:bg-purple-700 focus:shadow-lg focus:outline-none focus:ring-0 active:bg-purple-800 active:shadow-lg",
            "success" => "bg-green-500 text-white font-medium text-xs leading-tight uppercase rounded shadow-md hover:bg-green-600 hover:shadow-lg focus:bg-green-600 focus:shadow-lg focus:outline-none focus:ring-0 active:bg-green-700 active:shadow-lg",
            "danger" => "bg-red-600 text-white font-medium text-xs leading-tight uppercase rounded shadow-md hover:bg-red-700 hover:shadow-lg focus:bg-red-700 focus:shadow-lg focus:outline-none focus:ring-0 active:bg-red-800 active:shadow-lg",
            "warning" => "bg-yellow-500 text-white font-medium text-xs leading-tight uppercase rounded shadow-md hover:bg-yellow-600 hover:shadow-lg focus:bg-yellow-600 focus:shadow-lg focus:outline-none focus:ring-0 active:bg-yellow-700 active:shadow-lg",
            "info" => "bg-blue-400 text-white font-medium text-xs leading-tight uppercase rounded shadow-md hover:bg-blue-500 hover:shadow-lg focus:bg-blue-500 focus:shadow-lg focus:outline-none focus:ring-0 active:bg-blue-600 active:shadow-lg",
            "light" => "bg-gray-200 text-gray-700 font-medium text-xs leading-tight uppercase rounded shadow-md hover:bg-gray-300 hover:shadow-lg focus:bg-gray-300 focus:shadow-lg focus:outline-none focus:ring-0 active:bg-gray-400 active:shadow-lg",
            "dark" => "bg-gray-800 text-white font-medium text-xs leading-tight uppercase rounded shadow-md hover:bg-gray-900 hover:shadow-lg focus:bg-gray-900 focus:shadow-lg focus:outline-none focus:ring-0 active:bg-gray-900 active:shadow-lg",
            "link" => "bg-transparent text-blue-600 font-medium text-xs leading-tight uppercase rounded hover:text-blue-700 hover:bg-gray-100 focus:bg-gray-100 focus:outline-none focus:ring-0 active:bg-gray-200",
            _ => null,
        };
    }

    public override string ButtonOutline( Color color, bool outline )
    {
        var name = color?.Name;

        return name switch
        {
            "primary" => "border-blue-600 text-blue-600 font-medium text-xs leading-tight uppercase rounded hover:bg-black hover:bg-opacity-5 focus:outline-none focus:ring-0 transition duration-150 ease-in-out",
            "secondary" => "border-purple-600 text-purple-600 font-medium text-xs leading-tight uppercase rounded hover:bg-black hover:bg-opacity-5 focus:outline-none focus:ring-0 transition duration-150 ease-in-out",
            "success" => "border-green-500 text-green-500 font-medium text-xs leading-tight uppercase rounded hover:bg-black hover:bg-opacity-5 focus:outline-none focus:ring-0 transition duration-150 ease-in-out",
            "danger" => "border-red-600 text-red-600 font-medium text-xs leading-tight uppercase rounded hover:bg-black hover:bg-opacity-5 focus:outline-none focus:ring-0 transition duration-150 ease-in-out",
            "warning" => "border-yellow-500 text-yellow-500 font-medium text-xs leading-tight uppercase rounded hover:bg-black hover:bg-opacity-5 focus:outline-none focus:ring-0 transition duration-150 ease-in-out",
            "info" => "border-blue-400 text-blue-400 font-medium text-xs leading-tight uppercase rounded hover:bg-black hover:bg-opacity-5 focus:outline-none focus:ring-0 transition duration-150 ease-in-out",
            "light" => "border-gray-200 text-gray-200 font-medium text-xs leading-tight uppercase rounded hover:bg-black hover:bg-opacity-5 focus:outline-none focus:ring-0 transition duration-150 ease-in-out",
            "dark" => "border-gray-800 text-gray-800 font-medium text-xs leading-tight uppercase rounded hover:bg-black hover:bg-opacity-5 focus:outline-none focus:ring-0 transition duration-150 ease-in-out",
            "link" => null,
            _ => null,
        };
    }

    public override string ButtonSize( Size size, bool outline ) => $"px-{ToButtonSpacingSizeX( size )} py-{ToButtonSpacingSizeY( size )} text-{ToSize( size )}";

    public override string ButtonBlock( bool outline ) => "w-full";

    public override string ButtonActive( bool outline ) => "active";

    public override string ButtonDisabled( bool outline ) => "pointer-events-none opacity-60";

    public override string ButtonLoading( bool outline ) => null;

    public virtual string ToButtonSpacingSizeX( Size size )
    {
        return size switch
        {
            Blazorise.Size.ExtraSmall => "2",
            Blazorise.Size.Small => "2",
            Blazorise.Size.Medium => "3",
            Blazorise.Size.Large => "4",
            Blazorise.Size.ExtraLarge => "5",
            _ => null,
        };
    }

    public virtual string ToButtonSpacingSizeY( Size size )
    {
        return size switch
        {
            Blazorise.Size.ExtraSmall => "1",
            Blazorise.Size.Small => "1",
            Blazorise.Size.Medium => "2",
            Blazorise.Size.Large => "3",
            Blazorise.Size.ExtraLarge => "4",
            _ => null,
        };
    }

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

    public override string ButtonsSize( Size size ) => $"btn-group-{ToSize( size )}";

    #endregion

    #region CloseButton

    public override string CloseButton() => "close";

    #endregion

    #region Dropdown

    public override string Dropdown( bool isDropdownSubmenu ) => "dropdown";

    public override string DropdownGroup() => "btn-group";

    public override string DropdownObserverShow() => DropdownShow();

    public override string DropdownShow() => Show();

    public override string DropdownRight() => null;

    public override string DropdownItem() => "dropdown-item";

    public override string DropdownItemActive( bool active ) => active ? Active() : null;

    public override string DropdownItemDisabled( bool disabled ) => disabled ? Disabled() : null;

    public override string DropdownDivider() => "dropdown-divider";

    public override string DropdownHeader() => "dropdown-header";

    public override string DropdownMenu() => "dropdown-menu";

    public override string DropdownMenuScrollable() => "dropdown-menu-scrollable";

    //public override string DropdownMenuBody() => null;

    public override string DropdownMenuVisible( bool visible ) => visible ? Show() : null;

    public override string DropdownMenuRight() => "dropdown-menu-right";

    public override string DropdownToggle( bool isDropdownSubmenu ) => isDropdownSubmenu ? "dropdown-item dropdown-toggle" : "btn dropdown-toggle";

    public override string DropdownToggleColor( Color color ) => $"{Button( false )}-{ToColor( color )}";

    public override string DropdownToggleOutline( Color color ) => color != Blazorise.Color.Default ? $"{Button( false )}-outline-{ToColor( color )}" : $"{Button( false )}-outline";

    public override string DropdownToggleSize( Size size ) => $"{Button( false )}-{ToSize( size )}";

    public override string DropdownToggleSplit() => "dropdown-toggle-split";

    public override string DropdownToggleIcon( bool visible ) => visible ? null : "dropdown-toggle-hidden";

    public override string DropdownDirection( Direction direction ) => direction switch
    {
        Direction.Up => "dropup",
        Direction.End => "dropright",
        Direction.Start => "dropleft",
        _ => null,
    };

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

    public override string CarouselSlideSlidingLeft( bool left ) => left ? "carousel-item-left" : null;

    public override string CarouselSlideSlidingRight( bool right ) => right ? "carousel-item-right" : null;

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

    public override string JumbotronBackground( Background background ) => $"jumbotron-{ToBackground( background )}";

    public override string JumbotronTitle( JumbotronTitleSize jumbotronTitleSize ) => $"display-{ToJumbotronTitleSize( jumbotronTitleSize )}";

    public override string JumbotronSubtitle() => "lead";

    #endregion

    #region Card

    public override string CardDeck() => "card-deck";

    public override string CardGroup() => "card-group";

    public override string Card() => "block p-6 rounded-lg shadow-lg bg-white";

    public override string CardWhiteText() => "text-white";

    public override string CardActions() => "card-actions";

    public override string CardBody() => "card-body";

    public override string CardFooter() => "card-footer";

    public override string CardHeader() => "card-header";

    public override string CardImage() => null;

    public override string CardTitle( bool insideHeader ) => "text-gray-900 text-xl leading-tight font-medium mb-2";

    public override string CardTitleSize( bool insideHeader, int? size ) => null;

    public override string CardSubtitle( bool insideHeader ) => "card-subtitle";

    public override string CardSubtitleSize( bool insideHeader, int size ) => null;

    public override string CardText() => "text-gray-700 text-base mb-4";

    public override string CardLink() => "card-link";

    #endregion

    #region ListGroup

    public override string ListGroup() => "list-group";

    public override string ListGroupFlush() => "list-group-flush";

    public override string ListGroupItem() => "list-group-item";

    public override string ListGroupItemSelectable() => "list-group-item-action";

    public override string ListGroupItemActive() => Active();

    public override string ListGroupItemDisabled() => Disabled();

    public override string ListGroupItemColor( Color color ) => $"{ListGroupItem()}-{ToColor( color )}";

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

    public override string Accordion() => "accordion";

    #endregion

    #region Collapse

    public override string Collapse( bool accordion ) => "card";

    public override string CollapseActive( bool accordion, bool active ) => null;

    public override string CollapseHeader( bool accordion ) => "card-header";

    public override string CollapseBody( bool accordion ) => "collapse";

    public override string CollapseBodyActive( bool accordion, bool active ) => active ? Show() : null;

    public override string CollapseBodyContent( bool accordion ) => "card-body";

    #endregion

    #region Row

    public override string Row() => "grid grid-cols-12";

    public override string RowColumns( RowColumnsSize rowColumnsSize, RowColumnsDefinition rowColumnsDefinition )
    {
        if ( rowColumnsDefinition.Breakpoint != Breakpoint.None && rowColumnsDefinition.Breakpoint != Breakpoint.Mobile )
            return $"row-cols-{ToBreakpoint( rowColumnsDefinition.Breakpoint )}-{ToRowColumnsSize( rowColumnsSize )}";

        return $"row-cols-{ToRowColumnsSize( rowColumnsSize )}";
    }

    public override string RowNoGutters() => "no-gutters";

    #endregion

    #region Column

    public override string Column( bool hasSizes ) => hasSizes ? null : "col-span-12";

    public override string Column( ColumnWidth columnWidth, Breakpoint breakpoint, bool offset )
    {
        var baseClass = offset ? "offset" : "col-span";

        if ( breakpoint != Blazorise.Breakpoint.None && breakpoint != Blazorise.Breakpoint.Mobile )
        {
            return $"{ToBreakpoint( breakpoint )}:{baseClass}-{ToColumnWidth( columnWidth )}";
        }

        return $"{baseClass}-{ToColumnWidth( columnWidth )}";
    }

    public override string Column( ColumnWidth columnWidth, IEnumerable<(Breakpoint breakpoint, bool offset)> rules )
        => string.Join( " ", rules.Select( r => Column( columnWidth, r.breakpoint, r.offset ) ) );

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

    public override string Alert() => "rounded-lg py-5 px-6 mb-4 text-base mb-3";

    public override string AlertColor( Color color )
    {
        var name = color?.Name;

        return name switch
        {
            "primary" => "bg-blue-100 text-blue-700",
            "secondary" => "bg-purple-100 text-purple-700",
            "success" => "bg-green-100 text-green-700",
            "danger" => "bg-red-100 text-red-700",
            "warning" => "bg-yellow-100 text-yellow-700",
            "info" => "bg-indigo-100 text-indigo-700",
            "light" => "bg-gray-50 text-gray-500",
            "dark" => "bg-gray-300 text-gray-800",
            "link" => "",
            _ => null,
        };
    }

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

    public override string ModalFade( bool animation ) => animation ? Fade() : null;

    public override string ModalVisible( bool visible ) => visible ? Show() : null;

    public override string ModalBackdrop() => "modal-backdrop";

    public override string ModalBackdropFade() => Fade();

    public override string ModalBackdropVisible( bool visible ) => visible ? Show() : null;

    public override string ModalContent( bool dialog ) => "modal-content";

    public override string ModalContentSize( ModalSize modalSize ) => $"modal-{ToModalSize( modalSize )}";

    public override string ModalContentFullscreen( bool fullscreen ) => null;

    public override string ModalContentCentered( bool centered ) => null;

    public override string ModalContentScrollable( bool scrollable ) => null;

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

    public override string ProgressColor( Color color ) => null;

    public override string ProgressStriped() => null;

    public override string ProgressAnimated() => null;

    public override string ProgressWidth( int width ) => null;

    public override string ProgressBar() => "progress-bar";

    public override string ProgressBarSize( Size size ) => null;

    public override string ProgressBarColor( Color color ) => $"bg-{ToColor( color )}";

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

    #region Table

    public override string Table() => "table";

    public override string TableFullWidth() => null;

    public override string TableStriped() => "table-striped";

    public override string TableHoverable() => "table-hover";

    public override string TableBordered() => "table-bordered";

    public override string TableNarrow() => "table-sm";

    public override string TableBorderless() => "table-borderless";

    public override string TableHeader() => null;

    public override string TableHeaderThemeContrast( ThemeContrast themeContrast ) => $"table-thead-theme thead-{ToThemeContrast( themeContrast )}";

    public override string TableHeaderCell() => null;

    public override string TableFooter() => null;

    public override string TableBody() => null;

    public override string TableRow() => null;

    public override string TableRowColor( Color color ) => $"table-{ToColor( color )}";

    public override string TableRowHoverCursor() => "table-row-selectable";

    public override string TableRowIsSelected() => "selected";

    public override string TableRowHeader() => null;

    public override string TableRowCell() => null;

    public override string TableRowCellColor( Color color ) => $"table-{ToColor( color )}";

    public override string TableResponsive() => "table-responsive";

    public override string TableFixedHeader() => "table-fixed-header";

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

    public override string TextOverflow( TextOverflow textOverflow ) => $"text-{ToTextOverflow( textOverflow )}";

    public override string TextItalic() => "font-italic";

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

    public override string FigureSize( FigureSize figureSize ) => $"figure-is-{ToFigureSize( figureSize )}";

    public override string FigureImage() => "figure-img img-fluid";

    public override string FigureImageRounded() => "rounded";

    public override string FigureCaption() => "figure-caption";

    #endregion

    #region Image

    public override string Image() => null;

    public override string ImageFluid( bool fluid ) => fluid ? "img-fluid" : null;

    #endregion

    #region Breadcrumb

    public override string Breadcrumb() => "breadcrumb";

    public override string BreadcrumbItem() => "breadcrumb-item";

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

        if ( flexDefinition.Wrap != FlexWrap.Default )
            sb.Append( "flex-" ).Append( breakpoint ).Append( ToWrap( flexDefinition.Wrap ) );

        if ( flexDefinition.Order != FlexOrder.Default )
            sb.Append( "order-" ).Append( breakpoint ).Append( ToOrder( flexDefinition.Order ) );

        if ( flexDefinition.Fill )
            sb.Append( "flex-" ).Append( breakpoint ).Append( "fill" );

        return sb.ToString();
    }

    public override string Flex( FlexType flexType, IEnumerable<FlexDefinition> flexDefinitions )
    {
        var sb = new StringBuilder();

        if ( flexType != FlexType.Default )
            sb.Append( $"d-{ToFlexType( flexType )}" ).Append( ' ' );

        sb.Append( string.Join( ' ', flexDefinitions.Select( x => Flex( x ) ) ) );

        return sb.ToString();
    }

    public override string FlexAlignment( Alignment alignment ) => $"justify-content-{ToAlignment( alignment )}";

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

        sb.Append( $"-{ToSizingSize( sizingSize )}" );

        return sb.ToString();
    }

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
            Blazorise.ColumnWidth.Auto => "auto",
            _ => null,
        };
    }

    #endregion

    public override bool UseCustomInputStyles { get; set; } = true;

    public override string Provider => "Tailwind";
}