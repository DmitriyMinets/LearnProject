using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rocky.Data;
using Rocky_Models;
using Rocky_DataAccess.Repository.IRepository;
using System.Data;

namespace Rocky.Controllers
{
    [Authorize(Roles = WebConstant.AdminRole)]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepos;
        public CategoryController(ICategoryRepository categoryRepos)
        {
            _categoryRepos = categoryRepos;
        }
        public IActionResult Index()
        {
            IEnumerable<Category> objList = _categoryRepos.GetAll();
            return View(objList);
        }

        //GET-CREATE
        public IActionResult Create()
        {
            return View();
        }

        //POST-CREATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category obj)
        {
            if (ModelState.IsValid)
            {
                _categoryRepos.Add(obj);
                _categoryRepos.Save();
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        //GET-EDIT
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var obj = _categoryRepos.Find(id.GetValueOrDefault());
            if (obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }

        //POST-EDIT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category obj)
        {
            if (ModelState.IsValid)
            {
                _categoryRepos.Update(obj);
                _categoryRepos.Save();
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        //GET-DELETE
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var obj = _categoryRepos.Find(id.GetValueOrDefault());
            if (obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }

        //POST-DELETE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var obj = _categoryRepos.Find(id.GetValueOrDefault());
            if (obj == null)
            {
                return NotFound();
            }
            _categoryRepos.Remove(obj);
            _categoryRepos.Save();
            return RedirectToAction("Index");
        }
    }
}
