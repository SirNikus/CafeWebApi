using System;
using System.Collections.Generic;

namespace Belarus1
{
    public partial class Order
    {
        public Order()
        {
            FoodOrders = new HashSet<FoodOrder>();
        }

        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public string? Comment { get; set; }
        public DateTime OrderTime { get; set; }
        public bool Status { get; set; }
        public string Code { get; set; } = null!;

        public virtual AspNetUser User { get; set; } = null!;
        public virtual ICollection<FoodOrder> FoodOrders { get; set; }
    }
}
