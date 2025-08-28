using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExampleAbpApplicationLayer.Migrations
{
    /// <inheritdoc />
    public partial class Updated_Order_25082902090486 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "IdentityUserId",
                table: "AppOrders",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppOrders_IdentityUserId",
                table: "AppOrders",
                column: "IdentityUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppOrders_AbpUsers_IdentityUserId",
                table: "AppOrders",
                column: "IdentityUserId",
                principalTable: "AbpUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppOrders_AbpUsers_IdentityUserId",
                table: "AppOrders");

            migrationBuilder.DropIndex(
                name: "IX_AppOrders_IdentityUserId",
                table: "AppOrders");

            migrationBuilder.DropColumn(
                name: "IdentityUserId",
                table: "AppOrders");
        }
    }
}
