using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BugFixer.DataLayer.Migrations
{
    public partial class AddAnswerScorModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AnswerUserScores",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    AnswerID = table.Column<long>(type: "bigint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnswerUserScores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnswerUserScores_Answers_AnswerID",
                        column: x => x.AnswerID,
                        principalTable: "Answers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AnswerUserScores_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnswerUserScores_AnswerID",
                table: "AnswerUserScores",
                column: "AnswerID");

            migrationBuilder.CreateIndex(
                name: "IX_AnswerUserScores_UserId",
                table: "AnswerUserScores",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnswerUserScores");
        }
    }
}
