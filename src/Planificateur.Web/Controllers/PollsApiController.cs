using System.Collections;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Planificateur.Core;
using Planificateur.Core.Contracts;
using Planificateur.Core.Entities;
using Vote = Planificateur.Core.Contracts.Vote;

namespace Planificateur.Web.Controllers;

[AllowAnonymous]
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
    public async Task<ActionResult<PollWithoutVotes>> CreatePoll([FromBody] CreatePollRequest createPollRequest)
    {
        PollWithoutVotes poll = await pollApplication.CreatePoll(createPollRequest);
        return Created($"/api/polls/{poll.Id}", poll);
    }

    /// <summary>
    /// Get a Poll from its Id.
    /// </summary>
    /// <param name="id">The Poll Id.</param>
    /// <returns>The corresponding Poll.</returns>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PollWithVotes>> GetPoll(Guid id)
    {
        PollWithVotes? poll = await pollApplication.GetPoll(id);
        if (poll is not null)
        {
            return Ok(poll);
        }

        return NotFound();
    }

    /// <summary>
    /// Get the polls for which the current authenticated user is the author.
    /// </summary>
    /// <returns>The corresponding list of polls.</returns>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<IReadOnlyPollWithoutVotes>>> GetPolls()
    {
        return Ok(await pollApplication.GetCurrentUserPolls());
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
        return await pollApplication.Vote(pollId, createVoteRequest);
    }

    /// <summary>
    /// Delete a vote from its id
    /// </summary>
    /// <param name="pollId">The id of the poll to which the vote belongs.</param>
    /// <param name="voteId">The id of the vote to delete.</param>
    [HttpDelete("{pollId:guid}/votes/{voteId:guid}")]
    public async Task<IActionResult> RemoveVote(Guid pollId, Guid voteId)
    {
        await pollApplication.RemoveVote(voteId);
        return NoContent();
    }
}