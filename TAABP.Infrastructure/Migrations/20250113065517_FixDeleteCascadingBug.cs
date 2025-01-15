using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TAABP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixDeleteCascadingBug : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Carts_PaymentMethods_PaymentMethodId",
                table: "Carts");

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_PaymentMethods_PaymentMethodId",
                table: "Carts",
                column: "PaymentMethodId",
                principalTable: "PaymentMethods",
                principalColumn: "PaymentMethodId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Carts_PaymentMethods_PaymentMethodId",
                table: "Carts");

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_PaymentMethods_PaymentMethodId",
                table: "Carts",
                column: "PaymentMethodId",
                principalTable: "PaymentMethods",
                principalColumn: "PaymentMethodId");
        }
    }
}
