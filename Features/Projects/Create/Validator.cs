using FluentValidation;

namespace OnSet.Features.Projects.Create
{
    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Project name is required.")
                .MaximumLength(150);

            RuleFor(x => x.Description)
                .MaximumLength(1000);
        }
    }
}
