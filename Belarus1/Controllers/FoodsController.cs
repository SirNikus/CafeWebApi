#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Belarus1;
using Belarus1.Models;
using Belarus1.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Authorization;

namespace Belarus1.Controllers
{
    public class FoodsController : Controller
    {
        private readonly AUTHDBContext _context;
        UserManager<BelarusUser> _userManager;
        public static List<Korzina> korzinas = new List<Korzina>();
        public static List<int> korz = new List<int>();

        public FoodsController(AUTHDBContext context, UserManager<BelarusUser> userManager)
        {
            _userManager = userManager;
            _context = context;
        }


        private string GetUser()
        {
            var userId = _userManager.GetUserId(User);
            return userId;
        }
        [JSInvokable]
        public JsonResult GetData()
        {
            var userId = GetUser();

            return Json(_context.TemporaryOrders.Where(p=>p.UserId==userId).Select(p=>p.FoodId+p.Amount));
        }
        [JSInvokable]
        public JsonResult PlusKorzina(int id)
        {
            var userId = GetUser();
            var item = _context.TemporaryOrders.Where(p => p.UserId == userId && p.FoodId == id).FirstOrDefault();// korzinas.Where(p => p.FoodID == id).FirstOrDefault();
            if (item == null)
			{
                return Json(0);
            }
            item.Amount++;
            _context.SaveChanges();
            return Json(item.Amount);
        }
        
        public JsonResult MinusKorzina(int id)
		{
            var userId = GetUser();
            var item = _context.TemporaryOrders.Where(p => p.UserId == userId && p.FoodId==id);// korzinas.Where(p => p.FoodID == id).FirstOrDefault();
			var govno = _context.TemporaryOrders.Where(p => p.UserId == userId && p.FoodId==id).FirstOrDefault();// korzinas.Where(p => p.FoodID == id).FirstOrDefault();
			
            if (item == null)
			{
				return Json(0);
			}
			if (govno.Amount == 1)
			{
				_context.TemporaryOrders.Remove(govno);
                _context.SaveChanges();
				return Json(0);
			}
			else
			{
                var amount = item.FirstOrDefault();
                amount.Amount--;
                _context.SaveChanges();
                return Json(amount.Amount);
			}
		}
       
		public JsonResult GetKorzina(int id)
        {
            var userId = GetUser();
            var govno = _context.TemporaryOrders.Where(p => p.UserId == userId);
            int kal=govno.Count();
            var item = _context.TemporaryOrders.Where(p => p.UserId == userId && p.FoodId == id);//.Select(p=>p.Amount).FirstOrDefault();
            int check = item.Count();
            if (item.Count()==0)
            {

                _context.TemporaryOrders.Add(new TemporaryOrder
                {
                    
                    UserId = userId,
                    FoodId = id,
                    Amount = 1
                }) ;
                _context.SaveChanges();
            }
            else
            {
                var amount = item.FirstOrDefault();
                amount.Amount++;
                _context.SaveChanges();
            }

            return Json(_context.TemporaryOrders.Where(p => p.UserId == userId).Count());
        }

        public JsonResult GetInfo(Int64 id)
        {
            var value = new List<string>();
            for (long i = 0; i < id; i++)
            {
                value.Add("Элемент № " + i.ToString());
            }

            return Json(value);
        }

        // GET: Foods
        public async Task<IActionResult> Index()
        {
            return View(_context);
        }

        // GET: Foods/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var food = await _context.Foods
                .Include(f => f.Type)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (food == null)
            {
                return NotFound();
            }

            return View(food);
        }
        // GET: Foods/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["TypeId"] = new SelectList(_context.TypeOfFoods, "Id", "Id");
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Price,Weight,Image,TypeId")] Food food)
        {
            if (ModelState.IsValid)
            {
                _context.Add(food);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["TypeId"] = new SelectList(_context.TypeOfFoods, "Id", "Id", food.TypeId);
            return View(food);
        }

        // GET: Foods/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var food = await _context.Foods.FindAsync(id);
            if (food == null)
            {
                return NotFound();
            }
            ViewData["TypeId"] = new SelectList(_context.TypeOfFoods, "Id", "Id", food.TypeId);
            return View(food);
        }

        // POST: Foods/Edit/5
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price,Weight,Image,TypeId")] Food food)
        {
            if (id != food.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(food);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FoodExists(food.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["TypeId"] = new SelectList(_context.TypeOfFoods, "Id", "Id", food.TypeId);
            return View(food);
        }


        // GET: Foods/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var food = await _context.Foods
                .Include(f => f.Type)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (food == null)
            {
                return NotFound();
            }

            return View(food);
        }


        // POST: Foods/Delete/5
        [Authorize(Roles ="Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var food = await _context.Foods.FindAsync(id);
            _context.Foods.Remove(food);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FoodExists(int id)
        {
            return _context.Foods.Any(e => e.Id == id);
        }
    }
}
