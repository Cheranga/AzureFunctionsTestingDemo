using FluentValidation;
using FunkyCustomerCare.DTO;

namespace FunkyCustomerCare.Validators
{
    public class CategorizeCustomerRequestValidator : ModelValidatorBase<CategorizeCustomerRequest>
    {
        public CategorizeCustomerRequestValidator()
        {
            CascadeMode = CascadeMode.Stop;

            RuleFor(x => x.Id).NotNull().NotEmpty();
            RuleFor(x => x.AmountSpent).GreaterThan(0);
        }
    }
}