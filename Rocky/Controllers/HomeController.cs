using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rocky.Data;
using Rocky.Models;
using Rocky.Models.ViewModels;
using System.Diagnostics;
using Rocky.Utility;

namespace Rocky.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public IActionResult Index()
        {
            HomeVM homeVM = new()
            {
                Products = _db.Product.Include(x => x.Category).Include(x => x.ApplicationType),
                Categories = _db.Category
            };
            return View(homeVM);
        }

        //GET-DETAILS
        public IActionResult Details(int id) 
        {
            List<ShoppingCart> shoppingCartList = new();
            if (HttpContext.Session.Get<List<ShoppingCart>>(WebConstant.SessionCart) != null
                && HttpContext.Session.Get<List<ShoppingCart>>(WebConstant.SessionCart).Count > 0)
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WebConstant.SessionCart);
            }

            DetailsVM detailVM = new()
            {
                Product = _db.Product.Include(y => y.Category).Include(y => y.ApplicationType).Where(x => x.Id == id).FirstOrDefault(),
                ExistsInCard = false
            };

            foreach(var item in shoppingCartList)
            {
                if (item.ProductId == id)
                {
                    detailVM.ExistsInCard = true;
                }
            }
            return View(detailVM);
        }

        [HttpPost, ActionName(nameof(Details))]
        public IActionResult DetailsPost(int id)
        {
            List<ShoppingCart> shoppingCartList = new();
            if(HttpContext.Session.Get<List<ShoppingCart>>(WebConstant.SessionCart) != null 
                && HttpContext.Session.Get<List<ShoppingCart>>(WebConstant.SessionCart).Count > 0)
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WebConstant.SessionCart);
            }
            shoppingCartList.Add(new ShoppingCart { ProductId = id });
            HttpContext.Session.Set(WebConstant.SessionCart, shoppingCartList);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult RemoveFromCart(int id)
        {
            List<ShoppingCart> shoppingCartList = new();
            if (HttpContext.Session.Get<List<ShoppingCart>>(WebConstant.SessionCart) != null
                && HttpContext.Session.Get<List<ShoppingCart>>(WebConstant.SessionCart).Count > 0)
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WebConstant.SessionCart);
            }

            var itemToRemove = shoppingCartList.SingleOrDefault(x => x.ProductId == id);

            if(itemToRemove != null) 
            {
                shoppingCartList.Remove(itemToRemove);
            }

            HttpContext.Session.Set(WebConstant.SessionCart, shoppingCartList);
            return RedirectToAction(nameof(Index));
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