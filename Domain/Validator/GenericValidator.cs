using FluentValidation;

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
}
