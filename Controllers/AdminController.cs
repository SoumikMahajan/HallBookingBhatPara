using HallBookingBhatPara.Application.Interface;
using HallBookingBhatPara.Domain.DTO;
using HallBookingBhatPara.Domain.DTO.Admin;
using HallBookingBhatPara.Domain.Utility;
using HallBookingBhatPara.Infrastructure.Service;
using HallBookingBhatPara.Model.Validator;
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

        #region :: Category
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

            if (response == 0)
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCategory(long categoryId, string categoryName)
        {
            if (categoryId <= 0)
                return Json(ResponseService.BadRequestResponse<string>("CategoryId can not null or empty or 0"));
            if (string.IsNullOrEmpty(categoryName))
                return Json(ResponseService.BadRequestResponse<string>("Category Name can not null or empty"));

            var updated = await _unitOfWork.CategoryMasterRepository.UpdateAsync(categoryId, categoryName);

            if (updated)
                return Json(ResponseService.SuccessResponse<string>("Category updated successfully."));
            else
                return Json(ResponseService.InternalServerResponse<string>("Update failed. Record may not exist."));

        }
        #endregion

        #region :: SubCategory
        [Authorize]
        public async Task<IActionResult> SubCategoryList()
        {
            MultipleModel mm = new();
            var dropDownList = (await _unitOfWork.CategoryMasterRepository.GetAllAsync(c => c.active_status == 1))
                             .Select(c => new DropDownListDTO { Id = c.category_id_pk, Name = c.category_name })
                             .ToList();

            mm.dropDownListDTOs = dropDownList;
            return View(mm);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddSubCategory([FromForm] InsertSubCategoryDTO model)
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

        [Authorize]
        public async Task<IActionResult> GetAllSubCategoryList()
        {
            var SubcategoryList = await _unitOfWork.SPRepository.GetALlSubcategorisAsync();
            if (SubcategoryList == null || !SubcategoryList.Any())
            {
                return Json(ResponseService.NotFoundResponse<string>("No SubCategory Found."));
            }

            // Convert images using global FileHelper
            foreach (var item in SubcategoryList)
            {
                item.hall_image_base64 = FileHelper.ConvertToBase64Image(item.hall_image);
            }


            return Json(ResponseService.SuccessResponse(SubcategoryList));
        }

        [Authorize]
        public async Task<IActionResult> GetSubCategoryById(long SubCategoryId)
        {
            var dropDownList = (await _unitOfWork.CategoryMasterRepository.GetAllAsync(c => c.active_status == 1))
                             .Select(c => new DropDownListDTO { Id = c.category_id_pk, Name = c.category_name })
                             .ToList();

            var SubCategory = await _unitOfWork.SubCategoryMasterRepository.GetAsync(h => h.hall_id_pk == SubCategoryId);
            MultipleModel mm = new();

            mm.dropDownListDTOs = dropDownList;
            mm.hall_Master = SubCategory;

            return PartialView("_partialEditSubCategory", mm);

        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateSubCategory([FromForm] UpdateSubCategoryDTO model)
        {
            if (model.Subcategoryid <= 0)
                return Json(ResponseService.BadRequestResponse<string>("SubCategoryId can not null or empty or 0"));

            if (model.CategoryId == 0)
                return Json(ResponseService.BadRequestResponse<string>("CategoryId can not null or empty or 0"));

            if (string.IsNullOrEmpty(model.SubCategoryName))
                return Json(ResponseService.BadRequestResponse<string>("SubCategory Name can not null or empty"));

            if (model.HasNewImage)
            {
                if (model.fileUpload == null || model.fileUpload.Length == 0)
                    return Json(ResponseService.BadRequestResponse<string>("Image file is required."));

                model.ImageData = await FileHelper.ConvertToByteArrayAsync(model.fileUpload);
            }

            bool updated = await _unitOfWork.SubCategoryMasterRepository.UpdateAsync(model);

            if (updated)
                return Json(ResponseService.SuccessResponse<string>("SubCategory updated successfully."));
            else
                return Json(ResponseService.InternalServerResponse<string>("Update failed. Record may not exist."));

        }

        #endregion

        #region :: Hall Availability
        [Authorize]
        public async Task<IActionResult> AddHallAvailabilityDetails()
        {
            MultipleModel mm = new();
            var dropDownList = (await _unitOfWork.CategoryMasterRepository.GetAllAsync(c => c.active_status == 1))
                             .Select(c => new DropDownListDTO { Id = c.category_id_pk, Name = c.category_name })
                             .ToList();

            mm.dropDownListDTOs = dropDownList;
            return View(mm);
        }

        [Authorize]
        public async Task<IActionResult> GetSubCategoriesByCatId(long categoryId)
        {
            if (categoryId <= 0)
            {
                return Json(ResponseService.BadRequestResponse<string>("CategoryId can not null or empty or 0"));
            }
            var SubcategoryList = (await _unitOfWork.SubCategoryMasterRepository.GetAllAsync(c => c.active_status == 1 && c.category_id_fk == categoryId))
                        .Select(c => new DropDownListDTO { Id = c.hall_id_pk, Name = c.hall_name }).ToList();
            if (SubcategoryList == null || !SubcategoryList.Any())
            {
                return Json(ResponseService.NotFoundResponse<string>("No SubCategory Found."));
            }

            return Json(ResponseService.SuccessResponse(SubcategoryList));
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddHallAvailable([FromForm] InsertHallAvailableDTO model)
        {
            var validator = new HallAvailableValidator();
            var validationResult = validator.Validate(model);

            if (!validationResult.IsValid)
            {
                return Json(ResponseService.FluentValidationErrorResponse<object>(validationResult.Errors));
            }

            model.userClaims = _tokenProvider.GetUserClaims();



            var response = await _unitOfWork.SPRepository.AddHallAvailableAsync(model);

            if (response == 0)
            {
                return Json(ResponseService.InternalServerResponse<string>("Add Failed."));
            }

            return Json(ResponseService.SuccessResponse<string>("Insert Successfully"));

        }

        [Authorize]
        public async Task<IActionResult> GetAllHallAvailabilityList()
        {
            var HallAvailabilityList = await _unitOfWork.SPRepository.GetAllHallAvailableAsync();
            if (HallAvailabilityList == null || !HallAvailabilityList.Any())
            {
                return Json(ResponseService.NotFoundResponse<string>("No Hall Found."));
            }

            return Json(ResponseService.SuccessResponse(HallAvailabilityList));
        }

        #endregion


    }
}
