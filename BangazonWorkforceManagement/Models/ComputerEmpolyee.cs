using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforceManagement.Models
{
    public class ComputerEmpolyee
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }

        public int ComputerId { get; set; }

        public DateTime AssignDate { get; set; }

        public DateTime? UnassignDate { get; set; } = null;
    }
}
