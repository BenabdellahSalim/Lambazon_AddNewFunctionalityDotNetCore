using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace P3AddNewFunctionalityDotNetCore.Data.Models.ViewModels
{
    public class ProductViewModel
    {
        [BindNever]
        public int Id { get; set; }
        [Required(ErrorMessage = "ErrorMissingName")]
        public string Name { get; set; }
        public string Description { get; set; }
        public string Details { get; set; }
        [Required]
        [CustomStockValidationAttributeStock(
        MissingQuantityMessage = "MissingQuantity",
        NotAnIntegerMessage = "StockNotAnInteger",
        NotGreaterThanZeroMessage = "StockNotGreaterThanZero")]
        public string Stock { get; set; }


        [Required]
        [CustomStockValidationAttributePrice(
        MissingPriceMessage = "MissingPrice",
        NotAnIntegerMessage = "PriceNotAnInteger",
        NotGreaterThanZeroMessage = "PriceNotGreaterThanZero")]
        public string Price { get; set; }
    }

    public class CustomStockValidationAttributeStock : ValidationAttribute
    {
        public string MissingQuantityMessage { get; set; } = "MissingQuantity";
        public string NotAnIntegerMessage { get; set; } = "StockNotAnInteger";
        public string NotGreaterThanZeroMessage { get; set; } = "StockNotGreaterThanZero";

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(value?.ToString()))
            {
                return new ValidationResult(MissingQuantityMessage);
            }

            if (!int.TryParse(value.ToString(), out var stock))
            {
                return new ValidationResult(NotAnIntegerMessage);
            }

            return stock <= 0 ? new ValidationResult(NotGreaterThanZeroMessage) : ValidationResult.Success;
        }
    }
    public class CustomStockValidationAttributePrice : ValidationAttribute
    {
        public string MissingPriceMessage { get; set; } = "MissingPrice";
        public string NotAnIntegerMessage { get; set; } = "PriceNotAnInteger";
        public string NotGreaterThanZeroMessage { get; set; } = "PriceNotGreaterThanZero";

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(value?.ToString()))
            {
                return new ValidationResult(MissingPriceMessage);
            }

            if (!int.TryParse(value.ToString(), out var Price))
            {
                return new ValidationResult(NotAnIntegerMessage);
            }

            return Price <= 0 ? new ValidationResult(NotGreaterThanZeroMessage) : ValidationResult.Success;
        }
    }
}
