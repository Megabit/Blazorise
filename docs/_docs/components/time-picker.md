---
title: "TimePicker component"
permalink: /docs/components/time-picker/
excerpt: "Learn how to use TimePicker component."
toc: true
toc_label: "Guide"
---

## Time

TimePicker is based on a [flatpickr](https://flatpickr.js.org/) time picker and as such is not a native time input element. That means that unlike `TimeEdit` which will render `type="time"`, `TimePicker` will render `type="text"` in the DOM.

### Simple usage

```html
<TimePicker TValue="TimeSpan?" />
```

## Add icon

To add icon you can combine `TimePicker` with an `Addon`.

```html
<Addons>
    <Addon AddonType="AddonType.Body">
        <TimePicker @ref="@timePicker" TValue="TimeSpan?" />
    </Addon>
    <Addon AddonType="AddonType.End">
        <Button Color="Color.Light" Clicked="@(()=>timePicker.ToggleAsync())">
            <Icon Name="IconName.CalendarDay" />
        </Button>
    </Addon>
</Addons>
@code{
    TimePicker<TimeSpan?> timePicker;
}
```

## Attributes

| Name              | Type                                                                       | Default      | Description                                                                                                                    |
|-------------------|----------------------------------------------------------------------------|--------------|--------------------------------------------------------------------------------------------------------------------------------|
| Time              | `TValue`                                                                   | `default`    | Gets or sets the input time value.                                                                                             |
| Time              | `EventCallback<TValue>`                                                    |              | Occurs when the time has changed.                                                                                              |
| Min               | `TimeSpan?`                                                                | null         | The earliest time to accept.                                                                                                   |
| Max               | `TimeSpan?`                                                                | null         | The latest time to accept.                                                                                                     |
| Pattern           | `string`                                                                   | null         | The pattern attribute specifies a regular expression that the input element's value is checked against on form submission.     |
| Placeholder       | `string`                                                                   | null         | Sets the placeholder for the empty time.                                                                                       |
| DisplayFormat     | `string`                                                                   |  null        | Defines the display format of the time.                                                                                        |
| TimeAs24hr        | `bool  `                                                                   |  false       | Displays time picker in 24 hour mode without AM/PM selection when enabled.                                                     |