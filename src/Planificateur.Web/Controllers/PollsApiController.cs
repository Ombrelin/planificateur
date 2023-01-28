using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Planificateur.Core;
using Planificateur.Core.Contracts;
using Planificateur.Core.Entities;

namespace Planificateur.Web.Controllers;

[Route("api/polls")]
[Tags("Polls")]
public class PollsApiController : ControllerBase
{
    private readonly PollApplication pollApplication;

    public PollsApiController(PollApplication pollApplication)
    {
        this.pollApplication = pollApplication;
    }

    /// <summary>
    /// Create a new Poll.
    /// </summary>
    /// <param name="createPollRequest"></param>
    /// <returns>The created Poll.</returns>
    [HttpPost]
    public async Task<ActionResult<Poll>> CreatePoll([FromBody] CreatePollRequest createPollRequest)
    {
        Poll poll = await pollApplication.CreatePoll(createPollRequest);
        return Created($"/api/polls/{poll.Id}", poll);
    }

    /// <summary>
    /// Get a Poll from its Id.
    /// </summary>
    /// <param name="id">The Poll Id.</param>
    /// <returns>The corresponding Poll.</returns>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Poll>> GetPoll(Guid id)
    {
        return Ok(await pollApplication.GetPoll(id));
    }
    
    [HttpDelete("{id:guid}/votes/{voteId:guid}")]
    public async Task<ActionResult<Poll>> GetPoll(Guid pollId, Guid voteId)
    {
        await pollApplication.RemoveVote(voteId);
        return Ok();
    }
}