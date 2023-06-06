using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialAPI.Migrations
{
    /// <inheritdoc />
    public partial class fixads : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataPubblicazione",
                table: "Annunci");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DataPubblicazione",
                table: "Annunci",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
