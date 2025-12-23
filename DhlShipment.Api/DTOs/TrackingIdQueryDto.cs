using System.ComponentModel.DataAnnotations;

namespace DhlShipment.Api.DTOs;

public record TrackingIdQueryDto(
    [Required(ErrorMessage = "Tracking ID is required")]
    [RegularExpression(@"^DHL\d{6}$", ErrorMessage = "Invalid Tracking ID format. Expected format: DHL + 6 digits (e.g., DHL905514)")]
    string TrackingId
);
