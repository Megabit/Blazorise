---
title: Create A Simple Kanban Board With Blazorise
description: In the blog, we build a simple kanban board to demonstrate the drag-and-drop capabilities of Blazorise components 
permalink: /blog/create-a-simple-kanban-board-with-blazor
canonical: /blog/create-a-simple-kanban-board-with-blazor
image-url: /img/blog/2022-09-13/Create_A_Simple_Kanban_Board_With_Blazorise.png
image-text: Create a simple blazor drag and drop kanban board
author-name: James Amattey
author-image: james
posted-on: September 13th, 2022
read-time: 6 min
---

Blazorise as a component library helps to create functionality that enhances user experience. In this short demo, we will demonstrate how Blazorise components can help you program drag-and-drop functionality into your applications in just a few lines of code. 

## Prerequisites
To complete this demo successfully, here are a few prerequisites

- Integrated Development Environment such as Visual Studio or Rider
- The .NET 6 SDK

## Assumptions
As a blog made for developers of various experiences, we want to assume that a reader has  
- Some knowledge of C#
- Knowledge of HTML, CSS
- Knowledge of the .NET ecosystem in the context of web development
- Knowledge of Blazor and its underlying concepts

## Setup
To get started, create a new [Blazor application](/blog/create-a-blazor-application). Blazor is a framework that allows you to build cross-platform client-side and server-side applications. For the purpose of the demo, we will create a [Blazor WebAssembly Application](https://blazorise.com/blog/what-is-blazor-wasm). 

The drag-and-drop functionality of our web application will be demonstrated using [Blazorise Components](https://blazorise.com/docs/components/dragdrop).

To begin, let's edit the sidebar with the [Blazorise Bar Component](/docs/components/bar). 

```html
@layout RootLayout
@inherits LayoutComponentBase

@if ( layoutType == "fixed-header" )
{
    <Layout Sider>
        <LayoutSider>
            <LayoutSiderContent>
                <SideMenu />
            </LayoutSiderContent>
        </LayoutSider>
        <Layout>
            <LayoutHeader Fixed>
                <TopMenu ThemeEnabledChanged="@OnThemeEnabledChanged"
                         ThemeGradientChanged="@OnThemeGradientChanged"
                         ThemeRoundedChanged="@OnThemeRoundedChanged"
                         ThemeColorChanged="@OnThemeColorChanged"
                         @bind-LayoutType="@layoutType" />
            </LayoutHeader>
            <LayoutContent Padding="Padding.Is4.OnX">
                @Body
            </LayoutContent>
        </Layout>
    </Layout>
}
else if ( layoutType == "fixed-header-footer-only" )
{
    <Layout>
        <LayoutHeader Fixed>
            <TopMenu ThemeEnabledChanged="@OnThemeEnabledChanged"
                     ThemeGradientChanged="@OnThemeGradientChanged"
                     ThemeRoundedChanged="@OnThemeRoundedChanged"
                     ThemeColorChanged="@OnThemeColorChanged"
                     @bind-LayoutType="@layoutType" />
        </LayoutHeader>
        <LayoutContent>
            @Body
        </LayoutContent>
        <LayoutFooter Fixed>
            <FooterMenu></FooterMenu>
        </LayoutFooter>
    </Layout>
}
else if ( layoutType == "sider-with-header-on-top" )
{
    <Layout>
        <LayoutHeader Fixed>
            <TopMenu ThemeEnabledChanged="@OnThemeEnabledChanged"
                     ThemeGradientChanged="@OnThemeGradientChanged"
                     ThemeRoundedChanged="@OnThemeRoundedChanged"
                     ThemeColorChanged="@OnThemeColorChanged"
                     @bind-LayoutType="@layoutType" />
        </LayoutHeader>
        <Layout Sider>
            <LayoutSider>
                <LayoutSiderContent>
                    <SideMenu />
                </LayoutSiderContent>
            </LayoutSider>
            <Layout>
                <LayoutContent Padding="Padding.Is4.OnX">
                    @Body
                </LayoutContent>
            </Layout>
        </Layout>
    </Layout>
}
 ```


## The Structure of A Kanban Board
Let's analyze the components that come together to form a Kanban Board. 


![image.png](https://cdn.hashnode.com/res/hashnode/image/upload/v1662979515435/SRMrlmJKi.png align="left")

A Kanban Board has columns that are used to group a set of related items. These could be work items, user stories, or tasks. 

Each task is placed on a card that can be moved across various columns depending on their status.  

The WIP count refers to the total number of work items that are being worked on at the same time. 

Kanban swimlanes split the board into sections. For example, if you have a development team, you can create sections for work items to be done by frontend and backend teams. 

For this demo, we want to demonstrate how you can create columns and cards and demonstrate the ability to move cards between columns.
Swimlanes and WIP are more advanced topics of Kanban and will not be the focus of this demo. 


## Creating The Kanban Board Page
Create a page for the Kanban Board and add a page directory to the razor file. Remember to add the link to the sidebar so that you can easily access it from the sidebar menu. 


### Defining the Container
The `<DropContainer>` element defines the area for the Kanban board. 

```html
<DropContainer TItem="DropItem" Items="@items" ItemsFilter="@((item, dropZone) => item.Group == dropZone)" ItemDropped="@itemsDropped" Flex="Flex.Wrap.Grow.Is1">
   
</DropContainer>
```
The container is where the multiple columns will be hosted.

### Defining the Columns
Kanban Columns are individual panels used to group related items of work. The column is defined as a `<DropZone>` as cards can be dropped into them.

```html
 <ChildContent>
        <DropZone TItem="DropItem" Name="To-Do" Border="Boarder.Rounded" Background="Background.Light" Padding="Padding.Is3" Margin="Margin.Is3" Flex="Flex.Grow.Is1">
            <Heading Size="HeadingSize.Is4" Margin="Margin.Is3.FromBottom">To Do</Heading>
        </DropZone>
    </ChildContent>

The above code block will create a column and name it To-Do. You can repeat the `<dropzone element>` code block to create as many columns as you want. 