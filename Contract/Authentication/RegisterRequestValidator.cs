using BucketSurvey.Api.Abstractions.Const;

namespace BucketSurvey.Api.Contract.Authentication;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.password)
            .NotEmpty()
            .Matches(RegexPattern.Password);

        RuleFor(x => x.firstName)
            .MaximumLength(100)
            .NotEmpty();

        RuleFor(x => x.lastName)
            .MaximumLength(100)
            .NotEmpty();

        RuleFor(x => x.phonenumber)
            .NotEmpty();
             

    }



}
