using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class v15 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Creating the TestDrive table
            migrationBuilder.CreateTable(
                name: "TestDrives",
                columns: table => new
                {
                    TestDriveId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CarId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Testdrivedate = table.Column<DateOnly>(type: "date", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false, defaultValue: 0) // RequestStatus.Pending
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestDrives", x => x.TestDriveId);

                    // Foreign key to the UserRegistration table
                    table.ForeignKey(
                        name: "FK_TestDrives_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Creating the index for the UserId foreign key column
            migrationBuilder.CreateIndex(
                name: "IX_TestDrives_UserId",
                table: "TestDrives",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Dropping the TestDrives table if the migration is rolled back
            migrationBuilder.DropTable(
                name: "TestDrives");
        }

    }
}
