using System;
using System.Collections.Generic;

namespace Belarus1
{
    public partial class Food
    {
        public Food()
        {
            FoodIngredients = new HashSet<FoodIngredient>();
            FoodOrders = new HashSet<FoodOrder>();
            TemporaryOrders = new HashSet<TemporaryOrder>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public double Weight { get; set; }
        public byte[]? Image { get; set; }
        public int TypeId { get; set; }

        public virtual TypeOfFood Type { get; set; } = null!;
        public virtual ICollection<FoodIngredient> FoodIngredients { get; set; }
        public virtual ICollection<FoodOrder> FoodOrders { get; set; }
        public virtual ICollection<TemporaryOrder> TemporaryOrders { get; set; }
    }
}
