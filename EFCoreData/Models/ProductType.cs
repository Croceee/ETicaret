using System;
using System.Collections.Generic;

#nullable disable

namespace EFCoreData.Models
{
    public partial class ProductType
    {
        public ProductType()
        {
            Products = new HashSet<Product>();
        }

        public int ProductTypeId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public bool? Deleted { get; set; }
        public DateTime? LastUpdateDate { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
