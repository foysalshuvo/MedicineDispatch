using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    [DataContract]
    public class Drone
    {
        public int Id { get; set; }

        [DataMember(IsRequired = true)]
        [StringLength(100, MinimumLength = 4)]
        public string SerialNumber { get; set; }

        [DataMember(IsRequired = true)]
        public string Model { get; set; }

        [DataMember(IsRequired = true)]
        public double Weight { get; set; }

        [DataMember(IsRequired = true)]
        public double Battery { get; set; }

        [DataMember(IsRequired = true)]
        public string State { get; set; }

        [DataMember(IsRequired = true)]
        public bool IsActive { get; set; }

    }
}
