using HallBookingBhatPara.Application.Interface;
using HallBookingBhatPara.Domain.Entities;
using HallBookingBhatPara.Infrastructure.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HallBookingBhatPara.Controllers
{
    public class AdminController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public AdminController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [Authorize]
        public IActionResult CategoryList()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddCategory(string categoryName)
        {
            /// Validation will be done by Soumik
            var response = await _unitOfWork.SPRepository.AddHallCategoryAsync(categoryName);

            if (response == 0 || response == null)
            {
                return Json(ResponseService.InternalServerResponse<string>("Insert Failed."));
            }

            return Json(ResponseService.SuccessResponse<string>("Category Insert Successfully"));
            
        }

        [Authorize]
        public IActionResult SubCategoryList()
        {
            return View();
        }
    }
}
