using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class v2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Sells",
                columns: table => new
                {
                    SellId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OwnerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CarName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Variant = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Transmission = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FuelType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    VehicleType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Brand = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChassisNumber = table.Column<long>(type: "bigint", nullable: false),
                    Kilometers = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RcNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RearImage = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    FrontImage = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    LeftImage = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    RightImage = table.Column<byte[]>(type: "varbinary(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sells", x => x.SellId);
                    table.ForeignKey(
                        name: "FK_Sells_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Requests",
                columns: table => new
                {
                    RequestId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Userid = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Sellid = table.Column<int>(type: "int", nullable: false),
                    Sellername = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Carname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Requests", x => x.RequestId);
                    table.ForeignKey(
                        name: "FK_Requests_Sells_Sellid",
                        column: x => x.Sellid,
                        principalTable: "Sells",
                        principalColumn: "SellId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Requests_Sellid",
                table: "Requests",
                column: "Sellid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sells_UserId",
                table: "Sells",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Requests");

            migrationBuilder.DropTable(
                name: "Sells");
        }
    }
}
