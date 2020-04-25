#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Providers
{
    /// <summary>
    /// Used only when user wants to use extensions(Chart, Sidebar, etc) without CSS frameworks!!
    /// </summary>
    class EmptyClassProvider : IClassProvider
    {
        #region TextEdit

        public string TextEdit( bool plaintext ) => null;

        public string TextEditSize( Size size ) => null;

        public string TextEditColor( Color color ) => null;

        public string TextEditValidation( ValidationStatus validationStatus ) => null;

        #endregion

        #region MemoEdit

        public string MemoEdit() => null;

        public string MemoEditValidation( ValidationStatus validationStatus ) => null;

        #endregion

        #region Select

        public string Select() => null;

        public string SelectMultiple() => null;

        public string SelectSize( Size size ) => null;

        public string SelectValidation( ValidationStatus validationStatus ) => null;

        #endregion

        #region DateEdit

        public string DateEdit() => null;

        public string DateEditSize( Size size ) => null;

        public string DateEditValidation( ValidationStatus validationStatus ) => null;

        #endregion

        #region TimeEdit

        public string TimeEdit() => null;

        public string TimeEditSize( Size size ) => null;

        public string TimeEditValidation( ValidationStatus validationStatus ) => null;

        #endregion

        #region ColorEdit

        public string ColorEdit() => null;

        #endregion

        #region Check

        public string Check() => null;

        public string CheckInline() => null;

        public string CheckCursor( Cursor cursor ) => null;

        public string CheckValidation( ValidationStatus validationStatus ) => null;

        #endregion

        #region RadioGroup

        public string RadioGroup( bool buttons ) => null;

        public string RadioGroupInline() => null;

        #endregion

        #region Radio

        public string Radio( bool button ) => null;

        public string RadioInline() => null;

        #endregion

        #region Switch

        public string Switch() => null;

        public string SwitchChecked( bool @checked ) => null;

        public string SwitchCursor( Cursor cursor ) => null;

        public string SwitchValidation( ValidationStatus validationStatus ) => null;

        #endregion

        #region FileEdit

        public string FileEdit() => null;

        public string FileEditValidation( ValidationStatus validationStatus ) => null;

        #endregion

        #region Slider

        public string Slider() => null;

        public string SliderColor( Color color ) => null;

        #endregion

        #region Label

        public string Label() => null;

        public string LabelType( LabelType labelType ) => null;

        public string LabelCursor( Cursor cursor ) => null;

        #endregion

        #region Help

        public string Help() => null;

        #endregion

        #region Validation

        public string ValidationSuccess() => null;

        public string ValidationSuccessTooltip() => null;

        public string ValidationError() => null;

        public string ValidationErrorTooltip() => null;

        public string ValidationNone() => null;

        #endregion

        #region Fields

        public string Fields() => null;

        public string FieldsBody() => null;

        public string FieldsColumn() => null;

        #endregion

        #region Field

        public string Field() => null;

        public string FieldHorizontal() => null;

        public string FieldColumn() => null;

        public string FieldJustifyContent( JustifyContent justifyContent ) => null;

        public string FieldValidation( ValidationStatus validationStatus ) => null;

        #endregion

        #region FieldLabel

        public string FieldLabel() => null;

        public string FieldLabelHorizontal() => null;

        #endregion

        #region FieldBody

        public string FieldBody() => null;

        #endregion

        #region FieldHelp

        public string FieldHelp() => null;

        #endregion

        #region Control

        public string ControlCheck() => null;

        public string ControlRadio() => null;

        public string ControlSwitch() => null;

        public string ControlFile() => null;

        public string ControlText() => null;

        #endregion

        #region Addons

        public string Addons() => null;

        public string AddonsHasButton( bool hasButton ) => null;

        public string Addon( AddonType addonType ) => null;

        public string AddonLabel() => null;

        //public string AddonContainer() => null;

        #endregion

        #region Inline

        public string Inline() => "form-inline";

        #endregion

        #region Button

        public string Button() => null;

        public string ButtonColor( Color color ) => null;

        public string ButtonOutline( Color color ) => null;

        public string ButtonSize( ButtonSize buttonSize ) => null;

        public string ButtonBlock() => null;

        public string ButtonActive() => null;

        public string ButtonLoading() => null;

        #endregion

        #region Buttons

        //public  string Buttons() => null;

        public string ButtonsAddons() => null;

        public string ButtonsToolbar() => null;

        public string ButtonsSize( ButtonsSize buttonsSize ) => null;

        public string ButtonsVertical() => null;

        #endregion

        #region CloseButton

        public string CloseButton() => null;

        #endregion

        #region Dropdown

        public string Dropdown() => null;

        public string DropdownGroup() => null;

        public string DropdownShow() => null;

        public string DropdownRight() => null;

        public string DropdownItem() => null;

        public string DropdownItemActive() => null;

        public string DropdownDivider() => null;

        public string DropdownMenu() => null;

        //public string DropdownMenuBody() => null;

        public string DropdownMenuVisible( bool visible ) => null;

        public string DropdownMenuRight() => null;

        public string DropdownToggle() => null;

        public string DropdownToggleColor( Color color ) => null;

        public string DropdownToggleOutline( Color color ) => null;

        public string DropdownToggleSize( ButtonSize buttonSize ) => null;

        public string DropdownToggleSplit() => null;

        public string DropdownDirection( Direction direction ) => null;

        #endregion

        #region Tab

        public string Tabs() => null;

        public string TabsCards() => null;

        public string TabsPills() => null;

        public string TabsFullWidth() => null;

        public string TabsJustified() => null;

        public string TabsVertical() => null;

        public string TabItem() => null;

        public string TabItemActive( bool active ) => null;

        public string TabItemDisabled( bool disabled ) => null;

        public string TabLink() => null;

        public string TabLinkActive( bool active ) => null;

        public string TabLinkDisabled( bool disabled ) => null;

        public string TabsContent() => null;

        public string TabPanel() => null;

        public string TabPanelActive( bool active ) => null;

        #endregion

        #region Jumbotron

        public string Jumbotron() => null;

        public string JumbotronBackground( Background background ) => null;

        public string JumbotronTitle( JumbotronTitleSize jumbotronTitleSize ) => null;

        public string JumbotronSubtitle() => null;

        #endregion

        #region Card

        public string CardGroup() => null;

        public string Card() => null;

        public string CardWhiteText() => null;

        public string CardBackground( Background background ) => null;

        public string CardActions() => null;

        public string CardBody() => null;

        public string CardFooter() => null;

        public string CardHeader() => null;

        public string CardImage() => null;

        public string CardTitle( bool insideHeader ) => null;

        public string CardTitleSize( bool insideHeader, int? size ) => null;

        public string CardSubtitle( bool insideHeader ) => null;

        public string CardSubtitleSize( bool insideHeader, int size ) => null;

        public string CardText() => null;

        public string CardLink() => null;

        #endregion

        #region ListGroup

        public string ListGroup() => null;

        public string ListGroupFlush() => null;

        public string ListGroupItem() => null;

        public string ListGroupItemActive() => null;

        public string ListGroupItemDisabled() => null;

        #endregion

        #region Layout

        public string Layout() => null;

        public string LayoutHasSider() => null;

        public string LayoutBody() => null;

        public string LayoutContent() => null;

        public string LayoutHeader() => null;

        public string LayoutHeaderFixed() => null;

        public string LayoutFooter() => null;

        public string LayoutSider() => null;

        public string LayoutSiderContent() => null;

        #endregion

        #region Container

        public string Container() => null;

        public string ContainerFluid() => null;

        #endregion

        #region Bar

        public string Bar() => null;

        public string BarBackground( Background background ) => null;

        public string BarAlignment( Alignment alignment ) => null;

        public string BarThemeContrast( ThemeContrast themeContrast ) => null;

        public string BarBreakpoint( Breakpoint breakpoint ) => null;

        public string BarItem() => null;

        public string BarItemActive() => null;

        public string BarItemDisabled() => null;

        public string BarItemHasDropdown() => null;

        public string BarItemHasDropdownShow() => null;

        public string BarLink() => null;

        public string BarLinkDisabled() => null;

        //public  string BarCollapse() => null;

        public string BarBrand() => null;

        public string BarToggler() => null;

        public string BarTogglerCollapsed( bool isShow ) => null;

        public string BarMenu() => null;

        public string BarMenuShow() => null;

        public string BarStart() => null;

        public string BarEnd() => null;

        //public  string BarHasDropdown() => null;

        public string BarDropdown() => null;

        public string BarDropdownShow() => null;

        public string BarDropdownToggle() => null;

        public string BarDropdownItem() => null;

        public string BarTogglerIcon() => null;

        public string BarDropdownMenu() => null;

        public string BarDropdownMenuVisible( bool visible ) => null;

        public string BarDropdownMenuRight() => null;

        #endregion

        #region Accordion

        public string Accordion() => null;

        #endregion

        #region Collapse

        public string Collapse() => null;

        public string CollapseActive( bool active ) => null;

        public string CollapseHeader() => null;

        public string CollapseBody() => null;

        public string CollapseBodyActive( bool active ) => null;

        public string CollapseBodyContent() => null;

        #endregion

        #region Row

        public string Row() => null;

        #endregion

        #region Column

        public string Column() => null;

        public string Column( ColumnWidth columnWidth, IEnumerable<(Breakpoint breakpoint, bool offset)> rules ) => null;

        private string Column( ColumnWidth columnWidth, Breakpoint breakpoint, bool offset ) => null;

        #endregion

        #region Display

        public string Display( DisplayType displayType, Breakpoint breakpoint, DisplayDirection direction ) => null;

        public string Display( DisplayType displayType, IEnumerable<(Breakpoint breakpoint, DisplayDirection direction)> rules ) => null;

        #endregion

        #region Alert

        public string Alert() => null;

        public string AlertColor( Color color ) => null;

        public string AlertDismisable() => null;

        public string AlertFade() => null;

        public string AlertShow() => null;

        public string AlertHasMessage() => null;

        public string AlertHasDescription() => null;

        public string AlertMessage() => null;

        public string AlertDescription() => null;

        #endregion

        #region Modal

        public string Modal() => null;

        public string ModalFade() => null;

        public string ModalVisible( bool visible ) => null;

        public string ModalBackdrop() => null;

        public string ModalBackdropFade() => null;

        public string ModalBackdropVisible( bool visible ) => null;

        public string ModalContent( bool dialog ) => null;

        public string ModalContentSize( ModalSize modalSize ) => null;

        public string ModalContentCentered() => null;

        public string ModalBody() => null;

        public string ModalHeader() => null;

        public string ModalFooter() => null;

        public string ModalTitle() => null;

        #endregion

        #region Pagination

        public string Pagination() => null;

        public string PaginationSize( Size size ) => null;

        public string PaginationItem() => null;

        public string PaginationItemActive() => null;

        public string PaginationItemDisabled() => null;

        public string PaginationLink() => null;

        public string PaginationLinkActive() => null;

        public string PaginationLinkDisabled() => null;

        #endregion

        #region Progress

        public string Progress() => null;

        public string ProgressSize( Size size ) => null;

        public string ProgressBar() => null;

        public string ProgressBarColor( Background background ) => null;

        public string ProgressBarStriped() => null;

        public string ProgressBarAnimated() => null;

        public string ProgressBarWidth( int width ) => null;

        #endregion

        #region Chart

        public string Chart() => null;

        #endregion

        #region Colors

        public string BackgroundColor( Background color ) => null;

        #endregion

        #region Title

        public string Title() => null;

        public string TitleSize( int size ) => null;

        #endregion

        #region Table

        public string Table() => null;

        public string TableFullWidth() => null;

        public string TableStriped() => null;

        public string TableHoverable() => null;

        public string TableBordered() => null;

        public string TableNarrow() => null;

        public string TableBorderless() => null;

        public string TableHeader() => null;

        public string TableHeaderThemeContrast( ThemeContrast themeContrast ) => null;

        public string TableHeaderCell() => null;

        public string TableFooter() => null;

        public string TableBody() => null;

        public string TableRow() => null;

        public string TableRowColor( Color color ) => null;

        public string TableRowBackground( Background background ) => null;

        public string TableRowTextColor( TextColor textColor ) => null;

        public string TableRowHoverCursor() => null;

        public string TableRowIsSelected() => null;

        public string TableRowHeader() => null;

        public string TableRowCell() => null;

        public string TableRowCellColor( Color color ) => null;

        public string TableRowCellBackground( Background background ) => null;

        public string TableRowCellTextColor( TextColor textColor ) => null;

        public string TableRowCellTextAlignment( TextAlignment textAlignment ) => null;

        public string TableResponsive() => null;

        #endregion

        #region Badge

        public string Badge() => null;

        public string BadgeColor( Color color ) => null;

        public string BadgePill() => null;

        #endregion

        #region Media

        public string Media() => null;

        public string MediaLeft() => null;

        public string MediaRight() => null;

        public string MediaBody() => null;

        #endregion

        #region Text

        public string TextColor( TextColor textColor ) => null;

        public string TextAlignment( TextAlignment textAlignment ) => null;

        public string TextTransform( TextTransform textTransform ) => null;

        public string TextWeight( TextWeight textWeight ) => null;

        public string TextItalic() => null;

        #endregion

        #region Heading

        public string HeadingSize( HeadingSize headingSize ) => null;

        public string HeadingTextColor( TextColor textColor ) => null;

        #endregion

        #region DisplayHeading

        public string DisplayHeadingSize( DisplayHeadingSize displayHeadingSize ) => null;

        #endregion

        #region Paragraph

        public string Paragraph() => null;

        public string ParagraphColor( TextColor textColor ) => null;

        #endregion

        #region Figure

        public string Figure() => null;

        public string FigureSize( FigureSize figureSize ) => null;

        public string FigureImage() => null;

        public string FigureImageRounded() => null;

        public string FigureCaption() => null;

        #endregion

        #region Breadcrumb

        public string Breadcrumb() => null;

        public string BreadcrumbItem() => null;

        public string BreadcrumbItemActive() => null;

        public string BreadcrumbLink() => null;

        #endregion

        #region Tooltip

        public string Tooltip() => null;

        public string TooltipPlacement( Placement placement ) => null;

        public string TooltipMultiline() => null;

        public string TooltipAlwaysActive() => null;

        public string TooltipFade() => null;

        public string TooltipInline() => null;

        #endregion

        #region Divider

        public string Divider() => null;

        public string DividerType( DividerType dividerType ) => null;

        #endregion

        #region States

        public string Show() => null;

        public string Fade() => null;

        public string Active() => null;

        public string Disabled() => null;

        public string Collapsed() => null;

        #endregion

        #region Layout

        public string Spacing( Spacing spacing, SpacingSize spacingSize, Side side, Breakpoint breakpoint ) => null;

        public string Spacing( Spacing spacing, SpacingSize spacingSize, IEnumerable<(Side side, Breakpoint breakpoint)> rules ) => null;

        #endregion

        #region Flex

        public string FlexAlignment( Alignment alignment ) => null;

        #endregion

        #region Custom

        public string Casing( CharacterCasing characterCasing ) => null;

        #endregion

        #region Enums

        public string ToSize( Size size ) => null;

        public string ToBreakpoint( Breakpoint breakpoint ) => null;

        public string ToColor( Color color ) => null;

        public string ToBackground( Background color ) => null;

        public string ToTextColor( TextColor textColor ) => null;

        public string ToThemeContrast( ThemeContrast themeContrast ) => null;

        public string ToFloat( Float @float ) => null;

        public string ToSpacing( Spacing spacing ) => null;

        public string ToSide( Side side ) => null;

        public string ToAlignment( Alignment alignment ) => null;

        public string ToTextAlignment( TextAlignment textAlignment ) => null;

        public string ToTextTransform( TextTransform textTransform ) => null;

        public string ToTextWeight( TextWeight textWeight ) => null;

        public string ToColumnWidth( ColumnWidth columnWidth ) => null;

        public string ToDisplayType( DisplayType displayType ) => null;

        public string ToDisplayDirection( DisplayDirection displayDirection ) => null;

        public string ToModalSize( ModalSize modalSize ) => null;

        public string ToSpacingSize( SpacingSize spacingSize ) => null;

        public string ToJustifyContent( JustifyContent justifyContent ) => null;

        public string ToScreenreader( Screenreader screenreader ) => null;

        public string ToHeadingSize( HeadingSize headingSize ) => null;

        public string ToDisplayHeadingSize( DisplayHeadingSize displayHeadingSize ) => null;

        public string ToJumbotronTitleSize( JumbotronTitleSize jumbotronTitleSize ) => null;

        public string ToPlacement( Placement placement ) => null;

        public string ToFigureSize( FigureSize figureSize ) => null;

        public string ToCharacterCasing( CharacterCasing characterCasing ) => null;

        #endregion

        #region Properties

        public bool UseCustomInputStyles { get; set; } = false;

        public string Provider => "EmptyClassProvider";

        #endregion
    }
}
