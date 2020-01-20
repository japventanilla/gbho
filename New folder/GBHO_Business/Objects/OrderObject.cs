using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GBHO_Business.Objects
{
    public class OrderObject
    {
        public long OrderId { get; set; }
        public DateTime OrderDate { get; set; } 
        public decimal Amount { get; set; }
        public int Points { get; set; }
        public string Status { get; set; }
        public string OrderBy { get; set; }
    }
}
