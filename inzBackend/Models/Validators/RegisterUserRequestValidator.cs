using FluentValidation;
using inzBackend.Models.UserModels;

namespace inzBackend.Models.Validators
{
    public class RegisterUserRequestValidator : AbstractValidator<RegisterUserRequest>
    {
        public RegisterUserRequestValidator(GmitrzakEnglishAcademyDbContext dbContext)
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .WithMessage("Invalid email type");
            RuleFor(x => x.Password)
                .MinimumLength(6)
                .WithMessage("Password's minimum length is 6");
            RuleFor(x => x.Email)
                .Custom((value, context) =>
                {
                    var emailInUse = dbContext.Users.Any(e => e.Email == value);
                    if (emailInUse)
                    {
                        context.AddFailure("Email", "This email is already taken.");
                    }
                });
        }
    }
}
