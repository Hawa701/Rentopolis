using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rentopolis.Migrations
{
    /// <inheritdoc />
    public partial class Added_RentalRequest_Table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RentalRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PropertyId = table.Column<int>(type: "int", nullable: false),
                    TenantId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RentalRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RentalRequests_AspNetUsers_TenantId",
                        column: x => x.TenantId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RentalRequests_Properties_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "Properties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RentalRequests_PropertyId",
                table: "RentalRequests",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_RentalRequests_TenantId",
                table: "RentalRequests",
                column: "TenantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RentalRequests");
        }
    }
}
