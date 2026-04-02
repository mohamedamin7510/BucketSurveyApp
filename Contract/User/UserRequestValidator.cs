namespace BucketSurvey.Api.Contract.User;

public class UserRequestValidator : AbstractValidator<UserRequest>
{

    public UserRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .Length(3, 100)
            .NotEmpty()
            .WithMessage("First name is required");


        RuleFor(x => x.LastName)
            .Length(3, 100)
            .NotEmpty()
            .WithMessage("Lasr name is required");

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("{Property}: Should be in email format!");
 
        RuleFor(x => x.Password)
           .Matches(RegexPattern.Password)
            .WithMessage("The password must match contain capital, small letters, numbers, non-alphanumeric and at least 8 characters");



        RuleFor(x => x.Roles)
         .NotEmpty()
         .WithMessage("Roles should have value")
         .Must(x => x.Count() == x.Distinct().Count())
         .WithMessage("Roles should not have duplicated values")
         .When(x => x.Roles is not null);

    }

}
