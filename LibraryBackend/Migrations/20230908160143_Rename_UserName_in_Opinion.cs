using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryBackend.Migrations
{
    /// <inheritdoc />
    public partial class Rename_UserName_in_Opinion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "userName",
                table: "Opinion",
                newName: "UserName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "Opinion",
                newName: "userName");
        }
    }
}
