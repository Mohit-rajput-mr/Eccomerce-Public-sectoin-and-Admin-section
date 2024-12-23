using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OSP.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string DeliveryMethod { get; set; }
        public decimal DeliveryCharge { get; set; }
        public string DeliveryCountry { get; set; }
        public string DeliveryCity { get; set; }
        public string PostalCode { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }
    }
}
