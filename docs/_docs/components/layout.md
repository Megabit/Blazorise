---
title: "Layout component"
permalink: /docs/components/layout/
excerpt: "Learn how to use layout components."
toc: true
toc_label: "Guide"
---

Handle the overall layout of a page.

## Structure

- `Layout` The main layout container
  - `LayoutHeader` The top container for a Bar or navigation
  - `LayoutContent` The main content container
  - `LayoutFooter` The bottom layout 
  - `LayoutSider` The sidebar container
    - `LayoutSiderContent` Main content for sider component


## Usage

### Basic example

```html
<Layout>
    <LayoutHeader>
        Header
    </LayoutHeader>
    <LayoutContent>
        Content
    </LayoutContent>
    <LayoutFooter>
        Footer
    </LayoutFooter>
</Layout>
```

<iframe src="/examples/layout/basic/" frameborder="0" scrolling="no" style="width:100%;height:310px;"></iframe>

### With Sider

```html
<Layout Sider="true">
    <LayoutSider>
        <LayoutSiderContent>
            Sider
        </LayoutSiderContent>
    </LayoutSider>
    <Layout>
        <LayoutHeader Fixed="true">
            Header
        </LayoutHeader>
        <LayoutContent>
            Content
        </LayoutContent>
    </Layout>
</Layout>
```

<iframe src="/examples/layout/with-sider/" frameborder="0" scrolling="no" style="width:100%;height:310px;"></iframe>

## Attributes

### Layout

| Name              | Type      | Default | Description                                                                                          |
|-------------------|-----------|---------|------------------------------------------------------------------------------------------------------|
| Sider             | bool      | false   | Indicates that layout will contain sider.                                                            |

### LayoutHeader

| Name              | Type      | Default | Description                                                                                          |
|-------------------|-----------|---------|------------------------------------------------------------------------------------------------------|
| Fixed             | bool      | false   | If true header will be fixed to the top of the page.                                                 |