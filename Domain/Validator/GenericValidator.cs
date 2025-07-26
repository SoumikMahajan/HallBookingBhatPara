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
                .Must(BeTrimmed).WithMessage("Email must not have leading or trailing whitespace.");
            RuleFor(x => x.Password)
                .NotNull().WithMessage("Password is required.")
                .NotEmpty().WithMessage("Password cannot be empty.")
                .Must(BeTrimmed).WithMessage("Password must not have leading or trailing whitespace.");
        }
    }

    #endregion
}
