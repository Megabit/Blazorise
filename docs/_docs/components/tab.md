---
title: "Tabs"
permalink: /docs/components/tab/
excerpt: "Learn how to use tab components."
toc: true
toc_label: "Guide"
redirect_from: /docs/components/tabs/
---

## Basics

There are two pieces to a tabbed interface: the tabs themselves, and the content for each tab. 

- `<Tabs>` container for Tab items
  - `<Tab>` clickable tab items
- `<TabsContent>` container for tab panels
  - `<TabPanel>` container for tab content

The tabs are container for tab items. Each tab item contains a link to a tab panel. The `Name` of each tab item should match the `Name` of a tab panel.

The tab content container is used to hold tab panels. Each content pane also has a unique `Name`, which is targeted by a link in the tab-strip.

Put it all together, and we get this:

## Example

```html
<Tabs SelectedTab="@selectedTab" SelectedTabChanged="@OnSelectedTabChanged">
    <Tab Name="home">Home</Tab>
    <Tab Name="profile">Profile</Tab>
    <Tab Name="messages">Messages</Tab>
    <Tab Name="settings">Settings</Tab>
</Tabs>
<TabsContent SelectedPanel="@selectedTab">
    <TabPanel Name="home">
        ...
    </TabPanel>
    <TabPanel Name="profile">
        ...
    </TabPanel>
    <TabPanel Name="messages">
        ...
    </TabPanel>
    <TabPanel Name="settings">
        ...
    </TabPanel>
</TabsContent>
@code{
    string selectedTab = "2";

    private void OnSelectedTabChanged( string name )
    {
        selectedTab = name;
    }
}
```

<iframe src="/examples/tabs/basic/" frameborder="0" scrolling="no" style="width:100%;height:260px;"></iframe>

## Functions

| Name                    | Description                                                                                 |
|-------------------------|---------------------------------------------------------------------------------------------|
| SelectTab(string name)  | Sets the active tab by the name.                                                            |

## Attributes

### Tabs

| Name                | Type                                                                       | Default          | Description                                                                                           |
|---------------------|----------------------------------------------------------------------------|------------------|-------------------------------------------------------------------------------------------------------|
| IsPills             | boolean                                                                    | false            | Makes the tab items to appear as pills.                                                               |
| IsFullWidth         | boolean                                                                    | false            | Makes the tab items to extend the full available width.                                               |
| IsJustified         | boolean                                                                    | false            | Makes the tab items to extend the full available width, but every item will be the same width.        |
| IsVertical          | boolean                                                                    | false            | Stack the navigation items by changing the flex item direction.                                       |
| SelectedTabChanged  | event                                                                      |                  | Occurs after the selected tab has changed.                                                            |

### Tab

| Name                | Type                                                                       | Default          | Description                                                                                           |
|---------------------|----------------------------------------------------------------------------|------------------|-------------------------------------------------------------------------------------------------------|
| Name                | string                                                                     | null             | Defines the unique tab name.                                                                          |

### TabsContent

| Name                | Type                                                                       | Default          | Description                                                                                           |
|---------------------|----------------------------------------------------------------------------|------------------|-------------------------------------------------------------------------------------------------------|
| SelectedPanelChanged | event                                                                     |                  | Occurs after the selected panel has changed.                                                          |

### TabPanel

| Name                | Type                                                                       | Default          | Description                                                                                           |
|---------------------|----------------------------------------------------------------------------|------------------|-------------------------------------------------------------------------------------------------------|
| Name                | string                                                                     | null             | Defines the panel name that must match the corresponding tab name.                                    |