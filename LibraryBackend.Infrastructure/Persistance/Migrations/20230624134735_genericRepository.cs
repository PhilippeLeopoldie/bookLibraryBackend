using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryBackend.Migrations
{
    /// <inheritdoc />
    public partial class genericRepository : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OpinionId",
                table: "Opinion",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "BookId",
                table: "Book",
                newName: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Opinion",
                newName: "OpinionId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Book",
                newName: "BookId");
        }
    }
}
