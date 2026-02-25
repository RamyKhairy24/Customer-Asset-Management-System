using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CustomerFluent.Migrations
{
    /// <inheritdoc />
    public partial class CompleteReset : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_tblPaymentStatus_AmountPaid",
                schema: "dbo",
                table: "tblPaymentStatus");

            migrationBuilder.DropCheckConstraint(
                name: "CK_tblPaymentStatus_TotalAmount",
                schema: "dbo",
                table: "tblPaymentStatus");

            migrationBuilder.DropCheckConstraint(
                name: "CK_tblLocation_City",
                schema: "dbo",
                table: "tblLocation");

            migrationBuilder.DropCheckConstraint(
                name: "CK_tblLocation_Country",
                schema: "dbo",
                table: "tblLocation");

            migrationBuilder.DropCheckConstraint(
                name: "CK_tblLocation_Street",
                schema: "dbo",
                table: "tblLocation");

            migrationBuilder.DropCheckConstraint(
                name: "CK_tblHouse_HouseNumber",
                schema: "dbo",
                table: "tblHouse");

            migrationBuilder.DropCheckConstraint(
                name: "CK_tblCustomer_Age",
                schema: "dbo",
                table: "tblCustomer");

            migrationBuilder.DropCheckConstraint(
                name: "CK_tblCustomer_Height",
                schema: "dbo",
                table: "tblCustomer");

            migrationBuilder.DropCheckConstraint(
                name: "CK_tblCustomer_Name",
                schema: "dbo",
                table: "tblCustomer");

            migrationBuilder.DropCheckConstraint(
                name: "CK_tblCustomer_Phone",
                schema: "dbo",
                table: "tblCustomer");

            migrationBuilder.DropCheckConstraint(
                name: "CK_tblCustomer_Weight",
                schema: "dbo",
                table: "tblCustomer");

            migrationBuilder.DropCheckConstraint(
                name: "CK_tblBuilding_ApartmentNumber",
                schema: "dbo",
                table: "tblBuilding");

            migrationBuilder.DropCheckConstraint(
                name: "CK_tblBuilding_BuildingNumber",
                schema: "dbo",
                table: "tblBuilding");

            migrationBuilder.DropCheckConstraint(
                name: "CK_tblBuilding_Floor",
                schema: "dbo",
                table: "tblBuilding");

            migrationBuilder.AlterColumn<DateTime>(
                name: "PaymentDate",
                schema: "dbo",
                table: "tblPaymentStatus",
                type: "datetime2(7)",
                nullable: false,
                defaultValueSql: "GETDATE()",
                comment: "Date of payment transaction",
                oldClrType: typeof(DateTime),
                oldType: "datetime2(7)",
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.AlterColumn<decimal>(
                name: "AmountRemaining",
                schema: "dbo",
                table: "tblPaymentStatus",
                type: "decimal(18,2)",
                nullable: false,
                comment: "Remaining amount to be paid",
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "AmountPaid",
                schema: "dbo",
                table: "tblPaymentStatus",
                type: "decimal(18,2)",
                nullable: false,
                comment: "Amount already paid by customer",
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "Street",
                schema: "dbo",
                table: "tblLocation",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: false,
                comment: "Street address details",
                oldClrType: typeof(string),
                oldType: "nvarchar(300)",
                oldMaxLength: 300);

            migrationBuilder.AlterColumn<bool>(
                name: "IsHouse",
                schema: "dbo",
                table: "tblLocation",
                type: "bit",
                nullable: false,
                defaultValue: false,
                comment: "True for house, False for apartment building",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "Country",
                schema: "dbo",
                table: "tblLocation",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "مصر",
                comment: "Country name (default: Egypt)",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldDefaultValue: "مصر");

            migrationBuilder.AlterColumn<string>(
                name: "City",
                schema: "dbo",
                table: "tblLocation",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                comment: "City or governorate name",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<int>(
                name: "HouseNumber",
                schema: "dbo",
                table: "tblHouse",
                type: "int",
                nullable: false,
                comment: "House number on the street",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<decimal>(
                name: "Weight",
                schema: "dbo",
                table: "tblCustomer",
                type: "decimal(5,2)",
                nullable: false,
                comment: "Weight in kilograms",
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)");

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                schema: "dbo",
                table: "tblCustomer",
                type: "nvarchar(25)",
                maxLength: 25,
                nullable: false,
                comment: "Phone number in Egyptian format",
                oldClrType: typeof(string),
                oldType: "nvarchar(25)",
                oldMaxLength: 25);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "dbo",
                table: "tblCustomer",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                comment: "Full name of the customer",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                schema: "dbo",
                table: "tblCustomer",
                type: "datetime2(7)",
                nullable: false,
                defaultValueSql: "GETDATE()",
                comment: "Last modification timestamp",
                oldClrType: typeof(DateTime),
                oldType: "datetime2(7)",
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.AlterColumn<decimal>(
                name: "Height",
                schema: "dbo",
                table: "tblCustomer",
                type: "decimal(5,2)",
                nullable: false,
                comment: "Height in centimeters",
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                schema: "dbo",
                table: "tblCustomer",
                type: "datetime2(7)",
                nullable: false,
                defaultValueSql: "GETDATE()",
                comment: "Record creation timestamp",
                oldClrType: typeof(DateTime),
                oldType: "datetime2(7)",
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.AlterColumn<byte>(
                name: "Age",
                schema: "dbo",
                table: "tblCustomer",
                type: "tinyint",
                nullable: false,
                comment: "Age in years (must be between 16-120)",
                oldClrType: typeof(byte),
                oldType: "tinyint");

            migrationBuilder.AlterColumn<byte>(
                name: "Floor",
                schema: "dbo",
                table: "tblBuilding",
                type: "tinyint",
                nullable: false,
                comment: "Floor number (0=Ground, 1-200=Upper floors)",
                oldClrType: typeof(byte),
                oldType: "tinyint");

            migrationBuilder.AlterColumn<int>(
                name: "BuildingNumber",
                schema: "dbo",
                table: "tblBuilding",
                type: "int",
                nullable: false,
                comment: "Building number on the street",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ApartmentNumber",
                schema: "dbo",
                table: "tblBuilding",
                type: "int",
                nullable: false,
                comment: "Apartment number on the floor",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "Roles",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    LastLoginDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "dbo",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                schema: "dbo",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                schema: "dbo",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "dbo",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                schema: "dbo",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "dbo",
                table: "tblCustomer",
                columns: new[] { "Id", "Age", "CreatedDate", "Height", "ModifiedDate", "Name", "PhoneNumber", "Weight" },
                values: new object[,]
                {
                    { 1, (byte)28, new DateTime(2024, 1, 15, 10, 30, 0, 0, DateTimeKind.Unspecified), 175.50m, new DateTime(2024, 1, 15, 10, 30, 0, 0, DateTimeKind.Unspecified), "أحمد محمد علي", "+201234567890", 75.20m },
                    { 2, (byte)25, new DateTime(2024, 2, 10, 14, 20, 0, 0, DateTimeKind.Unspecified), 162.00m, new DateTime(2024, 2, 10, 14, 20, 0, 0, DateTimeKind.Unspecified), "فاطمة حسن محمود", "01098765432", 58.50m },
                    { 3, (byte)35, new DateTime(2024, 3, 5, 9, 15, 0, 0, DateTimeKind.Unspecified), 180.25m, new DateTime(2024, 3, 5, 9, 15, 0, 0, DateTimeKind.Unspecified), "محمد أحمد صالح", "+201555123456", 82.75m },
                    { 4, (byte)22, new DateTime(2024, 3, 20, 16, 45, 0, 0, DateTimeKind.Unspecified), 165.75m, new DateTime(2024, 3, 20, 16, 45, 0, 0, DateTimeKind.Unspecified), "مريم عبد الله", "01012345678", 55.30m },
                    { 5, (byte)42, new DateTime(2024, 4, 12, 11, 30, 0, 0, DateTimeKind.Unspecified), 178.00m, new DateTime(2024, 4, 12, 11, 30, 0, 0, DateTimeKind.Unspecified), "خالد إبراهيم", "+201777888999", 88.00m }
                });

            migrationBuilder.InsertData(
                schema: "dbo",
                table: "tblLocation",
                columns: new[] { "Id", "City", "Country", "CreatedDate", "CustomerId", "IsHouse", "ModifiedDate", "Street" },
                values: new object[] { 1, "القاهرة", "مصر", new DateTime(2024, 1, 15, 10, 35, 0, 0, DateTimeKind.Unspecified), 1, true, new DateTime(2024, 1, 15, 10, 35, 0, 0, DateTimeKind.Unspecified), "شارع النصر، مدينة نصر" });

            migrationBuilder.InsertData(
                schema: "dbo",
                table: "tblLocation",
                columns: new[] { "Id", "City", "Country", "CreatedDate", "CustomerId", "ModifiedDate", "Street" },
                values: new object[,]
                {
                    { 2, "الإسكندرية", "مصر", new DateTime(2024, 2, 10, 14, 25, 0, 0, DateTimeKind.Unspecified), 2, new DateTime(2024, 2, 10, 14, 25, 0, 0, DateTimeKind.Unspecified), "شارع الحرية، محطة الرمل" },
                    { 3, "الجيزة", "مصر", new DateTime(2024, 3, 5, 9, 20, 0, 0, DateTimeKind.Unspecified), 3, new DateTime(2024, 3, 5, 9, 20, 0, 0, DateTimeKind.Unspecified), "شارع الهرم، الطالبية" }
                });

            migrationBuilder.InsertData(
                schema: "dbo",
                table: "tblLocation",
                columns: new[] { "Id", "City", "Country", "CreatedDate", "CustomerId", "IsHouse", "ModifiedDate", "Street" },
                values: new object[] { 4, "القاهرة", "مصر", new DateTime(2024, 3, 20, 16, 50, 0, 0, DateTimeKind.Unspecified), 4, true, new DateTime(2024, 3, 20, 16, 50, 0, 0, DateTimeKind.Unspecified), "شارع التحرير، وسط البلد" });

            migrationBuilder.InsertData(
                schema: "dbo",
                table: "tblLocation",
                columns: new[] { "Id", "City", "Country", "CreatedDate", "CustomerId", "ModifiedDate", "Street" },
                values: new object[] { 5, "المنصورة", "مصر", new DateTime(2024, 4, 12, 11, 35, 0, 0, DateTimeKind.Unspecified), 5, new DateTime(2024, 4, 12, 11, 35, 0, 0, DateTimeKind.Unspecified), "شارع الجمهورية، وسط المدينة" });

            migrationBuilder.InsertData(
                schema: "dbo",
                table: "tblPaymentStatus",
                columns: new[] { "Id", "AmountPaid", "AmountRemaining", "CreatedDate", "CustomerId", "ModifiedDate", "PaymentDate" },
                values: new object[,]
                {
                    { 1, 5000.00m, 15000.00m, new DateTime(2024, 1, 20, 14, 30, 0, 0, DateTimeKind.Unspecified), 1, new DateTime(2024, 1, 20, 14, 30, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 20, 14, 30, 0, 0, DateTimeKind.Unspecified) },
                    { 2, 3000.00m, 12000.00m, new DateTime(2024, 2, 15, 10, 15, 0, 0, DateTimeKind.Unspecified), 1, new DateTime(2024, 2, 15, 10, 15, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 2, 15, 10, 15, 0, 0, DateTimeKind.Unspecified) },
                    { 3, 8500.50m, 6500.50m, new DateTime(2024, 2, 25, 16, 20, 0, 0, DateTimeKind.Unspecified), 2, new DateTime(2024, 2, 25, 16, 20, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 2, 25, 16, 20, 0, 0, DateTimeKind.Unspecified) },
                    { 4, 12000.75m, 8000.25m, new DateTime(2024, 3, 10, 11, 45, 0, 0, DateTimeKind.Unspecified), 3, new DateTime(2024, 3, 10, 11, 45, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 3, 10, 11, 45, 0, 0, DateTimeKind.Unspecified) },
                    { 5, 2500.00m, 7500.00m, new DateTime(2024, 3, 25, 13, 30, 0, 0, DateTimeKind.Unspecified), 4, new DateTime(2024, 3, 25, 13, 30, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 3, 25, 13, 30, 0, 0, DateTimeKind.Unspecified) },
                    { 6, 18000.00m, 0.00m, new DateTime(2024, 4, 15, 15, 45, 0, 0, DateTimeKind.Unspecified), 5, new DateTime(2024, 4, 15, 15, 45, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 4, 15, 15, 45, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                schema: "dbo",
                table: "tblBuilding",
                columns: new[] { "Id", "ApartmentNumber", "BuildingNumber", "CreatedDate", "Floor", "LocationId", "ModifiedDate" },
                values: new object[,]
                {
                    { 1, 8, 15, new DateTime(2024, 2, 10, 14, 30, 0, 0, DateTimeKind.Unspecified), (byte)3, 2, new DateTime(2024, 2, 10, 14, 30, 0, 0, DateTimeKind.Unspecified) },
                    { 2, 5, 88, new DateTime(2024, 3, 5, 9, 25, 0, 0, DateTimeKind.Unspecified), (byte)7, 3, new DateTime(2024, 3, 5, 9, 25, 0, 0, DateTimeKind.Unspecified) },
                    { 3, 4, 33, new DateTime(2024, 4, 12, 11, 40, 0, 0, DateTimeKind.Unspecified), (byte)2, 5, new DateTime(2024, 4, 12, 11, 40, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                schema: "dbo",
                table: "tblHouse",
                columns: new[] { "Id", "CreatedDate", "HouseNumber", "LocationId", "ModifiedDate" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 15, 10, 40, 0, 0, DateTimeKind.Unspecified), 25, 1, new DateTime(2024, 1, 15, 10, 40, 0, 0, DateTimeKind.Unspecified) },
                    { 2, new DateTime(2024, 3, 20, 16, 55, 0, 0, DateTimeKind.Unspecified), 142, 4, new DateTime(2024, 3, 20, 16, 55, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_tblPaymentStatus_AmountRemaining",
                schema: "dbo",
                table: "tblPaymentStatus",
                column: "AmountRemaining");

            migrationBuilder.CreateIndex(
                name: "IX_tblPaymentStatus_Customer_Date",
                schema: "dbo",
                table: "tblPaymentStatus",
                columns: new[] { "CustomerId", "PaymentDate" });

            migrationBuilder.AddCheckConstraint(
                name: "CK_tblPaymentStatus_AmountPaid",
                schema: "dbo",
                table: "tblPaymentStatus",
                sql: "[AmountPaid] >= 1.00");

            migrationBuilder.AddCheckConstraint(
                name: "CK_tblPaymentStatus_PaymentDate",
                schema: "dbo",
                table: "tblPaymentStatus",
                sql: "[PaymentDate] <= GETDATE()");

            migrationBuilder.AddCheckConstraint(
                name: "CK_tblPaymentStatus_TotalAmount",
                schema: "dbo",
                table: "tblPaymentStatus",
                sql: "[AmountPaid] + [AmountRemaining] >= 1.00");

            migrationBuilder.CreateIndex(
                name: "IX_tblLocation_IsHouse",
                schema: "dbo",
                table: "tblLocation",
                column: "IsHouse");

            migrationBuilder.AddCheckConstraint(
                name: "CK_tblLocation_City",
                schema: "dbo",
                table: "tblLocation",
                sql: "LEN(LTRIM(RTRIM([City]))) >= 2 AND [City] NOT LIKE '%[0-9]%'");

            migrationBuilder.AddCheckConstraint(
                name: "CK_tblLocation_Country",
                schema: "dbo",
                table: "tblLocation",
                sql: "LEN(LTRIM(RTRIM([Country]))) >= 2");

            migrationBuilder.AddCheckConstraint(
                name: "CK_tblLocation_Street",
                schema: "dbo",
                table: "tblLocation",
                sql: "LEN(LTRIM(RTRIM([Street]))) >= 5");

            migrationBuilder.AddCheckConstraint(
                name: "CK_tblLocation_Street_Content",
                schema: "dbo",
                table: "tblLocation",
                sql: "[Street] LIKE '%شارع%' OR [Street] LIKE '%ش.%' OR [Street] LIKE '%Street%' OR [Street] LIKE '%St.%' OR [Street] LIKE '%Avenue%' OR [Street] LIKE '%Road%' OR [Street] LIKE '%طريق%' OR LEN([Street]) >= 10");

            migrationBuilder.CreateIndex(
                name: "IX_tblHouse_HouseNumber",
                schema: "dbo",
                table: "tblHouse",
                column: "HouseNumber");

            migrationBuilder.AddCheckConstraint(
                name: "CK_tblHouse_HouseNumber",
                schema: "dbo",
                table: "tblHouse",
                sql: "[HouseNumber] BETWEEN 1 AND 9999");

            migrationBuilder.CreateIndex(
                name: "IX_tblCustomer_Age",
                schema: "dbo",
                table: "tblCustomer",
                column: "Age");

            migrationBuilder.CreateIndex(
                name: "IX_tblCustomer_PhoneNumber",
                schema: "dbo",
                table: "tblCustomer",
                column: "PhoneNumber",
                unique: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_tblCustomer_Age",
                schema: "dbo",
                table: "tblCustomer",
                sql: "[Age] BETWEEN 16 AND 120");

            migrationBuilder.AddCheckConstraint(
                name: "CK_tblCustomer_BMI_Logic",
                schema: "dbo",
                table: "tblCustomer",
                sql: "([Weight] / POWER([Height]/100.0, 2)) BETWEEN 10.0 AND 60.0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_tblCustomer_Height",
                schema: "dbo",
                table: "tblCustomer",
                sql: "[Height] BETWEEN 100.0 AND 250.0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_tblCustomer_Name",
                schema: "dbo",
                table: "tblCustomer",
                sql: "LEN(LTRIM(RTRIM([Name]))) >= 2 AND [Name] NOT LIKE '%[0-9]%'");

            migrationBuilder.AddCheckConstraint(
                name: "CK_tblCustomer_Phone",
                schema: "dbo",
                table: "tblCustomer",
                sql: "LEN([PhoneNumber]) >= 10 AND LEN([PhoneNumber]) <= 15");

            migrationBuilder.AddCheckConstraint(
                name: "CK_tblCustomer_Weight",
                schema: "dbo",
                table: "tblCustomer",
                sql: "[Weight] BETWEEN 20.0 AND 500.0");

            migrationBuilder.CreateIndex(
                name: "IX_tblBuilding_Complete_Address",
                schema: "dbo",
                table: "tblBuilding",
                columns: new[] { "BuildingNumber", "Floor", "ApartmentNumber" });

            migrationBuilder.AddCheckConstraint(
                name: "CK_tblBuilding_ApartmentNumber",
                schema: "dbo",
                table: "tblBuilding",
                sql: "[ApartmentNumber] BETWEEN 1 AND 999");

            migrationBuilder.AddCheckConstraint(
                name: "CK_tblBuilding_BuildingNumber",
                schema: "dbo",
                table: "tblBuilding",
                sql: "[BuildingNumber] BETWEEN 1 AND 9999");

            migrationBuilder.AddCheckConstraint(
                name: "CK_tblBuilding_Floor",
                schema: "dbo",
                table: "tblBuilding",
                sql: "[Floor] BETWEEN 0 AND 50");

            migrationBuilder.AddCheckConstraint(
                name: "CK_tblBuilding_Floor_Apartment_Logic",
                schema: "dbo",
                table: "tblBuilding",
                sql: "([Floor] = 0 AND [ApartmentNumber] BETWEEN 1 AND 20) OR ([Floor] > 0 AND [ApartmentNumber] BETWEEN 1 AND 10)");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                schema: "dbo",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                schema: "dbo",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                schema: "dbo",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                schema: "dbo",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                schema: "dbo",
                table: "Roles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                schema: "dbo",
                table: "Users",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                schema: "dbo",
                table: "Users",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Roles",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "dbo");

            migrationBuilder.DropIndex(
                name: "IX_tblPaymentStatus_AmountRemaining",
                schema: "dbo",
                table: "tblPaymentStatus");

            migrationBuilder.DropIndex(
                name: "IX_tblPaymentStatus_Customer_Date",
                schema: "dbo",
                table: "tblPaymentStatus");

            migrationBuilder.DropCheckConstraint(
                name: "CK_tblPaymentStatus_AmountPaid",
                schema: "dbo",
                table: "tblPaymentStatus");

            migrationBuilder.DropCheckConstraint(
                name: "CK_tblPaymentStatus_PaymentDate",
                schema: "dbo",
                table: "tblPaymentStatus");

            migrationBuilder.DropCheckConstraint(
                name: "CK_tblPaymentStatus_TotalAmount",
                schema: "dbo",
                table: "tblPaymentStatus");

            migrationBuilder.DropIndex(
                name: "IX_tblLocation_IsHouse",
                schema: "dbo",
                table: "tblLocation");

            migrationBuilder.DropCheckConstraint(
                name: "CK_tblLocation_City",
                schema: "dbo",
                table: "tblLocation");

            migrationBuilder.DropCheckConstraint(
                name: "CK_tblLocation_Country",
                schema: "dbo",
                table: "tblLocation");

            migrationBuilder.DropCheckConstraint(
                name: "CK_tblLocation_Street",
                schema: "dbo",
                table: "tblLocation");

            migrationBuilder.DropCheckConstraint(
                name: "CK_tblLocation_Street_Content",
                schema: "dbo",
                table: "tblLocation");

            migrationBuilder.DropIndex(
                name: "IX_tblHouse_HouseNumber",
                schema: "dbo",
                table: "tblHouse");

            migrationBuilder.DropCheckConstraint(
                name: "CK_tblHouse_HouseNumber",
                schema: "dbo",
                table: "tblHouse");

            migrationBuilder.DropIndex(
                name: "IX_tblCustomer_Age",
                schema: "dbo",
                table: "tblCustomer");

            migrationBuilder.DropIndex(
                name: "IX_tblCustomer_PhoneNumber",
                schema: "dbo",
                table: "tblCustomer");

            migrationBuilder.DropCheckConstraint(
                name: "CK_tblCustomer_Age",
                schema: "dbo",
                table: "tblCustomer");

            migrationBuilder.DropCheckConstraint(
                name: "CK_tblCustomer_BMI_Logic",
                schema: "dbo",
                table: "tblCustomer");

            migrationBuilder.DropCheckConstraint(
                name: "CK_tblCustomer_Height",
                schema: "dbo",
                table: "tblCustomer");

            migrationBuilder.DropCheckConstraint(
                name: "CK_tblCustomer_Name",
                schema: "dbo",
                table: "tblCustomer");

            migrationBuilder.DropCheckConstraint(
                name: "CK_tblCustomer_Phone",
                schema: "dbo",
                table: "tblCustomer");

            migrationBuilder.DropCheckConstraint(
                name: "CK_tblCustomer_Weight",
                schema: "dbo",
                table: "tblCustomer");

            migrationBuilder.DropIndex(
                name: "IX_tblBuilding_Complete_Address",
                schema: "dbo",
                table: "tblBuilding");

            migrationBuilder.DropCheckConstraint(
                name: "CK_tblBuilding_ApartmentNumber",
                schema: "dbo",
                table: "tblBuilding");

            migrationBuilder.DropCheckConstraint(
                name: "CK_tblBuilding_BuildingNumber",
                schema: "dbo",
                table: "tblBuilding");

            migrationBuilder.DropCheckConstraint(
                name: "CK_tblBuilding_Floor",
                schema: "dbo",
                table: "tblBuilding");

            migrationBuilder.DropCheckConstraint(
                name: "CK_tblBuilding_Floor_Apartment_Logic",
                schema: "dbo",
                table: "tblBuilding");

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "tblBuilding",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "tblBuilding",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "tblBuilding",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "tblHouse",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "tblHouse",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "tblPaymentStatus",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "tblPaymentStatus",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "tblPaymentStatus",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "tblPaymentStatus",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "tblPaymentStatus",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "tblPaymentStatus",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "tblLocation",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "tblLocation",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "tblLocation",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "tblLocation",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "tblLocation",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "tblCustomer",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "tblCustomer",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "tblCustomer",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "tblCustomer",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "tblCustomer",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.AlterColumn<DateTime>(
                name: "PaymentDate",
                schema: "dbo",
                table: "tblPaymentStatus",
                type: "datetime2(7)",
                nullable: false,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2(7)",
                oldDefaultValueSql: "GETDATE()",
                oldComment: "Date of payment transaction");

            migrationBuilder.AlterColumn<decimal>(
                name: "AmountRemaining",
                schema: "dbo",
                table: "tblPaymentStatus",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldComment: "Remaining amount to be paid");

            migrationBuilder.AlterColumn<decimal>(
                name: "AmountPaid",
                schema: "dbo",
                table: "tblPaymentStatus",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldComment: "Amount already paid by customer");

            migrationBuilder.AlterColumn<string>(
                name: "Street",
                schema: "dbo",
                table: "tblLocation",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(300)",
                oldMaxLength: 300,
                oldComment: "Street address details");

            migrationBuilder.AlterColumn<bool>(
                name: "IsHouse",
                schema: "dbo",
                table: "tblLocation",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false,
                oldComment: "True for house, False for apartment building");

            migrationBuilder.AlterColumn<string>(
                name: "Country",
                schema: "dbo",
                table: "tblLocation",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "مصر",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldDefaultValue: "مصر",
                oldComment: "Country name (default: Egypt)");

            migrationBuilder.AlterColumn<string>(
                name: "City",
                schema: "dbo",
                table: "tblLocation",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldComment: "City or governorate name");

            migrationBuilder.AlterColumn<int>(
                name: "HouseNumber",
                schema: "dbo",
                table: "tblHouse",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "House number on the street");

            migrationBuilder.AlterColumn<decimal>(
                name: "Weight",
                schema: "dbo",
                table: "tblCustomer",
                type: "decimal(5,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)",
                oldComment: "Weight in kilograms");

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                schema: "dbo",
                table: "tblCustomer",
                type: "nvarchar(25)",
                maxLength: 25,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(25)",
                oldMaxLength: 25,
                oldComment: "Phone number in Egyptian format");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "dbo",
                table: "tblCustomer",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldComment: "Full name of the customer");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                schema: "dbo",
                table: "tblCustomer",
                type: "datetime2(7)",
                nullable: false,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2(7)",
                oldDefaultValueSql: "GETDATE()",
                oldComment: "Last modification timestamp");

            migrationBuilder.AlterColumn<decimal>(
                name: "Height",
                schema: "dbo",
                table: "tblCustomer",
                type: "decimal(5,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)",
                oldComment: "Height in centimeters");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                schema: "dbo",
                table: "tblCustomer",
                type: "datetime2(7)",
                nullable: false,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2(7)",
                oldDefaultValueSql: "GETDATE()",
                oldComment: "Record creation timestamp");

            migrationBuilder.AlterColumn<byte>(
                name: "Age",
                schema: "dbo",
                table: "tblCustomer",
                type: "tinyint",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint",
                oldComment: "Age in years (must be between 16-120)");

            migrationBuilder.AlterColumn<byte>(
                name: "Floor",
                schema: "dbo",
                table: "tblBuilding",
                type: "tinyint",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint",
                oldComment: "Floor number (0=Ground, 1-200=Upper floors)");

            migrationBuilder.AlterColumn<int>(
                name: "BuildingNumber",
                schema: "dbo",
                table: "tblBuilding",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Building number on the street");

            migrationBuilder.AlterColumn<int>(
                name: "ApartmentNumber",
                schema: "dbo",
                table: "tblBuilding",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Apartment number on the floor");

            migrationBuilder.AddCheckConstraint(
                name: "CK_tblPaymentStatus_AmountPaid",
                schema: "dbo",
                table: "tblPaymentStatus",
                sql: "[AmountPaid] >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_tblPaymentStatus_TotalAmount",
                schema: "dbo",
                table: "tblPaymentStatus",
                sql: "[AmountPaid] + [AmountRemaining] > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_tblLocation_City",
                schema: "dbo",
                table: "tblLocation",
                sql: "LEN(LTRIM(RTRIM([City]))) >= 2");

            migrationBuilder.AddCheckConstraint(
                name: "CK_tblLocation_Country",
                schema: "dbo",
                table: "tblLocation",
                sql: "LEN(LTRIM(RTRIM([Country]))) > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_tblLocation_Street",
                schema: "dbo",
                table: "tblLocation",
                sql: "LEN(LTRIM(RTRIM([Street]))) >= 3");

            migrationBuilder.AddCheckConstraint(
                name: "CK_tblHouse_HouseNumber",
                schema: "dbo",
                table: "tblHouse",
                sql: "[HouseNumber] > 0 AND [HouseNumber] <= 999999");

            migrationBuilder.AddCheckConstraint(
                name: "CK_tblCustomer_Age",
                schema: "dbo",
                table: "tblCustomer",
                sql: "[Age] BETWEEN 1 AND 120");

            migrationBuilder.AddCheckConstraint(
                name: "CK_tblCustomer_Height",
                schema: "dbo",
                table: "tblCustomer",
                sql: "[Height] BETWEEN 50.0 AND 250.0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_tblCustomer_Name",
                schema: "dbo",
                table: "tblCustomer",
                sql: "LEN(LTRIM(RTRIM([Name]))) >= 2");

            migrationBuilder.AddCheckConstraint(
                name: "CK_tblCustomer_Phone",
                schema: "dbo",
                table: "tblCustomer",
                sql: "[PhoneNumber] LIKE '+20[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]' OR [PhoneNumber] LIKE '+20[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]' OR [PhoneNumber] LIKE '01[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]' OR [PhoneNumber] LIKE '0[2-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]'");

            migrationBuilder.AddCheckConstraint(
                name: "CK_tblCustomer_Weight",
                schema: "dbo",
                table: "tblCustomer",
                sql: "[Weight] BETWEEN 2.0 AND 300.0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_tblBuilding_ApartmentNumber",
                schema: "dbo",
                table: "tblBuilding",
                sql: "[ApartmentNumber] > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_tblBuilding_BuildingNumber",
                schema: "dbo",
                table: "tblBuilding",
                sql: "[BuildingNumber] > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_tblBuilding_Floor",
                schema: "dbo",
                table: "tblBuilding",
                sql: "[Floor] BETWEEN 0 AND 200");
        }
    }
}
