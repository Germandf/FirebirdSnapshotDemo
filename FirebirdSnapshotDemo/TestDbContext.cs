using Microsoft.EntityFrameworkCore;

public class TestDbContext : DbContext
{
    public DbSet<TestEntity> TestEntities => Set<TestEntity>();

    protected override void OnConfiguring(DbContextOptionsBuilder options) => options.UseFirebird(
        "User=sysdba;Password=masterkey;Database=localhost:/var/lib/firebird/data/snapshot_test.fdb");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TestEntity>(e =>
        {
            e.ToTable("TEST_ENTITY");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("ID").ValueGeneratedOnAdd();
            e.Property(x => x.Name).HasColumnName("NAME").HasMaxLength(50).IsRequired();
        });
    }
}

public class TestEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
}
