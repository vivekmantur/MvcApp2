using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class v8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "FrontImage",
                table: "CarDetails",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<string>(
                name: "FuelType",
                table: "CarDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<byte[]>(
                name: "LeftImage",
                table: "CarDetails",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "RearImage",
                table: "CarDetails",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "RightImage",
                table: "CarDetails",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<string>(
                name: "Transmission",
                table: "CarDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "VehicleType",
                table: "CarDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FrontImage",
                table: "CarDetails");

            migrationBuilder.DropColumn(
                name: "FuelType",
                table: "CarDetails");

            migrationBuilder.DropColumn(
                name: "LeftImage",
                table: "CarDetails");

            migrationBuilder.DropColumn(
                name: "RearImage",
                table: "CarDetails");

            migrationBuilder.DropColumn(
                name: "RightImage",
                table: "CarDetails");

            migrationBuilder.DropColumn(
                name: "Transmission",
                table: "CarDetails");

            migrationBuilder.DropColumn(
                name: "VehicleType",
                table: "CarDetails");
        }
    }
}
