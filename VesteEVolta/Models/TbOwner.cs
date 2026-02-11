using System;
using System.Collections.Generic;

namespace VesteEVolta.Models;

public partial class TbOwner
{
    public Guid UserId { get; set; }

    public virtual ICollection<TbClothing> TbClothings { get; set; } = new List<TbClothing>();

    public virtual TbUser User { get; set; } = null!;
}
