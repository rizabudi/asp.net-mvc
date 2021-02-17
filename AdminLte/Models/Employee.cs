using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AdminLte.Models
{
    //public enum Department
    //{
    //    Software, Finance, Admin, Marketing, Research, HR
    //}

    public class EmployeeGroup
    {
        [Key]
        public int GroupID { get; set; }

        [Required]
        public string Name { get; set; }
    }

    public class Employee
    {
        [Key]
        public int EmployeeId { get; set; }

        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Position { get; set; }
        [Required]
        public string JobTitle { get; set; }
        [Required]
        public Department Department { get; set; }
        [Required]
        public EmployeeGroup Group { get; set; }

        public double Salary { get; set; }

        public DateTime DateJoined { get; set; }
        public DateTime LastUpdated {get;set;}

    }
}
