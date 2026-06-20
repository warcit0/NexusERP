using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NexusERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLicenseAndPlanFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ValidTo",
                schema: "platform",
                table: "Licenses",
                newName: "StartDate");

            migrationBuilder.RenameColumn(
                name: "ValidFrom",
                schema: "platform",
                table: "Licenses",
                newName: "EndDate");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                schema: "platform",
                table: "SubscriptionPlans",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "DurationDays",
                schema: "platform",
                table: "SubscriptionPlans",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Code",
                schema: "platform",
                table: "SubscriptionPlans");

            migrationBuilder.DropColumn(
                name: "DurationDays",
                schema: "platform",
                table: "SubscriptionPlans");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                schema: "platform",
                table: "Licenses",
                newName: "ValidTo");

            migrationBuilder.RenameColumn(
                name: "EndDate",
                schema: "platform",
                table: "Licenses",
                newName: "ValidFrom");
        }
    }
}
