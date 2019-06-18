---
title: "Date component"
permalink: /docs/components/date/
excerpt: "Learn how to use date component."
toc: true
toc_label: "Guide"
---

## Date

```html
<DateEdit />
```

<iframe src="/examples/forms/date/" frameborder="0" scrolling="no" style="width:100%;height:55px;"></iframe>

## Usage

### With bind attribute

By using `bind-*` attribute the selected date will be automatically assigned to the member variable.

```html
<DateEdit bind-Date="@selectedDate" />

@code{
    DateTime? selectedDate;
}
```

### With event

When using the event `DateChanged`, you also must define the `Date` value attribute.

```html
<DateEdit Date="@selectedDate" DateChanged="@OnDateChanged" />

@code{
    DateTime? selectedDate;

    void OnDateChanged( DateTime? date )
    {
        selectedDate = date;
    }
}
```