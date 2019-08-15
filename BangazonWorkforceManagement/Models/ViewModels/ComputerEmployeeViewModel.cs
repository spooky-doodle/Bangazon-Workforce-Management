using BangazonAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.WebPages.Html;

namespace BangazonWorkforceManagement.Models.ViewModels
{
    public class ComputerEmployeeViewModel
    {
        public Computer Computer { get; set; }

        public List<SelectListItem> Employees { get; internal set; }
    }
}
