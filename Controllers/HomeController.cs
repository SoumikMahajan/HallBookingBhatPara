using HallBookingBhatPara.Application.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HallBookingBhatPara.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IJwtService _jwtService;
        private readonly ITokenProvider _tokenProvider;

        public HomeController(ILogger<HomeController> logger, IJwtService jwtService, ITokenProvider tokenProvider)
        {
            _logger = logger;
            _jwtService = jwtService;
            _tokenProvider = tokenProvider;
        }

        [Authorize]
        public IActionResult Index()
        {
            var userClaims = _tokenProvider.GetUserClaims();
            //ViewBag.UserName = userClaims?.Name ?? "Unknown User";
            //ViewBag.UserEmail = userClaims?.Email ?? "";

            return View();
        }

        [AllowAnonymous]
        public IActionResult CheckTokenAndRedirect()
        {
            if (_tokenProvider.IsTokenValid())
            {
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Login", "User");
        }

        public IActionResult BookingList()
        {
            return View();
        }

        //public IActionResult AddHallMaster()
        //{
        //    return View();
        //}

        [HttpGet]
        public IActionResult Error(string? message = null)
        {
            ViewBag.ErrorMessage = message ?? "An unexpected error occurred.";
            return View();
        }
    }
}
