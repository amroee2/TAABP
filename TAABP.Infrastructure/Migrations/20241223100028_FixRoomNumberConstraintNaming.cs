using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TAABP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixRoomNumberConstraintNaming : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Room_Number_Positive",
                table: "Rooms");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Room_Number_Positive",
                table: "Rooms",
                sql: "[RoomNumber] > 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Room_Number_Positive",
                table: "Rooms");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Room_Number_Positive",
                table: "Rooms",
                sql: "[Number] > 0");
        }
    }
}
