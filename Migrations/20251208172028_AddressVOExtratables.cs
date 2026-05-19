using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnSet.Migrations
{
    /// <inheritdoc />
    public partial class AddressVOExtratables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Location_PostalCode",
                table: "Projects",
                newName: "Location_ZipCode");

            migrationBuilder.RenameColumn(
                name: "HomeAddress_PostalCode",
                table: "AspNetUsers",
                newName: "HomeAddress_ZipCode");

            migrationBuilder.AddColumn<string>(
                name: "Location_ProvinceOrState",
                table: "Projects",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HomeAddress_ProvinceOrState",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Location_ProvinceOrState",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "HomeAddress_ProvinceOrState",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "Location_ZipCode",
                table: "Projects",
                newName: "Location_PostalCode");

            migrationBuilder.RenameColumn(
                name: "HomeAddress_ZipCode",
                table: "AspNetUsers",
                newName: "HomeAddress_PostalCode");
        }
    }
}
