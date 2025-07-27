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
            if (_tokenProvider.IsTokenValid())
            {
                return RedirectToAction("Index", "Home");
            }

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
                Name = "User Name",
                Roles = new List<string> { response.stake_holder_details }
            };

            var token = _jwtService.GenerateToken(userClaims);
            _tokenProvider.SetToken(token);

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

    }
}
