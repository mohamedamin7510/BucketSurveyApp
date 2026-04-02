namespace BucketSurvey.Api.Services;

public interface IPollNotificationService
{
    public Task SendNewPollNotification(int? pollid);
}
