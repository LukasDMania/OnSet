using MediatR;
using Microsoft.EntityFrameworkCore;
using OnSet.Domain.Enums;
using OnSet.Domain.ValueObjects;
using OnSet.Infrastructure.Data;
using OnSet.Utils;

namespace OnSet.Features.Users.Edit
{
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
                throw new UnauthorizedAccessException();
            }

            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

            if (user == null)
            {
                throw new Exception("user not found");
            }

            // Enum conversions  
            var mainRole = EnumConverter.ToEnum<ProjectRoles>(request.MainOccupationRole);

            var spokenLanguagesEnum = EnumConverter.ToEnumList<Languages>(request.SpokenLanguages);

            user.UpdateProfile(
                new FirstName(request.FirstName),
                new LastName(request.LastName),
                mainRole,
                request.YearsExperience,
                request.Bio,
                request.AvatarUrl,
                new Address(
                    request.Street,
                    request.City,
                    request.Province,
                    request.ZipCode,
                    request.Country),
                spokenLanguagesEnum,
                request.IsAvailableForBooking,
                request.NextAvailableDate,
                new EmergencyContact(
                    request.EmergencyContactName,
                    request.EmergencyContactPhone)
            );

            await _db.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
