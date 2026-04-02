
using BucketSurvey.Api.Helpers;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace BucketSurvey.Api.Services;

public class PollNotificationService(
    UserManager<ApplicationUser> userManager ,
    ApplicationContext context ,
    IHttpContextAccessor contextAccessor,
    IEmailSender emailSender
    ) : IPollNotificationService
{
    private readonly UserManager<ApplicationUser> _UserManager = userManager;
    private readonly ApplicationContext _Context = context;
    private readonly IHttpContextAccessor _HttpContextAccessor = contextAccessor;
    private readonly IEmailSender _EmailSender = emailSender;

    public async Task SendNewPollNotification(int? pollid)
    {
        List<Poll> polls = []; 

        if (pollid.HasValue)
        {
            var poll = await _Context.polls
                .SingleOrDefaultAsync(x => x.Id == pollid && x.IsPublished);

            polls.Add(poll!);

        }
        else
        {
            polls = await _Context.polls.
                   Where(x => x.IsPublished && x.StartsAt == DateOnly.FromDateTime(DateTime.UtcNow))
                   .AsNoTracking()
                   .ToListAsync();
        }

       
        var users = await _UserManager.Users.Where(x=>x.Email != null).ToListAsync();

        var origin = _HttpContextAccessor.HttpContext?.Request.Headers.Origin;


        users.ForEach(x=> polls.ForEach(async y =>
         {
             var placeholders = new Dictionary<string, string>()
             {
                 {"{{name}}", x.FirstName + " " + x.LastName},
                 {"{{pollTill}}", y.Title},
                 {"{{endDate}}", y.EndsAt.ToString() },
                 {"{{url}}", $"https://{origin}//polls//start/{y.Id}"},
             };

            var body = EmailBodyBuilder.GenerateEmailBody("PollNotification", placeholders);
           
            await  _EmailSender.SendEmailAsync(x.Email! , $"📣 Survey Basket: New Poll- {y.Title}" ,body);
         }          
        ));


    }
}
