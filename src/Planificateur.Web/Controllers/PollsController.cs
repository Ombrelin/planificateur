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
    public IActionResult CreatePollForm()
    {
        return View("Views/Polls/Create.cshtml");
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreatePollFormSubmit(Poll poll)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Errors = ModelState
                .Values
                .SelectMany(model => model.Errors)
                .Select(error => error.Exception?.Message)
                .Where(error => error is not null);
            return View("Views/Polls/Create.cshtml");
        }
        
        await pollApplication.CreatePoll(poll);
        
        return Redirect($"/polls/{poll.Id}");
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ViewPoll(Guid id)
    {
        Poll? poll = await pollApplication.GetPoll(id);
        return View("Views/Polls/Poll.cshtml", poll);
    }

    [HttpPost("{id:guid}/votes")]
    public async Task<IActionResult> AddVote(Guid id, IFormCollection data)
    {
        var vote = new Vote
        {
            PollId = id, 
            VoterName = data["voterName"],
            Availabilities = data
                .Where(formData => formData.Key.Contains("availability"))
                .Select(value => Enum.Parse<Availability>(value.Value))
                .ToList()
        };
        await pollApplication.Vote(vote);
        return Redirect($"/polls/{id}");
    }
}