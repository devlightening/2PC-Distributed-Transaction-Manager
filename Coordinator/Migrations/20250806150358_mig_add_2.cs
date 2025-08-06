using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Coordinator.Migrations
{
    /// <inheritdoc />
    public partial class mig_add_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Nodes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("2232fc19-662f-465e-a11a-71aa3ea231d7"), "PaymentAPI" },
                    { new Guid("27eccb15-462e-41fe-9cf6-1c8326969bb5"), "OrderAPI" },
                    { new Guid("48889f4f-5374-409d-8643-0c034be239e4"), "StockAPI" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Nodes",
                keyColumn: "Id",
                keyValue: new Guid("2232fc19-662f-465e-a11a-71aa3ea231d7"));

            migrationBuilder.DeleteData(
                table: "Nodes",
                keyColumn: "Id",
                keyValue: new Guid("27eccb15-462e-41fe-9cf6-1c8326969bb5"));

            migrationBuilder.DeleteData(
                table: "Nodes",
                keyColumn: "Id",
                keyValue: new Guid("48889f4f-5374-409d-8643-0c034be239e4"));
        }
    }
}
