using FluentValidation;
using HallBookingBhatPara.Domain.DTO.Admin;
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
    public class HallAvailableValidator : GenericValidator<InsertHallAvailableDTO>
    {
        public HallAvailableValidator()
        {
            RuleFor(x => x.CategoryId)
                .NotNull().WithMessage("CategoryId is required.")
                .GreaterThan(0).WithMessage("CategoryId must be greater than 0.");

            RuleFor(x => x.SubcategoryId)
                .NotNull().WithMessage("SubcategoryId is required.")
                .GreaterThan(0).WithMessage("SubcategoryId must be greater than 0.");

            RuleFor(x => x.AvailableFrom)
                .NotNull().WithMessage("AvailableFrom is required.")
                .NotEmpty().WithMessage("AvailableFrom is required.")
                .MaximumLength(100).WithMessage("AvailableFrom must not exceed 100 characters.");
            RuleFor(x => x.AvailableTo)
                .NotNull().WithMessage("AvailableTo is required.")
                .NotEmpty().WithMessage("AvailableTo is required.")
                .MaximumLength(100).WithMessage("AvailableTo must not exceed 100 characters.");

            RuleFor(x => x.ProposedRate)
                .NotNull().WithMessage("ProposedRate is required.")
                .GreaterThan(0).WithMessage("ProposedRate must be greater than 0.");

        }
    }

    #endregion
}
