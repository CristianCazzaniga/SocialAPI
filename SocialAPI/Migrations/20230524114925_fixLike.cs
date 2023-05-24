using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialAPI.Migrations
{
    /// <inheritdoc />
    public partial class fixLike : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LikeDTO",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TipoDestinazione = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdFk = table.Column<int>(type: "int", nullable: false),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CommentoId = table.Column<int>(type: "int", nullable: true),
                    MessaggioId = table.Column<int>(type: "int", nullable: true),
                    PostId = table.Column<int>(type: "int", nullable: true),
                    StoriaId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LikeDTO", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LikeDTO_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LikeDTO_Commenti_CommentoId",
                        column: x => x.CommentoId,
                        principalTable: "Commenti",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LikeDTO_Messaggi_MessaggioId",
                        column: x => x.MessaggioId,
                        principalTable: "Messaggi",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LikeDTO_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LikeDTO_Storie_StoriaId",
                        column: x => x.StoriaId,
                        principalTable: "Storie",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_LikeDTO_ApplicationUserId",
                table: "LikeDTO",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_LikeDTO_CommentoId",
                table: "LikeDTO",
                column: "CommentoId");

            migrationBuilder.CreateIndex(
                name: "IX_LikeDTO_MessaggioId",
                table: "LikeDTO",
                column: "MessaggioId");

            migrationBuilder.CreateIndex(
                name: "IX_LikeDTO_PostId",
                table: "LikeDTO",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_LikeDTO_StoriaId",
                table: "LikeDTO",
                column: "StoriaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LikeDTO");
        }
    }
}
