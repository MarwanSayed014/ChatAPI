using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddSenderId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SenderId",
                table: "PrivateMessages",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "SenderId",
                table: "GroupMessages",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_PrivateMessages_SenderId",
                table: "PrivateMessages",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupMessages_SenderId",
                table: "GroupMessages",
                column: "SenderId");

            migrationBuilder.AddForeignKey(
                name: "FK_GroupMessages_Users_SenderId",
                table: "GroupMessages",
                column: "SenderId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PrivateMessages_Users_SenderId",
                table: "PrivateMessages",
                column: "SenderId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroupMessages_Users_SenderId",
                table: "GroupMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_PrivateMessages_Users_SenderId",
                table: "PrivateMessages");

            migrationBuilder.DropIndex(
                name: "IX_PrivateMessages_SenderId",
                table: "PrivateMessages");

            migrationBuilder.DropIndex(
                name: "IX_GroupMessages_SenderId",
                table: "GroupMessages");

            migrationBuilder.DropColumn(
                name: "SenderId",
                table: "PrivateMessages");

            migrationBuilder.DropColumn(
                name: "SenderId",
                table: "GroupMessages");
        }
    }
}
