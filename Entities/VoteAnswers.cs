using System.ComponentModel.DataAnnotations.Schema;

namespace BucketSurvey.Api.Entities;

public class VoteAnswers
{
    public int Id { get; set; }
    public int VoteId { get; set; }
    public int QuesitonId { get; set; }
    public int AnswerId { get; set; }


    public Vote Vote { get; set; } = default!;
    [ForeignKey("QuesitonId")]
    public Question question { get; set; } = default!;
    public Answer Answer { get; set; } = default!;

}
