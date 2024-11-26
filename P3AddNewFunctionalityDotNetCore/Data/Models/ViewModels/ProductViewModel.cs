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
        
        [CustomStockValidation(
        MissingQuantityMessage = "MissingQuantity",
        NotAnIntegerMessage = "StockNotAnInteger",
        NotGreaterThanZeroMessage = "StockNotGreaterThanZero")]
        public string Stock { get; set; }
        
        
        [Required(ErrorMessage = "MissingPrice")]
        [Range(0.1, double.MaxValue, ErrorMessage ="PriceNotGreaterThanZero")]
        public string Price { get; set; }
    }
    
    public class CustomStockValidationAttribute : ValidationAttribute
    {
        public string MissingQuantityMessage { get; set; } = "MissingQuantity";
        public string NotAnIntegerMessage { get; set; } = "StockNotAnInteger";
        public string NotGreaterThanZeroMessage { get; set; } = "StockNotGreaterThanZero";

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(value.ToString()))
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
}
