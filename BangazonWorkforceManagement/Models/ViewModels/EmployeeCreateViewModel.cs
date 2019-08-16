using BangazonAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.WebPages.Html;

namespace BangazonWorkforceManagement.Models.ViewModels
{
    public class EmployeeDepartmentViewModel
    {
        public List<SelectListItem> Departments { get; set; }
        public Employee Employee { get; set; }
    }
}
