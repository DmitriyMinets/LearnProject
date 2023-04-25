using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Rocky.Data;
using Rocky.Models;
using Rocky.Models.ViewModels;
using Rocky.Utility;
using System.Security.Claims;
using System.Text;

namespace Rocky.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IEmailSender _emailSender;

        [BindProperty]
        public ProductUserVM ProductUserVM { get; set; }

        public CartController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment, IEmailSender emailSender)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
            _emailSender = emailSender;
        }

        public IActionResult Index()
        {
            List<ShoppingCart> shopingCartList = new();

            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstant.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstant.SessionCart).Any())
            {
                shopingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WebConstant.SessionCart);
            }

            List<int> productInCart = shopingCartList.Select(x => x.ProductId).ToList();
            IEnumerable<Product> productList = _db.Product.Where(x => productInCart.Contains(x.Id));

            return View(productList);
        }

        public IActionResult Remove(int id)
        {
            List<ShoppingCart> shopingCartList = new();

            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstant.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstant.SessionCart).Any())
            {
                shopingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WebConstant.SessionCart);
            }

            shopingCartList.Remove(shopingCartList.FirstOrDefault(x => x.ProductId == id));
            HttpContext.Session.Set(WebConstant.SessionCart, shopingCartList);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName(nameof(Index))]
        public IActionResult IndexPost()
        {

            return RedirectToAction(nameof(Summary));
        }

        public IActionResult Summary()
        {
            //var claimsIdentity = (ClaimsIdentity)User.Identity;
            //var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var userId = User.FindFirstValue(ClaimTypes.Name);

            List<ShoppingCart> shopingCartList = new();

            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstant.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstant.SessionCart).Any())
            {
                shopingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WebConstant.SessionCart);
            }

            List<int> productInCart = shopingCartList.Select(x => x.ProductId).ToList();
            IEnumerable<Product> productList = _db.Product.Where(x => productInCart.Contains(x.Id));

            ProductUserVM productUserVM = new()
            {
                ApplicationUser = _db.ApplicationUser.FirstOrDefault(x => x.Id == userId),
                ProductList = productList.ToList(),
            };

            return View(productUserVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName(nameof(Summary))]
        public async  Task<IActionResult> SummaryPost(ProductUserVM productUserVM)
        {
            var pathToTampate = _webHostEnvironment.WebRootPath 
                                + Path.DirectorySeparatorChar.ToString() + "template"
                                + Path.DirectorySeparatorChar.ToString() + "Inquiry.html";

            string subject = "New Inquiry";
            string HtmlBody = string.Empty;

            using(StreamReader sr = System.IO.File.OpenText(pathToTampate)) 
            {
                HtmlBody = sr.ReadToEnd();
            }

            StringBuilder productListSB = new StringBuilder();
            foreach(var product in productUserVM.ProductList) 
            {
                productListSB.Append($"Name: {product.Name} <span style='font-size:14px;' > (ID: {product.Id}) </span><br />");
            }

            string messageBody = string.Format(HtmlBody,
                productUserVM.ApplicationUser.FullName,
                productUserVM.ApplicationUser.Email, 
                productUserVM.ApplicationUser.PhoneNumber,
                productListSB.ToString());

            await _emailSender.SendEmailAsync(WebConstant.EmailAdmin, subject, messageBody);

            return RedirectToAction(nameof(InquiryConfiguration));
        }

        public IActionResult InquiryConfiguration()
        {
            HttpContext.Session.Clear();
            return View();
        }


    }
}
