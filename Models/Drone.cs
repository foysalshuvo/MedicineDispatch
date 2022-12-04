using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using static CommonEnum.CommonEnum;

namespace Models
{
    public class Drone
    {
        public int Id { get; set; }
       
        [MaxLength(100), MinLength(1)]
        [Required(ErrorMessage = "Drone serial number must be a string with the max lenght 100.")]
        public string? SerialNumber { get; set; }

        [Required]
        [Range(1, 4, ErrorMessage = "Drone model value must be between 1 to 4")]
        public int Model { get; set; }

        [Required]
        [Range(1,500, ErrorMessage = "Drone weight must be range between 1 to 500 gram")]
        [RegularExpression(@"^\d+(\.\d{1,2})?$")]
        public double Weight { get; set; }

        public DateTime RegistrationDate { get; set; }
        public string? DroneState { get; set; }
        public double DroneBatteryPercentage { get; set; }
    }
}
