using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using System.Linq;

namespace Ambev.DeveloperEvaluation.ORM
{
    public static class DataSeed
    {
        public static void Seed(DefaultContext context)
        {
            if (!context.Users.Any())
            {
                context.Users.AddRange(
                    new User
                    {
                        Id = Guid.NewGuid(),
                        Username = "AdminUser",
                        Password = "123QWE",
                        Email = "admin@company.com",
                        Phone = "1234567890",
                        Status = UserStatus.Active,
                        Role = UserRole.Admin
                    },
                    new User
                    {
                        Id = Guid.NewGuid(),
                        Username = "CustomerUser",
                        Password = "123QWE",
                        Email = "customer@company.com",
                        Phone = "9876543210",
                        Status = UserStatus.Active,
                        Role = UserRole.Customer
                    },
                    new User
                    {
                        Id = Guid.NewGuid(),
                        Username = "ManagerUser",
                        Password = "123QWE",
                        Email = "manager@company.com",
                        Phone = "1122334455",
                        Status = UserStatus.Active,
                        Role = UserRole.Manager
                    }
                );
                context.SaveChanges();
            }

            if (!context.Products.Any())
            {
                context.Products.AddRange(
                    new Product { Id = Guid.NewGuid(), Name = "Product 1", Description = "Product 1 description", Price = 10.00m, Quantity = 100, Status = ProductStatus.Active },
                    new Product { Id = Guid.NewGuid(), Name = "Product 2", Description = "Product 2 description", Price = 20.00m, Quantity = 100, Status = ProductStatus.Active },
                    new Product { Id = Guid.NewGuid(), Name = "Product 3", Description = "Product 3 description", Price = 30.00m, Quantity = 100, Status = ProductStatus.Active },
                    new Product { Id = Guid.NewGuid(), Name = "Product 4", Description = "Product 4 description", Price = 40.00m, Quantity = 100, Status = ProductStatus.Active },
                    new Product { Id = Guid.NewGuid(), Name = "Product 5", Description = "Product 5 description", Price = 50.00m, Quantity = 100, Status = ProductStatus.Active },
                    new Product { Id = Guid.NewGuid(), Name = "Product 6", Description = "Product 6 description", Price = 60.00m, Quantity = 100, Status = ProductStatus.Active },
                    new Product { Id = Guid.NewGuid(), Name = "Product 7", Description = "Product 7 description", Price = 70.00m, Quantity = 100, Status = ProductStatus.Active },
                    new Product { Id = Guid.NewGuid(), Name = "Product 8", Description = "Product 8 description", Price = 80.00m, Quantity = 100, Status = ProductStatus.Active },
                    new Product { Id = Guid.NewGuid(), Name = "Product 9", Description = "Product 9 description", Price = 90.00m, Quantity = 100, Status = ProductStatus.Active },
                    new Product { Id = Guid.NewGuid(), Name = "Product 10", Description = "Product 10 description", Price = 100.00m, Quantity = 100, Status = ProductStatus.Active }
                );
                context.SaveChanges();
            }

        }
    }
}
