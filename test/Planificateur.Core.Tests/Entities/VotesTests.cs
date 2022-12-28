using FluentAssertions;
using Planificateur.Core.Entities;

namespace Planificateur.Core.Tests.Entities;

public class VotesTests
{
    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void VoterName_WhenInvlaid_ThenThrows(string invalidVoterName)
    {
        // When
        var act = () => new Vote(Guid.NewGuid(), invalidVoterName);
        
        // Then
        act.Should().Throw<ArgumentException>();
    }
}