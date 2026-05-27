using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnSet.Migrations
{
    /// <inheritdoc />
    public partial class RefactorUserAndProjectForCrewProduction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActualCost",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Budget",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "IsAvailableForBooking",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "Location_ZipCode",
                table: "Projects",
                newName: "ProductionCompanyLocation_ZipCode");

            migrationBuilder.RenameColumn(
                name: "Location_Street",
                table: "Projects",
                newName: "ProductionCompanyLocation_Street");

            migrationBuilder.RenameColumn(
                name: "Location_ProvinceOrState",
                table: "Projects",
                newName: "ProductionCompanyLocation_ProvinceOrState");

            migrationBuilder.RenameColumn(
                name: "Location_Country",
                table: "Projects",
                newName: "ProductionCompanyLocation_Country");

            migrationBuilder.RenameColumn(
                name: "Location_City",
                table: "Projects",
                newName: "ProductionCompanyLocation_City");

            migrationBuilder.RenameColumn(
                name: "ClientName",
                table: "Projects",
                newName: "ProductionCompany");

            migrationBuilder.RenameColumn(
                name: "YearsExperience",
                table: "AspNetUsers",
                newName: "MaritalStatus");

            migrationBuilder.RenameColumn(
                name: "NextAvailableDate",
                table: "AspNetUsers",
                newName: "DateOfBirth");

            migrationBuilder.AddColumn<string>(
                name: "InvoiceAddress_City",
                table: "Projects",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InvoiceAddress_Country",
                table: "Projects",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InvoiceAddress_ProvinceOrState",
                table: "Projects",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InvoiceAddress_Street",
                table: "Projects",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InvoiceAddress_ZipCode",
                table: "Projects",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InvoiceCompanyName",
                table: "Projects",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InvoiceReference",
                table: "Projects",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InvoiceVatNumber",
                table: "Projects",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AccountType",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DietaryPreference",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NationalRegistrationNumber",
                table: "AspNetUsers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Nationality",
                table: "AspNetUsers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PlaceOfBirth",
                table: "AspNetUsers",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InvoiceAddress_City",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "InvoiceAddress_Country",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "InvoiceAddress_ProvinceOrState",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "InvoiceAddress_Street",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "InvoiceAddress_ZipCode",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "InvoiceCompanyName",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "InvoiceReference",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "InvoiceVatNumber",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "AccountType",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "DietaryPreference",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "NationalRegistrationNumber",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Nationality",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "PlaceOfBirth",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "ProductionCompanyLocation_ZipCode",
                table: "Projects",
                newName: "Location_ZipCode");

            migrationBuilder.RenameColumn(
                name: "ProductionCompanyLocation_Street",
                table: "Projects",
                newName: "Location_Street");

            migrationBuilder.RenameColumn(
                name: "ProductionCompanyLocation_ProvinceOrState",
                table: "Projects",
                newName: "Location_ProvinceOrState");

            migrationBuilder.RenameColumn(
                name: "ProductionCompanyLocation_Country",
                table: "Projects",
                newName: "Location_Country");

            migrationBuilder.RenameColumn(
                name: "ProductionCompanyLocation_City",
                table: "Projects",
                newName: "Location_City");

            migrationBuilder.RenameColumn(
                name: "ProductionCompany",
                table: "Projects",
                newName: "ClientName");

            migrationBuilder.RenameColumn(
                name: "MaritalStatus",
                table: "AspNetUsers",
                newName: "YearsExperience");

            migrationBuilder.RenameColumn(
                name: "DateOfBirth",
                table: "AspNetUsers",
                newName: "NextAvailableDate");

            migrationBuilder.AddColumn<decimal>(
                name: "ActualCost",
                table: "Projects",
                type: "money",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Budget",
                table: "Projects",
                type: "money",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Projects",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsAvailableForBooking",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
