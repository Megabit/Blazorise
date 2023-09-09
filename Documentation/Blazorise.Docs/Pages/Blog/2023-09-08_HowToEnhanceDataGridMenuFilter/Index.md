---
title: How to enhance the new DataGrid menu filter
description: 
permalink: /blog/how-to-enhance-the-new-datagrid-menu-filter
canonical: /blog/how-to-enhance-the-new-datagrid-menu-filter
image-url: 
image-title: How to enhance the new DataGrid menu filter
author-name: David Moreira
author-image: david
posted-on: September 8th, 2023
read-time: 5 min
---

# How to enhance the new DataGrid menu filter

With the release of Blazorise v1.3, we introduced a new Filter Method option that we've named Menu mode. With this new option you're now able to use pre defined filtering on a per-column basis.

While this is a great addition to the Blazorise `DataGrid` and improving the flexibility of the filtering, it's still not a perfect solution. And that's what we'll touch on, on this blog post.

## Contextual filtering by column type

Contextual filtering that makes sense as per your column types is currently not supported out of the box.

I.e A numeric column type should have ways to filter by "less than" or "greater than"" for instance.

Let's talk about how we can make this work by using the `DataGrid` tools that we have at our disposal.

### Custom Filter Method

Let's start with the first limitation, the `DataGridFilterMethod`. At the time of writing this only supports:
- Contains
- StartsWith
- EndsWith
- Equals
- NotEquals

So we definitely need a way to further enhance the filtering capabilities. Let's introduce a new `MyFilter` enum which additionally introduces "LessThan" and "GreaterThan".


```html|MyFilterExample

public enum MyFilter
{
	Equals, NotEquals, Contains, StartsWith, EndsWith, GreaterThan, LessThan
}
```

### Custom Filter Tracker

Next up, we need to find a way to track & glue our custom filter system together. Let's go ahead and create a custom `FilterTracker` class that will hold our custom filter values.

```html|FilterTrackerExample

private FilterTracker<FilterExample> _filterTracker = new();

public class FilterTracker<T>
{
    public List<ColumnFilter<T>> columnFilters { get; set; }

    public void ClearColumnFilter( DataGridColumn<T> column )
    {
        columnFilters ??= new();

        var columnFilter = columnFilters.FirstOrDefault( x => x.Column.Field == column.Field );
        if (columnFilter is not null)
        {
            columnFilters.Remove( columnFilter );
        }
    }

    public void SetColumnFilter( DataGridColumn<T> column, MyFilter myFilter )
    {
        columnFilters ??= new();

        var columnFilter = columnFilters.FirstOrDefault( x => x.Column.Field == column.Field );
        if (columnFilter is null)
        {
            columnFilters.Add( new()
                {
                    Column = column,
                    SelectedFilter = myFilter
                } );
        }
        else
        {
            columnFilter.SelectedFilter = myFilter;
        }
    }

    public void SetColumnSearchValue( DataGridColumn<T> column, string searchValue )
    {
        columnFilters ??= new();

        var columnFilter = columnFilters.FirstOrDefault( x => x.Column.Field == column.Field );
        if (columnFilter is null)
        {
            columnFilters.Add( new()
                {
                    Column = column,
                    SearchValue = searchValue
                } );
        }
        else
        {
            columnFilter.SearchValue = searchValue;
        }
    }

    public ColumnFilter<T> GetColumnFilter( string fieldName )
        => columnFilters?.FirstOrDefault( x => x.Column.Field == fieldName );

    public MyFilter GetColumnFilterValue( string fieldName )
        => GetColumnFilter( fieldName )?.SelectedFilter ?? MyFilter.Contains;

    public string GetColumnSearchValue( string fieldName )
        => GetColumnFilter( fieldName )?.SearchValue;

}
```

### Custom Filter Menu Template

Now we need to update the UI so it uses our custom implementation, let's use the provided FilterMenuTemplate in order to do so.

```html|FilterMenuTemplateExample

<FilterMenuTemplate>
    <Row>
        <Column ColumnSize="ColumnSize.Is4">
            <Select TValue="MyFilter" SelectedValue="@_filterTracker.GetColumnFilterValue(context.Column.Field)" SelectedValueChanged="e => { _filterTracker.SetColumnFilter(context.Column, e); }">
                <SelectItem TValue="MyFilter" Value="@MyFilter.Contains">Contains</SelectItem>
                <SelectItem TValue="MyFilter" Value="@MyFilter.StartsWith">Starts With</SelectItem>
                <SelectItem TValue="MyFilter" Value="@MyFilter.EndsWith">Ends With</SelectItem>
                <SelectItem TValue="MyFilter" Value="@MyFilter.Equals">Equals</SelectItem>
                <SelectItem TValue="MyFilter" Value="@MyFilter.NotEquals">Not Equals</SelectItem>
                @if (context.Column.ColumnType == DataGridColumnType.Numeric)
                {
                    <SelectItem TValue="MyFilter" Value="@MyFilter.GreaterThan">GreaterThan</SelectItem>
                    <SelectItem TValue="MyFilter" Value="@MyFilter.LessThan">LessThan</SelectItem>
                }
            </Select>
        </Column>
        <Column ColumnSize="ColumnSize.Is4">
            <TextEdit Text="@_filterTracker.GetColumnSearchValue(context.Column.Field)" TextChanged="@((newValue) => _filterTracker.SetColumnSearchValue(context.Column, newValue))" />
        </Column>

        <Column ColumnSize="ColumnSize.Is4">
            <Button Clicked="context.Filter" Color="Color.Primary"><Icon Name="IconName.Filter"></Icon> Filter</Button>
            <Button Clicked="@(() => { _filterTracker.ClearColumnFilter(context.Column); context.ClearFilter.InvokeAsync(); })" Color="Color.Light"><Icon Name="IconName.Clear"></Icon> Clear</Button>
        </Column>
    </Row>
</FilterMenuTemplate>
```

### Making it work 

Now that the user can submit the new filter values, and we are tracking everything under our `FilterTracker` we can leverage the Datagrid's `CustomFilter` in order to apply our custom filtering.

```html|CustomFilterExample

private bool MyCustomFilter( FilterExample row )
{
    return _filterTracker.columnFilters is null
        ? true
        : _filterTracker.columnFilters.All( x => EvaluateColumnFilter( x, row ) );
}

private bool EvaluateColumnFilter( ColumnFilter<FilterExample> columnFilter, FilterExample row )
{
    Console.WriteLine( $"Evaluating... {columnFilter.Column.Field}" );
    Console.WriteLine( $"Filter to apply... {columnFilter.SelectedFilter}" );
    Console.WriteLine( $"Search for... {columnFilter.SearchValue}" );


    //You might need some reflection based or expression based getter to get the value of the corresponding field dynamically
    //Do whatever boolean logic you need to do here
    //We opted to use the DataGrid.Utils.FunctionCompiler.CreateValueGetter to create a dynamic getter for the field and using a simple comparer with the new GreaterThan and LessThan comparisons.
    var columnFieldGetter = DataGrid.Utils.FunctionCompiler.CreateValueGetter<FilterExample>( columnFilter.Column.Field );
    var columnValue = columnFieldGetter( row );

    return CompareFilterValues( columnValue.ToString(), columnFilter.SearchValue, columnFilter.SelectedFilter );

}

private bool CompareFilterValues( string searchValue, string compareTo, MyFilter filterMethod )
{
    switch (filterMethod)
    {
        case MyFilter.StartsWith:
            return searchValue.StartsWith( compareTo, StringComparison.OrdinalIgnoreCase );
        case MyFilter.EndsWith:
            return searchValue.EndsWith( compareTo, StringComparison.OrdinalIgnoreCase );
        case MyFilter.Equals:
            return searchValue.Equals( compareTo, StringComparison.OrdinalIgnoreCase );
        case MyFilter.NotEquals:
            return !searchValue.Equals( compareTo, StringComparison.OrdinalIgnoreCase );
        case MyFilter.GreaterThan:
            if (int.TryParse( searchValue, out var parsedSearchValue ) && int.TryParse( compareTo, out var parsedCompareToValue ))
            {
                return parsedSearchValue > parsedCompareToValue;
            }
            return false;
        case MyFilter.LessThan:
            if (int.TryParse( searchValue, out var parsedSearchValueLessThan ) && int.TryParse( compareTo, out var parsedCompareToValueLessThan ))
            {
                return parsedSearchValueLessThan < parsedCompareToValueLessThan;
            }
            return false;
        case MyFilter.Contains:
        default:
            return searchValue.IndexOf( compareTo, StringComparison.OrdinalIgnoreCase ) >= 0;
    }
}
```

### Read Data
Optionally by using the Datagrid's `ReadData` feature this example still holds true, as you hold the filtering logic in your own hands. Of course you will have to do your own translation in order to make it work with your backend.

## Conclusion

In an ideal world, component libraries do most of the heavy lifting for us, but it's not unusual for a library to sometimes have certain gaps in functionality. This shows that with a little creativity we can still use our favorite libraries and enhance them in order to accomplish our use cases.

We'll definitely keep improving the `Datagrid`filtering in future versions, but in the meantime, we leave you with this alternative to improve the filtering capabilities of your `DataGrid`.

You can find the full code example by visiting the [following github issue](https://github.com/Megabit/Blazorise/issues/4941#issuecomment-1711836031).