using System.ComponentModel.DataAnnotations;
using DhlShipment.Api.Models;

namespace DhlShipment.Api.DTOs;

public record ShipmentResponseDto(
    [Required(ErrorMessage = "Tracking ID is required")]
    [RegularExpression(@"^DHL\d{6}$", ErrorMessage = "Invalid Tracking ID format. Expected format: DHL + 6 digits (e.g., DHL905514)")]
    string TrackingId,
    
    [Required(ErrorMessage = "Origin is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Origin must be between 2 and 100 characters")]
    string Origin,
    
    [Required(ErrorMessage = "Destination is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Destination must be between 2 and 100 characters")]
    string Destination,
    
    [Required(ErrorMessage = "Estimated Delivery Date is required")]
    DateTime EstimatedDeliveryDate,
    
    ShipmentStatus CurrentStatus,
    
    IEnumerable<ShipmentMilestoneDto> Milestones
);
