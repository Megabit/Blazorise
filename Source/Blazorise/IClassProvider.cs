#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Blazorise
{
    public interface IClassProvider
    {
        #region Text

        string Text( bool plaintext );

        string TextSize( Size size );

        string TextColor( Color color );

        #endregion

        #region Memo

        string Memo();

        #endregion

        #region Select

        string Select();

        string SelectSize( Size size );

        #endregion

        #region Date

        string Date();

        string DateSize( Size size );

        #endregion

        #region Check

        string Check();

        string CheckInline();

        #endregion

        #region Radio

        string Radio();

        #endregion

        #region File

        string File();

        #endregion

        #region Label

        string Label();

        string LabelCheck();

        string LabelFile();

        #endregion

        #region Help

        string Help();

        #endregion

        #region Fields

        string Fields();

        string FieldsBody();

        string FieldsColumn();

        #endregion

        #region Field

        string Field();

        string FieldHorizontal();

        string FieldColumn();

        string FieldJustifyContent( JustifyContent justifyContent );

        #endregion

        #region FieldLabel

        string FieldLabel();

        string FieldLabelHorizontal();

        #endregion

        #region FieldHelp

        string FieldHelp();

        #endregion

        #region Control

        string ControlCheck();

        string ControlRadio();

        string ControlFile();

        string ControlText();

        #endregion

        #region Addons

        string Addons();

        string Addon( AddonType addonType );

        string AddonLabel();

        string AddonContainer();

        #endregion

        #region Inline

        string Inline();

        #endregion

        #region Button

        string Button();

        string ButtonColor( Color color );

        string ButtonOutline( Color color );

        string ButtonSize( Size size );

        string ButtonBlock();

        string ButtonActive();

        #endregion

        #region Buttons

        //string Buttons();

        string ButtonsAddons();

        string ButtonsToolbar();

        string ButtonsSize( Size size );

        string ButtonsVertical();

        #endregion

        #region CloseButton

        string CloseButton();

        #endregion

        #region Dropdown

        string Dropdown();

        string DropdownGroup();

        string DropdownShow();

        string DropdownRight();

        string DropdownItem();

        string DropdownDivider();

        string DropdownMenu();

        string DropdownMenuBody();

        string DropdownMenuShow();

        string DropdownMenuRight();

        string DropdownToggle();

        string DropdownToggleSplit();

        string DropdownDirection( Direction direction );

        #endregion

        #region Tab

        string Tabs();

        string TabsCards();

        string TabsPills();

        string TabsFullWidth();

        string TabsJustified();

        string TabsVertical();

        string TabItem();

        string TabItemActive();

        string TabLink();

        string TabLinkActive();

        string TabContent();

        string TabPanel();

        string TabPanelActive();

        #endregion

        #region Card

        string CardGroup();

        string Card();

        string CardWhiteText();

        string CardBackground( Background background );

        string CardActions();

        string CardBody();

        string CardFooter();

        string CardHeader();

        string CardImage();

        string CardTitle();

        string CardSubtitle();

        string CardSubtitleSize( int size );

        string CardText();

        string CardLink();

        #endregion

        #region ListGroup

        string ListGroup();

        string ListGroupFlush();

        string ListGroupItem();

        string ListGroupItemActive();

        string ListGroupItemDisabled();

        #endregion

        #region Container

        string Container();

        string ContainerFluid();

        #endregion

        #region Panel

        string Panel();

        #endregion

        #region Nav

        string Nav();

        string NavItem();

        string NavLink();

        string NavTabs();

        string NavCards();

        string NavPills();

        string NavFill( NavFillType fillType );

        string NavVertical();

        #endregion

        #region Navbar

        string Bar();

        string BarShade( Theme theme );

        string BarBreakpoint( Breakpoint breakpoint );

        string BarItem();

        string BarItemActive();

        string BarItemDisabled();

        string BarItemDropdown();

        string BarItemDropdownShow();

        string BarLink();

        string BarLinkDisabled();

        //[Obsolete]
        //string BarCollapse();

        string BarBrand();

        string BarToggler();

        string BarMenu();

        string BarMenuShow();

        string BarStart();

        string BarEnd();

        //string BarHasDropdown();

        string BarDropdown();

        string BarDropdownShow();

        string BarDropdownToggler();

        string BarDropdownItem();

        string BarTogglerIcon();

        #endregion

        #region Collapse

        string Collapse();

        string CollapseShow();

        #endregion

        #region Row

        string Row();

        #endregion

        #region Col

        string Col();

        string Col( ColumnWidth columnWidth, IEnumerable<(Breakpoint breakpoint, bool offset)> rules );

        #endregion

        #region Snackbar

        string Snackbar();

        string SnackbarShow();

        string SnackbarMultiline();

        string SnackbarBody();

        string SnackbarAction();

        #endregion

        #region Alert

        string Alert();

        string AlertColor( Color color );

        string AlertDismisable();

        //string AlertShow();

        #endregion

        #region Modal

        string Modal();

        string ModalFade();

        string ModalShow();

        string ModalBackdrop();

        string ModalContent( bool isForm );

        string ModalContentCentered();

        string ModalBody();

        string ModalHeader();

        string ModalFooter();

        string ModalHeaderTitle();

        #endregion

        #region Pagination

        string Pagination();

        string PaginationSize( Size size );

        string PaginationItem();

        string PaginationLink();

        #endregion

        #region Progress

        string Progress();

        string ProgressSize( Size size );

        string ProgressBar();

        string ProgressBarStriped();

        string ProgressBarAnimated();

        string ProgressBarWidth( int width );

        #endregion

        #region Chart

        string Chart();

        #endregion

        #region Colors

        string BackgroundColor( Background background );

        #endregion

        #region Title

        string Title();

        string TitleSize( int size );

        #endregion

        #region Table

        string Table();

        string TableFullWidth();

        string TableStriped();

        string TableHoverable();

        string TableBordered();

        string TableHeader();

        string TableHeaderCell();

        string TableBody();

        string TableRow();

        string TableRowHeader();

        string TableRowCell();

        #endregion

        #region Badge

        string Badge();

        string BadgeColor( Color color );

        string BadgePill();

        #endregion

        #region Media

        string Media();

        string MediaLeft();

        string MediaRight();

        string MediaBody();

        #endregion

        #region SimpleText

        string SimpleTextColor( TextColor textColor );

        string SimpleTextAlignment( TextAlignment textAlignment );

        string SimpleTextTransform( TextTransform textTransform );

        string SimpleTextWeight( TextWeight textWeight );

        string SimpleTextItalic();

        #endregion

        #region Figure

        string Figure();

        #endregion

        #region States

        string Show();

        string Fade();

        string Active();

        string Disabled();

        string Collapsed();

        #endregion

        #region Layout

        string Spacing( Spacing spacing, SpacingSize spacingSize, Side side, Breakpoint breakpoint );

        string Spacing( Spacing spacing, SpacingSize spacingSize, IEnumerable<(Side side, Breakpoint breakpoint)> rules );

        #endregion

        #region Flex

        string FlexAlignment( Alignment alignment );

        #endregion

        #region Enums

        string Size( Size size );

        string Breakpoint( Breakpoint breakpoint );

        string Color( Color color );

        string Color( Background color );

        string TextColor( TextColor textColor );

        string Theme( Theme theme );

        string Float( Float @float );

        string Spacing( Spacing spacing );

        string Side( Side side );

        string Alignment( Alignment alignment );

        string TextAlignment( TextAlignment textAlignment );

        string TextTransform( TextTransform textTransform );

        string TextWeight( TextWeight textWeight );

        string ColumnWidth( ColumnWidth columnWidth );

        string ModalSize( ModalSize modalSize );

        string SpacingSize( SpacingSize spacingSize );

        string JustifyContent( JustifyContent justifyContent );

        #endregion

        /// <summary>
        /// Enables a custom css for select/check/radio/file inputs.
        /// </summary>
        bool Custom { get; set; }

        /// <summary>
        /// Gets the provider implementation name.
        /// </summary>
        string Provider { get; }
    }
}
