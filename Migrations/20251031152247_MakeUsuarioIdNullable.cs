using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIMIV.Migrations
{
    /// <inheritdoc />
    public partial class MakeUsuarioIdNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Usuarios_UsuarioId",
                table: "Tickets");

            migrationBuilder.AlterColumn<int>(
                name: "UsuarioId",
                table: "Tickets",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Usuarios_UsuarioId",
                table: "Tickets",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Usuarios_UsuarioId",
                table: "Tickets");

            migrationBuilder.AlterColumn<int>(
                name: "UsuarioId",
                table: "Tickets",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Usuarios_UsuarioId",
                table: "Tickets",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
