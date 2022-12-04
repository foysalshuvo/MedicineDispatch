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
        public string? DispatchCode { get; set; }

        [Required]
        public int DroneId { get; set; }
        public DateTime DispatchDate { get; set; }
        public string? DeliveryFrom { get; set; }
        public string? DeliveryTo { get; set; }
        public string? DroneControlBy { get; set; }
     
        [Required]
        [Range(1, 100, ErrorMessage = "Drone battery capacity be range between 1 to 100 percentage")]
        public double BatterCapacity { get; set; }

        [Required]
        [Range(1, 6, ErrorMessage = "Drone state value must be between 1 to 6")]
        public int DroneState { get; set; }

        public List<Medicine>? Medications { set; get; }

    }
}
