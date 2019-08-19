using BangazonAPI.Models;
using BangazonWorkforceManagement.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace BangazonWorkforceManagement.Models
{
    public class ComputerEmployeeViewModel
    {
        public Computer Computer { get; set; }

        public List<SelectListItem> Employees { get; set; }

        public int EmployeeId { get; set; }

    }

}

