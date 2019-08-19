using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using BangazonWorkforceManagement.Models;

namespace BangazonAPI.Models
{
    public class Computer
    {
        public int Id { get; set; }
        [Required]
        public DateTime PurchaseDate { get; set; }


        public DateTime? DecommissionDate { get; set; } = null;

        [Required]
        public string Make { get; set; }
        [Required]

        public string Manufacturer { get; set; }

        public Employee Employee { get; set; }

    }
}
