using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CallLogs.Migrations
{
    /// <inheritdoc />
    public partial class callerid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CallerId",
                table: "CallLogs",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CallerId",
                table: "CallLogs");
        }
    }
}
