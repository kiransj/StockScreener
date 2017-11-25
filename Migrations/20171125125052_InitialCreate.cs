using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace screener.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "companyInformation",
                columns: table => new
                {
                    symbol = table.Column<string>(nullable: false),
                    series = table.Column<string>(nullable: false),
                    isinNumber = table.Column<string>(nullable: false),
                    companyName = table.Column<string>(nullable: false),
                    dateOfListing = table.Column<DateTime>(nullable: false),
                    faceValue = table.Column<float>(nullable: false),
                    marketLot = table.Column<int>(nullable: false),
                    paidUpvalue = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_companyInformation", x => new { x.symbol, x.series, x.isinNumber });
                });

            migrationBuilder.CreateTable(
                name: "stockData",
                columns: table => new
                {
                    isinNumber = table.Column<string>(nullable: false),
                    date = table.Column<DateTime>(nullable: false),
                    series = table.Column<string>(nullable: false),
                    close = table.Column<float>(nullable: false),
                    deliverableQty = table.Column<long>(nullable: false),
                    deliveryPercentage = table.Column<float>(nullable: false),
                    high = table.Column<float>(nullable: false),
                    lastPrice = table.Column<float>(nullable: false),
                    low = table.Column<float>(nullable: false),
                    open = table.Column<float>(nullable: false),
                    prevClose = table.Column<float>(nullable: false),
                    symbol = table.Column<string>(nullable: false),
                    totalTradedQty = table.Column<long>(nullable: false),
                    totalTradedValue = table.Column<float>(nullable: false),
                    totalTrades = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stockData", x => new { x.isinNumber, x.date, x.series });
                });

            migrationBuilder.CreateIndex(
                name: "IX_companyInformation_symbol_isinNumber",
                table: "companyInformation",
                columns: new[] { "symbol", "isinNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_stockData_symbol_isinNumber_date_series",
                table: "stockData",
                columns: new[] { "symbol", "isinNumber", "date", "series" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "companyInformation");

            migrationBuilder.DropTable(
                name: "stockData");
        }
    }
}
