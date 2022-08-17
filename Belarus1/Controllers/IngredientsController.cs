using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Belarus1;
using Belarus1.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Security.Principal;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Belarus1.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class IngredientsController : ControllerBase
    {
        private readonly AUTHDBContext _context;
        SignInManager<BelarusUser> _signInManager;
        private readonly ILogger<IngredientsController> _logger;
        UserManager<BelarusUser> _userManager;
        private readonly IConfiguration _configuration;

        public IngredientsController(UserManager<BelarusUser> userManager,AUTHDBContext context, SignInManager<BelarusUser> signInManager, ILogger<IngredientsController> logger, IConfiguration configuration)
        {
            _configuration = configuration;
            _userManager = userManager;
            _logger = logger;
            _signInManager = signInManager;
            _context = context;
        }

        // GET: api/Ingredients
        [Route("getIngr")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Ingredient>>> GetIngredients()
        {
          if (_context.Ingredients == null)
          {
              return NotFound();
          }
            return await _context.Ingredients.ToListAsync();
        }

        [Route("getTypes")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TypeOfFood>>> GetTypes()
        {
            if (_context.TypeOfFoods == null)
            {
                return NotFound();
            }
            return await _context.TypeOfFoods.ToListAsync();
        }

        // GET: api/Ingredients/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Ingredient>> GetIngredient(int id)
        {
          if (_context.Ingredients == null)
          {
              return NotFound();
          }
            var ingredient = await _context.Ingredients.FindAsync(id);

            if (ingredient == null)
            {
                return NotFound();
            }

            return ingredient;
        }

        
        public partial class Userr
        {
            public string login { get; set; }
            public string password { get; set; }
        }

       
        [Authorize(Roles = "Admin")]
        [Route("addType")]
        [HttpPost]
        public async Task<ActionResult<TypeOfFood>> PostIngredient(TypeOfFood type)
        {
            if (_context.TypeOfFoods == null)
            {
                return Problem("Entity set 'AUTHDBContext.Ingredients'  is null.");
            }
            _context.TypeOfFoods.Add(new TypeOfFood
            {
                Name = type.Name
            });

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (IngredientExists(type.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetType", new { id = type.Id }, type);
        }

        // POST: api/Ingredients
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Admin")]
        [Route("addIngredient")]
        [HttpPost]
		public async Task<ActionResult<Ingredient>> PostIngredient(Ingredient ingredient)
		{
			if (_context.Ingredients == null)
			{
				return Problem("Entity set 'AUTHDBContext.Ingredients'  is null.");
			}
			_context.Ingredients.Add(new Ingredient
			{
                Name=ingredient.Name
			});
           
			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateException)
			{
				if (IngredientExists(ingredient.Id))
				{
					return Conflict();
				}
				else
				{
					throw;
				}
			}

			return CreatedAtAction("GetIngredient", new { id = ingredient.Id }, ingredient);
		}

		// DELETE: api/Ingredients/5
		[HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIngredient(int id)
        {
            if (_context.Ingredients == null)
            {
                return NotFound();
            }
            var ingredient = await _context.Ingredients.FindAsync(id);
            if (ingredient == null)
            {
                return NotFound();
            }

            _context.Ingredients.Remove(ingredient);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool IngredientExists(int id)
        {
            return (_context.Ingredients?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
