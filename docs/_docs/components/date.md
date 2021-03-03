---
title: "Date component"
permalink: /docs/components/date/
excerpt: "Learn how to use date component."
toc: true
toc_label: "Guide"
---

## Date

A native date field example with `type="date"`.

```html
<DateEdit TValue="DateTime?" />
```

<iframe src="/examples/forms/date/" frameborder="0" scrolling="no" style="width:100%;height:55px;"></iframe>

## Usage

### With bind attribute

By using `bind-*` attribute the selected date will be automatically assigned to the member variable.

```html
<DateEdit TValue="DateTime?" @bind-Date="@selectedDate" />

@code{
    DateTime? selectedDate;
}
```

### With event

When using the event `DateChanged`, you also must define the `Date` value attribute.

```html
<DateEdit TValue="DateTime?" Date="@selectedDate" DateChanged="@OnDateChanged" />

@code{
    DateTime? selectedDate;

    void OnDateChanged( DateTime? date )
    {
        selectedDate = date;
    }
}
```

## DateTime

DateEdit component also support entering a time part. To enable it just set `InputMode` parameter.

```html
<DateEdit TValue="DateTime?" InputMode="DateInputMode.DateTime" />
```

<iframe src="/examples/forms/datetime/" frameborder="0" scrolling="no" style="width:100%;height:55px;"></iframe>

## Attributes

| Name          | Type                                                                       | Default      | Description                                                                                                                    |
|---------------|----------------------------------------------------------------------------|--------------|--------------------------------------------------------------------------------------------------------------------------------|
| Date          | `TValue`                                                                   | `default`    | Gets or sets the input date value.                                                                                             |
| DateChanged   | `EventCallback<TValue>`                                                    |              | Occurs when the date has changed.                                                                                              |
| InputMode     | [DateInputMode]({{ "/docs/helpers/enums/#dateinputmode" | relative_url }}) | `Date`       | Hints at the type of data that might be entered by the user while editing the element or its contents.                         |
| Min           | `DateTimeOffset?`                                                          | null         | The earliest date to accept.                                                                                                   |
| Max           | `DateTimeOffset?`                                                          | null         | The latest date to accept.                                                                                                     |
| Pattern       | `string`                                                                   | null         | The pattern attribute specifies a regular expression that the input element's value is checked against on form submission.     |
| Placeholder   | `string`                                                                   | null         | Sets the placeholder for the empty date.                                                                                       |
| Autofocus     | `bool`                                                                     |  false       | Set's the focus to the component after the rendering is done.                                                                  |