using BucketSurvey.Api;
using HangfireBasicAuthenticationFilter;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDependencies(builder.Configuration);

builder.Host.UseSerilog((HostBuilderContext,Loggerconfiguration) => 
    Loggerconfiguration.ReadFrom.Configuration(HostBuilderContext.Configuration)
);





var app = builder.Build();

app.MapOpenApi();

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/openapi/v1.json", "BucketSurvey API v1");
});


app.UseHangfireDashboard("/jobs", 
    
    new DashboardOptions()
    {
        Authorization =
        [
            new HangfireCustomBasicAuthenticationFilter()
            {
               User = app.Configuration.GetValue<string>("HangfireAuth:User"),
               Pass = app.Configuration.GetValue<string>("HangfireAuth:Pass")
            }
        ],
        DashboardTitle = "SurveyBasket-Jobs-DashBoard"
    }

    );

var ServiceFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
using var scope = ServiceFactory.CreateScope();
var notificationservice = scope.ServiceProvider.GetRequiredService<IPollNotificationService>();

RecurringJob.AddOrUpdate("PollNotificationService",
    () => notificationservice.SendNewPollNotification(null), Cron.Daily);



app.UseSerilogRequestLogging();

app.UseHttpsRedirection();
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.UseExceptionHandler();

app.Run();

