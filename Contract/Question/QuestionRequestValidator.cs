namespace BucketSurvey.Api.Contract.Question;

public class QuestionRequestValidator:AbstractValidator<QuestionRequest>
{
    public QuestionRequestValidator()
    {
        RuleFor(x => x.Content).NotEmpty().
            Length(3, 1000);

        RuleFor(x=>x).NotEmpty();

        RuleFor(x => x.Answers)
            .Must(x => x.Count > 1)
            .When(x=>x.Answers != null)
            .WithMessage("The question has at least two answers");


        RuleFor(x => x.Answers)
            .Must(x => x.Count == x.Distinct().Count())
            .When(x => x.Answers != null)
            .WithState(x => StatusCodes.Status409Conflict)
            .WithMessage("You shouldn't repeate the answers for the same question");
    }


}
