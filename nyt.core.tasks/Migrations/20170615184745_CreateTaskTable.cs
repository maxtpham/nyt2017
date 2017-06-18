using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace nyt.core.tasks.Migrations
{
    public partial class CreateTaskTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tasks",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false)
                        .Annotation("MySql:ValueGeneratedOnAdd", true),
                    assignee = table.Column<Guid>(nullable: true),
                    code = table.Column<string>(maxLength: 50, nullable: false),
                    content = table.Column<string>(maxLength: 2147483647, nullable: true),
                    created = table.Column<DateTime>(nullable: false),
                    creator = table.Column<Guid>(nullable: true),
                    title = table.Column<string>(maxLength: 256, nullable: false),
                    updated = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tasks", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tasks");
        }
    }
}
