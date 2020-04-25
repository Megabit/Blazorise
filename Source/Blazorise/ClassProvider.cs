#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
#endregion

namespace Blazorise
{
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

        public abstract string MemoEditValidation( ValidationStatus validationStatus );

        #endregion

        #region Select

        public abstract string Select();

        public abstract string SelectMultiple();

        public abstract string SelectSize( Size size );

        public abstract string SelectValidation( ValidationStatus validationStatus );

        #endregion

        #region DateEdit

        public abstract string DateEdit();

        public abstract string DateEditSize( Size size );

        public abstract string DateEditValidation( ValidationStatus validationStatus );

        #endregion

        #region TimeEdit

        public abstract string TimeEdit();

        public abstract string TimeEditSize( Size size );

        public abstract string TimeEditValidation( ValidationStatus validationStatus );

        #endregion

        #region ColorEdit

        public abstract string ColorEdit();

        #endregion

        #region Check

        public abstract string Check();

        public abstract string CheckInline();

        public abstract string CheckCursor( Cursor cursor );

        public abstract string CheckValidation( ValidationStatus validationStatus );

        #endregion

        #region RadioGroup

        public abstract string RadioGroup( bool buttons );

        public abstract string RadioGroupInline();

        #endregion

        #region Radio

        public abstract string Radio( bool button );

        public abstract string RadioInline();

        #endregion

        #region Switch

        public abstract string Switch();

        public abstract string SwitchChecked( bool @checked );

        public abstract string SwitchCursor( Cursor cursor );

        public abstract string SwitchValidation( ValidationStatus validationStatus );

        #endregion

        #region FileEdit

        public abstract string FileEdit();

        public abstract string FileEditValidation( ValidationStatus validationStatus );

        #endregion

        #region Slider

        public abstract string Slider();

        public abstract string SliderColor( Color color );

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

        public abstract string ButtonSize( ButtonSize buttonSize );

        public abstract string ButtonBlock();

        public abstract string ButtonActive();

        public abstract string ButtonLoading();

        #endregion

        #region Buttons

        //public abstract string Buttons();

        public abstract string ButtonsAddons();

        public abstract string ButtonsToolbar();

        public abstract string ButtonsSize( ButtonsSize buttonsSize );

        public abstract string ButtonsVertical();

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

        public abstract string DropdownItemActive();

        public abstract string DropdownDivider();

        public abstract string DropdownMenu();

        //public abstract string DropdownMenuBody();

        public abstract string DropdownMenuVisible( bool visible );

        public abstract string DropdownMenuRight();

        public abstract string DropdownToggle();

        public abstract string DropdownToggleColor( Color color );

        public abstract string DropdownToggleOutline( Color color );

        public abstract string DropdownToggleSize( ButtonSize buttonSize );

        public abstract string DropdownToggleSplit();

        public abstract string DropdownDirection( Direction direction );

        #endregion

        #region Tab

        public abstract string Tabs();

        public abstract string TabsCards();

        public abstract string TabsPills();

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

        #region Jumbotron

        public abstract string Jumbotron();

        public abstract string JumbotronBackground( Background background );

        public abstract string JumbotronTitle( JumbotronTitleSize jumbotronTitleSize );

        public abstract string JumbotronSubtitle();

        #endregion

        #region Card

        public abstract string CardGroup();

        public abstract string Card();

        public abstract string CardWhiteText();

        public abstract string CardBackground( Background background );

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

        public abstract string ListGroupItemActive();

        public abstract string ListGroupItemDisabled();

        #endregion

        #region Layout

        public virtual string Layout() => "b-layout";

        public virtual string LayoutHasSider() => "b-layout-has-sider";

        public virtual string LayoutContent() => "b-layout-content";

        public virtual string LayoutHeader() => "b-layout-header";

        public virtual string LayoutHeaderFixed() => "b-layout-header-fixed";

        public virtual string LayoutFooter() => "b-layout-footer";

        public virtual string LayoutSider() => "b-layout-sider";

        public virtual string LayoutSiderContent() => "b-layout-sider-content";

        #endregion

        #region Container

        public abstract string Container();

        public abstract string ContainerFluid();

        #endregion

        #region Bar

        public abstract string Bar();

        public abstract string BarBackground( Background background );

        public abstract string BarAlignment( Alignment alignment );

        public abstract string BarThemeContrast( ThemeContrast themeContrast );

        public abstract string BarBreakpoint( Breakpoint breakpoint );

        public abstract string BarItem();

        public abstract string BarItemActive();

        public abstract string BarItemDisabled();

        public abstract string BarItemHasDropdown();

        public abstract string BarItemHasDropdownShow();

        public abstract string BarLink();

        public abstract string BarLinkDisabled();

        //public abstract string BarCollapse();

        public abstract string BarBrand();

        public abstract string BarToggler();

        public abstract string BarTogglerCollapsed( bool isShow );

        public abstract string BarMenu();

        public abstract string BarMenuShow();

        public abstract string BarStart();

        public abstract string BarEnd();

        //public abstract string BarHasDropdown();

        public abstract string BarDropdown();

        public abstract string BarDropdownShow();

        public abstract string BarDropdownToggle();

        public abstract string BarDropdownItem();

        public abstract string BarTogglerIcon();

        public abstract string BarDropdownMenu();

        public abstract string BarDropdownMenuVisible( bool visible );

        public abstract string BarDropdownMenuRight();

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

        #endregion

        #region Column

        public abstract string Column();

        public abstract string Column( ColumnWidth columnWidth, Breakpoint breakpoint, bool offset );

        public virtual string Column( ColumnWidth columnWidth, IEnumerable<(Breakpoint breakpoint, bool offset)> rules ) =>
            string.Join( " ", rules.Select( r => Column( columnWidth, r.breakpoint, r.offset ) ) );

        #endregion

        #region Display

        public abstract string Display( DisplayType displayType, Breakpoint breakpoint, DisplayDirection direction );

        public virtual string Display( DisplayType displayType, IEnumerable<(Breakpoint breakpoint, DisplayDirection direction)> rules )
            => string.Join( " ", rules.Select( r => Display( displayType, r.breakpoint, r.direction ) ) );

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

        public abstract string ModalContentCentered();

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

        public abstract string ProgressBar();

        public abstract string ProgressBarColor( Background background );

        public abstract string ProgressBarStriped();

        public abstract string ProgressBarAnimated();

        public abstract string ProgressBarWidth( int width );

        #endregion

        #region Chart

        public abstract string Chart();

        #endregion

        #region Colors

        public abstract string BackgroundColor( Background color );

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

        public abstract string TableRowBackground( Background background );

        public abstract string TableRowTextColor( TextColor textColor );

        public abstract string TableRowHoverCursor();

        public abstract string TableRowIsSelected();

        public abstract string TableRowHeader();

        public abstract string TableRowCell();

        public abstract string TableRowCellColor( Color color );

        public abstract string TableRowCellBackground( Background background );

        public abstract string TableRowCellTextColor( TextColor textColor );

        public abstract string TableRowCellTextAlignment( TextAlignment textAlignment );

        public abstract string TableResponsive();

        #endregion

        #region Badge

        public abstract string Badge();

        public abstract string BadgeColor( Color color );

        public abstract string BadgePill();

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

        public abstract string TextItalic();

        #endregion

        #region Heading

        public abstract string HeadingSize( HeadingSize headingSize );

        public abstract string HeadingTextColor( TextColor textColor );

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

        #region Breadcrumb

        public abstract string Breadcrumb();

        public abstract string BreadcrumbItem();

        public abstract string BreadcrumbItemActive();

        public abstract string BreadcrumbLink();

        #endregion

        #region Tooltip

        public abstract string Tooltip();

        public abstract string TooltipPlacement( Placement placement );

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

        #region Flex

        public abstract string FlexAlignment( Alignment alignment );

        #endregion

        #region Custom

        public virtual string Casing( CharacterCasing characterCasing ) => $"b-character-casing-{ToCharacterCasing( characterCasing )}";

        #endregion

        #region Enums

        public virtual string ToSize( Size size )
        {
            switch ( size )
            {
                case Blazorise.Size.ExtraSmall:
                    return "xs";
                case Blazorise.Size.Small:
                    return "sm";
                case Blazorise.Size.Medium:
                    return "md";
                case Blazorise.Size.Large:
                    return "lg";
                case Blazorise.Size.ExtraLarge:
                    return "xl";
                default:
                    return null;
            }
        }

        public virtual string ToBreakpoint( Breakpoint breakpoint )
        {
            switch ( breakpoint )
            {
                case Blazorise.Breakpoint.Mobile:
                    return "xs";
                case Blazorise.Breakpoint.Tablet:
                    return "sm";
                case Blazorise.Breakpoint.Desktop:
                    return "md";
                case Blazorise.Breakpoint.Widescreen:
                    return "lg";
                case Blazorise.Breakpoint.FullHD:
                    return "xl";
                default:
                    return null;
            }
        }

        public virtual string ToColor( Color color )
        {
            switch ( color )
            {
                case Blazorise.Color.Primary:
                    return "primary";
                case Blazorise.Color.Secondary:
                    return "secondary";
                case Blazorise.Color.Success:
                    return "success";
                case Blazorise.Color.Danger:
                    return "danger";
                case Blazorise.Color.Warning:
                    return "warning";
                case Blazorise.Color.Info:
                    return "info";
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

        public virtual string ToBackground( Background color )
        {
            switch ( color )
            {
                case Blazorise.Background.Primary:
                    return "primary";
                case Blazorise.Background.Secondary:
                    return "secondary";
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

        public virtual string ToTextColor( TextColor textColor )
        {
            switch ( textColor )
            {
                case Blazorise.TextColor.Primary:
                    return "primary";
                case Blazorise.TextColor.Secondary:
                    return "secondary";
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
                case Blazorise.TextColor.Body:
                    return "body";
                case Blazorise.TextColor.Muted:
                    return "muted";
                case Blazorise.TextColor.White:
                    return "white";
                case Blazorise.TextColor.Black50:
                    return "black-50";
                case Blazorise.TextColor.White50:
                    return "white-50";
                default:
                    return null;
            }
        }

        public virtual string ToThemeContrast( ThemeContrast themeContrast )
        {
            switch ( themeContrast )
            {
                case Blazorise.ThemeContrast.Light:
                    return "light";
                case Blazorise.ThemeContrast.Dark:
                    return "dark";
                default:
                    return null;
            }
        }

        public virtual string ToFloat( Float @float )
        {
            switch ( @float )
            {
                case Blazorise.Float.Left:
                    return "float-left";
                case Blazorise.Float.Right:
                    return "float-right";
                default:
                    return null;
            }
        }

        public virtual string ToSpacing( Spacing spacing )
        {
            switch ( spacing )
            {
                case Blazorise.Spacing.Margin:
                    return "m";
                case Blazorise.Spacing.Padding:
                    return "p";
                default:
                    return null;
            }
        }

        public virtual string ToSide( Side side )
        {
            switch ( side )
            {
                case Blazorise.Side.Top:
                    return "t";
                case Blazorise.Side.Bottom:
                    return "b";
                case Blazorise.Side.Left:
                    return "l";
                case Blazorise.Side.Right:
                    return "r";
                case Blazorise.Side.X:
                    return "x";
                case Blazorise.Side.Y:
                    return "y";
                default:
                    return null;
            }
        }

        public virtual string ToAlignment( Alignment alignment )
        {
            switch ( alignment )
            {
                case Blazorise.Alignment.Start:
                    return "start";
                case Blazorise.Alignment.Center:
                    return "center";
                case Blazorise.Alignment.End:
                    return "end";
                default:
                    return null;
            }
        }

        public virtual string ToTextAlignment( TextAlignment textAlignment )
        {
            switch ( textAlignment )
            {
                case Blazorise.TextAlignment.Left:
                    return "left";
                case Blazorise.TextAlignment.Center:
                    return "center";
                case Blazorise.TextAlignment.Right:
                    return "right";
                case Blazorise.TextAlignment.Justified:
                    return "justify";
                default:
                    return null;
            }
        }

        public virtual string ToTextTransform( TextTransform textTransform )
        {
            switch ( textTransform )
            {
                case Blazorise.TextTransform.Lowercase:
                    return "lowercase";
                case Blazorise.TextTransform.Uppercase:
                    return "uppercase";
                case Blazorise.TextTransform.Capitalize:
                    return "capitalize";
                default:
                    return null;
            }
        }

        public virtual string ToTextWeight( TextWeight textWeight )
        {
            switch ( textWeight )
            {
                case Blazorise.TextWeight.Normal:
                    return "normal";
                case Blazorise.TextWeight.Bold:
                    return "bold";
                case Blazorise.TextWeight.Light:
                    return "light";
                default:
                    return null;
            }
        }

        public virtual string ToColumnWidth( ColumnWidth columnWidth )
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
                case Blazorise.ColumnWidth.Auto:
                    return "auto";
                default:
                    return null;
            }
        }

        public virtual string ToDisplayType( DisplayType displayType )
        {
            switch ( displayType )
            {
                case Blazorise.DisplayType.Block:
                    return "block";
                case Blazorise.DisplayType.Inline:
                    return "inline";
                case Blazorise.DisplayType.InlineBlock:
                    return "inline-block";
                case Blazorise.DisplayType.Flex:
                    return "flex";
                case Blazorise.DisplayType.InlineFlex:
                    return "inline-flex";
                case Blazorise.DisplayType.Table:
                    return "table";
                case Blazorise.DisplayType.TableRow:
                    return "table-row";
                case Blazorise.DisplayType.TableCell:
                    return "table-cell";
                default:
                    return null;
            }
        }

        public virtual string ToDisplayDirection( DisplayDirection displayDirection )
        {
            switch ( displayDirection )
            {
                case DisplayDirection.Row:
                    return "row";
                case DisplayDirection.Column:
                    return "column";
                case DisplayDirection.ReverseRow:
                    return "row-reverse";
                case DisplayDirection.ReverseColumn:
                    return "column-reverse";
                default:
                    return null;
            }
        }

        public virtual string ToModalSize( ModalSize modalSize )
        {
            switch ( modalSize )
            {
                case Blazorise.ModalSize.Small:
                    return "sm";
                case Blazorise.ModalSize.Large:
                    return "lg";
                case Blazorise.ModalSize.ExtraLarge:
                    return "xl";
                case Blazorise.ModalSize.Default:
                default:
                    return null;
            }
        }

        public virtual string ToSpacingSize( SpacingSize spacingSize )
        {
            switch ( spacingSize )
            {
                case Blazorise.SpacingSize.Is0:
                    return "0";
                case Blazorise.SpacingSize.Is1:
                    return "1";
                case Blazorise.SpacingSize.Is2:
                    return "2";
                case Blazorise.SpacingSize.Is3:
                    return "3";
                case Blazorise.SpacingSize.Is4:
                    return "4";
                case Blazorise.SpacingSize.Is5:
                    return "5";
                case Blazorise.SpacingSize.IsAuto:
                    return "auto";
                default:
                    return null;
            }
        }

        public virtual string ToJustifyContent( JustifyContent justifyContent )
        {
            switch ( justifyContent )
            {
                case Blazorise.JustifyContent.Start:
                    return "justify-content-start";
                case Blazorise.JustifyContent.End:
                    return "justify-content-end";
                case Blazorise.JustifyContent.Center:
                    return "justify-content-center";
                case Blazorise.JustifyContent.Between:
                    return "justify-content-between";
                case Blazorise.JustifyContent.Around:
                    return "justify-content-around";
                default:
                    return null;
            }
        }

        public virtual string ToScreenreader( Screenreader screenreader )
        {
            switch ( screenreader )
            {
                case Blazorise.Screenreader.Only:
                    return "sr-only";
                case Blazorise.Screenreader.OnlyFocusable:
                    return "sr-only-focusable";
                default:
                    return null;
            }
        }

        public virtual string ToHeadingSize( HeadingSize headingSize )
        {
            switch ( headingSize )
            {
                case Blazorise.HeadingSize.Is1:
                    return "1";
                case Blazorise.HeadingSize.Is2:
                    return "2";
                case Blazorise.HeadingSize.Is3:
                    return "3";
                case Blazorise.HeadingSize.Is4:
                    return "4";
                case Blazorise.HeadingSize.Is5:
                    return "5";
                case Blazorise.HeadingSize.Is6:
                    return "6";
                default:
                    return null;
            }
        }

        public virtual string ToDisplayHeadingSize( DisplayHeadingSize displayHeadingSize )
        {
            switch ( displayHeadingSize )
            {
                case Blazorise.DisplayHeadingSize.Is1:
                    return "1";
                case Blazorise.DisplayHeadingSize.Is2:
                    return "2";
                case Blazorise.DisplayHeadingSize.Is3:
                    return "3";
                case Blazorise.DisplayHeadingSize.Is4:
                    return "4";
                default:
                    return null;
            }
        }

        public virtual string ToJumbotronTitleSize( JumbotronTitleSize jumbotronTitleSize )
        {
            switch ( jumbotronTitleSize )
            {
                case Blazorise.JumbotronTitleSize.Is1:
                    return "1";
                case Blazorise.JumbotronTitleSize.Is2:
                    return "2";
                case Blazorise.JumbotronTitleSize.Is3:
                    return "3";
                case Blazorise.JumbotronTitleSize.Is4:
                    return "4";
                default:
                    return null;
            }
        }

        public string ToPlacement( Placement placement )
        {
            switch ( placement )
            {
                case Blazorise.Placement.Bottom:
                    return "bottom";
                case Blazorise.Placement.Left:
                    return "left";
                case Blazorise.Placement.Right:
                    return "right";
                case Blazorise.Placement.Top:
                default:
                    return "top";
            }
        }

        public virtual string ToValidationStatus( ValidationStatus validationStatus )
        {
            switch ( validationStatus )
            {
                case Blazorise.ValidationStatus.Success:
                    return "is-valid";
                case Blazorise.ValidationStatus.Error:
                    return "is-invalid";
                default:
                    return null;
            }
        }

        public virtual string ToCursor( Cursor cursor )
        {
            switch ( cursor )
            {
                case Blazorise.Cursor.Pointer:
                    return "pointer";
                default:
                    return null;
            }
        }

        public virtual string ToFigureSize( FigureSize figureSize )
        {
            switch ( figureSize )
            {
                case Blazorise.FigureSize.Is16x16:
                    return "16x16";
                case Blazorise.FigureSize.Is24x24:
                    return "24x24";
                case Blazorise.FigureSize.Is32x32:
                    return "32x32";
                case Blazorise.FigureSize.Is48x48:
                    return "48x48";
                case Blazorise.FigureSize.Is64x64:
                    return "64x64";
                case Blazorise.FigureSize.Is96x96:
                    return "96x96";
                case Blazorise.FigureSize.Is128x128:
                    return "128x128";
                case Blazorise.FigureSize.Is256x256:
                    return "256x256";
                case Blazorise.FigureSize.Is512x512:
                    return "512x512";
                default:
                    return null;
            }
        }

        public virtual string ToCharacterCasing( CharacterCasing characterCasing )
        {
            switch ( characterCasing )
            {
                case CharacterCasing.Upper:
                    return "upper";
                case CharacterCasing.Lower:
                    return "lower";
                case CharacterCasing.Title:
                    return "title";
                case CharacterCasing.Normal:
                default:
                    return null;
            }
        }

        public virtual string ToDividerType( DividerType dividerType )
        {
            switch ( dividerType )
            {
                case Blazorise.DividerType.Dashed:
                    return "dashed";
                case Blazorise.DividerType.Dotted:
                    return "dotted";
                case Blazorise.DividerType.TextContent:
                    return "text";
                case Blazorise.DividerType.Solid:
                default:
                    return "solid";
            }
        }

        #endregion

        public abstract bool UseCustomInputStyles { get; set; }

        public abstract string Provider { get; }
    }
}
