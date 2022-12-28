using System.Text.Json.Serialization;
using Planificateur.Core.Contracts;
using Planificateur.Core.Entities;

namespace Planificateur.Web.Json;

[JsonSerializable(typeof(CreatePollRequest))]
[JsonSerializable(typeof(Poll))]
[JsonSerializable(typeof(object))]
internal partial class SourceGenerationSerialiser : JsonSerializerContext
{
    
}