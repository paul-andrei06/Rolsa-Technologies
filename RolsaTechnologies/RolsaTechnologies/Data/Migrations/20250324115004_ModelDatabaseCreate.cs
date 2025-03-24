using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RolsaTechnologies.Data.Migrations
{
    /// <inheritdoc />
    public partial class ModelDatabaseCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Calculator",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ElectricityUsage = table.Column<double>(type: "float", nullable: false),
                    GasUsage = table.Column<double>(type: "float", nullable: false),
                    CarMilesPerWeek = table.Column<double>(type: "float", nullable: false),
                    CarFuelEfficiency = table.Column<double>(type: "float", nullable: false),
                    PublicTransportMilesPerWeek = table.Column<double>(type: "float", nullable: false),
                    WasteProducedPerWeek = table.Column<double>(type: "float", nullable: false),
                    RecyclingHabits = table.Column<bool>(type: "bit", nullable: false),
                    MeatConsumptionPerWeek = table.Column<double>(type: "float", nullable: false),
                    CalculatedCarbonFootprint = table.Column<double>(type: "float", nullable: false),
                    DateCalculated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Calculator", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EnergyTracker",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Consumption = table.Column<double>(type: "float", nullable: false),
                    EnergyType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnergyTracker", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ScheduleConsultation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ScheduledDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ContactMethod = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Mobile = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContactEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduleConsultation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ScheduleInstalltion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ScheduledDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ApplianceType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Mobile = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduleInstalltion", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Calculator");

            migrationBuilder.DropTable(
                name: "EnergyTracker");

            migrationBuilder.DropTable(
                name: "ScheduleConsultation");

            migrationBuilder.DropTable(
                name: "ScheduleInstalltion");
        }
    }
}
