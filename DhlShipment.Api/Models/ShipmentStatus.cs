namespace DhlShipment.Api.Models;

public enum ShipmentStatus
{
    Created = 0,
    PickedUp = 1,
    InTransit = 2,
    ArrivedAtFacility = 3,
    OutForDelivery = 4,
    Delivered = 5,
    Delayed = 6,
    Exception = 7
}
