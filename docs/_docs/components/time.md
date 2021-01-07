---
title: "Time component"
permalink: /docs/components/time/
excerpt: "Learn how to use time component."
toc: true
toc_label: "Guide"
---

## Time

A native time field example with `type="time"`.

```html
<TimeEdit TValue="TimeSpan?" />
```

<iframe src="/examples/forms/time/" frameborder="0" scrolling="no" style="width:100%;height:55px;"></iframe>

## Usage

### With bind attribute

By using `bind-*` attribute the selected time will be automatically assigned to the member variable.

```html
<TimeEdit TValue="TimeSpan?" @bind-Time="@selectedTime" />

@code{
    TimeSpan? selectedTime;
}
```

### With event

When using the event `TimeChanged`, you also must define the `Time` value attribute.

```html
<TimeEdit TValue="TimeSpan?" Time="@selectedTime" TimeChanged="@OnTimeChanged" />

@code{
    TimeSpan? selectedTime;

    void OnTimeChanged( TimeSpan? Time )
    {
        selectedTime = Time;
    }
}
```

## Attributes

| Name          | Type                                                                       | Default      | Description                                                                                                                    |
|---------------|----------------------------------------------------------------------------|--------------|--------------------------------------------------------------------------------------------------------------------------------|
| Time          | TimeSpan?                                                                  | null         | Gets or sets the input time value.                                                                                             |
| TimeChanged   | event                                                                      |              | Occurs when the time has changed.                                                                                              |
| Pattern       | string                                                                     | null         | The pattern attribute specifies a regular expression that the input element's value is checked against on form submission.     |
| Placeholder   | string                                                                     | null         | Sets the placeholder for the empty time.                                                                                       |
| Autofocus     | `bool`                                                                     |  false       | Set's the focus to the component after the rendering is done.                                                                  |