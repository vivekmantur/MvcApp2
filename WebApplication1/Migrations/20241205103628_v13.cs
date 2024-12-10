using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class v13 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Documents",
                table: "Sells",
                newName: "Rc");

            migrationBuilder.AddColumn<byte[]>(
                name: "Insurance",
                table: "Sells",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Insurance",
                table: "Sells");

            migrationBuilder.RenameColumn(
                name: "Rc",
                table: "Sells",
                newName: "Documents");
        }
    }
}
