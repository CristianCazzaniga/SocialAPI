using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialAPI.Migrations
{
    /// <inheritdoc />
    public partial class fixReportToDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "fk_UtenteRichiedente",
                table: "Segnalazioni",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Segnalazioni_fk_UtenteRichiedente",
                table: "Segnalazioni",
                column: "fk_UtenteRichiedente");

            migrationBuilder.AddForeignKey(
                name: "FK_Segnalazioni_AspNetUsers_fk_UtenteRichiedente",
                table: "Segnalazioni",
                column: "fk_UtenteRichiedente",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Segnalazioni_AspNetUsers_fk_UtenteRichiedente",
                table: "Segnalazioni");

            migrationBuilder.DropIndex(
                name: "IX_Segnalazioni_fk_UtenteRichiedente",
                table: "Segnalazioni");

            migrationBuilder.AlterColumn<string>(
                name: "fk_UtenteRichiedente",
                table: "Segnalazioni",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
