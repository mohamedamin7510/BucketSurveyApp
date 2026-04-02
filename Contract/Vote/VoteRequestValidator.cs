namespace BucketSurvey.Api.Contract.Vote;

public class VoteRequestValidator : AbstractValidator<VoteRequest>
{
    public VoteRequestValidator()
    {
        RuleFor(x => x.Answers).NotEmpty()
            .WithMessage("The Body shouldn't be empty");


        RuleForEach(x => x.Answers)
          .SetInheritanceValidator(v => v.Add(
              new VoteAnswerRequestValidator()
            ));

    }
}
