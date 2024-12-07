using Lemoo_pos.Common.Enums;
using Lemoo_pos.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lemoo_pos.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {

        public required DbSet<Account> Accounts { get; set; }
        public required DbSet<AccountAuthority> AccountAuthorities { get; set; }
        public required DbSet<Authority> Authorities { get; set; }
        public required DbSet<Permission> Permissions { get; set; }
        public required DbSet<AuthorityPermission> AuthorityPermissions { get; set; }
        public required DbSet<Store> Stores { get; set; }
        public required DbSet<Branch> Branches { get; set; }
        public required DbSet<Staff> Staffs { get; set; }
        public required DbSet<Product> Products { get; set; }
        public required DbSet<ProductVariantAttribute> ProductVariantAttributes { get; set; }
        public required DbSet<ProductVariant> ProductVariants { get; set; }
        public required DbSet<ProductAttributeValue> ProductAttributeValues { get; set; }
        public required DbSet<ProductAttribute> ProductAttributes { get; set; }
        public required DbSet<ProductCategory> ProductCategories { get; set; }
        public required DbSet<Brand> Brands { get; set; }
        public required DbSet<Inventory> Inventories { get; set; }
        public required DbSet<InventoryLog> InventoryLogs { get; set; }
        public required DbSet<Customer> Customers { get; set; }
        public required DbSet<Order> Orders { get; set; }
        public required DbSet<OrderItem> OrderItems { get; set; }
        public required DbSet<Shift> Shifts { get; set; }



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