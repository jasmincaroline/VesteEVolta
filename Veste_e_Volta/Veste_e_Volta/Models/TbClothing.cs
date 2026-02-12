using System;
using System.Collections.Generic;

namespace VesteEVolta.Models;

public partial class TbClothing
{
    public Guid Id { get; set; }

    public string Description { get; set; } = null!;

    public decimal RentPrice { get; set; }

    public string AvailabilityStatus { get; set; } = null!;

    public Guid OwnerId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual TbOwner Owner { get; set; } = null!;

    public virtual ICollection<TbRating> TbRatings { get; set; } = new List<TbRating>();

    public virtual ICollection<TbRental> TbRentals { get; set; } = new List<TbRental>();

    public virtual ICollection<TbReport> TbReports { get; set; } = new List<TbReport>();

    public virtual ICollection<TbCategory> Categories { get; set; } = new List<TbCategory>();
}
