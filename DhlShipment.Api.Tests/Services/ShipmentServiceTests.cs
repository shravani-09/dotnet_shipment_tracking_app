using DhlShipment.Api.DTOs;
using DhlShipment.Api.Models;
using DhlShipment.Api.Services;
using FluentAssertions;
using Xunit;

namespace DhlShipment.Api.Tests.Services;

public class ShipmentServiceTests
{
    private readonly ShipmentService _service;

    public ShipmentServiceTests()
    {
        _service = new ShipmentService();
    }

    #region CreateShipment Tests

    [Fact]
    public void CreateShipment_WithValidData_ReturnsShipmentResponseDto()
    {
        var dto = new CreateShipmentDto(
            Origin: "New York",
            Destination: "Los Angeles",
            EstimatedDeliveryDate: DateTime.UtcNow.AddDays(5)
        );

        var result = _service.CreateShipment(dto);

        result.Should().NotBeNull();
        result.Origin.Should().Be("New York");
        result.Destination.Should().Be("Los Angeles");
        result.TrackingId.Should().StartWith("DHL");
        result.CurrentStatus.Should().Be(ShipmentStatus.Created);
        result.Milestones.Should().HaveCount(1);
    }

    [Fact]
    public void CreateShipment_GeneratesUniqueTrackingId()
    {
        var dto = new CreateShipmentDto(
            Origin: "London",
            Destination: "Paris",
            EstimatedDeliveryDate: DateTime.UtcNow.AddDays(3)
        );

        var result1 = _service.CreateShipment(dto);
        var result2 = _service.CreateShipment(dto);

        result1.TrackingId.Should().NotBe(result2.TrackingId);
    }

    [Fact]
    public void CreateShipment_CreatesInitialMilestone()
    {
        var origin = "Tokyo";
        var dto = new CreateShipmentDto(
            Origin: origin,
            Destination: "Singapore",
            EstimatedDeliveryDate: DateTime.UtcNow.AddDays(7)
        );

        var result = _service.CreateShipment(dto);

        result.Milestones.Should().HaveCount(1);
        var firstMilestone = result.Milestones.First();
        firstMilestone.Status.Should().Be(ShipmentStatus.Created);
        firstMilestone.Location.Should().Be(origin);
        firstMilestone.Timestamp.Should().BeBefore(DateTime.UtcNow.AddSeconds(1));
    }

    [Fact]
    public void CreateShipment_SetsEstimatedDeliveryDate()
    {
        var estimatedDate = DateTime.UtcNow.AddDays(10);
        var dto = new CreateShipmentDto(
            Origin: "Dubai",
            Destination: "Mumbai",
            EstimatedDeliveryDate: estimatedDate
        );

        var result = _service.CreateShipment(dto);

        result.EstimatedDeliveryDate.Should().Be(estimatedDate);
    }

    #endregion

    #region GetShipmentByTrackingId Tests

    [Fact]
    public void GetShipmentByTrackingId_WithValidTrackingId_ReturnsShipment()
    {
        var createDto = new CreateShipmentDto(
            Origin: "Berlin",
            Destination: "Amsterdam",
            EstimatedDeliveryDate: DateTime.UtcNow.AddDays(2)
        );
        var created = _service.CreateShipment(createDto);

        var result = _service.GetShipmentByTrackingId(created.TrackingId);

        result.Should().NotBeNull();
        result.TrackingId.Should().Be(created.TrackingId);
        result.Origin.Should().Be("Berlin");
        result.Destination.Should().Be("Amsterdam");
    }

    [Fact]
    public void GetShipmentByTrackingId_WithInvalidTrackingId_ThrowsKeyNotFoundException()
    {
        var action = () => _service.GetShipmentByTrackingId("INVALID123");
        action.Should().Throw<KeyNotFoundException>()
            .WithMessage("Shipment not found");
    }

    [Fact]
    public void GetShipmentByTrackingId_ReturnsCorrectMilestones()
    {
        var createDto = new CreateShipmentDto(
            Origin: "Sydney",
            Destination: "Melbourne",
            EstimatedDeliveryDate: DateTime.UtcNow.AddDays(1)
        );
        var created = _service.CreateShipment(createDto);

        var result = _service.GetShipmentByTrackingId(created.TrackingId);

        result.Milestones.Should().HaveCount(1);
        result.Milestones.First().Status.Should().Be(ShipmentStatus.Created);
    }

    #endregion

    #region UpdateShipmentStatus Tests

    [Fact]
    public void UpdateShipmentStatus_WithValidData_UpdatesStatusAndAddsMilestone()
    {
        var createDto = new CreateShipmentDto(
            Origin: "Istanbul",
            Destination: "Athens",
            EstimatedDeliveryDate: DateTime.UtcNow.AddDays(4)
        );
        var created = _service.CreateShipment(createDto);

        var updateDto = new UpdateShipmentStatusDto
        {
            Status = ShipmentStatus.PickedUp,
            Location = "Istanbul Airport"
        };

        var result = _service.UpdateShipmentStatus(created.TrackingId, updateDto);

        result.Should().NotBeNull();
        result.CurrentStatus.Should().Be(ShipmentStatus.PickedUp);
        result.Milestones.Should().HaveCount(2);
        result.Milestones.Last().Status.Should().Be(ShipmentStatus.PickedUp);
        result.Milestones.Last().Location.Should().Be("Istanbul Airport");
    }

    [Fact]
    public void UpdateShipmentStatus_WithInvalidTrackingId_ThrowsKeyNotFoundException()
    {
        var updateDto = new UpdateShipmentStatusDto
        {
            Status = ShipmentStatus.InTransit,
            Location = "Somewhere"
        };

        var action = () => _service.UpdateShipmentStatus("INVALID123", updateDto);
        action.Should().Throw<KeyNotFoundException>()
            .WithMessage("Shipment not found");
    }

    [Fact]
    public void UpdateShipmentStatus_CanUpdateMultipleTimes()
    {
        var createDto = new CreateShipmentDto(
            Origin: "Bangkok",
            Destination: "Ho Chi Minh",
            EstimatedDeliveryDate: DateTime.UtcNow.AddDays(3)
        );
        var created = _service.CreateShipment(createDto);

        var update1 = new UpdateShipmentStatusDto { Status = ShipmentStatus.PickedUp, Location = "Bangkok" };
        var update2 = new UpdateShipmentStatusDto { Status = ShipmentStatus.InTransit, Location = "Bangkok Airport" };
        var update3 = new UpdateShipmentStatusDto { Status = ShipmentStatus.ArrivedAtFacility, Location = "Ho Chi Minh Facility" };
        var update4 = new UpdateShipmentStatusDto { Status = ShipmentStatus.OutForDelivery, Location = "Ho Chi Minh" };

        var result1 = _service.UpdateShipmentStatus(created.TrackingId, update1);
        var result2 = _service.UpdateShipmentStatus(created.TrackingId, update2);
        var result3 = _service.UpdateShipmentStatus(created.TrackingId, update3);
        var result4 = _service.UpdateShipmentStatus(created.TrackingId, update4);

        result4.Milestones.Should().HaveCount(5);
        result4.CurrentStatus.Should().Be(ShipmentStatus.OutForDelivery);
        result4.Milestones.Should().Contain(m => m.Status == ShipmentStatus.Created);
        result4.Milestones.Should().Contain(m => m.Status == ShipmentStatus.PickedUp);
        result4.Milestones.Should().Contain(m => m.Status == ShipmentStatus.InTransit);
        result4.Milestones.Should().Contain(m => m.Status == ShipmentStatus.ArrivedAtFacility);
        result4.Milestones.Should().Contain(m => m.Status == ShipmentStatus.OutForDelivery);
    }

    [Fact]
    public void UpdateShipmentStatus_MaintainsTrackingIdAndOriginDestination()
    {
        var createDto = new CreateShipmentDto(
            Origin: "Singapore",
            Destination: "Malaysia",
            EstimatedDeliveryDate: DateTime.UtcNow.AddDays(2)
        );
        var created = _service.CreateShipment(createDto);
        var trackingId = created.TrackingId;

        // Follow valid transition path to Delivered
        var update1 = new UpdateShipmentStatusDto { Status = ShipmentStatus.PickedUp, Location = "Singapore" };
        var update2 = new UpdateShipmentStatusDto { Status = ShipmentStatus.InTransit, Location = "Singapore Airport" };
        var update3 = new UpdateShipmentStatusDto { Status = ShipmentStatus.ArrivedAtFacility, Location = "Malaysia Facility" };
        var update4 = new UpdateShipmentStatusDto { Status = ShipmentStatus.OutForDelivery, Location = "Malaysia" };
        var update5 = new UpdateShipmentStatusDto { Status = ShipmentStatus.Delivered, Location = "Malaysia" };

        _service.UpdateShipmentStatus(trackingId, update1);
        _service.UpdateShipmentStatus(trackingId, update2);
        _service.UpdateShipmentStatus(trackingId, update3);
        _service.UpdateShipmentStatus(trackingId, update4);
        var result = _service.UpdateShipmentStatus(trackingId, update5);

        result.TrackingId.Should().Be(trackingId);
        result.Origin.Should().Be("Singapore");
        result.Destination.Should().Be("Malaysia");
    }

    [Fact]
    public void UpdateShipmentStatus_TimestampIsRecent()
    {
        var createDto = new CreateShipmentDto(
            Origin: "Cairo",
            Destination: "Giza",
            EstimatedDeliveryDate: DateTime.UtcNow.AddDays(1)
        );
        var created = _service.CreateShipment(createDto);
        var beforeUpdate = DateTime.UtcNow;

        var updateDto = new UpdateShipmentStatusDto
        {
            Status = ShipmentStatus.PickedUp,
            Location = "Cairo"
        };

        var result = _service.UpdateShipmentStatus(created.TrackingId, updateDto);
        var afterUpdate = DateTime.UtcNow;

        var newMilestone = result.Milestones.Last();
        newMilestone.Timestamp.Should().BeOnOrAfter(beforeUpdate);
        newMilestone.Timestamp.Should().BeOnOrBefore(afterUpdate.AddSeconds(1));
    }

    #endregion

    #region GetAllShipments Tests

    [Fact]
    public void GetAllShipments_WithNoShipments_ReturnsEmptyList()
    {
        _service.ClearShipments();
        var result = _service.GetAllShipments();

        result.Should().BeEmpty();
    }

    [Fact]
    public void GetAllShipments_WithMultipleShipments_ReturnsAllShipments()
    {
        _service.ClearShipments();
        var dto1 = new CreateShipmentDto(
            Origin: "New York",
            Destination: "Los Angeles",
            EstimatedDeliveryDate: DateTime.UtcNow.AddDays(5)
        );
        var dto2 = new CreateShipmentDto(
            Origin: "London",
            Destination: "Paris",
            EstimatedDeliveryDate: DateTime.UtcNow.AddDays(3)
        );
        var dto3 = new CreateShipmentDto(
            Origin: "Tokyo",
            Destination: "Singapore",
            EstimatedDeliveryDate: DateTime.UtcNow.AddDays(7)
        );

        _service.CreateShipment(dto1);
        _service.CreateShipment(dto2);
        _service.CreateShipment(dto3);

        var result = _service.GetAllShipments();

        result.Should().HaveCount(3);
    }

    [Fact]
    public void GetAllShipments_ReturnsShipmentsInOrderCreated()
    {
        _service.ClearShipments();
        var dto1 = new CreateShipmentDto(
            Origin: "New York",
            Destination: "Los Angeles",
            EstimatedDeliveryDate: DateTime.UtcNow.AddDays(5)
        );
        var dto2 = new CreateShipmentDto(
            Origin: "London",
            Destination: "Paris",
            EstimatedDeliveryDate: DateTime.UtcNow.AddDays(3)
        );

        var shipment1 = _service.CreateShipment(dto1);
        var shipment2 = _service.CreateShipment(dto2);

        var result = _service.GetAllShipments().ToList();

        result[0].TrackingId.Should().Be(shipment1.TrackingId);
        result[1].TrackingId.Should().Be(shipment2.TrackingId);
    }

    [Fact]
    public void GetAllShipments_IncludesAllShipmentDetails()
    {
        _service.ClearShipments();
        var dto = new CreateShipmentDto(
            Origin: "New York",
            Destination: "Los Angeles",
            EstimatedDeliveryDate: DateTime.UtcNow.AddDays(5)
        );

        _service.CreateShipment(dto);

        var result = _service.GetAllShipments().First();

        result.Should().NotBeNull();
        result.TrackingId.Should().StartWith("DHL");
        result.Origin.Should().Be("New York");
        result.Destination.Should().Be("Los Angeles");
        result.CurrentStatus.Should().Be(ShipmentStatus.Created);
        result.Milestones.Should().HaveCount(1);
    }

    #endregion
}
