using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TAABP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class HotelNumberOfVisitsMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NumberOfVisits",
                table: "Hotels",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddCheckConstraint(
                name: "CK_Hotel_NumberOfVisits_Positive",
                table: "Hotels",
                sql: "[NumberOfVisits] >= 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Hotel_NumberOfVisits_Positive",
                table: "Hotels");

            migrationBuilder.DropColumn(
                name: "NumberOfVisits",
                table: "Hotels");
        }
    }
}
