---
title: Blazorise Select component with primitive and complex types
description: In this blog post we will look at how to use Blazorise Select Component to bind both primitive and complex types, the select element's limitations, and how to work around them.
permalink: /blog/how-to-handle-select-with-primitive-and-complex-types
canonical: /blog/how-to-handle-select-with-primitive-and-complex-types
image-url: img/blog/2022-06-25/Blazorise-Select-Component.png
image-title: How to handle binding of primitive and complex types with Blazorise Select component
author-name: David Moreira
author-image: david
posted-on: July 1st, 2022
read-time: 4 min
---

# Using the primitive and complex types with Blazorise Select component

Blazorise's Select component represents a dropdown list and is built upon the standard HTML Select element. See [HTML Select](https://developer.mozilla.org/en-US/docs/Web/HTML/Element/select).

This has the advantage of supporting the standard and semantic way of building a select element and dealing with any limitations the element may have.

In this blog post we will look at how to use [Blazorise Select Component](docs/components/select) to bind both primitive and complex types, the `<select>` element's limitations, and how to work around them.

## What are Primitive and Complex Types?

Before we start, we must first cover the differences between what primitive and complex types represent.

### Primitive Types

The primitive types are predefined by the language and they are named by reserved keywords. They represent the basic types of the language. In C#, The most famous primitive data types are: `int`, `object`, `short`, `char`, `float`, `double`, `char`, `bool`. They are called primitive because they are the main built-in types, and could be used to build other data types.

### Complex Types

The complex types are all built from a combination of primitive types.  Usually, they are represented as a `class` or a `struct` object. So whenever you create a custom class like `Employee`, or `Student` that is a complex type.

## How to bind Primitive Type?

For primitive types it's pretty simple to build the select. Let's go ahead and build a dropdown-list list with four employees, where their value is their employee Id, an int type.

```html|SelectComponentWithPrimitiveTypeExample.razor
@namespace Blazorise.Docs.Pages.Blog.UsingTheSelectComponent.Examples

<Row>
    <Column>
        <Field>
            <Select TValue="int">
                <SelectItem Value="11500">John</SelectItem>
                <SelectItem Value="11566">Julia</SelectItem>
                <SelectItem Value="11612">Maria</SelectItem>
                <SelectItem Value="10989">Peter</SelectItem>
            </Select>
        </Field>
    </Column>
</Row>
```

## How to bind Enumeration Type?

Although still a primitive type, an enumeration type can basically represent two values, the underlying numeric representation and the text definition. This matters because depending on the javascript serialization settings the value might not come back with the expected representation and fail to successfully bind the selected value.

Here's an example of how to properly handle an enumeration type, by configuring the settings appropriately

```html|SelectComponentWithEnumTypeExample.razor
@namespace Blazorise.Docs.Pages.Blog.UsingTheSelectComponent.Examples
@using System.Text.Json.Serialization

<Row>
    <Column>
        <Field>
            <Select TValue="Day" @bind-SelectedValue="@selectedDay">
                @foreach ( var enumValue in Enum.GetValues<Day>() )
                {
                    <SelectItem @key="enumValue" Value="@enumValue">@enumValue</SelectItem>
                }
            </Select>
        </Field>
    </Column>
</Row>
<Row>
    <Column>
        Selected Day is : @selectedDay.ToString("g")
    </Column>
</Row>

@code {
    public Day selectedDay;

    [Flags]
    [JsonConverter( typeof( System.Text.Json.Serialization.JsonStringEnumConverter ) )]
    public enum Day
    {
        None = 0,
        Sunday = 1,
        Monday = 2,
        Tuesday = 4,
        Wednesday = 8,
        Thursday = 16,
        Friday = 32,
        Saturday = 64
    }
}
```

## How to bind Complex Type?

Blazorise doesn't currently support binding a complex type directly because the `<select>` element has no way of uniquely identifying an item of a complex type, so we have to identify the item ourselves uniquely.

The main problem with complex types is that no built-in serialization could convert complex type values to a string representation. Remember, a `Select` component working with a native `<select>` HTML element means that all its `<option>` elements will in the end contain a string value.

The easiest and most recommended way to work with complex types is to use one of its fields as a `SelectItem` value.

Let's look at how a dropdown list with four employees would look, now with actual complex types.

```html|SelectComponentWithComplexTypeExample.razor
@namespace Blazorise.Docs.Pages.Blog.UsingTheSelectComponent.Examples

<Row>
    <Column>
        <Field>
            <Select TValue="int" SelectedValueChanged="@(value => selectedEmployee = employeeData.First(emp => emp.Id == value))">
                @foreach ( var employee in employeeData )
                {
                    <SelectItem @key="employee.Id" Value="@employee.Id">@employee.Name</SelectItem>
                }
            </Select>
        </Field>
    </Column>
</Row>
<Row>
    <Column>
        Selected Employee is : @selectedEmployee.Name
    </Column>
</Row>

@code {
    public Employee selectedEmployee;
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public List<Employee> employeeData;

    protected override void OnInitialized()
    {
        employeeData = new()
        {
            new (){ Id = 11500, Name = "John" },
            new (){ Id = 11566, Name = "Julia" },
            new (){ Id = 11612, Name = "Maria" },
            new (){ Id = 10989, Name = "Peter" }
        };
        selectedEmployee = employeeData.First();
        base.OnInitialized();
    }
}
```

## How to bind Null values?

Having a null value is a valid option when you want to consider the absense of a value, for example when handling a nullable type like `int?`

However handling null values with the select element can be tricky, and this has to do with how browsers work in general. When you have an empty value for the option element it will take its text or content and use that for the selected value.

So the option should be left empty, let's see this in action with a nullable type.

```html|SelectComponentWithNullableTypeExample.razor
@namespace Blazorise.Docs.Pages.Blog.UsingTheSelectComponent.Examples

<Row>
    <Column>
        <Field>
            <Select TValue="int?" @bind-SelectedValue="@selectedEmployeeId">
                <SelectItem Value="(int?)null"></SelectItem>
                <SelectItem Value="11500">John</SelectItem>
                <SelectItem Value="11566">Julia</SelectItem>
                <SelectItem Value="11612">Maria</SelectItem>
                <SelectItem Value="10989">Peter</SelectItem>
            </Select>
        </Field>
    </Column>
</Row>

<Row>
    <Column>
        Selected Employee Id is : @(selectedEmployeeId.HasValue ? selectedEmployeeId.Value : "empty")
    </Column>
</Row>

@code {
    private int? selectedEmployeeId = null;
}
```

## Conclusion

Thank you for your time! We just looked at the Blazorise Select component's features and the distinctions between primitive and complex types.

Download our [Blazorise NuGet](https://www.nuget.org/profiles/Megabit) to try our Blazor components or purchase our [commercial Blazorise web](commercial/) license to gain access to our support forum. To learn more about other available features, please see our online examples and [documentation](docs).