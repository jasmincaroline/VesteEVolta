using System;
using System.Collections.Generic;

namespace VesteEVolta.Models;

public partial class TbCategory
{
    public Guid CategoryId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<TbClothing> Clothings { get; set; } = new List<TbClothing>();
}
