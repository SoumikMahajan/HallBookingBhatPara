using HallBookingBhatPara.Application.Interface;
using HallBookingBhatPara.Domain.DTO;
using Microsoft.AspNetCore.Mvc;

namespace HallBookingBhatPara.Controllers
{
    public class UserBookingController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenProvider _tokenProvider;

        public UserBookingController(IUnitOfWork unitOfWork, ITokenProvider tokenProvider)
        {
            _unitOfWork = unitOfWork;
            _tokenProvider = tokenProvider;
        }
        public IActionResult UserHallBooking()
        {
            return View();
        }

        public async Task<IActionResult> HallAvailableSearchResult(long hallType, string startDate, string endDate)
        {
            MultipleModel mm = new();

            return PartialView("_partialHallAvailableSearchResult", mm);
        }
    }
}
