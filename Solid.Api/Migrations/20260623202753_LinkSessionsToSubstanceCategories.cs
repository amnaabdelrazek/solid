using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Solid.Api.Migrations
{
    /// <inheritdoc />
    public partial class LinkSessionsToSubstanceCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_therapy_sessions_groups_group_id",
                table: "therapy_sessions");

            migrationBuilder.RenameColumn(
                name: "group_id",
                table: "therapy_sessions",
                newName: "substance_category_id");

            migrationBuilder.RenameIndex(
                name: "IX_therapy_sessions_group_id",
                table: "therapy_sessions",
                newName: "IX_therapy_sessions_substance_category_id");

            migrationBuilder.Sql("""
                UPDATE ts
                SET substance_category_id = g.substance_category_id
                FROM therapy_sessions ts
                INNER JOIN groups g ON g.id = ts.substance_category_id
                WHERE g.substance_category_id IS NOT NULL
                """);

            migrationBuilder.AddForeignKey(
                name: "FK_therapy_sessions_substance_categories_substance_category_id",
                table: "therapy_sessions",
                column: "substance_category_id",
                principalTable: "substance_categories",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_therapy_sessions_substance_categories_substance_category_id",
                table: "therapy_sessions");

            migrationBuilder.RenameColumn(
                name: "substance_category_id",
                table: "therapy_sessions",
                newName: "group_id");

            migrationBuilder.RenameIndex(
                name: "IX_therapy_sessions_substance_category_id",
                table: "therapy_sessions",
                newName: "IX_therapy_sessions_group_id");

            migrationBuilder.Sql("""
                UPDATE ts
                SET group_id = g.id
                FROM therapy_sessions ts
                INNER JOIN groups g ON g.substance_category_id = ts.group_id
                WHERE g.id = (
                    SELECT TOP 1 candidate.id
                    FROM groups candidate
                    WHERE candidate.substance_category_id = ts.group_id
                    ORDER BY candidate.id
                )
                """);

            migrationBuilder.AddForeignKey(
                name: "FK_therapy_sessions_groups_group_id",
                table: "therapy_sessions",
                column: "group_id",
                principalTable: "groups",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
