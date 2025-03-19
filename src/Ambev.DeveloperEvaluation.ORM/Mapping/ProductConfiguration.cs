﻿using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).HasColumnType("uuid").HasDefaultValueSql("gen_random_uuid()");

        builder.Property(p => p.Name).IsRequired().HasMaxLength(50);
        builder.Property(p => p.Description).HasMaxLength(100);
        builder.Property(p => p.Price).IsRequired().HasPrecision(10, 2);
        builder.Property(p => p.Quantity).IsRequired().HasDefaultValue(100);
        builder.Property(p => p.Status).IsRequired().HasDefaultValue(ProductStatus.Active);

    }
}