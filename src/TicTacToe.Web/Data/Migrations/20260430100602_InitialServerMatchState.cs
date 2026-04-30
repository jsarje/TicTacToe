using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicTacToe.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialServerMatchState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BrowserIdentities",
                columns: table => new
                {
                    BrowserId = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    IssuedUtc = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    LastSeenUtc = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrowserIdentities", x => x.BrowserId);
                });

            migrationBuilder.CreateTable(
                name: "MatchSessions",
                columns: table => new
                {
                    BrowserId = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    MatchId = table.Column<Guid>(type: "TEXT", nullable: false),
                    BoardState = table.Column<string>(type: "TEXT", maxLength: 9, nullable: false),
                    CurrentPlayer = table.Column<int>(type: "INTEGER", nullable: false),
                    Result = table.Column<int>(type: "INTEGER", nullable: false),
                    WinningLineState = table.Column<string>(type: "TEXT", maxLength: 32, nullable: true),
                    Revision = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedUtc = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    LastUpdatedUtc = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchSessions", x => x.BrowserId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BrowserIdentities");

            migrationBuilder.DropTable(
                name: "MatchSessions");
        }
    }
}
