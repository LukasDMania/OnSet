using MediatR;
using Microsoft.AspNetCore.Identity;
using OnSet.Domain.Enums;
using OnSet.Domain.Models;
using OnSet.Domain.ValueObjects;
using OnSet.Infrastructure.Results;
using OnSet.Utils;

namespace OnSet.Features.Users.Register
{
    public class CommandHandler : IRequestHandler<Command, Result>
    {
        private readonly UserManager<User> _userManager;

        public CommandHandler(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var firstName = new FirstName(request.FirstName);
            var lastName = new LastName(request.LastName);

            Address? homeAddress = null;

            if (!string.IsNullOrWhiteSpace(request.Street))
            {
                homeAddress = new Address(
                    request.Street!,
                    request.City!,
                    request.Province,
                    request.Country!,
                    request.ZipCode!
                );
            }

            EmergencyContact? emergencyContact = null;

            if (!string.IsNullOrWhiteSpace(request.EmergencyContactName))
            {
                emergencyContact = new EmergencyContact(
                    request.EmergencyContactName!,
                    request.EmergencyContactPhone!
                );
            }

            // Map spoken languages
            var spokenLanguages = request.SpokenLanguages ?? new List<Languages>();

            // Create domain user
            var user = User.Create(
                firstName: firstName,
                lastName: lastName,
                mainOccupationRole: request.MainOccupationRole,
                yearsExperience: request.YearsExperience,
                bio: request.Bio,
                avatarUrl: request.AvatarUrl,
                homeAddress: homeAddress,
                spokenLanguages: spokenLanguages,
                isAvailableForBooking: request.IsAvailableForBooking,
                nextAvailableDate: request.NextAvailableDate,
                emergencyContact: emergencyContact
            );

            //Identity properties
            user.Email = request.Email;
            user.UserName = request.Email;
            user.EmailConfirmed = true;

            // Create in Identity
            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                return Result.Fail(result.Errors is not null ? GetErrors(result.Errors) : new[] { "Unknown error" });
            }
            await _userManager.AddToRoleAsync(user, Roles.StandardUser);

            return Result.Ok();
        }

        private IEnumerable<string> GetErrors(IEnumerable<IdentityError> errors)
        {
            foreach (var e in errors)
            {
                yield return e.Description;
            }
        }
    }
}
