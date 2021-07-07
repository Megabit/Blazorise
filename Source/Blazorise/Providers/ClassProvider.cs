﻿#region Using directives
using System.Collections.Generic;
using System.Linq;
#endregion

namespace Blazorise
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public abstract class ClassProvider : IClassProvider
    {
        #region TextEdit

        public abstract string TextEdit( bool plaintext );

        public abstract string TextEditSize( Size size );

        public abstract string TextEditColor( Color color );

        public abstract string TextEditValidation( ValidationStatus validationStatus );

        #endregion

        #region MemoEdit

        public abstract string MemoEdit();

        public abstract string MemoEditSize( Size size );

        public abstract string MemoEditValidation( ValidationStatus validationStatus );

        #endregion

        #region Select

        public abstract string Select();

        public abstract string SelectMultiple();

        public abstract string SelectSize( Size size );

        public abstract string SelectValidation( ValidationStatus validationStatus );

        #endregion

        #region NumericEdit

        public abstract string NumericEdit( bool plaintext );

        public abstract string NumericEditSize( Size size );

        public abstract string NumericEditColor( Color color );

        public abstract string NumericEditValidation( ValidationStatus validationStatus );

        #endregion

        #region DateEdit

        public abstract string DateEdit( bool plaintext );

        public abstract string DateEditSize( Size size );

        public abstract string DateEditColor( Color color );

        public abstract string DateEditValidation( ValidationStatus validationStatus );

        #endregion

        #region TimeEdit

        public abstract string TimeEdit( bool plaintext );

        public abstract string TimeEditSize( Size size );

        public abstract string TimeEditColor( Color color );

        public abstract string TimeEditValidation( ValidationStatus validationStatus );

        #endregion

        #region ColorEdit

        public abstract string ColorEdit();

        public abstract string ColorEditSize( Size size );

        #endregion

        #region Check

        public abstract string Check();

        public abstract string CheckSize( Size size );

        public abstract string CheckInline();

        public abstract string CheckCursor( Cursor cursor );

        public abstract string CheckValidation( ValidationStatus validationStatus );

        #endregion

        #region RadioGroup

        public abstract string RadioGroup( bool buttons, Orientation orientation );

        public abstract string RadioGroupSize( bool buttons, Orientation orientation, Size size );

        public abstract string RadioGroupValidation( ValidationStatus validationStatus );

        #endregion

        #region Radio

        public abstract string Radio( bool button );

        public abstract string RadioSize( bool button, Size size );

        public abstract string RadioInline( bool inline );

        public abstract string RadioCursor( Cursor cursor );

        public abstract string RadioValidation( ValidationStatus validationStatus );

        #endregion

        #region Switch

        public abstract string Switch();

        public abstract string SwitchColor( Color color );

        public abstract string SwitchSize( Size size );

        public abstract string SwitchChecked( bool @checked );

        public abstract string SwitchCursor( Cursor cursor );

        public abstract string SwitchValidation( ValidationStatus validationStatus );

        #endregion

        #region FileEdit

        public abstract string FileEdit();

        public abstract string FileEditSize( Size size );

        public abstract string FileEditValidation( ValidationStatus validationStatus );

        #endregion

        #region Slider

        public abstract string Slider();

        public abstract string SliderColor( Color color );

        public abstract string SliderValidation( ValidationStatus validationStatus );

        #endregion

        #region Rating

        public abstract string Rating();

        public abstract string RatingDisabled( bool disabled );

        public abstract string RatingReadonly( bool @readonly );

        public abstract string RatingItem();

        public abstract string RatingItemColor( Color color );

        public abstract string RatingItemSelected( bool selected );

        public abstract string RatingItemHovered( bool hover );

        #endregion

        #region Label

        public abstract string Label();

        public abstract string LabelType( LabelType labelType );

        public abstract string LabelCursor( Cursor cursor );

        #endregion

        #region Help

        public abstract string Help();

        #endregion

        #region Validation

        public abstract string ValidationSuccess();

        public abstract string ValidationSuccessTooltip();

        public abstract string ValidationError();

        public abstract string ValidationErrorTooltip();

        public abstract string ValidationNone();

        public abstract string ValidationSummary();

        public abstract string ValidationSummaryError();

        #endregion

        #region Fields

        public abstract string Fields();

        public abstract string FieldsBody();

        public abstract string FieldsColumn();

        #endregion

        #region Field

        public abstract string Field();

        public abstract string FieldHorizontal();

        public abstract string FieldColumn();

        public abstract string FieldJustifyContent( JustifyContent justifyContent );

        public abstract string FieldValidation( ValidationStatus validationStatus );

        #endregion

        #region FieldLabel

        public abstract string FieldLabel();

        public abstract string FieldLabelHorizontal();

        #endregion

        #region FieldBody

        public abstract string FieldBody();

        #endregion

        #region FieldHelp

        public abstract string FieldHelp();

        #endregion

        #region Control

        public abstract string ControlCheck();

        public abstract string ControlRadio();

        public abstract string ControlSwitch();

        public abstract string ControlFile();

        public abstract string ControlText();

        #endregion

        #region Addons

        public abstract string Addons();

        public abstract string AddonsSize( Size size );

        public abstract string AddonsHasButton( bool hasButton );

        public abstract string Addon( AddonType addonType );

        public abstract string AddonLabel();

        //public abstract string AddonContainer();

        #endregion

        #region Inline

        public abstract string Inline();

        #endregion

        #region Button

        public abstract string Button();

        public abstract string ButtonColor( Color color );

        public abstract string ButtonOutline( Color color );

        public abstract string ButtonSize( Size size );

        public abstract string ButtonBlock();

        public abstract string ButtonActive();

        public abstract string ButtonDisabled();

        public abstract string ButtonLoading();

        #endregion

        #region Buttons

        public abstract string Buttons( ButtonsRole role, Orientation orientation );

        public abstract string ButtonsSize( Size size );

        #endregion

        #region CloseButton

        public abstract string CloseButton();

        #endregion

        #region Dropdown

        public abstract string Dropdown();

        public abstract string DropdownGroup();

        public abstract string DropdownShow();

        public abstract string DropdownRight();

        public abstract string DropdownItem();

        public abstract string DropdownItemActive( bool active );

        public abstract string DropdownItemDisabled( bool disabled );

        public abstract string DropdownDivider();

        public abstract string DropdownHeader();

        public abstract string DropdownMenu();

        //public abstract string DropdownMenuBody();

        public abstract string DropdownMenuVisible( bool visible );

        public abstract string DropdownMenuRight();

        public abstract string DropdownToggle();

        public abstract string DropdownToggleColor( Color color );

        public abstract string DropdownToggleOutline( Color color );

        public abstract string DropdownToggleSize( Size size );

        public abstract string DropdownToggleSplit();

        public abstract string DropdownToggleIcon( bool visible );

        public abstract string DropdownDirection( Direction direction );

        public abstract string DropdownTableResponsive();

        #endregion

        #region Tabs

        public abstract string Tabs( bool pills );

        public abstract string TabsCards();

        public abstract string TabsFullWidth();

        public abstract string TabsJustified();

        public abstract string TabsVertical();

        public abstract string TabItem();

        public abstract string TabItemActive( bool active );

        public abstract string TabItemDisabled( bool disabled );

        public abstract string TabLink();

        public abstract string TabLinkActive( bool active );

        public abstract string TabLinkDisabled( bool disabled );

        public abstract string TabsContent();

        public abstract string TabPanel();

        public abstract string TabPanelActive( bool active );

        #endregion

        #region Steps

        public abstract string Steps();

        public abstract string StepItem();

        public abstract string StepItemActive( bool active );

        public abstract string StepItemCompleted( bool completed );

        public abstract string StepItemColor( Color color );

        public abstract string StepItemMarker();

        public abstract string StepItemDescription();

        public abstract string StepsContent();

        public abstract string StepPanel();

        public abstract string StepPanelActive( bool active );

        #endregion

        #region Carousel

        public abstract string Carousel();

        public abstract string CarouselSlides();

        public abstract string CarouselSlide();

        public abstract string CarouselSlideActive( bool active );

        public abstract string CarouselSlideSlidingLeft( bool left );

        public abstract string CarouselSlideSlidingRight( bool right );

        public abstract string CarouselSlideSlidingPrev( bool previous );

        public abstract string CarouselSlideSlidingNext( bool next );

        public abstract string CarouselIndicators();

        public abstract string CarouselIndicator();

        public abstract string CarouselIndicatorActive( bool active );

        public abstract string CarouselFade( bool fade );

        public abstract string CarouselCaption();

        #endregion

        #region Jumbotron

        public abstract string Jumbotron();

        public abstract string JumbotronBackground( Background background );

        public abstract string JumbotronTitle( JumbotronTitleSize jumbotronTitleSize );

        public abstract string JumbotronSubtitle();

        #endregion

        #region Card

        public abstract string CardDeck();

        public abstract string CardGroup();

        public abstract string Card();

        public abstract string CardWhiteText();

        public abstract string CardActions();

        public abstract string CardBody();

        public abstract string CardFooter();

        public abstract string CardHeader();

        public abstract string CardImage();

        public abstract string CardTitle( bool insideHeader );

        public abstract string CardTitleSize( bool insideHeader, int? size );

        public abstract string CardSubtitle( bool insideHeader );

        public abstract string CardSubtitleSize( bool insideHeader, int size );

        public abstract string CardText();

        public abstract string CardLink();

        #endregion

        #region ListGroup

        public abstract string ListGroup();

        public abstract string ListGroupFlush();

        public abstract string ListGroupItem();

        public abstract string ListGroupItemSelectable();

        public abstract string ListGroupItemActive();

        public abstract string ListGroupItemDisabled();

        public abstract string ListGroupItemColor( Color color );

        #endregion

        #region Layout

        public virtual string Layout() => "b-layout";

        public virtual string LayoutHasSider() => "b-layout-has-sider";

        public virtual string LayoutContent() => "b-layout-content";

        public virtual string LayoutHeader() => "b-layout-header";

        public virtual string LayoutHeaderFixed() => "b-layout-header-fixed";

        public virtual string LayoutFooter() => "b-layout-footer";

        public virtual string LayoutFooterFixed() => "b-layout-footer-fixed";

        public virtual string LayoutSider() => "b-layout-sider";

        public virtual string LayoutSiderContent() => "b-layout-sider-content";

        public virtual string LayoutLoading() => "b-layout-loading";

        public virtual string LayoutRoot() => "b-layout-root";

        #endregion

        #region Container

        public abstract string Container();

        public abstract string ContainerFluid();

        #endregion

        #region Bar

        public abstract string Bar();

        public abstract string BarAlignment( Alignment alignment );

        public abstract string BarThemeContrast( ThemeContrast themeContrast );

        public abstract string BarBreakpoint( Breakpoint breakpoint );

        public abstract string BarMode( BarMode mode );

        public abstract string BarItem( BarMode mode );

        public abstract string BarItemActive( BarMode mode );

        public abstract string BarItemDisabled( BarMode mode );

        public abstract string BarItemHasDropdown( BarMode mode );

        public abstract string BarItemHasDropdownShow( BarMode mode );

        public abstract string BarLink( BarMode mode );

        public abstract string BarLinkDisabled( BarMode mode );

        //public abstract string BarCollapse();

        public abstract string BarBrand( BarMode mode );

        public abstract string BarToggler( BarMode mode, BarTogglerMode togglerMode );

        public abstract string BarTogglerCollapsed( BarMode mode, BarTogglerMode togglerMode, bool isShow );

        public abstract string BarMenu( BarMode mode );

        public abstract string BarMenuShow( BarMode mode );

        public abstract string BarStart( BarMode mode );

        public abstract string BarEnd( BarMode mode );

        //public abstract string BarHasDropdown();

        public abstract string BarDropdown( BarMode mode );

        public abstract string BarDropdownShow( BarMode mode );

        public abstract string BarDropdownToggle( BarMode mode );

        public abstract string BarDropdownItem( BarMode mode );

        public abstract string BarDropdownDivider( BarMode mode );

        public abstract string BarTogglerIcon( BarMode mode );

        public abstract string BarDropdownMenu( BarMode mode );

        public abstract string BarDropdownMenuVisible( BarMode mode, bool visible );

        public abstract string BarDropdownMenuRight( BarMode mode );

        public abstract string BarDropdownMenuContainer( BarMode mode );

        public abstract string BarCollapsed( BarMode mode );

        public abstract string BarLabel();

        #endregion

        #region Accordion

        public abstract string Accordion();

        #endregion

        #region Collapse

        public abstract string Collapse();

        public abstract string CollapseActive( bool active );

        public abstract string CollapseHeader();

        public abstract string CollapseBody();

        public abstract string CollapseBodyActive( bool active );

        public abstract string CollapseBodyContent();

        #endregion

        #region Row

        public abstract string Row();

        public abstract string RowColumns( RowColumnsSize rowColumnsSize, RowColumnsDefinition rowColumnsDefinition );

        public abstract string RowNoGutters();

        #endregion

        #region Column

        public abstract string Column( bool hasSizes );

        public abstract string Column( ColumnWidth columnWidth, Breakpoint breakpoint, bool offset );

        public virtual string Column( ColumnWidth columnWidth, IEnumerable<(Breakpoint breakpoint, bool offset)> rules ) =>
            string.Join( " ", rules.Select( r => Column( columnWidth, r.breakpoint, r.offset ) ) );

        #endregion

        #region Display

        public abstract string Display( DisplayType displayType, DisplayDefinition displayDefinition );

        public virtual string Display( DisplayType displayType, IEnumerable<DisplayDefinition> displayDefinitions )
            => string.Join( " ", displayDefinitions.Select( displayDefinition => Display( displayType, displayDefinition ) ) );

        #endregion

        #region Alert

        public abstract string Alert();

        public abstract string AlertColor( Color color );

        public abstract string AlertDismisable();

        public abstract string AlertFade();

        public abstract string AlertShow();

        public abstract string AlertHasMessage();

        public abstract string AlertHasDescription();

        public abstract string AlertMessage();

        public abstract string AlertDescription();

        #endregion

        #region Modal

        public abstract string Modal();

        public abstract string ModalFade();

        public abstract string ModalVisible( bool visible );

        public abstract string ModalBackdrop();

        public abstract string ModalBackdropFade();

        public abstract string ModalBackdropVisible( bool visible );

        public abstract string ModalContent( bool dialog );

        public abstract string ModalContentSize( ModalSize modalSize );

        public abstract string ModalContentCentered( bool centered );

        public abstract string ModalContentScrollable( bool scrollable );

        public abstract string ModalBody();

        public abstract string ModalHeader();

        public abstract string ModalFooter();

        public abstract string ModalTitle();

        #endregion

        #region Pagination

        public abstract string Pagination();

        public abstract string PaginationSize( Size size );

        public abstract string PaginationItem();

        public abstract string PaginationItemActive();

        public abstract string PaginationItemDisabled();

        public abstract string PaginationLink();

        public abstract string PaginationLinkActive();

        public abstract string PaginationLinkDisabled();

        #endregion

        #region Progress

        public abstract string Progress();

        public abstract string ProgressSize( Size size );

        public abstract string ProgressColor( Color color );

        public abstract string ProgressStriped();

        public abstract string ProgressAnimated();

        public abstract string ProgressWidth( int width );

        public abstract string ProgressBar();

        public abstract string ProgressBarSize( Size size );

        public abstract string ProgressBarColor( Color color );

        public abstract string ProgressBarStriped();

        public abstract string ProgressBarAnimated();

        public abstract string ProgressBarWidth( int width );

        #endregion

        #region Chart

        public abstract string Chart();

        #endregion

        #region Colors

        public abstract string BackgroundColor( Background background );

        #endregion

        #region Title

        public abstract string Title();

        public abstract string TitleSize( int size );

        #endregion

        #region Table

        public abstract string Table();

        public abstract string TableFullWidth();

        public abstract string TableStriped();

        public abstract string TableHoverable();

        public abstract string TableBordered();

        public abstract string TableNarrow();

        public abstract string TableBorderless();

        public abstract string TableHeader();

        public abstract string TableHeaderThemeContrast( ThemeContrast themeContrast );

        public abstract string TableHeaderCell();

        public abstract string TableFooter();

        public abstract string TableBody();

        public abstract string TableRow();

        public abstract string TableRowColor( Color color );

        public abstract string TableRowHoverCursor();

        public abstract string TableRowIsSelected();

        public abstract string TableRowHeader();

        public abstract string TableRowCell();

        public abstract string TableRowCellColor( Color color );

        public abstract string TableResponsive();

        public abstract string TableFixedHeader();

        #endregion

        #region Badge

        public abstract string Badge();

        public abstract string BadgeColor( Color color );

        public abstract string BadgePill();

        public abstract string BadgeClose();

        #endregion

        #region Media

        public abstract string Media();

        public abstract string MediaLeft();

        public abstract string MediaRight();

        public abstract string MediaBody();

        #endregion

        #region Text

        public abstract string TextColor( TextColor textColor );

        public abstract string TextAlignment( TextAlignment textAlignment );

        public abstract string TextTransform( TextTransform textTransform );

        public abstract string TextWeight( TextWeight textWeight );

        public abstract string TextOverflow( TextOverflow textOverflow );

        public abstract string TextItalic();

        #endregion

        #region Heading

        public abstract string HeadingSize( HeadingSize headingSize );

        #endregion

        #region DisplayHeading

        public abstract string DisplayHeadingSize( DisplayHeadingSize displayHeadingSize );

        #endregion

        #region Paragraph

        public abstract string Paragraph();

        public abstract string ParagraphColor( TextColor textColor );

        #endregion

        #region Figure

        public abstract string Figure();

        public abstract string FigureSize( FigureSize figureSize );

        public abstract string FigureImage();

        public abstract string FigureImageRounded();

        public abstract string FigureCaption();

        #endregion

        #region Image

        public abstract string Image();

        public abstract string ImageFluid( bool fluid );

        #endregion

        #region Breadcrumb

        public abstract string Breadcrumb();

        public abstract string BreadcrumbItem();

        public abstract string BreadcrumbItemActive();

        public abstract string BreadcrumbLink();

        #endregion

        #region Tooltip

        public abstract string Tooltip();

        public abstract string TooltipPlacement( TooltipPlacement tooltipPlacement );

        public abstract string TooltipMultiline();

        public abstract string TooltipAlwaysActive();

        public abstract string TooltipFade();

        public abstract string TooltipInline();

        #endregion

        #region Divider

        public abstract string Divider();

        public abstract string DividerType( DividerType dividerType );

        #endregion

        #region States

        public abstract string Show();

        public abstract string Fade();

        public abstract string Active();

        public abstract string Disabled();

        public abstract string Collapsed();

        #endregion

        #region Layout

        public abstract string Spacing( Spacing spacing, SpacingSize spacingSize, Side side, Breakpoint breakpoint );

        public abstract string Spacing( Spacing spacing, SpacingSize spacingSize, IEnumerable<(Side side, Breakpoint breakpoint)> rules );

        #endregion

        #region Borders

        public abstract string Border( BorderSize borderSize, BorderSide borderSide, BorderColor borderColor );

        public abstract string Border( BorderSize borderSize, IEnumerable<(BorderSide borderSide, BorderColor borderColor)> rules );

        public virtual string BorderRadius( BorderRadius borderRadius )
            => ToBorderRadius( borderRadius );

        #endregion

        #region Flex

        public abstract string Flex( FlexType flexType );

        public abstract string Flex( FlexDefinition flexDefinition );

        public abstract string Flex( FlexType flexType, IEnumerable<FlexDefinition> flexDefinitions );

        public abstract string FlexAlignment( Alignment alignment );

        #endregion

        #region Sizing

        public abstract string Sizing( SizingType sizingType, SizingSize sizingSize, SizingDefinition sizingDefinition );

        #endregion

        #region Float

        public virtual string Float( Float @float ) => $"float-{ToFloat( @float )}";

        #endregion

        #region Visibility

        public abstract string Visibility( Visibility visibility );

        #endregion

        #region VerticalAlignment

        public abstract string VerticalAlignment( VerticalAlignment verticalAlignment );

        #endregion

        #region Shadow

        public abstract string Shadow( Shadow shadow );

        #endregion

        #region Overflow

        public abstract string Overflow( Overflow overflow );

        #endregion

        #region Custom

        public virtual string Casing( CharacterCasing characterCasing ) => $"b-character-casing-{ToCharacterCasing( characterCasing )}";

        #endregion

        #region Enums

        public virtual string ToSize( Size size )
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

        public virtual string ToBreakpoint( Breakpoint breakpoint )
        {
            return breakpoint switch
            {
                Blazorise.Breakpoint.Mobile => "xs",
                Blazorise.Breakpoint.Tablet => "sm",
                Blazorise.Breakpoint.Desktop => "md",
                Blazorise.Breakpoint.Widescreen => "lg",
                Blazorise.Breakpoint.FullHD => "xl",
                _ => null,
            };
        }

        public virtual string ToColor( Color color )
        {
            return color switch
            {
                Blazorise.Color.Primary => "primary",
                Blazorise.Color.Secondary => "secondary",
                Blazorise.Color.Success => "success",
                Blazorise.Color.Danger => "danger",
                Blazorise.Color.Warning => "warning",
                Blazorise.Color.Info => "info",
                Blazorise.Color.Light => "light",
                Blazorise.Color.Dark => "dark",
                Blazorise.Color.Link => "link",
                _ => null,
            };
        }

        public virtual string ToBackground( Background background )
        {
            return background switch
            {
                Blazorise.Background.Primary => "primary",
                Blazorise.Background.Secondary => "secondary",
                Blazorise.Background.Success => "success",
                Blazorise.Background.Danger => "danger",
                Blazorise.Background.Warning => "warning",
                Blazorise.Background.Info => "info",
                Blazorise.Background.Light => "light",
                Blazorise.Background.Dark => "dark",
                Blazorise.Background.White => "white",
                Blazorise.Background.Transparent => "transparent",
                _ => null,
            };
        }

        public virtual string ToTextColor( TextColor textColor )
        {
            return textColor switch
            {
                Blazorise.TextColor.Primary => "primary",
                Blazorise.TextColor.Secondary => "secondary",
                Blazorise.TextColor.Success => "success",
                Blazorise.TextColor.Danger => "danger",
                Blazorise.TextColor.Warning => "warning",
                Blazorise.TextColor.Info => "info",
                Blazorise.TextColor.Light => "light",
                Blazorise.TextColor.Dark => "dark",
                Blazorise.TextColor.Body => "body",
                Blazorise.TextColor.Muted => "muted",
                Blazorise.TextColor.White => "white",
                Blazorise.TextColor.Black50 => "black-50",
                Blazorise.TextColor.White50 => "white-50",
                _ => null,
            };
        }

        public virtual string ToThemeContrast( ThemeContrast themeContrast )
        {
            return themeContrast switch
            {
                Blazorise.ThemeContrast.Light => "light",
                Blazorise.ThemeContrast.Dark => "dark",
                _ => null,
            };
        }

        public virtual string ToFloat( Float @float )
        {
            return @float switch
            {
                Blazorise.Float.Left => "left",
                Blazorise.Float.Right => "right",
                _ => null,
            };
        }

        public virtual string ToBorderRadius( BorderRadius borderRadius )
        {
            return borderRadius switch
            {
                Blazorise.BorderRadius.Rounded => "rounded",
                Blazorise.BorderRadius.RoundedTop => "rounded-top",
                Blazorise.BorderRadius.RoundedRight => "rounded-right",
                Blazorise.BorderRadius.RoundedBottom => "rounded-bottom",
                Blazorise.BorderRadius.RoundedLeft => "rounded-left",
                Blazorise.BorderRadius.RoundedCircle => "rounded-circle",
                Blazorise.BorderRadius.RoundedPill => "rounded-pill",
                Blazorise.BorderRadius.RoundedZero => "rounded-0",
                _ => null,
            };
        }

        public virtual string ToSpacing( Spacing spacing )
        {
            return spacing switch
            {
                Blazorise.Spacing.Margin => "m",
                Blazorise.Spacing.Padding => "p",
                _ => null,
            };
        }

        public virtual string ToSide( Side side )
        {
            return side switch
            {
                Blazorise.Side.Top => "t",
                Blazorise.Side.Bottom => "b",
                Blazorise.Side.Left => "l",
                Blazorise.Side.Right => "r",
                Blazorise.Side.X => "x",
                Blazorise.Side.Y => "y",
                _ => null,
            };
        }

        public virtual string ToAlignment( Alignment alignment )
        {
            return alignment switch
            {
                Blazorise.Alignment.Start => "start",
                Blazorise.Alignment.Center => "center",
                Blazorise.Alignment.End => "end",
                _ => null,
            };
        }

        public virtual string ToTextAlignment( TextAlignment textAlignment )
        {
            return textAlignment switch
            {
                Blazorise.TextAlignment.Left => "left",
                Blazorise.TextAlignment.Center => "center",
                Blazorise.TextAlignment.Right => "right",
                Blazorise.TextAlignment.Justified => "justify",
                _ => null,
            };
        }

        public virtual string ToTextTransform( TextTransform textTransform )
        {
            return textTransform switch
            {
                Blazorise.TextTransform.Lowercase => "lowercase",
                Blazorise.TextTransform.Uppercase => "uppercase",
                Blazorise.TextTransform.Capitalize => "capitalize",
                _ => null,
            };
        }

        public virtual string ToTextWeight( TextWeight textWeight )
        {
            return textWeight switch
            {
                Blazorise.TextWeight.Normal => "normal",
                Blazorise.TextWeight.Bold => "bold",
                Blazorise.TextWeight.Light => "light",
                _ => null,
            };
        }

        public virtual string ToTextOverflow( TextOverflow textOverflow )
        {
            return textOverflow switch
            {
                Blazorise.TextOverflow.Wrap => "wrap",
                Blazorise.TextOverflow.NoWrap => "nowrap",
                Blazorise.TextOverflow.Truncate => "truncate",
                _ => null,
            };
        }

        public virtual string ToColumnWidth( ColumnWidth columnWidth )
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

        public virtual string ToRowColumnsSize( RowColumnsSize rowColumnsSize )
        {
            return rowColumnsSize switch
            {
                Blazorise.RowColumnsSize.Are1 => "1",
                Blazorise.RowColumnsSize.Are2 => "2",
                Blazorise.RowColumnsSize.Are3 => "3",
                Blazorise.RowColumnsSize.Are4 => "4",
                Blazorise.RowColumnsSize.Are5 => "5",
                Blazorise.RowColumnsSize.Are6 => "6",
                _ => null,
            };
        }

        public virtual string ToFlexType( FlexType flexType )
        {
            return flexType switch
            {
                Blazorise.FlexType.Flex => "flex",
                Blazorise.FlexType.InlineFlex => "inline-flex",
                _ => null,
            };
        }

        public virtual string ToDisplayType( DisplayType displayType )
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

        public virtual string ToDisplayDirection( DisplayDirection displayDirection )
        {
            return displayDirection switch
            {
                DisplayDirection.Row => "row",
                DisplayDirection.Column => "column",
                DisplayDirection.ReverseRow => "row-reverse",
                DisplayDirection.ReverseColumn => "column-reverse",
                _ => null,
            };
        }

        public virtual string ToModalSize( ModalSize modalSize )
        {
            return modalSize switch
            {
                Blazorise.ModalSize.Small => "sm",
                Blazorise.ModalSize.Large => "lg",
                Blazorise.ModalSize.ExtraLarge => "xl",
                _ => null,
            };
        }

        public virtual string ToSpacingSize( SpacingSize spacingSize )
        {
            return spacingSize switch
            {
                Blazorise.SpacingSize.Is0 => "0",
                Blazorise.SpacingSize.Is1 => "1",
                Blazorise.SpacingSize.Is2 => "2",
                Blazorise.SpacingSize.Is3 => "3",
                Blazorise.SpacingSize.Is4 => "4",
                Blazorise.SpacingSize.Is5 => "5",
                Blazorise.SpacingSize.IsAuto => "auto",
                _ => null,
            };
        }

        public virtual string ToJustifyContent( JustifyContent justifyContent )
        {
            return justifyContent switch
            {
                Blazorise.JustifyContent.Start => "justify-content-start",
                Blazorise.JustifyContent.End => "justify-content-end",
                Blazorise.JustifyContent.Center => "justify-content-center",
                Blazorise.JustifyContent.Between => "justify-content-between",
                Blazorise.JustifyContent.Around => "justify-content-around",
                _ => null,
            };
        }

        public virtual string ToScreenreader( Screenreader screenreader )
        {
            return screenreader switch
            {
                Blazorise.Screenreader.Only => "sr-only",
                Blazorise.Screenreader.OnlyFocusable => "sr-only-focusable",
                _ => null,
            };
        }

        public virtual string ToHeadingSize( HeadingSize headingSize )
        {
            return headingSize switch
            {
                Blazorise.HeadingSize.Is1 => "1",
                Blazorise.HeadingSize.Is2 => "2",
                Blazorise.HeadingSize.Is3 => "3",
                Blazorise.HeadingSize.Is4 => "4",
                Blazorise.HeadingSize.Is5 => "5",
                Blazorise.HeadingSize.Is6 => "6",
                _ => null,
            };
        }

        public virtual string ToDisplayHeadingSize( DisplayHeadingSize displayHeadingSize )
        {
            return displayHeadingSize switch
            {
                Blazorise.DisplayHeadingSize.Is1 => "1",
                Blazorise.DisplayHeadingSize.Is2 => "2",
                Blazorise.DisplayHeadingSize.Is3 => "3",
                Blazorise.DisplayHeadingSize.Is4 => "4",
                _ => null,
            };
        }

        public virtual string ToJumbotronTitleSize( JumbotronTitleSize jumbotronTitleSize )
        {
            return jumbotronTitleSize switch
            {
                Blazorise.JumbotronTitleSize.Is1 => "1",
                Blazorise.JumbotronTitleSize.Is2 => "2",
                Blazorise.JumbotronTitleSize.Is3 => "3",
                Blazorise.JumbotronTitleSize.Is4 => "4",
                _ => null,
            };
        }

        public string ToPlacement( Placement placement )
        {
            return placement switch
            {
                Blazorise.Placement.Bottom => "bottom",
                Blazorise.Placement.Left => "left",
                Blazorise.Placement.Right => "right",
                _ => "top",
            };
        }

        public string ToTooltipPlacement( TooltipPlacement tooltipPlacement )
        {
            return tooltipPlacement switch
            {
                Blazorise.TooltipPlacement.Bottom => "bottom",
                Blazorise.TooltipPlacement.BottomStart => "bottom-start",
                Blazorise.TooltipPlacement.BottomEnd => "bottom-end",
                Blazorise.TooltipPlacement.Left => "left",
                Blazorise.TooltipPlacement.LeftStart => "left-start",
                Blazorise.TooltipPlacement.LeftEnd => "left-end",
                Blazorise.TooltipPlacement.Right => "right",
                Blazorise.TooltipPlacement.RightStart => "right-start",
                Blazorise.TooltipPlacement.RightEnd => "right-end",
                Blazorise.TooltipPlacement.TopStart => "top-start",
                Blazorise.TooltipPlacement.TopEnd => "top-end",
                _ => "top",
            };
        }

        public virtual string ToValidationStatus( ValidationStatus validationStatus )
        {
            return validationStatus switch
            {
                Blazorise.ValidationStatus.Success => "is-valid",
                Blazorise.ValidationStatus.Error => "is-invalid",
                _ => null,
            };
        }

        public virtual string ToCursor( Cursor cursor )
        {
            return cursor switch
            {
                Blazorise.Cursor.Pointer => "pointer",
                _ => null,
            };
        }

        public virtual string ToFigureSize( FigureSize figureSize )
        {
            return figureSize switch
            {
                Blazorise.FigureSize.Is16x16 => "16x16",
                Blazorise.FigureSize.Is24x24 => "24x24",
                Blazorise.FigureSize.Is32x32 => "32x32",
                Blazorise.FigureSize.Is48x48 => "48x48",
                Blazorise.FigureSize.Is64x64 => "64x64",
                Blazorise.FigureSize.Is96x96 => "96x96",
                Blazorise.FigureSize.Is128x128 => "128x128",
                Blazorise.FigureSize.Is256x256 => "256x256",
                Blazorise.FigureSize.Is512x512 => "512x512",
                _ => null,
            };
        }

        public virtual string ToCharacterCasing( CharacterCasing characterCasing )
        {
            return characterCasing switch
            {
                CharacterCasing.Upper => "upper",
                CharacterCasing.Lower => "lower",
                CharacterCasing.Title => "title",
                _ => null,
            };
        }

        public virtual string ToDividerType( DividerType dividerType )
        {
            return dividerType switch
            {
                Blazorise.DividerType.Dashed => "dashed",
                Blazorise.DividerType.Dotted => "dotted",
                Blazorise.DividerType.TextContent => "text",
                _ => "solid",
            };
        }

        public virtual string ToBarMode( BarMode mode )
        {
            return mode switch
            {
                Blazorise.BarMode.Horizontal => "horizontal",
                Blazorise.BarMode.VerticalPopout => "vertical-popout",
                Blazorise.BarMode.VerticalInline => "vertical-inline",
                Blazorise.BarMode.VerticalSmall => "vertical-small",
                _ => null,
            };
        }

        public virtual string ToBarCollapsedMode( BarCollapseMode collapseMode )
        {
            return collapseMode switch
            {
                BarCollapseMode.Hide => "hide",
                BarCollapseMode.Small => "small",
                _ => null,
            };
        }

        public virtual string ToBorderSide( BorderSide borderSide )
        {
            return borderSide switch
            {
                Blazorise.BorderSide.Bottom => "bottom",
                Blazorise.BorderSide.Left => "left",
                Blazorise.BorderSide.Right => "right",
                _ => "top",
            };
        }

        public virtual string ToBorderColor( BorderColor borderColor )
        {
            return borderColor switch
            {
                Blazorise.BorderColor.Primary => "primary",
                Blazorise.BorderColor.Secondary => "secondary",
                Blazorise.BorderColor.Success => "success",
                Blazorise.BorderColor.Danger => "danger",
                Blazorise.BorderColor.Warning => "warning",
                Blazorise.BorderColor.Info => "info",
                Blazorise.BorderColor.Light => "light",
                Blazorise.BorderColor.Dark => "dark",
                Blazorise.BorderColor.White => "white",
                _ => null,
            };
        }

        public virtual string ToDirection( FlexDirection direction )
        {
            return direction switch
            {
                Blazorise.FlexDirection.Row => "row",
                Blazorise.FlexDirection.ReverseRow => "row-reverse",
                Blazorise.FlexDirection.Column => "column",
                Blazorise.FlexDirection.ReverseColumn => "column-reverse",
                _ => null,
            };
        }

        public virtual string ToJustifyContent( FlexJustifyContent justifyContent )
        {
            return justifyContent switch
            {
                Blazorise.FlexJustifyContent.Start => "start",
                Blazorise.FlexJustifyContent.End => "end",
                Blazorise.FlexJustifyContent.Center => "center",
                Blazorise.FlexJustifyContent.Between => "between",
                Blazorise.FlexJustifyContent.Around => "around",
                _ => null,
            };
        }

        public virtual string ToAlignItems( FlexAlignItems alignItems )
        {
            return alignItems switch
            {
                Blazorise.FlexAlignItems.Start => "start",
                Blazorise.FlexAlignItems.End => "end",
                Blazorise.FlexAlignItems.Center => "center",
                Blazorise.FlexAlignItems.Baseline => "baseline",
                Blazorise.FlexAlignItems.Stretch => "stretch",
                _ => null,
            };
        }

        public virtual string ToAlignSelf( FlexAlignSelf alignSelf )
        {
            return alignSelf switch
            {
                Blazorise.FlexAlignSelf.Auto => "auto",
                Blazorise.FlexAlignSelf.Start => "start",
                Blazorise.FlexAlignSelf.End => "end",
                Blazorise.FlexAlignSelf.Center => "center",
                Blazorise.FlexAlignSelf.Baseline => "baseline",
                Blazorise.FlexAlignSelf.Stretch => "stretch",
                _ => null,
            };
        }

        public virtual string ToAlignContent( FlexAlignContent alignContent )
        {
            return alignContent switch
            {
                Blazorise.FlexAlignContent.Start => "start",
                Blazorise.FlexAlignContent.End => "end",
                Blazorise.FlexAlignContent.Center => "center",
                Blazorise.FlexAlignContent.Between => "between",
                Blazorise.FlexAlignContent.Around => "around",
                Blazorise.FlexAlignContent.Stretch => "stretch",
                _ => null,
            };
        }

        public virtual string ToGrowShrink( FlexGrowShrink growShrink )
        {
            return growShrink switch
            {
                Blazorise.FlexGrowShrink.Grow => "grow",
                Blazorise.FlexGrowShrink.Shrink => "shrink",
                _ => null,
            };
        }

        public virtual string ToGrowShrinkSize( FlexGrowShrinkSize growShrinkSize )
        {
            return growShrinkSize switch
            {
                Blazorise.FlexGrowShrinkSize.Is0 => "0",
                Blazorise.FlexGrowShrinkSize.Is1 => "1",
                _ => null,
            };
        }

        public virtual string ToWrap( FlexWrap wrap )
        {
            return wrap switch
            {
                Blazorise.FlexWrap.Wrap => "wrap",
                Blazorise.FlexWrap.ReverseWrap => "wrap-reverse",
                Blazorise.FlexWrap.NoWrap => "nowrap",
                _ => null,
            };
        }

        public virtual string ToOrder( FlexOrder order )
        {
            return order switch
            {
                Blazorise.FlexOrder.Is0 => "0",
                Blazorise.FlexOrder.Is1 => "1",
                Blazorise.FlexOrder.Is2 => "2",
                Blazorise.FlexOrder.Is3 => "3",
                Blazorise.FlexOrder.Is4 => "4",
                Blazorise.FlexOrder.Is5 => "5",
                Blazorise.FlexOrder.Is6 => "6",
                Blazorise.FlexOrder.Is7 => "7",
                Blazorise.FlexOrder.Is8 => "8",
                Blazorise.FlexOrder.Is9 => "9",
                Blazorise.FlexOrder.Is10 => "10",
                Blazorise.FlexOrder.Is11 => "11",
                Blazorise.FlexOrder.Is12 => "12",
                _ => null,
            };
        }

        public virtual string ToSizingType( SizingType sizingType )
        {
            return sizingType switch
            {
                Blazorise.SizingType.Width => "w",
                Blazorise.SizingType.Height => "h",
                _ => null,
            };
        }

        public virtual string ToSizingSize( SizingSize sizingSize )
        {
            return sizingSize switch
            {
                Blazorise.SizingSize.Is25 => "25",
                Blazorise.SizingSize.Is50 => "50",
                Blazorise.SizingSize.Is75 => "75",
                Blazorise.SizingSize.Is100 => "100",
                Blazorise.SizingSize.Auto => "auto",
                _ => null,
            };
        }

        public virtual string ToVerticalAlignment( VerticalAlignment verticalAlignment )
        {
            return verticalAlignment switch
            {
                Blazorise.VerticalAlignment.Baseline => "baseline",
                Blazorise.VerticalAlignment.Top => "top",
                Blazorise.VerticalAlignment.Middle => "middle",
                Blazorise.VerticalAlignment.Bottom => "bottom",
                Blazorise.VerticalAlignment.TextTop => "text-top",
                Blazorise.VerticalAlignment.TextBottom => "text-bottom",
                _ => null,
            };
        }

        public virtual string ToShadow( Shadow shadow )
        {
            return shadow switch
            {
                Blazorise.Shadow.Remove => "none",
                Blazorise.Shadow.Small => "sm",
                Blazorise.Shadow.Large => "lg",
                _ => null,
            };
        }

        public virtual string ToOverflow( Overflow overflow )
        {
            return overflow switch
            {
                Blazorise.Overflow.Visible => "visible",
                Blazorise.Overflow.Hidden => "hidden",
                Blazorise.Overflow.Scroll => "scroll",
                Blazorise.Overflow.Auto => "auto",
                _ => null,
            };
        }

        #endregion

        public abstract bool UseCustomInputStyles { get; set; }

        public abstract string Provider { get; }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
