using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Planificateur.Core;
using Planificateur.Core.Contracts;
using Planificateur.Core.Entities;

namespace Planificateur.Web.Controllers;

[Route("api/polls")]
public class PollsApiController : ControllerBase
{
    private readonly PollApplication pollApplication;

    public PollsApiController(PollApplication pollApplication)
    {
        this.pollApplication = pollApplication;
    }

    [HttpPost]
    public async Task<ActionResult<Poll>> CreatePoll([FromBody] CreatePollRequest createPollRequest)
    {
        Poll poll = await pollApplication.CreatePoll(createPollRequest);
        return Created($"/api/polls/{poll.Id}", poll);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Poll>> GetPoll(Guid id)
    {
        return Ok(await pollApplication.GetPoll(id));
    }
}