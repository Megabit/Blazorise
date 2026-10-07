# Blazorise.Ribbon

A provider-independent ribbon extension built entirely with Blazorise components and utilities. Tabs declare their headings and groups together; the extension does not use the provider's `Tabs` component or tab classes.

Add a reference to `Blazorise.Ribbon`, configure Blazorise and a provider as usual, and import the namespace:

```razor
@using Blazorise.Ribbon
```

No extension stylesheet, additional services, or JavaScript assets need registration. Layout, spacing, sizing, overflow, and selection indicators use Blazorise utility parameters. Keyboard navigation uses Blazorise's existing tab keyboard module.

```razor
<Ribbon @bind-SelectedTab="@selectedTab">
    <RibbonTabs>
        <RibbonTab Name="home" Text="Home">
            <RibbonGroup Text="Clipboard">
                <RibbonButton Text="Paste" Icon="IconName.Paste"
                              Size="RibbonItemSize.Large" Clicked="@OnPasteHandler" />
                <RibbonColumn>
                    <RibbonButton Text="Cut" Icon="IconName.Cut" />
                    <RibbonButton Text="Copy" Icon="IconName.Copy" />
                    <RibbonButton Text="Format Painter" Icon="IconName.PaintBrush" />
                </RibbonColumn>
            </RibbonGroup>
            <RibbonGroup Text="Font" Layout="RibbonGroupLayout.Rows">
                <RibbonRow>
                    @fontControls
                </RibbonRow>
                <RibbonRow>
                    <RibbonToggleButton Text="Bold" Icon="IconName.Bold"
                                        Size="RibbonItemSize.Small" @bind-Checked="@bold" />
                    <RibbonColorPicker Size="Size.Small" @bind-Value="@fontColor"
                                       aria-label="Font color" title="Font color" />
                </RibbonRow>
            </RibbonGroup>
        </RibbonTab>
    </RibbonTabs>
</Ribbon>
```

## Composition

Use dedicated ribbon components for standard commands so their sizing, presentation, and accessibility stay consistent. Rows and columns also accept other Blazorise components for specialized controls.

- `RibbonTab.Name` must be non-empty and unique within the ribbon. `Text` supplies the heading; `HeaderContent` can replace it.
- `RibbonGroup` arranges large commands and explicit `RibbonColumn` stacks side by side. Use columns containing up to three small or medium commands for the classic ribbon arrangement. Columns accept arbitrary Blazorise controls; their contents are not automatically redistributed.
- `Layout="RibbonGroupLayout.Rows"` stacks explicit `RibbonRow` children. Rows accept arbitrary Blazorise controls.
- `RibbonButton`, `RibbonToggleButton`, `RibbonDropdown`, and `RibbonSplitButton` share `Text`, `Icon`, `Size`, `Color`, and `Disabled`. For buttons and toggles, `ChildContent` replaces their icon/text content.
- Default commands use the native `Ghost` appearance: transparent at rest, provider-themed shading on hover and activation, and visible keyboard focus. Command colors control the foreground and the hover and active tint. Application triggers, tab headings, gallery previews, launchers, and the collapse button also use the native ghost appearance.
- `Small` shows the icon and uses `Text` as its accessible name. `Medium` shows an icon beside text. `Large` shows an icon above text.
- Dropdown and split commands place the menu arrow beside the command for `Small` and `Medium`, and below it for `Large`. This follows the effective size, so large commands reduced to medium in simplified mode become horizontal.
- A large `RibbonSplitButton` divides its height equally between the icon action button and the text-and-arrow menu toggle below it. Both parts have matching width and padding. Clicking the icon invokes `Clicked`; clicking the text or arrow opens the menu. Medium and small split commands keep the text/icon presentation in the action button and the arrow beside it.
- `RibbonDropdown` and `RibbonSplitButton` accept `RibbonDropdownItem` and `RibbonDropdownDivider` components directly as child content. `HeaderContent` can replace their default icon/text heading. Split commands expose a separate `Clicked` event for the default action.
- `RibbonDropdownItem` supports `Text`, `Icon`, `Disabled`, `Active`, and `Clicked`. `ChildContent` can replace the default icon/text content. Selecting an item closes the native dropdown hierarchy by default; set `CloseParentDropdowns="false"` to close only the current menu.
- `RibbonColorPicker` wraps a declaratively rendered `ColorPicker` without changing its native swatch or popup. It forwards `Value`, `ValueChanged`, `ShowValue`, `Size`, `Palette`, `Disabled`, `ReadOnly`, and the picker visibility and localization options. `ShowValue` defaults to `false` to keep ribbon controls compact. `Focus()` focuses the inner picker. Use `Size.Small` for compact ribbon controls and `aria-label` and `title` to identify them.
- `RibbonGallery` binds `SelectedValue` to the string `Value` of a `RibbonGalleryItem`. Each item accepts custom preview content.
- Groups expose `ShowLauncher`, `LauncherAriaLabel`, and `LauncherClicked`; applications own the launched dialogs.

```razor
<RibbonDropdown Text="Share" Icon="IconName.ShareAlt" Size="RibbonItemSize.Medium">
    <RibbonDropdownItem Text="Share document" Clicked="@OnShareHandler" />
    <RibbonDropdownItem Text="Copy link" Clicked="@OnCopyLinkHandler" />
</RibbonDropdown>

<RibbonSplitButton Text="Paste" Icon="IconName.Paste" Size="RibbonItemSize.Large" Clicked="@OnPasteHandler">
    <RibbonDropdownItem Text="Keep source formatting" Clicked="@OnPasteHandler" />
    <RibbonDropdownItem Text="Keep text only" Clicked="@OnPasteTextHandler" />
    <RibbonDropdownDivider />
    <RibbonDropdownItem Text="Paste special…" Clicked="@OnClipboardLauncherHandler" />
</RibbonSplitButton>
```

Declare tabs inside the explicit `<RibbonTabs>` fragment. `<RibbonApplicationTab>` accepts the application entry preceding the tabs, usually File. Choose `RibbonApplicationMenu` for commands and nested submenus, or `RibbonApplicationButton` for opening the optional backstage or invoking a custom action. Their headings share the same markup, appearance, and alignment as tab headings. The application entry does not change ribbon tab selection or participate in tab arrow-key navigation. `QuickAccessContent` and `TabStripContent` accept commands before and after the tab strip.

## Selection and rendering

`SelectedTab` and `Collapsed` support two-way binding. `SelectTab(name)` selects an enabled, visible tab and expands the ribbon. `SetCollapsed(bool)` uses the same notification path as the collapse button.

Use `RibbonContextualTabs` and `ActiveContextualGroups` for named editing contexts, or `Visible` and conditional Razor declarations for individual tabs. If the selected tab becomes unavailable, selection restores the last available regular tab before falling back to the first available tab. With no available tabs, selection becomes null.

`RibbonTab.Order` controls heading order, fallback selection, contextual auto-selection, and animation direction across regular and contextual tabs. Lower values come first; equal values retain registration order. The default is zero. For dynamic collections, use `@key` to preserve tab identity and bind `Order` to the current item index. Update `Order` when reordering existing entries.

Changing `Animated` enables or disables transitions without recreating tabs or their retained controls.

The default `RenderMode` preserves all panel content. `TabsRenderMode.LazyLoad` creates a panel on its first selection and preserves it; `LazyReload` creates only the selected panel. Collapsing the ribbon preserves the selected panel.

Arrow keys and Home/End move focus between enabled, visible headings. Enter or Space selects the focused heading. Navigation respects right-to-left layouts. Command controls use their existing Blazorise keyboard behavior.

Classic groups scroll horizontally when space is limited. `DisplayMode="RibbonDisplayMode.Simplified"` turns groups, columns, and rows into one horizontal command strip and hides group captions. It does not clone commands or automatically move them into overflow menus.

## Application menu

`RibbonApplicationMenu` renders a native Blazorise dropdown with a File trigger styled like a ribbon tab. Its dropdown indicator is hidden by default; set `ShowToggleIcon` to display it. `HeaderContent` can replace the trigger text. The native toggle manages `aria-expanded` and `aria-controls` internally.

```razor
<Ribbon>
    <RibbonApplicationTab>
        <RibbonApplicationMenu Text="File">
            <RibbonApplicationMenuItem Text="New" Icon="IconName.File" Clicked="@OnNewHandler" />
            <RibbonApplicationMenuItem Text="Open" Icon="IconName.FolderOpen" Clicked="@OnOpenHandler" />
            <RibbonApplicationMenuItem Text="Save As" Icon="IconName.Save">
                <RibbonApplicationMenuItem Text="Word Document" Clicked="@OnSaveWordHandler" />
                <RibbonApplicationMenuItem Text="PDF" Clicked="@OnSavePdfHandler" />
            </RibbonApplicationMenuItem>
            <RibbonApplicationMenuDivider />
            <RibbonApplicationMenuItem Text="Print" Icon="IconName.Print" Clicked="@OnPrintHandler" />
        </RibbonApplicationMenu>
    </RibbonApplicationTab>
    <RibbonTabs>
        <RibbonTab Name="home" Text="Home">
            @* Ribbon groups *@
        </RibbonTab>
    </RibbonTabs>
</Ribbon>
```

- A `RibbonApplicationMenuItem` with `ChildContent` opens a submenu. A leaf item invokes its `Clicked` callback after the native dropdown closes the menu hierarchy. Submenus can nest to multiple levels.
- Items support `Text`, `Icon`, and `Disabled`. Use `RibbonApplicationMenuDivider` to separate application menu commands. Menus also accept existing `DropdownHeader` and nested `Dropdown` components.
- `Visible` and `VisibleChanged` optionally bind the open state. `Show()`, `Hide()`, and `Focus()` delegate to the native dropdown and its trigger. `Direction` controls where the top-level menu opens; nested menus use the native submenu direction.
- Utility parameters, `Class`, `Style`, and unmatched attributes apply to the menu trigger or item. Positioning, keyboard interaction, and dismissal use the existing dropdown behavior. No extension CSS or JavaScript is required.

## Backstage

`RibbonBackstage` optionally displays file commands and pages inside the application's workspace. Place it beside the ribbon and keep the title bar outside the editing area that is hidden while backstage is open. `RibbonApplicationButton` supports `Text`, `Disabled`, `Expanded`, `Clicked`, custom `ChildContent`, and `Focus()`. Bind `Expanded` to backstage visibility; the button manages `aria-expanded` internally. Leave `Expanded` unset for a command without expandable content.

```razor
<Div Display="@(backstageVisible ? Display.None : Display.Block)" hidden="@backstageVisible">
    <Ribbon>
        <RibbonApplicationTab>
            <RibbonApplicationButton @ref="@fileButtonRef" Text="File" Expanded="@backstageVisible" Clicked="@OnFileHandler"
                                     aria-controls="document-backstage" />
        </RibbonApplicationTab>
        <RibbonTabs>
            <RibbonTab Name="home" Text="Home">
                <RibbonGroup Text="Clipboard">
                    <RibbonButton Text="Paste" Icon="IconName.Paste" />
                </RibbonGroup>
            </RibbonTab>
        </RibbonTabs>
    </Ribbon>
    @* Document editing area *@
</Div>

<RibbonBackstage @ref="@backstageRef" ElementId="document-backstage"
                 @bind-Visible="@backstageVisible" @bind-SelectedItem="@selectedItem"
                 Closed="@OnBackstageClosedHandler">
    <RibbonBackstageItem Name="info" Text="Info" Icon="IconName.InfoCircle">
        <Heading Size="HeadingSize.Is3">Document information</Heading>
        <Paragraph>@documentTitle</Paragraph>
    </RibbonBackstageItem>
    <RibbonBackstageItem Name="save" Text="Save" Icon="IconName.Save"
                        Clicked="@OnSaveHandler" />
    <RibbonBackstageItem Name="print" Text="Print" Icon="IconName.Print">
        <Heading Size="HeadingSize.Is3">Print</Heading>
        <Button Color="Color.Primary" Clicked="@OnPrintHandler">Print document</Button>
    </RibbonBackstageItem>
</RibbonBackstage>

@code {
    private RibbonBackstage backstageRef;
    private RibbonApplicationButton fileButtonRef;
    private bool backstageVisible;
    private string selectedItem = "info";
    private string documentTitle = "Quarterly overview";

    private Task OnFileHandler() => backstageRef.Show();

    private Task OnBackstageClosedHandler() => fileButtonRef.Focus( false );

    private async Task OnSaveHandler()
    {
        // Save the document here, then return to editing.
        await backstageRef.Hide();
    }

    private Task OnPrintHandler()
    {
        // Invoke the application's print workflow here.
        return Task.CompletedTask;
    }
}
```

- Every `RibbonBackstageItem` requires a non-empty, unique `Name`. An item with `ChildContent` opens that page; an item without content invokes its `Clicked` callback without changing the selected page.
- Page selection awaits `SelectedItemChanged` before invoking the item's `Clicked` callback. Commands remain open unless the application calls `Hide()` or changes `Visible`.
- `Show()` and `Hide()` share the `VisibleChanged` notification path. The Back button and Escape close backstage. `Opened` runs after the Back button receives focus; `Closed` runs after the surface is hidden so the application can restore focus to File.
- `SelectItem(name)` selects an enabled, visible page without opening backstage. If the selected page becomes unavailable, selection falls back to the first available page, or null when there are none. Command entries cannot become selected pages.
- The default `RenderMode`, `TabsRenderMode.LazyLoad`, creates a page on its first visible selection and preserves it. `LazyReload` recreates pages when switching selection; `Default` creates all pages. Hiding backstage preserves the current page.
- Item utility parameters, `Class`, `Style`, and unmatched attributes apply to its navigation button. Compose page content with Blazorise components and utilities. The backstage surface accepts its own utility parameters, including `Height`, and accessible labels through `AriaLabel`, `NavigationAriaLabel`, and `BackText`.
- `RibbonBackstageItem.Order` controls navigation and fallback selection order. Lower values come first; equal values retain registration order. Bind `Order` to the current item index for dynamic collections and use `@key` to preserve item identity.

Backstage uses normal document navigation and independently scrolling navigation and content areas. It requires no Offcanvas, backdrop, focus trap, extension stylesheet, or additional JavaScript registration.

## Word sample

The shared demo exposes `/tests/ribbon`, including Home, Insert, Design, Layout, Review, View, Help, and contextual Table Tools. Formatting changes the sample document as a whole. Undo/redo, session save, a sample clipboard, style selection, search counts, and zoom demonstrate application-owned command handling. Other commands report their invocation; the sample is not a rich text editor.

File opens inline backstage with Info, New, Open, Save, Save As, and Print. New and Open use sample documents, saves last for the demo session, and Print demonstrates settings and command handling without submitting a print job. Back and Escape return to the editor and restore focus to File. The title bar remains visible, and page controls retain their state between visits.

A separate simple ribbon demonstrates an application menu with New, Open, Save, Print, disabled items, and Save As submenus nested to multiple levels. Its document commands update the Word sample; format commands only report their invocation.

The hierarchy and command arrangements were inspired by [RibbonSpace's Word sample](https://github.com/wieslawsoltes/RibbonSpace/blob/main/samples/RibbonSpace.Demo/Pages/WordPage.xaml); this extension uses Blazorise controls and event conventions.