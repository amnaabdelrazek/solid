using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Solid.Api.Migrations
{
    /// <inheritdoc />
    public partial class Addrecommendationcreatedbyuser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CreatedByUserId",
                table: "recommendations",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_recommendations_CreatedByUserId",
                table: "recommendations",
                column: "CreatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_recommendations_users_CreatedByUserId",
                table: "recommendations",
                column: "CreatedByUserId",
                principalTable: "users",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_recommendations_users_CreatedByUserId",
                table: "recommendations");

            migrationBuilder.DropIndex(
                name: "IX_recommendations_CreatedByUserId",
                table: "recommendations");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "recommendations");
        }
    }
}
