namespace TaskManagement.Core.Interfaces.Model
{
    public interface IAudit
    {
        string CreatedBy { get; set; }

        string UpdatedBy { get; set; }
    }
}
