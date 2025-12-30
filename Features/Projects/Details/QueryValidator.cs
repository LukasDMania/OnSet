using FluentValidation;

namespace OnSet.Features.Projects.Details
{
    public class QueryValidator : AbstractValidator<Query>
    {
        public QueryValidator()
        {
            RuleFor(p => p.Id).NotNull();
        }
    }
}
