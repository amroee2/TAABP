using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TAABP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CartUserRelationshipMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Carts_PaymentMethods_PaymentMethodId",
                table: "Carts");

            migrationBuilder.DropIndex(
                name: "IX_Carts_PaymentMethodId",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "PaymentMethodId",
                table: "Carts");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Carts",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Carts_UserId",
                table: "Carts",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_AspNetUsers_UserId",
                table: "Carts",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Carts_AspNetUsers_UserId",
                table: "Carts");

            migrationBuilder.DropIndex(
                name: "IX_Carts_UserId",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Carts");

            migrationBuilder.AddColumn<int>(
                name: "PaymentMethodId",
                table: "Carts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Carts_PaymentMethodId",
                table: "Carts",
                column: "PaymentMethodId");

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_PaymentMethods_PaymentMethodId",
                table: "Carts",
                column: "PaymentMethodId",
                principalTable: "PaymentMethods",
                principalColumn: "PaymentMethodId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
