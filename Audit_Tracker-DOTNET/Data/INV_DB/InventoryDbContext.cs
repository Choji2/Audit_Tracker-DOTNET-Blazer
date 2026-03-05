using Microsoft.EntityFrameworkCore;
using Models.DB_objects;

namespace Audit_Tracker_Blazor.Data.INV_DB
{
    public class InventoryDbContext : DbContext
    {


        public InventoryDbContext(DbContextOptions<InventoryDbContext> options):base(options)
        {
            
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var allEntities = modelBuilder.Model.GetEntityTypes();

            foreach (var entity in allEntities)
            {
                try
                {
                    entity.AddProperty("CreatedDate", typeof(DateTime));
                    entity.AddProperty("UpdatedDate", typeof(DateTime));

                }
                catch (Exception ex)
                {
                    //Console.WriteLine(ex.ToString());
                }

            }
        }
        public override int SaveChanges()
        {
            var entries = ChangeTracker
                .Entries()
                .Where(e =>
                        e.State == EntityState.Added
                        || e.State == EntityState.Modified);

            foreach (var entityEntry in entries)
            {
                entityEntry.Property("UpdatedDate").CurrentValue = DateTime.Now;

                if (entityEntry.State == EntityState.Added)
                {
                    entityEntry.Property("CreatedDate").CurrentValue = DateTime.Now;
                }

            }

            return base.SaveChanges();

        }


        public DbSet<Divisions> Divisions { get; set; }
        public DbSet<Div_Zones> Div_Zones { get; set; }
        public DbSet<Inventories> Inventories { get; set; }
        public DbSet<Inventory_Records> inventory_Records { get; set; }

    }
}
