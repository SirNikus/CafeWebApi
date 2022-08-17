using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Belarus1;
using Microsoft.AspNetCore.Authorization;

namespace Belarus1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FoodsApiController : ControllerBase
    {
        private readonly AUTHDBContext _context;

        public FoodsApiController(AUTHDBContext context)
        {
            _context = context;
        }

        public class Fods
		{
            public int id { get; set; }
            public string name { get; set; }
            public float price { get; set; }
            public float weight { get; set; }
            public List<string> ingredients { get; set; }
            public string type { get; set; }
            public byte[] image { get; set; }

        }

        List<Fods> fods = new List<Fods>();
        private List<Fods> setFods()
		{
            foreach(var item in _context.Foods)
			{
                fods.Add(new Fods
                {
                    id = item.Id,
                    name = item.Name,
                    price = (float)item.Price,
                    weight = (float)item.Weight,
                    image = item.Image,
                    ingredients = _context.FoodIngredients.Where(p=>p.FoodId==item.Id).Select(p => p.Ingredient.Name).ToList(),
                    type = _context.TypeOfFoods.Where(p=>p.Id==item.TypeId).Select(p=>p.Name).FirstOrDefault(),
                });
			}
            return fods;
		}

        // GET: api/FoodsApi
        [HttpGet]
        public async Task<ActionResult<List<Fods>>> GetFoods()
        {
          if (_context.Foods == null)
          {
              return NotFound();
          }
          setFods();
            return fods;  
        }

        // GET: api/FoodsApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Food>> GetFood(int id)
        {
          if (_context.Foods == null)
          {
              return NotFound();
          }
            var food = await _context.Foods.FindAsync(id);

            if (food == null)
            {
                return NotFound();
            }

            return food;
        }

       public class TempFood
		{
            public string name { get; set; }
            public float price { get; set; }
            public float weight { get; set; }
            public List<string> ingredients { get; set; }
            public string type { get; set; }
            public byte[] image { get; set; }
        }

        // POST: api/FoodsApi
        [Authorize(Roles ="Admin")]
        [Route("postFood")]
        [HttpPost]
        public async Task<ActionResult<TempFood>> PostFood(TempFood food)
        {
          
            _context.Foods.Add(new Food
			{
                Name=food.name,
                Price=(decimal)food.price,
                Weight=food.weight,
                Image=food.image,
                TypeId=_context.TypeOfFoods.Where(p=>p.Name==food.type).Select(p=>p.Id).FirstOrDefault(),
			});

            await _context.SaveChangesAsync();

            foreach(var item in food.ingredients)
			{
                _context.FoodIngredients.Add(new FoodIngredient
                {
                    FoodId = _context.Foods.Where(p => p.Name == food.name).Select(p => p.Id).FirstOrDefault(),
                    IngredientId = _context.Ingredients.Where(p => p.Name == item).Select(p => p.Id).FirstOrDefault(),
                });
                await _context.SaveChangesAsync();
			}

            

            return CreatedAtAction("GetFood",food);
        }
        public class ChangedFood
        {
            public int id { get; set; }
            public string name { get; set; }
            public float price { get; set; }
            public float weight { get; set; }
            public List<string> ingredients { get; set; }
            public string type { get; set; }
            public byte[] image { get; set; }
        }

        [Authorize(Roles = "Admin")]
        [Route("put")]
        [HttpPost]
        public async Task<IActionResult> PutFood(ChangedFood food)
        {
            var item = _context.Foods.Where(p => p.Id == food.id).FirstOrDefault();
            _context.FoodIngredients.RemoveRange(_context.FoodIngredients.Where(p => p.FoodId == food.id));

            item.Name = food.name;
            item.Price = (decimal)food.price;
            item.Weight = food.weight;
            item.Image = food.image;
            item.TypeId = _context.TypeOfFoods.Where(p => p.Name == food.type).Select(p => p.Id).FirstOrDefault();
            foreach(var ing in food.ingredients)
			{
                _context.FoodIngredients.Add(new FoodIngredient
                {
                    IngredientId = _context.Ingredients.Where(p => p.Name == ing).Select(p => p.Id).FirstOrDefault(),
                    FoodId=food.id
                });
			}
            _context.SaveChanges();

            return NoContent();
        }

        public class DelFood
		{
            public int id { get; set; }
		}


        // DELETE: api/FoodsApi/5
        [Authorize(Roles ="Admin")]
        [Route("del")]
        [HttpPost]
        public async Task<IActionResult> DeleteFood(DelFood delFood)
        {
            if (_context.Foods == null)
            {
                return NotFound();
            }
            var food = await _context.Foods.FindAsync(delFood.id);
            if (food == null)
            {
                return NotFound();
            }
            foreach(var item in _context.FoodIngredients.Where(p=>p.FoodId==delFood.id))
			{
                _context.FoodIngredients.Remove(item);
			}
            _context.Foods.Remove(food);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
