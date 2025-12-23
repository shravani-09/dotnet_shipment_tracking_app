using System.ComponentModel.DataAnnotations;
using DhlShipment.Api.Models;

namespace DhlShipment.Api.DTOs;

public class UpdateShipmentStatusDto
{
    [Required(ErrorMessage = "Status is required")]
    public ShipmentStatus Status { get; set; }
    
    [Required(ErrorMessage = "Location is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Location must be between 2 and 100 characters")]
    [RegularExpression(@"^[a-zA-Z\s\-]*$", ErrorMessage = "Location must contain only letters, spaces, and hyphens")]
    public string Location { get; set; } = string.Empty;
}
