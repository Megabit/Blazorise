#region Using directives
using System;
#endregion

namespace Blazorise.Reporting.Internal;

[Flags]
internal enum ReportDesignerRefreshTarget
{
    None = 0,
    Surface = 1,
    SelectedPanel = 2,
    ElementSelection = 4,
    FieldsExplorer = 8,
    Toolbar = 16,
    Designer = Surface | SelectedPanel | Toolbar,
    DesignerWithFieldsExplorer = Designer | FieldsExplorer,
    All = DesignerWithFieldsExplorer | ElementSelection,
}