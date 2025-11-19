using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ItbApi.Migrations
{
    /// <inheritdoc />
    public partial class DefaultData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ReviewTypes",
                columns: new[] { "Id", "Abbr", "Accepted", "DateAdded", "IpAddress", "Name" },
                values: new object[,]
                {
                    { 1, "BD", true, new DateTime(2025, 11, 19, 20, 6, 22, 318, DateTimeKind.Utc).AddTicks(8090), "System", "Bottle Design" },
                    { 2, "LIG", true, new DateTime(2025, 11, 19, 20, 6, 22, 318, DateTimeKind.Utc).AddTicks(8110), "System", "Look in Glass" },
                    { 3, "SM / A", true, new DateTime(2025, 11, 19, 20, 6, 22, 318, DateTimeKind.Utc).AddTicks(8110), "System", "Smell / Aroma" },
                    { 4, "TST", true, new DateTime(2025, 11, 19, 20, 6, 22, 318, DateTimeKind.Utc).AddTicks(8110), "System", "Taste" },
                    { 5, "MF", true, new DateTime(2025, 11, 19, 20, 6, 22, 318, DateTimeKind.Utc).AddTicks(8130), "System", "Mouthfeel" },
                    { 6, "AT", true, new DateTime(2025, 11, 19, 20, 6, 22, 318, DateTimeKind.Utc).AddTicks(8130), "System", "After Taste" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ReviewTypes",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ReviewTypes",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ReviewTypes",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "ReviewTypes",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "ReviewTypes",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "ReviewTypes",
                keyColumn: "Id",
                keyValue: 6);
        }
    }
}
