using FluentValidation;
using HallBookingBhatPara.Domain.DTO.User;

namespace HallBookingBhatPara.Model.Validator
{
    public class GenericValidator<T> : AbstractValidator<T>
    {
        protected bool BeTrimmed(string? value)
        {
            return value == value?.Trim();
        }
    }

    public class GenericListValidator<T> : AbstractValidator<List<T>>
    {
        public GenericListValidator(AbstractValidator<T> itemValidator)
        {
            RuleForEach(x => x).SetValidator(itemValidator);
        }
    }

    #region :: User

    public class LoginValidator : GenericValidator<LoginRequestDTO>
    {
        public LoginValidator()
        {
            RuleFor(x => x.Email)
                .NotNull().WithMessage("Email is required.")
                .NotEmpty().WithMessage("Email is required")
                .Must(BeTrimmed).WithMessage("Email must not have leading or trailing whitespace.")
                .EmailAddress().WithMessage("Invalid email format.");
            RuleFor(x => x.Password)
                .NotNull().WithMessage("Password is required.")
                .NotEmpty().WithMessage("Password cannot be empty.")
                .Must(BeTrimmed).WithMessage("Password must not have leading or trailing whitespace.");
        }
    }
    public class RegistrationValidator : GenericValidator<UserRegistrationDto>
    {
        public RegistrationValidator()
        {
            RuleFor(x => x.FirstName)
                .NotNull().WithMessage("FirstName is required.")
                .NotEmpty().WithMessage("FirstName is required")
                .Must(BeTrimmed).WithMessage("FirstName must not have leading or trailing whitespace.");
            RuleFor(x => x.LastName)
                .NotNull().WithMessage("LastName is required.")
                .NotEmpty().WithMessage("LastName is required")
                .Must(BeTrimmed).WithMessage("LastName must not have leading or trailing whitespace.");
            RuleFor(x => x.Email)
                .NotNull().WithMessage("Email is required.")
                .NotEmpty().WithMessage("Email is required")
                .Must(BeTrimmed).WithMessage("Email must not have leading or trailing whitespace.")
                .EmailAddress().WithMessage("Invalid email format.");
            RuleFor(x => x.Phone)
                .NotNull().WithMessage("Phone is required.")
                .NotEmpty().WithMessage("Phone is required")
                .Must(BeTrimmed).WithMessage("Phone must not have leading or trailing whitespace.");
            RuleFor(x => x.Password)
                .NotNull().WithMessage("Password is required.")
                .NotEmpty().WithMessage("Password cannot be empty.")
                .Must(BeTrimmed).WithMessage("Password must not have leading or trailing whitespace.");
        }
    }

    #endregion
}
