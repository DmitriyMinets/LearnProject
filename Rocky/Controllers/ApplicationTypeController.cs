using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rocky.Data;
using Rocky_Models;
using Rocky_DataAccess.Repository.IRepository;

namespace Rocky.Controllers
{
    [Authorize(Roles = WebConstant.AdminRole)]
    public class ApplicationTypeController : Controller
    {
        private readonly IApplicationTypeRepository _applicatonRepos;
        public ApplicationTypeController(IApplicationTypeRepository applicatonRepos)
        {
            _applicatonRepos = applicatonRepos;
        }

        public IActionResult Index()
        {
            IEnumerable<ApplicationType> obj = _applicatonRepos.GetAll();
            return View(obj);
        }

        //GET-CREATE
        public IActionResult Create()
        {
            return View();
        }

        //POST-CREATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ApplicationType obj)
        {
            if (ModelState.IsValid)
            {
                _applicatonRepos.Add(obj);
                _applicatonRepos.Save();
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
            var obj = _applicatonRepos.Find(id.GetValueOrDefault());
            if (obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }

        //POST-EDIT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ApplicationType obj)
        {
            if (ModelState.IsValid)
            {
                _applicatonRepos.Update(obj);
                _applicatonRepos.Save();
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
            var obj = _applicatonRepos.Find(id.GetValueOrDefault());
            if (obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }

        //POST-EDIT
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var obj = _applicatonRepos.Find(id.GetValueOrDefault());
            if (obj == null)
            {
                return NotFound();
            }
            _applicatonRepos.Remove(obj);
            _applicatonRepos.Save();
            return RedirectToAction("Index");
        }
    }
}
