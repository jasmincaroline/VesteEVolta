using System;
using System.Collections.Generic;

namespace VesteEVolta.Models;

public partial class TbRating
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid RentId { get; set; }

    public Guid ClothingId { get; set; }

    public int Rating { get; set; }

    public string? Comment { get; set; }

    public DateOnly Date { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual TbClothing Clothing { get; set; } = null!;

    public virtual TbRental Rent { get; set; } = null!;

    public virtual TbUser User { get; set; } = null!;
}
