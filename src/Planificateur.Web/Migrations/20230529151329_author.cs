using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Planificateur.Web.Migrations
{
    /// <inheritdoc />
    public partial class author : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AuthorId",
                table: "Polls",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "Polls");
        }
    }
}
