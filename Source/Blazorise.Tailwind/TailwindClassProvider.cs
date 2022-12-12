#region Using directives
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
#endregion

namespace Blazorise.Tailwind;

public class TailwindClassProvider : ClassProvider
{
    #region TextEdit

    public override string TextEdit( bool plaintext ) => plaintext
        ? "text-gray-900 text-sm border-none focus:ring-primary-500 focus:border-primary-500 block w-full p-2.5 dark:text-white dark:focus:ring-primary-500 dark:focus:border-primary-500"
        : "bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-primary-500 focus:border-primary-500 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-primary-500 dark:focus:border-primary-500";

    public override string TextEditSize( Size size ) => $"form-control-{ToSize( size )}";

    public override string TextEditColor( Color color ) => $"text-{ToColor( color )}";

    public override string TextEditValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

    #endregion

    #region MemoEdit

    public override string MemoEdit( bool plaintext ) => plaintext
        ? "form-control-plaintext"
        : "block p-2.5 w-full text-sm text-gray-900 bg-gray-50 rounded-lg border border-gray-300 focus:ring-primary-500 focus:border-primary-500 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-primary-500 dark:focus:border-primary-500";

    public override string MemoEditSize( Size size ) => $"form-control-{ToSize( size )}";

    public override string MemoEditValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

    #endregion

    #region Select

    public override string Select() => "bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-primary-500 focus:border-primary-500 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-primary-500 dark:focus:border-primary-500";

    public override string SelectMultiple() => null;

    public override string SelectSize( Size size ) => $"{Select()}-{ToSize( size )}";

    public override string SelectValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

    #endregion

    #region NumericEdit

    public override string NumericEdit( bool plaintext ) => plaintext
        ? "form-control-plaintext"
        : "bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-primary-500 focus:border-primary-500 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-primary-500 dark:focus:border-primary-500";

    public override string NumericEditSize( Size size ) => $"form-control-{ToSize( size )}";

    public override string NumericEditColor( Color color ) => $"text-{ToColor( color )}";

    public override string NumericEditValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

    #endregion

    #region DateEdit

    public override string DateEdit( bool plaintext ) => "bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-primary-500 focus:border-primary-500 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-primary-500 dark:focus:border-primary-500";

    public override string DateEditSize( Size size ) => $"form-control-{ToSize( size )}";

    public override string DateEditColor( Color color ) => $"text-{ToColor( color )}";

    public override string DateEditValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

    #endregion

    #region TimeEdit

    public override string TimeEdit( bool plaintext ) => "bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-primary-500 focus:border-primary-500 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-primary-500 dark:focus:border-primary-500";

    public override string TimeEditSize( Size size ) => $"form-control-{ToSize( size )}";

    public override string TimeEditColor( Color color ) => $"text-{ToColor( color )}";

    public override string TimeEditValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

    #endregion

    #region ColorEdit

    public override string ColorEdit() => "form-control";

    public override string ColorEditSize( Size size ) => $"form-control-{ToSize( size )}";

    #endregion

    #region DatePicker

    public override string DatePicker( bool plaintext ) => "bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-primary-500 focus:border-primary-500 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-primary-500 dark:focus:border-primary-500";

    public override string DatePickerSize( Size size ) => $"form-control-{ToSize( size )}";

    public override string DatePickerColor( Color color ) => $"text-{ToColor( color )}";

    public override string DatePickerValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

    #endregion

    #region TimePicker

    public override string TimePicker( bool plaintext ) => "bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-primary-500 focus:border-primary-500 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-primary-500 dark:focus:border-primary-500";

    public override string TimePickerSize( Size size ) => $"form-control-{ToSize( size )}";

    public override string TimePickerColor( Color color ) => $"text-{ToColor( color )}";

    public override string TimePickerValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

    #endregion

    #region ColorPicker

    public override string ColorPicker() => "form-control b-input-color-picker";

    public override string ColorPickerSize( Size size ) => $"form-control-{ToSize( size )}";

    #endregion

    #region NumericPicker

    public override string NumericPicker( bool plaintext ) => "bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-primary-500 focus:border-primary-500 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-primary-500 dark:focus:border-primary-500";

    public override string NumericPickerSize( Size size ) => $"form-control-{ToSize( size )}";

    public override string NumericPickerColor( Color color ) => $"text-{ToColor( color )}";

    public override string NumericPickerValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

    #endregion

    #region InputMask

    public override string InputMask( bool plaintext ) => "bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-primary-500 focus:border-primary-500 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-primary-500 dark:focus:border-primary-500";

    public override string InputMaskSize( Size size ) => $"form-control-{ToSize( size )}";

    public override string InputMaskColor( Color color ) => $"text-{ToColor( color )}";

    public override string InputMaskValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

    #endregion

    #region Check

    public override string Check() => "w-4 h-4 text-primary-600 bg-gray-100 rounded border-gray-300 focus:ring-primary-500 dark:focus:ring-primary-600 dark:ring-offset-gray-800 focus:ring-2 dark:bg-gray-700 dark:border-gray-600";

    public override string CheckSize( Size size ) => $"{Check()}-{ToSize( size )}";

    public override string CheckInline() => UseCustomInputStyles ? "custom-control-inline" : "form-check-inline";

    public override string CheckCursor( Cursor cursor ) => $"{Check()}-{ToCursor( cursor )}";

    public override string CheckValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

    #endregion

    #region RadioGroup

    public override string RadioGroup( bool buttons, Orientation orientation ) => buttons
        ? orientation == Orientation.Horizontal
            ? "items-center w-full text-sm font-medium text-gray-900 bg-white rounded-lg border border-gray-200 sm:flex dark:bg-gray-700 dark:border-gray-600 dark:text-white"
            : "w-48 text-sm font-medium text-gray-900 bg-white rounded-lg border border-gray-200 dark:bg-gray-700 dark:border-gray-600 dark:text-white"
        : "flex flex-wrap";

    public override string RadioGroupSize( bool buttons, Orientation orientation, Size size ) => buttons
        ? orientation == Orientation.Horizontal ? $"btn-group-{ToSize( size )}" : $"btn-group-vertical-{ToSize( size )}"
        : null;

    public override string RadioGroupValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

    #endregion

    #region Radio

    public override string Radio( bool button ) => button ? null : "w-4 h-4 text-primary-600 bg-gray-100 border-gray-300 focus:ring-primary-500 dark:focus:ring-primary-600 dark:ring-offset-gray-800 focus:ring-2 dark:bg-gray-700 dark:border-gray-600";

    public override string RadioSize( bool button, Size size ) => $"{Radio( button )}-{ToSize( size )}";

    public override string RadioInline( bool inline ) => inline
        ? UseCustomInputStyles ? "custom-control-inline" : "form-check-inline"
        : null;

    public override string RadioCursor( Cursor cursor ) => $"{( UseCustomInputStyles ? "custom-control-input" : "form-check-input" )}-{ToCursor( cursor )}";

    public override string RadioValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

    #endregion

    #region Switch

    public override string Switch() => "sr-only peer";

    public override string SwitchColor( Color color ) => $"{Switch()}-{ToColor( color )}";

    public override string SwitchSize( Size size ) => $"custom-control-input-{ToSize( size )}";

    public override string SwitchChecked( bool @checked ) => null;

    public override string SwitchCursor( Cursor cursor ) => null;

    public override string SwitchValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

    #endregion

    #region FileEdit

    public override string FileEdit() => "block w-full text-sm text-gray-900 border border-gray-300 rounded-lg cursor-pointer bg-gray-50 dark:text-gray-400 focus:outline-none dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400";

    public override string FileEditSize( Size size ) => $"{FileEdit()}-{ToSize( size )}";

    public override string FileEditValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

    #endregion

    #region Slider

    public override string Slider() => "w-full h-2 bg-gray-200 rounded-lg appearance-none cursor-pointer dark:bg-gray-700";

    public override string SliderColor( Color color ) => $"form-control-range-{ToColor( color )}";

    public override string SliderValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

    #endregion

    #region Rating

    public override string Rating() => "flex items-center";

    public override string RatingDisabled( bool disabled ) => disabled ? "cursor-not-allowed opacity-60" : null;

    public override string RatingReadonly( bool @readonly ) => null;

    public override string RatingItem() => "w-5 h-5";

    public override string RatingItemColor( Color color ) => $"text-{ToColor( color )}-400";

    public override string RatingItemSelected( bool selected ) => null;

    public override string RatingItemHovered( bool hover ) => hover ? "hover:opacity-80" : null;

    #endregion

    #region Label

    public override string Label() => null;

    public override string LabelType( LabelType labelType )
    {
        return labelType switch
        {
            Blazorise.LabelType.Check or Blazorise.LabelType.Radio => "ml-2 text-sm font-medium text-gray-900 dark:text-gray-300",
            Blazorise.LabelType.Switch => "ml-3 text-sm font-medium text-gray-900 dark:text-gray-300",
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

    public override string FieldLabel( bool horizontal ) => "block mb-2 text-sm font-medium text-gray-900 dark:text-white";

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

    public override string ControlCheck() => "flex items-center mb-4 mr-4";

    public override string ControlRadio() => "flex items-center mb-4 mr-4";

    public override string ControlSwitch() => "inline-flex relative items-center cursor-pointer mb-4 mr-4";

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
        ? "border focus:ring-4 focus:outline-none font-medium rounded-lg text-center mr-2 mb-2 group:mr:2 group:mb:0"
        : "focus:ring-4 font-medium rounded-lg focus:outline-none rounded-lg mr-2 mb-2 group:mr-0 group:mb-0";

    public override string ButtonColor( Color color, bool outline )
    {
        var name = color?.Name;

        return name switch
        {
            "primary" => "text-white bg-primary-700 hover:bg-primary-800 focus:ring-primary-300 dark:bg-primary-600 dark:hover:bg-primary-700 dark:focus:ring-primary-800",
            "secondary" => "text-white bg-secondary-500 hover:bg-secondary-600 focus:ring-secondary-100 dark:bg-secondary-400 dark:hover:bg-secondary-500 dark:focus:ring-secondary-600",
            "success" => "text-white bg-success-700 hover:bg-success-800 focus:ring-success-300 dark:bg-success-600 dark:hover:bg-success-700 dark:focus:ring-success-800",
            "danger" => "text-white bg-danger-700 hover:bg-danger-800 focus:ring-danger-300 dark:bg-danger-600 dark:hover:bg-danger-700 dark:focus:ring-danger-900",
            "warning" => "text-white bg-warning-400 hover:bg-warning-500 focus:ring-warning-300 dark:focus:ring-warning-900",
            "info" => "text-white bg-info-700 hover:bg-info-800 focus:ring-info-300 dark:bg-info-600 dark:hover:bg-info-700 dark:focus:ring-info-900",
            "light" => "text-light-900 bg-light-300 border border-light-300 hover:bg-light-100 focus:ring-light-200 dark:bg-light-800 dark:text-white dark:border-light-600 dark:hover:bg-light-700 dark:hover:border-light-600 dark:focus:ring-light-700",
            "dark" => "text-white bg-dark-800 hover:bg-dark-900 focus:ring-dark-300 dark:bg-dark-800 dark:hover:bg-dark-700 dark:focus:ring-dark-700 dark:border-dark-700",
            "link" => "text-primary-600 dark:text-primary-500 hover:underline",
            _ => null,
        };
    }

    public override string ButtonOutline( Color color, bool outline )
    {
        var name = color?.Name;

        return name switch
        {
            "primary" => "text-primary-700 hover:text-white border border-primary-700 hover:bg-primary-800 focus:ring-primary-300 dark:border-primary-500 dark:text-primary-500 dark:hover:text-white dark:hover:bg-primary-600 dark:focus:ring-primary-800",
            "secondary" => "text-secondary-500 hover:text-white border border-secondary-500 hover:bg-secondary-600 focus:ring-secondary-100 dark:border-secondary-300 dark:text-secondary-300 dark:hover:text-white dark:hover:bg-secondary-400 dark:focus:ring-secondary-600",
            "success" => "text-success-700 hover:text-white border-success-700 hover:bg-success-800 focus:ring-success-300 dark:border-success-500 dark:text-success-500 dark:hover:text-white dark:hover:bg-success-600 dark:focus:ring-success-800",
            "danger" => "text-danger-700 hover:text-white border-danger-700 hover:bg-danger-800 focus:ring-danger-300 dark:border-danger-500 dark:text-danger-500 dark:hover:text-white dark:hover:bg-danger-600 dark:focus:ring-danger-900",
            "warning" => "text-warning-400 hover:text-white border-warning-400 hover:bg-warning-500 focus:ring-warning-300 dark:border-warning-300 dark:text-warning-300 dark:hover:text-white dark:hover:bg-warning-400 dark:focus:ring-warning-900",
            "info" => "text-info-700 hover:text-white border-info-700 hover:bg-info-800 focus:ring-info-300 dark:border-info-400 dark:text-info-400 dark:hover:text-white dark:hover:bg-info-500 dark:focus:ring-info-900",
            "light" => "text-light-900 hover:text-white border-light-300 hover:bg-light-100 focus:ring-light-200 dark:border-light-600 dark:text-white dark:hover:text-white dark:hover:bg-light-700 dark:focus:ring-light-700",
            "dark" => "text-dark-900 hover:text-white border-dark-800 hover:bg-dark-900 focus:ring-dark-300 dark:border-dark-600 dark:text-dark-400 dark:hover:text-white dark:hover:bg-dark-600 dark:focus:ring-dark-800",
            "link" => "",
            _ => null,
        };
    }

    public override string ButtonSize( Size size, bool outline )
    {
        return size switch
        {
            Size.ExtraSmall => "px-3 py-2 text-xs",
            Size.Small => "px-3 py-2 text-sm",
            Size.Medium => "px-6 py-4 text-base",
            Size.Large => "px-6 py-5 text-base",
            Size.ExtraLarge => "px-6 py-6 text-base",
            _ => "px-5 py-2.5 text-sm"
        };
    }

    public override string ButtonBlock( bool outline ) => "w-full";

    public override string ButtonActive( bool outline ) => "active";

    public override string ButtonDisabled( bool outline ) => "cursor-not-allowed opacity-60";

    public override string ButtonLoading( bool outline ) => null;

    #endregion

    #region Buttons

    public override string Buttons( ButtonsRole role, Orientation orientation )
    {
        if ( role == ButtonsRole.Toolbar )
            return "btn-toolbar";

        if ( orientation == Orientation.Vertical )
            return "btn-group-vertical";

        return "group inline-flex rounded-md shadow-sm";
    }

    public override string ButtonsSize( Size size ) => $"btn-group-{ToSize( size )}";

    #endregion

    #region CloseButton

    public override string CloseButton() => "text-gray-400 bg-transparent hover:bg-gray-200 hover:text-gray-900 rounded-lg text-sm p-1.5 ml-auto inline-flex items-center dark:hover:bg-gray-600 dark:hover:text-white";

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

    public override string Tabs( bool pills ) => pills ? "flex flex-wrap text-sm font-medium text-center text-gray-500 dark:text-gray-400" : "flex flex-wrap text-sm font-medium text-center text-gray-500 border-b border-gray-200 dark:border-gray-700 dark:text-gray-400";

    public override string TabsCards() => "card-header-tabs";

    public override string TabsFullWidth() => "w-full";

    public override string TabsJustified() => "nav-justified";

    public override string TabsVertical() => "flex-column";

    public override string TabItem() => "mr-2";

    public override string TabItemActive( bool active ) => null;

    public override string TabItemDisabled( bool disabled ) => null;

    public override string TabLink() => "inline-block p-4 rounded-t-lg";

    public override string TabLinkActive( bool active ) => active ? "text-primary-600 dark:text-primary-500" : "hover:text-gray-600 hover:bg-gray-50 dark:hover:bg-gray-800 dark:hover:text-gray-300";

    public override string TabLinkDisabled( bool disabled ) => disabled ? "opacity-60" : null;

    public override string TabsContent() => null;

    public override string TabPanel() => "p-4 bg-gray-50 rounded-lg dark:bg-gray-800";

    public override string TabPanelActive( bool active ) => active ? null : "hidden";

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

    public override string Carousel() => "relative";

    public override string CarouselSlides() => "relative overflow-hidden rounded-lg h-56 md:h-96";

    public override string CarouselSlide() => "duration-700 ease-in-out absolute inset-0 transition-all transform";

    public override string CarouselSlideActive( bool active ) => null;

    public override string CarouselSlideIndex( int activeSlideIndex, int slideindex, int totalSlides )
    {
        if ( activeSlideIndex == slideindex )
            return "z-20 translate-x-0";

        if ( ( activeSlideIndex == 0 && slideindex == totalSlides - 1 )
            || ( slideindex == activeSlideIndex - 1 ) )
            return "z-10 -translate-x-full";

        if ( ( activeSlideIndex == totalSlides - 1 && slideindex == 0 )
            || ( slideindex == activeSlideIndex + 1 ) )
            return "z-10 translate-x-full";

        return "z-10 translate-x-full hidden";
    }

    public override string CarouselSlideSlidingLeft( bool left ) => null;

    public override string CarouselSlideSlidingRight( bool right ) => null;

    public override string CarouselSlideSlidingPrev( bool previous ) => null;

    public override string CarouselSlideSlidingNext( bool next ) => null;

    public override string CarouselIndicators() => "absolute z-30 flex space-x-3 -translate-x-1/2 bottom-5 left-1/2";

    public override string CarouselIndicator() => "w-3 h-3 rounded-full";

    public override string CarouselIndicatorActive( bool active ) => active ? "bg-white dark:bg-gray-800" : "bg-white/50 dark:bg-gray-800/50 hover:bg-white dark:hover:bg-gray-800";

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

    public override string Card() => "max-w bg-white border border-gray-200 rounded-lg shadow-md dark:bg-gray-800 dark:border-gray-700";

    public override string CardWhiteText() => "text-white";

    public override string CardActions() => "card-actions";

    public override string CardBody() => "p-5";

    public override string CardFooter() => "px-5 py-4";

    public override string CardHeader() => "px-5 py-4";

    public override string CardImage() => "rounded-t-lg";

    public override string CardTitle( bool insideHeader ) => "mb-2 text-2xl font-bold tracking-tight text-gray-900 dark:text-white";

    public override string CardTitleSize( bool insideHeader, int? size ) => null;

    public override string CardSubtitle( bool insideHeader ) => "card-subtitle";

    public override string CardSubtitleSize( bool insideHeader, int size ) => null;

    public override string CardText() => "mb-3 font-normal text-gray-700 dark:text-gray-400";

    public override string CardLink() => "card-link";

    #endregion

    #region ListGroup

    public override string ListGroup() => "w-full text-sm font-medium text-gray-900 bg-white rounded-lg border border-gray-200 dark:bg-gray-700 dark:border-gray-600 dark:text-white";

    public override string ListGroupFlush() => "border-x-0";

    public override string ListGroupItem() => "py-3 px-4 w-full border-b border-gray-200 dark:border-gray-600";

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

    public override string Bar() => "bg-white border-gray-200 px-2 sm:px-4 py-2.5 rounded dark:bg-gray-900";

    public override string BarInitial( bool initial ) => initial ? "b-bar-initial" : null;

    public override string BarAlignment( Alignment alignment ) => FlexAlignment( alignment );

    public override string BarThemeContrast( ThemeContrast themeContrast ) => $"navbar-{ToThemeContrast( themeContrast )} b-bar-{ToThemeContrast( themeContrast )}";

    public override string BarBreakpoint( Breakpoint breakpoint ) => breakpoint != Breakpoint.None && breakpoint != Breakpoint.Mobile ? $"navbar-expand-{ToBreakpoint( breakpoint )}" : null;

    public override string BarMode( BarMode mode ) => $"b-bar-{ToBarMode( mode )}";

    public override string BarItem( BarMode mode, bool hasDropdown ) => mode == Blazorise.BarMode.Horizontal
        ? hasDropdown
            ? null
            : null
        : "b-bar-item";

    public override string BarItemActive( BarMode mode ) => Active();

    public override string BarItemDisabled( BarMode mode ) => Disabled();

    public override string BarItemHasDropdown( BarMode mode ) => null;

    public override string BarItemHasDropdownShow( BarMode mode ) => null;

    public override string BarLink( BarMode mode ) => mode == Blazorise.BarMode.Horizontal ? "block py-2 pl-3 pr-4 text-gray-700 rounded hover:bg-gray-100 md:hover:bg-transparent md:border-0 md:hover:text-primary-700 md:p-0 dark:text-gray-400 md:dark:hover:text-white dark:hover:bg-gray-700 dark:hover:text-white md:dark:hover:bg-transparent" : "b-bar-link";

    public override string BarLinkDisabled( BarMode mode ) => Disabled();

    public override string BarBrand( BarMode mode ) => mode == Blazorise.BarMode.Horizontal ? "flex items-center" : "b-bar-brand";

    public override string BarToggler( BarMode mode, BarTogglerMode togglerMode ) => mode == Blazorise.BarMode.Horizontal ? "navbar-toggler" :
        togglerMode == BarTogglerMode.Popout ? "b-bar-toggler-popout" : "b-bar-toggler-inline";

    public override string BarTogglerCollapsed( BarMode mode, BarTogglerMode togglerMode, bool isShow ) => isShow || mode != Blazorise.BarMode.Horizontal ? null : "collapsed";

    public override string BarMenu( BarMode mode ) => mode == Blazorise.BarMode.Horizontal ? "collapse navbar-collapse" : "b-bar-menu";

    public override string BarMenuShow( BarMode mode ) => Show();

    public override string BarStart( BarMode mode ) => mode == Blazorise.BarMode.Horizontal ? "items-center justify-between hidden w-full md:flex md:w-auto md:order-1 mr-auto" : "b-bar-start";

    public override string BarEnd( BarMode mode ) => mode == Blazorise.BarMode.Horizontal ? "items-center justify-between hidden w-full md:flex md:w-auto md:order-1 ml-auto" : "b-bar-end";

    public override string BarDropdown( BarMode mode, bool isBarDropDownSubmenu ) => mode == Blazorise.BarMode.Horizontal
        ? null
        : "b-bar-dropdown";

    public override string BarDropdownShow( BarMode mode ) => Show();

    public override string BarDropdownToggle( BarMode mode, bool isBarDropDownSubmenu ) => mode == Blazorise.BarMode.Horizontal
        ? isBarDropDownSubmenu
            ? "flex items-center justify-between w-full py-2 pl-3 pr-4 font-medium text-gray-700 border-b border-gray-100 hover:bg-gray-50 md:hover:bg-transparent md:border-0 md:hover:text-primary-700 md:p-0 md:w-auto dark:text-gray-400 dark:hover:text-white dark:focus:text-white dark:border-gray-700 dark:hover:bg-gray-700 md:dark:hover:bg-transparent"
            : "flex items-center justify-between w-full py-2 pl-3 pr-4 font-medium text-gray-700 border-b border-gray-100 hover:bg-gray-50 md:hover:bg-transparent md:border-0 md:hover:text-primary-700 md:p-0 md:w-auto dark:text-gray-400 dark:hover:text-white dark:focus:text-white dark:border-gray-700 dark:hover:bg-gray-700 md:dark:hover:bg-transparent"
        : "b-bar-link b-bar-dropdown-toggle";

    public override string BarDropdownToggleDisabled( BarMode mode, bool isBarDropDownSubmenu, bool disabled )
        => mode == Blazorise.BarMode.Horizontal && disabled ? "disabled" : null;

    public override string BarDropdownItem( BarMode mode ) => mode == Blazorise.BarMode.Horizontal ? "block px-4 py-2 hover:bg-gray-100 dark:hover:bg-gray-600 dark:hover:text-white" : "b-bar-dropdown-item";

    public override string BarTogglerIcon( BarMode mode ) => "navbar-toggler-icon";

    public override string BarDropdownDivider( BarMode mode ) => "dropdown-divider";

    public override string BarDropdownMenu( BarMode mode ) => mode == Blazorise.BarMode.Horizontal ? "z-10 font-normal bg-white divide-y divide-gray-100 rounded shadow w-44 dark:bg-gray-700 dark:divide-gray-600" : "b-bar-dropdown-menu";

    public override string BarDropdownMenuVisible( BarMode mode, bool visible ) => visible ? "block" : "hidden";

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

    public override string Row() => "grid grid-cols-12 gap-x-4";

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

    public override string Column( ColumnWidth columnWidth, Breakpoint breakpoint, bool offset, int startFrom )
    {
        var columnWidthValue = ToColumnWidth( columnWidth );

        if ( offset && columnWidthValue != null && columnWidthValue != "auto"
            && int.TryParse( columnWidthValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out var columnWidthNumber ) )
        {
            if ( breakpoint != Blazorise.Breakpoint.None && breakpoint != Blazorise.Breakpoint.Mobile )
            {
                return $"{ToBreakpoint( breakpoint )}:col-start-{startFrom + 1}";
            }

            return $"col-start-{startFrom + 1}";
        }

        if ( breakpoint != Blazorise.Breakpoint.None && breakpoint != Blazorise.Breakpoint.Mobile )
        {
            return $"{ToBreakpoint( breakpoint )}:col-span-{columnWidthValue}";
        }

        return $"col-span-{columnWidthValue}";
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

    public override string Alert() => "p-4 mb-4 text-sm rounded-lg";

    public override string AlertColor( Color color )
    {
        var name = color?.Name;

        return name switch
        {
            "primary" => "text-primary-800 bg-primary-300 dark:bg-primary-500 dark:text-primary-800",
            "secondary" => "text-secondary-500 bg-secondary-300 dark:bg-secondary-100 dark:text-secondary-600",
            "success" => "text-success-700 bg-success-100 dark:bg-success-200 dark:text-success-800",
            "danger" => "text-danger-700 bg-danger-100 dark:bg-danger-200 dark:text-danger-800",
            "warning" => "text-warning-700 bg-warning-100 dark:bg-warning-200 dark:text-warning-800",
            "info" => "text-info-700 bg-info-100 dark:bg-info-200 dark:text-info-800",
            "light" => "text-light-500 bg-light-100 dark:bg-light-100 dark:text-light-600",
            "dark" => "text-dark-100 bg-dark-800 dark:bg-dark-300 dark:text-dark-700",
            "link" => "text-primary-600 dark:text-primary-500 hover:underline",
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

    public override string Modal() => "fixed top-0 left-0 right-0 z-50 w-full p-4 overflow-x-hidden overflow-y-auto md:inset-0 h-modal md:h-full justify-center items-center";

    public override string ModalFade() => null;

    public override string ModalFade( bool animation ) => null;

    public override string ModalVisible( bool visible ) => visible ? "flex" : "hidden";

    public override string ModalBackdrop() => "bg-gray-900 bg-opacity-50 dark:bg-opacity-80 fixed inset-0 z-40";

    public override string ModalBackdropFade() => null;

    public override string ModalBackdropVisible( bool visible ) => null;

    public override string ModalContent( bool dialog ) => "relative bg-white rounded-lg shadow dark:bg-gray-700";

    public override string ModalContentSize( ModalSize modalSize ) => null;

    public override string ModalContentFullscreen( bool fullscreen ) => null;

    public override string ModalContentCentered( bool centered ) => null;

    public override string ModalContentScrollable( bool scrollable ) => null;

    public override string ModalBody() => "p-6 space-y-6";

    public override string ModalHeader() => "flex items-start justify-between p-4 border-b rounded-t dark:border-gray-600";

    public override string ModalFooter() => "flex items-center p-6 space-x-2 border-t border-gray-200 rounded-b dark:border-gray-600";

    public override string ModalTitle() => "text-xl font-semibold text-gray-900 dark:text-white";

    #endregion

    #region Pagination

    public override string Pagination() => "flex list-style-none";

    public override string PaginationSize( Size size ) => $"{Pagination()}-{ToSize( size )}";

    public override string PaginationItem() => "page-item";

    public override string PaginationItemActive() => "active";

    public override string PaginationItemDisabled() => "disabled";

    public override string PaginationLink() => "page-link relative block py-1.5 px-3 rounded border-0 bg-transparent outline-none transition-all duration-300 rounded text-gray-800 focus:shadow-none";

    public override string PaginationLinkActive() => "bg-primary-600";

    public override string PaginationLinkDisabled() => null;

    #endregion

    #region Progress

    public override string Progress() => "w-full bg-gray-200 rounded-full dark:bg-gray-700";

    public override string ProgressSize( Size size )
    {
        return size switch
        {
            Blazorise.Size.ExtraSmall => "h-1",
            Blazorise.Size.Small => "h-1.5",
            Blazorise.Size.Medium => "h-3",
            Blazorise.Size.Large => "h-4",
            Blazorise.Size.ExtraLarge => "h-6",
            _ => "h-2.5",
        };
    }

    public override string ProgressColor( Color color ) => null;

    public override string ProgressStriped() => null;

    public override string ProgressAnimated() => null;

    public override string ProgressWidth( int width ) => null;

    public override string ProgressBar() => "text-xs shadow-none flex flex-col text-center whitespace-nowrap justify-center";

    public override string ProgressBarSize( Size size ) => null;

    public override string ProgressBarColor( Color color )
    {
        var name = color?.Name;

        return name switch
        {
            "primary" => "bg-primary-600 text-primary-100",
            "secondary" => "bg-secondary-600 text-secondary-100",
            "success" => "bg-success-600 text-success-100",
            "danger" => "bg-danger-600 text-danger-100",
            "warning" => "bg-warning-400 text-warning-100",
            "info" => "bg-info-600 text-info-100",
            "light" => "bg-light-100 text-light-600",
            "dark" => "bg-dark-600 text-dark-100",
            _ => null,
        };
    }

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

    public override string Badge() => "text-xs font-semibold mr-2 px-2.5 py-0.5 rounded";

    public override string BadgeColor( Color color )
    {
        var name = color?.Name;

        return name switch
        {
            "primary" => "bg-primary-100 text-primary-800 dark:bg-primary-200 dark:text-primary-800",
            "secondary" => "bg-secondary-100 text-secondary-800 dark:bg-secondary-200 dark:text-secondary-800",
            "success" => "bg-success-100 text-success-800 dark:bg-success-200 dark:text-success-900",
            "danger" => "bg-danger-100 text-danger-800 dark:bg-danger-200 dark:text-danger-900",
            "warning" => "bg-warning-100 text-warning-800 dark:bg-warning-200 dark:text-warning-900",
            "info" => "bg-info-100 text-info-800 dark:bg-info-200 dark:text-info-900",
            "light" => "bg-light-100 text-light-800 dark:bg-light-200 dark:text-light-800",
            "dark" => "bg-dark-800 text-dark-100 dark:bg-dark-300 dark:text-dark-700",
            "link" => "text-primary-600 dark:text-primary-500 hover:underline",
            _ => null,
        };
    }

    public override string BadgePill() => "rounded-full";

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

    public override string Breadcrumb() => "flex px-5 py-3 text-gray-700 border border-gray-200 rounded-lg bg-gray-50 dark:bg-gray-800 dark:border-gray-700";

    public override string BreadcrumbItem() => null;

    public override string BreadcrumbItemActive() => null;

    public override string BreadcrumbLink() => "flex items-center";

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

    public override string ToModalSize( ModalSize modalSize )
    {
        return modalSize switch
        {
            Blazorise.ModalSize.Small => "max-w-md md:h-aut",
            Blazorise.ModalSize.Large => "max-w-4xl md:h-auto",
            Blazorise.ModalSize.ExtraLarge => "max-w-7xl md:h-auto",
            _ => "max-w-2xl md:h-auto",
        };
    }

    #endregion

    public override bool UseCustomInputStyles { get; set; } = true;

    public override string Provider => "Tailwind";
}