using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject
{
    public class Order
    {
        public int OrderId { get; set; }
        public string MemberId { get; set; }
        public  DateTime OrderDate { get; set; }
        public DateTime RequiredDate { get; set; }
        public DateTime ShippedDate { get; set; }
        public int Freight { get; set; }
        [ForeignKey("MemberId")]
        public User? Member { get; set; }
        public IList<OrderDetail>? OrderDetails { get; set; }

    }
}
