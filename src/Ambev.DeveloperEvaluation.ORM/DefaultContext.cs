using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace Ambev.DeveloperEvaluation.ORM;

public class DefaultContext : DbContext
{
    private readonly IConfiguration? _configuration;
    public DbSet<User> Users { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Sale> Sales { get; set; }
    public DbSet<ProductSale> ProductSales { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultContext"/> class.
    /// This constructor accepts options for configuring the DbContext and an optional IConfiguration 
    /// for accessing configuration settings. It automatically applies any pending database migrations 
    /// and seeds the database with initial data by calling the <see cref="DataSeed.Seed"/> method.
    /// </summary>
    /// <param name="options">The DbContext options used to configure the context.</param>
    /// <param name="configuration">An optional configuration instance for accessing application settings.</param>
    public DefaultContext(DbContextOptions<DefaultContext> options, IConfiguration? configuration = null) : base(options)
    {
        _configuration = configuration;
        Database.Migrate();
        DataSeed.Seed(this);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var connectionString = _configuration?.GetConnectionString("DefaultConnection");
            optionsBuilder.UseNpgsql(connectionString, b => b.MigrationsAssembly("Ambev.DeveloperEvaluation.ORM"));

        }
    }
}
public class YourDbContextFactory : IDesignTimeDbContextFactory<DefaultContext>
{
    public DefaultContext CreateDbContext(string[] args)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var builder = new DbContextOptionsBuilder<DefaultContext>();
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        builder.UseNpgsql(
               connectionString,
               b => b.MigrationsAssembly("Ambev.DeveloperEvaluation.ORM")
        );

        return new DefaultContext(builder.Options);
    }
}