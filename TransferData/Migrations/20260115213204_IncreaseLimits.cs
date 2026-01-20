using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TransferData.Server.Migrations
{
    /// <inheritdoc />
    public partial class IncreaseLimits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "LogDate",
                table: "AndroidLog",
                type: "varchar(100)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(18)");

            migrationBuilder.AlterColumn<string>(
                name: "Component",
                table: "AndroidLog",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "LogDate",
                table: "AndroidLog",
                type: "varchar(18)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)");

            migrationBuilder.AlterColumn<string>(
                name: "Component",
                table: "AndroidLog",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);
        }
    }
}
