using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BrainThrust.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigrationss : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Passed",
                table: "UserQuizAttempts",
                newName: "IsPassed");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsPassed",
                table: "UserQuizAttempts",
                newName: "Passed");
        }
    }
}
