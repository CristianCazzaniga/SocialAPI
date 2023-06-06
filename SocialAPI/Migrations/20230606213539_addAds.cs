using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialAPI.Migrations
{
    /// <inheritdoc />
    public partial class addAds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Href",
                table: "Annunci");

            migrationBuilder.DropColumn(
                name: "Prezzo",
                table: "Annunci");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "Annunci");

            migrationBuilder.RenameColumn(
                name: "Url",
                table: "Annunci",
                newName: "Media");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Media",
                table: "Annunci",
                newName: "Url");

            migrationBuilder.AddColumn<string>(
                name: "Href",
                table: "Annunci",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "Prezzo",
                table: "Annunci",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "Annunci",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
