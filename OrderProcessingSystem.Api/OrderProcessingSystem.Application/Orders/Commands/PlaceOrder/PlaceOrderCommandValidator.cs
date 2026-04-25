using FluentValidation;

namespace OrderProcessingSystem.Application.Orders.Commands.PlaceOrder
{
    public class PlaceOrderCommandValidator : AbstractValidator<PlaceOrderCommand>
    {
        public PlaceOrderCommandValidator()
        {
            RuleFor(x => x.CustomerId).NotEmpty();
            RuleFor(x => x.IdempotencyKey).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Items).NotEmpty().WithMessage("Order must contain at least one item");

            RuleForEach(x => x.Items).ChildRules(item =>
            {
                item.RuleFor(i => i.ProductId).NotEmpty();
                item.RuleFor(i => i.Quantity).GreaterThan(0).LessThanOrEqualTo(1000);
            });
        }
    }
}
