---
title: "Pagination component"
permalink: /docs/components/pagination/
excerpt: "Learn how to use pagination components."
toc: true
toc_label: "Guide"
---

## Basics

- `<Pagination>`
  - `<PaginationItem>`
    - `<PaginationLink>`

## Example

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

## Attributes

| Name          | Type                                                                       | Default          | Description                                                                                 |
|---------------|----------------------------------------------------------------------------|------------------|---------------------------------------------------------------------------------------------|
| Size          | [Size]({{ "/docs/helpers/sizes/#size" | relative_url }})                   | `None`           | Gets or sets the pagination size.                                                           |
| Alignment     | [Alignment]({{ "/docs/helpers/enums/#alignment" | relative_url }})         | `None`           | Gets or sets the pagination alignment.                                                      |
| Background    | [Background]({{ "/docs/helpers/colors/#background" | relative_url }})      | `None`           | Gets or sets the pagination background color.                                               |