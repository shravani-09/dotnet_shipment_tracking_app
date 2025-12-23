using DhlShipment.Api.Exceptions;
using DhlShipment.Api.Models;

namespace DhlShipment.Api.Services;

public interface IShipmentLifecycleValidator
{
    void ValidateTransition(ShipmentStatus currentStatus, ShipmentStatus newStatus);
}

public class ShipmentLifecycleValidator : IShipmentLifecycleValidator
{
    private static readonly Dictionary<ShipmentStatus, HashSet<ShipmentStatus>> ValidTransitions = new()
    {
        {
            ShipmentStatus.Created, new()
            {
                ShipmentStatus.PickedUp
            }
        },
        {
            ShipmentStatus.PickedUp, new()
            {
                ShipmentStatus.InTransit
            }
        },
        {
            ShipmentStatus.InTransit, new()
            {
                ShipmentStatus.ArrivedAtFacility,
                ShipmentStatus.Delayed,
                ShipmentStatus.Exception
            }
        },
        {
            ShipmentStatus.ArrivedAtFacility, new()
            {
                ShipmentStatus.OutForDelivery,
                ShipmentStatus.Delayed,
                ShipmentStatus.Exception
            }
        },
        {
            ShipmentStatus.OutForDelivery, new()
            {
                ShipmentStatus.Delivered,
                ShipmentStatus.Delayed,
                ShipmentStatus.Exception
            }
        },
        {
            ShipmentStatus.Delayed, new()
            {
                ShipmentStatus.InTransit,
                ShipmentStatus.OutForDelivery
            }
        },
        {
            ShipmentStatus.Exception, new()
            {
                ShipmentStatus.InTransit
            }
        }
    };

    public void ValidateTransition(ShipmentStatus currentStatus, ShipmentStatus newStatus)
    {
        if (currentStatus == ShipmentStatus.Delivered)
        {
            throw new InvalidShipmentTransitionException(
                currentStatus.ToString(),
                newStatus.ToString(),
                "Delivered is a terminal state. No further updates are allowed.");
        }

        if (currentStatus == newStatus)
        {
            throw new InvalidShipmentTransitionException(
                currentStatus.ToString(),
                newStatus.ToString(),
                "Status is already set to this value.");
        }

        if (!ValidTransitions.ContainsKey(currentStatus))
        {
            throw new InvalidShipmentTransitionException(
                currentStatus.ToString(),
                newStatus.ToString(),
                $"Current status {currentStatus} is not recognized.");
        }

        if (!ValidTransitions[currentStatus].Contains(newStatus))
        {
            throw new InvalidShipmentTransitionException(
                currentStatus.ToString(),
                newStatus.ToString(),
                $"This transition is not allowed in the shipment lifecycle.");
        }
    }
}
