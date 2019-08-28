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
<Table IsStriped="true">
    ...
</Table>
```

<iframe src="/examples/table/striped/" frameborder="0" scrolling="no" style="width:100%;height:200px;"></iframe>

### Hoverable

```html
<Table IsHoverable="true">
    ...
</Table>
```

<iframe src="/examples/table/hoverable/" frameborder="0" scrolling="no" style="width:100%;height:200px;"></iframe>

### Bordered

```html
<Table IsBordered="true">
    ...
</Table>
```

<iframe src="/examples/table/bordered/" frameborder="0" scrolling="no" style="width:100%;height:200px;"></iframe>

### Borderless

```html
<Table IsBorderless="true">
    ...
</Table>
```
<iframe src="/examples/table/borderless/" frameborder="0" scrolling="no" style="width:100%;height:200px;"></iframe>

### Small table

```html
<Table IsNarrow="true">
    ...
</Table>
```
<iframe src="/examples/table/narrowed/" frameborder="0" scrolling="no" style="width:100%;height:200px;"></iframe>

### Light header

```html
<Table Theme="Theme.Light">
    ...
</Table>
```
<iframe src="/examples/table/head-light/" frameborder="0" scrolling="no" style="width:100%;height:200px;"></iframe>

### Dark header

```html
<Table Theme="Theme.Dark">
    ...
</Table>
```
<iframe src="/examples/table/head-dark/" frameborder="0" scrolling="no" style="width:100%;height:200px;"></iframe>

## Attributes

| Name         | Type    | Default | Description                                                   |
|--------------|---------|---------|---------------------------------------------------------------|
| IsFullWidth  | boolean | false   | Makes the table to fill entire horizontal space.              |
| IsStriped    | boolean | false   | Adds stripes to the table.                                    |
| IsBordered   | boolean | false   | Adds borders to all the cells.                                |
| IsHoverable  | boolean | false   | Adds a hover effect on each row.                              |
| IsBorderless | boolean | false   | Table without any borders.                                    |
| IsNarrow     | boolean | false   | Makes the table more compact by cutting cell padding in half. |