---
title: "SelectList extension"
permalink: /docs/extensions/selectlist/
excerpt: "Learn how to use SelectList components."
toc: true
toc_label: "Guide"
---

## Basics

The `SelectList` component allows you to select a value from a list of predefined items.

## Installation

The SelectList extension is part of the **Blazorise.Components** NuGet package.
{: .notice--info}

### NuGet

Install extension from NuGet.

```
Install-Package Blazorise.Components
```

## Usage

### Markup

```html
<SelectList
    TItem="MySelectModel"
    TValue="int"
    Data="@myDdlData"
    TextField="@((item)=>item.MyTextField)"
    ValueField="@((item)=>item.MyValueField)"
    SelectedValue="@selectedListValue"
    SelectedValueChanged="@MyListValueChangedHandler" />
```

### Data binding

```cs
@code{
    public class MySelectModel
    {
        public int MyValueField { get; set; }
        public string MyTextField { get; set; }
    }

    static string[] Countries = { "Albania", "Andorra", "Armenia", "Austria", "Azerbaijan", "Belarus", "Belgium", "Bosnia & Herzegovina", "Bulgaria", "Croatia", "Cyprus", "Czech Republic", "Denmark", "Estonia", "Finland", "France", "Georgia", "Germany", "Greece", "Hungary", "Iceland", "Ireland", "Italy", "Kosovo", "Latvia", "Liechtenstein", "Lithuania", "Luxembourg", "Macedonia", "Malta", "Moldova", "Monaco", "Montenegro", "Netherlands", "Norway", "Poland", "Portugal", "Romania", "Russia", "San Marino", "Serbia", "Slovakia", "Slovenia", "Spain", "Sweden", "Switzerland", "Turkey", "Ukraine", "United Kingdom", "Vatican City" };
    IEnumerable<MySelectModel> myDdlData = Enumerable.Range( 1, Countries.Length ).Select( x => new MySelectModel { MyTextField = Countries[x - 1], MyValueField = x } );

    int selectedListValue { get; set; } = 3;

    void MyListValueChangedHandler( int newValue )
    {
        selectedListValue = newValue;
        StateHasChanged();
    }
}
```

## Attributes

| Name                 | Type                       | Default    | Description                                           |
|----------------------|----------------------------|------------|-------------------------------------------------------|
| TItem                | typeparam                  |            | Model data type.                                      |
| TValue               | typeparam                  |            | Bound value data type.                                |
| Data                 | IEnumerable<TItem>         |            | Data used for selection.                              |
| TextField            | `Func<TItem, string>`      |            | Selector for the display name field.                  |
| ValueField           | `Func<TItem, TValue>`      |            | Selector for the value field.                         |
| SelectedValue        | TValue                     |            | Currently selected value.                             |
| SelectedValueChanged | `EventCallback<TValue>`    |            | Raises an event after the selected value has changed. |
| MaxVisibleItems      | `int?`                     | `null`     | Specifies how many options should be shown at once.   |