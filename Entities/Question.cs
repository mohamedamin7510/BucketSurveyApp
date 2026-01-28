using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace BucketSurvey.Api.Entities;

public class Question : AuditibleLogging
{
    public int Id { get; set; }

    public string Content { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public int pollId { get; set; }

    public virtual Poll poll { get; set; } = default!;

    public virtual ICollection<Answer> Answers { get; set; } = [];

}

