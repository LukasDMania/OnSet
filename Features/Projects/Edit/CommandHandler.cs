using MediatR;
using OnSet.Application.Exceptions;
using OnSet.Domain.ValueObjects;
using OnSet.Infrastructure.Persistence;

namespace OnSet.Features.Projects.Edit
{
    /// <summary>MediatR handler for this feature slice.</summary>
    public class CommandHandler : IRequestHandler<Command, Unit>
    {
        private readonly OnSetDbContext _db;

        public CommandHandler(OnSetDbContext db)
        {
            _db = db;
        }

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            var project = await _db.Projects.FindAsync([request.Id], cancellationToken);
            if (project == null)
            {
                throw new NotFoundException("Project", request.Id);
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
                    street: request.Street!,
                    city: request.City!,
                    provinceOrState: request.Province,
                    country: request.Country!,
                    zipCode: request.ZipCode!
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
                    street: request.InvoiceStreet!,
                    city: request.InvoiceCity!,
                    provinceOrState: request.InvoiceProvince,
                    country: request.InvoiceCountry!,
                    zipCode: request.InvoiceZipCode!
                );
            }

            project.UpdateDetails(
                name: request.Name,
                description: request.Description,
                startDate: request.StartDate,
                endDate: request.EndDate,
                productionCompany: request.ProductionCompany,
                referenceCode: request.ReferenceCode,
                productionCompanyLocation: productionCompanyLocation,
                invoiceCompanyName: request.InvoiceCompanyName,
                invoiceAddress: invoiceAddress,
                invoiceVatNumber: request.InvoiceVatNumber,
                invoiceReference: request.InvoiceReference
            );

            await _db.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}

