
using BucketSurvey.Api.Entities;
using BucketSurvey.Api.Presistence;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BucketSurvey.Api.PollServices.Processing;

public class PollService(ApplicationContext context) : IPollService
{
    private readonly ApplicationContext _Context = context;

    public async Task<Result<IEnumerable<PollResponse>>> GetAllAsync(CancellationToken cancellationToken =default!)=> 
         Result.Success<IEnumerable<PollResponse>>
        (await _Context.polls.ProjectToType<PollResponse>().AsNoTracking().ToListAsync());

    public  async Task<Result<IEnumerable<PollResponse>>> GetCurrentAsync(CancellationToken cancellationToken = default!)
    {
        var Currentpolls = await _Context.polls.
            Where(x=> x.IsPublished 
            && x.StartsAt <= DateOnly.FromDateTime(DateTime.UtcNow) 
            && x.EndsAt >= DateOnly.FromDateTime(DateTime.UtcNow))
           .ProjectToType<PollResponse>().AsNoTracking().ToListAsync(cancellationToken);

        return Result.Success<IEnumerable<PollResponse>>(Currentpolls); 
    }

    public async Task<Result<PollResponse>> GetAsync(int Id, CancellationToken cancellationToken)
    {
        var poll = await _Context.polls.FindAsync(Id, cancellationToken);

        return poll is  null ?  Result.Faluire<PollResponse>(PollErrors.PollNotFound) : Result.Success(poll.Adapt<PollResponse>())!;
    }

    public async Task<Result<PollResponse>> AddAsync(PollRequest PollReq, CancellationToken cancellationToken = default)
    {
        var poll = PollReq.Adapt<Poll>();
        var IsExisting = await  _Context.polls.AnyAsync(x=>x.Title  == PollReq.Title , cancellationToken);
        if (IsExisting)
        {
            return Result.Faluire<PollResponse>(PollErrors.TittleDuplicated);
        }

        await _Context.polls.AddAsync(poll, cancellationToken);
        await _Context.SaveChangesAsync();
        return Result.Success(poll.Adapt<PollResponse>());
    }

    public async Task<Result> UpdateAsync(int Id, PollRequest pollReq, CancellationToken cancellationToken = default)
    {

        var updatedpoll = await _Context.polls.FindAsync(Id, cancellationToken);

        var IsExisting  = await _Context.polls.AnyAsync(x => x.Title == pollReq.Title && x.Id != Id, cancellationToken);
        if (IsExisting)
        {
            return Result.Faluire<PollResponse>(PollErrors.TittleDuplicated);
        }

        if (updatedpoll is null)
            return Result.Faluire(PollErrors.PollNotFound);

        updatedpoll.Title = pollReq.Title;
        updatedpoll.Summary = pollReq.Summary;
        updatedpoll.StartsAt = pollReq.StartsAt;
        updatedpoll.EndsAt = pollReq.EndsAt;

        await _Context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> DeleteAsync(int Id, CancellationToken cancellationToken = default)
    {
        var DeletedResource = await _Context.polls.FindAsync(Id, cancellationToken);
        if (DeletedResource is null)
            return Result.Faluire(PollErrors.PollNotFound);

        _Context.polls.Remove(DeletedResource);
        await _Context.SaveChangesAsync(cancellationToken);


        return Result.Success();
    }

    public async Task<Result> ToggleIsPublishedStatus(int Id, CancellationToken cancellationToken)
    {
        Poll? updatedpoll = await _Context.polls.FindAsync(Id, cancellationToken);
        if (updatedpoll is null)
            return Result.Faluire(PollErrors.PollNotFound);

        updatedpoll.IsPublished = !updatedpoll.IsPublished;

         await _Context.SaveChangesAsync(cancellationToken);

        return Result.Success();

    }
}
