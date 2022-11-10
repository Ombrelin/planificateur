using FluentAssertions;
using Planificateur.Core.Entities;

namespace Planificateur.Core.Tests.Entities;

public class PollTests
{
    [Fact]
    public void NewPoll_WithGuid_SetsGuid()
    {
        // Given
        var guid = Guid.NewGuid();
        
        // When
        var poll = new Poll {Id = guid, Name = "Test Name"};
        
        // Then
        poll.Id.Should().Be(guid);
    }
    
    [Fact]
    public void NewPoll_NoGuid_GeneratesGuid()
    {
        // When
        var poll = new Poll {Name = "Test Name"};
        
        // Then
        poll.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void NewPoll_NoExpirationDate_SetsExpirationDateInTwoMonths()
    {
        // When
        var poll = new Poll {Name = "Test Name"};
        
        // Then
        poll.ExpirationDate.Should().BeCloseTo(DateTime.Now.AddMonths(2), TimeSpan.FromMinutes(1));
    }
}