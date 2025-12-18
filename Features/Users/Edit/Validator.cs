using FluentValidation;

namespace OnSet.Features.Users.Edit
{
    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.FirstName).NotEmpty().MaximumLength(50);
            RuleFor(x => x.LastName).NotEmpty().MaximumLength(50);

            RuleFor(x => x.YearsExperience)
                .InclusiveBetween(0, 80)
                .When(x => x.YearsExperience.HasValue);

            RuleFor(x => x.Bio)
                .MaximumLength(500);

            RuleFor(x => x.AvatarUrl)
                .Must(uri => string.IsNullOrEmpty(uri) || Uri.TryCreate(uri, UriKind.Absolute, out _))
                .WithMessage("Invalid URL format for Avatar.");

            RuleFor(x => x.NextAvailableDate)
                .GreaterThan(DateTime.Today)
                .When(x => x.IsAvailableForBooking && x.NextAvailableDate.HasValue)
                .WithMessage("Next available date must be in the future.");
        }
    }
}
