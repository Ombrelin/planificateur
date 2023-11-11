using System.Net;
using Planificateur.Core.Contracts;

namespace Planificateur.ClientSdk.ClientSdk;

public interface IPlanificateurClient
{
    Uri BaseUri { get; }
    Task<(PollWithoutVotes?, HttpStatusCode)> CreatePoll(CreatePollRequest createPollRequest);
    Task<(PollWithVotes?, HttpStatusCode)> GetPoll(Guid id);
    Task<(IReadOnlyCollection<PollWithoutVotes>?, HttpStatusCode)> GetPolls();
    Task<(Vote?, HttpStatusCode)> Vote(Guid pollId, CreateVoteRequest createVoteRequest);
    Task<HttpStatusCode> RemoveVote(Guid pollId, Guid voteId);
    Task<(RegisterResponse?, HttpStatusCode)> Register(RegisterRequest request);
    Task<(LoginResponse?, HttpStatusCode)> Login(LoginRequest request);
}