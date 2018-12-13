---
title: "Fields"
permalink: /docs/components/fields/
excerpt: "Field components."
toc: true
toc_label: "Components"
---

## Field

The Field is a container for:

- `Text`
- `Select`
- `Date`
- `Check`
- `Memo`
- `Button`

It is recomended to always place input components inside of field. That way you will keep the right spacing and arangement between input.

```html
<Field>
    <TextEdit Placeholder="Name" />
<Field>
```

<iframe class="frame" src="/examples/fields/basic/" frameborder="0" scrolling="no" style="width:100%;height:50px;"></iframe>

### With label

```html
<Field>
    <FieldLabel>Email address</FieldLabel>
    <TextEdit Placeholder="Enter email" />
<Field>
```

<iframe src="/examples/fields/field-label/" frameborder="0" scrolling="no" style="width:100%;height:80px;"></iframe>

### Horizontal field

```html
<Field IsHorizontal="true">
    <FieldLabel ColumnSize="ColumnSize.Is2">Url</FieldLabel>
    <TextEdit Role="TextRole.Url" ColumnSize="ColumnSize.Is10" />
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
        <SelectEdit>
        </SelectEdit>
    </Field>
    <Field ColumnSize="ColumnSize.Is2.OnDesktop">
        <FieldLabel>Zip</FieldLabel>
        <TextEdit />
    </Field>
</Fields>
```

<iframe class="frame" src="/examples/fields/fields/" frameborder="0" scrolling="no" style="width:100%;height:80px;"></iframe>