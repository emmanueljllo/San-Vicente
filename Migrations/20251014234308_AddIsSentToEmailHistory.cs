using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hospital_San_Vicente.Migrations
{
    /// <inheritdoc />
    public partial class AddIsSentToEmailHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SendStatus",
                table: "EmailHistory");

            migrationBuilder.RenameColumn(
                name: "SentDate",
                table: "EmailHistory",
                newName: "SentAt");

            migrationBuilder.AddColumn<bool>(
                name: "IsSent",
                table: "EmailHistory",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSent",
                table: "EmailHistory");

            migrationBuilder.RenameColumn(
                name: "SentAt",
                table: "EmailHistory",
                newName: "SentDate");

            migrationBuilder.AddColumn<int>(
                name: "SendStatus",
                table: "EmailHistory",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
