using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Belarus1;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Belarus1.Models;
using Belarus1.Areas.Identity.Data;
using Microsoft.JSInterop;

namespace Belarus1.Controllers
{

    public class OrdersController : Controller
    {
        private readonly AUTHDBContext _context;
        UserManager<BelarusUser> _userManager;
        RoleManager<IdentityRole> _roleManager;
        SignInManager<BelarusUser> _signInManager;
        private readonly ILogger<OrdersController> _logger;

		public OrdersController(AUTHDBContext context, UserManager<BelarusUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<BelarusUser> signInManager, ILogger<OrdersController> logger)
		{
            _logger = logger;
			_signInManager = signInManager;
			_context = context;
			_userManager = userManager;
			_logger = logger;
            _roleManager = roleManager;
		}

		private string GetUser()
		{
			var userId = _userManager.GetUserId(User);
            return userId;
		}


        public void CreateOrder()
		{
			var rnd = new Random();
            

            _context.Orders.Add(new Order
			{
                UserId = GetUser(),
				Comment = "",
                OrderTime=DateTime.Now,
				Status = false,
				Code = rnd.Next(1000, 99999).ToString(),
			});
            
			_context.SaveChanges();
            var orderId = _context.Orders.OrderBy(p=>p.Id).Select(p => p.Id).LastOrDefault();
			foreach (var food in _context.TemporaryOrders.Where(p => p.UserId == GetUser()))
			{
                _context.FoodOrders.Add(new FoodOrder
                {
                   
                    OrderId = orderId,
                    FoodId = food.FoodId,
                    Amount = food.Amount
                });
			}
            _context.SaveChanges();
            ClearOrders();
		}
       
        public JsonResult UpdateTotalPrice()
		{
            var userId = GetUser();
            
            float price=0;
            foreach (var item in _context.TemporaryOrders.Where(p=>p.UserId==userId))
			{
                price += item.Amount * (float)_context.Foods.Where(p => p.Id == item.FoodId).Select(p => p.Price).FirstOrDefault();
			}
            return Json(price);
		}
        
        public void  ClearOrders()
		{
            var userId = GetUser();
            var item = _context.TemporaryOrders.Where(p => p.UserId == userId);
            _context.TemporaryOrders.RemoveRange(item);
            _context.SaveChanges();
		}
        
        public JsonResult UpdateTotalAmount()
        {
            var userId = GetUser();
            float amount = 0;
            foreach (var item in _context.TemporaryOrders.Where(p => p.UserId == userId))
            {
                amount += item.Amount;
            }
            return Json(amount);
        }
        
        public JsonResult UpdatePrices(int id,int amount)
        {
			try
			{
                var userId = GetUser();
                double itog = Convert.ToDouble( _context.Foods.Where(p => p.Id == id).Select(p => p.Price).FirstOrDefault());
                itog *= amount;
                return Json(itog);
			}
			catch
			{
                return Json(0);
			}
        }
        // GET: Orders
       [Authorize]
        public async Task<IActionResult> Index()
        {
            var aUTHDBContext = _context.Orders.Include(o => o.User);
            return View(_context);
        }


        // GET: Orders/Details/5
        [Authorize]
        public async Task<IActionResult> Details()
        {
            var order = _context.Orders.Where(p => p.UserId == GetUser());
            if (order == null)
            {
                return NotFound();
            }
            return View(_context);
        }

        // GET: Orders/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.AspNetUsers, "Id", "Id");
            return View();
        }

        // POST: Orders/Create
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,Comment,OrderTime,Status,Code")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.AspNetUsers, "Id", "Id", order.UserId);
            return View(order);
        }

        // GET: Orders/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.AspNetUsers, "Id", "Id", order.UserId);
            return View(order);
        }

        // POST: Orders/Edit/5
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,Comment,OrderTime,Status,Code")] Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
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
            ViewData["UserId"] = new SelectList(_context.AspNetUsers, "Id", "Id", order.UserId);
            return View(order);
        }


        // GET: Orders/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Orders == null)
            {
                return Problem("Entity set 'AUTHDBContext.Orders'  is null.");
            }
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
          return (_context.Orders?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
