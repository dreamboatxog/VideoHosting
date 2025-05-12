using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VideoService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class VideoEnitiyUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HLSPath",
                table: "Videos");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HLSPath",
                table: "Videos",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
