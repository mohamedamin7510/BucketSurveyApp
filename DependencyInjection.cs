
using BucketSurvey.Api.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace BucketSurvey.Api;


public static class DependencyInjection
{

    public static IServiceCollection AddDependencies(this IServiceCollection services,
        IConfiguration config)
    {

        services.AddOpenApi()
                .AddControllers();

        var allowedOrigins = config.GetSection("AllowedOrigins").Get<string[]>();

        services.AddCors(opts => opts.AddDefaultPolicy(
            policy => policy.WithOrigins(allowedOrigins!)
            .AllowAnyHeader()
            .AllowAnyMethod()
            ));



        services.AddValidatorsFromAssemblyContaining<Program>()
                                        .AddFluentValidationAutoValidation();


        // for exception handler  
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails(); 


        services.AddApplicationContextDependency( config);
        services.AddGlobalConfigurationDependency();
        services._AuthConfig(config);
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IPollService, PollService>();
        services.AddScoped<IQuestionService, QuestionService>();
        services.AddScoped<IVoteService, VoteService>();
    
        return services;
    }

    private static void AddApplicationContextDependency(this IServiceCollection services, IConfiguration config)
    {
        string connectionstring = config.
           GetConnectionString("APPConn") ?? throw new InvalidOperationException("this Connection string has no founded");


        services.AddDbContext<ApplicationContext>(optionsAction: (opts => opts.UseSqlServer(connectionstring)));
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
         services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationContext>();
        services.AddOptions<JwtOptions>().BindConfiguration(JwtOptions.SectionName) .ValidateDataAnnotations().ValidateOnStart();
        var JwtSeetings = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>();

        services.AddAuthentication(opts =>
        {

            //Default for Authorize attr , to use in Controller or actions without prove it it with bearer 
            opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            opts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
            //this add middleware that know how to read and validate the incoming token from the request.
            .AddJwtBearer(opts =>
            {
                opts.SaveToken = true; 
                opts.TokenValidationParameters = new TokenValidationParameters
                {
                    // the rules that used to validate the Token 
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,

 
                    //the values to check 
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtSeetings!.SymmetricKey)),
                    ValidIssuer = JwtSeetings.Issuer,
                    ValidAudience = JwtSeetings.Audience


                };
            });


        return services;


    }
}
