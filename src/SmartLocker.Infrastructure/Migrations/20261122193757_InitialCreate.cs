using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SmartLocker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LockerMachines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LockerMachines", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LockerCompartments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LockerMachineId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompartmentNumber = table.Column<int>(type: "int", nullable: false),
                    Size = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LockerCompartments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LockerCompartments_LockerMachines_LockerMachineId",
                        column: x => x.LockerMachineId,
                        principalTable: "LockerMachines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Parcels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TrackingNumber = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    RecipientPhone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    RequiredSize = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    AssignedCompartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PickupCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DepositedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PickedUpAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parcels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Parcels_LockerCompartments_AssignedCompartmentId",
                        column: x => x.AssignedCompartmentId,
                        principalTable: "LockerCompartments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ParcelEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParcelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventType = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    OccurredAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParcelEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ParcelEvents_Parcels_ParcelId",
                        column: x => x.ParcelId,
                        principalTable: "Parcels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "LockerMachines",
                columns: new[] { "Id", "Address", "City", "Code", "IsActive" },
                values: new object[] { new Guid("11111111-1111-1111-1111-111111111111"), "ul. Marszalkowska 10", "Warszawa", "WAW-01", true });

            migrationBuilder.InsertData(
                table: "LockerCompartments",
                columns: new[] { "Id", "CompartmentNumber", "LockerMachineId", "Size", "Status" },
                values: new object[,]
                {
                    { new Guid("22222222-2222-2222-2222-222222222201"), 1, new Guid("11111111-1111-1111-1111-111111111111"), 1, 1 },
                    { new Guid("22222222-2222-2222-2222-222222222202"), 2, new Guid("11111111-1111-1111-1111-111111111111"), 1, 1 },
                    { new Guid("22222222-2222-2222-2222-222222222203"), 3, new Guid("11111111-1111-1111-1111-111111111111"), 2, 1 },
                    { new Guid("22222222-2222-2222-2222-222222222204"), 4, new Guid("11111111-1111-1111-1111-111111111111"), 3, 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_LockerCompartments_LockerMachineId_CompartmentNumber",
                table: "LockerCompartments",
                columns: new[] { "LockerMachineId", "CompartmentNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LockerMachines_Code",
                table: "LockerMachines",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ParcelEvents_ParcelId",
                table: "ParcelEvents",
                column: "ParcelId");

            migrationBuilder.CreateIndex(
                name: "IX_Parcels_AssignedCompartmentId",
                table: "Parcels",
                column: "AssignedCompartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Parcels_TrackingNumber",
                table: "Parcels",
                column: "TrackingNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ParcelEvents");

            migrationBuilder.DropTable(
                name: "Parcels");

            migrationBuilder.DropTable(
                name: "LockerCompartments");

            migrationBuilder.DropTable(
                name: "LockerMachines");
        }
    }
}
