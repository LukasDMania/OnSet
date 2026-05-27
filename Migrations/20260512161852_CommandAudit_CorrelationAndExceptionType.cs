using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnSet.Migrations
{
    /// <inheritdoc />
    public partial class CommandAudit_CorrelationAndExceptionType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CorrelationId",
                table: "CommandAuditLogs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExceptionType",
                table: "CommandAuditLogs",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CorrelationId",
                table: "CommandAuditLogs");

            migrationBuilder.DropColumn(
                name: "ExceptionType",
                table: "CommandAuditLogs");
        }
    }
}
