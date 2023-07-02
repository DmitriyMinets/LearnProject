using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rocky_Models;
using Rocky_Models.ViewModels;
using Rocky_DataAccess.Repository.IRepository;

namespace Rocky.Controllers
{
    [Authorize(Roles = WebConstant.AdminRole)]
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepos;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IProductRepository productRepos, IWebHostEnvironment webHostEnvironment)
        {
            _productRepos = productRepos;
            _webHostEnvironment = webHostEnvironment;
        }

        //GET-INDEX
        public IActionResult Index()
        {
            //Жадная загрузка
            IEnumerable<Product> objList = _productRepos.GetAll(includeProperties: "ApplicationType,Category");
            //foreach (var obj in objList)
            //{
            //    obj.Category = _db.Category.FirstOrDefault(x => x.Id == obj.CategoryId);
            //    obj.ApplicationType = _db.ApplicationType.FirstOrDefault(x => x.Id == obj.ApplicationTypeId);
            //}
            return View(objList);
        }

        //GET-UPSERT
        public IActionResult Upsert(int? id)
        {
            ProductVM ProductVM = new()
            {
                Product = new Product(),
                CategorySelectList = _productRepos.GetAllDropDownList(WebConstant.CategoryName),
                ApplicationTypeSelectList = _productRepos.GetAllDropDownList(WebConstant.ApplicaionTypeName),
            };

            if (id == null)
            {
                //this is for create
                return View(ProductVM);
            }
            else
            {
                ProductVM.Product = _productRepos.Find(id.GetValueOrDefault());
                if (ProductVM.Product == null)
                {
                    return NotFound();
                }
                return View(ProductVM);
            }
        }

        //POST-UPSERT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM productVM)
        {
            //Разобратсья с валидацие на стороне сервера
            var files = HttpContext.Request.Form.Files;
            string webRootPath = _webHostEnvironment.WebRootPath;

            if (productVM.Product.Id == 0)
            {
                //create
                string upload = webRootPath + WebConstant.ImagePath;
                string fileName = Guid.NewGuid().ToString();
                string extension = Path.GetExtension(files[0].FileName);

                using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension),
                    FileMode.Create))
                {
                    files[0].CopyTo(fileStream);
                }

                productVM.Product.Image = fileName + extension;
                _productRepos.Add(productVM.Product);
            }
            else
            {
                //update
                var objFromDb = _productRepos.FirstOrDefault(u => u.Id == productVM.Product.Id, isTracking: false);
                if (objFromDb == null)
                {
                    return NotFound();
                }
                else
                {
                    if (files.Count > 0)
                    {
                        string upload = webRootPath + WebConstant.ImagePath;
                        string fileName = Guid.NewGuid().ToString();
                        string extension = Path.GetExtension(files[0].FileName);

                        var oldFile = Path.Combine(upload, objFromDb.Image);

                        if (System.IO.File.Exists(oldFile))
                        {
                            System.IO.File.Delete(oldFile);
                        }

                        using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                        {
                            files[0].CopyTo(fileStream);
                        }
                        productVM.Product.Image = fileName + extension;
                    }
                    else
                    {
                        productVM.Product.Image = objFromDb.Image;
                    }
                    _productRepos.Update(productVM.Product);
                }
            }
            _productRepos.Save();
            return RedirectToAction("Index");
        }


        //GET-DELETE
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Product product = _productRepos.FirstOrDefault(x => x.Id == id, includeProperties: "Category, ApplicationType");
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        //POST-DELETE
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var obj = _productRepos.Find(id.GetValueOrDefault());
            if (obj == null)
            {
                return NotFound();
            }

            string upload = _webHostEnvironment.WebRootPath + WebConstant.ImagePath;
            var oldFile = Path.Combine(upload, obj.Image);

            if (System.IO.File.Exists(oldFile))
            {
                System.IO.File.Delete(oldFile);
            }

            _productRepos.Remove(obj);
            _productRepos.Save();
            return RedirectToAction("Index");
        }
    }
}
