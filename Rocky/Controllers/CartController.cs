using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Rocky.Data;
using Rocky_Models;
using Rocky_Models.ViewModels;
using Rocky.Utility;
using System.Security.Claims;
using System.Text;
using Rocky_DataAccess.Repository.IRepository;
using Rocky_DataAccess.Repository;

namespace Rocky.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly IApplicationUserRepository _applicationUserRepository;
        private readonly IInquiryHeaderRepository _inqiryHeaderRepository;
        private readonly IInquiryDetailsRepository _inqiryDetailsRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IEmailSender _emailSender;

        [BindProperty]
        public ProductUserVM ProductUserVM { get; set; }

        public CartController(IProductRepository productRepository,
                              IApplicationUserRepository applicationUserRepository,
                              IInquiryHeaderRepository inqiryHeaderRepository,
                              IInquiryDetailsRepository inqiryDetailsRepository,
                              IWebHostEnvironment webHostEnvironment, 
                              IEmailSender emailSender)
        {
            _productRepository = productRepository;
            _applicationUserRepository = applicationUserRepository;
            _inqiryHeaderRepository = inqiryHeaderRepository;
            _inqiryDetailsRepository = inqiryDetailsRepository;
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
            IEnumerable<Product> productList = _productRepository.GetAll(x => productInCart.Contains(x.Id));

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
            IEnumerable<Product> productList = _productRepository.GetAll(x => productInCart.Contains(x.Id));

            ProductUserVM productUserVM = new()
            {
                ApplicationUser = _applicationUserRepository.FirstOrDefault(x => x.Id == userId),
                ProductList = productList.ToList(),
            };

            return View(productUserVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName(nameof(Summary))]
        public async  Task<IActionResult> SummaryPost(ProductUserVM productUserVM)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);

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

            InquiryHeader  inquiryHeader = new()
            {
                ApplicationUserId = claim.Value,
                FullName = productUserVM.ApplicationUser.FullName,
                PhoneNumber = productUserVM.ApplicationUser.PhoneNumber,
                Email = productUserVM.ApplicationUser.Email,
                InquiryDate = DateTime.Now,
            };

            _inqiryHeaderRepository.Add(inquiryHeader);
            _inqiryHeaderRepository.Save();

            foreach (var product in productUserVM.ProductList)
            {
                InquiryDetails inqiryDetails = new()
                {
                    InquiryHeaderId = inquiryHeader.Id,
                    ProductId = product.Id,
                };
                _inqiryDetailsRepository.Add(inqiryDetails);
            };
            _inqiryDetailsRepository.Save();

            return RedirectToAction(nameof(InquiryConfiguration));
        }

        public IActionResult InquiryConfiguration()
        {
            HttpContext.Session.Clear();
            return View();
        }


    }
}
