using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rentopolis.Migrations
{
    /// <inheritdoc />
    public partial class Added_Status_Column_In_RentalRequestsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "RentalRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "RentalRequests");
        }
    }
}
