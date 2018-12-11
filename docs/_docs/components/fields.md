---
title: "Fields"
permalink: /docs/components/fields/
excerpt: "Field components."
toc: true
---

## Field

Field should be used as a group container for all input fields and labels.

```html
<Field>
    <FieldLabel>Name</FieldLabel>
    <TextEdit Placeholder="Name" />
</Field>
```

### Horizontal field

```html
<Field IsHorizontal="true">
    <FieldLabel ColumnSize="ColumnSize.Is2">Url</FieldLabel>
    <TextEdit Role="TextRole.Url" ColumnSize="ColumnSize.Is10" />
</Field>
```

### Visibility

Use `Visibility` attribute to hide a field while still preserving it's space.

```html
<Field Visibility="Visibility.Never">
    <TextEdit />
</Field>
```

## Fields

`Fields` is used to group multiple `Field` components. For example if you need to group fields into columns you must use fields component.

```html
<Fields>
    <Field ColumnSize="ColumnSize.Is6.OnDesktop">
        <FieldLabel>City</FieldLabel>
        <TextEdit />
    </Field>
    <Field ColumnSize="ColumnSize.Is4.OnDesktop">
        <FieldLabel>State</FieldLabel>
        <SelectEdit>
        </SelectEdit>
    </Field>
    <Field ColumnSize="ColumnSize.Is2.OnDesktop">
        <FieldLabel>Zip</FieldLabel>
        <TextEdit />
    </Field>
</Fields>
```