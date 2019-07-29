---
title: "DataGrid extension"
permalink: /docs/extensions/datagrid/
excerpt: "Learn how to use DataGrid extension components."
toc: true
toc_label: "Guide"
---

## Overview

To create a basic grid in Blazorise you need to set the _Column_ that will define the grid structure and behavior.

### Structure

- `<DataGrid>` the main **container**
  - `<DataGridColumn>` column template for text editor
  - `<DataGridNumericColumn>` column template for numeric values
  - `<DataGridDateColumn>` column template for datetime values
  - `<DataGridCheckColumn>` column template for boolean values
  - `<DataGridSelectColumn>` column template for selectable values
  - `<DataGridCommandColumn>` column template for editing commands like Edit, Save, Cancel, etc.

## Installation

DataGrid component is created as an extension for Blazorise so before you continue you need to first get it from NuGet.

### NuGet

Install DataGrid extension from NuGet.

```
Install-Package Blazorise.DataGrid
```

### Imports

In your main _Imports.razor_ add:

```cs
@using Blazorise.DataGrid
```

## Features

### Sorting

All columns can be sorted automatically if the option  `AllowSort` is enabled on the column.

### Filtering

Use an attribute `AllowFilter` to enable or disable automatic filtering in grid component.

### Paging

Paging is handled automatically by the DataGrid. You also have some additional attributes to configure paging based on your requirements.

- `ShowPager` to hide or show pagination controls
- `PageSize` the maximum number of items for each page.
- `CurrentPage` current page number.

### Editing

The grid can perform some basic CRUD operations on the supplied `Data` collection. By default every time the `Item` is saved it will be automatically handled by the datagrid itself. That means that all its fields will be populated after the user clicks on Save button. If you want to change that, you can just disable it by setting the `UseInternalEditing` to **false**.

The grid can work in two different editing modes that can provide different user experiences.

- `Form` - editing is done in the internal DataGrid form
- `InRow` - editing is done in the current row

## Usage

The basic structure is pretty straightforward. You must set the `TItem` typeparam and set `Data` attribute. Other attributes on the DataGrid are optional.

Next you must set the Columns that you want to see in the grid. When defining the columns the required fields are:

- `TItem` this is always the same model as on DataGrid
- `Field` name of the field in the supplied model
- `Caption` the column caption text

As you can see in the example the _Salary_ field is defined different that the other columns. It has `DisplayTemplate` and `EditTemplate` with which you have more freedom to show your data and how it will be edited. Every column can be defined like this. Please note that this templates are just optional. The grid will use the default editors if the templates are not defined.

```html
<DataGrid TItem="Employee"
        Data="@employeeList"
        @bind-SelectedRow="@selectedEmployee">
    <DataGridCommandColumn TItem="Employee" />
    <DataGridColumn TItem="Employee" Field="@nameof(Employee.Id)" Caption="#" AllowSort="false" />
    <DataGridColumn TItem="Employee" Field="@nameof(Employee.FirstName)" Caption="First Name" AllowEdit="true" />
    <DataGridColumn TItem="Employee" Field="@nameof(Employee.LastName)" Caption="Last Name" AllowEdit="true" />
    <DataGridColumn TItem="Employee" Field="@nameof(Employee.EMail)" Caption="EMail" AllowEdit="true" />
    <DataGridColumn TItem="Employee" Field="@nameof(Employee.City)" Caption="City" AllowEdit="true" />
    <DataGridColumn TItem="Employee" Field="@nameof(Employee.Zip)" Caption="Zip" AllowEdit="true" />
    <DataGridNumericColumn TItem="Employee" Field="@nameof(Employee.Childrens)" Caption="Childrens" AllowEdit="true" />
    <DataGridColumn TItem="Employee" Field="@nameof(Employee.Salary)" Caption="Salary" AllowEdit="true">
        <DisplayTemplate>
            @($"{( context as Employee )?.Salary} €")
        </DisplayTemplate>
        <EditTemplate>
            <NumericEdit TValue="decimal" Value="@((decimal)(((CellEditContext)context).CellValue))" ValueChanged="@(v=>((CellEditContext)context).CellValue=v)" />
        </EditTemplate>
    </DataGridColumn>
</DataGrid>
```

### DisplayTemplate

This is the special component fragment that you can use to format displayed cell values. It will provide you with the `context` keyword that represents the grid model. To learn more about context please look at the official blazor [documentation](https://docs.microsoft.com/hr-hr/aspnet/core/blazor/components?view=aspnetcore-3.0#templated-components).

### EditTemplate

This template will give you a way to handle the editing of grid cell values. The template `context` is CellEditContext. Use it to get or set the cell values.

## Attributes

| Name                  | Type                                                                | Default | Description                                                                                           |
|-----------------------|---------------------------------------------------------------------|---------|-------------------------------------------------------------------------------------------------------|
| Data                  | IEnumerable<TItem>                                                  |         | Grid data-source.                                                                                     |
| EditMode              | [EditMode]({{ "/docs/extensions/datagrid/#editmode" | relative_url }})| `Form`  | Specifies the grid editing modes.                                                                      |
| UseInternalEditing    | boolean                                                             | true    | Specifies the behavior of DataGrid editing.                                                          |
| AllowEdit             | boolean                                                             | false   | Whether users can edit DataGrid rows.                                                                 |
| AllowSort             | boolean                                                             | true    | Whether end-users can sort data by the column's values.                                               |
| AllowFilter           | boolean                                                             | false   | Whether users can filter rows by its cell values.                                                     |
| ShowPager             | boolean                                                             | false   | Whether users can navigate DataGrid by using pagination controls.                                     |
| CurrentPage           | boolean                                                             | 1       | Current page number.                                                                                  |
| PageSize              | int                                                                 | 5       | Maximum number of items for each page.                                                                |
| SelectedRow           | TItem                                                               |         | Currently selected row.                                                                               |
| SelectedRowChanged    | EventCallback                                                       |         | Occurs after the selected row has changed.                                                            |
| RowSaving             | Action                                                              |         | Cancelable event called before the row is inserted or updated.                                        |
| RowInserted           | EventCallback                                                       |         | Event called after the row is inserted.                                                               |
| RowUpdated            | EventCallback                                                       |         | Event called after the row is updated.                                                                |
| RowRemoving           | Action                                                              |         | Cancelable event called before the row is removed.                                                    |
| RowRemoved            | EventCallback                                                       |         | Event called after the row is removed.                                                                |
| PageChanged           | Action                                                              |         | Occurs after the selected page has changed.                                                           |

### EditMode

Specifies the grid editing modes.

- `Form` Specifies that the mask feature is disabled.
- `InRow` Specifies that the editor should accept numeric values and that the mask string must use the Numeric format syntax.