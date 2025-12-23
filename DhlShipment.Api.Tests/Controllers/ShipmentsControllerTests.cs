using DhlShipment.Api.Controllers;
using DhlShipment.Api.DTOs;
using DhlShipment.Api.Models;
using DhlShipment.Api.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace DhlShipment.Api.Tests.Controllers;

public class ShipmentsControllerTests
{
    private readonly Mock<IShipmentService> _mockShipmentService;
    private readonly ShipmentsController _controller;

    public ShipmentsControllerTests()
    {
        _mockShipmentService = new Mock<IShipmentService>();
        _controller = new ShipmentsController(_mockShipmentService.Object);
    }

    #region GetByTrackingId Tests

    [Fact]
    public void GetByTrackingId_WithValidTrackingId_ReturnsOkResultWithShipment()
    {
        var trackingId = "DHL123456";
        var shipmentDto = new ShipmentResponseDto(
            TrackingId: trackingId,
            Origin: "New York",
            Destination: "Los Angeles",
            EstimatedDeliveryDate: DateTime.UtcNow.AddDays(5),
            CurrentStatus: ShipmentStatus.Created,
            Milestones: new List<ShipmentMilestoneDto>
            {
                new(ShipmentStatus.Created, "New York", DateTime.UtcNow)
            }
        );

        _mockShipmentService
            .Setup(s => s.GetShipmentByTrackingId(trackingId))
            .Returns(shipmentDto);

        var result = _controller.GetByTrackingId(trackingId);

        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().Be(shipmentDto);
        _mockShipmentService.Verify(s => s.GetShipmentByTrackingId(trackingId), Times.Once);
    }

    [Fact]
    public void GetByTrackingId_WithInvalidTrackingId_ThrowsKeyNotFoundException()
    {
        var trackingId = "INVALID";
        _mockShipmentService
            .Setup(s => s.GetShipmentByTrackingId(trackingId))
            .Throws(new KeyNotFoundException("Shipment not found"));

        var action = () => _controller.GetByTrackingId(trackingId);
        action.Should().Throw<KeyNotFoundException>();
    }

    [Fact]
    public void GetByTrackingId_CallsServiceWithCorrectTrackingId()
    {
        var trackingId = "DHL999999";
        var shipmentDto = new ShipmentResponseDto(
            TrackingId: trackingId,
            Origin: "London",
            Destination: "Paris",
            EstimatedDeliveryDate: DateTime.UtcNow.AddDays(3),
            CurrentStatus: ShipmentStatus.InTransit,
            Milestones: new List<ShipmentMilestoneDto>()
        );

        _mockShipmentService
            .Setup(s => s.GetShipmentByTrackingId(trackingId))
            .Returns(shipmentDto);

        _controller.GetByTrackingId(trackingId);

        _mockShipmentService.Verify(s => s.GetShipmentByTrackingId(trackingId), Times.Once);
    }

    #endregion

    #region CreateShipment Tests

    [Fact]
    public void CreateShipment_WithValidData_ReturnsCreatedAtAction()
    {
        var createDto = new CreateShipmentDto(
            Origin: "Berlin",
            Destination: "Amsterdam",
            EstimatedDeliveryDate: DateTime.UtcNow.AddDays(2)
        );

        var shipmentDto = new ShipmentResponseDto(
            TrackingId: "DHL123456",
            Origin: createDto.Origin,
            Destination: createDto.Destination,
            EstimatedDeliveryDate: createDto.EstimatedDeliveryDate,
            CurrentStatus: ShipmentStatus.Created,
            Milestones: new List<ShipmentMilestoneDto>()
        );

        _mockShipmentService
            .Setup(s => s.CreateShipment(createDto))
            .Returns(shipmentDto);

        var result = _controller.CreateShipment(createDto);

        var createdResult = result.Result as CreatedAtActionResult;
        createdResult.Should().NotBeNull();
        createdResult.StatusCode.Should().Be(201);
        createdResult.Value.Should().Be(shipmentDto);
        _mockShipmentService.Verify(s => s.CreateShipment(createDto), Times.Once);
    }

    [Fact]
    public void CreateShipment_ReturnsCorrectRouteValues()
    {
        var createDto = new CreateShipmentDto(
            Origin: "Tokyo",
            Destination: "Singapore",
            EstimatedDeliveryDate: DateTime.UtcNow.AddDays(7)
        );

        var trackingId = "DHL654321";
        var shipmentDto = new ShipmentResponseDto(
            TrackingId: trackingId,
            Origin: createDto.Origin,
            Destination: createDto.Destination,
            EstimatedDeliveryDate: createDto.EstimatedDeliveryDate,
            CurrentStatus: ShipmentStatus.Created,
            Milestones: new List<ShipmentMilestoneDto>()
        );

        _mockShipmentService
            .Setup(s => s.CreateShipment(createDto))
            .Returns(shipmentDto);

        var result = _controller.CreateShipment(createDto);

        var createdResult = result.Result as CreatedAtActionResult;
        createdResult.Should().NotBeNull();
        createdResult?.RouteValues.Should().Contain("trackingId", trackingId);
    }

    [Fact]
    public void CreateShipment_CallsServiceWithCorrectDto()
    {
        var createDto = new CreateShipmentDto(
            Origin: "Dubai",
            Destination: "Mumbai",
            EstimatedDeliveryDate: DateTime.UtcNow.AddDays(4)
        );

        var shipmentDto = new ShipmentResponseDto(
            TrackingId: "DHL111111",
            Origin: createDto.Origin,
            Destination: createDto.Destination,
            EstimatedDeliveryDate: createDto.EstimatedDeliveryDate,
            CurrentStatus: ShipmentStatus.Created,
            Milestones: new List<ShipmentMilestoneDto>()
        );

        _mockShipmentService
            .Setup(s => s.CreateShipment(createDto))
            .Returns(shipmentDto);

        _controller.CreateShipment(createDto);

        _mockShipmentService.Verify(s => s.CreateShipment(createDto), Times.Once);
    }

    #endregion

    #region UpdateStatus Tests

    [Fact]
    public void UpdateStatus_WithValidData_ReturnsOkResultWithUpdatedShipment()
    {
        var trackingId = "DHL123456";
        var updateDto = new UpdateShipmentStatusDto
        {
            Status = ShipmentStatus.InTransit,
            Location = "Airport"
        };

        var shipmentDto = new ShipmentResponseDto(
            TrackingId: trackingId,
            Origin: "London",
            Destination: "Paris",
            EstimatedDeliveryDate: DateTime.UtcNow.AddDays(3),
            CurrentStatus: ShipmentStatus.InTransit,
            Milestones: new List<ShipmentMilestoneDto>
            {
                new(ShipmentStatus.Created, "London", DateTime.UtcNow),
                new(ShipmentStatus.InTransit, "Airport", DateTime.UtcNow)
            }
        );

        _mockShipmentService
            .Setup(s => s.UpdateShipmentStatus(trackingId, updateDto))
            .Returns(shipmentDto);

        var result = _controller.UpdateStatus(trackingId, updateDto);

        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().Be(shipmentDto);
        _mockShipmentService.Verify(s => s.UpdateShipmentStatus(trackingId, updateDto), Times.Once);
    }

    [Fact]
    public void UpdateStatus_WithInvalidTrackingId_ThrowsKeyNotFoundException()
    {
        var trackingId = "INVALID";
        var updateDto = new UpdateShipmentStatusDto
        {
            Status = ShipmentStatus.InTransit,
            Location = "Somewhere"
        };

        _mockShipmentService
            .Setup(s => s.UpdateShipmentStatus(trackingId, updateDto))
            .Throws(new KeyNotFoundException("Shipment not found"));

        var action = () => _controller.UpdateStatus(trackingId, updateDto);
        action.Should().Throw<KeyNotFoundException>();
    }

    [Fact]
    public void UpdateStatus_CallsServiceWithCorrectParameters()
    {
        var trackingId = "DHL999999";
        var updateDto = new UpdateShipmentStatusDto
        {
            Status = ShipmentStatus.OutForDelivery,
            Location = "Local Hub"
        };

        var shipmentDto = new ShipmentResponseDto(
            TrackingId: trackingId,
            Origin: "Cairo",
            Destination: "Giza",
            EstimatedDeliveryDate: DateTime.UtcNow.AddDays(1),
            CurrentStatus: ShipmentStatus.OutForDelivery,
            Milestones: new List<ShipmentMilestoneDto>()
        );

        _mockShipmentService
            .Setup(s => s.UpdateShipmentStatus(trackingId, updateDto))
            .Returns(shipmentDto);

        _controller.UpdateStatus(trackingId, updateDto);

        _mockShipmentService.Verify(s => s.UpdateShipmentStatus(trackingId, updateDto), Times.Once);
    }

    [Fact]
    public void UpdateStatus_WithDifferentStatuses_ReturnsCorrectStatus()
    {
        var trackingId = "DHL555555";
        var statuses = new[] { ShipmentStatus.InTransit, ShipmentStatus.OutForDelivery, ShipmentStatus.Delivered };
        var updateDtos = statuses.Select(s => new UpdateShipmentStatusDto
        {
            Status = s,
            Location = "Some Location"
        }).ToList();

        for (int i = 0; i < updateDtos.Count; i++)
        {
            var shipmentDto = new ShipmentResponseDto(
                TrackingId: trackingId,
                Origin: "Origin",
                Destination: "Destination",
                EstimatedDeliveryDate: DateTime.UtcNow,
                CurrentStatus: statuses[i],
                Milestones: new List<ShipmentMilestoneDto>()
            );

            _mockShipmentService
                .Setup(s => s.UpdateShipmentStatus(trackingId, updateDtos[i]))
                .Returns(shipmentDto);
        }

        for (int i = 0; i < updateDtos.Count; i++)
        {
            var result = _controller.UpdateStatus(trackingId, updateDtos[i]);
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            ((ShipmentResponseDto)okResult.Value!).CurrentStatus.Should().Be(statuses[i]);
        }
    }

    #endregion

    #region GetAllShipments Tests

    [Fact]
    public void GetAllShipments_WithNoShipments_ReturnsOkResultWithEmptyList()
    {
        _mockShipmentService
            .Setup(s => s.GetAllShipments())
            .Returns(new List<ShipmentResponseDto>());

        var result = _controller.GetAllShipments();

        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(200);
        var shipments = (okResult.Value as IEnumerable<ShipmentResponseDto>)?.ToList();
        shipments.Should().BeEmpty();
        _mockShipmentService.Verify(s => s.GetAllShipments(), Times.Once);
    }

    [Fact]
    public void GetAllShipments_WithMultipleShipments_ReturnsOkResultWithAllShipments()
    {
        var shipmentsDto = new List<ShipmentResponseDto>
        {
            new(
                TrackingId: "DHL111111",
                Origin: "New York",
                Destination: "Los Angeles",
                EstimatedDeliveryDate: DateTime.UtcNow.AddDays(5),
                CurrentStatus: ShipmentStatus.Created,
                Milestones: new List<ShipmentMilestoneDto>
                {
                    new(ShipmentStatus.Created, "New York", DateTime.UtcNow)
                }
            ),
            new(
                TrackingId: "DHL222222",
                Origin: "London",
                Destination: "Paris",
                EstimatedDeliveryDate: DateTime.UtcNow.AddDays(3),
                CurrentStatus: ShipmentStatus.InTransit,
                Milestones: new List<ShipmentMilestoneDto>
                {
                    new(ShipmentStatus.Created, "London", DateTime.UtcNow.AddHours(-2)),
                    new(ShipmentStatus.InTransit, "English Channel", DateTime.UtcNow)
                }
            ),
            new(
                TrackingId: "DHL333333",
                Origin: "Tokyo",
                Destination: "Singapore",
                EstimatedDeliveryDate: DateTime.UtcNow.AddDays(7),
                CurrentStatus: ShipmentStatus.PickedUp,
                Milestones: new List<ShipmentMilestoneDto>
                {
                    new(ShipmentStatus.Created, "Tokyo", DateTime.UtcNow.AddHours(-4)),
                    new(ShipmentStatus.PickedUp, "Tokyo Airport", DateTime.UtcNow)
                }
            )
        };

        _mockShipmentService
            .Setup(s => s.GetAllShipments())
            .Returns(shipmentsDto);

        var result = _controller.GetAllShipments();

        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(200);
        var returnedShipments = (okResult.Value as IEnumerable<ShipmentResponseDto>)?.ToList();
        returnedShipments.Should().HaveCount(3);
        returnedShipments![0].TrackingId.Should().Be("DHL111111");
        returnedShipments[1].TrackingId.Should().Be("DHL222222");
        returnedShipments[2].TrackingId.Should().Be("DHL333333");
        _mockShipmentService.Verify(s => s.GetAllShipments(), Times.Once);
    }

    [Fact]
    public void GetAllShipments_CallsServiceMethod()
    {
        _mockShipmentService
            .Setup(s => s.GetAllShipments())
            .Returns(new List<ShipmentResponseDto>());

        _controller.GetAllShipments();

        _mockShipmentService.Verify(s => s.GetAllShipments(), Times.Once);
    }

    #endregion
}
