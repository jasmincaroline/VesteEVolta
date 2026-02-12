using System;
using System.Collections.Generic;

namespace VesteEVolta.Models;

public partial class TbCustomer
{
    public Guid UserId { get; set; }

    public virtual TbUser User { get; set; } = null!;
}
