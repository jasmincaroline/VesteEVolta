namespace VesteEVolta.Models;

public partial class TbUser
{
    public virtual ICollection<TbReport> ReportsReceived { get; set; }
        = new List<TbReport>();

    public virtual ICollection<TbReport> ReportsMade { get; set; }
        = new List<TbReport>();
}
