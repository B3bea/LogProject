using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TransferData.Server.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreateWithNewSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AndroidLog",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LogDate = table.Column<string>(type: "varchar(18)", nullable: false),
                    Pid = table.Column<int>(type: "int", nullable: false),
                    Tid = table.Column<int>(type: "int", nullable: false),
                    Level = table.Column<string>(type: "char(1)", nullable: false),
                    Component = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AndroidLog", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AndroidLog");
        }
    }
}
