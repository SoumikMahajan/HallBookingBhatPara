using HallBookingBhatPara.Application.Interface;
using HallBookingBhatPara.Domain.DTO;
using HallBookingBhatPara.Domain.DTO.User;
using HallBookingBhatPara.Domain.Utility;
using HallBookingBhatPara.Infrastructure.Service;
using HallBookingBhatPara.Model.Validator;
using Microsoft.AspNetCore.Mvc;

namespace HallBookingBhatPara.Controllers
{
    public class UserController : Controller
    {
        private readonly IJwtService _jwtService;
        private readonly ITokenProvider _tokenProvider;
        private readonly IUnitOfWork _unitOfWork;

        public UserController(IJwtService jwtService, ITokenProvider tokenProvider, IUnitOfWork unitOfWork)
        {
            _jwtService = jwtService;
            _tokenProvider = tokenProvider;
            _unitOfWork = unitOfWork;
        }
        public IActionResult Login()
        {
            // If already authenticated and token is valid, redirect to Index
            //if (_tokenProvider.IsTokenValid())
            //{
            //    return RedirectToAction("Index", "Home");
            //}

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginRequestDTO model)
        {
            var validator = new LoginValidator();
            var validationResult = validator.Validate(model);

            if (!validationResult.IsValid)
            {
                return Json(ResponseService.FluentValidationErrorResponse<object>(validationResult.Errors));
            }

            var svvs = PasswordHasher.ComputeSha256Hash(model.Password);

            var response = await _unitOfWork.SPRepository.LoginAsync(model);
            if (response == null)
            {
                return Json(ResponseService.NotFoundResponse<string>("Invalid email or password."));
            }

            var userClaims = new UserClaims
            {
                Id = response.stake_holder_login_id_pk.ToString(),
                Email = response.login_id,
                Name = response.stake_holder_details,
                Roles = response.stake_details,
                RolesId = response.stake_details_id_fk.ToString(),
                StackHolderId = response.stake_details_id_fk.ToString()
            };

            var token = _jwtService.GenerateToken(userClaims);
            _tokenProvider.SetToken(token);

            TempData["GlobalSuccessMessage"] = "Login successful! Redirecting...";

            response.RedirectUrl = Url.Action("Index", "Home");

            return Json(ResponseService.SuccessResponse<object>(response));

        }

        public IActionResult Logout()
        {
            _tokenProvider.ClearToken();
            return RedirectToAction("Login", "User");
        }

        public IActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registration([FromForm] UserRegistrationDto model)
        {
            var validator = new RegistrationValidator();
            var validationResult = validator.Validate(model);

            if (!validationResult.IsValid)
            {
                return Json(ResponseService.FluentValidationErrorResponse<object>(validationResult.Errors));
            }

            model.EntryIP = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown IP";

            model.CreatedBy = 1;

            if (model.ProfilePic != null && model.ProfilePic.Length > 0)
            {
                model.ImageData = await FileHelper.ConvertToByteArrayAsync(model.ProfilePic);
            }

            var EncriptedPassword = PasswordHasher.ComputeSha256Hash(model.Password);
            model.Password = EncriptedPassword;

            var userId = await _unitOfWork.SPRepository.RegistrationAsync(model);
            if (userId <= 0)
            {
                return Json(ResponseService.InternalServerResponse<object>("Registration failed. Please try again."));
            }

            return Json(ResponseService.SuccessResponse<object>("Registration successful!"));

        }

    }
}
