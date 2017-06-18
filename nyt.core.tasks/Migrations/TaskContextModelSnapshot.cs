using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using nyt.core.tasks.Models;

namespace nyt.core.tasks.Migrations
{
    [DbContext(typeof(TaskContext))]
    partial class TaskContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2");

            modelBuilder.Entity("nyt.core.tasks.Models.task", b =>
                {
                    b.Property<Guid>("id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid?>("assignee");

                    b.Property<string>("code")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("content")
                        .HasMaxLength(2147483647);

                    b.Property<DateTime>("created");

                    b.Property<Guid?>("creator");

                    b.Property<string>("title")
                        .IsRequired()
                        .HasMaxLength(256);

                    b.Property<DateTime>("updated");

                    b.HasKey("id");

                    b.ToTable("tasks");
                });
        }
    }
}
