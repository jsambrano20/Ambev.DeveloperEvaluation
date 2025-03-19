using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Validation;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;

namespace Ambev.DeveloperEvaluation.Domain.Entities
{

    public class Product : BaseEntity
    {
        /// <summary>
        /// Gets or sets the product name.
        /// </summary>
        [Required(ErrorMessage = "The product name is required.")]
        [StringLength(100, ErrorMessage = "The product name must be at most 100 characters long.")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the product description.
        /// </summary>
        [StringLength(500, ErrorMessage = "The product description must be at most 500 characters long.")]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the product price.
        /// </summary>
        [Range(0.01, double.MaxValue, ErrorMessage = "The product price must be greater than zero.")]
        public decimal Price { get; set; }

        /// <summary>
        /// Gets or sets the product quantity.
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "The product quantity must be at least 1.")]
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or sets the product status.
        /// </summary>
        public ProductStatus Status { get; set; }

        /// <summary>
        /// Performs validation of the <see cref="Product" /> entity using the <see cref="ProductValidator" /> rules.
        /// </summary>
        /// <returns>
        /// A <see cref="ValidationResultDetail"/> containing:
        /// - IsValid: Indicates whether all validation rules passed
        /// - Errors: Collection of validation errors if any rules failed
        /// </returns>
        public ValidationResultDetail Validate()
        {
            var validator = new ProductValidator();
            var result = validator.Validate(this);
            return new ValidationResultDetail
            {
                IsValid = result.IsValid,
                Errors = result.Errors.Select(o => (ValidationErrorDetail)o)
            };
        }

        /// <summary>
        /// Gets or sets the products in the sale.
        /// </summary>
        public ICollection<ProductSale> ProductSales { get; set; } = [];
    }
}