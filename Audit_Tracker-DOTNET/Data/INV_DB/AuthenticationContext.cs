using AAP_Authentication;
using Microsoft.EntityFrameworkCore;

namespace Data.INV_DB
{
    public class AuthenticationContext : DbContext
    {
        public AuthenticationContext(DbContextOptions<AuthenticationContext> options) : base(options)
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


        public  DbSet<Roles> Roles { get; set; }
        public DbSet<Audit_Admins> Admins { get; set; }



    }




}
