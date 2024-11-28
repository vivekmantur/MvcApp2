using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class v10 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VerificationAppointments",
                columns: table => new
                {
                    VerificationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestId = table.Column<int>(type: "int", nullable: false),
                    VerificationDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Verified = table.Column<string>(type: "nvarchar(max)", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VerificationAppointments", x => x.VerificationId);
                    table.ForeignKey(
                        name: "FK_VerificationAppointments_Requests_RequestId",
                        column: x => x.RequestId,
                        principalTable: "Requests",
                        principalColumn: "RequestId",
                        onDelete: ReferentialAction.Cascade);
                });

            // You may also want to add indexes here if needed
            migrationBuilder.CreateIndex(
                name: "IX_VerificationAppointments_RequestId",
                table: "VerificationAppointments",
                column: "RequestId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VerificationAppointments");
        }
    }
}
