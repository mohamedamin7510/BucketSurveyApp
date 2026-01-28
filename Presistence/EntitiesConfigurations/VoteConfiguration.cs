
namespace BucketSurvey.Api.Presistence.EntitiesConfigurations;

public class VoteConfiguration : IEntityTypeConfiguration<Vote>
{
    public void Configure(EntityTypeBuilder<Vote> builder)
    {

        builder.HasIndex(x => new { x.pollId, x.Id}).IsUnique();

    }
}
