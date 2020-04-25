#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace Blazorise.Frolic
{
    public class FrolicClassProvider : ClassProvider
    {
        #region TextEdit

        public override string TextEdit( bool plaintext ) => "e-control";

        public override string TextEditSize( Size size ) => $"{TextEdit( false )}-{ToSize( size )}";

        public override string TextEditColor( Color color ) => $"text-{ToColor( color )}";

        public override string TextEditValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

        #endregion

        #region Memo

        public override string MemoEdit() => "e-control";

        public override string MemoEditValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

        #endregion

        #region Select

        public override string Select() => "e-select";

        public override string SelectMultiple() => null;

        public override string SelectSize( Size size ) => $"{Select()}-{ToSize( size )}";

        public override string SelectValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

        #endregion

        #region DateEdit

        public override string DateEdit() => "e-control";

        public override string DateEditSize( Size size ) => $"{DateEdit()}-{ToSize( size )}";

        public override string DateEditValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

        #endregion

        #region TimeEdit

        public override string TimeEdit() => "e-control";

        public override string TimeEditSize( Size size ) => $"{TimeEdit()}-{ToSize( size )}";

        public override string TimeEditValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

        #endregion

        #region ColorEdit

        public override string ColorEdit() => "e-control";

        #endregion

        #region Check

        public override string Check() => null;

        public override string CheckInline() => null;

        public override string CheckCursor( Cursor cursor ) => $"e-check-{ToCursor( cursor )}";

        public override string CheckValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

        #endregion

        #region RadioGroup

        public override string RadioGroup( bool buttons ) => buttons ? "e-buttons unified" : null;

        public override string RadioGroupInline() => null;

        #endregion

        #region Radio

        public override string Radio( bool button ) => null;

        public override string RadioInline() => null;

        #endregion

        #region Switch

        public override string Switch() => "e-switch";

        public override string SwitchChecked( bool @checked ) => null;

        public override string SwitchCursor( Cursor cursor ) => $"e-check-{ToCursor( cursor )}";

        public override string SwitchValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

        #endregion

        #region FileEdit

        public override string FileEdit() => "e-control";

        public override string FileEditValidation( ValidationStatus validationStatus ) => ToValidationStatus( validationStatus );

        #endregion

        #region Slider

        public override string Slider() => "e-range";

        public override string SliderColor( Color color ) => ToColor( color );

        #endregion

        #region Label

        public override string Label() => "e-label";

        public override string LabelType( LabelType labelType )
        {
            switch ( labelType )
            {
                case Blazorise.LabelType.File:
                    return "e-label";
                case Blazorise.LabelType.Check:
                case Blazorise.LabelType.Radio:
                case Blazorise.LabelType.Switch:
                case Blazorise.LabelType.None:
                default:
                    return null;
            }
        }

        public override string LabelCursor( Cursor cursor ) => $"e-label-{ToCursor( cursor )}";

        #endregion

        #region Help

        public override string Help() => "e-form-info";

        #endregion

        #region Validation

        public override string ValidationSuccess() => "e-form-info text-success";

        public override string ValidationSuccessTooltip() => "e-form-info text-success";

        public override string ValidationError() => "e-form-info text-danger";

        public override string ValidationErrorTooltip() => "e-form-info text-danger";

        public override string ValidationNone() => "e-form-info text-muted";

        #endregion

        #region Fields

        public override string Fields() => "e-cols";

        public override string FieldsBody() => null;

        public override string FieldsColumn() => $"{Column()}";

        #endregion

        #region Field

        public override string Field() => "e-form-group";

        public override string FieldHorizontal() => "e-cols no-gap";

        public override string FieldColumn() => $"{Column()}";

        public override string FieldJustifyContent( JustifyContent justifyContent ) => ToJustifyContent( justifyContent );

        public override string FieldValidation( ValidationStatus validationStatus ) => null;

        #endregion

        #region FieldLabel

        public override string FieldLabel() => "e-label";

        public override string FieldLabelHorizontal() => "e-form-group";

        #endregion

        #region FieldBody

        public override string FieldBody() => null;

        #endregion

        #region FieldHelp

        public override string FieldHelp() => "form-text text-muted";

        #endregion

        #region Control

        public override string ControlCheck() => UseCustomInputStyles ? "custom-control custom-checkbox" : "form-check";

        public override string ControlRadio() => UseCustomInputStyles ? "custom-control custom-radio" : "form-check";

        public override string ControlSwitch() => UseCustomInputStyles ? "custom-control custom-switch" : "form-check";

        public override string ControlFile() => UseCustomInputStyles ? "custom-control custom-file" : "form-group";

        public override string ControlText() => null;

        #endregion

        #region Addons

        public override string Addons() => "e-form-group unified";

        public override string AddonsHasButton( bool hasButton ) => null;

        public override string Addon( AddonType addonType ) => "e-control-helper";

        public override string AddonLabel() => null;

        //public override string AddonContainer() => null;

        #endregion

        #region Inline

        public override string Inline() => "form-inline";

        #endregion

        #region Button

        public override string Button() => "e-btn";

        public override string ButtonColor( Color color ) => ToColor( color );

        public override string ButtonOutline( Color color ) => color != Blazorise.Color.None ? $"outlined {ToColor( color )}" : $"outlined";

        public override string ButtonSize( ButtonSize buttonSize )
        {
            switch ( buttonSize )
            {
                case Blazorise.ButtonSize.Small:
                    return "small";
                case Blazorise.ButtonSize.Large:
                    return "plus";
                default:
                    return null;
            }
        }

        public override string ButtonBlock() => $"fullwidth";

        public override string ButtonActive() => "active";

        public override string ButtonLoading() => "anime";

        #endregion

        #region Buttons

        //public override string Buttons() => "btn-group";

        public override string ButtonsAddons() => "e-buttons unified";

        public override string ButtonsToolbar() => "e-toolbar";

        public override string ButtonsSize( ButtonsSize buttonsSize )
        {
            switch ( buttonsSize )
            {
                case Blazorise.ButtonsSize.Small:
                    return "small";
                case Blazorise.ButtonsSize.Large:
                    return "plus";
                default:
                    return null;
            }
        }

        public override string ButtonsVertical() => "btn-group-vertical";

        #endregion

        #region CloseButton

        public override string CloseButton() => "e-delete";

        #endregion

        #region Dropdown

        public override string Dropdown() => "e-dropdown";

        public override string DropdownGroup() => "btn-group";

        public override string DropdownShow() => Show();

        public override string DropdownRight() => "drop-right";

        public override string DropdownItem() => "drop-item";

        public override string DropdownItemActive() => Active();

        public override string DropdownDivider() => "dropdown-divider";

        public override string DropdownMenu() => "drop-items";

        //public override string DropdownMenuBody() => null;

        public override string DropdownMenuVisible( bool visible ) => visible ? Show() : null;

        public override string DropdownMenuRight() => null;

        public override string DropdownToggle() => "button dropdown";

        public override string DropdownToggleColor( Color color ) => ToColor( color );

        public override string DropdownToggleOutline( Color color ) => color != Blazorise.Color.None ? $"outlined {ToColor( color )}" : $"outlined";

        public override string DropdownToggleSize( ButtonSize buttonSize )
        {
            switch ( buttonSize )
            {
                case Blazorise.ButtonSize.Small:
                    return "small";
                case Blazorise.ButtonSize.Large:
                    return "plus";
                default:
                    return null;
            }
        }

        public override string DropdownToggleSplit() => "button split";

        public override string DropdownDirection( Direction direction )
        {
            switch ( direction )
            {
                case Direction.Up:
                    return "dropup";
                case Direction.Right:
                    return "dropright";
                case Direction.Left:
                    return "dropleft";
                case Direction.Down:
                case Direction.None:
                default:
                    return null;
            }
        }

        #endregion

        #region Tab

        public override string Tabs() => "e-tabs";

        public override string TabsCards() => "card-header-tabs";

        public override string TabsPills() => "nav-pills";

        public override string TabsFullWidth() => "nav-fill";

        public override string TabsJustified() => "nav-justified";

        public override string TabsVertical() => "vertical";

        public override string TabItem() => null;

        public override string TabItemActive( bool active ) => active ? Active() : null;

        public override string TabItemDisabled( bool disabled ) => null;

        public override string TabLink() => null;

        public override string TabLinkActive( bool active ) => null;

        public override string TabLinkDisabled( bool disabled ) => null;

        public override string TabsContent() => "e-tabs-content";

        public override string TabPanel() => "e-tabs-panel";

        public override string TabPanelActive( bool active ) => active ? Active() : null;

        #endregion

        #region Jumbotron

        public override string Jumbotron() => "e-face";

        public override string JumbotronBackground( Background background ) => $"e-face-{ToBackground( background )}";

        public override string JumbotronTitle( JumbotronTitleSize jumbotronTitleSize ) => $"e-title size-{ToJumbotronTitleSize( jumbotronTitleSize )}";

        public override string JumbotronSubtitle() => "e-face-subtitle";

        #endregion

        #region Card

        public override string CardGroup() => "e-cards unified";

        public override string Card() => "e-card";

        public override string CardWhiteText() => "text-white";

        public override string CardBackground( Background background ) => ToBackground( background );

        public override string CardActions() => "card-actions";

        public override string CardBody() => "card-body";

        public override string CardFooter() => "card-body";

        public override string CardHeader() => "card-body";

        public override string CardImage() => null;

        public override string CardTitle( bool insideHeader ) => "card-title";

        public override string CardTitleSize( bool insideHeader, int? size ) => null;

        public override string CardSubtitle( bool insideHeader ) => "card-subtitle";

        public override string CardSubtitleSize( bool insideHeader, int size ) => null;

        public override string CardText() => "card-text";

        public override string CardLink() => "card-link";

        #endregion

        #region ListGroup

        public override string ListGroup() => "e-list";

        public override string ListGroupFlush() => "no-border";

        public override string ListGroupItem() => "e-list-item";

        public override string ListGroupItemActive() => Active();

        public override string ListGroupItemDisabled() => Disabled();

        #endregion

        #region Container

        public override string Container() => "e-container";

        public override string ContainerFluid() => "e-container-fluid";

        #endregion

        #region Bar

        public override string Bar() => "e-nav";

        public override string BarBackground( Background background ) => BackgroundColor( background );

        public override string BarAlignment( Alignment alignment ) => FlexAlignment( alignment );

        public override string BarThemeContrast( ThemeContrast themeContrast ) => $"navbar-{ToThemeContrast( themeContrast )}";

        public override string BarBreakpoint( Breakpoint breakpoint ) => $"navbar-expand-{ToBreakpoint( breakpoint )}";

        public override string BarItem() => null;

        public override string BarItemActive() => null;

        public override string BarItemDisabled() => Disabled();

        public override string BarItemHasDropdown() => "dropdown";

        public override string BarItemHasDropdownShow() => Show();

        public override string BarLink() => "e-menu-item";

        public override string BarLinkDisabled() => Disabled();

        //public override string BarCollapse() => "navbar-collapse";

        public override string BarBrand() => "navbar-brand";

        public override string BarToggler() => "e-btn no-shadow no-desktop";

        public override string BarTogglerCollapsed( bool isShow ) => isShow ? Show() : null;

        public override string BarMenu() => "e-menu";

        public override string BarMenuShow() => Show();

        public override string BarStart() => "e-distribution";

        public override string BarEnd() => "e-distribution";

        //public override string BarHasDropdown() => "dropdown";

        public override string BarDropdown() => "e-dropdown";

        public override string BarDropdownShow() => null;

        public override string BarDropdownToggle() => null;

        public override string BarDropdownItem() => "drop-item";

        public override string BarTogglerIcon() => "navbar-toggler-icon";

        public override string BarDropdownMenu() => "drop-items";

        public override string BarDropdownMenuVisible( bool visible ) => visible ? Show() : null;

        public override string BarDropdownMenuRight() => "drop-items-right";

        #endregion

        #region Accordion

        public override string Accordion() => "accordion";

        #endregion

        #region Collapse

        public override string Collapse() => "e-card";

        public override string CollapseActive( bool active ) => null;

        public override string CollapseHeader() => "card-body";

        public override string CollapseBody() => "collapse";

        public override string CollapseBodyActive( bool active ) => active ? Show() : null;

        public override string CollapseBodyContent() => "card-body";

        #endregion

        #region Row

        public override string Row() => "e-cols";

        #endregion

        #region Column

        public override string Column() => "e-col";

        public override string Column( ColumnWidth columnWidth, Breakpoint breakpoint, bool offset )
        {
            var baseClass = offset ? "offset" : Column();

            if ( breakpoint != Blazorise.Breakpoint.None )
            {
                if ( columnWidth == Blazorise.ColumnWidth.None )
                    return $"{baseClass}-{ToBreakpoint( breakpoint )}";

                return $"{baseClass}-{ToBreakpoint( breakpoint )}-{ToColumnWidth( columnWidth )}";
            }

            return $"{baseClass}-{ToColumnWidth( columnWidth )}";
        }

        public override string Column( ColumnWidth columnWidth, IEnumerable<(Breakpoint breakpoint, bool offset)> rules ) =>
            string.Join( " ", rules.Select( r => Column( columnWidth, r.breakpoint, r.offset ) ) );

        #endregion

        #region Display

        public override string Display( DisplayType displayType, Breakpoint breakpoint, DisplayDirection direction )
        {
            var baseClass = breakpoint != Breakpoint.None
                ? $"e-is-{ToBreakpoint( breakpoint )}-{ToDisplayType( displayType )}"
                : $"e-is-{ToDisplayType( displayType )}";

            if ( direction != DisplayDirection.None )
                return $"{baseClass} flex-{ToDisplayDirection( direction )}";

            return baseClass;
        }

        #endregion

        #region Alert

        public override string Alert() => "e-alert";

        public override string AlertColor( Color color ) => ToColor( color );

        public override string AlertDismisable() => null;

        public override string AlertFade() => Fade();

        public override string AlertShow() => Show();

        public override string AlertHasMessage() => null;

        public override string AlertHasDescription() => null;

        public override string AlertMessage() => null;

        public override string AlertDescription() => null;

        #endregion

        #region Modal

        public override string Modal() => "e-modal";

        public override string ModalFade() => "e-modal-e";

        public override string ModalVisible( bool visible ) => visible ? "launch" : null;

        public override string ModalBackdrop() => "e-modal-backdrop";

        public override string ModalBackdropFade() => Fade();

        public override string ModalBackdropVisible( bool visible ) => visible ? Show() : null;

        public override string ModalContent( bool dialog ) => "e-modal-content";

        public override string ModalContentSize( ModalSize modalSize ) => $"modal-{ToModalSize( modalSize )}";

        public override string ModalContentCentered() => "modal-dialog-centered";

        public override string ModalBody() => "e-modal-body";

        public override string ModalHeader() => "e-modal-header";

        public override string ModalFooter() => "e-modal-footer";

        public override string ModalTitle() => "e-modal-title";

        #endregion

        #region Pagination

        public override string Pagination() => "e-pagination";

        public override string PaginationSize( Size size ) => $"{Pagination()}-{ToSize( size )}";

        public override string PaginationItem() => "e-page-item";

        public override string PaginationItemActive() => "on-page";

        public override string PaginationItemDisabled() => Disabled();

        public override string PaginationLink() => null;

        public override string PaginationLinkActive() => null;

        public override string PaginationLinkDisabled() => null;

        #endregion

        #region Progress

        public override string Progress() => null;

        public override string ProgressSize( Size size ) => null;

        public override string ProgressBar() => "e-progress";

        public override string ProgressBarColor( Background background ) => ToBackground( background );

        public override string ProgressBarStriped() => "progress-bar-striped";

        public override string ProgressBarAnimated() => "progress-bar-animated";

        public override string ProgressBarWidth( int width ) => $"w-{width}";

        #endregion

        #region Chart

        public override string Chart() => null;

        #endregion

        #region Colors

        public override string BackgroundColor( Background color ) => $"bg-{ToBackground( color )}";

        #endregion

        #region Title

        public override string Title() => "e-title";

        public override string TitleSize( int size ) => $"h{size}";

        #endregion

        #region Table

        public override string Table() => "e-table";

        public override string TableFullWidth() => null;

        public override string TableStriped() => "striped";

        public override string TableHoverable() => "hovered";

        public override string TableBordered() => "bordered";

        public override string TableNarrow() => "narrowed";

        public override string TableBorderless() => "borderless";

        public override string TableHeader() => "e-thead";

        public override string TableHeaderThemeContrast( ThemeContrast themeContrast ) => ToThemeContrast( themeContrast );

        public override string TableHeaderCell() => null;

        public override string TableFooter() => null;

        public override string TableBody() => null;

        public override string TableRow() => "e-row";

        public override string TableRowColor( Color color ) => ToColor( color );

        public override string TableRowBackground( Background background ) => ToBackground( background );

        public override string TableRowTextColor( TextColor textColor ) => $"text-{ToTextColor( textColor )}";

        public override string TableRowHoverCursor() => "table-row-selectable";

        public override string TableRowIsSelected() => "selected";

        public override string TableRowHeader() => null;

        public override string TableRowCell() => null;

        public override string TableRowCellColor( Color color ) => ToColor( color );

        public override string TableRowCellBackground( Background background ) => ToBackground( background );

        public override string TableRowCellTextColor( TextColor textColor ) => $"text-{ToTextColor( textColor )}";

        public override string TableRowCellTextAlignment( TextAlignment textAlignment ) => $"text-{ToTextAlignment( textAlignment )}";

        public override string TableResponsive() => "e-table-responsive";

        #endregion

        #region Badge

        public override string Badge() => "e-tag";

        public override string BadgeColor( Color color ) => $"{Badge()} {ToColor( color )}";

        public override string BadgePill() => $"{Badge()} rounded";

        #endregion

        #region Media

        public override string Media() => "e-media";

        public override string MediaLeft() => "media-left";

        public override string MediaRight() => "media-right";

        public override string MediaBody() => "e-media-body";

        #endregion

        #region Text

        public override string TextColor( TextColor textColor ) => $"text-{ToTextColor( textColor )}";

        public override string TextAlignment( TextAlignment textAlignment ) => $"text-{ToTextAlignment( textAlignment )}";

        public override string TextTransform( TextTransform textTransform ) => $"text-{ToTextTransform( textTransform )}";

        public override string TextWeight( TextWeight textWeight ) => $"font-weight-{ToTextWeight( textWeight )}";

        public override string TextItalic() => "font-italic";

        #endregion

        #region Heading

        public override string HeadingSize( HeadingSize headingSize ) => null;

        public override string HeadingTextColor( TextColor textColor ) => $"text-{ToTextColor( textColor )}";

        #endregion

        #region DisplayHeading

        public override string DisplayHeadingSize( DisplayHeadingSize displayHeadingSize ) => $"size-{ToDisplayHeadingSize( displayHeadingSize )}";

        #endregion

        #region Paragraph

        public override string Paragraph() => null;

        public override string ParagraphColor( TextColor textColor ) => $"text-{ToTextColor( textColor )}";

        #endregion

        #region Figure

        public override string Figure() => "e-figure";

        public override string FigureSize( FigureSize figureSize ) => $"e-figure-is-{ToFigureSize( figureSize )}";

        public override string FigureImage() => "e-figure-img";

        public override string FigureImageRounded() => "e-rounded";

        public override string FigureCaption() => "e-figure-caption";

        #endregion

        #region Breadcrumb

        public override string Breadcrumb() => "e-breadcrumb";

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

        #region Flex

        public override string FlexAlignment( Alignment alignment ) => $"justify-content-{ToAlignment( alignment )}";

        #endregion

        #region Enums

        public override string ToSize( Size size )
        {
            switch ( size )
            {
                case Blazorise.Size.ExtraSmall:
                    return "tiny";
                case Blazorise.Size.Small:
                    return "small";
                case Blazorise.Size.Medium:
                    return null;
                case Blazorise.Size.Large:
                case Blazorise.Size.ExtraLarge:
                    return "large";
                default:
                    return null;
            }
        }

        public override string ToColor( Color color )
        {
            switch ( color )
            {
                case Blazorise.Color.Primary:
                    return "primary";
                case Blazorise.Color.Secondary:
                    return "gray";
                case Blazorise.Color.Success:
                    return "success";
                case Blazorise.Color.Danger:
                    return "danger";
                case Blazorise.Color.Warning:
                    return "warning";
                case Blazorise.Color.Info:
                    return "sky";
                case Blazorise.Color.Light:
                    return "light";
                case Blazorise.Color.Dark:
                    return "dark";
                case Blazorise.Color.Link:
                    return "link";
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
                    return "gray";
                case Blazorise.Background.Success:
                    return "success";
                case Blazorise.Background.Danger:
                    return "danger";
                case Blazorise.Background.Warning:
                    return "warning";
                case Blazorise.Background.Info:
                    return "sky";
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

        public override string ToSpacingSize( SpacingSize spacingSize )
        {
            switch ( spacingSize )
            {
                case Blazorise.SpacingSize.Is0:
                    return "0";
                case Blazorise.SpacingSize.Is1:
                    return "5";
                case Blazorise.SpacingSize.Is2:
                    return "4";
                case Blazorise.SpacingSize.Is3:
                    return "3";
                case Blazorise.SpacingSize.Is4:
                    return "3";
                case Blazorise.SpacingSize.Is5:
                    return "1";
                case Blazorise.SpacingSize.IsAuto:
                    return "auto";
                default:
                    return null;
            }
        }

        #endregion

        public override bool UseCustomInputStyles { get; set; } = true;

        public override string Provider => "eFrolic";
    }
}
