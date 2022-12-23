using HRSystem.Models.Agents;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRSystem.Controllers
{
    [Authorize]
    public class AgentsController : Controller
    {
        public IActionResult Become()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Become(BecomeAgentFormModel agent)
        {
            //Check ModelState
            //Add new agent info in the database.

            return RedirectToAction("All", "Houses");
        }
    }
}
