using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialAPI.Migrations
{
    /// <inheritdoc />
    public partial class fixChatToDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationUserChat");

            migrationBuilder.AddColumn<string>(
                name: "UtenteA",
                table: "Chat",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UtenteB",
                table: "Chat",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Chat_UtenteA",
                table: "Chat",
                column: "UtenteA");

            migrationBuilder.CreateIndex(
                name: "IX_Chat_UtenteB",
                table: "Chat",
                column: "UtenteB");

            migrationBuilder.AddForeignKey(
                name: "FK_Chat_AspNetUsers_UtenteA",
                table: "Chat",
                column: "UtenteA",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Chat_AspNetUsers_UtenteB",
                table: "Chat",
                column: "UtenteB",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chat_AspNetUsers_UtenteA",
                table: "Chat");

            migrationBuilder.DropForeignKey(
                name: "FK_Chat_AspNetUsers_UtenteB",
                table: "Chat");

            migrationBuilder.DropIndex(
                name: "IX_Chat_UtenteA",
                table: "Chat");

            migrationBuilder.DropIndex(
                name: "IX_Chat_UtenteB",
                table: "Chat");

            migrationBuilder.DropColumn(
                name: "UtenteA",
                table: "Chat");

            migrationBuilder.DropColumn(
                name: "UtenteB",
                table: "Chat");

            migrationBuilder.CreateTable(
                name: "ApplicationUserChat",
                columns: table => new
                {
                    ChatsId = table.Column<int>(type: "int", nullable: false),
                    UtentiId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUserChat", x => new { x.ChatsId, x.UtentiId });
                    table.ForeignKey(
                        name: "FK_ApplicationUserChat_AspNetUsers_UtentiId",
                        column: x => x.UtentiId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationUserChat_Chat_ChatsId",
                        column: x => x.ChatsId,
                        principalTable: "Chat",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserChat_UtentiId",
                table: "ApplicationUserChat",
                column: "UtentiId");
        }
    }
}
