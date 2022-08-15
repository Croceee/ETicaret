using System;
using System.Collections.Generic;

#nullable disable

namespace EFCoreData.Models
{
    public partial class Order
    {
        public Order()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int OrderId { get; set; }
        public DateTime? OrderDate { get; set; }
        public int? State { get; set; }
        public string Address { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public Guid? OrderGuid { get; set; }
        public int? UserId { get; set; }

        public virtual User User { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
