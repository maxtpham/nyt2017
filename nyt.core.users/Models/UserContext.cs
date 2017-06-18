using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace nyt.core.users.Models
{
    public partial class UserContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseMySql(@"Server=localhost;User Id=root;Password=123456;Database=nyt00");
        }

        public DbSet<User> Users { get; set; }
    }
}