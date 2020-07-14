---
title: "Autocomplete extension"
permalink: /docs/extensions/autocomplete/
excerpt: "Learn how to use autocomplete components."
toc: true
toc_label: "Guide"
---

## Basics

The `Autocomplete` component provides suggestions while you type into the field. The component is in essence a text box which, at runtime, filters data in a drop-down by a `Filter` operator when a user captures a value.

## Installation

The Autocomplete extension is part of the **Blazorise.Components** Nuget package.
{: .notice--info}

### Nuget

Install extension from nuget.

```
Install-Package Blazorise.Components
```

## Usage

### Markup

```html
<Autocomplete Data="@myDdlData"
    TextField="@((item)=>item.MyTextField)"
    ValueField="@((item)=>item.MyTextField)"
    SelectedValue="@selectedSearchValue"
    SelectedValueChanged="@MySearchHandler"
    Placeholder="Search..." />
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

    object selectedSearchValue { get; set; }

    void MySearchHandler( object newValue )
    {
        selectedSearchValue = newValue;
    }
}
```

## Attributes

| Name                 | Type               | Default    | Description                                           |
|----------------------|--------------------|------------|-------------------------------------------------------|
| TItem                | generic            |            | Model data type.                                      |
| Data                 | IEnumerable<TItem> |            | Data used for the search.                             |
| TextField            | Func               |            | Selector for the display name field.                  |
| ValueField           | Func               |            | Selector for the value field.                         |
| SelectedValue        | object             |            | Currently selected value.                             |
| SelectedValueChanged | event              |            | Raises an event after the selected value has changed. |
| SearchChanged        | event              |            | Occurs on every search text change.                   |
| Placeholder          | string             |            | Placeholder for the empty search field.               |
| MinLength            | int                | 1          | Minimum number of character needed to start search.   |
| Filter               | enum               | StartsWith | Filter method used to search the data.                |
| Disabled             | boolean            | false      | Disable the input field.                              |