using FluentValidation;

namespace OnSet.Features.Projects.Edit
{
    public class Validator : AbstractValidator<Command>
    {
        public Validator() 
        {
            RuleFor(p => p.StartDate).NotNull();
        }
    }
}
