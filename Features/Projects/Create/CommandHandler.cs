using MediatR;
using OnSet.Application.Notifications.Projects;
using OnSet.Application.Services;
using OnSet.Domain.Models;
using OnSet.Domain.ValueObjects;
using OnSet.Infrastructure.Persistence;
using OnSet.Infrastructure.Results;

namespace OnSet.Features.Projects.Create
{
    /// <summary>MediatR handler for this feature slice.</summary>
    public class CommandHandler : IRequestHandler<Command, Result>
    {
        private readonly OnSetDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMediator _mediator;

        public CommandHandler(
            OnSetDbContext context,
            ICurrentUserService currentUserService,
            IMediator mediator)
        {
            _context = context;
            _currentUserService = currentUserService;
            _mediator = mediator;
        }

        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Result.Fail("User is not authenticated.");
            }

            Address? productionCompanyLocation = null;

            var hasAnyAddressPart =
                !string.IsNullOrWhiteSpace(request.Street) ||
                !string.IsNullOrWhiteSpace(request.City) ||
                !string.IsNullOrWhiteSpace(request.ZipCode) ||
                !string.IsNullOrWhiteSpace(request.Country) ||
                !string.IsNullOrWhiteSpace(request.Province);

            if (hasAnyAddressPart)
            {
                productionCompanyLocation = new Address(
                    request.Street!,
                    request.City!,
                    request.Province,
                    request.Country!,
                    request.ZipCode!
                );
            }

            Address? invoiceAddress = null;
            var hasAnyInvoiceAddressPart =
                !string.IsNullOrWhiteSpace(request.InvoiceStreet) ||
                !string.IsNullOrWhiteSpace(request.InvoiceCity) ||
                !string.IsNullOrWhiteSpace(request.InvoiceZipCode) ||
                !string.IsNullOrWhiteSpace(request.InvoiceCountry) ||
                !string.IsNullOrWhiteSpace(request.InvoiceProvince);

            if (hasAnyInvoiceAddressPart)
            {
                invoiceAddress = new Address(
                    request.InvoiceStreet!,
                    request.InvoiceCity!,
                    request.InvoiceProvince,
                    request.InvoiceCountry!,
                    request.InvoiceZipCode!
                );
            }

            var project = Project.Create(
                name: request.ProjectName,
                startDate: request.StartDate,
                creatorRole: request.CreatorRole,
                ownerId: userId,
                description: request.Description,
                productionCompany: request.ProductionCompany,
                referenceCode: request.ReferenceCode,
                productionCompanyLocation: productionCompanyLocation,
                invoiceCompanyName: request.InvoiceCompanyName,
                invoiceAddress: invoiceAddress,
                invoiceVatNumber: request.InvoiceVatNumber,
                invoiceReference: request.InvoiceReference
            );

            _context.Projects.Add(project);
            await _context.SaveChangesAsync(cancellationToken);

            await _mediator.Publish(
                new ProjectCreatedNotification(project.Id, userId, project.Name),
                cancellationToken);

            return Result.Ok();
        }
    }
}
