using FluentValidation;

namespace OnSet.Features.Projects.ProjectDashboard
{
    /// <summary>FluentValidation rules for this feature slice.</summary>
    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(p => p.Id).NotNull();
        }
    }
}

