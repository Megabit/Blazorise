using System;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Blazorise.Shared.Models;
using Microsoft.Extensions.Caching.Memory;

namespace Blazorise.Shared.Data;

public class PageEntryData
{
    private readonly IMemoryCache cache;
    private readonly string cacheKey = "cache_searchentries";

    /// <summary>
    /// Simplified code to get & cache data in memory...
    /// </summary>
    public PageEntryData( IMemoryCache memoryCache )
    {
        cache = memoryCache;
    }

    public Task<PageEntry[]> GetDataAsync()
        => cache.GetOrCreateAsync( cacheKey, LoadData );

    private Task<PageEntry[]> LoadData( ICacheEntry cacheEntry )
    {
        return Task.FromResult( new PageEntry[]
        {
            new PageEntry( "docs", "Overview" ),
            new PageEntry( "docs/start", "Start" ),
            new PageEntry( "docs/usage", "Usage" ),
            new PageEntry( "docs/theming", "Theming" ),
            new PageEntry( "docs/pwa", "PWA" ),

                new PageEntry( "docs/components/accordion", "Accordion", "Build vertically collapsing accordions in combination with our Collapse component." ),
                new PageEntry( "docs/components/addon", "Addon", "Easily extend form controls by adding text, or buttons on either side of textual inputs, custom selects, and custom file inputs." ),
                new PageEntry( "docs/components/alert", "Alert", "Provide contextual feedback messages for typical user actions with the handful of available and flexible alert messages." ),
                new PageEntry( "docs/components/badge", "Badge", "Badges are used to draw attention and display statuses or counts." ),
                new PageEntry( "docs/components/bar", "Bar", "A responsive navbar that can support images, links, buttons, and dropdowns." ),
                new PageEntry( "docs/components/breadcrumb", "Breadcrumb", "A simple breadcrumb component to improve your navigation experience." ),
                new PageEntry( "docs/components/button", "Button", "Use Blazorise custom button styles for actions in forms, dialogs, and more with support for multiple sizes, states, and more." ),
                new PageEntry( "docs/components/card", "Card", "Blazorise cards provide a flexible and extensible content container with multiple variants and options." ),
                new PageEntry( "docs/components/carousel", "Carousel", "Loop a series of images or texts in a limited space." ),
                new PageEntry( "docs/components/check", "Check", "Check allow the user to toggle an option on or off." ),
                new PageEntry( "docs/components/close-button", "Close Button", "A generic close button for dismissing content like modals and alerts." ),
                new PageEntry( "docs/components/color", "Color Edit", "The ColorEdit allow the user to select a color." ),
                new PageEntry( "docs/components/color-picker", "Color Picker", "The ColorPicker allow the user to select a color using a variety of input methods." ),
                new PageEntry( "docs/components/date", "Date Edit", "DateEdit is an input field that allows the user to enter a date by typing or by selecting from a calendar overlay." ),
                new PageEntry( "docs/components/date-picker", "Date Picker", "DatePicker is an input field that allows the user to enter a date by typing or by selecting from a calendar overlay." ),
                new PageEntry( "docs/components/divider", "Divider", "Dividers are used to visually separate or group elements." ),
                new PageEntry( "docs/components/dragdrop", "Drag & Drop", "Quickly drag and drop elements between the containers." ),
                new PageEntry( "docs/components/dropdown", "Dropdown", "Dropdowns expose additional content that \"drops down\" in a menu." ),
                new PageEntry( "docs/components/field", "Field", "A generic container used to properly layout input elements on a form." ),
                new PageEntry( "docs/components/figure", "Figure", "Documentation and examples for displaying related images and text with the figure component in Blazorise." ),
                new PageEntry( "docs/components/file", "File Edit", "The FileEdit component is a specialized input that provides a clean interface for selecting files." ),
                new PageEntry( "docs/components/focus-trap", "FocusTrap", "Manages focus of the forms and its input elements." ),
                new PageEntry( "docs/components/grid", "Grid", "A simple way to build responsive columns." ),
                new PageEntry( "docs/components/highlighter", "Highlighter", "Visually highlight part of the text based on the search term." ),
                new PageEntry( "docs/components/jumbotron", "Jumbotron", "Lightweight, flexible component for showcasing hero unit style content." ),
                new PageEntry( "docs/components/layout", "Layout", "Handling the overall layout of a page." ),
                new PageEntry( "docs/components/link", "Link", "Provides declarative, accessible navigation around your application." ),
                new PageEntry( "docs/components/list-group", "List Group", "List groups are a flexible and powerful component for displaying a series of content." ),
                new PageEntry( "docs/components/input-mask", "Input Mask", "Input mask allows the user to input a value in a specific format while typing." ),
                new PageEntry( "docs/components/memo", "Memo Edit", "MemoEdit collect data from the user and allow multiple lines of text." ),
                new PageEntry( "docs/components/modal", "Modal", "Dialog is a small window that can be used to present information and user interface elements in an overlay." ),
                new PageEntry( "docs/components/numeric", "Numeric Edit", "A native numeric <input> component built around the <input type=\"number\">." ),
                new PageEntry( "docs/components/numeric-picker", "Numeric Picker", "A customizable NumericPicker component allows you to enter numeric values and contains controls for increasing and reducing the value." ),
                new PageEntry( "docs/components/pagination", "Pagination", "A responsive, usable, and flexible pagination." ),
                new PageEntry( "docs/components/progress", "Progress", "Progress bars are used to show the status of an ongoing operation." ),
                new PageEntry( "docs/components/radio", "Radio", "The Radio allow the user to select a single option from a group." ),
                new PageEntry( "docs/components/rating", "Rating", "Ratings provide insight regarding others opinions and experiences with a product." ),
                new PageEntry( "docs/components/repeater", "Repeater", "The repeater component is a helper component that repeats the child content for each element in a collection." ),
                new PageEntry( "docs/components/select", "Select", "Selects allow you to choose one or more items from a dropdown menu." ),
                new PageEntry( "docs/components/slider", "Slider", "Sliders allow users to make selections from a range of values." ),
                new PageEntry( "docs/components/step", "Step", "The Step component displays progress through numbered steps." ),
                new PageEntry( "docs/components/switch", "Switch", "Switch is used for switching between two opposing states." ),
                new PageEntry( "docs/components/tab", "Tab", "Tabs are used to organize and group content into sections that the user can navigate between." ),
                new PageEntry( "docs/components/table", "Table", "Basic table is just for data display." ),
                new PageEntry( "docs/components/time", "Time Edit", "A native time input component build around the <input type=\"time\">." ),
                new PageEntry( "docs/components/time-picker", "Time Picker", "A customizable time input component with an option to manually write time or choose from a menu." ),
                new PageEntry( "docs/components/tooltip", "Tooltip", "Tooltips display additional information based on a specific action." ),
                new PageEntry( "docs/components/text", "Text Edit", "The TextEdit allows the user to input and edit text." ),
                new PageEntry( "docs/components/typography", "Typography", "Control text size, alignment, wrapping, overflow, transforms and more." ),
                new PageEntry( "docs/components/validation", "Validation", "The Validation component allows you to verify your data, helping you find and correct errors." ),

                new PageEntry( "docs/extensions/animate", "Animate", "" ),
                new PageEntry( "docs/extensions/autocomplete", "Autocomplete", "The Autocomplete component offers simple and flexible type-ahead functionality." ),
                new PageEntry( "docs/extensions/chart", "Chart", "Simple yet flexible charting for designers & developers." ),
                new PageEntry( "docs/extensions/chart-datalabels", "Chart DataLabels", "Display labels on data for any type of charts." ),
                new PageEntry( "docs/extensions/chart-live", "Chart Streaming", "Chart plugin for live streaming data." ),
                new PageEntry( "docs/extensions/chart-trendline", "Chart Trendline", "This plugin draws an linear trendline in your Chart." ),
                new PageEntry( "docs/extensions/datagrid/aggregates", "DataGrid Aggregates" ),

                new PageEntry( "docs/extensions/datagrid/getting-started", "DataGrid", "The DataGrid component is used for displaying tabular data. Features include sorting, searching, pagination, content-editing, and row selection." ),

                new PageEntry( "docs/extensions/datagrid/binding-data", "DataGrid Binding Data" ),
                new PageEntry( "docs/extensions/datagrid/binding-data/in-memory", "DataGrid Binding Data : In Memory", "The DataGrid can bind to your data in memory, allowing for quick and efficient display of your data sets." ),
                new PageEntry( "docs/extensions/datagrid/binding-data/large-data", "DataGrid Binding Data : Large Data", "The DataGrid Read Data feature allows you to handle Large Data by providing you with a centralized ReadData Method that allows you to query your data by pages." ),
                new PageEntry( "docs/extensions/datagrid/binding-data/virtualize", "DataGrid Binding Data : Virtualize", "Optimize performance for large data sets with the Blazorise DataGrid's virtualization feature. By only rendering the data currently visible on the screen, virtualization reduces the amount of DOM elements, resulting in improved performance and load times." ),

                new PageEntry( "docs/extensions/datagrid/features", "DataGrid Features" ),
                new PageEntry( "docs/extensions/datagrid/features/context-menu", "DataGrid Context Menu", "Right-click on any row to access options such as editing, deleting, and custom actions." ),
                new PageEntry( "docs/extensions/datagrid/features/editing", "DataGrid Editing", "The DataGrid can perform some basic CRUD operations on the supplied Data collection." ),
                new PageEntry( "docs/extensions/datagrid/features/filtering", "DataGrid Filtering", "Quickly search and filter by any column, or customize filter options to fit your needs." ),
                new PageEntry( "docs/extensions/datagrid/features/fixed-header", "DataGrid Fixed Header", "The table header remains visible as you scroll, making it easy to identify and reference the data in your grid." ),
                new PageEntry( "docs/extensions/datagrid/features/grouping", "DataGrid Grouping", "Grouping feature for Blazorise DataGrid allows you to easily group and organize your data by specific columns." ),
                new PageEntry( "docs/extensions/datagrid/features/paging", "DataGrid Paging", "Paginate your data with customizable page size options and intuitive navigation controls." ),
                new PageEntry( "docs/extensions/datagrid/features/resizing", "DataGrid Resizing", "Easily adjust the size of your columns with the Blazorise DataGrid's resizing feature. Drag and drop column edges to resize, or use customized options to fit your needs." ),
                new PageEntry( "docs/extensions/datagrid/features/sorting", "DataGrid Sorting", "Blazorise DataGrid offers efficient data sorting with customizable options." ),

                
                new PageEntry( "docs/extensions/datagrid/selection", "DataGrid Selection" ),
                new PageEntry( "docs/extensions/datagrid/selection/single", "DataGrid Single Selection", "Easily select and manage a single row of data with the Blazorise DataGrid's single selection feature. Select a row by clicking on it or programmatically, and access the selected data for further use." ),
            new PageEntry( "docs/extensions/datagrid/selection/multiple", "DataGrid Multiple Selection", "Select and manage multiple rows of data with ease using the Blazorise DataGrid's multiple selection feature. Select rows by clicking on them or programmatically, and access the selected data for further use." ),
                new PageEntry( "docs/extensions/datagrid/selection/custom-row-colors", "DataGrid Custom Row Colors", "Easily color code your data with the Blazorise DataGrid's custom row color feature." ),

                new PageEntry( "docs/extensions/datagrid/templates", "DataGrid Templates" ),
                new PageEntry( "docs/extensions/datagrid/templates/button-row", "DataGrid Templates : Button Row", "Easily customize the action buttons shown with the Blazorise DataGrid's button row template feature." ),
                new PageEntry( "docs/extensions/datagrid/templates/commands", "DataGrid Templates : Commands", "Customize your action commands with the Blazorise DataGrid's commands template feature. Assign custom templates to the command column, such as editing and deleting, or add your own custom actions." ),
                new PageEntry( "docs/extensions/datagrid/templates/detail-row", "DataGrid Templates : Detail Row", "Easily expand and display additional information for each row using the Blazorise DataGrid's detail row template feature." ),
                new PageEntry( "docs/extensions/datagrid/templates/display", "DataGrid Templates : Display", "Customize how your data is displayed in each cell using the Blazorise DataGrid's display template feature. Assign custom templates to individual columns, allowing for more control over how your data is displayed. " ),
                new PageEntry( "docs/extensions/datagrid/templates/edit", "DataGrid Templates : Edit", "Assign custom edit templates to individual columns, allowing for more control over how data is edited." ),
                new PageEntry( "docs/extensions/datagrid/templates/loading", "DataGrid Templates : Loading", "Customize the loading look of the Blazorise DataGrid with the loading template feature." ),

                new PageEntry( "docs/extensions/datagrid/validations", "DataGrid Validations" ),
                new PageEntry( "docs/extensions/dropdownlist", "DropdownList", "The DropdownList component allows you to select a value from a list of predefined items." ),
                new PageEntry( "docs/extensions/icons", "Icons", "Icons are symbols that can be used to represent various options within an application." ),
                new PageEntry( "docs/extensions/icons-available", "Available Icons" ),
                new PageEntry( "docs/extensions/list-view", "List View", "List views are a flexible and powerful component for displaying a series of content in a contained scrollable view by providing a data source." ),
                new PageEntry( "docs/extensions/lottie-animation", "Lottie Animation", "The LottieAnimation component is used to embed JSON-based Lottie animations." ),
                new PageEntry( "docs/extensions/cropper", "Cropper", "A component used to crop images." ),
                new PageEntry( "docs/extensions/loadingindicator", "LoadingIndicator", "A wrapper component used to add loading indocators UI to other components." ),
                new PageEntry( "docs/extensions/markdown", "Markdown", "The Markdown component allows you to edit markdown strings." ),
                new PageEntry( "docs/extensions/richtextedit", "RichTextEdit", "The RichTextEdit component allows you to add and use a ‘WYSIWYG’ rich text editor." ),
                new PageEntry( "docs/extensions/selectlist", "SelectList", "The SelectList component allows you to select a value from a list of predefined items." ),
                new PageEntry( "docs/extensions/sidebar", "Sidebar", "The Sidebar component is an expandable and collapsible container area that holds primary and secondary information placed alongside the main content of a webpage." ),
                new PageEntry( "docs/extensions/snackbar", "Snackbar", "Snackbar provide brief messages about app processes. The component is also known as a toast." ),
                new PageEntry( "docs/extensions/spinkit", "SpinKit", "A component used to show loading indicators animated with CSS." ),
                new PageEntry( "docs/extensions/treeview", "TreeView", "The TreeView component is a graphical control element that presents a hierarchical view of information." ),
                new PageEntry( "docs/extensions/video", "Video", "A Video component used to play a regular or streaming media." ),
                new PageEntry( "docs/extensions/qrcode", "QRCode", "A component used to generate QR codes." ),
                new PageEntry( "docs/extensions/fluent-validation", "FluentValidation", "A validation component for building strongly-typed validation rules." ),

            new PageEntry( "docs/usage/ant-design", "AntDesign Usage" ),
            new PageEntry( "docs/usage/bootstrap4", "Bootstrap 4 Usage" ),
            new PageEntry( "docs/usage/bootstrap5", "Bootstrap 5 Usage" ),
            new PageEntry( "docs/usage/bulma", "Bulma Usage" ),
            new PageEntry( "docs/usage/material", "Material Usage" ),
            new PageEntry( "docs/helpers/colors", "Color Utilities" ),
            new PageEntry( "docs/helpers/enums/bar", "Enums: Bar" ),
            new PageEntry( "docs/helpers/enums/button", "Enums: Button" ),
            new PageEntry( "docs/helpers/enums/chart", "Enums: Button" ),
            new PageEntry( "docs/helpers/enums/common", "Enums: Common" ),
            new PageEntry( "docs/helpers/enums/datagrid", "Enums: DataGrid" ),
            new PageEntry( "docs/helpers/enums/date", "Enums: Date" ),
            new PageEntry( "docs/helpers/enums/divider", "Enums: Divider" ),
            new PageEntry( "docs/helpers/enums/dropdown", "Enums: Dropdown" ),
            new PageEntry( "docs/helpers/enums/heading", "Enums: Heading" ),
            new PageEntry( "docs/helpers/enums/icon", "Enums: Icon" ),
            new PageEntry( "docs/helpers/enums/listgroup", "Enums: ListGroup" ),
            new PageEntry( "docs/helpers/enums/snackbar", "Enums: Snackbar" ),
            new PageEntry( "docs/helpers/enums/spinkit", "Enums: SpinKit" ),
            new PageEntry( "docs/helpers/enums/table", "Enums: Table" ),
            new PageEntry( "docs/helpers/enums/tabs", "Enums: Tabs" ),
            new PageEntry( "docs/helpers/enums/text", "Enums: Text" ),
            new PageEntry( "docs/helpers/enums/tooltip", "Enums: Tooltip" ),
            new PageEntry( "docs/helpers/enums/validation", "Enums: Validation" ),
            new PageEntry( "docs/helpers/localization", "Localization" ),
            new PageEntry( "docs/helpers/sizes", "Sizes" ),
            new PageEntry( "docs/helpers/utilities/position", "Position" ),
            new PageEntry( "docs/helpers/utilities", "Utilities" ),

            new PageEntry( "docs/services/message-provider", "Message Provider", "Message service is used for quick user confirmation actions." ),
            new PageEntry( "docs/services/modal-provider", "Modal Provider", "Programatically instantiate modals with custom content." ),
            new PageEntry( "docs/services/notification-provider", "Notification Provider", "Notification service is used to provide feedback to the user." ),
            new PageEntry( "docs/services/page-progress-provider", "Page Progress Provider", "Page Progress service is used to provide a page loading indicator to the user." ),
            new PageEntry( "docs/faq", "FAQ" ),
            new PageEntry( "https://blazorise.com/license", "License" ),
        }.ToArray() );
    }
}