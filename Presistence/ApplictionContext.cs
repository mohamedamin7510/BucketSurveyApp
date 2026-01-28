namespace BucketSurvey.Api.Presistence;


public class ApplicationContext(DbContextOptions<ApplicationContext> options, IHttpContextAccessor httpContextAccessor) :
    IdentityDbContext<ApplicationUser>(options)
{
    private readonly IHttpContextAccessor _HttpContextAccessor = httpContextAccessor;

    public DbSet<Answer> Answers { get; set; }
    public DbSet<Poll> polls { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<ApplicationUser> Users { get; set; }
    public DbSet<Vote> Votes { get; set; }
    public DbSet<VoteAnswers>  voteAnswers { get; set; }



    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());


        var CascadeFks = modelBuilder.Model
            .GetEntityTypes().SelectMany(f => f.GetForeignKeys()).Where(x => x.DeleteBehavior == DeleteBehavior.Cascade && !x.IsOwnership);

        foreach (var FK in CascadeFks)
        {
            FK.DeleteBehavior = DeleteBehavior.Restrict;
        }



        base.OnModelCreating(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var claimId = _HttpContextAccessor.HttpContext!.User.GetUserId();

        var EntityEntries = ChangeTracker.Entries<AuditibleLogging>();

        foreach (var entityEntry in EntityEntries)
        {
            if (entityEntry.State == EntityState.Added)
            {
                entityEntry.Property(x => x.CreatedById).CurrentValue  = claimId!;
            }
            else if (entityEntry.State == EntityState.Modified)
            {
                entityEntry.Property(x => x.UpdatedById).CurrentValue = claimId;
                entityEntry.Property(x => x.UpdatedAt).CurrentValue = DateTime.UtcNow;

            }

        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
