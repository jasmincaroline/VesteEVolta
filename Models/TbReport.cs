using System;
using System.Collections.Generic;

namespace VesteEVolta.Models;

public partial class TbReport
{
    public Guid ReportId { get; set; }

    public Guid ReporterId { get; set; }

    public Guid ReportedId { get; set; }

    public Guid ReportedClothingId { get; set; }

    public Guid? RentalId { get; set; }

    public string Type { get; set; } = null!;

    public DateOnly Date { get; set; }

    public string Reason { get; set; } = null!;

    public string? Description { get; set; }

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual TbRental? Rental { get; set; }

    public virtual TbUser Reported { get; set; } = null!;

    public virtual TbClothing ReportedClothing { get; set; } = null!;

    public virtual TbUser Reporter { get; set; } = null!;
}
