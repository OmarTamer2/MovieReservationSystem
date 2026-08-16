using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieReservationSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddingEndTimeToShowTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "date",
                table: "ShowTimes",
                newName: "startTime");

            migrationBuilder.AddColumn<DateTime>(
                name: "endTime",
                table: "ShowTimes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "endTime",
                table: "ShowTimes");

            migrationBuilder.RenameColumn(
                name: "startTime",
                table: "ShowTimes",
                newName: "date");
        }
    }
}
