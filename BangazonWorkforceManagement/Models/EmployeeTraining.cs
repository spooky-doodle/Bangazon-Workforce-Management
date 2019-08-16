﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforceManagement.Models
{
    public class EmployeeTraining
    {
        public int Id { get; set; }

        [Required]
        public int EmployeeId { get; set; }
        [Required]
        public int TrainingProgramId { get; set; }
    }
}
