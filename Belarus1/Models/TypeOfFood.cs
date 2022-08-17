using System;
using System.Collections.Generic;

namespace Belarus1
{
    public partial class TypeOfFood
    {
        public TypeOfFood()
        {
            Foods = new HashSet<Food>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public virtual ICollection<Food> Foods { get; set; }
    }
}
