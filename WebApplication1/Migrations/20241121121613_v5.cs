using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class v5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
               name: "CarsSold",
               columns: table => new
               {
                   SoldId = table.Column<int>(type: "int", nullable: false)
                       .Annotation("SqlServer:Identity", "1, 1"),
                   UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                   CarId = table.Column<int>(type: "int", nullable: false),
                   SellerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                   BuyerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                   CarName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                   CarType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                   Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
               },
               constraints: table =>
               {
                   table.PrimaryKey("PK_CarsSold", x => x.SoldId);
                   table.ForeignKey(
                       name: "FK_CarsSold_AspNetUsers_UserId",
                       column: x => x.UserId,
                       principalTable: "AspNetUsers",
                       principalColumn: "Id",
                       onDelete: ReferentialAction.Cascade);
               });

            migrationBuilder.CreateIndex(
                name: "IX_CarsSold_UserId",
                table: "CarsSold",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
               name: "CarsSold");
        }
    }
}
