using HRSystem.Models.Houses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRSystem.Controllers
{
    public class HousesController : Controller
    {
        public IActionResult All()
        {
            var model = new AllHousesQueryModel();

            return View(model);
        }

        [Authorize]
        public IActionResult Mine()
        {
            var model = new AllHousesQueryModel();

            return View(model);
        }

        public IActionResult Details(int id)
        {
            var model = new HouseDetailsViewModel();

            return View(model);
        }

        [Authorize]
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult Add(HouseFormModel model)
        {
            //Check ModelState
            //Add new House to the Database

            return RedirectToAction(nameof(Details), new { id = 1 });
        }

        [Authorize]
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var model = new HouseFormModel();

            return View(model);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Edit(HouseFormModel model, int id)
        {
            //Check ModelState
            //Update house in the database.

            return RedirectToAction(nameof(Details), new { id = 1 });
        }

        [Authorize]
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var model = new HouseDetailsViewModel();

            return View(model);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Delete(HouseDetailsViewModel model, int id)
        {
            //Delete House from the Database.

            return RedirectToAction(nameof(All));
        }

        [Authorize]
        [HttpPost]
        public IActionResult Rent(int id)
        {
            return RedirectToAction(nameof(Mine));
        }

        [Authorize]
        [HttpPost]
        public IActionResult Leave(int id)
        {
            return RedirectToAction(nameof(Mine));
        }
    }
}
