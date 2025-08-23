using FluentValidation;
using HallBookingBhatPara.Domain.DTO.Admin;
using HallBookingBhatPara.Domain.DTO.HallBooking;
using HallBookingBhatPara.Domain.DTO.User;

namespace HallBookingBhatPara.Model.Validator
{
    public class GenericValidator<T> : AbstractValidator<T>
    {
        protected bool BeTrimmed(string? value)
        {
            return value == value?.Trim();
        }

        protected bool BeAValidDate(string date)
        {
            return DateTime.TryParse(date, out _);
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

    #region :: Admin
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
            RuleFor(x => x.PaymentTypeId)
                .NotNull().WithMessage("PaymentType is required.")
                .GreaterThan(0).WithMessage("PaymentType must be greater than 0.");

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

    public class UpdateHallAvailableValidator : GenericValidator<UpdateHallAvailableDTO>
    {
        public UpdateHallAvailableValidator()
        {
            RuleFor(x => x.HallAvailId)
                .NotNull().WithMessage("Id is required.")
                .GreaterThan(0).WithMessage("Id must be greater than 0.");

            RuleFor(x => x.CategoryId)
                .NotNull().WithMessage("CategoryId is required.")
                .GreaterThan(0).WithMessage("CategoryId must be greater than 0.");

            RuleFor(x => x.SubcategoryId)
                .NotNull().WithMessage("SubcategoryId is required.")
                .GreaterThan(0).WithMessage("SubcategoryId must be greater than 0.");

            RuleFor(x => x.PaymentTypeId)
                .NotNull().WithMessage("PaymentType is required.")
                .GreaterThan(0).WithMessage("PaymentType must be greater than 0.");

            RuleFor(x => x.AvailableFrom)
                .NotNull().WithMessage("AvailableFrom is required.");
            RuleFor(x => x.AvailableTo)
                .NotNull().WithMessage("AvailableTo is required.");

            RuleFor(x => x.ProposedRate)
                .NotNull().WithMessage("ProposedRate is required.")
                .GreaterThan(0).WithMessage("ProposedRate must be greater than 0.");

        }
    }
    #endregion

    #region :: User Booking
    public class InsertConfirmBookHallValidator : GenericValidator<InsertUserConfirmhallDTO>
    {
        public InsertConfirmBookHallValidator()
        {
            RuleFor(x => x.catId)
             .GreaterThan(0).WithMessage("Category is required.");

            RuleFor(x => x.hallId)
                .GreaterThan(0).WithMessage("Hall is required.");

            RuleFor(x => x.hallAvailId)
                .GreaterThan(0).WithMessage("Hall availability is required.");

            RuleFor(x => x.rate)
                .GreaterThan(0).WithMessage("Rate must be greater than 0.");

            RuleFor(x => x.securityMoney)
                .GreaterThanOrEqualTo(0).WithMessage("Security money cannot be negative.");

            RuleFor(x => x.fullName)
                .NotEmpty().WithMessage("Full name is required.")
                .MaximumLength(100).WithMessage("Full name cannot exceed 100 characters.");

            RuleFor(x => x.phone)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(@"^[0-9]{10}$").WithMessage("Phone number must be a valid 10-digit number.");

            RuleFor(x => x.alternatePhone)
                .Matches(@"^[0-9]{10}$").When(x => !string.IsNullOrEmpty(x.alternatePhone))
                .WithMessage("Alternate phone number must be a valid 10-digit number.");

            RuleFor(x => x.email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.")
                .MaximumLength(150).WithMessage("Email cannot exceed 150 characters.");

            RuleFor(x => x.address)
                .NotEmpty().WithMessage("Address is required.")
                .MaximumLength(250).WithMessage("Address cannot exceed 250 characters.");

            RuleFor(x => x.eventType)
                .GreaterThan(0).WithMessage("Event type is required.");

            RuleFor(x => x.eventDate)
                .NotEmpty().WithMessage("Event date is required.")
                .Must(BeAValidDate).WithMessage("Event date must be a valid date (yyyy-MM-dd).");

        }
    }
    #endregion
}
