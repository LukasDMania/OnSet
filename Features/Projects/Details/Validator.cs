using FluentValidation;

namespace OnSet.Features.Projects.Details
{
    public class Validator : AbstractValidator<Query>
    {
        public Validator() 
        {
            RuleFor(m => m.Id).NotNull();
        }
    }
}
