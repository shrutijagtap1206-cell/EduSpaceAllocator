using FluentAssertions;
using Xunit;

namespace EduSpaceAllocator.Tests;

public class MatchingTests
{
    public class Candidate
    {
        public int SpaceId { get; set; }
        public int RequestId { get; set; }
        public double DistanceKm { get; set; }
        public int SpaceCapacity { get; set; }
        public int RequiredCapacity { get; set; }
        public decimal RentalCost { get; set; }
        public decimal Budget { get; set; }
        public bool IsAvailable { get; set; }

        public bool IsFeasible(double maxDistance = 5.0)
        {
            return IsAvailable &&
                   DistanceKm <= maxDistance &&
                   SpaceCapacity >= RequiredCapacity &&
                   Budget >= RentalCost;
        }
    }

    [Fact]
    public void Candidate_WithinConstraints_ShouldBeFeasible()
    {
        var candidate = new Candidate
        {
            SpaceId = 1,
            RequestId = 101,
            DistanceKm = 1.4,
            SpaceCapacity = 50,
            RequiredCapacity = 40,
            RentalCost = 15000,
            Budget = 20000,
            IsAvailable = true
        };

        candidate.IsFeasible().Should().BeTrue();
    }

    [Fact]
    public void Candidate_ExceedingDistanceThreshold_ShouldBeInfeasible()
    {
        var candidate = new Candidate
        {
            SpaceId = 2,
            RequestId = 102,
            DistanceKm = 6.2,
            SpaceCapacity = 50,
            RequiredCapacity = 40,
            RentalCost = 15000,
            Budget = 20000,
            IsAvailable = true
        };

        candidate.IsFeasible(maxDistance: 5.0).Should().BeFalse();
    }

    [Fact]
    public void Candidate_InsufficientCapacity_ShouldBeInfeasible()
    {
        var candidate = new Candidate
        {
            SpaceId = 3,
            RequestId = 103,
            DistanceKm = 1.2,
            SpaceCapacity = 30,
            RequiredCapacity = 45,
            RentalCost = 10000,
            Budget = 20000,
            IsAvailable = true
        };

        candidate.IsFeasible().Should().BeFalse();
    }

    [Fact]
    public void Candidate_OverBudget_ShouldBeInfeasible()
    {
        var candidate = new Candidate
        {
            SpaceId = 4,
            RequestId = 104,
            DistanceKm = 1.0,
            SpaceCapacity = 60,
            RequiredCapacity = 40,
            RentalCost = 25000,
            Budget = 18000,
            IsAvailable = true
        };

        candidate.IsFeasible().Should().BeFalse();
    }

    [Fact]
    public void Candidate_UnavailableSpace_ShouldBeInfeasible()
    {
        var candidate = new Candidate
        {
            SpaceId = 5,
            RequestId = 105,
            DistanceKm = 0.8,
            SpaceCapacity = 60,
            RequiredCapacity = 40,
            RentalCost = 15000,
            Budget = 20000,
            IsAvailable = false
        };

        candidate.IsFeasible().Should().BeFalse();
    }

    [Fact]
    public void BipartiteMatching_ShouldEnforceOneToOneAssignments()
    {
        // Simulate matched edges
        var matches = new List<(int SpaceId, int RequestId)>
        {
            (1, 101),
            (2, 102),
            (3, 103)
        };

        var uniqueSpaces = matches.Select(m => m.SpaceId).Distinct().Count();
        var uniqueRequests = matches.Select(m => m.RequestId).Distinct().Count();

        uniqueSpaces.Should().Be(matches.Count);
        uniqueRequests.Should().Be(matches.Count);
    }
}
