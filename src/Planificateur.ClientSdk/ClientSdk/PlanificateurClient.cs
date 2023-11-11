using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Planificateur.Core.Contracts;
using Planificateur.Web.Json;

namespace Planificateur.ClientSdk.ClientSdk;

public class PlanificateurClient : IPlanificateurClient
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    static PlanificateurClient()
    {
        JsonOptions.Converters.Add(new JsonStringEnumConverter());
        JsonOptions.AddContext<SourceGenerationSerialiser>();
    }
    
    private readonly HttpClient httpClient;

    public PlanificateurClient(HttpClient httpClient)
    {
        ArgumentNullException.ThrowIfNull(httpClient.BaseAddress);
        this.httpClient = httpClient;
    }


    public Uri BaseUri => this.httpClient.BaseAddress!;

    public async Task<(PollWithoutVotes?, HttpStatusCode)> CreatePoll(CreatePollRequest createPollRequest)
    {
        HttpResponseMessage response = await this.httpClient.PostAsJsonAsync($"api/polls", createPollRequest, JsonOptions);

        try
        {
            return (await response.Content.ReadFromJsonAsync<PollWithoutVotes>(JsonOptions), response.StatusCode);
        }
        catch
        {
            return (null, response.StatusCode);
        }
    }


    public async Task<(PollWithVotes?, HttpStatusCode)> GetPoll(Guid id)
    {
        HttpResponseMessage response = await this.httpClient.GetAsync($"api/polls/{id}");
        try
        {
            return (await response.Content.ReadFromJsonAsync<PollWithVotes>(JsonOptions), response.StatusCode);
        }
        catch
        {
            return (null, response.StatusCode);
        }
    }


    public async Task<(IReadOnlyCollection<PollWithoutVotes>?, HttpStatusCode)> GetPolls()
    {
        HttpResponseMessage response = await this.httpClient.GetAsync($"api/polls");
        try
        {
            return (await response.Content.ReadFromJsonAsync<PollWithoutVotes[]>(JsonOptions), response.StatusCode);
        }
        catch
        {
            return (null, response.StatusCode);
        }
    }


    public async Task<(Vote?, HttpStatusCode)> Vote(Guid pollId, CreateVoteRequest createVoteRequest)
    {
        HttpResponseMessage response =
            await httpClient.PostAsJsonAsync($"api/polls/{pollId}/votes", createVoteRequest, JsonOptions);
        try
        {
            return (await response.Content.ReadFromJsonAsync<Vote>(JsonOptions), response.StatusCode);
        }
        catch
        {
            return (null, response.StatusCode);
        }
    }

    public async Task<HttpStatusCode> RemoveVote(Guid pollId, Guid voteId)
    {
        HttpResponseMessage response = await this.httpClient.DeleteAsync($"api/polls/{pollId}/votes/{voteId}");
        return response.StatusCode;
    }


    public async Task<(RegisterResponse?, HttpStatusCode)> Register(RegisterRequest request)
    {
        HttpResponseMessage response = await this.httpClient.PostAsJsonAsync($"api/authentication/register", request, JsonOptions);
        try
        {
            return (await response.Content.ReadFromJsonAsync<RegisterResponse>(), response.StatusCode);
        }
        catch
        {
            return (null, response.StatusCode);
        }
    }

    public async Task<(LoginResponse?, HttpStatusCode)> Login(LoginRequest request)
    {
        HttpResponseMessage response = await this.httpClient.PostAsJsonAsync($"api/authentication/login", request, JsonOptions);
        try
        {
            var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", loginResponse!.Token);

            return (loginResponse, response.StatusCode);
        }
        catch
        {
            return (null, response.StatusCode);
        }
    }
}