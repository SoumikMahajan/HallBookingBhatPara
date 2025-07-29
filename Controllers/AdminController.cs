using HallBookingBhatPara.Application.Interface;
using HallBookingBhatPara.Infrastructure.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HallBookingBhatPara.Controllers
{
    public class AdminController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenProvider _tokenProvider;

        public AdminController(IUnitOfWork unitOfWork, ITokenProvider tokenProvider)
        {
            _unitOfWork = unitOfWork;
            _tokenProvider = tokenProvider;
        }

        [Authorize]
        public IActionResult CategoryList()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCategory(string categoryName)
        {
            if (string.IsNullOrEmpty(categoryName))
            {
                return Json(ResponseService.BadRequestResponse<string>("Category Name can not null or empty"));
            }
            var response = await _unitOfWork.SPRepository.AddHallCategoryAsync(categoryName);

            if (response == 0 || response == null)
            {
                return Json(ResponseService.InternalServerResponse<string>("Insert Failed."));
            }

            return Json(ResponseService.SuccessResponse<string>("Category Insert Successfully"));

        }
        [Authorize]
        public async Task<IActionResult> GetAllCategoryList()
        {
            var categoryList = await _unitOfWork.CategoryMasterRepository.GetAllAsync(c => c.active_status == 1);
            if (categoryList == null || !categoryList.Any())
            {
                return Json(ResponseService.NotFoundResponse<string>("No Category Found."));
            }
            return Json(ResponseService.SuccessResponse(categoryList));
        }

        [Authorize]
        public IActionResult SubCategoryList()
        {
            return View();
        }

        [Authorize]
        public IActionResult AddHallAvailabilityDetails()
        {
            return View();
        }
    }
}
