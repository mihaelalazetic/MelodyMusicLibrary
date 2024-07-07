using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MelodyMusicLibrary.Migrations
{
    /// <inheritdoc />
    public partial class YoutubeIDAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "YouTubeVideoId",
                table: "Song",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "YouTubeVideoId",
                table: "Song");
        }
    }
}
