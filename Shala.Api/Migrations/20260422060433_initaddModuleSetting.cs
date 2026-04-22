using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shala.Api.Migrations
{
    /// <inheritdoc />
    public partial class initaddModuleSetting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRegistrationModuleEnabled",
                table: "RegistrationFeeConfigurations",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRegistrationModuleEnabled",
                table: "RegistrationFeeConfigurations");
        }
    }
}
