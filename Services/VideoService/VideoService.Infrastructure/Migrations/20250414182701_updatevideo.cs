using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VideoService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updatevideo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HLSPath",
                table: "Videos",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ThumbnailPath",
                table: "Videos",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HLSPath",
                table: "Videos");

            migrationBuilder.DropColumn(
                name: "ThumbnailPath",
                table: "Videos");
        }
    }
}
