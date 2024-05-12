using System.Text.Json.Serialization;
using Planificateur.Core.Contracts;
using Planificateur.Core.Entities;

namespace Planificateur.Web.Json;

[JsonSerializable(typeof(CreatePollRequest))]
[JsonSerializable(typeof(List<PollWithoutVotes>))]
[JsonSerializable(typeof(PollWithoutVotes[]))]
[JsonSerializable(typeof(List<PollWithVotes>))]
[JsonSerializable(typeof(List<Planificateur.Core.Contracts.Vote>))]
[JsonSerializable(typeof(object))]
[JsonSerializable(typeof(CreateVoteRequest))]
[JsonSerializable(typeof(RegisterRequest))]
[JsonSerializable(typeof(RegisterResponse))]
[JsonSerializable(typeof(LoginRequest))]
[JsonSerializable(typeof(LoginResponse))]
public partial class SourceGenerationSerialiser : JsonSerializerContext
{
}