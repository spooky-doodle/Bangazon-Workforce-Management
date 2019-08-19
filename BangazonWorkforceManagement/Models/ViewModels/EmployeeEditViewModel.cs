using BangazonAPI.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace BangazonWorkforceManagement.Models.ViewModels
{
    public class EmployeeEditViewModel
    {
        public List<SelectListItem> Departments { get; set; }

        public Employee Employee { get; set; }

        public List<SelectListItem> Computers { get; set; }

        public List<string> CompIds { get; set; }

        public List<SelectListItem> EmpComps { get; set; }

        public MultiSelectList Comps { get; set; }

      
    }

 
}