using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BucketSurvey.Api.Migrations
{
    /// <inheritdoc />
    public partial class addDisabledPropToUserTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDisabled",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

           
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDisabled",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "82e8f39a-175a-4b24-9c95-c2f7732e2f71",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEO9M/NNU8obzzyKEbKxLuIhz4gdAvo15QBJt/SCr42WzO48/xZCy5elEe04Cfz5LPA==");
        }
    }
}
