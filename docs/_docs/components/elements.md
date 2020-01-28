---
title: "Elements"
permalink: /docs/components/elements/
excerpt: "List of smaller elements like Badge, Pagination and Alert."
toc: true
toc_label: "Guide"
---

## Badge

Simply set the Color attribute and you're good to go.

```html
<Badge Color="Color.Primary">Primary</Badge>
<Badge Color="Color.Secondary">Secondary</Badge>
```

<iframe class="frame" src="/examples/elements/badge/" frameborder="0" scrolling="no" style="width:100%;height:35px;"></iframe>

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

<iframe class="frame" src="/examples/elements/pagination/" frameborder="0" scrolling="no" style="width:100%;height:50px;"></iframe>

## Breadcrumb

```html
<Breadcrumb>
    <BreadcrumbItem>
        <BreadcrumbLink To="#">Home</BreadcrumbLink>
    </BreadcrumbItem>
    <BreadcrumbItem>
        <BreadcrumbLink To="#">Library</BreadcrumbLink>
    </BreadcrumbItem>
    <BreadcrumbItem Active="true">
        <BreadcrumbLink To="#">Data</BreadcrumbLink>
    </BreadcrumbItem>
</Breadcrumb>
```

<iframe class="frame" src="/examples/elements/breadcrumb/" frameborder="0" scrolling="no" style="width:100%;height:50px;"></iframe>

## Alert

```html
<Alert Color="Color.Success">
    <strong>Well done!</strong> You successfully read this important alert message.
</Alert>
```

<iframe class="frame" src="/examples/elements/alert/" frameborder="0" scrolling="no" style="width:100%;height:60px;"></iframe>

### With close

You can also add a close button.

```html
<Alert Color="Color.Success">
    Lorem ipsum dolor sit amet, consectetur adipisicing elit. <strong>Alert Link.</strong>
    <CloseButton />
</Alert>
```

<iframe class="frame" src="/examples/elements/alert-close/" frameborder="0" scrolling="no" style="width:100%;height:60px;"></iframe>

### With content

You can also add a close button.

```html
<Alert Color="Color.Info" Visible="true">
    <Heading Size="HeadingSize.Is4" TextColor="TextColor.Success">
        Big one!
        <CloseButton />
    </Heading>
    <Paragraph>
        Lorem ipsum dolor sit amet, consectetur adipisicing elit. Duis mollis, est non commodo luctus, nisi erat porttitor ligula, eget lacinia odio sem nec elit. Cras mattis consectetur purus sit amet fermentum.
    </Paragraph>
    <Paragraph>
        <Button Color="Color.Info">Wanna do this</Button>
        <Button Color="Color.Light">Or do this</Button>
    </Paragraph>
</Alert>
```

<iframe class="frame" src="/examples/elements/alert-close-big/" frameborder="0" scrolling="no" style="width:100%;height:215px;"></iframe>

### How to use

To show alert just set `Visible` attribute to true.

```html
<Alert Color="Color.Success" Visible="true">
    ...
</Alert>
```

or programmatically

```cs
<Alert @ref="myAlert" Color="Color.Success">
    ...
</Alert>

@code{
    Alert myAlert;

    void ButtonClick()
    {
        myAlert.Show();
    }
}
```