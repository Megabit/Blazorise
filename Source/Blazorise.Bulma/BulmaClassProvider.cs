#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace Blazorise.Bulma
{
    class BulmaClassProvider : ClassProvider
    {
        #region TextEdit

        public override string TextEdit( bool plaintext ) => plaintext ? "input is-static" : "input";

        public override string TextEditSize( Size size ) => ToSize( size );

        public override string TextEditColor( Color color ) => $"is-{ToColor( color )}";

        public override string TextEditValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

        #endregion

        #region MemoEdit

        public override string MemoEdit() => "textarea";

        public override string MemoEditValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

        #endregion

        #region Select

        public override string Select() => "select is-fullwidth";

        public override string SelectMultiple() => "is-multiple";

        public override string SelectSize( Size size ) => $"{ToSize( size )}";

        public override string SelectValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

        #endregion

        #region DateEdit

        public override string DateEdit() => "input";

        public override string DateEditSize( Size size ) => $"{ToSize( size )}";

        public override string DateEditValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

        #endregion

        #region TimeEdit

        public override string TimeEdit() => "input";

        public override string TimeEditSize( Size size ) => $"{ToSize( size )}";

        public override string TimeEditValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

        #endregion

        #region ColorEdit

        public override string ColorEdit() => "input";

        #endregion

        #region Check

        public override string Check() => "checkbox";

        public override string CheckInline() => "is-inline";

        public override string CheckCursor( Cursor cursor ) => $"{Check()}-{ToCursor( cursor )}";

        public override string CheckValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

        #endregion

        #region RadioGroup

        public override string RadioGroup( bool buttons ) => buttons ? "field has-addons" : "control";

        public override string RadioGroupInline() => null;

        #endregion

        #region Radio

        public override string Radio( bool button ) => "radio";

        public override string RadioInline() => "is-inline";

        #endregion

        #region Switch

        public override string Switch() => "switch";

        public override string SwitchChecked( bool @checked ) => null;

        public override string SwitchCursor( Cursor cursor ) => $"{Switch()}-{ToCursor( cursor )}";

        public override string SwitchValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

        #endregion

        #region FileEdit

        public override string FileEdit() => "file-input";

        public override string FileEditValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

        #endregion

        #region Slider

        public override string Slider() => "slider";

        public override string SliderColor( Color color ) => $"is-{ToColor( color )}";

        #endregion

        #region Label

        public override string Label() => "label";

        public override string LabelType( LabelType labelType )
        {
            switch ( labelType )
            {
                case Blazorise.LabelType.Check:
                    return "checkbox";
                case Blazorise.LabelType.Radio:
                    return "radio";
                case Blazorise.LabelType.Switch:
                    return null;
                case Blazorise.LabelType.File:
                    return "file-label";
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

        #endregion

        #endregion

        #region Fields

        public override string Fields() => "field";

        public override string FieldsBody() => "field-body";

        public override string FieldsColumn() => $"{Column()}";

        //public override string FieldsColumnSize( ColumnSize columnSize ) => $"is-{ColumnSize( columnSize )}";

        #endregion

        #region Field

        public override string Field() => "field";

        public override string FieldHorizontal() => "is-horizontal";

        public override string FieldColumn() => $"{Column()}";

        public override string FieldJustifyContent( JustifyContent justifyContent ) => ToJustifyContent( justifyContent );

        public override string FieldValidation( ValidationStatus validationStatus ) => null;

        #endregion

        #region FieldLabel

        public override string FieldLabel() => "field-label";

        public override string FieldLabelHorizontal() => "is-normal";

        #endregion

        #region FieldBody

        public override string FieldBody() => "field-body";

        #endregion

        #region FieldHelp

        public override string FieldHelp() => "help";

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

        public override string AddonLabel() => "button is-static";

        //public override string AddonContainer() => "control";

        #endregion

        #region Inline

        public override string Inline() => "field is-horizontal";

        #endregion

        #region Button

        public override string Button() => "button";

        public override string ButtonColor( Color color ) => $"is-{ToColor( color )}";

        public override string ButtonOutline( Color color ) => $"is-{ToColor( color )} is-outlined";

        public override string ButtonSize( ButtonSize buttonSize )
        {
            switch ( buttonSize )
            {
                case Blazorise.ButtonSize.Small:
                    return "is-small";
                case Blazorise.ButtonSize.Large:
                    return "is-large";
                default:
                    return null;
            }
        }

        public override string ButtonBlock() => $"is-fullwidth";

        public override string ButtonActive() => "is-active";

        public override string ButtonLoading() => "is-loading";

        #endregion

        #region Buttons

        //public override string Buttons() => "buttons has-addons";

        public override string ButtonsAddons() => "field has-addons";

        public override string ButtonsToolbar() => "field is-grouped";

        public override string ButtonsSize( ButtonsSize buttonsSize )
        {
            switch ( buttonsSize )
            {
                case Blazorise.ButtonsSize.Small:
                    return "are-small";
                case Blazorise.ButtonsSize.Large:
                    return "are-large";
                default:
                    return null;
            }
        }

        public override string ButtonsVertical() => "buttons";

        #endregion

        #region CloseButton

        public override string CloseButton() => "delete";

        #endregion

        #region Dropdown

        public override string Dropdown() => "dropdown";

        public override string DropdownGroup() => "field has-addons";

        public override string DropdownShow() => Active();

        public override string DropdownRight() => "is-right";

        public override string DropdownItem() => "dropdown-item";

        public override string DropdownItemActive() => Active();

        public override string DropdownDivider() => "dropdown-divider";

        public override string DropdownMenu() => "dropdown-menu";

        //public override string DropdownMenuBody() => "dropdown-content";

        public override string DropdownMenuVisible( bool visible ) => null;

        public override string DropdownMenuRight() => null;

        public override string DropdownToggle() => "button dropdown-trigger";

        public override string DropdownToggleColor( Color color ) => $"is-{ToColor( color )}";

        public override string DropdownToggleOutline( Color color ) => $"is-{ToColor( color )} is-outlined";

        public override string DropdownToggleSize( ButtonSize buttonSize )
        {
            switch ( buttonSize )
            {
                case Blazorise.ButtonSize.Small:
                    return "is-small";
                case Blazorise.ButtonSize.Large:
                    return "is-large";
                default:
                    return null;
            }
        }

        public override string DropdownToggleSplit() => null;

        public override string DropdownDirection( Direction direction )
        {
            switch ( direction )
            {
                case Direction.Up:
                    return "is-up";
                case Direction.Right:
                    return "is-right";
                case Direction.Left:
                    return "is-left";
                case Direction.Down:
                case Direction.None:
                default:
                    return null;
            }
        }

        #endregion

        #region Tab

        public override string Tabs() => "tabs";

        public override string TabsCards() => null;

        public override string TabsPills() => "is-toggle";

        public override string TabsFullWidth() => "is-fullwidth";

        public override string TabsJustified() => "is-justified";

        public override string TabsVertical() => "is-vertical"; // this is custom class, bulma natively does not have vertical tabs

        public override string TabItem() => null;

        public override string TabItemActive( bool active ) => active ? $"{Active()}" : null;

        public override string TabItemDisabled( bool disabled ) => null;

        public override string TabLinkDisabled( bool disabled ) => disabled ? "is-disabled" : null;

        public override string TabLink() => null;

        public override string TabLinkActive( bool active ) => null;

        public override string TabsContent() => "tab-content";

        public override string TabPanel() => "tab-pane";

        public override string TabPanelActive( bool active ) => active ? $"{Active()}" : null;

        #endregion

        #region Jumbotron

        public override string Jumbotron() => "hero";

        public override string JumbotronBackground( Background background ) => $"hero-{ToBackground( background )}";

        public override string JumbotronTitle( JumbotronTitleSize jumbotronTitleSize ) => $"title is-{ToJumbotronTitleSize( jumbotronTitleSize )}";

        public override string JumbotronSubtitle() => "subtitle";

        #endregion

        #region Card

        public override string CardGroup() => "card-group";

        public override string Card() => "card";

        public override string CardWhiteText() => "has-text-white";

        public override string CardBackground( Background background ) => BackgroundColor( background );

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

        #endregion

        #region ListGroup

        public override string ListGroup() => "list-group";

        public override string ListGroupFlush() => "list-group-flush";

        public override string ListGroupItem() => "list-group-item";

        public override string ListGroupItemActive() => Active();

        public override string ListGroupItemDisabled() => Disabled();

        #endregion

        #region Container

        public override string Container() => "container";

        public override string ContainerFluid() => "container-fluid";

        #endregion

        #region Bar

        public override string Bar() => "navbar";

        public override string BarBackground( Background background ) => BackgroundColor( background );

        public override string BarAlignment( Alignment alignment ) => FlexAlignment( alignment );

        public override string BarThemeContrast( ThemeContrast themeContrast ) => null;

        public override string BarBreakpoint( Breakpoint breakpoint ) => $"navbar-expand-{ToBreakpoint( breakpoint )}";

        public override string BarItem() => "navbar-item";

        public override string BarItemActive() => Active();

        public override string BarItemDisabled() => Disabled();

        public override string BarItemHasDropdown() => "has-dropdown";

        public override string BarItemHasDropdownShow() => Active();

        public override string BarLink() => "navbar-item";

        public override string BarLinkDisabled() => Disabled();

        //public override string BarCollapse() => "navbar-menu";

        public override string BarBrand() => "navbar-brand";

        public override string BarToggler() => "navbar-burger";

        public override string BarTogglerCollapsed( bool isShow ) => isShow ? Active() : null;

        public override string BarMenu() => "navbar-menu";

        public override string BarMenuShow() => Active();

        public override string BarStart() => "navbar-start";

        public override string BarEnd() => "navbar-end";

        //public override string BarHasDropdown() => "has-dropdown";

        public override string BarDropdown() => null;

        public override string BarDropdownShow() => null;

        public override string BarDropdownToggle() => "navbar-link";

        public override string BarDropdownItem() => "navbar-item";

        public override string BarTogglerIcon() => null;

        public override string BarDropdownMenu() => "navbar-dropdown";

        public override string BarDropdownMenuVisible( bool visible ) => visible ? Show() : null;

        public override string BarDropdownMenuRight() => "is-right";

        #endregion

        #region Accordion

        public override string Accordion() => "accordion";

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

        public override string Row() => "columns";

        #endregion

        #region Column

        public override string Column() => "column";

        public override string Column( ColumnWidth columnWidth, Breakpoint breakpoint, bool offset )
        {
            var baseClass = offset ? "offset-" : null;

            if ( breakpoint != Blazorise.Breakpoint.None )
            {
                if ( columnWidth == Blazorise.ColumnWidth.None )
                    return $"{Column()} is-{baseClass}{ToBreakpoint( breakpoint )}";

                return $"{Column()} is-{baseClass}{ToBreakpoint( breakpoint )}-{ToColumnWidth( columnWidth )}";
            }

            return $"{Column()} is-{baseClass}{ToColumnWidth( columnWidth )}";
        }

        public override string Column( ColumnWidth columnWidth, IEnumerable<(Breakpoint breakpoint, bool offset)> rules ) =>
              string.Join( " ", rules.Select( r => Column( columnWidth, r.breakpoint, r.offset ) ) );

        #endregion

        #region Display

        public override string Display( DisplayType displayType, Breakpoint breakpoint, DisplayDirection direction )
        {
            var baseClass = breakpoint != Breakpoint.None
                ? $"is-{ToDisplayType( displayType )}-{ToBreakpoint( breakpoint )}"
                : $"is-{ToDisplayType( displayType )}";

            if ( direction != DisplayDirection.None )
                return $"{baseClass} is-flex-{ToDisplayDirection( direction )}";

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

        public override string ModalFade() => null;

        public override string ModalVisible( bool visible ) => visible ? Active() : null;

        public override string ModalBackdrop() => "modal-background";

        public override string ModalBackdropFade() => Fade();

        public override string ModalBackdropVisible( bool visible ) => visible ? Show() : null;

        public override string ModalContent( bool dialog ) => dialog ? "modal-card" : "modal-content";

        public override string ModalContentSize( ModalSize modalSize ) => $"modal-{ToModalSize( modalSize )}";

        public override string ModalContentCentered() => null;

        public override string ModalBody() => "modal-card-body";

        public override string ModalHeader() => "modal-card-head";

        public override string ModalFooter() => "modal-card-foot";

        public override string ModalTitle() => "modal-card-title";

        #endregion

        #region Pagination

        public override string Pagination() => "pagination-list";

        public override string PaginationSize( Size size ) => $"{ToSize( size )}";

        public override string PaginationItem() => null;

        public override string PaginationItemActive() => null;

        public override string PaginationItemDisabled() => null;

        public override string PaginationLink() => "pagination-link";

        public override string PaginationLinkActive() => "is-current";

        public override string PaginationLinkDisabled() => "disabled";

        #endregion

        #region Progress

        public override string Progress() => "progress";

        public override string ProgressSize( Size size ) => $"is-{ToSize( size )}";

        public override string ProgressBar() => "progress";

        public override string ProgressBarColor( Background background ) => BackgroundColor( background );

        public override string ProgressBarStriped() => "progress-bar-striped";

        public override string ProgressBarAnimated() => "progress-bar-animated";

        public override string ProgressBarWidth( int width ) => $"w-{width}";

        #endregion

        #region Chart

        public override string Chart() => null;

        #endregion

        #region Colors

        public override string BackgroundColor( Background color ) => $"has-background-{ToBackground( color )}";

        #endregion

        #region Title

        public override string Title() => "title";

        public override string TitleSize( int size ) => $"is-{size}";

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

        public override string TableHeaderThemeContrast( ThemeContrast themeContrast ) => $"has-background-{ToThemeContrast( themeContrast )}";

        public override string TableHeaderCell() => null;

        public override string TableFooter() => null;

        public override string TableBody() => null;

        public override string TableRow() => null;

        public override string TableRowColor( Color color ) => $"has-background-{ToColor( color )}";

        public override string TableRowBackground( Background background ) => BackgroundColor( background );

        public override string TableRowTextColor( TextColor textColor ) => $"has-text-{ToTextColor( textColor )}";

        public override string TableRowHoverCursor() => "table-row-selectable";

        public override string TableRowIsSelected() => "is-selected";

        public override string TableRowHeader() => null;

        public override string TableRowCell() => null;

        public override string TableRowCellColor( Color color ) => $"has-background-{ToColor( color )}";

        public override string TableRowCellBackground( Background background ) => BackgroundColor( background );

        public override string TableRowCellTextColor( TextColor textColor ) => $"has-text-{ToTextColor( textColor )}";

        public override string TableRowCellTextAlignment( TextAlignment textAlignment ) => $"has-text-{ToTextAlignment( textAlignment )}";

        public override string TableResponsive() => "table-container";

        #endregion

        #region Badge

        public override string Badge() => "tag";

        public override string BadgeColor( Color color ) => $"is-{ToColor( color )}";

        public override string BadgePill() => null;

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

        public override string TextItalic() => "is-italic";

        #endregion

        #region Heading

        public override string HeadingSize( HeadingSize headingSize ) => $"title is-{ToHeadingSize( headingSize )}";

        public override string HeadingTextColor( TextColor textColor ) => $"has-text-{ToTextColor( textColor )}";

        #endregion

        #region DisplayHeading

        public override string DisplayHeadingSize( DisplayHeadingSize displayHeadingSize ) => $"title is-{ToDisplayHeadingSize( displayHeadingSize )}";

        #endregion

        #region Paragraph

        public override string Paragraph() => null;

        public override string ParagraphColor( TextColor textColor ) => $"has-text-{ToTextColor( textColor )}";

        #endregion

        #region Figure

        public override string Figure() => "image";

        public override string FigureSize( FigureSize figureSize ) => $"is-{ToFigureSize( figureSize )}";

        public override string FigureImage() => "figure-img";

        public override string FigureImageRounded() => "is-rounded";

        public override string FigureCaption() => "figure-caption";

        #endregion

        #region Breadcrumb

        public override string Breadcrumb() => "breadcrumb";

        public override string BreadcrumbItem() => null;

        public override string BreadcrumbItemActive() => Active();

        public override string BreadcrumbLink() => null;

        #endregion

        #region Tooltip

        public override string Tooltip() => "b-tooltip";

        public override string TooltipPlacement( Placement placement ) => $"b-tooltip-{ToPlacement( placement )}";

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

        #region Flex

        public override string FlexAlignment( Alignment alignment ) => $"justify-content-{ToAlignment( alignment )}";

        #endregion

        #region Enums

        public override string ToSize( Size size )
        {
            switch ( size )
            {
                case Blazorise.Size.ExtraSmall:
                case Blazorise.Size.Small:
                    return "is-small";
                case Blazorise.Size.Medium:
                    return "is-medium";
                case Blazorise.Size.Large:
                case Blazorise.Size.ExtraLarge:
                    return "is-large";
                default:
                    return null;
            }
        }

        public override string ToBreakpoint( Breakpoint breakpoint )
        {
            switch ( breakpoint )
            {
                case Blazorise.Breakpoint.Mobile:
                    return "mobile";
                case Blazorise.Breakpoint.Tablet:
                    return "tablet";
                case Blazorise.Breakpoint.Desktop:
                    return "desktop";
                case Blazorise.Breakpoint.Widescreen:
                    return "widescreen";
                case Blazorise.Breakpoint.FullHD:
                    return "fullhd";
                default:
                    return null;
            }
        }

        public override string ToBackground( Background color )
        {
            switch ( color )
            {
                case Blazorise.Background.Primary:
                    return "primary";
                case Blazorise.Background.Secondary:
                    return "light";
                case Blazorise.Background.Success:
                    return "success";
                case Blazorise.Background.Danger:
                    return "danger";
                case Blazorise.Background.Warning:
                    return "warning";
                case Blazorise.Background.Info:
                    return "info";
                case Blazorise.Background.Light:
                    return "light";
                case Blazorise.Background.Dark:
                    return "dark";
                case Blazorise.Background.White:
                    return "white";
                case Blazorise.Background.Transparent:
                    return "transparent";
                default:
                    return null;
            }
        }

        public override string ToTextColor( TextColor textColor )
        {
            switch ( textColor )
            {
                case Blazorise.TextColor.Primary:
                    return "primary";
                //case Blazorise.TextColor.Secondary:
                //    return "secondary";
                case Blazorise.TextColor.Success:
                    return "success";
                case Blazorise.TextColor.Danger:
                    return "danger";
                case Blazorise.TextColor.Warning:
                    return "warning";
                case Blazorise.TextColor.Info:
                    return "info";
                case Blazorise.TextColor.Light:
                    return "light";
                case Blazorise.TextColor.Dark:
                    return "dark";
                //case Blazorise.TextColor.Body:
                //    return "body";
                //case Blazorise.TextColor.Muted:
                //    return "muted";
                case Blazorise.TextColor.White:
                    return "white";
                //case Blazorise.TextColor.Black50:
                //    return "black-50";
                //case Blazorise.TextColor.White50:
                //    return "white-50";
                default:
                    return null;
            }
        }

        public override string ToFloat( Float @float )
        {
            switch ( @float )
            {
                case Blazorise.Float.Left:
                    return "is-pulled-left";
                case Blazorise.Float.Right:
                    return "is-pulled-right	";
                default:
                    return null;
            }
        }

        public override string ToColumnWidth( ColumnWidth columnWidth )
        {
            switch ( columnWidth )
            {
                case Blazorise.ColumnWidth.Is1:
                    return "1";
                case Blazorise.ColumnWidth.Is2:
                    return "2";
                case Blazorise.ColumnWidth.Is3:
                case Blazorise.ColumnWidth.Quarter:
                    return "3";
                case Blazorise.ColumnWidth.Is4:
                case Blazorise.ColumnWidth.Third:
                    return "4";
                case Blazorise.ColumnWidth.Is5:
                    return "5";
                case Blazorise.ColumnWidth.Is6:
                case Blazorise.ColumnWidth.Half:
                    return "6";
                case Blazorise.ColumnWidth.Is7:
                    return "7";
                case Blazorise.ColumnWidth.Is8:
                    return "8";
                case Blazorise.ColumnWidth.Is9:
                    return "9";
                case Blazorise.ColumnWidth.Is10:
                    return "10";
                case Blazorise.ColumnWidth.Is11:
                    return "11";
                case Blazorise.ColumnWidth.Is12:
                case Blazorise.ColumnWidth.Full:
                    return "12";
                default:
                    return null;
            }
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

        public override string ToTextAlignment( TextAlignment textAlignment )
        {
            switch ( textAlignment )
            {
                case Blazorise.TextAlignment.Left:
                    return "left";
                case Blazorise.TextAlignment.Center:
                    return "centered";
                case Blazorise.TextAlignment.Right:
                    return "right";
                case Blazorise.TextAlignment.Justified:
                    return "justified";
                default:
                    return null;
            }
        }

        #endregion

        public override bool UseCustomInputStyles { get; set; } = false;

        public override string Provider => "Bulma";
    }
}
