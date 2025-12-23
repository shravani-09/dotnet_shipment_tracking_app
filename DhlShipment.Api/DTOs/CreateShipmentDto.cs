using System.ComponentModel.DataAnnotations;
using DhlShipment.Api.Validation;

namespace DhlShipment.Api.DTOs;

public record CreateShipmentDto(
    [Required(ErrorMessage = "Origin is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Origin must be between 2 and 100 characters")]
    [RegularExpression(@"^[a-zA-Z\s\-]*$", ErrorMessage = "Origin must contain only letters, spaces, and hyphens")]
    string Origin,
    
    [Required(ErrorMessage = "Destination is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Destination must be between 2 and 100 characters")]
    [RegularExpression(@"^[a-zA-Z\s\-]*$", ErrorMessage = "Destination must contain only letters, spaces, and hyphens")]
    string Destination,
    
    [Required(ErrorMessage = "Estimated Delivery Date is required")]
    [FutureDate(ErrorMessage = "Estimated Delivery Date must be in the future")]
    DateTime EstimatedDeliveryDate
);
