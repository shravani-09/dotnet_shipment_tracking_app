using DhlShipment.Api.DTOs;

public interface IShipmentService
{
    IEnumerable<ShipmentResponseDto> GetAllShipments();
    ShipmentResponseDto GetShipmentByTrackingId(string trackingId);
    ShipmentResponseDto CreateShipment(CreateShipmentDto dto);

    ShipmentResponseDto UpdateShipmentStatus(
        string trackingId,
        UpdateShipmentStatusDto dto
    );

    void ClearShipments();
}
