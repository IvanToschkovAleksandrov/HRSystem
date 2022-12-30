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

        public async Task<IActionResult> Details(int id)
        {
            if (!await houseService.ExistAsync(id))
            {
                return BadRequest();
            }

            var model = await houseService.HouseDetailsByIdAsync(id);

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
                Categories = await houseService.AllCategoriesAsync()
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
                model.Categories = await houseService.AllCategoriesAsync();

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
        public async Task<IActionResult> Edit(int id)
        {
            if (!await houseService.ExistAsync(id))
            {
                return BadRequest();
            }

            if (!await houseService.HasAgentWithIdAsync(id, User.Id()))
            {
                return Unauthorized();
            }

            var house = await houseService.HouseDetailsByIdAsync(id);
            var categoryId = await houseService.GetHouseCategoryIdAsync(house.Id);

            var houseModel = new HouseFormModel()
            {
                Title = house.Title,
                Address = house.Address,
                ImageUrl = house.ImageUrl,
                Description = house.Description,
                CategoryId = categoryId,
                PricePerMonth = house.PricePerMonth,
                Categories = await houseService.AllCategoriesAsync()
            };

            return View(houseModel);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Edit(HouseFormModel model, int id)
        {
            if (!await houseService.ExistAsync(id))
            {
                return BadRequest();
            }

            if (!await houseService.HasAgentWithIdAsync(id, User.Id()))
            {
                return Unauthorized();
            }

            if (!await houseService.CategoryExistAsync(model.CategoryId))
            {
                ModelState.AddModelError(nameof(model.CategoryId), "Category does not exist!");
            }

            if (!ModelState.IsValid)
            {
                model.Categories = await houseService.AllCategoriesAsync();

                return View(model);
            }

            await houseService.EditAsync(id, model.Title, model.Address, model.Description, model.ImageUrl, model.PricePerMonth, model.CategoryId);

            return RedirectToAction(nameof(Details), new { id = id });
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            if (!await houseService.ExistAsync(id))
            {
                return BadRequest();
            }

            if (!await houseService.HasAgentWithIdAsync(id, User.Id()))
            {
                return Unauthorized();
            }

            var house = await houseService.HouseDetailsByIdAsync(id);
            var model = new HouseDetailsViewModel()
            {
                Id = id,
                Address = house.Address,
                Title = house.Title,
                ImageUrl = house.ImageUrl
            };

            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Delete(HouseDetailsViewModel model, int id)
        {
            if (!await houseService.ExistAsync(id))
            {
                return BadRequest();
            }

            if (!await houseService.HasAgentWithIdAsync(id, User.Id()))
            {
                return Unauthorized();
            }

            await houseService.DeleteAsync(id);

            return RedirectToAction(nameof(All));
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Rent(int id)
        {
            if (!await houseService.ExistAsync(id))
            {
                return BadRequest();
            }

            if (await agentService.ExistByIdAsync(User.Id()))
            {
                return Unauthorized();
            }

            if (await houseService.IsRentedAsync(id))
            {
                return BadRequest();
            }

            await houseService.RentAsync(id, User.Id());

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
