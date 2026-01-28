using System.ComponentModel.DataAnnotations.Schema;

namespace BucketSurvey.Api.Entities;

public class AuditibleLogging
{
    
    public string CreatedById { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? UpdatedById { get; set; }
    public DateTime? UpdatedAt { get; set; }


    public ApplicationUser CreatedBy { get; set; } = new();
    public ApplicationUser? UpdatedBy { get; set; }= new();

}
