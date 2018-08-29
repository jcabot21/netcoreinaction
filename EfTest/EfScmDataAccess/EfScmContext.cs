using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace EfScmDataAccess
{
    public class EfScmContext : DbContext
    {
        public DbSet<PartType> Parts { get; set; }
        public DbSet<InventoryItem> Inventory { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=efscm.db");
        }
    }
}