using FluentValidation;

namespace OnSet.Features.Projects.Details
{
    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(p => p.Id).NotNull();
        }
    }
}
