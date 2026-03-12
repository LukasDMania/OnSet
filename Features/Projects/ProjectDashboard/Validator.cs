using FluentValidation;

namespace OnSet.Features.Projects.ProjectDashboard
{
    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(p => p.Id).NotNull();
        }
    }
}

