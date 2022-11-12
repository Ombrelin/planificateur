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
        var poll = new Poll { Id = guid, Name = "Test Name", Dates = new List<DateTime> { DateTime.Now } };

        // Then
        poll.Id.Should().Be(guid);
    }

    [Fact]
    public void NewPoll_NoGuid_GeneratesGuid()
    {
        // When
        var poll = new Poll { Name = "Test Name", Dates = new List<DateTime> { DateTime.Now } };

        // Then
        poll.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void NewPoll_NoExpirationDate_SetsExpirationDateInTwoMonths()
    {
        // When
        var poll = new Poll { Name = "Test Name", Dates = new List<DateTime> { DateTime.Now } };

        // Then
        poll.ExpirationDate.Should().BeCloseTo(DateTime.Now.AddMonths(2), TimeSpan.FromMinutes(1));
    }

    [Fact]
    public void NewPoll_EmptyDates_Throws()
    {
        // When
        var act = () => new Poll { Name = "Test Name", Dates = new List<DateTime>() };

        // Then
        act.Should().Throw<ArgumentException>();
    }
    
    [Fact]
    public void NewPoll_EmptyName_Throws()
    {
        // When
        var act = () => new Poll { Name = string.Empty, Dates = new List<DateTime> { DateTime.Now } };

        // Then
        act.Should().Throw<ArgumentException>();
    }
    
    [Fact]
    public void BestDates_NoVotes_ReturnsEmpty()
    {
        // When
        var poll = new Poll { Name = "Test Name", Dates = new List<DateTime> { DateTime.Now } };

        // Then
        poll.BestDates.dates.Should().BeEmpty();
        poll.BestDates.score.Should().BeNull();
    }

    [Fact]
    public void BestDates_ReturnsDatesWithBestScoreAndCorrectScore()
    {
        // Given
        var poll = new Poll
        {
            Name = "Test Name",
            Dates = new List<DateTime>
            {
                new(2022, 11, 13),
                new(2022, 11, 14),
                new(2022, 11, 15),
                new(2022, 11, 16),
                new(2022, 11, 17),
            }
        };
        poll.Votes = new List<Vote>
        {
            new()
            {
                VoterName = "Arsène",
                PollId = poll.Id,
                Availability = new List<Availability>
                {
                    Availability.Available,
                    Availability.Available,
                    Availability.NotAvailable,
                    Availability.NotAvailable,
                    Availability.Available
                }
            },
            new()
            {
                VoterName = "Matthieu",
                PollId = poll.Id,
                Availability = new List<Availability>
                {
                    Availability.Available,
                    Availability.Available,
                    Availability.Available,
                    Availability.Available,
                    Availability.NotAvailable
                }
            },
            new()
            {
                VoterName = "Gautier",
                PollId = poll.Id,
                Availability = new List<Availability>
                {
                    Availability.NotAvailable,
                    Availability.Available,
                    Availability.NotAvailable,
                    Availability.Available,
                    Availability.NotAvailable
                }
            }
        };

        // When
        var bestDates = poll.BestDates;

        // Then
        bestDates.dates.Should().ContainSingle();
        bestDates.dates.First().Should().Be(new DateTime(2022, 11, 14));
        bestDates.score.Should().Be(3);
    }
}