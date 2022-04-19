using System;
using System.ComponentModel.DataAnnotations;

namespace DeliciousFood.Services.Attributes
{
    /// <summary>
    /// A validation attribute to check that the enumeration has a valid value
    /// </summary>
    public class EnumerationRequiredAttribute : RequiredAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var baseValidationResult = base.IsValid(value, validationContext);
            if (baseValidationResult != ValidationResult.Success)
                return baseValidationResult;

            var type = value.GetType();
            return type.IsEnum && Enum.IsDefined(type, value)
                ? ValidationResult.Success
                : new ValidationResult($"Enumeration {type.Name} field has the incorrect value!");
        }
    }
}
