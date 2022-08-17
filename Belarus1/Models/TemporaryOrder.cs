using System;
using System.Collections.Generic;

namespace Belarus1
{
    public partial class TemporaryOrder
    {
        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public int FoodId { get; set; }
        public int Amount { get; set; }

        public virtual Food Food { get; set; } = null!;
        public virtual AspNetUser User { get; set; } = null!;
    }
}
