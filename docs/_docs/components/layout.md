---
title: 'Layout component'
permalink: /docs/components/layout/
excerpt: 'Learn how to use layout components.'
toc: true
toc_label: 'Guide'
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

### Sider with Header on top

```html
<Layout>
  <LayoutHeader Fixed="true">
    Header
  </LayoutHeader>
  <Layout Sider="true">
    <LayoutSider>
      <LayoutSiderContent>
        Sider
      </LayoutSiderContent>
    </LayoutSider>
    <Layout>
      <LayoutContent>
        Content
      </LayoutContent>
    </Layout>
  </Layout>
</Layout>
```

<iframe src="/examples/layout/header-on-top-with-sider/" frameborder="0" scrolling="no" style="width:100%;height:310px;"></iframe>

## Attributes

### Layout

| Name    | Type | Default | Description                                                                               |
| ------- | ---- | ------- | ----------------------------------------------------------------------------------------- |
| Sider   | bool | false   | Indicates that layout will contain sider.                                                 |
| Loading | bool | false   | If true, an overlay will be created so the user cannot click anything until set to false. |

### LayoutHeader

| Name  | Type | Default | Description                                          |
| ----- | ---- | ------- | ---------------------------------------------------- |
| Fixed | bool | false   | If true header will be fixed to the top of the page. |

### LayoutFooter

| Name  | Type | Default | Description                                             |
| ----- | ---- | ------- | ------------------------------------------------------- |
| Fixed | bool | false   | If true footer will be fixed to the bottom of the page. |
