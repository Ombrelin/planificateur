using System.Text.Json.Serialization;
using Planificateur.Core.Contracts;
using Planificateur.Core.Entities;

namespace Planificateur.Web.Json;

[JsonSerializable(typeof(CreatePollRequest))]
[JsonSerializable(typeof(Poll))]
[JsonSerializable(typeof(Poll[]))]
[JsonSerializable(typeof(List<PollWithoutVotes>))]
[JsonSerializable(typeof(object))]
[JsonSerializable(typeof(CreateVoteRequest))]
[JsonSerializable(typeof(RegisterRequest))]
[JsonSerializable(typeof(RegisterResponse))]
[JsonSerializable(typeof(LoginRequest))]
[JsonSerializable(typeof(LoginResponse))]
internal partial class SourceGenerationSerialiser : JsonSerializerContext
{
}