using HallBookingBhatPara.Application.Interface;
using HallBookingBhatPara.Domain.DTO;
using HallBookingBhatPara.Domain.DTO.Admin;
using HallBookingBhatPara.Domain.DTO.HallBooking;
using HallBookingBhatPara.Domain.Utility;
using HallBookingBhatPara.Infrastructure.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HallBookingBhatPara.Controllers
{
    [Authorize(Roles = "Public User,Dev")]
    public class UserBookingController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenProvider _tokenProvider;

        public UserBookingController(IUnitOfWork unitOfWork, ITokenProvider tokenProvider)
        {
            _unitOfWork = unitOfWork;
            _tokenProvider = tokenProvider;
        }

        public async Task<IActionResult> UserHallBooking()
        {
            MultipleModel mm = new();
            var dropDownList = (await _unitOfWork.CategoryMasterRepository.GetAllAsync(c => c.active_status == 1))
                             .Select(c => new DropDownListDTO { Id = c.category_id_pk, Name = c.category_name })
                             .ToList();

            mm.dropDownListDTOs = dropDownList;
            return View(mm);
        }

        public async Task<IActionResult> HallAvailableSearchResult(long catType, string startDate, string endDate)
        {
            MultipleModel mm = new();
            List<HallSearchDTO> hallSearchList = new();

            var response = await _unitOfWork.SPRepository.HallAvailableSearchResultAsync(catType, startDate, endDate);

            mm.hallSearchList = response ?? new List<HallSearchDTO>();

            return PartialView("_partialHallAvailableSearchResult", mm);
        }

        public async Task<IActionResult> HallDetailsBooking(long hallAvlId)
        {
            MultipleModel mm = new();
            var hallDetails = await _unitOfWork.SPRepository.GetHallDetailsAfterSearchAsync(hallAvlId);

            mm.hallBookingDTO = hallDetails;

            var drpDownHallEvent = (await _unitOfWork.HallEventMasterRepository.GetAllAsync(h => h.active_status == 1))
                .Select(h => new DropDownListDTO { Id = h.hall_event_type_id_pk, Name = h.event_type_name })
                .ToList();

            //var FloorList = await _unitOfWork.SPRepository.GetFloorListBySubCatIdAsync(hallDetails.hall_id_pk);

            mm.dropDownListDTOs = drpDownHallEvent;
            //mm.FloorList = FloorList;

            return View(mm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BookUserConfirmedHall([FromForm] InsertSubCategoryDTO model)
        {
            if (model.CategoryId == 0)
                return Json(ResponseService.BadRequestResponse<string>("CategoryId can not null or empty or 0"));

            if (string.IsNullOrEmpty(model.SubCategoryName))
                return Json(ResponseService.BadRequestResponse<string>("SubCategory Name can not null or empty"));
            if (model.fileUpload == null || model.fileUpload.Length == 0)
                return Json(ResponseService.BadRequestResponse<string>("Profile Picture can not null or empty"));

            model.CreatedBy = Convert.ToInt64(_tokenProvider.GetUserClaims().Id);
            if (model.fileUpload != null && model.fileUpload.Length > 0)
            {
                model.ImageData = await FileHelper.ConvertToByteArrayAsync(model.fileUpload);
            }

            var response = await _unitOfWork.SPRepository.AddHallSubCategoryAsync(model);

            if (response == 0)
            {
                return Json(ResponseService.InternalServerResponse<string>("SubCategory Insert Failed."));
            }

            return Json(ResponseService.SuccessResponse<string>("SubCategory Insert Successfully"));

        }
    }
}
