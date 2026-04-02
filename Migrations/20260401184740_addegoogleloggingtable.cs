using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BucketSurvey.Api.Migrations
{
    /// <inheritdoc />
    public partial class addegoogleloggingtable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "82e8f39a-175a-4b24-9c95-c2f7732e2f71",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEPnMoc/HjWHPwj8c8peU97xo9Ja6BO1BAhP0Zd/JkS3ULf7Gk+WqJzgIXlgM7Xr+Cg==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "82e8f39a-175a-4b24-9c95-c2f7732e2f71",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAECTPTqRpnf3bjeClzHGjqvu/EWA4SLlgXMz6OPmInKSvLNseDzTcNjyodXMJsl4GkQ==");
        }
    }
}
