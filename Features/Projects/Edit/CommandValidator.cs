using FluentValidation;

namespace OnSet.Features.Projects.Edit
{
    public class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {
            RuleFor(p => p.Name).NotNull().Length(3, 50);
            RuleFor(p => p.Description).NotNull();
        }
    }
}
