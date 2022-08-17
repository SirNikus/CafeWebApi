using Belarus1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Belarus1.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly AUTHDBContext _context;

		public HomeController(ILogger<HomeController> logger, AUTHDBContext context)
		{
			_context = context;
			_logger = logger;
		}

		public  class Fod
		{
			public int Id { get; set; }
			public int Amount { get; set; }
		}
		public static List<Fod> fod=new List<Fod>();

		private void Load()
		{
			foreach(var item in _context.FoodOrders)
			{
				var check = fod.Where(p => p.Id == item.FoodId).FirstOrDefault();
				if (check==null){
					fod.Add(new Fod
					{
						Id = item.FoodId,
						Amount = item.Amount
					});

				}
				else
				{
					check.Amount += item.Amount;
				}
			}
		}

		public IActionResult Index()
		{
			fod.Clear();
			Load();
			return View(_context);
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}