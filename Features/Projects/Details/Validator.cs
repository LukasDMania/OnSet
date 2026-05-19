using FluentValidation;

namespace OnSet.Features.Projects.Details
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
