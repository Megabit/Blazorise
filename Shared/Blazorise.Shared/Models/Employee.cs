using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Blazorise.Shared.Models;

public class Employee
{
    public Employee()
    {

    }

    public Employee( Employee other )
    {
        Id = other.Id;
        Childrens = other.Childrens;
        DateOfBirth = other.DateOfBirth;
        City = other.City;
        Email = other.Email;
        FirstName = other.FirstName;
        LastName = other.LastName;
        Gender = other.Gender;
        IsActive = other.IsActive;
        Salaries = other.Salaries;
        Salary = other.Salary;
        Tax = other.Tax;
        Zip = other.Zip;
    }

    [Display( Name = "Id" )]
    public int Id { get; set; }

    [Required]
    [Display( Name = "First Name" )]
    public string FirstName { get; set; }

    [Required]
    [Display( Name = "Last Name" )]
    public string LastName { get; set; }

    [Required]
    [EmailAddress]
    [Display( Name = "Email" )]
    public string Email { get; set; }

    [Display( Name = "City" )]
    public string City { get; set; }

    [Display( Name = "Zip" )]
    public string Zip { get; set; }

    [Display( Name = "DOB" )]
    public DateTime? DateOfBirth { get; set; }

    [Display( Name = "Childrens" )]
    public int? Childrens { get; set; }

    [Display( Name = "Gender" )]
    public string Gender { get; set; }

    [Display( Name = "Salary" )]
    public decimal Salary { get; set; }

    [Display( Name = "Tax" )]
    public decimal Tax
    {
        get
        {
            if ( tax == 0 && Salary > 0 )
                tax = Salary * TaxPercentage;
            return tax;
        }
        set { tax = value; }
    }

    [Display( Name = "Active" )]
    public bool IsActive { get; set; }

    public List<Salary> Salaries { get; set; } = new();

    [Display( Name = "Children Per Salary" )]
    public decimal ChildrensPerSalary
        => Salary == 0m
        ? 0m
        : ( Childrens is null || Childrens == 0 ? 1 : Childrens.Value ) / Salary;

    public decimal TaxPercentage = 0.25m;

    private decimal tax;
}