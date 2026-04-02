namespace BucketSurvey.Api.Contract.Vote;

public class VoteAnswerRequestValidator : AbstractValidator<VoteAnswerRequest>
{
    public VoteAnswerRequestValidator()
    {
        RuleFor(x => x.QuestionId)
            .NotEmpty();

        RuleFor(x => x.AnswerId)
            .NotEmpty();
    }
}

