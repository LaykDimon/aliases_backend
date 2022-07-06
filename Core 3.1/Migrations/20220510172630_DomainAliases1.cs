using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Core_3._1.Migrations
{
    public partial class DomainAliases1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "DomainAliases",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "DomainAliases",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string));
        }
    }
}
