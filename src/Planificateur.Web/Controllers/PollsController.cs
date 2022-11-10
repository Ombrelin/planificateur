using Microsoft.AspNetCore.Mvc;
using Planificateur.Core.Entities;

namespace Planificateur.Web.Controllers;

[Route("polls")]
public class PollsController : Controller
{
    [HttpGet("create")]
    public IActionResult CreatePollForm([FromQuery] int? datesCount)
    {
        return View("Views/Polls/Create.cshtml", datesCount);
    }
    
    [HttpPost("create")]
    public IActionResult CreatePollFormSubmit(Poll poll)
    {
        return View("Views/Polls/Create.cshtml");
    }
}