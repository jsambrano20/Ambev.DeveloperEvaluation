using System.ComponentModel.DataAnnotations;
using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Validation;

namespace Ambev.DeveloperEvaluation.Domain.ValueObjects;

public class ProductSale
{
    /// <summary>
    /// Gets or sets the sale ID.
    /// Represents the unique identifier of the sale to which this product belongs.
    /// </summary>
    [Required(ErrorMessage = "Sale ID is required.")]
    public Guid SaleId { get; set; }

    /// <summary>
    /// Gets or sets the sale details.
    /// </summary>
    public Sale? Sale { get; set; }

    /// <summary>
    /// Gets or sets the product ID.
    /// Represents the unique identifier of the product being sold.
    /// </summary>
    [Required(ErrorMessage = "Product ID is required.")]
    public Guid ProductId { get; set; }

    /// <summary>
    /// Gets or sets the product details.
    /// </summary>
    public Product? Product { get; set; }

    /// <summary>
    /// Gets or sets the quantity of the product in the sale.
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
    public int Quantity { get; set; }

    /// <summary>
    /// Gets or sets the total amount for this product in the sale.
    /// This value is calculated as: Quantity * Product Price - Discount.
    /// </summary>
    [Range(0, double.MaxValue, ErrorMessage = "Total amount must be zero or greater.")]
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Gets or sets the discount percentage applied to the product.
    /// </summary>
    [Range(0, 100, ErrorMessage = "Discount must be between 0 and 100.")]
    public decimal Discount { get; set; }

    /// <summary>
    /// Gets or sets the status of the product in the sale.
    /// </summary>
    [Required(ErrorMessage = "Status is required.")]
    public SaleStatus Status { get; set; } = SaleStatus.Active;

    /// <summary>
    /// Gets or sets the date and time when the product sale was created.
    /// Uses UTC format.
    /// </summary>
    [Required(ErrorMessage = "Date is required.")]
    public DateTime Date { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the date and time when the product sale was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Validates the <see cref="ProductSale"/> object using <see cref="ProductSaleValidator"/> rules.
    /// </summary>
    /// <returns>
    /// A <see cref="ValidationResultDetail"/> containing:
    /// - IsValid: Indicates whether all validation rules passed.
    /// - Errors: Collection of validation errors if any rules failed.
    /// </returns>
    public ValidationResultDetail Validate()
    {
        var validator = new ProductSaleValidator();
        var result = validator.Validate(this);
        return new ValidationResultDetail
        {
            IsValid = result.IsValid,
            Errors = result.Errors.Select(o => (ValidationErrorDetail)o)
        };
    }

    /// <summary>
    /// Calculates the total amount for this product in the sale.
    /// Applies a discount based on quantity and updates the <see cref="TotalAmount"/>.
    /// </summary>
    public void CalculateTotalAmount()
    {
        ApplyDiscount();
        TotalAmount = Quantity * (Product?.Price ?? 0) * (1M - Discount / 100M);
    }

    /// <summary>
    /// Applies a discount based on the quantity of the product in the sale.
    /// </summary>
    private void ApplyDiscount()
    {
        if (Quantity >= 10)
        {
            Discount = 20;
        }
        else if (Quantity >= 4)
        {
            Discount = 10;
        }
        else
        {
            Discount = 0;
        }
    }
}
