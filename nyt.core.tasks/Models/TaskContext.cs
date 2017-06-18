using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace nyt.core.tasks.Models
{
    public partial class TaskContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseMySql(@"Server=localhost;User Id=root;Password=123456;Database=nyttask");
        }

        public DbSet<task> tasks { get; set; }
    }
}