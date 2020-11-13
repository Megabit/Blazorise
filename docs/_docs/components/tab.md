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
  - `<Items>` container for tab items
    - `<Tab>` clickable tab items
  - `<Content>` container for tab panels
    - `<TabPanel>` container for tab content

The tabs are container for tab items. Each tab item contains a link to a tab panel. The `Name` of each tab item should match the `Name` of a tab panel.

- `<TabsContent>` container for tab panels
  - `<TabPanel>` container for tab content

The tab content container is used to hold tab panels. Each content pane also has a unique `Name`, which is targeted by a link in the tab-strip.

Most of the time you will only need to use `Tabs` component as it is crafted to hold both clickable tab items and tab content. Only in the advanced scenario where the content will be separated from the tab items you will need to use `<TabsContent>` component.

So for a basic tabs when we put it all together, we get this:

## Example

```html
<Tabs SelectedTab="@selectedTab" SelectedTabChanged="@OnSelectedTabChanged">
    <Items>
        <Tab Name="home">Home</Tab>
        <Tab Name="profile">Profile</Tab>
        <Tab Name="messages">Messages</Tab>
        <Tab Name="settings">Settings</Tab>
    </Items>
    <Content>
        <TabPanel Name="home">
            Content for home.
        </TabPanel>
        <TabPanel Name="profile">
            Content for profile.
        </TabPanel>
        <TabPanel Name="messages">
            Content for messages.
        </TabPanel>
        <TabPanel Name="settings">
            Content for settings.
        </TabPanel>
    </Content>
</Tabs>
@code{
    string selectedTab = "profile";

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
| Pills               | boolean                                                                    | false            | Makes the tab items to appear as pills.                                                               |
| FullWidth           | boolean                                                                    | false            | Makes the tab items to extend the full available width.                                               |
| Justified           | boolean                                                                    | false            | Makes the tab items to extend the full available width, but every item will be the same width.        |
| TabPosition         | [TabPosition]({{ "/docs/helpers/enums/#tabposition" | relative_url }})     | Top              | Defines the placement of a tab items.                                                                 |
| SelectedTab         | string                                                                     |                  | Currently selected tab name.                                                                           |
| SelectedTabChanged  | event                                                                      |                  | Occurs after the selected tab has changed.                                                             |

### Tab

| Name                | Type                                                                       | Default          | Description                                                                                           |
|---------------------|----------------------------------------------------------------------------|------------------|-------------------------------------------------------------------------------------------------------|
| Name                | string                                                                     | null             | Defines the unique tab name.                                                                           |
| Clicked             | event                                                                      |                  | Occurs when the button is clicked. 
                                              |
| Disabled            | boolean                                                                    | false            | Prevents user interactions and make it appear lighter.  
                                              |

### TabsContent

| Name                | Type                                                                       | Default          | Description                                                                                           |
|---------------------|----------------------------------------------------------------------------|------------------|-------------------------------------------------------------------------------------------------------|
| SelectedPanel       | string                                                                     |                  | Currently selected panel name.                                                                         |
| SelectedPanelChanged | event                                                                     |                  | Occurs after the selected panel has changed.                                                           |

### TabPanel

| Name                | Type                                                                       | Default          | Description                                                                                           |
|---------------------|----------------------------------------------------------------------------|------------------|-------------------------------------------------------------------------------------------------------|
| Name                | string                                                                     | null             | Defines the panel name that must match the corresponding tab name.                                     |
