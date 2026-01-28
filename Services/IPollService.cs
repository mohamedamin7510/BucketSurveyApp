namespace BucketSurvey.Api.PollServices.Processing;

public interface IPollService
{
    public Task <Result<IEnumerable<PollResponse>> > GetAllAsync(CancellationToken cancellationToken = default!);
    public Task <Result<IEnumerable<PollResponse>> > GetCurrentAsync(CancellationToken cancellationToken = default!);

    public Task<Result<PollResponse>> GetAsync(int Id, CancellationToken cancellationToken);
    public Task<Result<PollResponse>> AddAsync(PollRequest pollReq, CancellationToken cancellationToken = default);
    public Task<Result> UpdateAsync(int Id, PollRequest pollrequest , CancellationToken cancellationToken = default);
    public Task<Result> DeleteAsync(int Id, CancellationToken cancellationToken = default);
    public Task<Result> ToggleIsPublishedStatus(int id, CancellationToken cancellationToken);
}
