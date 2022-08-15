using System;
using System.Collections.Generic;

#nullable disable

namespace EFCoreData.Models
{
    public partial class User
    {
        public User()
        {
            Orders = new HashSet<Order>();
        }

        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string SurName { get; set; }
        public bool? Deleted { get; set; }
        public DateTime? LastUpdateDate { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}
