namespace DhlShipment.Api.Exceptions;

public class InvalidShipmentTransitionException : Exception
{
    public InvalidShipmentTransitionException(string message) : base(message)
    {
    }

    public InvalidShipmentTransitionException(
        string currentStatus,
        string newStatus,
        string reason)
        : base($"Cannot transition from {currentStatus} to {newStatus}. {reason}")
    {
    }
}
