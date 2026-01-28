
namespace BucketSurvey.Api.Presistence.EntitiesConfigurations;

public class QuestionConfiguration : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        builder.HasIndex(x => new { x.Id, x.Content }).IsUnique();
        builder.Property(x => x.Content).HasMaxLength(1000);
    }
}
