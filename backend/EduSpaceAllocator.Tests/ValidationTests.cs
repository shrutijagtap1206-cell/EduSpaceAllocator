using System.ComponentModel.DataAnnotations;
using EduSpaceAllocator.API.DTOs;
using FluentAssertions;
using Xunit;

namespace EduSpaceAllocator.Tests;

public class ValidationTests
{
    private static IList<ValidationResult> ValidateModel(object model)
    {
        var validationResults = new List<ValidationResult>();
        var ctx = new ValidationContext(model, null, null);
        Validator.TryValidateObject(model, ctx, validationResults, true);
        return validationResults;
    }

    [Fact]
    public void ValidSpaceDto_ShouldPassValidation()
    {
        var validSpace = new SpaceDto
        {
            SpaceId = 1,
            BuildingName = "Community Learning Center",
            Address = "123 MG Road, Pune",
            City = "Pune",
            Latitude = 18.5204,
            Longitude = 73.8567,
            FloorArea = 1200,
            Capacity = 45,
            RentalCost = 15000,
            Availability = true
        };

        var errors = ValidateModel(validSpace);

        errors.Should().BeEmpty();
    }

    [Theory]
    [InlineData("", "Short address", "Pune", 18.5, 73.8, 500, 30, 5000)]
    [InlineData("Valid Building", "A", "Pune", 18.5, 73.8, 500, 30, 5000)]
    [InlineData("Valid Building", "Valid Address", "P", 18.5, 73.8, 500, 30, 5000)]
    [InlineData("Valid Building", "Valid Address", "Pune", 150.0, 73.8, 500, 30, 5000)]
    [InlineData("Valid Building", "Valid Address", "Pune", 18.5, 200.0, 500, 30, 5000)]
    [InlineData("Valid Building", "Valid Address", "Pune", 18.5, 73.8, -10, 30, 5000)]
    [InlineData("Valid Building", "Valid Address", "Pune", 18.5, 73.8, 500, 0, 5000)]
    [InlineData("Valid Building", "Valid Address", "Pune", 18.5, 73.8, 500, 30, -100)]
    public void InvalidSpaceDto_ShouldFailValidation(
        string buildingName,
        string address,
        string city,
        double latitude,
        double longitude,
        double floorArea,
        int capacity,
        decimal rentalCost)
    {
        var badSpace = new SpaceDto
        {
            BuildingName = buildingName,
            Address = address,
            City = city,
            Latitude = latitude,
            Longitude = longitude,
            FloorArea = floorArea,
            Capacity = capacity,
            RentalCost = rentalCost,
            Availability = true
        };

        var errors = ValidateModel(badSpace);

        errors.Should().NotBeEmpty();
    }

    [Fact]
    public void ValidLearningRequestDto_ShouldPassValidation()
    {
        var validRequest = new LearningRequestDto
        {
            RequestId = 1,
            Organization = "Pratham NGO",
            ProgramType = "Remedial Coding",
            StudentCapacity = 35,
            Budget = 20000,
            PreferredLocation = "Shivajinagar, Pune",
            PreferredLatitude = 18.5314,
            PreferredLongitude = 73.8446,
            CommunityId = 3
        };

        var errors = ValidateModel(validRequest);

        errors.Should().BeEmpty();
    }

    [Theory]
    [InlineData("", "Program", 30, 10000, "Location", 18.5, 73.8, 1)]
    [InlineData("Org", "", 30, 10000, "Location", 18.5, 73.8, 1)]
    [InlineData("Org", "Program", 0, 10000, "Location", 18.5, 73.8, 1)]
    [InlineData("Org", "Program", 30, -500, "Location", 18.5, 73.8, 1)]
    [InlineData("Org", "Program", 30, 10000, "L", 18.5, 73.8, 1)]
    [InlineData("Org", "Program", 30, 10000, "Location", -95.0, 73.8, 1)]
    [InlineData("Org", "Program", 30, 10000, "Location", 18.5, 200.0, 1)]
    [InlineData("Org", "Program", 30, 10000, "Location", 18.5, 73.8, 0)]
    public void InvalidLearningRequestDto_ShouldFailValidation(
        string org,
        string program,
        int students,
        decimal budget,
        string location,
        double lat,
        double lon,
        int communityId)
    {
        var badRequest = new LearningRequestDto
        {
            Organization = org,
            ProgramType = program,
            StudentCapacity = students,
            Budget = budget,
            PreferredLocation = location,
            PreferredLatitude = lat,
            PreferredLongitude = lon,
            CommunityId = communityId
        };

        var errors = ValidateModel(badRequest);

        errors.Should().NotBeEmpty();
    }
}
