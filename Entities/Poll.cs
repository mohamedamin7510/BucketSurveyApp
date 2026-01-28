using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace BucketSurvey.Api.Entities;

public class Poll: AuditibleLogging
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Summary { get; set; }

    public bool IsPublished { get; set; }

    public DateOnly StartsAt { get; set; }

    public DateOnly EndsAt { get; set; }




   public  virtual ICollection<Question> Questions { get; set; } = []; 
   public  virtual ICollection<Vote> Votes { get; set; } = []; 

}
