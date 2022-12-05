using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Medicine
    {
        public int Id { set; get; }
        public string? DispatchCode { set; get; }
        public int DroneId { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9-_ ]+$", ErrorMessage = "Only letters, numbers, ‘-‘, ‘_’ allow in medication name")]
        public string? Name { set; get; }
        public double Weight { set; get; }

        [RegularExpression(@"^[A-Z0-9_]+$", ErrorMessage = "Only upper case letters,numbers and (_) allow in medication code")]
        public string? Code { set; get; }
        public byte[]? Image { get; set; }
    }
}
