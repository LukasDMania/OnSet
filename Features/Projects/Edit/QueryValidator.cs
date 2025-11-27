using FluentValidation;

namespace OnSet.Features.Projects.Edit
{
    public class QueryValidator : AbstractValidator<Query>
    {
        public QueryValidator()
        {
            RuleFor(p => p.Id).NotNull();
        }
    }
}
