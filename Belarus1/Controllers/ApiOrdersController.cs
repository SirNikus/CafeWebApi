using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Belarus1;
using Microsoft.AspNetCore.Identity;
using Belarus1.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Belarus1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiOrdersController : ControllerBase
    {
        private readonly AUTHDBContext _context;
        SignInManager<BelarusUser> _signInManager;
        private readonly ILogger<IngredientsController> _logger;
        UserManager<BelarusUser> _userManager;
        private readonly IConfiguration _configuration;

        public ApiOrdersController(UserManager<BelarusUser> userManager, AUTHDBContext context, SignInManager<BelarusUser> signInManager, ILogger<IngredientsController> logger, IConfiguration configuration)
        {
            _configuration = configuration;
            _userManager = userManager;
            _logger = logger;
            _signInManager = signInManager;
            _context = context;
        }

        public class OrderFood
		{
            public int OrderId { get; set; }
            public string FoodName { get; set; }
            public int Amount { get; set; }
		}
        public class SendOrders
        {
            public int Id { get; set; }
            public string userName { get; set; }
            public string? Comment { get; set; }
            public DateTime OrderTime { get; set; }
            public string Code { get; set; }
            
		}
        private string GetUser()
        {
            var userId = _userManager.GetUserId(User);
            return userId;
        }

        [Route("orderFood")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderFood>>> GetOrderFoods()
        {
            List<OrderFood> orderFoods = new List<OrderFood>();
            foreach(var item in _context.FoodOrders)
			{
                if (!_context.Orders.Where(p => p.Id == item.Id).Select(p => p.Status).FirstOrDefault())
				{
                    orderFoods.Add(new OrderFood
                    {
                        OrderId = item.OrderId,
                        FoodName = _context.Foods.Where(p => p.Id == item.FoodId).Select(p => p.Name).FirstOrDefault(),
                        Amount=item.Amount
                    });
				}
			}
            return orderFoods.ToList();
        }
            // GET: api/ApiOrders
        [Route("orders")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SendOrders>>> GetOrders()
        {
            List<SendOrders> sendOrders = new List<SendOrders>();
            List<OrderFood> tempOrd = new List<OrderFood>();
          if (_context.Orders == null)
          {
              return NotFound();
          }

          foreach(var item in _context.Orders.Where(p=>p.Status!=true))
			{
                var userName = _context.AspNetUsers.Where(p => p.Id == item.UserId).Select(p => p.Name).FirstOrDefault();
                var userMail = _context.AspNetUsers.Where(p => p.Id == item.UserId).Select(p => p.UserName).FirstOrDefault();
                
                sendOrders.Add(new SendOrders
                {
                    Id = item.Id,
                    userName = userName,
                    Comment = item.Comment,
                    OrderTime = item.OrderTime,
                    Code = item.Code,
                    
                });
			}
            return sendOrders.ToList();
        }
        
        public class CompleteOrder
		{
            public int id { get; set; }
		}
        [Authorize(Roles = "Admin")]
        [Route("complete")]
        [HttpPost]
        public async Task<IActionResult> PutOrder(CompleteOrder order)
        {
            var item = _context.Orders.Where(p => p.Id == order.id).FirstOrDefault();
            if (item == null)
            {
                return BadRequest();
            }
            else
            {
                item.Status = true;
                await _context.SaveChangesAsync();
            }
            return NoContent();
        }
        public class GetOrder
        {
            public string userName { get; set; }
            public string Comment { get; set; }
            public List<KorzinaList> korzinaLists { get; set; }
        }
        public class KorzinaList
		{
            public int FoodId { get; set; }
            public int Amount { get; set; }
		}
        // POST: api/ApiOrders
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<GetOrder>> PostOrder(GetOrder order)
        {
          if (_context.Orders == null)
          {
              return Problem("Entity set 'AUTHDBContext.Orders'  is null.");
          }
          
            var userName= _userManager.GetUserName(User);
            var userId = _context.AspNetUsers.Where(p=>p.UserName==userName).Select(p=>p.Id).FirstOrDefault();
            Random rnd = new Random();
            string Code = rnd.Next(1000, 99999).ToString();
            _context.Orders.Add(new Order
			{
                UserId=userId,
                Comment=order.Comment,
                OrderTime=DateTime.Now,
                Status=false,
                Code= Code,
            });
            await _context.SaveChangesAsync();
            int orderId = _context.Orders.Where(p => p.UserId == userId && p.Code == Code).Select(p => p.Id).FirstOrDefault();
            foreach(var food in order.korzinaLists)
			{
                _context.FoodOrders.Add(new FoodOrder
                {
                    OrderId=orderId,
                    FoodId=food.FoodId,
                    Amount=food.Amount
                });
                await _context.SaveChangesAsync();
			}
            return CreatedAtAction("GetOrder",order);
        }

    }
}
