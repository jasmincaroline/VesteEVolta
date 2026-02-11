using System;
using System.Collections.Generic;

namespace VesteEVolta.Models;

public partial class TbUser
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Telephone { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public bool Reported { get; set; }

    public string ProfileType { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual TbCustomer? TbCustomer { get; set; }

    public virtual TbOwner? TbOwner { get; set; }

    public virtual ICollection<TbRating> TbRatings { get; set; } = new List<TbRating>();

    public virtual ICollection<TbRental> TbRentals { get; set; } = new List<TbRental>();

    public virtual ICollection<TbReport> TbReportReporteds { get; set; } = new List<TbReport>();

    public virtual ICollection<TbReport> TbReportReporters { get; set; } = new List<TbReport>();
}
