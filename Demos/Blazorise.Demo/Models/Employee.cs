using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Blazorise.Demo.Models
{
    public class Employee
    {
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string City { get; set; }
        public string Zip { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public int? Childrens { get; set; }
        public string Gender { get; set; }
        public decimal Salary { get; set; }
        public bool IsActive { get; set; }

        public List<Salary> Salaries { get; set; } = new();
    }
}