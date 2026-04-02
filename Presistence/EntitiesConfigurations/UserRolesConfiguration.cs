namespace BucketSurvey.Api.Presistence.EntitiesConfigurations;

public class UserRolesConfiguration : IEntityTypeConfiguration<IdentityUserRole<string>>
{

    public void Configure(EntityTypeBuilder<IdentityUserRole<string>> builder)
    {

        builder.HasData(
                new IdentityUserRole<string>
                {
                    RoleId = DefaultRoles.AdminRoleId,
                    UserId = DefaultUsers.AdminId
                }
            );

    }
}
