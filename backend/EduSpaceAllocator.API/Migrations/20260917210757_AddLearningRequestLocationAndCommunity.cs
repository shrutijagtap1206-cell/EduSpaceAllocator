using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduSpaceAllocator.API.Migrations
{
    /// <inheritdoc />
    public partial class AddLearningRequestLocationAndCommunity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CommunityId",
                table: "LearningRequests",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "PreferredLatitude",
                table: "LearningRequests",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "PreferredLongitude",
                table: "LearningRequests",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateIndex(
                name: "IX_LearningRequests_CommunityId",
                table: "LearningRequests",
                column: "CommunityId");

            migrationBuilder.AddForeignKey(
                name: "FK_LearningRequests_Communities_CommunityId",
                table: "LearningRequests",
                column: "CommunityId",
                principalTable: "Communities",
                principalColumn: "CommunityId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LearningRequests_Communities_CommunityId",
                table: "LearningRequests");

            migrationBuilder.DropIndex(
                name: "IX_LearningRequests_CommunityId",
                table: "LearningRequests");

            migrationBuilder.DropColumn(
                name: "CommunityId",
                table: "LearningRequests");

            migrationBuilder.DropColumn(
                name: "PreferredLatitude",
                table: "LearningRequests");

            migrationBuilder.DropColumn(
                name: "PreferredLongitude",
                table: "LearningRequests");
        }
    }
}
