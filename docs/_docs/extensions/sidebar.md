---
title: "Sidebar extension"
permalink: /docs/extensions/sidebar/
excerpt: "Learn how to use Sidebar component."
toc: true
toc_label: "Guide"
---

## Overview

The Sidebar component is an expandable and collapsible container area that holds primary and secondary information placed alongside the main content of a webpage.

## Structure

The sidebar extension is defined of several different components:

- `Sidebar` main sidebar component
  - `SidebarContent` container for the sidebar brand and navigation
    - `SidebarBrand` brand logo or link located in the sidebar header
    - `SidebarNavigation` container for the sidebar navigation items
      - `SidebarLabel` simple label to separate navigation items
      - `SidebarItem` navigation item that holds the link or subitems
        - `SidebarLink` navigation link
        - `SidebarSubItem` container for sidebar child items

## Installation

### NuGet

Install sidebar extension from NuGet.

```
Install-Package Blazorise.Sidebar
```

### Imports

In your main _Imports.razor_ add:

```cs
@using Blazorise.Sidebar
```

### Static files

Include CSS link into your `index.html` or `_Host.cshtml` file, depending if you're using a Blazor WebAssembly or Blazor Server side project.

```html
<link href="_content/Blazorise.Sidebar/blazorise.sidebar.css" rel="stylesheet" />
```

## Usage

When defining a sidebar structure you can chose between manual or dynamic building. Please note that you cannot combine both of them so you have to chose the one that suits you best.

### Manual

When building your sidebar manually you have full control of it's content and navigation item. You can combine every sidebar component as you wish.

```cs
<Sidebar @ref="sidebar">
    <SidebarContent>
        <SidebarBrand>
            <a href="">Blazorise Sidebar</a>
        </SidebarBrand>
        <SidebarNavigation>
            <SidebarLabel>Main</SidebarLabel>
            <SidebarItem>
                <SidebarLink To="" Title="Home">
                    <Icon Name="IconName.Home" Margin="Margin.Is3.FromRight" />Home
                </SidebarLink>
            </SidebarItem>
            <SidebarItem>
                <SidebarLink Toggled="(isOpen)=> mailSidebarSubItems.Toggle(isOpen)" IsShow="true">
                    <Icon Name="IconName.Mail" Margin="Margin.Is3.FromRight" />Email
                </SidebarLink>
                <SidebarSubItem @ref="mailSidebarSubItems" IsShow="true">
                    <SidebarItem>
                        <SidebarLink To="email/inbox">Inbox</SidebarLink>
                    </SidebarItem>
                    <SidebarItem>
                        <SidebarLink To="email/compose">Compose Email</SidebarLink>
                    </SidebarItem>
                    @* other subitems *@
                </SidebarSubItem>
            </SidebarItem>
            <SidebarItem>
                <SidebarLink Toggled="(isOpen)=> appsSidebarSubItems.Toggle(isOpen)" IsShow="true">
                    <Icon Name="IconName.Smartphone" Margin="Margin.Is3.FromRight" />Apps
                </SidebarLink>
                <SidebarSubItem @ref="appsSidebarSubItems" IsShow="true">
                    <SidebarItem>
                        <SidebarLink To="apps/todo">Todo List</SidebarLink>
                    </SidebarItem>
                </SidebarSubItem>
            </SidebarItem>
        </SidebarNavigation>
    </SidebarContent>
</Sidebar>

@code{
    Sidebar sidebar;
    SidebarSubItem mailSidebarSubItems;
    SidebarSubItem appsSidebarSubItems;

    void ToggleSidebar()
    {
        sidebar.Toggle();
    }
}
```

### Dynamic

You can also build sidebar dynamically by using the `Data` attribute and the `SidebarInfo` class. The `SidebarInfo` is fully serializable so you can save it to an external source or database.

```cs
<Sidebar Data="@sidebarInfo" />

@code{
    Sidebar sidebar;
    
    SidebarInfo sidebarInfo = new SidebarInfo
    {
        Brand = new SidebarBrandInfo
        {
            Text = "Blazorise Demo"
        },
        Items = new List<SidebarItemInfo>
        {
            new SidebarItemInfo { To = "", Text = "Dashboard" },
            new SidebarItemInfo
            {
                Text = "Email",
                Icon = IconName.Mail,
                SubItems = new List<SidebarItemInfo>
                {
                    new SidebarItemInfo { To = "email/inbox", Text = "Inbox" },
                    new SidebarItemInfo { To = "email/compose", Text = "Compose Email" },
                }
            },
            new SidebarItemInfo
            {
                Text = "Applications",
                SubItems = new List<SidebarItemInfo>
                {
                    new SidebarItemInfo { To = "apps/todo", Text = "Todo List" }
                }
            },
        }
    };
}
```
