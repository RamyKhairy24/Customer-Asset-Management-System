using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CustomerFluent.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "tblCustomer",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Age = table.Column<byte>(type: "tinyint", nullable: false),
                    Height = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Weight = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "GETDATE()"),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblCustomer", x => x.Id);
                    table.CheckConstraint("CK_tblCustomer_Age", "[Age] BETWEEN 1 AND 120");
                    table.CheckConstraint("CK_tblCustomer_Height", "[Height] BETWEEN 50.0 AND 250.0");
                    table.CheckConstraint("CK_tblCustomer_Name", "LEN(LTRIM(RTRIM([Name]))) >= 2");
                    table.CheckConstraint("CK_tblCustomer_Phone", "[PhoneNumber] LIKE '+20[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]' OR [PhoneNumber] LIKE '+20[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]' OR [PhoneNumber] LIKE '01[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]' OR [PhoneNumber] LIKE '0[2-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]'");
                    table.CheckConstraint("CK_tblCustomer_Weight", "[Weight] BETWEEN 2.0 AND 300.0");
                });

            migrationBuilder.CreateTable(
                name: "tblLocation",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Country = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "مصر"),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Street = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    IsHouse = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "GETDATE()"),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblLocation", x => x.Id);
                    table.CheckConstraint("CK_tblLocation_City", "LEN(LTRIM(RTRIM([City]))) >= 2");
                    table.CheckConstraint("CK_tblLocation_Country", "LEN(LTRIM(RTRIM([Country]))) > 0");
                    table.CheckConstraint("CK_tblLocation_Street", "LEN(LTRIM(RTRIM([Street]))) >= 3");
                    table.ForeignKey(
                        name: "FK_tblLocation_Customer",
                        column: x => x.CustomerId,
                        principalSchema: "dbo",
                        principalTable: "tblCustomer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblPaymentStatus",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AmountPaid = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AmountRemaining = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "GETDATE()"),
                    CreatedDate = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "GETDATE()"),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblPaymentStatus", x => x.Id);
                    table.CheckConstraint("CK_tblPaymentStatus_AmountPaid", "[AmountPaid] >= 0");
                    table.CheckConstraint("CK_tblPaymentStatus_AmountRemaining", "[AmountRemaining] >= 0");
                    table.CheckConstraint("CK_tblPaymentStatus_MaxAmount", "[AmountPaid] <= 99999999.99 AND [AmountRemaining] <= 99999999.99");
                    table.CheckConstraint("CK_tblPaymentStatus_TotalAmount", "[AmountPaid] + [AmountRemaining] > 0");
                    table.ForeignKey(
                        name: "FK_tblPaymentStatus_Customer",
                        column: x => x.CustomerId,
                        principalSchema: "dbo",
                        principalTable: "tblCustomer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblBuilding",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BuildingNumber = table.Column<int>(type: "int", nullable: false),
                    Floor = table.Column<byte>(type: "tinyint", nullable: false),
                    ApartmentNumber = table.Column<int>(type: "int", nullable: false),
                    LocationId = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "GETDATE()"),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblBuilding", x => x.Id);
                    table.CheckConstraint("CK_tblBuilding_ApartmentNumber", "[ApartmentNumber] > 0");
                    table.CheckConstraint("CK_tblBuilding_BuildingNumber", "[BuildingNumber] > 0");
                    table.CheckConstraint("CK_tblBuilding_Floor", "[Floor] BETWEEN 0 AND 200");
                    table.ForeignKey(
                        name: "FK_tblBuilding_Location",
                        column: x => x.LocationId,
                        principalSchema: "dbo",
                        principalTable: "tblLocation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblHouse",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HouseNumber = table.Column<int>(type: "int", nullable: false),
                    LocationId = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "GETDATE()"),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblHouse", x => x.Id);
                    table.CheckConstraint("CK_tblHouse_HouseNumber", "[HouseNumber] > 0 AND [HouseNumber] <= 999999");
                    table.ForeignKey(
                        name: "FK_tblHouse_Location",
                        column: x => x.LocationId,
                        principalSchema: "dbo",
                        principalTable: "tblLocation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tblBuilding_LocationId",
                schema: "dbo",
                table: "tblBuilding",
                column: "LocationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tblCustomer_Name",
                schema: "dbo",
                table: "tblCustomer",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_tblHouse_LocationId",
                schema: "dbo",
                table: "tblHouse",
                column: "LocationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tblLocation_Country_City",
                schema: "dbo",
                table: "tblLocation",
                columns: new[] { "Country", "City" });

            migrationBuilder.CreateIndex(
                name: "IX_tblLocation_CustomerId",
                schema: "dbo",
                table: "tblLocation",
                column: "CustomerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tblPaymentStatus_CustomerId",
                schema: "dbo",
                table: "tblPaymentStatus",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_tblPaymentStatus_PaymentDate",
                schema: "dbo",
                table: "tblPaymentStatus",
                column: "PaymentDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tblBuilding",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tblHouse",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tblPaymentStatus",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tblLocation",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tblCustomer",
                schema: "dbo");
        }
    }
}
