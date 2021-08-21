using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Blazorise.Docs.Models
{
    public class Employee
    {
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public string Gender { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public decimal Salary { get; set; }
        public decimal Tax { get; set; }
        public bool IsActive { get; set; }
        public DateTime DateOfBirth { get; set; }
        public List<Salary> Salaries { get; set; }
    }

    public class Salary
    {
        public DateTime Date { get; set; }
        public decimal Total { get; set; }
    }

    public static class EmployeeData
    {
        public static List<Employee> EmployeeList = new List<Employee>()
            {
                new()
                {
                    Id = 1,
                    FirstName = "Samuel",
                    LastName = "Collier",
                    Email = "Samuel.Collier62@gmail.com",
                    Salary = 86041,
                    Tax = 86041 * .25m,
                    IsActive = true,
                    DateOfBirth = DateTime.Now.AddYears(-40),
                    Salaries = new(){ new(){ Date = DateTime.Now, Total = 50000 }, new(){ Date = DateTime.Now.AddDays(-31), Total = 30000 }  }
                },
                new()
                {
                    Id = 2,
                    FirstName = "Irvin",
                    LastName = "Ziemann",
                    Email = "Irvin.Ziemann@gmail.com",
                    Salary = 61731,
                    Tax = 61731 * .25m,
                    IsActive = true,
                    DateOfBirth = DateTime.Now.AddYears(-28),
                    Salaries = new(){ new(){ Date = DateTime.Now, Total = 100000 }, new(){ Date =  DateTime.Now.AddDays(-31), Total = 10000 }  }
                },
                new()
                {
                    Id = 3,
                    FirstName = "Gerald",
                    LastName = "Pollich	",
                    Email = "Gerald82@yahoo.com",
                    Salary = 58875,
                    Tax = 58875 * .25m,
                    DateOfBirth = DateTime.Now.AddYears(-65),
                    IsActive = false
                }
            };
    }
}