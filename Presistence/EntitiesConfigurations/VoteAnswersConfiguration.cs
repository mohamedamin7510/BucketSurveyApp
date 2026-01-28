
namespace BucketSurvey.Api.Presistence.EntitiesConfigurations;

public class VoteAnswersConfiguration : IEntityTypeConfiguration<VoteAnswers>
{
    public void Configure(EntityTypeBuilder<VoteAnswers> builder)
    {
        builder.HasIndex(x => new { x.VoteId , x.QuesitonId}).IsUnique();
    }
}
