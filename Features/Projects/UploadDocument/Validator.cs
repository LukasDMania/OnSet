using FluentValidation;

namespace OnSet.Features.Projects.UploadDocument
{
    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.ProjectId)
                .GreaterThan(0);

            RuleFor(x => x.File)
                .NotNull().WithMessage("A file is required.");

            RuleFor(x => x.File!.Length)
                .GreaterThan(0)
                .When(x => x.File != null)
                .WithMessage("The file is empty.");
        }
    }
}

