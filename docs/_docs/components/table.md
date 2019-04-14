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

- `<TableContainer>` the main **container**
  - `<TableHeader>` the optional **top** part of the table
    - `<TableRow>` header **row**
      - `<TableHeaderCell>` a header **cell**
  - `<TableFooter>` the optional **bottom** part of the table
  - `<TableBody>` the main **content** of the table
    - `<TableRow>` each table **row**
      - `<TableRowHeader>` a table cell **heading**
      - `<TableRowCell>` a table **cell**

**Note:** TableContainer will be renamed to Table once the Razor [issue](https://github.com/aspnet/AspNetCore/issues/5550) is resolved.
{: .notice--info}

## Example

### Simple

```html
<TableContainer>
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
</TableContainer>
```

<iframe src="/examples/table/basic/" frameborder="0" scrolling="no" style="width:100%;height:200px;"></iframe>

### Striped

```html
<TableContainer IsStriped="true">
    ...
</TableContainer>
```

<iframe src="/examples/table/striped/" frameborder="0" scrolling="no" style="width:100%;height:200px;"></iframe>

### Hoverable

```html
<TableContainer IsHoverable="true">
    ...
</TableContainer>
```

<iframe src="/examples/table/hoverable/" frameborder="0" scrolling="no" style="width:100%;height:200px;"></iframe>

### Bordered

```html
<TableContainer IsBordered="true">
    ...
</TableContainer>
```

<iframe src="/examples/table/bordered/" frameborder="0" scrolling="no" style="width:100%;height:200px;"></iframe>

## Attributes

| Name        | Type    | Default | Description                      |
|-------------|---------|---------|----------------------------------|
| IsFullWidth | boolean | false   | You can have a full width table. |
| IsStriped   | boolean | false   | Adds stripes to the table.       |
| IsBordered  | boolean | false   | Adds borders to all the cells.   |
| IsHoverable | boolean | false   | Adds a hover effect on each row. |