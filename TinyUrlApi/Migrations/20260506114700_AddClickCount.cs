using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TinyUrlApi.Migrations
{
    /// <inheritdoc />
    public partial class AddClickCount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPrivate",
                table: "Urls");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPrivate",
                table: "Urls",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
