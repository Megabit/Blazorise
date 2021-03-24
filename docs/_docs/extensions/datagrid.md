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
  - `DataGridColumns` container for datagrid columns
    - `<DataGridColumn>` column template for text editor
    - `<DataGridNumericColumn>` column template for numeric values
    - `<DataGridDateColumn>` column template for datetime values
    - `<DataGridCheckColumn>` column template for boolean values
    - `<DataGridSelectColumn>` column template for selectable values
    - `<DataGridCommandColumn>` column template for editing commands like Edit, Save, Cancel, etc.
  - `DataGridAggregates` container for datagrid aggregates
    - `DataGridAggregate` defines the column and aggregation function type

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

### Static Files

Include CSS link into your index.html or _Host.cshtml file, depending if you’re using a Blazor WebAssembly or Blazor Server side project.

```html
<link href="_content/Blazorise.DataGrid/blazorise.datagrid.css" rel="stylesheet" />

<script src="_content/Blazorise.DataGrid/blazorise.datagrid.js"></script>
```

## Features

### Sorting

All columns can be sorted automatically if the option  `Sortable` is enabled on the column.

### Filtering

Use an attribute `Filterable` to enable or disable automatic filtering in grid component.

Default method for filtering is `Contains`. If you want to change it you can set the `FilterMethod` attribute on data grid. Supported methods are:

- `Contains` search for any occurrence  (default)
- `StartsWith` search only the beginning
- `EndsWith` search only the ending
- `Equals` search must match the entire value
- `NotEquals` opposite of Equals

**Example:**

```html
<DataGrid TItem="Employee"
    Data="@employeeList"
    Filterable="true"
    FilterMethod="DataGridFilterMethod.StartsWith">
```

### Custom Filtering

Regular filter works on per field basis. To enable advanced search capabilities you can use an attribute `CustomFilter`. More can be found in Usage section.

### Paging

Paging is handled automatically by the DataGrid. You also have some additional attributes to configure paging based on your requirements.

- `ShowPager` to hide or show pagination controls
- `PageSize` the maximum number of items for each page.
- `CurrentPage` current page number.
- `PreviousPageButtonTemplate` template for previous page button
- `NextPageButtonTemplate` template for next page button
- `FirstPageButtonTemplate` template for first page button
- `LastPageButtonTemplate` template for last page button
- `PageButtonTemplate` template for explicated page button with `PageButtonContext` as parameter

### Editing

The grid can perform some basic CRUD operations on the supplied `Data` collection. To enable editing on data-grid just set the `Editable` attribute to **true**.

By default every time the `Item` is saved it will be automatically handled by the data-grid itself. That means that all its fields will be populated after the user clicks on Save button. If you want to change that, you can just disable it by setting the `UseInternalEditing` to **false**.

The grid can work in two different editing modes that can provide different user experiences.

- `Form` editing is done in the internal DataGrid form
- `Inline` editing is done in the current row
- `Popup` editing is done in the the modal dialog

### Selecting

If you need to control how and when the grid row will be selected you can use a `RowSelectable` event handler. A simple example is:

```html
<DataGrid TItem="Employee"
        Data="@employeeList"
        @bind-SelectedRow="@selectedEmployee"
        RowSelectable=@((item)=>item.FirstName != "John")>
    ...
</DataGrid>
```

### Large Data

By default, DataGrid will load everything in memory and it will perform the necessary operations like paging, sorting and filtering. For large datasets this is impractical and so for these scenarios it is advised to load data page-by-page. This is accomplished with the use of `ReadData` event handler and `TotalItems` attribute. When you define the usage of `ReadData` the DataGrid will automatically switch to manual mode and every interaction with the grid will be proxied through the `ReadData`. This means that you as a developer will be responsible for all the loading, filtering and sorting of the data.

- `ReadData` event handler used to handle the loading of data
- `TotalItems` total number of items in the source data-set

Bellow you can find a [basic example]({{ "/docs/extensions/datagrid/#large-data-example" | relative_url }}) of how to load large data and apply it to the DataGrid.

### Aggregates

The DataGrid provider several built-in aggregates for column values. Supported aggregate functions are:

- `Sum` Calculate total(sum) value of the collection.
- `Average` Calculates the average of the numeric items in the collection.
- `Min` Finds the smallest value in the collection.
- `Max` Finds the largest value in the collection.
- `Count`  Counts the elements in a collection.
- `TrueCount` Counts boolean elements with true value.
- `FalseCount` Counts boolean elements with false value.

### Validations

The DataGrid provides validations of column values at editing or creating items. For using validation of DataGrid you have to use these properties:

- `ShowValidationFeedback` of DataGrid to hide or show feedback for validation.
- `ShowValidationsSummary` of DataGrid to hide or show validations summary.
- `ValidationsSummaryLabel` of DataGrid to set label of validations summary.
- `Validator` of DataGridColumn validates the input value after trying to save.
- `ValidationPattern` of DataGridColumn forces validation to use regex pattern matching instead of default validator handler.

To enable basic validation you only need to define a `Validator` attribute and assign it to your validation method.

```html
<DataGridColumn TItem="Employee" Field="@nameof( Employee.EMail )" Caption="EMail" Validator="@CheckEMail" Editable="true" />

public void CheckEMail( ValidatorEventArgs validationArgs )
{
    ValidationRule.IsEmail( validationArgs );

    if ( validationArgs.Status == ValidationStatus.Error )
    {
        validationArgs.ErrorText = "EMail has to be valid email";
    }
}
```

If you use `EditTemplate` to customize editing of columns, then using `Validator` or `ValidationPattern` will not work and you have to use `Validation` like this:

```html
<DataGridColumn TItem="Employee" Field="@nameof(Employee.Salary)" Caption="Salary" Editable="true">
    ...
    <EditTemplate>
        <Validation Validator="@CheckSalary" >
            <NumericEdit ... >
                <Feedback>
                    <ValidationError/>
                </Feedback>
            </NumericEdit >
        </Validation>
    </EditTemplate>
</DataGridColumn>
```

## Usage

The basic structure is pretty straightforward. You must define data and columns for the grid.

### DataGrid

For DataGrid the required fields are `TItem` typeparam and `Data` attribute. Other attributes on the DataGrid are optional.

### Columns

Next you must set the Columns that you want to see in the grid. When defining the columns the required fields are:

- `TItem` this is always the same model as on DataGrid.
- `Field` name of the field in the supplied model.
- `Caption` the column caption text.

### Nested fields

Field attribute also supports nested fields. You can define a column with field name like `"City.Country.Name"` and it will work. Just keep in mind that when editing nested fields they must be initialized first or otherwise they will raise an exception.

### Basic Example

```html
<DataGrid TItem="Employee"
        Data="@employeeList"
        @bind-SelectedRow="@selectedEmployee">
    <DataGridCommandColumn TItem="Employee" />
    <DataGridColumn TItem="Employee" Field="@nameof(Employee.Id)" Caption="#" Sortable="false" />
    <DataGridColumn TItem="Employee" Field="@nameof(Employee.FirstName)" Caption="First Name" Editable="true" />
    <DataGridColumn TItem="Employee" Field="@nameof(Employee.LastName)" Caption="Last Name" Editable="true" />
    <DataGridColumn TItem="Employee" Field="@nameof(Employee.EMail)" Caption="EMail" Editable="true" />
    <DataGridColumn TItem="Employee" Field="@nameof(Employee.City)" Caption="City" Editable="true" />
    <DataGridColumn TItem="Employee" Field="@nameof(Employee.Zip)" Caption="Zip" Editable="true" />
    <DataGridNumericColumn TItem="Employee" Field="@nameof(Employee.Childrens)" Caption="Childrens" Editable="true" />
    <DataGridColumn TItem="Employee" Field="@nameof(Employee.Salary)" Caption="Salary" Editable="true">
        <DisplayTemplate>
            @($"{( context as Employee )?.Salary} €")
        </DisplayTemplate>
        <EditTemplate>
            <NumericEdit TValue="decimal" Value="@((decimal)(((CellEditContext)context).CellValue))" ValueChanged="@(v=>((CellEditContext)context).CellValue=v)" />
        </EditTemplate>
    </DataGridColumn>
</DataGrid>
```

### Large Data Example

Just as in the previous example everything is the same except that now we must define the attribute `ReadData` and `TotalItems`. They're used to handle all of the loading, filtering and sorting of an actual data.

```html
<DataGrid TItem="Employee"
        Data="@employeeList"
        ReadData="@OnReadData"
        TotalItems="@totalEmployees">
    <DataGridCommandColumn TItem="Employee" />
    <DataGridColumn TItem="Employee" Field="@nameof(Employee.Id)" Caption="#" Sortable="false" />
    <DataGridColumn TItem="Employee" Field="@nameof(Employee.FirstName)" Caption="First Name" Editable="true" />
    <DataGridColumn TItem="Employee" Field="@nameof(Employee.LastName)" Caption="Last Name" Editable="true" />
    <DataGridColumn TItem="Employee" Field="@nameof(Employee.EMail)" Caption="EMail" Editable="true" />
    <DataGridColumn TItem="Employee" Field="@nameof(Employee.City)" Caption="City" Editable="true" />
    <DataGridColumn TItem="Employee" Field="@nameof(Employee.Zip)" Caption="Zip" Editable="true" />
    <DataGridNumericColumn TItem="Employee" Field="@nameof(Employee.Childrens)" Caption="Childrens" Editable="true" />
    <DataGridColumn TItem="Employee" Field="@nameof(Employee.Salary)" Caption="Salary" Editable="true">
        <DisplayTemplate>
            @($"{( context as Employee )?.Salary} €")
        </DisplayTemplate>
        <EditTemplate>
            <NumericEdit TValue="decimal" Value="@((decimal)(((CellEditContext)context).CellValue))" ValueChanged="@(v=>((CellEditContext)context).CellValue=v)" />
        </EditTemplate>
    </DataGridColumn>
</DataGrid>
```

```cs
@code
{
    Employee[] employeeList;
    int totalEmployees;

    async Task OnReadData( DataGridReadDataEventArgs<Employee> e )
    {
        // this can be call to anything, in this case we're calling a fictional api
        var response = await Http.GetJsonAsync<Employee[]>( $"some-api/employees?page={e.Page}&pageSize={e.PageSize}" );

        employeeList = response.Data; // an actual data for the current page
        totalEmployees = response.Total; // this is used to tell datagrid how many items are available so that pagination will work

        // always call StateHasChanged!
        StateHasChanged();
    }
}
```

### Aggregates

DataGrid will automatically generate necessary group cells based on the defined `DataGridAggregate` options.

```html
<DataGrid TItem="Employee" Data="@employeeList">
    <DataGridAggregates>
        <DataGridAggregate TItem="Employee" Field="@nameof( Employee.EMail )" Aggregate="DataGridAggregateType.Count">
            <DisplayTemplate>
                @($"Total emails: {context.Value}")
            </DisplayTemplate>
        </DataGridAggregate>
        <DataGridAggregate TItem="Employee" Field="@nameof( Employee.Salary )" Aggregate="DataGridAggregateType.Sum" DisplayFormat="{0:C}" DisplayFormatProvider="@System.Globalization.CultureInfo.GetCultureInfo("fr-FR")" />
        <DataGridAggregate TItem="Employee" Field="@nameof( Employee.IsActive )" Aggregate="DataGridAggregateType.TrueCount" />
    </DataGridAggregates>
    <DataGridColumns>
        ...
    </DataGridColumns>
</DataGrid>
```

By default all aggregate operations are run on in-memory `Data`. When working with large datasets that is not possible. So just as in the previous examples for large datasets you need to work with `ReadData` and set the `AggregateData` property.

```html
<DataGrid TItem="Employee"
        Data="@employeeList"
        ReadData="@OnReadData"
        TotalItems="@totalEmployees"
        AggregateData="@employeeSummary">
</DataGrid>
```

```cs
@code
{
    Employee[] employeeList;
    Employee[] employeeSummary;
    int totalEmployees;

    async Task OnReadData( DataGridReadDataEventArgs<Employee> e )
    {
        // this can be call to anything, in this case we're calling a fictional api
        var response = await Http.GetJsonAsync<Employee[]>( $"some-api/employees?page={e.Page}&pageSize={e.PageSize}" );
        var aggregateResponse = await Http.GetJsonAsync<Employee[]>( $"some-aggregate-api/employees" );

        employeeList = response.Data; // an actual data for the current page
        totalEmployees = response.Total; // this is used to tell datagrid how many items are available so that pagination will work

        employeeSummary = aggregateResponse.Data;

        // always call StateHasChanged!
        StateHasChanged();
    }
}
```


### Custom Filtering

Filter API is fairly straightforward. All you need is to attach `CustomFilter` to a function and bind search value to `TextEdit` field. DataGrid will automatically respond to entered value.

```html
<TextEdit @bind-Text="@customFilterValue" />

<DataGrid TItem="Employee"
        Data="@employeeList"
        CustomFilter="@OnCustomFilter">
    ...
</DataGrid>
```

```cs
@code
{
    string customFilterValue;

    bool OnCustomFilter( Employee model )
    {
        // We want to accept empty value as valid or otherwise
        // datagrid will not show anything.
        if ( string.IsNullOrEmpty( customFilterValue ) )
            return true;

        return
            model.FirstName?.Contains( customFilterValue, StringComparison.OrdinalIgnoreCase ) == true
            || model.LastName?.Contains( customFilterValue, StringComparison.OrdinalIgnoreCase ) == true
            || model.EMail?.Contains( customFilterValue, StringComparison.OrdinalIgnoreCase ) == true;
    }
}
```

### Custom Row Colors

You have full control over appearance of each row, including the selected rows.

```html
<DataGrid TItem="Employee"
        Data="@employeeList"
        CustomFilter="@OnCustomFilter"
        RowStyling="@OnRowStyling"
        SelectedRowStyling="@OnSelectedRowStyling">
    ...
</DataGrid>
```

```cs
@code
{
    void OnRowStyling( Employee employee, DataGridRowStyling styling )
    {
        if ( !employee.IsActive )
            styling.Style = "color: red;";
    }

    void OnSelectedRowStyling( Employee employee, DataGridRowStyling styling )
    {
        styling.Background = Background.Info;
    }
}
```

### NewItemDefaultSetter

`NewItemDefaultSetter` function is used to set default values when new item is created and before the edit form is shown. It will only be evaluate, if datagrid is editable.

```html
<DataGrid TItem="Employee" Editable="true" NewItemDefaultSetter="@OnEmployeeNewItemDefaultSetter">
  ...
</DataGrid>
@code{
    void OnEmployeeNewItemDefaultSetter( Employee employee )
    {
        employee.Salary = 100.0M;
        employee.IsActive = true;
    }
}
```

## Templates

For extra customization DataGrid will provide you with two additional templates that you can use to extend it's default behavior. A display template is used to customize display cells and an edit template is used to customize cell editors. You can place anything inside of the templates, be it a Blazorise components, regular html tags or your own components.

Both templates have a special `context` attribute that is used to give access to the underline cell value. To learn more about `context` please go to official Blazor [documentation](https://docs.microsoft.com/en-us/aspnet/core/blazor/templated-components).

### DisplayTemplate

Display template is using `TItem` as a context value.

```html
<DataGridNumericColumn TItem="Employee" Field="@nameof(Employee.DateOfBirth)" Caption="Date Of Birth" Editable="true">
    <DisplayTemplate>
        @{
            var date = ( context as Employee )?.DateOfBirth;

            if ( date != null )
            {
                @($"{date.Value.ToShortDateString()}, age: {( DateTime.Now.Year - date.Value.Year )}")
            }
        }
    </DisplayTemplate>
</DataGridNumericColumn>
```

### EditTemplate

Edit template will give you a way to handle the editing of grid cell values. For this template `CellEditContext` is used as a `context` value. Use it to get or set the cell values.

```html
<DataGridColumn TItem="Employee" Field="@nameof(Employee.Salary)" Caption="Salary" Editable="true">
    <DisplayTemplate>
        @($"{( context as Employee )?.Salary} €")
    </DisplayTemplate>
    <EditTemplate>
        <NumericEdit TValue="decimal" Value="@((decimal)(((CellEditContext)context).CellValue))" ValueChanged="@(v=>((CellEditContext)context).CellValue=v)" />
    </EditTemplate>
</DataGridColumn>
```

### DetailRowTemplate

DetailRow template allows you to display nested structure bellow each row in the grid. One of the examples is "master-detail" relationship between two data-source inside the DataGrid.

For this template the `context` value is the item from the parent grid.

```html
<DetailRowTemplate>
    @{
        var salaries = ( context as Employee ).Salaries;

        <DataGrid TItem="Salary"
                  Data="salaries"
                  Sortable="false"
                  ShowCaptions="false">
            <DataGridCommandColumn TItem="Salary" />
            <DataGridDateColumn TItem="Salary" Field="@nameof(Salary.Date)" Caption="Date" />
            <DataGridNumericColumn TItem="Salary" Field="@nameof(Salary.Total)" Caption="Total" />
        </DataGrid>
    }
</DetailRowTemplate>
```

Once it's defined a DetailRow will be visible for every row in the grid. If you want to control the visibility of detail-row you can use `DetailRowTrigger` attribute that can be defined in it's parent grid.

```html
<DataGrid TItem="Employee"
          Data="@employees"
          ...
          @bind-SelectedRow="@selectedEmployee"
          DetailRowTrigger="@((item)=>item.Salaries?.Count > 0 && item.Id == selectedEmployee?.Id)">
    ...
</DataGrid>
```

### Command Templates

If you want to change default buttons you can use following templates

- `NewCommandTemplate`
- `EditCommandTemplate`
- `SaveCommandTemplate`
- `CancelCommandTemplate`
- `DeleteCommandTemplate`
- `ClearFilterCommandTemplate`

```html
<DataGridCommandColumn TItem="Employee">
    <NewCommandTemplate>
        <Button Color="Color.Success" Clicked="@context.Clicked">New</Button>
    </NewCommandTemplate>
    <EditCommandTemplate>
        <Button Color="Color.Primary" Clicked="@context.Clicked">Edit</Button>
    </EditCommandTemplate>
</DataGridCommandColumn>
```

### Loading Templates

If you want to change display of content, while grid is empty or `ReadData` is executing, you can use following templates:

- `EmptyTemplate`
- `LoadingTemplate`

```html
<DataGrid TItem="Employee"
    Data="@employeeList"
    TotalItems="@totalEmployees"
    ReadData="@LoadEmployeesFromService">
    <ChildContent>
    	<!--DataGridColumns-->
    </ChildContent>
    <EmptyTemplate>
    	<div class="box">
        	No employees are found!
        </div>
    </EmptyTemplate>
    <LoadingTemplate>
    	<div class="box">
        	<progress class="progress is-small is-primary" max="100"/>
        </div>
    </LoadingTemplate>
</DataGrid>
```

```cs
@code
{
    Employee[] employeeList;
    int totalEmployees;

    async Task LoadEmployeesFromService( DataGridReadDataEventArgs<Employee> e )
    {
        /*
        * This can be call to anything like calling api for load employees
        * and while execution 'LoadingTemplate' will be displayed.
        * If your api call returns empty result, then 'EmptyTemplate' will be displayed,
        * so that you can see easily, that your loading is finish, but your result is empty.
        */
    }
}
```


### Empty Cell Template

If you want to change cell content display when cell's value is null, use `EmptyCellTemplate`.

```html
<DataGrid TItem="Employee"
    Data="@employeeList"
    TotalItems="@totalEmployees"
    ReadData="@LoadEmployeesFromService">
    <ChildContent>
    	<!--DataGridColumns-->
    </ChildContent>
    <EmptyCellTemplate>
    	<Text Style="opacity: .5;">-</Text>
    </EmptyTemplate>
</DataGrid>
```

### DataGrid Multiple Selection

Set `SelectionMode` to `DataGridSelectionMode.Multiple` to enable multiple selection on Datagrid. 

```html
<DataGrid TItem="Employee"
    Data="@employeeList"
    SelectionMode="DataGridSelectionMode.Multiple"
    @bind-SelectedRows="@selectedEmployees">
</DataGrid>
```

Clicking rows will now select multiple records at a time. You can now access them by using the `SelectedRows` parameter and also bind to the `SelectedRowsChanged` event callback.

Optionally you can use the new Datagrid column `<DataGridMultiSelectColumn>` to enable a checkbox column that works exclusively with multiple selection. You can either use your own `MultiSelectTemplate` render fragment to customize the input that will appear in the column and trigger the multiple selection by then binding to the provided `SelectedChanged` event callback or just use the provided default by not specifying a `MultiSelectTemplate` render fragment. When using this extra column, the top row column, will provide the ability to select or unselect all rows.

```html
<DataGrid TItem="Employee"
    Data="@employeeList"
    SelectionMode="DataGridSelectionMode.Multiple">
    <DataGridColumns>
        <DataGridMultiSelectColumn TItem="Employee" Width="30px"></DataGridMultiSelectColumn>
        ...
    </DataGridColumns>
</DataGrid>
```

### DataGrid Button Row

Provide a `ButtonRowTemplate` and have the DataGridCommandMode set to either `Default` or `ButtonRow`. 

The template has access to the internal commands so you're also able to construct your own buttons on the pager that can also trigger the Datagrid's CRUD and clear filter operations as shown in the example below:
```html
<ButtonRowTemplate>
    <Button Color="Color.Success" Clicked="@context.NewCommand.Clicked">New</Button>
    <Button Color="Color.Primary" Disabled="@(selectedEmployee is null)" Clicked="context.EditCommand.Clicked">Edit</Button>
    <Button Color="Color.Danger" Disabled="@(selectedEmployee is null)" Clicked="context.DeleteCommand.Clicked">Delete</Button>
    <Button Color="Color.Link" Clicked="@context.ClearFilterCommand.Clicked">Clear Filter</Button>
</ButtonRowTemplate>
```

### DataGrid Resizable

Set `Resizable` to `true` and you'll be able to resize the datagrid columns. 

## Attributes

### DataGrid

| Name                   | Type                                                                                         | Default                | Description                                                                                                          |
|------------------------|----------------------------------------------------------------------------------------------|------------------------|----------------------------------------------------------------------------------------------------------------------|
| Data                   | IEnumerable<TItem>                                                                           |                        | Grid data-source.                                                                                                    |
| EditMode               | [EditMode]({{ "/docs/extensions/datagrid/#editmode" | relative_url }})                       | `Form`                 | Specifies the grid editing modes.                                                                                    |
| UseInternalEditing     | boolean                                                                                      | `true`                 | Specifies the behavior of DataGrid editing.                                                                          |
| Editable               | boolean                                                                                      | `false`                | Whether users can edit DataGrid rows.                                                                                |
| Resizable              | boolean                                                                                      | `false`                | Whether users can resize DataGrid columns.                                                                           |
| ResizeMode             | [DataGridResizeMode]({{ "/docs/helpers/enums/#datagridresizemode" | relative_url }})         | `Header`               | Defines the resize mode of the data grid columns.                                                                    |
| Sortable               | boolean                                                                                      | `true`                 | Whether end-users can sort data by the column's values.                                                              |
| ShowCaptions           | boolean                                                                                      | `true`                 | Gets or sets whether user can see a column captions.                                                                 |
| Filterable             | boolean                                                                                      | `false`                | Whether users can filter rows by its cell values.                                                                    |
| ShowPager              | boolean                                                                                      | `false`                | Whether users can navigate DataGrid by using pagination controls.                                                    |
| CurrentPage            | boolean                                                                                      | `1`                    | Current page number.                                                                                                 |
| PageSize               | int                                                                                          | `5`                    | Maximum number of items for each page.                                                                               |
| Striped                | boolean                                                                                      | `false`                | Adds stripes to the table.                                                                                           |
| Bordered               | boolean                                                                                      | `false`                | Adds borders to all the cells.                                                                                       |
| Borderless             | boolean                                                                                      | `false`                | Makes the table without any borders.                                                                                 |
| Hoverable              | boolean                                                                                      | `false`                | Adds a hover effect when moussing over rows.                                                                         |
| Narrow                 | boolean                                                                                      | `false`                | Makes the table more compact by cutting cell padding in half.                                                        |
| ReadData               | EventCallback                                                                                |                        | Handles the manual loading of large data sets.                                                                       |
| SelectedRow            | TItem                                                                                        |                        | Currently selected row.                                                                                              |
| SelectedRowChanged     | EventCallback                                                                                |                        | Occurs after the selected row has changed.                                                                           |
| RowSelectable          | `Func<TItem,bool>`                                                                           |                        | Handles the selection of the clicked row. If not set it will default to always true.                                 |
| RowHoverCursor         |` Func<TItem,Blazorise.Cursor>`                                                               |                        | Handles the selection of the cursor for a hovered row. If not set, `Blazorise.Cursor.Pointer` will be used.          |
| DetailRowTrigger       | `Func<TItem,bool>`                                                                           |                        | A trigger function used to handle the visibility of detail row.                                                      |
| RowInserting           | Action                                                                                       |                        | Cancelable event called before the row is inserted.                                                                  |
| RowUpdating            | Action                                                                                       |                        | Cancelable event called before the row is updated.                                                                   |
| RowInserted            | EventCallback                                                                                |                        | Event called after the row is inserted.                                                                              |
| RowUpdated             | EventCallback                                                                                |                        | Event called after the row is updated.                                                                               |
| RowRemoving            | Action                                                                                       |                        | Cancelable event called before the row is removed.                                                                   |
| RowRemoved             | EventCallback                                                                                |                        | Event called after the row is removed.                                                                               |
| PageChanged            | EventCallback                                                                                |                        | Occurs after the selected page has changed.                                                                          |
| EmptyCellTemplate      | RenderingFragment                                                                            |                        | Define the format for empty data cell                                                                                |
| EmptyTemplate          | RenderingFragment                                                                            |                        | Define the format for empty data collection                                                                          |
| LoadingTemplate        | RenderingFragment                                                                            |                        | Define the format for signal of loading data                                                                         |
| PopupTitleTemplate     | `RenderFragment<PopupTitleContext<TItem>>`                                                   |                        | Template for custom title of edit popup dialog                                                                       |
| NewItemDefaultSetter   | `Action<TItem>`                                                                              |                        | Action will be called for setting default values of property, when create new entry                                  |
| PageButtonTemplate     | `RenderTemplate<PageButtonContext>`                                                          |                        | Define the format a pagination button                                                                                |
| ShowValidationFeedback | boolean                                                                                      | false                  | Hide or show feedback for validation                                                                                 |
| ShowValidationsSummary | boolean                                                                                      | true                   | Hide or show validations summary                                                                                     |
| ValidationsSummaryLabel| string                                                                                       | null                   | Set label of validations summary                                                                                     |
| SortMode               | [DataGridSortMode]({{ "/docs/helpers/enums/#datagridsortmode" | relative_url }})             | `Multiple`             | Defines whether the user can sort only by one column or by multiple.                                                 |
| SelectionMode          | [DataGridSelectionMode]({{ "/docs/helpers/enums/#datagridselectionmode" | relative_url }})   | `Single`               | Defines whether the datagrid is set to single or multiple selection mode.                                            |
| Localizers             | `DataGridLocalizers`                                                                         |                        | Custom localizer handlers to override default  localization.                                                         |
| CommandMode            | [DataGridCommandMode]({{ "/docs/helpers/enums/#datagridcommandmode" | relative_url }})       | `Default`              | Defines whether the datagrid renders both commands and button row or just either one of them.                        |

### EditMode

Specifies the grid editing modes.

- `Form` editing is done in the internal DataGrid form
- `Inline` editing is done in the current row
- `Popup` editing is done in the the modal dialog

### DataGridColumn

| Name                      | Type                                                                | Default             | Description                                                                                                   |
|---------------------------|---------------------------------------------------------------------|---------------------|---------------------------------------------------------------------------------------------------------------|
| Field                     | string                                                              |                     | TItem data field name.                                                                                        |
| Caption                   | string                                                              |                     | Column's display caption. It will be displayed, if ColumnTemplate is not set.                                 |
| Filter                    | FilterContext                                                       |                     | Filter value for this column.                                                                                 |
| Direction                 | SortDirection                                                       | `None`              | Column initial sort direction.                                                                                |
| TextAlignment             | TextAlignment                                                       | `None`              | Defines the alignment for display cell.                                                                       |
| HeaderTextAlignment       | TextAlignment                                                       | `None`              | Defines the alignment for column header cell.                                                                 |
| Editable                  | bool                                                                | false               | Whether users can edit cell values under this column.                                                         |
| Displayable               | bool                                                                | true                | Whether column can be displayed on a grid.                                                                    |
| DisplayOrder              | int                                                                 | 0                   | Where column will be displayed on a grid.                                                                     |
| Sortable                  | bool                                                                | true                | Whether end-users can sort data by the column's values.                                                       |
| Readonly                  | bool                                                                | false               | whether end-users are prevented from editing the column's cell values.                                        |
| ShowCaption               | bool                                                                | true                | whether the column's caption is displayed within the column header.                                           |
| Filterable                | bool                                                                | true                | Whether users can filter rows by its cell values.                                                             |
| Width                     | string                                                              | null                | The width of the column.                                                                                      |
| DisplayFormat             | string                                                              |                     | Defines the format for display value.                                                                         |
| DisplayFormatProvider     | IFormatProvider                                                     |                     | Defines the format provider info for display value.                                                           |
| CellClass                 | `Func<TItem, string>`                                               |                     | Custom classname handler for cell based on the current row item.                                              |
| CellStyle                 | `Func<TItem, string>`                                               |                     | Custom style handler for cell based on the current row item.                                                  |
| HeaderCellClass           | string                                                              |                     | Custom classname for header cell.                                                                             |
| HeaderCellStyle           | string                                                              |                     | Custom style for header cell.                                                                                 |
| FilterCellClass           | string                                                              |                     | Custom classname for filter cell.                                                                             |
| FilterCellStyle           | string                                                              |                     | Custom style for filter cell.                                                                                 |
| GroupCellClass            | string                                                              |                     | Custom classname for group cell.                                                                              |
| GroupCellStyle            | string                                                              |                     | Custom style for group cell.                                                                                  |
| DisplayTemplate           | `RenderFragment<TItem>`                                             |                     | Template for custom cell display formating.                                                                   |
| EditTemplate              | `RenderFragment<CellEditContext>`                                   |                     | Template for custom cell editing.                                                                             |
| FilterTemplate            | `RenderFragment<FilterContext>`                                     |                     | Template for custom column filter rendering.                                                                  |
| PopupSize                 | [ModalSize]({{ "/docs/helpers/sizes/#modalsize" | relative_url }})  |  `Default`          | Defines the size of popup modal.                                                                              |
| PopupFieldColumnSize      | `IFluentColumn`                                                     |  `IsHalf.OnDesktop` | Defines the size of field for popup modal.                                                                    |
| CaptionTemplate           | `RenderingFragment<DataGridColumn<TItem>>`                          |                     | Template for custom caption. CaptionTemplate will block caption template.                                     |
| SortDirectionTemplate     | `RenderingFragment<SortDirection>`                                  |                     | Template for custom sort direction icon.                                                                      |
| Validator                 | `Action<ValidatorEventArgs>`                                        |                     | Validates the input value after trying to save.                                                               |
| ValidationPattern         | string                                                              |                     | Forces validation to use regex pattern matching instead of default validator handler.                         |
