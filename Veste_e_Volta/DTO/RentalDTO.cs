using System.ComponentModel.DataAnnotations;

namespace VesteEVolta.DTO;

public class RentalDTO
{
    [Required(ErrorMessage = "UserId is required")]
    public Guid UserId { get; set; }
    
    [Required(ErrorMessage = "ClothingId is required")]
    public Guid ClothingId { get; set; }
    
    [Required(ErrorMessage = "StartDate is required")]
    public DateOnly StartDate { get; set; }
    
    [Required(ErrorMessage = "EndDate is required")]
    public DateOnly EndDate { get; set; }
    
    public string Status { get; set; } = "pending";
}
