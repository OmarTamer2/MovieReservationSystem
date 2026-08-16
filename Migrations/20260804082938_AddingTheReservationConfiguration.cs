using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieReservationSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddingTheReservationConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "IdempotencyKey",
                table: "Reservations",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Seats_hallId",
                table: "Seats",
                column: "hallId");

            migrationBuilder.AddForeignKey(
                name: "FK_Seats_Halls_hallId",
                table: "Seats",
                column: "hallId",
                principalTable: "Halls",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Seats_Halls_hallId",
                table: "Seats");

            migrationBuilder.DropIndex(
                name: "IX_Seats_hallId",
                table: "Seats");

            migrationBuilder.DropColumn(
                name: "IdempotencyKey",
                table: "Reservations");
        }
    }
}
