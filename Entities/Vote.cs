namespace BucketSurvey.Api.Entities;

public class Vote
{
    public int Id { get; set; }
    public int pollId { get; set; }
    public string userId { get; set; } = string.Empty;
    public DateTime SubmitAt{ get; set; } = DateTime.Now;
    public ICollection<VoteAnswers> VoteAnswers { get; set; } = []; 


    public virtual Poll Poll { get; set; } = default!;
    public virtual ApplicationUser User { get; set; } = default!;

}
