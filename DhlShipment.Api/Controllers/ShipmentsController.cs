using DhlShipment.Api.DTOs;
using DhlShipment.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DhlShipment.Api.Controllers;

[ApiController]
[Route("api/shipment")]
public class ShipmentsController : ControllerBase
{
    private readonly IShipmentService _shipmentService;

    public ShipmentsController(IShipmentService shipmentService)
    {
        _shipmentService = shipmentService;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("view")]
    public ActionResult<IEnumerable<ShipmentResponseDto>> GetAllShipments()
    {
        var shipments = _shipmentService.GetAllShipments();
        return Ok(shipments);
    }

    [AllowAnonymous]
    [HttpGet("{trackingId}")]
    public ActionResult<ShipmentResponseDto> GetByTrackingId(string trackingId)
    {
        var shipment = _shipmentService.GetShipmentByTrackingId(trackingId);
        return Ok(shipment);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("create")]
    public ActionResult<ShipmentResponseDto> CreateShipment([FromBody] CreateShipmentDto dto)
    {
        var shipment = _shipmentService.CreateShipment(dto);

        return CreatedAtAction(
            nameof(GetByTrackingId),
            new { trackingId = shipment.TrackingId },
            shipment
        );
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{trackingId}/status")]
    public ActionResult<ShipmentResponseDto> UpdateStatus(
        string trackingId,
        [FromBody] UpdateShipmentStatusDto dto)
    {
        var shipment = _shipmentService.UpdateShipmentStatus(trackingId, dto);
        return Ok(shipment);
    }
}
