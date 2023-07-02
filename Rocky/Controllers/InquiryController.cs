using Microsoft.AspNetCore.Mvc;
using Rocky_DataAccess.Repository.IRepository;
using Rocky_Models.ViewModels;

namespace Rocky.Controllers
{
    public class InquiryController : Controller
    {
        private readonly IInquiryHeaderRepository _inquiryHeaderRepository;
        private readonly IInquiryDetailsRepository _inquiryDetailsRepository;

        [BindProperty]
        public InquiryVM InquiryVM {  get; set; }

        public InquiryController(IInquiryHeaderRepository inqiryHeaderRepository,
                                  IInquiryDetailsRepository inqiryDetailsRepository)
        {
            _inquiryDetailsRepository = inqiryDetailsRepository;
            _inquiryHeaderRepository = inqiryHeaderRepository;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int id)
        {
            InquiryVM = new()
            {
                InquiryHeader = _inquiryHeaderRepository.FirstOrDefault(x => x.Id == id),
                InquiryDetails = _inquiryDetailsRepository.GetAll(x => x.InquiryHeaderId == id, includeProperties: "Product"),
            };
            return View(InquiryVM);
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetInquiryList()
        {
            return Json(new { data = _inquiryHeaderRepository.GetAll() });
        }
        #endregion
    }
}
