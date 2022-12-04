using FluentAssertions;
using Planificateur.Core.Entities;

namespace Planificateur.Core.Tests.Entities;

public class VoteTests
{
    [Theory]
    [InlineData("b34d188e-75d1-4a22-89fb-03401b0c6df5", "TestVoterName", "vote-b34d188e-75d1-4a22-89fb-03401b0c6df5-TestVoterName")]
    [InlineData("b34d188e-75d1-4a22-89fb-03401b0c6df5", "Test Voter Name", "vote-b34d188e-75d1-4a22-89fb-03401b0c6df5-Test_Voter_Name")]
    public void Id_SetsExpectedId(string pollId, string voterName, string expectedId)
    {
        // When
        var vote = new Vote { PollId = Guid.Parse(pollId), VoterName = voterName };
        
        // then
        vote.Id.Should().Be(expectedId);
    }
}