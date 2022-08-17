using System;
using System.Collections.Generic;

namespace Belarus1
{
    public partial class Ingredient
    {
        public Ingredient()
        {
            FoodIngredients = new HashSet<FoodIngredient>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public virtual ICollection<FoodIngredient> FoodIngredients { get; set; }
    }
}
