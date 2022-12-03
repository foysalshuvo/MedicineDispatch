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
        public string? DispatchCode { set; get;}
        public string? Name { set; get; }
        public double Weight { set; get; }
        public string? Code { set; get; }
        public byte[]? Image { get; set; }
    }
}
