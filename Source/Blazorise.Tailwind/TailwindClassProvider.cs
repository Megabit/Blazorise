#region Using directives
using System;
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
        ? "text-gray-900 border-none rounded-lg focus:ring-primary-500 focus:border-primary-500 block w-full dark:bg-gray-800 dark:text-white dark:focus:ring-primary-500 dark:focus:border-primary-500 disabled:cursor-not-allowed disabled:opacity-75"
        : "bg-gray-50 border border-gray-300 text-gray-900 rounded-lg focus:ring-primary-500 focus:border-primary-500 block w-full dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-primary-500 dark:focus:border-primary-500 disabled:cursor-not-allowed disabled:opacity-75";

    public override string TextEditSize( Size size )
    {
        return size switch
        {
            Size.ExtraSmall => "p-1.5 sm:text-xs",
            Size.Small => "p-2 sm:text-xs",
            Size.Medium => "p-3 text-md",
            Size.Large => "p-4 sm:text-md",
            Size.ExtraLarge => "p-4 sm:text-lg",
            _ => "p-2.5 text-sm"
        };
    }

    public override string TextEditColor( Color color ) => color?.Name?.Length > 0 ? $"text-{ToColor( color )}" : null;

    public override string TextEditValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

    #endregion

    #region MemoEdit

    public override string MemoEdit( bool plaintext ) => plaintext
        ? "block w-full text-gray-900 border-none focus:ring-primary-500 focus:border-primary-500 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-primary-500 dark:focus:border-primary-500"
        : "block w-full text-gray-900 bg-gray-50 rounded-lg border border-gray-300 focus:ring-primary-500 focus:border-primary-500 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-primary-500 dark:focus:border-primary-500";

    public override string MemoEditSize( Size size ) => TextEditSize( size );

    public override string MemoEditValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

    #endregion

    #region Select

    public override string Select() => TextEdit( false );

    public override string SelectMultiple() => null;

    public override string SelectSize( Size size ) => TextEditSize( size );

    public override string SelectValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

    #endregion

    #region NumericEdit

    public override string NumericEdit( bool plaintext ) => TextEdit( plaintext );

    public override string NumericEditSize( Size size ) => TextEditSize( size );

    public override string NumericEditColor( Color color ) => color?.Name?.Length > 0 ? $"text-{ToColor( color )}" : null;

    public override string NumericEditValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

    #endregion

    #region DateEdit

    public override string DateEdit( bool plaintext ) => TextEdit( plaintext );

    public override string DateEditSize( Size size ) => TextEditSize( size );

    public override string DateEditColor( Color color ) => color?.Name?.Length > 0 ? $"text-{ToColor( color )}" : null;

    public override string DateEditValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

    #endregion

    #region TimeEdit

    public override string TimeEdit( bool plaintext ) => TextEdit( plaintext );

    public override string TimeEditSize( Size size ) => TextEditSize( size );

    public override string TimeEditColor( Color color ) => color?.Name?.Length > 0 ? $"text-{ToColor( color )}" : null;

    public override string TimeEditValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

    #endregion

    #region ColorEdit

    public override string ColorEdit() => TextEdit( false );

    public override string ColorEditSize( Size size )
    {
        return size switch
        {
            Size.ExtraSmall => "p-1.5 sm:text-xs h-6",
            Size.Small => "p-2 sm:text-xs h-8",
            Size.Medium => "p-3 text-md h-12",
            Size.Large => "p-4 sm:text-md h-14",
            Size.ExtraLarge => "p-4 sm:text h-16",
            _ => "p-2 text-sm h-10"
        };
    }

    #endregion

    #region DatePicker

    public override string DatePicker( bool plaintext ) => TextEdit( plaintext );

    public override string DatePickerSize( Size size ) => TextEditSize( size );

    public override string DatePickerColor( Color color ) => color?.Name?.Length > 0 ? $"text-{ToColor( color )}" : null;

    public override string DatePickerValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

    #endregion

    #region TimePicker

    public override string TimePicker( bool plaintext ) => TextEdit( plaintext );

    public override string TimePickerSize( Size size ) => TextEditSize( size );

    public override string TimePickerColor( Color color ) => color?.Name?.Length > 0 ? $"text-{ToColor( color )}" : null;

    public override string TimePickerValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

    #endregion

    #region ColorPicker

    public override string ColorPicker() => TextEdit( false );

    public override string ColorPickerSize( Size size )
    {
        return size switch
        {
            Size.ExtraSmall => "pl-8 py-1.2 sm:text-xs",
            Size.Small => "pl-8 py-2 sm:text-xs",
            Size.Medium => "pl-8 py-3 text-md",
            Size.Large => "pl-8 py-4 sm:text-md",
            Size.ExtraLarge => "pl-8 py-4 sm:text-lg",
            _ => "pl-8 py-2.5 text-sm"
        };
    }

    #endregion

    #region NumericPicker

    public override string NumericPicker( bool plaintext ) => TextEdit( plaintext );

    public override string NumericPickerSize( Size size ) => TextEditSize( size );

    public override string NumericPickerColor( Color color ) => color?.Name?.Length > 0 ? $"text-{ToColor( color )}" : null;

    public override string NumericPickerValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

    #endregion

    #region InputMask

    public override string InputMask( bool plaintext ) => TextEdit( plaintext );

    public override string InputMaskSize( Size size ) => TextEditSize( size );

    public override string InputMaskColor( Color color ) => color?.Name?.Length > 0 ? $"text-{ToColor( color )}" : null;

    public override string InputMaskValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

    #endregion

    #region Check

    public override string Check() => "text-primary-600 bg-gray-100 rounded border-gray-300 focus:ring-primary-500 dark:focus:ring-primary-600 dark:ring-offset-gray-800 focus:ring-2 dark:bg-gray-700 dark:border-gray-600";

    public override string CheckSize( Size size )
    {
        return size switch
        {
            Size.ExtraSmall => "w-2 h-2",
            Size.Small => "w-3 h-3",
            Size.Medium => "w-5 h-5",
            Size.Large => "w-6 h-6",
            Size.ExtraLarge => "w-8 h-8",
            _ => "w-4 h-4"
        };
    }

    public override string CheckInline() => null;

    public override string CheckCursor( Cursor cursor ) => $"cursor-{ToCursor( cursor )}";

    public override string CheckValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

    #endregion

    #region RadioGroup

    public override string RadioGroup( bool buttons, Orientation orientation )
        => buttons
            ? orientation == Orientation.Horizontal
                ? "inline-flex flex-row items-center text-sm font-medium"
                : "inline-flex flex-col items-start justify-center text-sm font-medium"
                    : "inline-flex flex-wrap";

    public override string RadioGroupSize( bool buttons, Orientation orientation, Size size ) => null;

    public override string RadioGroupValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

    #endregion

    #region Radio

    public override string Radio( bool button ) => button
        ? "absolute cursor-none"
        : "text-primary-600 bg-gray-100 border-gray-300 focus:ring-primary-500 dark:focus:ring-primary-600 dark:ring-offset-gray-800 focus:ring-2 dark:bg-gray-700 dark:border-gray-600";

    public override string RadioSize( bool button, Size size )
    {
        return size switch
        {
            Size.ExtraSmall => "w-2 h-2",
            Size.Small => "w-3 h-3",
            Size.Medium => "w-5 h-5",
            Size.Large => "w-6 h-6",
            Size.ExtraLarge => "w-8 h-8",
            _ => "w-4 h-4"
        };
    }

    public override string RadioInline( bool inline ) => inline
        ? UseCustomInputStyles ? "custom-control-inline" : "form-check-inline"
        : null;

    public override string RadioCursor( Cursor cursor ) => $"cursor-{ToCursor( cursor )}";

    public override string RadioValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

    #endregion

    #region Switch

    public override string Switch() => "sr-only peer";

    public override string SwitchColor( Color color ) => $"{Switch()}-{ToColor( color )}";

    public override string SwitchSize( Size size ) => null;

    public override string SwitchChecked( bool @checked ) => null;

    public override string SwitchCursor( Cursor cursor ) => null;

    public override string SwitchValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

    #endregion

    #region FileEdit

    public override string FileEdit() => "block w-full text-gray-900 border border-gray-300 rounded-lg cursor-pointer bg-gray-50 dark:text-gray-400 focus:outline-none dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400";

    public override string FileEditSize( Size size ) => TextEditSize( size );

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
            Blazorise.LabelType.Switch => "inline-flex relative items-center cursor-pointer",
            Blazorise.LabelType.File => UseCustomInputStyles ? "custom-file-label" : null,
            _ => null,
        };
    }

    public override string LabelCursor( Cursor cursor ) => $"cursor-{ToCursor( cursor )}";

    #endregion

    #region Help

    public override string Help() => "form-text text-muted";

    #endregion

    #region Validation

    public override string ValidationSuccess() => "mt-2 text-sm text-success-600 dark:text-success-500";

    public override string ValidationSuccessTooltip() => "valid-tooltip";

    public override string ValidationError() => "mt-2 text-sm text-danger-600 dark:text-danger-500";

    public override string ValidationErrorTooltip() => "invalid-tooltip";

    public override string ValidationNone() => "form-text text-muted";

    public override string ValidationSummary() => "text-danger";

    public override string ValidationSummaryError() => "text-danger";

    #endregion

    #region Fields

    public override string Fields() => "flex flex-row flex-wrap -mx-2";

    public override string FieldsBody() => null;

    public override string FieldsColumn() => "basis-0 grow pl-2 pr-2 w-full";

    #endregion

    #region Field

    public override string Field() => "mb-3";

    public override string FieldHorizontal() => "flex flex-wrap flex-row";

    public override string FieldColumn() => "relative basis-0 grow pl-2 pr-2 w-full max-w-full";

    public override string FieldJustifyContent( JustifyContent justifyContent ) => ToJustifyContent( justifyContent );

    public override string FieldValidation( ValidationStatus validationStatus ) => null;

    #endregion

    #region FieldLabel

    public override string FieldLabel( bool horizontal ) => horizontal
        ? "block my-auto text-sm font-medium text-gray-900 dark:text-white"
        : "block my-2 text-sm font-medium text-gray-900 dark:text-white";

    public override string FieldLabelRequiredIndicator( bool requiredIndicator )
        => requiredIndicator
            ? "after:content-[' *'] after:[color:var(--b-theme-danger, --btw-color-danger-500)]"
            : null;

    #endregion

    #region FieldBody

    public override string FieldBody() => null;

    #endregion

    #region FieldHelp

    public override string FieldHelp() => "block mt-1 text-xs text-gray-600";

    #endregion

    #region FocusTrap

    public override string FocusTrap() => "contents";

    #endregion

    #region Control

    public override string ControlCheck() => "inline-flex items-center mr-4";

    public override string ControlRadio() => "inline-flex items-center mr-4";

    public override string ControlSwitch() => "inline-flex relative items-center cursor-pointer mr-4";

    public override string ControlFile() => UseCustomInputStyles ? "custom-file" : "form-group";

    public override string ControlText() => null;

    #endregion

    #region Addons

    public override string Addons() => "b-addons flex";

    public override string AddonsSize( Size size ) => null;

    public override string AddonsHasButton( bool hasButton ) => null;

    public override string Addon( AddonType addonType )
    {
        return addonType switch
        {
            AddonType.Start => "b-addon-start inline-flex items-center text-gray-900 bg-gray-200 border border-r-0 border-gray-300 rounded-l-md dark:bg-gray-600 dark:text-gray-400 dark:border-gray-600",
            AddonType.End => "b-addon-end inline-flex items-center text-gray-900 bg-gray-200 border border-l-0 border-gray-300 rounded-r-md dark:bg-gray-600 dark:text-gray-400 dark:border-gray-600",
            _ => "b-addon-body contents",
        };
    }

    public override string AddonSize( Size size )
    {
        return size switch
        {
            Size.ExtraSmall => "sm:text-xs",
            Size.Small => "sm:text-xs",
            Size.Medium => "text-md",
            Size.Large => "sm:text-md",
            Size.ExtraLarge => "sm:text-lg",
            _ => "text-sm"
        };
    }

    public override string AddonLabel() => "b-addon-label block px-3 font-medium text-gray-900 dark:text-white";

    //public override string AddonContainer() => null;

    #endregion

    #region Inline

    public override string Inline() => "flex flex-row flex-wrap";

    #endregion

    #region Button

    public override string Button( bool outline ) => outline
        ? "b-button inline-flex items-center border focus:ring-4 focus:outline-none font-medium text-center"
        : "b-button inline-flex items-center focus:ring-4 font-medium focus:outline-none";

    public override string ButtonColor( Color color, bool outline )
    {
        var name = color?.Name;

        if ( outline )
        {
            return name switch
            {
                "primary" => "b-button-outline-primary text-700 hover:text-white border border-primary-700 hover:bg-primary-800 focus:ring-primary-300 dark:border-primary-500 dark:text-primary-500 dark:hover:text-white dark:hover:bg-primary-600 dark:focus:ring-primary-800",
                "secondary" => "b-button-outline-secondary text-secondary-500 hover:text-white border border-secondary-500 hover:bg-secondary-600 focus:ring-secondary-100 dark:border-secondary-300 dark:text-secondary-300 dark:hover:text-white dark:hover:bg-secondary-400 dark:focus:ring-secondary-600",
                "success" => "b-button-outline-success text-success-700 hover:text-white border-success-700 hover:bg-success-800 focus:ring-success-300 dark:border-success-500 dark:text-success-500 dark:hover:text-white dark:hover:bg-success-600 dark:focus:ring-success-800",
                "danger" => "b-button-outline-danger text-danger-700 hover:text-white border-danger-700 hover:bg-danger-800 focus:ring-danger-300 dark:border-danger-500 dark:text-danger-500 dark:hover:text-white dark:hover:bg-danger-600 dark:focus:ring-danger-900",
                "warning" => "b-button-outline-warning text-warning-400 hover:text-white border-warning-400 hover:bg-warning-500 focus:ring-warning-300 dark:border-warning-300 dark:text-warning-300 dark:hover:text-white dark:hover:bg-warning-400 dark:focus:ring-warning-900",
                "info" => "b-button-outline-info text-info-700 hover:text-white border-info-700 hover:bg-info-800 focus:ring-info-300 dark:border-info-400 dark:text-info-400 dark:hover:text-white dark:hover:bg-info-500 dark:focus:ring-info-900",
                "light" => "b-button-outline-light text-light-200 hover:text-white border-light-300 hover:bg-light-100 focus:ring-light-200 dark:border-light-600 dark:text-white dark:hover:text-white dark:hover:bg-light-700 dark:focus:ring-light-700",
                "dark" => "b-button-outline-dark text-dark-900 hover:text-white border-dark-800 hover:bg-dark-900 focus:ring-dark-300 dark:border-dark-600 dark:text-dark-400 dark:hover:text-white dark:hover:bg-dark-600 dark:focus:ring-dark-800",
                "link" => "b-button-outline-link text-primary-600 dark:text-primary-500 hover:underline",
                _ => null,
            };
        }

        return name switch
        {
            "primary" => "b-button-primary text-white bg-primary-700 hover:bg-primary-800 focus:ring-primary-300 dark:bg-primary-600 dark:hover:bg-primary-700 dark:focus:ring-primary-800",
            "secondary" => "b-button-secondary text-white bg-secondary-500 hover:bg-secondary-600 focus:ring-secondary-100 dark:bg-secondary-400 dark:hover:bg-secondary-500 dark:focus:ring-secondary-600",
            "success" => "b-button-success text-white bg-success-700 hover:bg-success-800 focus:ring-success-300 dark:bg-success-600 dark:hover:bg-success-700 dark:focus:ring-success-800",
            "danger" => "b-button-danger text-white bg-danger-700 hover:bg-danger-800 focus:ring-danger-300 dark:bg-danger-600 dark:hover:bg-danger-700 dark:focus:ring-danger-900",
            "warning" => "b-button-warning text-white bg-warning-400 hover:bg-warning-500 focus:ring-warning-300 dark:focus:ring-warning-900",
            "info" => "b-button-info text-white bg-info-700 hover:bg-info-800 focus:ring-info-300 dark:bg-info-600 dark:hover:bg-info-700 dark:focus:ring-info-900",
            "light" => "b-button-light text-light-900 bg-light-300 border border-light-300 hover:bg-light-100 focus:ring-light-200 dark:bg-light-800 dark:text-white dark:border-light-600 dark:hover:bg-light-700 dark:hover:border-light-600 dark:focus:ring-light-700",
            "dark" => "b-button-dark text-white bg-dark-800 hover:bg-dark-900 focus:ring-dark-300 dark:bg-dark-800 dark:hover:bg-dark-700 dark:focus:ring-dark-700 dark:border-dark-700",
            "link" => "b-button-link text-primary-600 dark:text-primary-500 hover:underline",
            _ => null,
        };
    }

    public override string ButtonSize( Size size, bool outline )
    {
        return size switch
        {
            Size.ExtraSmall => "px-3 py-2 text-xs",
            Size.Small => "px-3 py-2 text-sm",
            Size.Medium => "px-6 py-3 text-base",
            Size.Large => "px-6 py-3.5 text-base",
            Size.ExtraLarge => "px-6 py-4 text-base",
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
            return "group b-button-group-toolbar inline-flex gap-x-2";

        if ( orientation == Orientation.Vertical )
            return "b-button-group-vertical";

        return "group b-button-group inline-flex gap-x-0";
    }

    public override string ButtonsSize( Size size ) => $"btn-group-{ToSize( size )}";

    #endregion

    #region CloseButton

    public override string CloseButton() => "text-sm inline-flex";

    #endregion

    #region Dropdown

    public override string Dropdown( bool isDropdownSubmenu ) => isDropdownSubmenu
        ? "b-dropdown b-dropdown-submenu relative inline-flex w-full"
        : "b-dropdown relative inline-flex";

    public override string DropdownDisabled() => "b-dropdown-disabled";

    public override string DropdownGroup() => "b-dropdown-group align-middle";

    public override string DropdownObserverShow() => DropdownShow();

    public override string DropdownShow() => "b-dropdown-show";

    public override string DropdownRight() => null;

    public override string DropdownItem() => "b-dropdown-item block py-2 px-4 hover:bg-gray-100 dark:hover:bg-gray-600 dark:hover:text-white";

    public override string DropdownItemActive( bool active ) => "b-dropdown-item-active";

    public override string DropdownItemDisabled( bool disabled ) => disabled ? "b-dropdown-item-disabled cursor-not-allowed opacity-60" : "cursor-pointer";

    public override string DropdownDivider() => "b-dropdown-divider";

    public override string DropdownHeader() => "b-dropdown-header py-3 px-4 text-sm text-gray-900 dark:text-white";

    public override string DropdownMenu() => "b-dropdown-menu z-10 w-max bg-white rounded divide-y divide-gray-100 shadow dark:bg-gray-700";

    public override string DropdownMenuPositionStrategy( DropdownPositionStrategy dropdownPositionStrategy )
        => $"max-w-max top-0 left-0 {( dropdownPositionStrategy == DropdownPositionStrategy.Fixed ? "fixed" : "absolute" )}";

    public override string DropdownFixedHeaderVisible( bool visible )
        => visible ? "!z-20" : null;

    public override string DropdownMenuSelector() => "b-dropdown-menu>ul";

    public override string DropdownMenuScrollable() => "b-dropdown-menu-scrollable max-h-[var(--dropdown-list-menu-max-height)] overflow-y-scroll";

    //public override string DropdownMenuBody() => null;

    public override string DropdownMenuVisible( bool visible ) => visible
        ? "b-dropdown-menu-show block"
        : "b-dropdown-menu-hide hidden";

    public override string DropdownMenuRight() => "b-dropdown-menu-right";

    public override string DropdownToggle( bool isDropdownSubmenu, bool outline )
    {
        var sb = new StringBuilder( isDropdownSubmenu
            ? "b-dropdown-toggle-submenu block flex flex-row justify-between w-full py-2 px-4 hover:bg-gray-100 dark:hover:bg-gray-600 dark:hover:text-white"
            : "b-button b-dropdown-toggle focus:outline-none font-medium text-sm text-center inline-flex items-center" );

        if ( outline )
        {
            sb.Append( " focus:ring-4" );
        }

        return sb.ToString();
    }

    public override string DropdownToggleSelector( bool isDropdownSubmenu ) => isDropdownSubmenu
        ? "b-dropdown-toggle-submenu"
        : "b-button b-dropdown-toggle";

    public override string DropdownToggleColor( Color color, bool outline )
    {
        var name = color?.Name;

        if ( outline )
        {
            return name switch
            {
                "primary" => "b-button-outline-primary text-700 hover:text-white border border-primary-700 hover:bg-primary-800 focus:ring-primary-300 dark:border-primary-500 dark:text-primary-500 dark:hover:text-white dark:hover:bg-primary-600 dark:focus:ring-primary-800",
                "secondary" => "b-button-outline-secondary text-secondary-500 hover:text-white border border-secondary-500 hover:bg-secondary-600 focus:ring-secondary-100 dark:border-secondary-300 dark:text-secondary-300 dark:hover:text-white dark:hover:bg-secondary-400 dark:focus:ring-secondary-600",
                "success" => "b-button-outline-success text-success-700 hover:text-white border-success-700 hover:bg-success-800 focus:ring-success-300 dark:border-success-500 dark:text-success-500 dark:hover:text-white dark:hover:bg-success-600 dark:focus:ring-success-800",
                "danger" => "b-button-outline-danger text-danger-700 hover:text-white border-danger-700 hover:bg-danger-800 focus:ring-danger-300 dark:border-danger-500 dark:text-danger-500 dark:hover:text-white dark:hover:bg-danger-600 dark:focus:ring-danger-900",
                "warning" => "b-button-outline-warning text-warning-400 hover:text-white border-warning-400 hover:bg-warning-500 focus:ring-warning-300 dark:border-warning-300 dark:text-warning-300 dark:hover:text-white dark:hover:bg-warning-400 dark:focus:ring-warning-900",
                "info" => "b-button-outline-info text-info-700 hover:text-white border-info-700 hover:bg-info-800 focus:ring-info-300 dark:border-info-400 dark:text-info-400 dark:hover:text-white dark:hover:bg-info-500 dark:focus:ring-info-900",
                "light" => "b-button-outline-light text-light-200 hover:text-white border-light-300 hover:bg-light-100 focus:ring-light-200 dark:border-light-600 dark:text-white dark:hover:text-white dark:hover:bg-light-700 dark:focus:ring-light-700",
                "dark" => "b-button-outline-dark text-dark-900 hover:text-white border-dark-800 hover:bg-dark-900 focus:ring-dark-300 dark:border-dark-600 dark:text-dark-400 dark:hover:text-white dark:hover:bg-dark-600 dark:focus:ring-dark-800",
                "link" => "b-button-outline-link text-primary-600 dark:text-primary-500 hover:underline",
                _ => null,
            };
        }

        return name switch
        {
            "primary" => "b-button-primary text-white bg-primary-700 hover:bg-primary-800 focus:ring-primary-300 dark:bg-primary-600 dark:hover:bg-primary-700 dark:focus:ring-primary-800",
            "secondary" => "b-button-secondary text-white bg-secondary-500 hover:bg-secondary-600 focus:ring-secondary-100 dark:bg-secondary-400 dark:hover:bg-secondary-500 dark:focus:ring-secondary-600",
            "success" => "b-button-success text-white bg-success-700 hover:bg-success-800 focus:ring-success-300 dark:bg-success-600 dark:hover:bg-success-700 dark:focus:ring-success-800",
            "danger" => "b-button-danger text-white bg-danger-700 hover:bg-danger-800 focus:ring-danger-300 dark:bg-danger-600 dark:hover:bg-danger-700 dark:focus:ring-danger-900",
            "warning" => "b-button-warning text-white bg-warning-400 hover:bg-warning-500 focus:ring-warning-300 dark:focus:ring-warning-900",
            "info" => "b-button-info text-white bg-info-700 hover:bg-info-800 focus:ring-info-300 dark:bg-info-600 dark:hover:bg-info-700 dark:focus:ring-info-900",
            "light" => "b-button-light text-light-900 bg-light-300 border border-light-300 hover:bg-light-100 focus:ring-light-200 dark:bg-light-800 dark:text-white dark:border-light-600 dark:hover:bg-light-700 dark:hover:border-light-600 dark:focus:ring-light-700",
            "dark" => "b-button-dark text-white bg-dark-800 hover:bg-dark-900 focus:ring-dark-300 dark:bg-dark-800 dark:hover:bg-dark-700 dark:focus:ring-dark-700 dark:border-dark-700",
            "link" => "b-button-link text-primary-600 dark:text-primary-500 hover:underline",
            _ => null,
        };
    }

    public override string DropdownToggleSize( Size size, bool outline )
    {
        return size switch
        {
            Size.ExtraSmall => "px-3 py-2 text-xs",
            Size.Small => "px-3 py-2 text-sm",
            Size.Medium => "px-6 py-4 text-base",
            Size.Large => "px-6 py-5 text-base",
            Size.ExtraLarge => "px-6 py-6 text-base",
            _ => "px-4 py-2.5 text-sm"
        };
    }

    public override string DropdownToggleSplit( bool split ) => split
        ? "b-dropdown-toggle-split rounded-l-0 rounded-r-lg"
        : "rounded-lg";

    public override string DropdownToggleIcon( bool visible ) => visible ? null : "b-dropdown-toggle-hidden";

    public override string DropdownDirection( Direction direction ) => direction switch
    {
        Direction.Up => "dropup",
        Direction.End => "dropright",
        Direction.Start => "dropleft",
        _ => null,
    };

    #endregion

    #region Tabs

    public override string Tabs( bool pills ) => pills
        ? "flex flex-wrap text-sm font-medium text-center text-gray-500 dark:text-gray-400"
        : "flex flex-wrap text-sm font-medium text-center text-gray-500 border-b border-gray-200 dark:border-gray-700 dark:text-gray-400";

    public override string TabsCards() => "card-header-tabs";

    public override string TabsFullWidth() => "w-full";

    public override string TabsJustified() => "nav-justified";

    public override string TabsVertical() => "flex-col";

    public override string TabItem() => "mr-2";

    public override string TabItemActive( bool active ) => null;

    public override string TabItemDisabled( bool disabled ) => null;

    public override string TabLink( TabPosition tabPosition )
    {
        return tabPosition switch
        {
            TabPosition.Start => "w-full inline-block p-4 rounded-l-lg border-transparent border-r-2",
            TabPosition.End => "w-full inline-block p-4 rounded-r-lg border-transparent border-l-2",
            TabPosition.Bottom => "inline-block p-4 rounded-b-lg border-transparent border-t-2",
            _ => "inline-block p-4 rounded-t-lg border-transparent border-b-2"
        };
    }
    public override string TabLinkActive( bool active ) => active
        ? "text-primary-600 hover:text-primary-600 dark:text-primary-500 dark:hover:text-primary-500 border-primary-600 dark:border-primary-500"
        : "dark:border-transparent text-gray-500 hover:text-gray-600 dark:text-gray-400 border-gray-100 hover:border-gray-300 dark:border-gray-700 dark:hover:text-gray-300";

    public override string TabLinkDisabled( bool disabled )
        => disabled ? "cursor-not-allowed opacity-60" : "cursor-pointer";

    public override string TabsContent() => null;

    public override string TabPanel() => "p-4 bg-gray-50 rounded-lg dark:bg-gray-800";

    public override string TabPanelActive( bool active ) => active ? null : "hidden";

    #endregion

    #region Steps

    public override string Steps() => "b-steps relative flex justify-between w-full list-none overflow-hidden";

    public override string StepItem() => "b-step-item flex-auto h-16";

    public override string StepItemActive( bool active ) => active ? "b-step-active" : null;

    public override string StepItemCompleted( bool completed ) => completed ? "b-step-completed" : null;

    public override string StepItemColor( Color color ) => null;

    public override string StepItemMarker() => "b-step-item-head-icon my-6 mr-2 flex justify-center items-center rounded-full w-7 h-7 text-sm border-2";

    public override string StepItemMarkerColor( Color color, bool active )
    {
        var name = color?.Name;

        if ( active )
        {
            return name switch
            {
                "primary" => "text-white bg-primary-800 hover:bg-primary-900 focus:ring-primary-400 dark:bg-primary-700 dark:hover:bg-primary-800 dark:focus:ring-primary-900 shadow-md shadow-blue-900",
                "secondary" => "text-white bg-secondary-500 hover:bg-secondary-600 focus:ring-secondary-100 dark:bg-secondary-400 dark:hover:bg-secondary-500 dark:focus:ring-secondary-600 shadow-md shadow-secondary-600",
                "success" => "text-white bg-success-700 hover:bg-success-800 focus:ring-success-300 dark:bg-success-600 dark:hover:bg-success-700 dark:focus:ring-success-800 shadow-md shadow-success-800",
                "danger" => "text-white bg-danger-700 hover:bg-danger-800 focus:ring-danger-300 dark:bg-danger-600 dark:hover:bg-danger-700 dark:focus:ring-danger-900 shadow-md shadow-danger-800",
                "warning" => "text-white bg-warning-400 hover:bg-warning-500 focus:ring-warning-300 dark:focus:ring-warning-900 shadow-md shadow-blue-900 shadow-md shadow-warning-500",
                "info" => "text-white bg-info-700 hover:bg-info-800 focus:ring-info-300 dark:bg-info-600 dark:hover:bg-info-700 dark:focus:ring-info-900 shadow-md shadow-info-800",
                "light" => "text-light-900 bg-light-300 border border-light-300 hover:bg-light-100 focus:ring-light-200 dark:bg-light-800 dark:text-white dark:border-light-600 dark:hover:bg-light-700 dark:hover:border-light-600 dark:focus:ring-light-700 shadow-md shadow-light-400",
                "dark" => "text-white bg-dark-800 hover:bg-dark-900 focus:ring-dark-300 dark:bg-dark-800 dark:hover:bg-dark-700 dark:focus:ring-dark-700 dark:border-dark-700 shadow-md shadow-dark-900",
                "link" => "text-primary-600 dark:text-primary-500 hover:underline",
                _ => "text-white bg-primary-800 hover:bg-primary-900 focus:ring-primary-400 dark:bg-primary-700 dark:hover:bg-primary-800 dark:focus:ring-primary-900 shadow-md shadow-blue-900",
            };
        }

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

    public override string StepItemDescription() => "b-step-item-head-text font-medium";

    public override string StepsContent() => "b-steps-content";

    public override string StepPanel() => "b-step-panel";

    public override string StepPanelActive( bool active ) => active ? "block" : "hidden";

    #endregion

    #region Carousel

    public override string Carousel() => "b-carousel relative";

    public override string CarouselSlides() => "b-carousel-slides relative overflow-hidden rounded-lg h-56 md:h-96";

    public override string CarouselSlide() => "b-carousel-slide duration-700 ease-in-out absolute inset-0 transition-all transform";

    public override string CarouselSlideActive( bool active ) => active ? "b-carousel-slide-active" : null;

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

    public override string CarouselIndicators() => "b-carousel-indicators absolute z-30 flex space-x-3 -translate-x-1/2 bottom-5 left-1/2";

    public override string CarouselIndicator() => "b-carousel-indicator w-3 h-3 rounded-full";

    public override string CarouselIndicatorActive( bool active ) => active ? "b-carousel-indicator-active bg-white dark:bg-gray-800" : "bg-white/50 dark:bg-gray-800/50 hover:bg-white dark:hover:bg-gray-800";

    public override string CarouselFade( bool fade ) => fade ? "b-carousel-fade" : null;

    public override string CarouselCaption() => "b-carousel-caption";

    #endregion

    #region Jumbotron

    public override string Jumbotron() => "b-jumbotron py-8 px-4 mx-auto w-full text-center lg:py-16 lg:px-12 text-gray-100 dark:text-gray-800";

    public override string JumbotronBackground( Background background ) => $"bg-{ToBackground( background )}";

    public override string JumbotronTitle( JumbotronTitleSize jumbotronTitleSize ) => $"b-jumbotron-title text-gray-100 dark:text-gray-800 {DisplayHeadingSize( (Blazorise.DisplayHeadingSize)jumbotronTitleSize )}";

    public override string JumbotronSubtitle() => "b-jumbotron-subtitle mb-8 text-lg font-normal text-gray-100 lg:text-xl sm:px-16 xl:px-48 dark:text-gray-800";

    #endregion

    #region Card

    public override string CardDeck() => "b-card-deck flex flex-row gap-x-4";

    public override string CardGroup() => "b-card-group flex flex-row gap-x-0";

    public override string Card() => "b-card max-w bg-white border border-gray-200 rounded-lg shadow-md dark:bg-gray-800 dark:border-gray-700 ";

    public override string CardWhiteText() => "text-white";

    public override string CardActions() => "b-card-actions";

    public override string CardBody() => "b-card-body p-5 text-inherit text-gray-700 dark:text-gray-400";

    public override string CardFooter() => "b-card-footer px-5 py-4 text-inherit";

    public override string CardHeader() => "b-card-header px-5 py-4 text-inherit";

    public override string CardImage() => "b-card-image rounded-t-lg text-inherit";

    public override string CardTitle( bool insideHeader ) => "b-card-title mb-2 text-2xl font-bold tracking-tight text-inherit text-gray-900 dark:text-white";

    public override string CardTitleSize( bool insideHeader, int? size ) => null;

    public override string CardSubtitle( bool insideHeader ) => "b-card-subtitle text-inherit";

    public override string CardSubtitleSize( bool insideHeader, int size ) => null;

    public override string CardText() => "b-card-text mb-3 font-normal text-inherit";

    public override string CardLink() => "b-card-link inline-flex items-center text-primary-600 dark:text-primary-500 hover:underline";

    public override string CardLinkActive( bool active ) => LinkActive( active );

    #endregion

    #region ListGroup

    public override string ListGroup() => "b-listgroup w-full text-sm font-medium text-gray-900 bg-white border-gray-200 dark:bg-gray-700 dark:border-gray-600 dark:text-white";

    public override string ListGroupFlush( bool flush ) => flush ? "b-listgroup-flush border-y border-x-0" : "border rounded-lg";

    public override string ListGroupScrollable( bool scrollable ) => scrollable ? "b-listgroup-scrollable overflow-y-scroll" : null;

    public override string ListGroupItem() => "b-listgroup-item py-3 px-4 w-full border-b last:border-b-0";

    public override string ListGroupItemSelectable() => "b-listgroup-item-selectable cursor-pointer focus:outline-none";

    public override string ListGroupItemActive() => "b-listgroup-item-active";

    public override string ListGroupItemDisabled() => "b-listgroup-item-disabled bg-gray-100 cursor-not-allowed dark:bg-gray-600 dark:text-gray-400";

    public override string ListGroupItemColor( Color color, bool selectable, bool active )
    {
        var sb = new StringBuilder();

        var name = color?.Name;

        if ( active )
        {
            sb.Append( name switch
            {
                "primary" => "text-white bg-primary-700",
                "secondary" => "text-white bg-secondary-700",
                "success" => "text-white bg-success-700",
                "danger" => "text-white bg-danger-700",
                "warning" => "text-white bg-warning-700",
                "info" => "text-white bg-info-700",
                "light" => "text-gray-800 bg-light-300",
                "dark" => "text-white bg-dark-700",
                "link" => "text-primary-700 dark:text-primary-200",
                _ => "text-white bg-primary-700",
            } );
        }
        else
        {
            sb.Append( name switch
            {
                "primary" => "text-white bg-primary-400 dark:bg-primary-600",
                "secondary" => "text-white bg-secondary-400 dark:bg-secondary-400",
                "success" => "text-white bg-success-400 dark:bg-success-600",
                "danger" => "text-white bg-danger-400 dark:bg-danger-600",
                "warning" => "text-white bg-warning-400 dark:bg-warning-600",
                "info" => "text-white bg-info-400 dark:bg-info-600",
                "light" => "text-gray-800 bg-light-100 dark:text-white dark:bg-light-300",
                "dark" => "text-white bg-dark-700",
                "link" => "text-primary-400 dark:text-primary-500",
                _ => name,
            } );
        }

        if ( color != Color.Default && selectable )
        {
            sb.Append( ' ' ).Append( name switch
            {
                "primary" => "hover:bg-primary-500 focus:ring-primary-500 dark:hover:bg-primary-700 dark:focus:ring-primary-800",
                "secondary" => "hover:bg-secondary-500 focus:ring-secondary-500 dark:hover:bg-secondary-500 dark:focus:ring-secondary-600",
                "success" => "hover:bg-success-500 focus:ring-success-500 dark:hover:bg-success-700 dark:focus:ring-success-800",
                "danger" => "hover:bg-danger-500 focus:ring-danger-500 dark:hover:bg-danger-700 dark:focus:ring-danger-900",
                "warning" => "hover:bg-warning-500 focus:ring-warning-500 dark:focus:ring-warning-900",
                "info" => "hover:bg-info-500 focus:ring-info-500 dark:hover:bg-info-700 dark:focus:ring-info-900",
                "light" => "hover:bg-light-200 focus:ring-light-300 dark:hover:border-light-600 dark:focus:ring-light-700",
                "dark" => "hover:bg-dark-600 focus:ring-dark-600 dark:bg-dark-800 dark:hover:bg-dark-700 dark:focus:ring-dark-700",
                "link" => "hover:underline",
                _ => name,
            } );
        }
        else
        {
            sb.Append( ' ' ).Append( "border-gray-200 dark:border-gray-600" );
        }

        return sb.ToString();
    }

    #endregion

    #region Layout

    public override string Layout() => "b-layout";

    public override string LayoutHasSider() => "b-layout-has-sider";

    public override string LayoutContent() => "b-layout-content";

    public override string LayoutHeader() => "b-layout-header z-30";

    public override string LayoutHeaderFixed() => "b-layout-header-fixed";

    public override string LayoutFooter() => "b-layout-footer";

    public override string LayoutFooterFixed() => "b-layout-footer-fixed";

    public override string LayoutSider() => "b-layout-sider";

    public override string LayoutSiderContent() => "b-layout-sider-content";

    public override string LayoutLoading() => "b-layout-loading";

    public override string LayoutRoot() => "b-layout-root";

    #endregion

    #region Container

    public override string Container( Breakpoint breakpoint ) => breakpoint != Breakpoint.None && breakpoint != Breakpoint.Mobile
        ? $"{ToBreakpoint( breakpoint )}:container {ToBreakpoint( breakpoint )}:mx-auto {ToBreakpoint( breakpoint )}:px-4"
        : "container mx-auto px-4";

    public override string ContainerFluid() => "container mx-auto";

    #endregion

    #region Bar

    public override string Bar() => "b-bar";

    public override string BarInitial( bool initial ) => initial ? "b-bar-initial" : null;

    public override string BarAlignment( Alignment alignment ) => FlexAlignment( alignment );

    public override string BarThemeContrast( ThemeContrast themeContrast ) => $"navbar-{ToThemeContrast( themeContrast )} b-bar-{ToThemeContrast( themeContrast )}";

    public override string BarBreakpoint( Breakpoint breakpoint ) => breakpoint != Breakpoint.None && breakpoint != Breakpoint.Mobile
        ? $"navbar-expand-{ToBreakpoint( breakpoint )}"
        : null;

    public override string BarMode( BarMode mode ) => mode == Blazorise.BarMode.Horizontal
        ? $"b-bar-{ToBarMode( mode )} px-2 sm:px-4 py-2.5"
        : $"b-bar-{ToBarMode( mode )}";

    public override string BarItem( BarMode mode, bool hasDropdown ) => mode == Blazorise.BarMode.Horizontal
        ? hasDropdown
            ? "b-bar-item relative"
            : "b-bar-item relative"
        : "b-bar-item";

    public override string BarItemActive( BarMode mode ) => Active();

    public override string BarItemDisabled( BarMode mode ) => Disabled();

    public override string BarItemHasDropdown( BarMode mode ) => null;

    public override string BarItemHasDropdownShow( BarMode mode ) => null;

    public override string BarLink( BarMode mode ) => mode == Blazorise.BarMode.Horizontal
        ? "b-bar-link block px-4 py-2 text-gray-700 rounded hover:bg-gray-100 md:hover:bg-transparent md:border-0 md:hover:text-primary-700 dark:text-gray-400 md:dark:hover:text-primary-600 dark:hover:bg-gray-700 md:dark:hover:bg-transparent"
        : "b-bar-link";

    public override string BarLinkDisabled( BarMode mode ) => Disabled();

    public override string BarBrand( BarMode mode ) => mode == Blazorise.BarMode.Horizontal
        ? "b-bar-brand flex items-center"
        : "b-bar-brand";

    public override string BarToggler( BarMode mode, BarTogglerMode togglerMode ) => mode == Blazorise.BarMode.Horizontal ? "b-bar-toggler navbar-toggler" :
        togglerMode == BarTogglerMode.Popout ? "b-bar-toggler-popout" : "b-bar-toggler-inline";

    public override string BarTogglerCollapsed( BarMode mode, BarTogglerMode togglerMode, bool isShow ) => isShow || mode != Blazorise.BarMode.Horizontal ? null : "collapsed";

    public override string BarMenu( BarMode mode ) => mode == Blazorise.BarMode.Horizontal
        ? "b-bar-menu collapse navbar-collapse z-30"
        : "b-bar-menu";

    public override string BarMenuShow( BarMode mode ) => Show();

    public override string BarStart( BarMode mode ) => mode == Blazorise.BarMode.Horizontal
        ? "b-bar-start items-center justify-between hidden w-full md:flex md:w-auto md:order-1 mr-auto"
        : "b-bar-start";

    public override string BarEnd( BarMode mode ) => mode == Blazorise.BarMode.Horizontal
        ? "b-bar-end items-center justify-between hidden w-full md:flex md:w-auto md:order-1 ml-auto"
        : "b-bar-end";

    public override string BarDropdown( BarMode mode, bool isBarDropDownSubmenu ) => mode == Blazorise.BarMode.Horizontal
        ? "b-bar-dropdown b-bar-dropdown-horizontal relative"
        : "b-bar-dropdown";

    public override string BarDropdownShow( BarMode mode ) => Show();

    public override string BarDropdownToggle( BarMode mode, bool isBarDropDownSubmenu ) => mode == Blazorise.BarMode.Horizontal
        ? isBarDropDownSubmenu
            ? "b-bar-dropdown-toggle flex items-center justify-between w-full px-4 py-2 font-medium text-gray-700 border-b border-gray-100 hover:bg-gray-50 md:hover:bg-transparent md:border-0 md:hover:text-primary-700 dark:text-gray-400 dark:border-gray-700 dark:hover:bg-gray-700 md:dark:hover:bg-transparent"
            : "b-bar-dropdown-toggle flex items-center justify-between w-full px-4 py-2 font-medium text-gray-700 md:hover:text-primary-700 md:w-auto dark:text-gray-400"
        : "b-bar-link b-bar-dropdown-toggle";

    public override string BarDropdownToggleDisabled( BarMode mode, bool isBarDropDownSubmenu, bool disabled ) => mode == Blazorise.BarMode.Horizontal && disabled
        ? "disabled"
        : null;

    public override string BarDropdownItem( BarMode mode ) => mode == Blazorise.BarMode.Horizontal
        ? "b-bar-dropdown-item block px-4 py-2 hover:bg-gray-100 dark:hover:bg-gray-600 dark:hover:text-white"
        : "b-bar-dropdown-item";

    public override string BarTogglerIcon( BarMode mode ) => "b-bar-toggler-icon";

    public override string BarDropdownDivider( BarMode mode ) => "b-bar-dropdown-divider";

    public override string BarDropdownMenu( BarMode mode ) => mode == Blazorise.BarMode.Horizontal
        ? "b-bar-dropdown-menu absolute z-10 font-normal bg-white divide-y divide-gray-100 rounded shadow w-max dark:bg-gray-700 dark:divide-gray-600"
        : "b-bar-dropdown-menu";

    public override string BarDropdownMenuVisible( BarMode mode, bool visible ) => visible
        ? "b-bar-dropdown-menu-show block"
        : "b-bar-dropdown-menu-hide hidden";

    public override string BarDropdownMenuRight( BarMode mode ) => mode == Blazorise.BarMode.Horizontal
        ? "b-bar-dropdown-menu-right right-0 left-auto"
        : "b-bar-right";

    public override string BarDropdownMenuContainer( BarMode mode ) => mode == Blazorise.BarMode.Horizontal ? null : "b-bar-dropdown-menu-container";

    public override string BarCollapsed( BarMode mode ) => null;

    public override string BarLabel() => "b-bar-label";

    #endregion

    #region Accordion

    public override string Accordion() => "b-accordion";

    #endregion

    #region AccordionToggle

    public override string AccordionToggle() => "b-accordion-button flex items-center justify-between w-full p-5 font-medium text-left border border-gray-200 rounded-t-xl focus:ring-4 focus:ring-gray-200 dark:focus:ring-gray-800 dark:border-gray-700 hover:bg-gray-100 dark:hover:bg-gray-800";

    public override string AccordionToggleCollapsed( bool collapsed ) => collapsed
        ? "b-accordion-toggle-collapsed bg-gray-100 dark:bg-gray-800 text-gray-900 dark:text-white"
        : "b-accordion-toggle-collapsed text-gray-500 dark:text-gray-400";

    #endregion

    #region Collapse

    public override string Collapse( bool accordion ) => "b-collapse";

    public override string CollapseActive( bool accordion, bool active ) => "b-collapse-active";

    public override string CollapseHeader( bool accordion ) => "b-collapse-header";

    public override string CollapseBody( bool accordion ) => "b-collapse-body";

    public override string CollapseBodyActive( bool accordion, bool active ) => active ? null : "hidden";

    public override string CollapseBodyContent( bool accordion, bool firstInAccordion, bool lastInAccordion )
        => lastInAccordion
        ? "p-5 font-light border border-gray-200 dark:border-gray-700 dark:bg-gray-900"
        : "p-5 font-light border border-b-0 border-gray-200 dark:border-gray-700 dark:bg-gray-900";

    #endregion

    #region Row

    public override string Row() => "flex flex-row flex-wrap -mx-2";

    public override string RowColumns( RowColumnsSize rowColumnsSize, RowColumnsDefinition rowColumnsDefinition )
    {
        if ( rowColumnsDefinition.Breakpoint != Breakpoint.None && rowColumnsDefinition.Breakpoint != Breakpoint.Mobile )
            return $"row-cols-{ToBreakpoint( rowColumnsDefinition.Breakpoint )}-{ToRowColumnsSize( rowColumnsSize )}";

        return $"row-cols-{ToRowColumnsSize( rowColumnsSize )}";
    }

    public override string RowNoGutters( bool noGutters ) => noGutters ? "mx-0 no-gutters" : null;

    #endregion

    #region Column

    public override string Column( bool grid, bool hasSizes ) => hasSizes ? null : $"relative w-full basis-0 grow{( grid ? null : " pl-2 pr-2" )}";

    public override string Column( bool grid, ColumnWidth columnWidth, Breakpoint breakpoint, bool offset )
    {
        var columnWidthNumber = ToColumnWidthNumber( columnWidth );
        var breakpointPart = breakpoint != Blazorise.Breakpoint.None && breakpoint >= Blazorise.Breakpoint.Tablet
            ? $"{ToBreakpoint( breakpoint )}:"
            : null;

        if ( grid )
        {
            var columnSpanValue = ToColumnSpan( columnWidth );

            if ( columnSpanValue == "auto" )
            {
                return $"relative w-auto max-w-full {breakpointPart}col-{columnSpanValue}";
            }

            return $"relative w-full {breakpointPart}col-span-{columnSpanValue}";
        }

        var columnWidthValue = ToColumnWidth( columnWidth );

        if ( offset && columnWidthNumber > 0 )
        {
            var percentage = Math.Round( ( columnWidthNumber / 12d ) * 100, 6 ).ToString( CultureInfo.InvariantCulture );

            return $"{breakpointPart}ml-[{percentage}%]";
        }

        if ( columnWidthValue == "auto" )
        {
            return $"relative w-auto max-w-full {breakpointPart}basis-{columnWidthValue}";
        }

        return $"relative w-full {breakpointPart}basis-{columnWidthValue}";
    }

    public override string Column( bool grid, IEnumerable<ColumnDefinition> columnDefinitions )
       => $"{string.Join( ' ', columnDefinitions.Select( x => Column( grid, x.ColumnWidth, x.Breakpoint, x.Offset ) ) )}{( grid ? null : " pl-2 pr-2" )}";

    #endregion

    #region Grid

    public override string Grid() => "grid grid-cols-12 gap-4";

    public override string GridRows( GridRowsSize gridRows, GridRowsDefinition gridRowsDefinition )
    {
        var breakpointPart = gridRowsDefinition.Breakpoint != Blazorise.Breakpoint.None && gridRowsDefinition.Breakpoint >= Blazorise.Breakpoint.Tablet
            ? $"{ToBreakpoint( gridRowsDefinition.Breakpoint )}:"
            : null;

        return $"{breakpointPart}grid-rows-{ToGridRowsSize( gridRows )}";
    }

    public override string GridColumns( GridColumnsSize gridColumns, GridColumnsDefinition gridColumnsDefinition )
    {
        var breakpointPart = gridColumnsDefinition.Breakpoint != Blazorise.Breakpoint.None && gridColumnsDefinition.Breakpoint >= Blazorise.Breakpoint.Tablet
            ? $"{ToBreakpoint( gridColumnsDefinition.Breakpoint )}:"
            : null;

        return $"{breakpointPart}grid-cols-{ToGridColumnsSize( gridColumns )}";
    }

    #endregion

    #region Display

    public override string Display( DisplayType displayType, DisplayDefinition displayDefinition )
    {
        var baseClass = displayDefinition.Breakpoint != Breakpoint.None && displayDefinition.Breakpoint != Blazorise.Breakpoint.Mobile
            ? $"{ToBreakpoint( displayDefinition.Breakpoint )}:{ToDisplayType( displayType )}"
            : $"{ToDisplayType( displayType )}";

        if ( displayDefinition.Direction != DisplayDirection.Default )
            return $"{baseClass} flex-{ToDisplayDirection( displayDefinition.Direction )}";

        return baseClass;
    }

    #endregion

    #region Alert

    public override string Alert() => "b-alert group relative p-4 mb-4 text-sm rounded-lg";

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

    public override string Modal() => "b-modal fixed top-0 left-0 right-0 z-50 w-full p-4 overflow-x-hidden overflow-y-auto md:inset-0 h-modal md:h-full justify-center items-center";

    public override string ModalFade() => "b-modal-fade";

    public override string ModalFade( bool animation ) => animation ? "b-modal-fade" : null;

    public override string ModalVisible( bool visible ) => visible ? "flex" : null;

    public override string ModalBackdrop() => "bg-gray-900 bg-opacity-50 dark:bg-opacity-80 fixed inset-0 z-40";

    public override string ModalBackdropFade() => "b-modal-fade";

    public override string ModalBackdropVisible( bool visible ) => null;

    public override string ModalContent( bool dialog ) => "relative flex flex-col w-full bg-white rounded-lg shadow dark:bg-gray-700";

    public override string ModalContentSize( ModalSize modalSize )
    {
        if ( modalSize == ModalSize.Fullscreen )
            return "h-screen w-screen max-w-none h-full m-0 rounded-none border-none border-0";

        return null;
    }

    public override string ModalContentCentered( bool centered ) => null;

    public override string ModalContentScrollable( bool scrollable ) => null;

    public override string ModalBody() => "relative flex-auto p-6 space-y-6";

    public override string ModalHeader() => "flex items-start justify-between p-4 border-b rounded-t dark:border-gray-600";

    public override string ModalFooter() => "flex items-center p-6 space-x-2 border-t border-gray-200 rounded-b dark:border-gray-600";

    public override string ModalTitle() => "text-xl font-semibold text-gray-900 dark:text-white";

    #endregion

    #region Offcanvas

    public override string Offcanvas() => "fixed flex flex-col z-40 bg-white dark:bg-gray-800 transition-transform";

    public override string OffcanvasPlacement( Placement placement, bool visible )
    {
        var sb = new StringBuilder( placement switch
        {
            Placement.End => "top-0 right-0 h-screen overflow-y-auto border-l w-80 border-gray-200",
            Placement.Top => "top-0 left-0 right-0 w-full h-60",
            Placement.Bottom => "bottom-0 left-0 right-0 w-full h-60",
            _ => "top-0 left-0 h-screen overflow-y-auto border-r w-80 border-gray-200",
        } );

        if ( !visible )
        {
            sb.Append( placement switch
            {
                Placement.Start => " -translate-x-full",
                Placement.End => " translate-x-full",
                Placement.Top => " -translate-y-full",
                Placement.Bottom => " translate-y-full",
                _ => null,
            } );
        }

        return sb.ToString();
    }

    public override string OffcanvasFade( bool showing, bool hiding ) => showing
        ? "opacity-100"
        : hiding
            ? "transform-none pointer-events-none"
            : null;

    public override string OffcanvasVisible( bool visible ) => null;

    public override string OffcanvasHeader() => "flex items-center justify-between p-4";

    public override string OffcanvasFooter() => "flex items-center justify-between p-4";

    public override string OffcanvasBody() => "flex grow text-sm text-gray-500 dark:text-gray-400 p-4";

    public override string OffcanvasBackdrop() => "bg-gray-900 bg-opacity-50 dark:bg-opacity-80 fixed inset-0 z-30";

    public override string OffcanvasBackdropFade() => "fade";

    public override string OffcanvasBackdropVisible( bool visible ) => visible ? Show() : null;

    #endregion

    #region Pagination

    public override string Pagination() => "pagination flex -space-x-px mb-3";

    public override string PaginationSize( Size size ) => $"{Pagination()}-{ToSize( size )}";

    public override string PaginationItem() => "pagination-item";

    public override string PaginationItemActive() => null;

    public override string PaginationItemDisabled() => null;

    public override string PaginationLink()
        => "pagination-link relative block leading-tight border rounded-lg";

    public override string PaginationLinkSize( Size size )
    {
        return size switch
        {
            Size.ExtraSmall => "px-2 py-1.5",
            Size.Small => "px-2.5 py-2",
            Size.Medium => "px-3 py-3",
            Size.Large => "px-4 py-3.5",
            Size.ExtraLarge => "px-3 py-2.5",
            _ => "px-3 py-2.5"
        };
    }

    public override string PaginationLinkActive( bool active ) => active
        ? "text-primary-600 bg-primary-50 hover:bg-primary-100 hover:text-primary-700 dark:bg-gray-700 dark:text-white"
        : "text-gray-500 bg-white hover:bg-gray-100 hover:text-gray-700 dark:bg-gray-800 dark:border-gray-700 dark:text-gray-400 dark:hover:bg-gray-700 dark:hover:text-white";

    public override string PaginationLinkDisabled( bool disabled ) => disabled
        ? "cursor-not-allowed opacity-60"
        : "cursor-pointer";

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

    public override string BackgroundColor( Background background )
    {
        var name = background?.Name;

        return name switch
        {
            "primary" => "!bg-primary-500",
            "secondary" => "!bg-secondary-500",
            "success" => "!bg-success-500",
            "danger" => "!bg-danger-500",
            "warning" => "!bg-warning-400",
            "info" => "!bg-info",
            "light" => "!bg-light",
            "dark" => "!bg-dark",
            "white" => "!bg-white",
            "transparent" => "!bg-transparent",
            "body" => "!bg-body",
            _ => name,
        };
    }

    #endregion

    #region Table

    public override string Table() => "b-table w-full text-sm text-left text-gray-500 dark:text-gray-400 group mb-3";

    public override string TableFullWidth() => "b-table-fullwidth";

    public override string TableStriped() => "b-table-striped";

    public override string TableHoverable() => "b-table-hoverable";

    public override string TableBordered() => "b-table-bordered border";

    public override string TableNarrow() => "b-table-sm";

    public override string TableBorderless() => "b-table-borderless border-0";

    public override string TableHeader() => "text-xs text-gray-800 bg-white dark:bg-gray-700 dark:text-gray-400";

    public override string TableHeaderThemeContrast( ThemeContrast themeContrast )
        => themeContrast == ThemeContrast.Light
            ? "text-gray-700 bg-gray-200 dark:text-gray-200 dark:bg-gray-700"
            : themeContrast == ThemeContrast.Dark
                ? "text-gray-100 bg-gray-700 dark:text-gray-700 dark:bg-gray-100"
                : null;

    public override string TableHeaderCell() => "py-4 px-4";

    public override string TableHeaderCellCursor( Cursor cursor ) => cursor != Cursor.Default ? $"cursor-{ToCursor( cursor )}" : null;

    public override string TableFooter() => "b-table-footer";

    public override string TableBody() => "b-table-body";

    public override string TableRow( bool striped, bool hoverable )
    {
        if ( striped && hoverable )
            return "b-table-row odd:bg-white even:bg-gray-50 odd:hover:bg-gray-100 even:hover:bg-white odd:bg-white even:bg-gray-50 hover:bg-gray-100 group-[:not(.border-0)]:border-b dark:bg-gray-800 group-[:not(.border-0)]:dark:border-gray-700";

        if ( striped )
            return "b-table-row odd:bg-white even:bg-gray-50 group-[:not(.border-0)]:border-b dark:bg-gray-800 group-[:not(.border-0)]:dark:border-gray-700";

        if ( hoverable )
            return "b-table-row hover:bg-gray-100 group-[:not(.border-0)]:border-b dark:bg-gray-800 group-[:not(.border-0)]:dark:border-gray-700";

        return "b-table-row group-[:not(.border-0)]:border-b dark:bg-gray-800 group-[:not(.border-0)]:dark:border-gray-700";
    }

    public override string TableRowColor( Color color )
    {
        var name = color?.Name;

        return name switch
        {
            "primary" => "!text-primary-800 !bg-primary-300 !dark:bg-primary-500 !dark:text-primary-800",
            "secondary" => "!text-secondary-500 !bg-secondary-300 !dark:bg-secondary-100 !dark:text-secondary-600",
            "success" => "!text-success-700 !bg-success-100 !dark:bg-success-200 !dark:text-success-800",
            "danger" => "!text-danger-700 !bg-danger-100 !dark:bg-danger-200 !dark:text-danger-800",
            "warning" => "!text-warning-700 !bg-warning-100 !dark:bg-warning-200 !dark:text-warning-800",
            "info" => "!text-info-700 !bg-info-100 !dark:bg-info-200 !dark:text-info-800",
            "light" => "!text-light-500 bg-light-100 !dark:bg-light-100 !dark:text-light-600",
            "dark" => "!text-dark-100 !bg-dark-800 !dark:bg-dark-300 !dark:text-dark-700",
            "link" => "!text-primary-600 !dark:text-primary-500 !hover:underline",
            _ => null,
        };
    }

    public override string TableRowHoverCursor() => "b-table-row-selectable cursor-pointer";

    public override string TableRowIsSelected() => "b-table-row-selected !text-primary-800 !bg-primary-300 !dark:bg-primary-500 !dark:text-primary-800";

    public override string TableRowHeader() => "group-[.b-table-sm]:py-2 group-[:not(.b-table-sm)]:py-4 px-4 font-medium text-gray-900 whitespace-nowrap dark:text-white";

    public override string TableRowCell() => "group-[.b-table-sm]:py-2 group-[:not(.b-table-sm)]:py-4 px-4";

    public override string TableRowCellColor( Color color ) => $"table-{ToColor( color )}";

    public override string TableRowGroup( bool expanded ) => "b-table-group bg-gray-50 group-[:not(.border-0)]:border-b dark:bg-gray-800 group-[:not(.border-0)]:dark:border-gray-700 font-bold cursor-pointer";

    public override string TableRowGroupCell() => "b-table-group-cell group-[.b-table-sm]:py-2 group-[:not(.b-table-sm)]:py-4 px-4";

    public override string TableRowGroupIndentCell() => "b-table-group-indentcell";

    public override string TableResponsive() => "b-table-responsive overflow-x-auto relative shadow-md sm:rounded-lg";

    public override string TableFixedHeader() => "b-table-fixed-header";

    #endregion

    #region Badge

    public override string Badge() => "b-badge inline-flex items-center text-[0.75em] leading-[1em] font-semibold px-2.5 py-1 rounded";

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

    public override string BadgePill() => "b-badge-pill rounded-full";

    public override string BadgeClose() => null;

    #endregion

    #region Media

    public override string Media() => "b-media flex";

    public override string MediaLeft() => "b-media-left shrink-0";

    public override string MediaRight() => "b-media-right shrink ml-3";

    public override string MediaBody() => "b-media-body shrink ml-3";

    #endregion

    #region Text

    public override string TextColor( TextColor textColor )
    {
        var name = textColor?.Name;

        return name switch
        {
            "primary" => "!text-primary-600 !dark:text-primary-500",
            "secondary" => "!text-secondary-600 !dark:text-secondary-500",
            "success" => "!text-success-500 !dark:text-success-400",
            "danger" => "!text-danger-600 !dark:text-danger-500",
            "warning" => "!text-warning-400 !dark:text-warning-300",
            "info" => "!text-info-600 !dark:text-info-500",
            "light" => "!text-light-600 !dark:text-light-500",
            "dark" => "!text-gray-800 !dark:text-gray-200",
            "body" => "!text-gray-500 !dark:text-gray-400",
            "muted" => "!text-gray-300 !dark:text-gray-600",
            "white" => "!text-white !dark:text-black",
            "black-50" => "!text-gray-600 !dark:text-gray-300",
            "white-50" => "!text-gray-300 !dark:text-gray-400",
            _ => "!text-gray-500 !dark:text-gray-400",
        };
    }

    public override string TextAlignment( TextAlignment textAlignment ) => $"text-{ToTextAlignment( textAlignment )}";

    public override string TextTransform( TextTransform textTransform ) => ToTextTransform( textTransform );

    public override string TextWeight( TextWeight textWeight ) => $"font-{ToTextWeight( textWeight )}";

    public override string TextOverflow( TextOverflow textOverflow )
    {
        return textOverflow switch
        {
            Blazorise.TextOverflow.Wrap => "whitespace-normal",
            Blazorise.TextOverflow.NoWrap => "whitespace-nowrap",
            Blazorise.TextOverflow.Truncate => "text-ellipsis overflow-hidden whitespace-nowrap",
            _ => null,
        };
    }

    public override string TextSize( TextSize textSize ) => $"text-{ToTextSize( textSize )}";

    public override string TextItalic() => "italic";

    #endregion

    #region Code

    public override string Code() => "b-code";

    #endregion

    #region Heading

    public override string HeadingSize( HeadingSize headingSize )
    {
        return headingSize switch
        {
            Blazorise.HeadingSize.Is1 => "b-heading-1 text-5xl font-extrabold dark:text-white",
            Blazorise.HeadingSize.Is2 => "b-heading-2 text-4xl font-bold dark:text-white",
            Blazorise.HeadingSize.Is3 => "b-heading-3 text-3xl font-bold dark:text-white",
            Blazorise.HeadingSize.Is4 => "b-heading-4 text-2xl font-bold dark:text-white",
            Blazorise.HeadingSize.Is5 => "b-heading-5 text-xl font-bold dark:text-white",
            Blazorise.HeadingSize.Is6 => "b-heading-6 text-lg font-bold dark:text-white",
            _ => null,
        };
    }

    #endregion

    #region DisplayHeading

    public override string DisplayHeadingSize( DisplayHeadingSize displayHeadingSize )
    {
        return displayHeadingSize switch
        {
            Blazorise.DisplayHeadingSize.Is1 => "b-displayheading-1 text-7xl font-light dark:text-white",
            Blazorise.DisplayHeadingSize.Is2 => "b-displayheading-2 text-6xl font-light dark:text-white",
            Blazorise.DisplayHeadingSize.Is3 => "b-displayheading-3 text-5xl font-light dark:text-white",
            Blazorise.DisplayHeadingSize.Is4 => "b-displayheading-4 text-4xl font-light dark:text-white",
            _ => null,
        };
    }

    #endregion

    #region Lead

    public override string Lead() => "b-lead mb-3 text-lg font-light text-gray-500 md:text-xl dark:text-gray-400";

    #endregion

    #region Paragraph

    public override string Paragraph() => "b-paragraph";

    public override string ParagraphColor( TextColor textColor )
    {
        var name = textColor?.Name;

        return name switch
        {
            "primary" => "text-primary-600 dark:text-primary-500",
            "secondary" => "text-secondary-600 dark:text-secondary-500",
            "success" => "text-success-500 dark:text-success-400",
            "danger" => "text-danger-600 dark:text-danger-500",
            "warning" => "text-warning-400 dark:text-warning-300",
            "info" => "text-info-600 dark:text-info-500",
            "light" => "text-light-600 dark:text-light-500",
            "dark" => "text-gray-500 dark:text-gray-400",
            "body" => "text-gray-500 dark:text-gray-400",
            "muted" => "text-gray-500 dark:text-gray-400",
            "white" => "text-gray-500 dark:text-gray-400",
            "black-50" => "text-gray-500 dark:text-gray-400",
            "white-50" => "text-gray-500 dark:text-gray-400",
            _ => "text-gray-500 dark:text-gray-400",
        };
    }

    #endregion

    #region Blockquote

    public override string Blockquote() => "b-blockquote text-xl italic font-semibold text-gray-900 dark:text-white";

    public override string BlockquoteFooter() => "b-blockquote-footer text-sm font-light text-gray-500 dark:text-gray-400";

    #endregion

    #region Figure

    public override string Figure() => "b-figure max-w-lg";

    public override string FigureSize( FigureSize figureSize ) => $"b-figure-{ToFigureSize( figureSize )}";

    public override string FigureImage() => "b-figure-image max-w-full h-auto";

    public override string FigureImageRounded() => "b-figure-rounded rounded-lg";

    public override string FigureCaption() => "b-figure-caption mt-2 text-sm text-center text-gray-500 dark:text-gray-400";

    #endregion

    #region Image

    public override string Image() => null;

    public override string ImageFluid( bool fluid ) => fluid ? "img-fluid" : null;

    #endregion

    #region Breadcrumb

    public override string Breadcrumb() => "b-breadcrumb flex px-5 py-3 mb-3 text-gray-700 border border-gray-200 rounded-lg bg-gray-50 dark:bg-gray-800 dark:border-gray-700";

    public override string BreadcrumbItem() => "b-breadcrumb-item";

    public override string BreadcrumbItemActive() => null;

    public override string BreadcrumbLink() => "b-breadcrumb-link flex items-center";

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

    public override string Link() => "";

    public override string LinkActive( bool active ) => active ? "font-medium active" : null;

    public override string LinkUnstyled( bool unstyled )
    {
        if ( unstyled )
        {
            return "font-medium text-inherit hover:underline";
        }
        else
        {
            return "font-medium text-primary-600 dark:text-primary-500 hover:underline";
        }
    }

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
            return $"!{ToBreakpoint( breakpoint )}:{ToSpacing( spacing )}{ToSide( side )}-{ToSpacingSize( spacingSize )}";

        return $"!{ToSpacing( spacing )}{ToSide( side )}-{ToSpacingSize( spacingSize )}";
    }

    public override string Spacing( Spacing spacing, SpacingSize spacingSize, IEnumerable<(Side side, Breakpoint breakpoint)> rules )
        => string.Join( " ", rules.Select( x => Spacing( spacing, spacingSize, x.side, x.breakpoint ) ) );

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

    #region Borders

    public override string Border( BorderSize borderSize, BorderSide borderSide, BorderColor borderColor )
    {
        var sb = new StringBuilder( "!border" );

        if ( borderSide != BorderSide.All )
            sb.Append( '-' ).Append( ToBorderSide( borderSide ) );

        if ( borderSize != BorderSize.Default )
            sb.Append( '-' ).Append( ToBorderSize( borderSize ) );

        if ( borderColor != BorderColor.None )
            sb.Append( " !border-" ).Append( ToBorderColor( borderColor ) );

        return sb.ToString();
    }

    public override string Border( BorderSize borderSize, IEnumerable<(BorderSide borderSide, BorderColor borderColor)> rules )
        => string.Join( " ", rules.Select( x => Border( borderSize, x.borderSide, x.borderColor ) ) );

    #endregion

    #region Flex

    public override string Flex( FlexType flexType )
    {
        return flexType != FlexType.Default
            ? $"{ToFlexType( flexType )}"
            : null;
    }

    public override string Flex( FlexDefinition flexDefinition )
    {
        var sb = new StringBuilder();

        var breakpoint = flexDefinition.Breakpoint != Breakpoint.None
            ? $"{ToBreakpoint( flexDefinition.Breakpoint )}:"
            : null;

        if ( flexDefinition.Direction != FlexDirection.Default )
            sb.Append( "flex-" ).Append( breakpoint ).Append( ToDirection( flexDefinition.Direction ) );

        if ( flexDefinition.JustifyContent != FlexJustifyContent.Default )
            sb.Append( "justify-" ).Append( breakpoint ).Append( ToJustifyContent( flexDefinition.JustifyContent ) );

        if ( flexDefinition.AlignItems != FlexAlignItems.Default )
            sb.Append( "items-" ).Append( breakpoint ).Append( ToAlignItems( flexDefinition.AlignItems ) );

        if ( flexDefinition.AlignSelf != FlexAlignSelf.Default )
            sb.Append( "self-" ).Append( breakpoint ).Append( ToAlignSelf( flexDefinition.AlignSelf ) );

        if ( flexDefinition.AlignContent != FlexAlignContent.Default )
            sb.Append( "content-" ).Append( breakpoint ).Append( ToAlignContent( flexDefinition.AlignContent ) );

        if ( flexDefinition.GrowShrink != FlexGrowShrink.Default && flexDefinition.GrowShrinkSize != FlexGrowShrinkSize.Default )
            sb.Append( breakpoint ).Append( ToGrowShrink( flexDefinition.GrowShrink ) ).Append( "-" ).Append( ToGrowShrinkSize( flexDefinition.GrowShrinkSize ) );

        if ( flexDefinition.Wrap != FlexWrap.Default )
            sb.Append( "flex-" ).Append( breakpoint ).Append( ToWrap( flexDefinition.Wrap ) );

        if ( flexDefinition.Order != FlexOrder.Default )
            sb.Append( "order-" ).Append( breakpoint ).Append( ToOrder( flexDefinition.Order ) );

        if ( flexDefinition.Fill )
            sb.Append( "flex-" ).Append( breakpoint ).Append( "auto" );

        return sb.ToString();
    }

    public override string Flex( FlexType flexType, IEnumerable<FlexDefinition> flexDefinitions )
    {
        var sb = new StringBuilder();

        if ( flexType != FlexType.Default )
            sb.Append( $"{ToFlexType( flexType )}" ).Append( ' ' );

        sb.Append( string.Join( ' ', flexDefinitions.Select( x => Flex( x ) ) ) );

        return sb.ToString();
    }

    public override string FlexAlignment( Alignment alignment ) => $"justify-{ToAlignment( alignment )}";

    #endregion

    #region Sizing

    public override string Sizing( SizingType sizingType, SizingSize sizingSize, SizingDefinition sizingDefinition )
    {
        var sb = new StringBuilder();

        if ( sizingDefinition.Breakpoint != Breakpoint.None && sizingDefinition.Breakpoint != Breakpoint.Mobile )
            sb.Append( $"{ToBreakpoint( sizingDefinition.Breakpoint )}:" );

        if ( sizingDefinition.IsMin )
            sb.Append( "min-" );
        else if ( sizingDefinition.IsMax )
            sb.Append( "max-" );

        sb.Append( sizingType == SizingType.Width
            ? "w"
            : "h" );

        sb.Append( $"-{ToSizingSize( sizingSize )}" );

        if ( sizingDefinition.IsViewport )
            sb.Append( "screen" );

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
        return $"{ToPositionEdgeType( edgeType )}-{( edgeOffset == 50 ? "1/2" : edgeOffset == 100 ? "full" : edgeOffset )}";
    }

    public override string Position( PositionType positionType, IEnumerable<(PositionEdgeType edgeType, int edgeOffset)> edges, PositionTranslateType translateType )
    {
        var sb = new StringBuilder( $"{ToPositionType( positionType )}" );

        if ( edges != null && edges.Count() > 0 )
            sb.Append( ' ' ).Append( string.Join( " ", edges.Select( x => Position( positionType, x.edgeType, x.edgeOffset, translateType ) ) ) );

        if ( translateType != PositionTranslateType.None )
            sb.Append( ' ' ).Append( ToPositionTranslateType( translateType ) );

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
            Blazorise.ColumnWidth.Is1 => "1/12",
            Blazorise.ColumnWidth.Is2 => "2/12",
            Blazorise.ColumnWidth.Is3 or Blazorise.ColumnWidth.Quarter => "3/12",
            Blazorise.ColumnWidth.Is4 or Blazorise.ColumnWidth.Third => "4/12",
            Blazorise.ColumnWidth.Is5 => "5/12",
            Blazorise.ColumnWidth.Is6 or Blazorise.ColumnWidth.Half => "6/12",
            Blazorise.ColumnWidth.Is7 => "7/12",
            Blazorise.ColumnWidth.Is8 => "8/12",
            Blazorise.ColumnWidth.Is9 => "9/12",
            Blazorise.ColumnWidth.Is10 => "10/12",
            Blazorise.ColumnWidth.Is11 => "11/12",
            Blazorise.ColumnWidth.Is12 or Blazorise.ColumnWidth.Full => "full",
            Blazorise.ColumnWidth.Auto => "auto",
            _ => null,
        };
    }

    private static string ToColumnSpan( ColumnWidth columnWidth )
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
            Blazorise.ColumnWidth.Is12 or Blazorise.ColumnWidth.Full => "full",
            Blazorise.ColumnWidth.Auto => "auto",
            _ => null,
        };
    }

    private static int ToColumnWidthNumber( ColumnWidth columnWidth )
    {
        return columnWidth switch
        {
            ColumnWidth.Is1 => 1,
            ColumnWidth.Is2 => 2,
            ColumnWidth.Is3 or ColumnWidth.Quarter => 3,
            ColumnWidth.Is4 or ColumnWidth.Third => 4,
            ColumnWidth.Is5 => 5,
            ColumnWidth.Is6 or ColumnWidth.Half => 6,
            ColumnWidth.Is7 => 7,
            ColumnWidth.Is8 => 8,
            ColumnWidth.Is9 => 9,
            ColumnWidth.Is10 => 10,
            ColumnWidth.Is11 => 11,
            ColumnWidth.Is12 or ColumnWidth.Full => 12,
            ColumnWidth.Auto => 0,
            _ => 0,
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

    public override string ToBorderSide( BorderSide borderSide )
    {
        return borderSide switch
        {
            Blazorise.BorderSide.Bottom => "b",
            Blazorise.BorderSide.Start => "l",
            Blazorise.BorderSide.End => "r",
            _ => "t",
        };
    }

    public override string ToBorderRadius( BorderRadius borderRadius )
    {
        return borderRadius switch
        {
            Blazorise.BorderRadius.Rounded => "rounded",
            Blazorise.BorderRadius.RoundedTop => "rounded-t",
            Blazorise.BorderRadius.RoundedEnd => "rounded-r",
            Blazorise.BorderRadius.RoundedBottom => "rounded-b",
            Blazorise.BorderRadius.RoundedStart => "rounded-l",
            Blazorise.BorderRadius.RoundedCircle => "rounded-full",
            Blazorise.BorderRadius.RoundedPill => "rounded-full",
            Blazorise.BorderRadius.RoundedZero => "rounded-none",
            _ => null,
        };
    }

    public override string ToDirection( FlexDirection direction )
    {
        return direction switch
        {
            Blazorise.FlexDirection.Row => "row",
            Blazorise.FlexDirection.ReverseRow => "row-reverse",
            Blazorise.FlexDirection.Column => "col",
            Blazorise.FlexDirection.ReverseColumn => "col-reverse",
            _ => null,
        };
    }

    public override string ToPositionEdgeType( PositionEdgeType positionEdgeType )
    {
        return positionEdgeType switch
        {
            Blazorise.PositionEdgeType.Top => "top",
            Blazorise.PositionEdgeType.Start => "left",
            Blazorise.PositionEdgeType.Bottom => "bottom",
            Blazorise.PositionEdgeType.End => "right",
            _ => null,
        };
    }

    public override string ToPositionTranslateType( PositionTranslateType positionTranslateType )
    {
        return positionTranslateType switch
        {
            Blazorise.PositionTranslateType.Middle => "-translate-x-1/2 -translate-y-1/2",
            Blazorise.PositionTranslateType.MiddleX => "-translate-x-1/2",
            Blazorise.PositionTranslateType.MiddleY => "-translate-y-1/2",
            _ => null,
        };
    }

    public override string ToSizingSize( SizingSize sizingSize )
    {
        return sizingSize switch
        {
            Blazorise.SizingSize.Is25 => "1/4",
            Blazorise.SizingSize.Is33 => "1/3",
            Blazorise.SizingSize.Is50 => "1/2",
            Blazorise.SizingSize.Is66 => "2/3",
            Blazorise.SizingSize.Is75 => "3/4",
            Blazorise.SizingSize.Is100 => "full",
            Blazorise.SizingSize.Auto => "auto",
            _ => null,
        };
    }

    public override string ToValidationStatus( ValidationStatus validationStatus )
    {
        return validationStatus switch
        {
            Blazorise.ValidationStatus.Success => "bg-green-50 border border-green-500 text-green-900 dark:text-green-400 placeholder-green-700 dark:placeholder-green-500 focus:ring-green-500 focus:border-green-500 dark:bg-gray-700 dark:border-green-500",
            Blazorise.ValidationStatus.Error => "bg-red-50 border border-red-500 text-red-900 placeholder-red-700 focus:ring-red-500 dark:bg-gray-700 focus:border-red-500 dark:text-red-500 dark:placeholder-red-500 dark:border-red-500",
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

    public override string ToJustifyContent( JustifyContent justifyContent )
    {
        return justifyContent switch
        {
            Blazorise.JustifyContent.Start => "justify-start",
            Blazorise.JustifyContent.End => "justify-end",
            Blazorise.JustifyContent.Center => "justify-center",
            Blazorise.JustifyContent.Between => "justify-between",
            Blazorise.JustifyContent.Around => "justify-around",
            _ => null,
        };
    }

    public override string ToBorderColor( BorderColor borderColor )
    {
        return $"{borderColor.Name}-600";
    }

    public override string ToBreakpoint( Breakpoint breakpoint )
    {
        return breakpoint switch
        {
            Blazorise.Breakpoint.Mobile => "sm",
            Blazorise.Breakpoint.Tablet => "md",
            Blazorise.Breakpoint.Desktop => "lg",
            Blazorise.Breakpoint.Widescreen => "xl",
            Blazorise.Breakpoint.FullHD => "2xl",
            _ => null,
        };
    }

    public override string ToTextSize( TextSize textSize )
    {
        return textSize switch
        {
            Blazorise.TextSize.ExtraSmall => "xs",
            Blazorise.TextSize.Small => "sm",
            Blazorise.TextSize.Medium => "md",
            Blazorise.TextSize.Large => "lg",
            Blazorise.TextSize.ExtraLarge => "xl",
            Blazorise.TextSize.Heading1 => "5xl",
            Blazorise.TextSize.Heading2 => "4xl",
            Blazorise.TextSize.Heading3 => "3xl",
            Blazorise.TextSize.Heading4 => "2xl",
            Blazorise.TextSize.Heading5 => "xl",
            Blazorise.TextSize.Heading6 => "lg",
            _ => null,
        };
    }

    #endregion

    public override bool UseCustomInputStyles { get; set; } = true;

    public override string Provider => "Tailwind";
}