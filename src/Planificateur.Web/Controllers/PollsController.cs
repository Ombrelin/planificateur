using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Planificateur.Core;
using Planificateur.Core.Contracts;
using Planificateur.Core.Entities;

namespace Planificateur.Web.Controllers;

[AllowAnonymous]
[Route("polls")]
[ApiExplorerSettings(IgnoreApi = true)]
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
    public async Task<IActionResult> CreatePollFormSubmit(CreatePollRequest createPollRequest, IFormCollection data)
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

        StringValues timezone = data.First(kvp => kvp.Key is "timezone").Value;
        TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timezone);

        Poll poll = await pollApplication.CreatePoll(createPollRequest with
        {
            Dates = createPollRequest
                .Dates
                .Select(date => TimeZoneInfo.ConvertTimeToUtc(date, timeZoneInfo))
                .ToArray()
        });

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
        if (!data.ContainsKey("voterName"))
        {
            return BadRequest("Voter Name is required");
        }

        var availabilities = ParseAvailabilities(data);

        if (availabilities.Any(availability => availability is null))
        {
            return BadRequest("Invalid availability value");
        }

        var vote = new CreateVoteRequest(
            data["voterName"]!,
            availabilities.Select(availability => availability!.Value)
        );
        
        await pollApplication.Vote(id, vote);
        return Redirect($"/polls/{id}");
    }

    private static List<Availability?> ParseAvailabilities(IFormCollection data)
    {
        var availabilities = data
            .Where(formData => formData.Key.Contains("availability"))
            .Select(value =>
                Enum.TryParse(value.Value, out Availability availability) ? availability : (Availability?)null)
            .ToList();
        return availabilities;
    }
}