using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class DispatchMedicine
    {
        public int Id { get; set; }

        [Required]
        public int DroneId { get; set; }
        public DateTime DispatchDate { get; set; }
        public string? DeliveryFrom { get; set; }
        public string? DeliveryTo { get; set; }
        public string? DroneControlBy { get; set; }
        public string? DispatchStatus { get; set; }
        public List<Medicine>? Medications { set; get; }

    }
}
