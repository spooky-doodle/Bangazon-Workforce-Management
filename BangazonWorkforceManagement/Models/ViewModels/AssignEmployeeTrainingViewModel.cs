using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforceManagement.Models.ViewModels
{
    public class AssignEmployeeTrainingViewModel
    {
        public int EmployeeId { get; set; }

        public int TrainingProgramId { get; set; }

        public List<SelectListItem>  TrainingOptions { get; set; }
    }
}
