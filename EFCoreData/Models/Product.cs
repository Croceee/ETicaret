using System;
using System.Collections.Generic;

#nullable disable

namespace EFCoreData.Models
{
    public partial class Product
    {
        public Product()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public bool? Deleted { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public string ImageUrl { get; set; }
        public int? ProductTypeId { get; set; }
        public decimal? Price { get; set; }

        public virtual ProductType ProductType { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
