using HRSystem.Models;
using HRSystem.Models.Home;
using HRSystem.Services.Houses;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace HRSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHouseService houseService;

        public HomeController(IHouseService houseService)
        {
            this.houseService = houseService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var houses = await houseService.LastThreeHousesAsync();

            return View(houses);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int statusCode)
        {
            if (statusCode == 400)
            {
                return View("Error400");
            }

            if (statusCode == 401)
            {
                return View("Error401");
            }

            return View();
        }
    }
}