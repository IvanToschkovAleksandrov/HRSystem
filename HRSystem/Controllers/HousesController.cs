using HRSystem.Data.Models;
using HRSystem.Infrastructure;
using HRSystem.Models.Agents;
using HRSystem.Models.Houses;
using HRSystem.Services.Agents;
using HRSystem.Services.Houses;
using HRSystem.Services.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace HRSystem.Controllers
{
    public class HousesController : Controller
    {
        private readonly IHouseService houseService;
        private readonly IAgentService agentService;

        public HousesController(
            IHouseService houseService,
            IAgentService agentService)
        {
            this.houseService = houseService;
            this.agentService = agentService;
        }

        //public IActionResult All()
        //{
        //    var model = new AllHousesQueryModel();

        //    return View(model);
        //}

        [HttpGet]
        public async Task<IActionResult> All([FromQuery] AllHousesQueryModel query)
        {
            var model = await houseService.AllAsync(
                query.Category,
                query.SearchTerm,
                query.Sorting,
                query.CurrentPage,
                AllHousesQueryModel.HousesPerPage);

            query.TotalHousesCount = model.TotalHousesCount;
            query.Houses = model.Houses;

            var houseCateogories = await houseService.AllCategoriesNamesAsync();
            query.Categories = houseCateogories;

            return View(query);
        }

        [Authorize]
        public async Task<IActionResult> Mine()
        {
            IEnumerable<HouseServiceModel> myHouses = null;
            var userId = User.Id();

            if (await agentService.ExistByIdAsync(userId))
            {
                var agentId = await agentService.GetAgentIdAsync(userId);
                myHouses = await houseService.AllHousesByAgentId(agentId);
            }
            else
            {
                myHouses = await houseService.AllHousesByUserIdAsync(userId);
            }

            return View(myHouses);
        }

        public IActionResult Details(int id)
        {
            var model = new HouseDetailsViewModel();

            return View(model);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            if(!await agentService.ExistByIdAsync(User.Id()))
            {
                return RedirectToAction("Become", "Agents");
            }

            var model = new HouseFormModel()
            {
                Cagetories = await houseService.AllCategoriesAsync()
            };

            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Add(HouseFormModel model)
        {
            if (!await agentService.ExistByIdAsync(User.Id()))
            {
                return RedirectToAction("Become", "Agents");
            }
            
            if (!await houseService.CategoryExistAsync(model.CategoryId))
            {
                ModelState.AddModelError(nameof(model.CategoryId), "Category does not exist.");
            }

            if (!ModelState.IsValid)
            {
                model.Cagetories = await houseService.AllCategoriesAsync();

                return View(model);
            }

            var agentId = await agentService.GetAgentIdAsync(this.User.Id());
            int newHouseId = await houseService
                .CreateAsync(
                model.Title, 
                model.Address, 
                model.Description, 
                model.ImageUrl,
                model.PricePerMonth, 
                model.CategoryId,
                agentId);
            
            return RedirectToAction(nameof(Details), new { id = newHouseId });
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
