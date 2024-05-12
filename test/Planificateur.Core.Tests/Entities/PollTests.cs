using FluentAssertions;
using Planificateur.Core.Entities;

namespace Planificateur.Core.Tests.Entities;

public class PollTests
{
    [Fact]
    public void NewPoll_NoGuid_GeneratesGuid()
    {
        // When
        var poll = new Poll("Test Name", new[] { DateTime.UtcNow, DateTime.UtcNow.AddDays(1) });

        // Then
        poll.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void NewPoll_NoExpirationDate_SetsExpirationDateInTwoMonths()
    {
        // When
        var poll = new Poll("Test Name", new[] { DateTime.UtcNow, DateTime.UtcNow.AddDays(1) });

        // Then
        poll.ExpirationDate.Should().BeCloseTo(DateTime.UtcNow.AddMonths(2), TimeSpan.FromMinutes(1));
    }

    [Fact]
    public void NewPoll_EmptyDates_Throws()
    {
        // When
        var acts = new[]
        {
            () => new Poll("Test Name", Array.Empty<DateTime>()),
            () => new Poll("Test Name", new[] { DateTime.UtcNow }),
        };

        // Then
        foreach (var act in acts)
        {
            act.Should().Throw<ArgumentException>();
        }
    }

    [Fact]
    public void NewPoll_EmptyName_Throws()
    {
        // When
        var act = () => new Poll(string.Empty, new[] { DateTime.UtcNow });
        ;

        // Then
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void BestDates_NoVotes_ReturnsEmpty()
    {
        // When
        var poll = new Poll("Test Name", new[] { DateTime.UtcNow, DateTime.UtcNow.AddDays(1) });

        // Then
        poll.BestDates.dates.Should().BeEmpty();
        poll.BestDates.score.Should().BeNull();
    }


    [Fact]
    public void Constructor_WhenUnOrderedDates_SortsDates()
    {
        // When
        var poll = new Poll
        (
            "Test Name",
            new DateTime[]
            {
                new(2022, 11, 14),
                new(2022, 11, 16),
                new(2022, 11, 17),
                new(2022, 11, 13),
                new(2022, 11, 15),
            }
        );

        // Then
        poll.Dates[0].Should().Be(new DateTime(2022, 11, 13));
        poll.Dates[1].Should().Be(new DateTime(2022, 11, 14));
        poll.Dates[2].Should().Be(new DateTime(2022, 11, 15));
        poll.Dates[3].Should().Be(new DateTime(2022, 11, 16));
        poll.Dates[4].Should().Be(new DateTime(2022, 11, 17));
    }

    [Fact]
    public void BestDates_ReturnsDatesWithBestScoreAndCorrectScore()
    {
        // Given
        var poll = new Poll
        (
            "Test Name",
            new DateTime[]
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
                "Arsène"
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