using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIMIV.Migrations
{
    /// <inheritdoc />
    public partial class AddRespostaIAToTicket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RespostaIA",
                table: "Tickets",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RespostaIA",
                table: "Tickets");
        }
    }
}
