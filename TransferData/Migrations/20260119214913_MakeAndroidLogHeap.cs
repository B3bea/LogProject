using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TransferData.Server.Migrations
{
    /// <inheritdoc />
    public partial class MakeAndroidLogHeap : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
        ALTER TABLE dbo.AndroidLog
        DROP CONSTRAINT PK_AndroidLog;
    ");

            migrationBuilder.Sql(@"
        ALTER TABLE dbo.AndroidLog
        ADD CONSTRAINT PK_AndroidLog
        PRIMARY KEY NONCLUSTERED (Id);
    ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
        ALTER TABLE dbo.AndroidLog
        DROP CONSTRAINT PK_AndroidLog;
    ");

            migrationBuilder.Sql(@"
        ALTER TABLE dbo.AndroidLog
        ADD CONSTRAINT PK_AndroidLog
        PRIMARY KEY CLUSTERED (Id);
    ");
        }
    }
}
