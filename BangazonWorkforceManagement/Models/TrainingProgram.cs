using BangazonWorkforceManagement.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonAPI.Models
{
    public class TrainingProgram
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        //[DateValidation]
        public DateTime StartDate { get; set; }

        [Required]
        //[DateValidation]
        public DateTime EndDate { get; set; }
        [Required]
        public int MaxAttendees { get; set; }

        public List<Employee> Attendees { get; set; }

    }
}
