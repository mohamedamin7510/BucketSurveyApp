namespace BucketSurvey.Api.Contract.User;

public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{

    public UpdateUserRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .Length(3 , 100)
            .WithMessage("First name is required");


        RuleFor(x => x.LastName)
            .Length(3, 100)
            .NotEmpty()
            .WithMessage("Lasr name is required");


        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("{Property}: Should be in email format!");
 

        RuleFor(x => x.Roles)
         .NotEmpty()
         .WithMessage("Roles should have value")
         .Must(x => x.Count() == x.Distinct().Count())
         .WithMessage("Roles should not have duplicated values")
         .When(x => x.Roles is not null);

    }

}
