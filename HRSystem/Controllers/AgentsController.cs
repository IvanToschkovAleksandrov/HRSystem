using HRSystem.Infrastructure;
using HRSystem.Models.Agents;
using HRSystem.Services.Agents;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRSystem.Controllers
{
    [Authorize]
    public class AgentsController : Controller
    {
        private readonly IAgentService agentService;

        public AgentsController(IAgentService agentService)
        {
            this.agentService = agentService;
        }

        public async Task<IActionResult> Become()
        {
            if(await agentService.ExistByIdAsync(this.User.Id()))
            {
                return BadRequest();
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Become(BecomeAgentFormModel model)
        {
            var userId = this.User.Id();
            
            if(await this.agentService.ExistByIdAsync(userId))
            {
                return BadRequest();
            }

            if(await agentService.UserWithPhoneNumberExistAsync(model.PhoneNumber))
            {
                ModelState.AddModelError(nameof(model.PhoneNumber), "Phone number already exist.Enter another one.");
            }

            if(await agentService.UserHasRentsAsync(userId))
            {
                ModelState.AddModelError("Error", "You should have no rents to become an agent.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await agentService.CreateAsync(userId, model.PhoneNumber);
            
            return RedirectToAction("All", "Houses");
        }
    }
}
