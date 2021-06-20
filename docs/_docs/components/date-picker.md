---
title: "DatePicker component"
permalink: /docs/components/date-picker/
excerpt: "Learn how to use DatePicker component."
toc: true
toc_label: "Guide"
---

## Date

DatePicker is based on a [flatpickr](https://flatpickr.js.org/) datetime picker and as such is not a native date input element. That means that unlike `DateEdit` which will render `type="date"`, `DatePicker` will render `type="text"` in the DOM.

### Simple usage

```html
<DatePicker TValue="DateTime?" />
```

## Add icon

To add icon you can combine `DatePicker` with an `Addon`.

```html
<Addons>
    <Addon AddonType="AddonType.Body">
        <DatePicker @ref="@datePicker" TValue="DateTime?" />
    </Addon>
    <Addon AddonType="AddonType.End">
        <Button Color="Color.Light" Clicked="@(()=>datePicker.ToggleAsync())">
            <Icon Name="IconName.CalendarDay" />
        </Button>
    </Addon>
</Addons>
@code{
    DatePicker<DateTime?> datePicker;
}
```

### Format

Native **Flatpickr** component has some special rules regarding the date format string. To make it easier to use we tried to map the flatpickr custom formatter to behave as close to `.Net` date format string. Bellow you can find the list of available and supported mappings between Blazorise and flatpickr.

| .Net value | Flatpickr value | Description                                                                    |
|:----------:|:---------------:|--------------------------------------------------------------------------------|
|      d     |        j        | Day of the month without leading zeros, 1 to 31.                               |
|     dd     |        d        | Day of the month, 2 digits with leading zeros, 01 to 31.                       |
|     ddd    |        D        | A textual representation of a day, Mon through Sun.                            |
|    dddd    |        l        | A full textual representation of the day of the week, Sunday through Saturday. |
|      M     |        n        | Numeric representation of a month, without leading zeros, 1 through 12.        |
|     MM     |        m        | Numeric representation of a month, with leading zero, 01 through 12.           |
|     MMM    |        M        | A short textual representation of a month, Jan through Dec.                    |
|    MMMM    |        F        | A full textual representation of a month, January through December.            |
|      y     |        y        | A two digit representation of a year. 99 or 03.                                |
|     yy     |        y        | -\|\|-                                                                         |
|     yyy    |        Y        | A full numeric representation of a year, 4 digits, 1999 or 2003.               |
|    yyyy    |        Y        | -\|\|-                                                                         |
|    yyyy    |        Y        | -\|\|-                                                                         |
|      H     |        H        | Hours (24 hours), 00 to 23.                                                    |
|     HH     |        H        | -\|\|-                                                                         |
|      h     |        h        | Hours, 1 to 12.                                                                |
|     hh     |        G        | Hours, 2 digits with leading zeros, 01 to 12.                                  |
|      m     |        i        | Minutes, 00 to 59.                                                             |
|     mm     |        i        | -\|\|-                                                                         |
|      s     |        s        | Seconds, 0, 1 to 59.                                                           |
|     ss     |        S        | Seconds, 2 digits, 00 to 59.                                                   |
|      t     |        K        | AM/PM designator.                                                              |
|     tt     |        K        | -\|\|-                                                                         |

## Attributes

| Name              | Type                                                                       | Default      | Description                                                                                                                    |
|-------------------|----------------------------------------------------------------------------|--------------|--------------------------------------------------------------------------------------------------------------------------------|
| Date              | `TValue`                                                                   | `default`    | Gets or sets the input date value.                                                                                             |
| DateChanged       | `EventCallback<TValue>`                                                    |              | Occurs when the date has changed.                                                                                              |
| InputMode         | [DateInputMode]({{ "/docs/helpers/enums/#dateinputmode" | relative_url }}) | `Date`       | Hints at the type of data that might be entered by the user while editing the element or its contents.                         |
| Min               | `DateTimeOffset?`                                                          | null         | The earliest date to accept.                                                                                                   |
| Max               | `DateTimeOffset?`                                                          | null         | The latest date to accept.                                                                                                     |
| Pattern           | `string`                                                                   | null         | The pattern attribute specifies a regular expression that the input element's value is checked against on form submission.     |
| Placeholder       | `string`                                                                   | null         | Sets the placeholder for the empty date.                                                                                       |
| Step              | `int`                                                                      | 1            | The step attribute specifies the legal day intervals to choose from when the user opens the calendar in a date field.          |
| FirstDayOfWeek    | `DayOfWeek`                                                                | `Sunday`     | Defines the first day of the week.                                                                                             |
| DisplayFormat     | `string`                                                                   |  null        | Defines the display format of the date.                                                                                        |
| TimeAs24hr        | `bool  `                                                                   |  false       | Displays time picker in 24 hour mode without AM/PM selection when enabled.                                                     |