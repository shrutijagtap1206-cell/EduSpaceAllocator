using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EduSpaceAllocator.API.Migrations
{
    /// <inheritdoc />
    public partial class AddLearningRequests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LearningRequests",
                columns: table => new
                {
                    RequestId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Organization = table.Column<string>(type: "text", nullable: false),
                    ProgramType = table.Column<string>(type: "text", nullable: false),
                    StudentCapacity = table.Column<int>(type: "integer", nullable: false),
                    Budget = table.Column<decimal>(type: "numeric", nullable: false),
                    PreferredLocation = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LearningRequests", x => x.RequestId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LearningRequests");
        }
    }
}
