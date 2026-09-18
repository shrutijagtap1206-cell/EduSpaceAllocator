using FluentAssertions;
using Xunit;

namespace EduSpaceAllocator.Tests;

public class ScheduleConflictTests
{
    public class TimeSlot
    {
        public DayOfWeek Day { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public bool ConflictsWith(TimeSlot other)
        {
            if (Day != other.Day)
                return false;

            return StartTime < other.EndTime && other.StartTime < EndTime;
        }
    }

    [Fact]
    public void OverlappingSlots_OnSameDay_ShouldDetectConflict()
    {
        var slot1 = new TimeSlot
        {
            Day = DayOfWeek.Monday,
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(11, 0, 0)
        };

        var slot2 = new TimeSlot
        {
            Day = DayOfWeek.Monday,
            StartTime = new TimeSpan(10, 0, 0),
            EndTime = new TimeSpan(12, 0, 0)
        };

        slot1.ConflictsWith(slot2).Should().BeTrue();
        slot2.ConflictsWith(slot1).Should().BeTrue();
    }

    [Fact]
    public void ConsecutiveSlots_OnSameDay_ShouldNotConflict()
    {
        var slot1 = new TimeSlot
        {
            Day = DayOfWeek.Monday,
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(11, 0, 0)
        };

        var slot2 = new TimeSlot
        {
            Day = DayOfWeek.Monday,
            StartTime = new TimeSpan(11, 0, 0),
            EndTime = new TimeSpan(13, 0, 0)
        };

        slot1.ConflictsWith(slot2).Should().BeFalse();
        slot2.ConflictsWith(slot1).Should().BeFalse();
    }

    [Fact]
    public void IdenticalSlots_OnDifferentDays_ShouldNotConflict()
    {
        var slot1 = new TimeSlot
        {
            Day = DayOfWeek.Monday,
            StartTime = new TimeSpan(10, 0, 0),
            EndTime = new TimeSpan(12, 0, 0)
        };

        var slot2 = new TimeSlot
        {
            Day = DayOfWeek.Tuesday,
            StartTime = new TimeSpan(10, 0, 0),
            EndTime = new TimeSpan(12, 0, 0)
        };

        slot1.ConflictsWith(slot2).Should().BeFalse();
    }
}
