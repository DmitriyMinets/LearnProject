using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rocky.Data;
using Rocky_Models;
using Rocky_Models.ViewModels;
using System.Diagnostics;
using Rocky.Utility;
using Rocky_DataAccess.Repository.IRepository;

namespace Rocky.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        public HomeController(ILogger<HomeController> logger, IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            _logger = logger;
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        public IActionResult Index()
        {
            HomeVM homeVM = new()
            {
                Products = _productRepository.GetAll(includeProperties: "Category,ApplicationType"),
                Categories = _categoryRepository.GetAll()
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
                Product = _productRepository.FirstOrDefault(x => x.Id == id, includeProperties: "Category,ApplicationType"),
                ExistsInCard = false
            };

            foreach (var item in shoppingCartList)
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
            if (HttpContext.Session.Get<List<ShoppingCart>>(WebConstant.SessionCart) != null
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

            if (itemToRemove != null)
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