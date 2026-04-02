
namespace BucketSurvey.Api.Presistence.EntitiesConfigurations;

public class VoteConfiguration : IEntityTypeConfiguration<VotePerQuestionResponse>
{
    public void Configure(EntityTypeBuilder<VotePerQuestionResponse> builder)
    {

        builder.HasIndex(x => new { x.pollId, x.Id}).IsUnique();

    }
}
