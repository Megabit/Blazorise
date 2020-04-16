---
title: "Table"
permalink: /docs/components/table/
excerpt: "Learn how to use table components."
toc: true
toc_label: "Guide"
redirect_from: /docs/components/table/
---

## Basics

Table displays information in a way that’s easy to scan, so that users can look for patterns and insights. They can be embedded in primary content, such as cards.

## Structure

- `<Table>` the main **container**
  - `<TableHeader>` the optional **top** part of the table
    - `<TableRow>` header **row**
      - `<TableHeaderCell>` a header **cell**
  - `<TableFooter>` the optional **bottom** part of the table
  - `<TableBody>` the main **content** of the table
    - `<TableRow>` each table **row**
      - `<TableRowHeader>` a table cell **heading**
      - `<TableRowCell>` a table **cell**

## Example

### Simple

```html
<Table>
    <TableHeader>
        <TableRow>
            <TableHeaderCell>#</TableHeaderCell>
            <TableHeaderCell>First Name</TableHeaderCell>
            <TableHeaderCell>Last Name</TableHeaderCell>
            <TableHeaderCell>Username</TableHeaderCell>
        </TableRow>
    </TableHeader>
    <TableBody>
        <TableRow>
            <TableRowHeader>1</TableRowHeader>
            <TableRowCell>Mark</TableRowCell>
            <TableRowCell>Otto</TableRowCell>
            <TableRowCell>@@mdo</TableRowCell>
        </TableRow>
        <TableRow>
            <TableRowHeader>2</TableRowHeader>
            <TableRowCell>Jacob</TableRowCell>
            <TableRowCell>Thornton</TableRowCell>
            <TableRowCell>@@fat</TableRowCell>
        </TableRow>
        <TableRow>
            <TableRowHeader>3</TableRowHeader>
            <TableRowCell>Larry</TableRowCell>
            <TableRowCell>the Bird</TableRowCell>
            <TableRowCell>@@twitter</TableRowCell>
        </TableRow>
    </TableBody>
</Table>
```

<iframe src="/examples/table/basic/" frameborder="0" scrolling="no" style="width:100%;height:200px;"></iframe>

### Striped

```html
<Table Striped="true">
    ...
</Table>
```

<iframe src="/examples/table/striped/" frameborder="0" scrolling="no" style="width:100%;height:200px;"></iframe>

### Hoverable

```html
<Table Hoverable="true">
    ...
</Table>
```

<iframe src="/examples/table/hoverable/" frameborder="0" scrolling="no" style="width:100%;height:200px;"></iframe>

### Bordered

```html
<Table Bordered="true">
    ...
</Table>
```

<iframe src="/examples/table/bordered/" frameborder="0" scrolling="no" style="width:100%;height:200px;"></iframe>

### Borderless

```html
<Table Borderless="true">
    ...
</Table>
```
<iframe src="/examples/table/borderless/" frameborder="0" scrolling="no" style="width:100%;height:200px;"></iframe>

### Small table

```html
<Table Narrow="true">
    ...
</Table>
```
<iframe src="/examples/table/narrowed/" frameborder="0" scrolling="no" style="width:100%;height:200px;"></iframe>

### Light header

```html
<Table>
  <TableHeader ThemeContrast="ThemeContrast.Light">
     ...
  </TableHeader>
    ...
</Table>
```
<iframe src="/examples/table/head-light/" frameborder="0" scrolling="no" style="width:100%;height:200px;"></iframe>

### Dark header

```html
<Table>
  <TableHeader ThemeContrast="ThemeContrast.Dark">
     ...
  </TableHeader>
    ...
</Table>
```
<iframe src="/examples/table/head-dark/" frameborder="0" scrolling="no" style="width:100%;height:200px;"></iframe>

## Attributes

| Name         | Type    | Default | Description                                                   |
|--------------|---------|---------|---------------------------------------------------------------|
| FullWidth    | boolean | false   | Makes the table to fill entire horizontal space.              |
| Striped      | boolean | false   | Adds stripes to the table.                                    |
| Bordered     | boolean | false   | Adds borders to all the cells.                                |
| Hoverable    | boolean | false   | Adds a hover effect on each row.                              |
| Borderless   | boolean | false   | Table without any borders.                                    |
| Narrow       | boolean | false   | Makes the table more compact by cutting cell padding in half. |