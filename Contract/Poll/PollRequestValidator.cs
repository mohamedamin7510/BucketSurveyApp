namespace BucketSurvey.Api.Contract.Poll;

public class PollRequestValidator : AbstractValidator<PollRequest>
{

    public PollRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty().Length(3, max: 100);

        RuleFor(x => x.Summary)
            .NotEmpty()
            .MaximumLength(1500);

        RuleFor(x => x.StartsAt)
             .NotEmpty()
             .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.Now))
             .WithMessage("{PropertyName} must be not null and greater than or equal to todaytime. ");

       RuleFor(x => x)
                      .Must(BeEndsDateValidate)
                      .WithName(nameof(PollRequest.EndsAt))
                      .WithMessage("{PropertyName} must be not null and grater than or equal to `StartsAt ` Property");
    
         
    }

    bool BeEndsDateValidate(PollRequest PollRequest) => PollRequest.EndsAt >= PollRequest.StartsAt ;
    
}
