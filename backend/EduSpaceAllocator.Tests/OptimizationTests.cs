using FluentAssertions;
using Xunit;

namespace EduSpaceAllocator.Tests;

public class OptimizationTests
{
    [Fact]
    public void CapacityUtilization_ShouldNeverExceedOne()
    {
        var students = 100;
        var capacity = 50;

        var utilization = Math.Min(1.0, (double)students / capacity);

        utilization.Should().Be(1.0);
    }

    [Fact]
    public void CapacityUtilization_ShouldCalculateAccurately()
    {
        var students = 40;
        var capacity = 50;

        var utilization = Math.Min(1.0, (double)students / capacity);

        utilization.Should().Be(0.8);
    }

    [Fact]
    public void CostEfficiency_ShouldBeCappedAtOne()
    {
        decimal budget = 25000;
        decimal rentalCost = 20000;

        var efficiency = Math.Min(1.0, (double)(budget / rentalCost));

        efficiency.Should().Be(1.0);
    }

    [Fact]
    public void CostEfficiency_ShouldReflectBudgetConstraint()
    {
        decimal budget = 15000;
        decimal rentalCost = 20000;

        var efficiency = Math.Min(1.0, (double)(budget / rentalCost));

        efficiency.Should().Be(0.75);
    }

    [Theory]
    [InlineData(0.0, 5.0, 1.0)]
    [InlineData(2.5, 5.0, 0.5)]
    [InlineData(5.0, 5.0, 0.0)]
    [InlineData(7.0, 5.0, 0.0)]
    public void DistanceScore_ShouldDecreaseLinearlyUntilZero(double distance, double maxDistance, double expectedScore)
    {
        var score = Math.Max(0.0, 1.0 - (distance / maxDistance));

        score.Should().BeApproximately(expectedScore, 0.001);
    }

    [Fact]
    public void CompositeOptimizationScore_ShouldWeightedAverageComponents()
    {
        // Weights: Distance = 0.4, Capacity = 0.35, Cost = 0.25
        double wDistance = 0.40;
        double wCapacity = 0.35;
        double wCost = 0.25;

        double scoreDistance = 0.80;
        double scoreCapacity = 0.70;
        double scoreCost = 0.60;

        double composite = (wDistance * scoreDistance) + (wCapacity * scoreCapacity) + (wCost * scoreCost);

        // 0.40 * 0.80 = 0.32
        // 0.35 * 0.70 = 0.245
        // 0.25 * 0.60 = 0.15
        // Sum = 0.715 (71.5%)
        composite.Should().BeApproximately(0.715, 0.001);
    }
}
