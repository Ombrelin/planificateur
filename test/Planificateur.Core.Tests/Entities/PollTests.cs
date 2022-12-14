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
        var poll = new Poll(guid,"Test Name", new List<DateTime> { DateTime.UtcNow });

        // Then
        poll.Id.Should().Be(guid);
    }

    [Fact]
    public void NewPoll_NoGuid_GeneratesGuid()
    {
        // When
        var poll = new Poll("Test Name", new List<DateTime> { DateTime.UtcNow });

        // Then
        poll.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void NewPoll_NoExpirationDate_SetsExpirationDateInTwoMonths()
    {
        // When
        var poll = new Poll("Test Name", new List<DateTime> { DateTime.UtcNow });

        // Then
        poll.ExpirationDate.Should().BeCloseTo(DateTime.UtcNow.AddMonths(2), TimeSpan.FromMinutes(1));
    }

    [Fact]
    public void NewPoll_EmptyDates_Throws()
    {
        // When
        var act = () => new Poll("Test Name", new List<DateTime>());
        ;

        // Then
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void NewPoll_EmptyName_Throws()
    {
        // When
        var act = () => new Poll(string.Empty, new List<DateTime> { DateTime.UtcNow });
        ;

        // Then
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void BestDates_NoVotes_ReturnsEmpty()
    {
        // When
        var poll = new Poll("Test Name", new List<DateTime> { DateTime.UtcNow });

        // Then
        poll.BestDates.dates.Should().BeEmpty();
        poll.BestDates.score.Should().BeNull();
    }

    [Fact]
    public void BestDates_ReturnsDatesWithBestScoreAndCorrectScore()
    {
        // Given
        var poll = new Poll
        (
            "Test Name",
            new List<DateTime>
            {
                new(2022, 11, 13),
                new(2022, 11, 14),
                new(2022, 11, 15),
                new(2022, 11, 16),
                new(2022, 11, 17),
            }
        );
        poll.Votes = new List<Vote>
        {
            new Vote(
                poll.Id,
                "Ars??ne"
            )
            {
                Availabilities = new List<Availability>
                {
                    Availability.Available,
                    Availability.Available,
                    Availability.NotAvailable,
                    Availability.NotAvailable,
                    Availability.Available
                }
            },
            new Vote(
                poll.Id,
                "Matthieu"
            )
            {
                Availabilities = new List<Availability>
                {
                    Availability.Available,
                    Availability.Available,
                    Availability.Available,
                    Availability.Available,
                    Availability.NotAvailable
                }
            },
            new Vote(poll.Id, "Gautier")
            {
                Availabilities = new List<Availability>
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