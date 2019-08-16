using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonAPI.Models
{
    public class Employee
    {

        [Required]
        [Display(Name = "Employee Id")]
        public int Id { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Department Id")]
        public int DepartmentId { get; set; }

        [Required]
        public bool IsSupervisor { get; set; }

        [Display(Name = "Department Name")]
        public Department Department = new Department();

        public List<Computer> Computers = new List<Computer>();

        public List<TrainingProgram> Trainings = new List<TrainingProgram>();

        [Display(Name = "Full Name")]
        public string FullName

        {
            get
            {
                return $"{FirstName} {LastName}";
            }
        }
    }
}
