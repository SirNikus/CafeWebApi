using System;
using System.Collections.Generic;

namespace Belarus1
{
    public partial class FoodIngredient
    {
        public int FoodIngredient1 { get; set; }
        public int IngredientId { get; set; }
        public int FoodId { get; set; }

        public virtual Food Food { get; set; } = null!;
        public virtual Ingredient Ingredient { get; set; } = null!;
    }
}
