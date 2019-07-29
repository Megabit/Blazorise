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
<DateEdit @bind-Date="@selectedDate" />

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

## Attributes

| Name          | Type                                                                       | Default      | Description                                                                                                                    |
|---------------|----------------------------------------------------------------------------|--------------|--------------------------------------------------------------------------------------------------------------------------------|
| Date          | DateTime?                                                                  | null         | Gets or sets the input date value.                                                                                             |
| DateChanged   | event                                                                      |              | Occurs when the date has changed.                                                                                              |
| Min           | DateTime?                                                                  | null         | The earliest date to accept.                                                                                                   |
| Max           | DateTime?                                                                  | null         | The latest date to accept.                                                                                                     |
| Pattern       | string                                                                     | null         | The pattern attribute specifies a regular expression that the input element's value is checked against on form submission.     |
| Placeholder   | string                                                                     | null         | Sets the placeholder for the empty date.                                                                                       |