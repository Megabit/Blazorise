#region Using directives
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Blazorise.Shared.Models;
using Microsoft.Extensions.Caching.Memory;
#endregion

namespace Blazorise.Shared.Data
{
    public class SearchEntryData
    {
        private readonly IMemoryCache cache;
        private readonly string cacheKey = "cache_searchentries";

        /// <summary>
        /// Simplified code to get & cache data in memory...
        /// </summary>
        public SearchEntryData( IMemoryCache memoryCache )
        {
            cache = memoryCache;
        }

        public Task<SearchEntry[]> GetDataAsync()
            => cache.GetOrCreateAsync( cacheKey, LoadData );

        private Task<SearchEntry[]> LoadData( ICacheEntry cacheEntry ) //of course this cannot be the final method of loading this data
        {
            return Task.FromResult( new SearchEntry[] {
                new SearchEntry("/docs/components/accordion", "Accordion"),
                new SearchEntry("/docs/components/addon", "Adddon"),
                new SearchEntry("/docs/components/alert", "Alert"),
                new SearchEntry("/docs/components/badge", "Badge"),
                new SearchEntry("/docs/components/bar", "Bar"),
                new SearchEntry("/docs/components/breadcrumb", "Breadcrumb"),
                new SearchEntry("/docs/components/button", "Button"),
                new SearchEntry("/docs/components/card", "Card"),
                new SearchEntry("/docs/components/carousel", "Carousel"),
                new SearchEntry("/docs/components/check", "Check"),
                new SearchEntry("/docs/components/close-button", "CloseButton"),
                new SearchEntry("/docs/components/color", "Color"),
                new SearchEntry("/docs/components/color-picker", "ColorPicker"),
                new SearchEntry("/docs/components/date", "DateEdit"),
                new SearchEntry("/docs/components/date-picker", "DatePicker"),
                new SearchEntry("/docs/components/divider", "Divider"),
                new SearchEntry("/docs/components/dropzone", "DropZone"),
                new SearchEntry("/docs/components/dropdown", "Dropdown"),
                new SearchEntry("/docs/components/field", "Field"),
                new SearchEntry("/docs/components/figure", "Figure"),
                new SearchEntry("/docs/components/file", "FileEdit"),
                new SearchEntry("/docs/components/focus-trap", "FocusTrap"),
                new SearchEntry("/docs/components/grid", "Grid"),
                new SearchEntry("/docs/components/highlighter", "Highlighter"),
                new SearchEntry("/docs/components/input-mask", "InputMask"),
                new SearchEntry("/docs/components/jumbotron", "Jumbotron"),
                new SearchEntry("/docs/components/layout", "Layout"),
                new SearchEntry("/docs/components/link", "Link"),
                new SearchEntry("/docs/components/list-group", "ListGroup"),
                new SearchEntry("/docs/components/memo", "MemoEdit"),
                new SearchEntry("/docs/components/modal", "Modal"),
                new SearchEntry("/docs/components/numeric", "NumericEdit"),
                new SearchEntry("/docs/components/numeric-picker", "NumericPicker"),
                new SearchEntry("/docs/components/pagination", "Pagination"),
                new SearchEntry("/docs/components/progress", "Progress"),
                new SearchEntry("/docs/components/radio", "Radio"),
                new SearchEntry("/docs/components/rating", "Rating"),
                new SearchEntry("/docs/components/repeater", "Repeater"),
                new SearchEntry("/docs/components/select", "Select"),
                new SearchEntry("/docs/components/slider", "Slider"),
                new SearchEntry("/docs/components/step", "Step"),
                new SearchEntry("/docs/components/switch", "Switch"),
                new SearchEntry("/docs/components/table", "Table"),
                new SearchEntry("/docs/components/tab", "Tab"),
                new SearchEntry("/docs/components/text", "TextEdit"),
                new SearchEntry("/docs/theming", "Theming" ),
                new SearchEntry("/docs/components/time", "TimeEdit"),
                new SearchEntry("/docs/components/time-picker", "TimePicker"),
                new SearchEntry("/docs/components/tooltip", "Tooltip"),
                new SearchEntry("/docs/components/typography", "Typography"),
                new SearchEntry("/docs/components/validation", "Validation"),
                new SearchEntry("/docs/extensions/animate", "Animate" ),
                new SearchEntry("/docs/extensions/autocomplete", "Autocomplete" ),
                new SearchEntry("/docs/extensions/chart", "Chart" ),
                new SearchEntry("/docs/extensions/chart-live", "Chart Streaming" ),
                new SearchEntry("/docs/extensions/chart-trendline", "Chart Trendline" ),
                new SearchEntry("/docs/extensions/datagrid/aggregates", "DataGrid aggregates" ),
                new SearchEntry("/docs/extensions/datagrid/binding-data", "DataGrid Binding Data" ),
                new SearchEntry("/docs/extensions/datagrid/binding-data", "DataGrid features" ),
                new SearchEntry("/docs/extensions/datagrid/getting-started", "DataGrid" ),
                new SearchEntry("/docs/extensions/datagrid/selection", "DataGrid selection" ),
                new SearchEntry("/docs/extensions/datagrid/templates", "DataGrid templates" ),
                new SearchEntry("/docs/extensions/datagrid/validations", "DataGrid validations" ),
                new SearchEntry("/docs/extensions/dropdownlist", "DropdownList" ),
                new SearchEntry("/docs/extensions/icons-available", "Available Icons" ),
                new SearchEntry("/docs/extensions/icons", "Icons" ),
                new SearchEntry("/docs/extensions/list-view", "List View" ),
                new SearchEntry("/docs/extensions/markdown", "Markdown" ),
                new SearchEntry("/docs/extensions/qrcode", "QRCode"),
                new SearchEntry("/docs/extensions/richtextedit", "RichTextEdit" ),
                new SearchEntry("/docs/extensions/selectlist", "SelectList" ),
                new SearchEntry("/docs/extensions/sidebar", "Sidebar" ),
                new SearchEntry("/docs/extensions/snackbar", "Snackbar" ),
                new SearchEntry("/docs/extensions/spinkit", "SpinKit" ),
                new SearchEntry("/docs/extensions/treeview", "TreeView" ),
                new SearchEntry("/docs/extensions/video", "Video"),
                new SearchEntry("/docs/faq", "Blazorise FAQ" ),
                new SearchEntry("/docs/usage/ant-design", "AntDesign Usage" ),
                new SearchEntry("/docs/usage/bootstrap4", "Bootstrap 4 Usage" ),
                new SearchEntry("/docs/usage/bootstrap5", "Bootstrap 5 Usage" ),
                new SearchEntry("/docs/usage/bulma", "Bulma Usage" ),
                new SearchEntry("/docs/usage/material", "Material Usage" ),
                new SearchEntry("/docs/helpers/colors", "Color Utilities" ),
                new SearchEntry("/docs/helpers/enums/bar", "Enums: Bar" ),
                new SearchEntry("/docs/helpers/enums/button", "Enums: Button" ),
                new SearchEntry("/docs/helpers/enums/chart", "Enums: Button" ),
                new SearchEntry("/docs/helpers/enums/common", "Enums: Common" ),
                new SearchEntry("/docs/helpers/enums/datagrid", "Enums: DataGrid" ),
                new SearchEntry("/docs/helpers/enums/date", "Enums: Date" ),
                new SearchEntry("/docs/helpers/enums/divider", "Enums: Divider" ),
                new SearchEntry("/docs/helpers/enums/dropdown", "Enums: Dropdown" ),
                new SearchEntry("/docs/helpers/enums/heading", "Enums: Heading" ),
                new SearchEntry("/docs/helpers/enums/icon", "Enums: Icon" ),
                new SearchEntry("/docs/helpers/enums/listgroup", "Enums: ListGroup" ),
                new SearchEntry("/docs/helpers/enums/snackbar", "Enums: Snackbar" ),
                new SearchEntry("/docs/helpers/enums/spinkit", "Enums: SpinKit" ),
                new SearchEntry("/docs/helpers/enums/table", "Enums: Table" ),
                new SearchEntry("/docs/helpers/enums/tabs", "Enums: Tabs" ),
                new SearchEntry("/docs/helpers/enums/text", "Enums: Text" ),
                new SearchEntry("/docs/helpers/enums/tooltip", "Enums: Tooltip" ),
                new SearchEntry("/docs/helpers/enums/validation", "Enums: Validation" ),
                new SearchEntry("/docs/helpers/localization", "Localization" ),
                new SearchEntry("/docs/helpers/sizes", "Sizes" ),
                new SearchEntry("/docs/helpers/utilities/position", "Position" ),
                new SearchEntry("/docs/helpers/utilities", "Utilities" ),
                new SearchEntry("/docs/pwa", "PWA" ),
                new SearchEntry("/docs/services/message", "Message service" ),
                new SearchEntry("/docs/services/notification", "Notification service" ),
                new SearchEntry("/docs/services/page-progress", "Page Progress service" ),
                new SearchEntry("/docs/start", "Start" ),
                new SearchEntry("/docs/usage", "Usage" )
                }.OrderBy(x => x.Name).ToArray());
        }
    }
}