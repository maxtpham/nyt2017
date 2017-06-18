using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace nyt.core.users.Migrations
{
    public partial class user : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false)
                        .Annotation("MySql:ValueGeneratedOnAdd", true),
                    Description = table.Column<string>(maxLength: 2147483647, nullable: true),
                    Name = table.Column<string>(maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
