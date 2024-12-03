using Lemoo_pos.Common.Enums;
using Lemoo_pos.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lemoo_pos.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {

        public DbSet<Account> Accounts { get; set; }
        public DbSet<AccountAuthority> AccountAuthorities { get; set; }
        public DbSet<Authority> Authorities { get; set; } 
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<AuthorityPermission> AuthorityPermissions { get; set; } 
        public DbSet<Store> Stores { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductVariantAttribute> ProductVariantAttributes { get; set; }
        public DbSet<ProductVariant> ProductVariants { get; set; }
        public DbSet<ProductAttributeValue> ProductAttributeValues { get; set; }
        public DbSet<ProductAttribute> ProductAttributes { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<InventoryLog> InventoryLogs { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }


        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Permission>()
                .Property(p => p.Type)
                .HasConversion(
                    v => v.ToString(),  
                    v => (PermissionType)Enum.Parse(typeof(PermissionType), v) 
                );


            modelBuilder
               .Entity<AuthorityPermission>()
               .HasOne(ap => ap.Authority)
               .WithMany(a => a.Permissions)
               .HasForeignKey(ap => ap.AuthorityId);

        }

        public override int SaveChanges()
        {
            AddTimestamps();
            return base.SaveChanges();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder
            //    .LogTo(Console.WriteLine, LogLevel.Error)
            //    .EnableSensitiveDataLogging()
            //    .EnableDetailedErrors()
            //    ;
        }


        private void AddTimestamps()
        {
            var entities = ChangeTracker.Entries()
                .Where(x => x.Entity is BaseEntity && (x.State == EntityState.Added || x.State == EntityState.Modified));

            foreach (var entity in entities)
            {
                var now = DateTime.UtcNow; // current datetime

                if (entity.State == EntityState.Added)
                {
                    BaseEntity e = (BaseEntity)entity.Entity;

                    e.CreatedAt = now;
                }
               ((BaseEntity)entity.Entity).UpdatedAt = now;
            }
        }
    }
}