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
        Poll? poll = await pollApplication.GetPoll(id);
        if (poll is not null)
        {
            return Ok(poll);
        }

        return NotFound();
    }
    
    /// <summary>
    /// Add a vote to a poll.
    /// </summary>
    /// <param name="pollId">Id of the poll to which add a vote.</param>
    /// <param name="createVoteRequest">Vote to add.</param>
    /// <returns>The created vote.</returns>
    [HttpPost("{pollId:guid}/votes")]
    public async Task<ActionResult<Vote>> Vote(Guid pollId, [FromBody] CreateVoteRequest createVoteRequest)
    {
        return await pollApplication.Vote(pollId,createVoteRequest); 
    }
        
    /// <summary>
    /// Delete a vote from its id
    /// </summary>
    /// <param name="pollId">The id of the poll to which the vote belongs</param>
    /// <param name="voteId">The id of the vote to delete</param>
    [HttpDelete("{id:guid}/votes/{voteId:guid}")]
    public async Task<ActionResult<Poll>> RemoveVote(Guid pollId, Guid voteId)
    {
        await pollApplication.RemoveVote(voteId);
        return NoContent();
    }
}