﻿using Microsoft.AspNetCore.Mvc;

namespace HallBookingBhatPara.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
