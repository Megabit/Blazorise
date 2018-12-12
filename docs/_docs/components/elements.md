---
title: "Elements"
permalink: /docs/components/elements/
excerpt: "List of smaller elements."
toc: true
toc_label: "Components"
---

## Badge

Simply set the Color attribute and you're good to go.

```html
<Badge Color="Color.Primary">Primary</Badge>
<Badge Color="Color.Secondary">Secondary</Badge>
```

## Pagination

```html
<Pagination>
    <PaginationItem>
        <PaginationLink>
            <span aria-hidden="true">«</span>
        </PaginationLink>
    </PaginationItem>
    <PaginationItem>
        <PaginationLink>
            1
        </PaginationLink>
    </PaginationItem>
    <PaginationItem>
        <PaginationLink>
            2
        </PaginationLink>
    </PaginationItem>
    <PaginationItem>
        <PaginationLink>
            3
        </PaginationLink>
    </PaginationItem>
    <PaginationItem>
        <PaginationLink>
            <span aria-hidden="true">»</span>
        </PaginationLink>
    </PaginationItem>
</Pagination>
```

## Alert

```html
<Alert Color="Color.Success">
    <strong>Well done!</strong> You successfully read this important alert message.
</Alert>
```

### Usage

To show alert just set `IsShow` attribute to true.

```html
<Alert Color="Color.Success" IsShow="true">
    ...
</Alert>
```

or programatically

```cs
<Alert ref="myAlert" Color="Color.Success">
    ...
</Alert>

@functions{
    Alert myAlert;

    void ButtonClick()
    {
        myAlert.Show();
    }
}
```

### Close

You can also add a close button.

```html
<Alert Color="Color.Success">
    Lorem ipsum dolor sit amet, consectetur adipisicing elit. <strong>Alert Link.</strong>
    <CloseButton />
</Alert>
```