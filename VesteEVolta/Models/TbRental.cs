using System;
using System.Collections.Generic;

namespace VesteEVolta.Models;

public partial class TbRental
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid ClothingId { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public decimal TotalValue { get; set; }

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual TbClothing Clothing { get; set; } = null!;

    public virtual ICollection<TbPayment> TbPayments { get; set; } = new List<TbPayment>();

    public virtual ICollection<TbRating> TbRatings { get; set; } = new List<TbRating>();

    public virtual ICollection<TbReport> TbReports { get; set; } = new List<TbReport>();

    public virtual TbUser User { get; set; } = null!;
}
