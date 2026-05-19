using MediatR;
using Microsoft.EntityFrameworkCore;
using OnSet.Domain.Enums;
using OnSet.Domain.ValueObjects;
using OnSet.Infrastructure.Data;
using OnSet.Utils;
using OnSet.Application.Exceptions;

namespace OnSet.Features.Users.Edit
{
    /// <summary>MediatR handler for this feature slice.</summary>
    public class CommandHandler : IRequestHandler<Command, Unit>
    {
        private readonly OnSetDbContext _db;
        private readonly ICurrentUserService _currentUserService;

        public CommandHandler(OnSetDbContext db, ICurrentUserService currentUserService)
        {
            _db = db;
            _currentUserService = currentUserService;
        }

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            // Auth guarding  
            if (request.UserId != _currentUserService.UserId)
            {
                throw new ForbiddenAccessException();
            }

            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

            if (user == null)
            {
                throw new NotFoundException("User", request.UserId);
            }

            // Enum conversions  
            ProjectRoles? mainRole = null;
            if (!string.IsNullOrWhiteSpace(request.MainOccupationRole))
            {
                mainRole = EnumConverter.ToEnum<ProjectRoles>(request.MainOccupationRole);
            }

            var spokenLanguagesEnum = EnumConverter.ToEnumList<Languages>(request.SpokenLanguages);

            Address? homeAddress = null;
            var hasAnyAddressPart =
                !string.IsNullOrWhiteSpace(request.Street) ||
                !string.IsNullOrWhiteSpace(request.City) ||
                !string.IsNullOrWhiteSpace(request.ZipCode) ||
                !string.IsNullOrWhiteSpace(request.Country) ||
                !string.IsNullOrWhiteSpace(request.Province);
            if (hasAnyAddressPart)
            {
                homeAddress = new Address(
                    request.Street,
                    request.City,
                    request.Province,
                    request.Country,
                    request.ZipCode
                );
            }

            EmergencyContact? emergencyContact = null;
            var hasAnyEmergencyPart =
                !string.IsNullOrWhiteSpace(request.EmergencyContactName) ||
                !string.IsNullOrWhiteSpace(request.EmergencyContactPhone);
            if (hasAnyEmergencyPart)
            {
                emergencyContact = new EmergencyContact(
                    request.EmergencyContactName,
                    request.EmergencyContactPhone
                );
            }

            user.UpdateProfile(
                new FirstName(request.FirstName),
                new LastName(request.LastName),
                mainRole,
                request.YearsExperience,
                request.Bio,
                request.AvatarUrl,
                homeAddress,
                spokenLanguagesEnum,
                request.IsAvailableForBooking,
                request.NextAvailableDate,
                emergencyContact
            );

            await _db.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
