
using BucketSurvey.Api.Abstractions.Const;

namespace BucketSurvey.Api.Presistence.EntitiesConfigurations;

public class ApplictionUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{

    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {

        builder.Property(x => x.FirstName)
            .HasMaxLength(100);

        builder.Property(x => x.LastName)
            .HasMaxLength(100);

        builder.OwnsMany(x => x.RefreshTokens)
        .ToTable("RefreshTokens")
        .WithOwner().HasForeignKey("UserId");

        var PsswordHasher = new PasswordHasher<ApplicationUser>();
        //Default User 
        builder.HasData(
            [
            new ApplicationUser
            {
                Id = DefaultUsers.AdminId,
                FirstName = DefaultUsers.AdminFirstName,
                LastName = DefaultUsers.AdminSecondName,
                Email = DefaultUsers.AdminEmail,
                NormalizedEmail = DefaultUsers.AdminEmail.ToUpper(),
                UserName = DefaultUsers.AdminEmail,
                NormalizedUserName = DefaultUsers.AdminEmail.ToUpper(),
                PasswordHash = PsswordHasher.HashPassword(null!, DefaultUsers.AdminPass),
                SecurityStamp = DefaultUsers.AdminSecurityStamp,
                ConcurrencyStamp = DefaultUsers.AdminConcurrencyStamp,
                PhoneNumber = DefaultUsers.AdminPhoneNumber,
                EmailConfirmed = true,
                PhoneNumberConfirmed= true,
            }
            
            ]
            );

    }
}
