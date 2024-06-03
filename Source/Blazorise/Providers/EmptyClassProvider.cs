#region Using directives
using System.Collections.Generic;
#endregion

namespace Blazorise.Providers;

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

    public string MemoEdit( bool plaintext ) => null;

    public string MemoEditSize( Size size ) => null;

    public string MemoEditValidation( ValidationStatus validationStatus ) => null;

    #endregion

    #region Select

    public string Select() => null;

    public string SelectMultiple() => null;

    public string SelectSize( Size size ) => null;

    public string SelectValidation( ValidationStatus validationStatus ) => null;

    #endregion

    #region NumericEdit

    public string NumericEdit( bool plaintext ) => null;

    public string NumericEditSize( Size size ) => null;

    public string NumericEditColor( Color color ) => null;

    public string NumericEditValidation( ValidationStatus validationStatus ) => null;

    #endregion

    #region DateEdit

    public string DateEdit( bool plaintext ) => null;

    public string DateEditSize( Size size ) => null;

    public string DateEditColor( Color color ) => null;

    public string DateEditValidation( ValidationStatus validationStatus ) => null;

    #endregion

    #region TimeEdit

    public string TimeEdit( bool plaintext ) => null;

    public string TimeEditSize( Size size ) => null;

    public string TimeEditColor( Color color ) => null;

    public string TimeEditValidation( ValidationStatus validationStatus ) => null;

    #endregion

    #region ColorEdit

    public string ColorEdit() => null;

    public string ColorEditSize( Size size ) => null;

    #endregion

    #region DatePicker

    public string DatePicker( bool plaintext ) => null;

    public string DatePickerSize( Size size ) => null;

    public string DatePickerColor( Color color ) => null;

    public string DatePickerValidation( ValidationStatus validationStatus ) => null;

    #endregion

    #region TimePicker

    public string TimePicker( bool plaintext ) => null;

    public string TimePickerSize( Size size ) => null;

    public string TimePickerColor( Color color ) => null;

    public string TimePickerValidation( ValidationStatus validationStatus ) => null;

    #endregion

    #region ColorPicker

    public string ColorPicker() => null;

    public string ColorPickerSize( Size size ) => null;

    #endregion

    #region NumericPicker

    public string NumericPicker( bool plaintext ) => null;

    public string NumericPickerSize( Size size ) => null;

    public string NumericPickerColor( Color color ) => null;

    public string NumericPickerValidation( ValidationStatus validationStatus ) => null;

    #endregion

    #region InputMask

    public string InputMask( bool plaintext ) => null;

    public string InputMaskSize( Size size ) => null;

    public string InputMaskColor( Color color ) => null;

    public string InputMaskValidation( ValidationStatus validationStatus ) => null;

    #endregion

    #region Check

    public string Check() => null;

    public string CheckSize( Size size ) => null;

    public string CheckInline() => null;

    public string CheckCursor( Cursor cursor ) => null;

    public string CheckValidation( ValidationStatus validationStatus ) => null;

    #endregion

    #region RadioGroup

    public string RadioGroup( bool buttons, Orientation orientation ) => null;

    public string RadioGroupSize( bool buttons, Orientation orientation, Size size ) => null;

    public string RadioGroupValidation( ValidationStatus validationStatus ) => null;

    #endregion

    #region Radio

    public string Radio( bool button ) => null;

    public string RadioSize( bool button, Size size ) => null;

    public string RadioInline( bool inline ) => null;

    public string RadioCursor( Cursor cursor ) => null;

    public string RadioValidation( ValidationStatus validationStatus ) => null;

    #endregion

    #region Switch

    public string Switch() => null;

    public string SwitchColor( Color color ) => null;

    public string SwitchSize( Size size ) => null;

    public string SwitchChecked( bool @checked ) => null;

    public string SwitchCursor( Cursor cursor ) => null;

    public string SwitchValidation( ValidationStatus validationStatus ) => null;

    #endregion

    #region FileEdit

    public string FileEdit() => null;

    public string FileEditSize( Size size ) => null;

    public string FileEditValidation( ValidationStatus validationStatus ) => null;

    #endregion

    #region Slider

    public string Slider() => null;

    public string SliderColor( Color color ) => null;

    public string SliderValidation( ValidationStatus validationStatus ) => null;

    #endregion

    #region Rating

    public string Rating() => null;

    public string RatingDisabled( bool disabled ) => null;

    public string RatingReadonly( bool @readonly ) => null;

    public string RatingItem() => null;

    public string RatingItemColor( Color color ) => null;

    public string RatingItemSelected( bool selected ) => null;

    public string RatingItemHovered( bool hover ) => null;

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

    public string ValidationSummary() => null;

    public string ValidationSummaryError() => null;

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

    public string FieldSize( Size size ) => null;

    public string FieldJustifyContent( JustifyContent justifyContent ) => null;

    public string FieldValidation( ValidationStatus validationStatus ) => null;

    #endregion

    #region FieldLabel

    public string FieldLabel( bool horizontal ) => null;

    public string FieldLabelRequiredIndicator( bool requiredIndicator ) => null;

    #endregion

    #region FieldBody

    public string FieldBody() => null;

    #endregion

    #region FieldHelp

    public string FieldHelp() => null;

    #endregion

    #region FocusTrap

    public string FocusTrap() => null;

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

    public string AddonsSize( Size size ) => null;

    public string AddonsHasButton( bool hasButton ) => null;

    public string Addon( AddonType addonType ) => null;

    public string AddonSize( Size size ) => null;

    public string AddonLabel() => null;

    #endregion

    #region Inline

    public string Inline() => "form-inline";

    #endregion

    #region Button

    public string Button( bool outline ) => null;

    public string ButtonColor( Color color, bool outline ) => null;

    public string ButtonSize( Size size, bool outline ) => null;

    public string ButtonBlock( bool outline ) => null;

    public string ButtonActive( bool outline ) => null;

    public string ButtonDisabled( bool outline ) => null;

    public string ButtonLoading( bool outline ) => null;

    public string ButtonStretchedLink( bool stretched ) => null;

    #endregion

    #region Buttons

    public string Buttons( ButtonsRole role, Orientation orientation ) => null;

    public string ButtonsSize( Size size ) => null;

    #endregion

    #region CloseButton

    public string CloseButton() => null;

    #endregion

    #region Dropdown

    public string Dropdown( bool isDropdownSubmenu ) => null;

    public string DropdownDisabled() => null;

    public string DropdownGroup() => null;

    public string DropdownObserverShow() => null;

    public string DropdownShow() => null;

    public string DropdownRight() => null;

    public string DropdownItem() => null;

    public string DropdownItemActive( bool active ) => null;

    public string DropdownItemDisabled( bool disabled ) => null;

    public string DropdownDivider() => null;

    public string DropdownHeader() => null;

    public string DropdownMenu() => null;

    public string DropdownMenuPositionStrategy( DropdownPositionStrategy dropdownPositionStrategy ) => null;

    public string DropdownFixedHeaderVisible( bool visible ) => null;

    public string DropdownMenuSelector() => null;

    public string DropdownMenuScrollable() => null;

    public string DropdownMenuVisible( bool visible ) => null;

    public string DropdownMenuRight() => null;

    public string DropdownToggle( bool isDropdownSubmenu, bool outline ) => null;

    public string DropdownToggleSelector( bool isDropdownSubmenu ) => null;

    public string DropdownToggleColor( Color color, bool outline ) => null;

    public string DropdownToggleSize( Size size, bool outline ) => null;

    public string DropdownToggleSplit( bool split ) => null;

    public string DropdownToggleIcon( bool visible ) => null;

    public string DropdownDirection( Direction direction ) => null;

    #endregion

    #region Tabs

    public string Tabs( bool pills ) => null;

    public string TabsCards() => null;

    public string TabsFullWidth() => null;

    public string TabsJustified() => null;

    public string TabsVertical() => null;

    public string TabItem( TabPosition tabPosition ) => null;

    public string TabItemActive( bool active ) => null;

    public string TabItemDisabled( bool disabled ) => null;

    public string TabLink( TabPosition tabPosition ) => null;

    public string TabLinkActive( bool active ) => null;

    public string TabLinkDisabled( bool disabled ) => null;

    public string TabsContent() => null;

    public string TabPanel() => null;

    public string TabPanelActive( bool active ) => null;

    #endregion

    #region Steps

    public string Steps() => null;

    public string StepItem() => null;

    public string StepItemActive( bool active ) => null;

    public string StepItemCompleted( bool completed ) => null;

    public string StepItemColor( Color color ) => null;

    public string StepItemMarker() => null;

    public string StepItemMarkerColor( Color color, bool active ) => null;

    public string StepItemDescription() => null;

    public string StepsContent() => null;

    public string StepPanel() => null;

    public string StepPanelActive( bool active ) => null;

    #endregion

    #region Carousel

    public string Carousel() => null;

    public string CarouselSlides() => null;

    public string CarouselSlide() => null;

    public string CarouselSlideActive( bool active ) => null;

    public string CarouselSlideIndex( int activeSlideIndex, int slideindex, int totalSlides ) => null;

    public string CarouselSlideSlidingLeft( bool left ) => null;

    public string CarouselSlideSlidingRight( bool right ) => null;

    public string CarouselSlideSlidingPrev( bool previous ) => null;

    public string CarouselSlideSlidingNext( bool next ) => null;

    public string CarouselIndicators() => null;

    public string CarouselIndicator() => null;

    public string CarouselIndicatorActive( bool active ) => null;

    public string CarouselFade( bool fade ) => null;

    public string CarouselCaption() => null;

    #endregion

    #region Jumbotron

    public string Jumbotron() => null;

    public string JumbotronBackground( Background background ) => null;

    public string JumbotronTitle( JumbotronTitleSize jumbotronTitleSize ) => null;

    public string JumbotronSubtitle() => null;

    #endregion

    #region Card

    public string CardDeck() => null;

    public string CardGroup() => null;

    public string Card() => null;

    public string CardWhiteText() => null;

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

    public string CardLinkUnstyled( bool unstyled ) => null;

    public string CardLinkActive( bool active ) => null;

    #endregion

    #region ListGroup

    public string ListGroup() => null;

    public string ListGroupFlush( bool flush ) => null;

    public string ListGroupScrollable( bool scrollable ) => null;

    public string ListGroupItem() => null;

    public string ListGroupItemSelectable() => null;

    public string ListGroupItemActive( bool active ) => null;

    public string ListGroupItemDisabled( bool disabled ) => null;

    public string ListGroupItemColor( Color color, bool selectable, bool active ) => null;

    #endregion

    #region Layout

    public string Layout() => null;

    public string LayoutHasSider() => null;

    public string LayoutBody() => null;

    public string LayoutContent() => null;

    public string LayoutHeader() => null;

    public string LayoutHeaderFixed() => null;

    public string LayoutFooter() => null;

    public string LayoutFooterFixed() => null;

    public string LayoutSider() => null;

    public string LayoutSiderContent() => null;

    public string LayoutLoading() => null;

    public string LayoutRoot() => null;

    #endregion

    #region Container

    public string Container( Breakpoint breakpoint ) => null;

    public string ContainerFluid() => null;

    #endregion

    #region Bar

    public string Bar( BarMode mode ) => null;

    public string BarInitial( BarMode mode, bool initial ) => null;

    public string BarAlignment( BarMode mode, Alignment alignment ) => null;

    public string BarThemeContrast( BarMode mode, ThemeContrast themeContrast ) => null;

    public string BarBreakpoint( BarMode mode, Breakpoint breakpoint ) => null;

    public string BarMode( BarMode mode ) => null;

    public string BarItem( BarMode mode, bool hasDropdown ) => null;

    public string BarItemActive( BarMode mode ) => null;

    public string BarItemDisabled( BarMode mode ) => null;

    public string BarItemHasDropdown( BarMode mode ) => null;

    public string BarItemHasDropdownShow( BarMode mode ) => null;

    public string BarLink( BarMode mode ) => null;

    public string BarLinkDisabled( BarMode mode ) => null;

    public string BarBrand( BarMode mode ) => null;

    public string BarToggler( BarMode mode, BarTogglerMode togglerMode ) => null;

    public string BarTogglerCollapsed( BarMode mode, BarTogglerMode togglerMode, bool isShow ) => null;

    public string BarMenu( BarMode mode ) => null;

    public string BarMenuShow( BarMode mode ) => null;

    public string BarStart( BarMode mode ) => null;

    public string BarEnd( BarMode mode ) => null;

    public string BarDropdown( BarMode mode, bool isBarDropDownSubmenu ) => null;

    public string BarDropdownShow( BarMode mode ) => null;

    public string BarDropdownToggle( BarMode mode, bool isBarDropDownSubmenu ) => null;

    public string BarDropdownToggleDisabled( BarMode mode, bool isBarDropDownSubmenu, bool disabled ) => null;

    public string BarDropdownItem( BarMode mode ) => null;

    public string BarDropdownDivider( BarMode mode ) => null;

    public string BarTogglerIcon( BarMode mode ) => null;

    public string BarDropdownMenu( BarMode mode ) => null;

    public string BarDropdownMenuVisible( BarMode mode, bool visible ) => null;

    public string BarDropdownMenuRight( BarMode mode ) => null;

    public string BarDropdownMenuContainer( BarMode mode ) => null;

    public string BarCollapsed( BarMode mode ) => null;

    public string BarLabel( BarMode mode ) => null;

    #endregion

    #region Accordion

    public string Accordion() => null;

    public string AccordionToggle() => null;

    public string AccordionToggleCollapsed( bool collapsed ) => null;

    public string AccordionItem() => null;

    public string AccordionItemActive( bool active ) => null;

    public string AccordionHeader() => null;

    public string AccordionBody() => null;

    public string AccordionBodyActive( bool active ) => null;

    public string AccordionBodyContent( bool firstInAccordion, bool lastInAccordion ) => null;

    #endregion

    #region Collapse

    public string Collapse( bool accordion ) => null;

    public string CollapseActive( bool accordion, bool active ) => null;

    public string CollapseHeader( bool accordion ) => null;

    public string CollapseBody( bool accordion ) => null;

    public string CollapseBodyActive( bool accordion, bool active ) => null;

    public string CollapseBodyContent( bool accordion, bool firstInAccordion, bool lastInAccordion ) => null;

    #endregion

    #region Row

    public string Row() => null;

    public string RowColumns( RowColumnsSize rowColumnsSize, RowColumnsDefinition rowColumnsDefinition ) => null;

    public string RowNoGutters( bool noGutters ) => null;

    #endregion

    #region Column

    public string Column( bool grid, bool hasSizes ) => null;

    public string Column( bool grid, ColumnWidth columnWidth, Breakpoint breakpoint, bool offset ) => null;

    public string Column( bool grid, IEnumerable<ColumnDefinition> columnDefinitions ) => null;

    #endregion

    #region Grid

    public string Grid() => null;

    public string GridRows( GridRowsSize gridRows, GridRowsDefinition gridRowsDefinition ) => null;

    public string GridColumns( GridColumnsSize gridColumns, GridColumnsDefinition gridColumnsDefinition ) => null;

    #endregion

    #region Display

    public string Display( DisplayType displayType, DisplayDefinition displayDefinition ) => null;

    public string Display( DisplayType displayType, IEnumerable<DisplayDefinition> displayDefinitions ) => null;

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

    public string ModalFade( bool showing, bool hiding ) => null;

    public string ModalVisible( bool visible ) => null;

    public string ModalSize( ModalSize modalSize ) => null;

    public string ModalCentered( bool centered ) => null;

    public string ModalBackdrop() => null;

    public string ModalBackdropFade() => null;

    public string ModalBackdropVisible( bool visible ) => null;

    public string ModalContent( bool dialog ) => null;

    public string ModalContentSize( ModalSize modalSize ) => null;

    public string ModalContentCentered( bool centered ) => null;

    public string ModalContentScrollable( bool scrollable ) => null;

    public string ModalBody() => null;

    public string ModalHeader() => null;

    public string ModalFooter() => null;

    public string ModalTitle() => null;

    #endregion

    #region Offcanvas

    public string Offcanvas() => null;

    public string OffcanvasPlacement( Placement placement, bool visible ) => null;

    public string OffcanvasFade( bool showing, bool hiding ) => null;

    public string OffcanvasVisible( bool visible ) => null;

    public string OffcanvasHeader() => null;

    public string OffcanvasFooter() => null;

    public string OffcanvasBody() => null;

    public string OffcanvasBackdrop() => null;

    public string OffcanvasBackdropFade( bool showing, bool hiding ) => null;

    public string OffcanvasBackdropVisible( bool visible ) => null;

    #endregion

    #region Toast

    public string Toast() => null;

    public string ToastAnimated( bool animated ) => null;

    public string ToastFade( bool visible, bool showing, bool hiding ) => null;

    public string ToastVisible( bool visible ) => null;

    public string ToastHeader() => null;

    public string ToastBody() => null;

    public string Toaster() => null;

    public string ToasterPlacement( ToasterPlacement placement ) => null;

    public string ToasterPlacementStrategy( ToasterPlacementStrategy placementStrategy ) => null;

    #endregion

    #region Pagination

    public string Pagination() => null;

    public string PaginationSize( Size size ) => null;

    public string PaginationItem() => null;

    public string PaginationItemActive( bool active ) => null;

    public string PaginationItemDisabled( bool disabled ) => null;

    public string PaginationLink() => null;

    public string PaginationLinkSize( Size size ) => null;

    public string PaginationLinkActive( bool active ) => null;

    public string PaginationLinkDisabled( bool disabled ) => null;

    #endregion

    #region Progress

    public string Progress() => null;

    public string ProgressSize( Size size ) => null;

    public string ProgressColor( Color color ) => null;

    public string ProgressStriped() => null;

    public string ProgressAnimated() => null;

    public string ProgressIndeterminate() => null;

    public string ProgressWidth( int width ) => null;

    public string ProgressBar() => null;

    public string ProgressBarSize( Size size ) => null;

    public string ProgressBarColor( Color color ) => null;

    public string ProgressBarStriped() => null;

    public string ProgressBarAnimated() => null;

    public string ProgressBarIndeterminate() => null;

    public string ProgressBarWidth( int width ) => null;

    #endregion

    #region Chart

    public string Chart() => null;

    #endregion

    #region Colors

    public string BackgroundColor( Background background ) => null;

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

    public string TableHeaderCellCursor( Cursor cursor ) => null;

    public string TableHeaderCellFixed( TableColumnFixedPosition fixedPosition ) => null;

    public string TableFooter() => null;

    public string TableBody() => null;

    public string TableRow( bool striped, bool hoverable ) => null;

    public string TableRowColor( Color color ) => null;

    public string TableRowHoverCursor() => null;

    public string TableRowIsSelected() => null;

    public string TableRowHeader() => null;

    public string TableRowHeaderFixed( TableColumnFixedPosition fixedPosition ) => null;

    public string TableRowCell() => null;

    public string TableRowCellColor( Color color ) => null;

    public string TableRowCellFixed( TableColumnFixedPosition fixedPosition ) => null;

    public string TableRowGroup( bool expanded ) => null;

    public string TableRowGroupCell() => null;

    public string TableRowGroupIndentCell() => null;

    public string TableResponsive( bool responsive ) => null;

    public string TableFixedHeader( bool fixedHeader ) => null;

    public string TableFixedColumns( bool fixedColumns ) => null;

    #endregion

    #region Badge

    public string Badge() => null;

    public string BadgeColor( Color color ) => null;

    public string BadgePill() => null;

    public string BadgeClose() => null;

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

    public string TextOverflow( TextOverflow textOverflow ) => null;

    public string TextSize( TextSizeType textSizeType, TextSizeDefinition textSizeDefinition ) => null;

    public string TextItalic() => null;

    #endregion

    #region Code

    public string Code() => null;

    #endregion

    #region Heading

    public string HeadingSize( HeadingSize headingSize ) => null;

    #endregion

    #region DisplayHeading

    public string DisplayHeadingSize( DisplayHeadingSize displayHeadingSize ) => null;

    #endregion

    #region Lead

    public string Lead() => null;

    #endregion

    #region Paragraph

    public string Paragraph() => null;

    public string ParagraphColor( TextColor textColor ) => null;

    #endregion

    #region Blockquote

    public string Blockquote() => null;

    public string BlockquoteFooter() => null;

    #endregion

    #region Figure

    public string Figure() => null;

    public string FigureSize( FigureSize figureSize ) => null;

    public string FigureImage() => null;

    public string FigureImageRounded() => null;

    public string FigureCaption() => null;

    #endregion

    #region Image

    public string Image() => null;

    public string ImageFluid( bool fluid ) => null;

    #endregion

    #region Breadcrumb

    public string Breadcrumb() => null;

    public string BreadcrumbItem() => null;

    public string BreadcrumbItemActive() => null;

    public string BreadcrumbLink() => null;

    #endregion

    #region Tooltip

    public string Tooltip() => null;

    public string TooltipPlacement( TooltipPlacement tooltipPlacement ) => null;

    public string TooltipMultiline() => null;

    public string TooltipAlwaysActive() => null;

    public string TooltipFade() => null;

    public string TooltipInline() => null;

    #endregion

    #region Divider

    public string Divider() => null;

    public string DividerType( DividerType dividerType ) => null;

    #endregion

    #region Link

    public string Link() => null;

    public string LinkActive( bool active ) => null;

    public string LinkUnstyled( bool unstyled ) => null;

    public string LinkStretched( bool stretched ) => null;

    public string LinkDisabled( bool disabled ) => null;

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

    #region Gap

    public string Gap( GapSize gapSize, GapSide gapSide ) => null;

    public string Gap( GapSize gapSize, IEnumerable<GapSide> rules ) => null;

    #endregion

    #region Borders

    public string Border( BorderSize borderSize, BorderSide borderSide, BorderColor borderColor ) => null;

    public string Border( BorderSize borderSize, IEnumerable<(BorderSide borderSide, BorderColor borderColor)> rules ) => null;

    public string BorderRadius( BorderRadius borderRadius ) => null;

    #endregion

    #region Flex

    public string Flex( FlexType flexType ) => null;

    public string Flex( FlexDefinition flexDefinition ) => null;

    public string Flex( FlexType flexType, IEnumerable<FlexDefinition> flexDefinitions ) => null;

    public string FlexAlignment( Alignment alignment ) => null;

    #endregion

    #region Sizing

    public string Sizing( SizingType sizingType, SizingSize sizingSize, SizingDefinition sizingDefinition ) => null;

    public string Sizing( SizingType sizingType, SizingSize sizingSize, IEnumerable<SizingDefinition> rules ) => null;

    #endregion

    #region Float

    public string Float( Float @float ) => null;

    public string Clearfix() => null;

    #endregion

    #region Visibility

    public string Visibility( Visibility visibility ) => null;

    #endregion

    #region VerticalAlignment

    public string VerticalAlignment( VerticalAlignment verticalAlignment ) => null;

    #endregion

    #region Shadow

    public string Shadow( Shadow shadow ) => null;

    #endregion

    #region Overflow

    public string Overflow( OverflowType overflowType, OverflowType secondOverflowType ) => null;

    #endregion

    #region Position

    public string Position( PositionType positionType, PositionEdgeType edgeType, int edgeOffset, PositionTranslateType translateType ) => null;

    public string Position( PositionType positionType, IEnumerable<(PositionEdgeType edgeType, int edgeOffset)> edges, PositionTranslateType translateType ) => null;

    #endregion

    #region Custom

    public string Casing( CharacterCasing characterCasing ) => null;

    #endregion

    #region Elements

    public string UnorderedList() => null;

    public string UnorderedListUnstyled( bool unstyled ) => null;

    public string OrderedList() => null;

    public string OrderedListUnstyled( bool unstyled ) => null;

    public string OrderedListType( OrderedListType orderedListType ) => null;

    public string DescriptionList() => null;

    public string DescriptionListTerm() => null;

    public string DescriptionListDefinition() => null;

    #endregion

    #region Enums

    public string ToSize( Size size ) => null;

    public string ToBreakpoint( Breakpoint breakpoint ) => null;

    public string ToColor( Color color ) => null;

    public string ToBackground( Background background ) => null;

    public string ToTextColor( TextColor textColor ) => null;

    public string ToThemeContrast( ThemeContrast themeContrast ) => null;

    public string ToFloat( Float @float ) => null;

    public string ToBorderRadius( BorderRadius borderRadius ) => null;

    public string ToSpacing( Spacing spacing ) => null;

    public string ToSide( Side side ) => null;

    public string ToAlignment( Alignment alignment ) => null;

    public string ToTextAlignment( TextAlignment textAlignment ) => null;

    public string ToTextTransform( TextTransform textTransform ) => null;

    public string ToTextWeight( TextWeight textWeight ) => null;

    public string ToTextOverflow( TextOverflow textOverflow ) => null;

    public string ToTextSizeType( TextSizeType textSizeType ) => null;

    public string ToColumnWidth( ColumnWidth columnWidth ) => null;

    public string ToDisplayType( DisplayType displayType ) => null;

    public string ToDisplayDirection( DisplayDirection displayDirection ) => null;

    public string ToModalSize( ModalSize modalSize ) => null;

    public string ToSpacingSize( SpacingSize spacingSize ) => null;

    public string ToGapSize( GapSize gapSize ) => null;

    public string ToJustifyContent( JustifyContent justifyContent ) => null;

    public string ToJustifyContent( FlexJustifyContent justifyContent ) => null;

    public string ToScreenreader( Screenreader screenreader ) => null;

    public string ToHeadingSize( HeadingSize headingSize ) => null;

    public string ToDisplayHeadingSize( DisplayHeadingSize displayHeadingSize ) => null;

    public string ToJumbotronTitleSize( JumbotronTitleSize jumbotronTitleSize ) => null;

    public string ToPlacement( Placement placement ) => null;

    public string ToTooltipPlacement( TooltipPlacement tooltipPlacement ) => null;

    public string ToFigureSize( FigureSize figureSize ) => null;

    public string ToCharacterCasing( CharacterCasing characterCasing ) => null;

    public string ToBarMode( BarMode mode ) => null;

    public string ToBarCollapsedMode( BarCollapseMode collapseMode ) => null;

    public string ToDirection( FlexDirection direction ) => null;

    public string ToAlignItems( FlexAlignItems alignItems ) => null;

    public string ToAlignSelf( FlexAlignSelf alignSelf ) => null;

    public string ToAlignContent( FlexAlignContent alignContent ) => null;

    public string ToGrowShrink( FlexGrowShrink growShrink ) => null;

    public string ToGrowShrinkSize( FlexGrowShrinkSize growShrinkSize ) => null;

    public string ToWrap( FlexWrap wrap ) => null;

    public string ToOrder( FlexOrder order ) => null;

    public string ToSizingType( SizingType sizingType ) => null;

    public string ToSizingSize( SizingSize sizingSize ) => null;

    public string ToVerticalAlignment( VerticalAlignment verticalAlignment ) => null;

    public string ToShadow( Shadow shadow ) => null;

    public string ToOrderedListType( OrderedListType orderedListType ) => null;

    public string ToPositionType( PositionType positionType ) => null;

    public string ToPositionEdgeType( PositionEdgeType positionEdgeType ) => null;

    public string ToPositionTranslateType( PositionTranslateType positionTranslateType ) => null;

    public string ToTableColumnFixedPosition( TableColumnFixedPosition tableColumnFixedPosition ) => null;

    #endregion

    #region Extensions

    #region Autocomplete

    public string AutocompleteItemFocus( bool focus ) => null;

    #endregion

    #endregion

    #region Properties

    public bool UseCustomInputStyles { get; set; } = false;

    public string Provider => "EmptyClassProvider";

    #endregion
}