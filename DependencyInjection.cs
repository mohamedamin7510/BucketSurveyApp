using BucketSurvey.Api.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;

namespace BucketSurvey.Api;


public static class DependencyInjection
{

    public static IServiceCollection AddDependencies(this IServiceCollection services,
        IConfiguration config)
    {

        services.AddOpenApi().AddControllers();
               
        var allowedOrigins = config.GetSection("AllowedOrigins").Get<string[]>();

        services.AddCors(opts => opts.AddDefaultPolicy(
            policy => policy.WithOrigins(allowedOrigins!).AllowAnyHeader().AllowAnyMethod()));
          
        services.AddValidatorsFromAssemblyContaining<Program>()
                                        .AddFluentValidationAutoValidation();

        // for exception handler  
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails(); 
        services.AddHybridCache();  
        services.AddOptions<MailSettings>().BindConfiguration(nameof(MailSettings)).ValidateDataAnnotations()
            .ValidateOnStart();   
        services.AddScoped<IQuestionService, QuestionService>();
        services.AddScoped<IResultService, ResultService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IPollService, PollService>();
        services.AddScoped<IVoteService, VoteService>(); 
        services.AddScoped<IEmailSender, EmailService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IPollNotificationService, PollNotificationService>();
        services.AddTransient<IAuthorizationHandler, PermissionAuthorizationHandler>();
        services.AddTransient<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();


        services.AddApplicationContextDependency( config);
        services.AddGlobalConfigurationDependency();
        services._AuthConfig(config);
        services.AddJobs(config);
     

  

        return services;
    }

    private static void AddApplicationContextDependency(this IServiceCollection services, IConfiguration config)
    {
        string connectionstring = config.
           GetConnectionString("APPConn") ?? throw new InvalidOperationException("this Connection string has no founded");


        services.AddDbContext<ApplicationContext>(
            optionsAction: (opts => opts.UseSqlServer(connectionstring)
             .ConfigureWarnings(w =>
             w.Ignore(RelationalEventId.PendingModelChangesWarning))            

            )
            );
    }

    private static void AddGlobalConfigurationDependency(this IServiceCollection services)
    {
        var mappConfig = TypeAdapterConfig.GlobalSettings;
        mappConfig.Scan(Assembly.GetExecutingAssembly()) ;
        services.AddSingleton<IMapper>(new Mapper(mappConfig));
    }
    private static IServiceCollection _AuthConfig(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IJwtProvider, JwtProvider>();

        services.AddIdentity<ApplicationUser, ApplicationRole>()
            .AddEntityFrameworkStores<ApplicationContext>()
            .AddDefaultTokenProviders();

        services.AddAuthorization();

        services.AddOptions<JwtOptions>()
            .BindConfiguration(JwtOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        var JwtSeetings = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>();

        services.AddAuthentication(opts =>
        {
            opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            opts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(opts =>
        {
            opts.SaveToken = true;
            opts.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtSeetings!.SymmetricKey)),
                ValidIssuer = JwtSeetings.Issuer,
                ValidAudience = JwtSeetings.Audience
            };
        });

        return services;
    }
    private static IServiceCollection AddJobs(this IServiceCollection services, IConfiguration configuration)
    {


        // Add Hangfire services.
        services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(configuration.GetConnectionString("HangfireConnection")));

        // Add the processing server as IHostedService
        services.AddHangfireServer();

        // Add framework services.
        services.AddMvc();

        return services; 
    }



}
