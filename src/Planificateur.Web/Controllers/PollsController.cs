using Microsoft.AspNetCore.Mvc;
using Planificateur.Core;
using Planificateur.Core.Entities;

namespace Planificateur.Web.Controllers;

[Route("polls")]
public class PollsController : Controller
{
    private readonly PollApplication pollApplication;

    public PollsController(PollApplication pollApplication)
    {
        this.pollApplication = pollApplication;
    }

    [HttpGet("create")]
    public IActionResult CreatePollForm([FromQuery] int? datesCount)
    {
        return View("Views/Polls/Create.cshtml", datesCount);
    }
    
    [HttpPost("create")]
    public async Task<IActionResult> CreatePollFormSubmit(Poll poll)
    {
        await pollApplication.CreatePoll(poll);
        return Redirect($"/polls/{poll.Id}");
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ViewPoll(Guid id)
    {
        Poll? poll = await pollApplication.GetPoll(id);
        if (poll is null)
        {
            return NotFound();
        }
        return View("Views/Polls/Poll.cshtml", poll);
    }
}