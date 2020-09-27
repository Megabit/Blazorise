---
title: "Field component"
permalink: /docs/components/field/
excerpt: "Field components."
toc: true
toc_label: "Guide"
redirect_from: /docs/components/fields/
---

## Field

The Field is a container for `Text`, `Select`, `Date`, `Check`, `Memo`, and optionally for `Button`. Structure is very simple:

- `Field` the main container
  - `FieldLabel` a field label
  - `FieldBody` used only for _horizontal_ fields
  - `FieldHelp` small text bellow the field
- `Fields` container used to group several `Field` components

It is recommended to always place input components inside of a field. That way you will keep the right spacing and arrangement between input controls.

### Basic example

```html
<Field>
    <TextEdit Placeholder="Name" />
</Field>
```

<iframe class="frame" src="/examples/fields/basic/" frameborder="0" scrolling="no" style="width:100%;height:50px;"></iframe>

### With label

```html
<Field>
    <FieldLabel>Email address</FieldLabel>
    <TextEdit Placeholder="Enter email" />
</Field>
```

<iframe src="/examples/fields/field-label/" frameborder="0" scrolling="no" style="width:100%;height:80px;"></iframe>

### With help
```html
<Field>
    <FieldLabel>Email address</FieldLabel>
    <TextEdit Placeholder="Enter email">
        <FieldHelp>Please enter a valid email address</FieldHelp>
    </TextEdit>
</Field>
```

<iframe src="/examples/fields/field-help/" frameborder="0" scrolling="no" style="width:100%;height:105px;"></iframe>

### Horizontal field

When using horizontal field you must place input controls inside of the `FieldBody` tag.

```html
<Field Horizontal="true">
    <FieldLabel ColumnSize="ColumnSize.Is2">Name</FieldLabel>
    <FieldBody ColumnSize="ColumnSize.Is10">
        <TextEdit Placeholder="Some text value..." />
    </FieldBody>
</Field>
```

<iframe class="frame" src="/examples/fields/field-horizontal/" frameborder="0" scrolling="no" style="width:100%;height:50px;"></iframe>

### Visibility

Use `Visibility` attribute to hide a field while still preserving it's space.

```html
<Field Visibility="Visibility.Never">
    <TextEdit />
</Field>
```

## Fields

`Fields` component is used to group multiple `Field` components. For example if you need to group fields into columns you must use fields component.

```html
<Fields>
    <Field ColumnSize="ColumnSize.Is6.OnDesktop">
        <FieldLabel>City</FieldLabel>
        <TextEdit />
    </Field>
    <Field ColumnSize="ColumnSize.Is4.OnDesktop">
        <FieldLabel>State</FieldLabel>
        <Select>
            ...
        </Select>
    </Field>
    <Field ColumnSize="ColumnSize.Is2.OnDesktop">
        <FieldLabel>Zip</FieldLabel>
        <TextEdit />
    </Field>
</Fields>
```

<iframe class="frame" src="/examples/fields/fields/" frameborder="0" scrolling="no" style="width:100%;height:80px;"></iframe>

## Attributes

### Field

| Name            | Type                                                                              | Default   | Description                                                                                                             |
|-----------------|-----------------------------------------------------------------------------------|-----------|-------------------------------------------------------------------------------------------------------------------------|
| Horizontal      | boolean                                                                           | false     | Aligns the controls for horizontal form.                                                                                |
| ColumnSize      | [ColumnSize]({{ "/docs/helpers/utilities/#columnsize" | relative_url }})          | null      | Determines how much space will be used by the field inside of the grid row.                                             |
| JustifyContent  | [JustifyContent]({{ "/docs/helpers/enums/#justifycontent" | relative_url }})      | `None`    | Aligns the flexible container's items when the items do not use all available space on the main-axis (horizontally).    |

### FieldLabel

| Name            | Type                                                                              | Default   | Description                                                                                                             |
|-----------------|-----------------------------------------------------------------------------------|-----------|-------------------------------------------------------------------------------------------------------------------------|
| Screenreader    | [Screenreader]({{ "/docs/helpers/enums/#screenreader" | relative_url }})          | `Always`  | Defines the visibility for screen readers.                                                                              |

### Fields

| Name            | Type                                                                              | Default   | Description                                                                                                             |
|-----------------|-----------------------------------------------------------------------------------|-----------|-------------------------------------------------------------------------------------------------------------------------|
| Label           | string                                                                            | false     | Sets the field group label.                                                                                             |
| Help            | string                                                                            | false     | Sets the field group  help-text positioned bellow the field.                                                            |
| ColumnSize      | [ColumnSize]({{ "/docs/helpers/utilities/#columnsize" | relative_url }})          | null      | Determines how much space will be used by the field inside of the grid row.                                             |