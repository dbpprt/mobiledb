using System;
using MobileDB.Common.Validation;

namespace MobileDB.Common.Attributes
{
    public abstract class ValidationAttribute : Attribute
    {
        public ValidationAttribute(string errorMessage)
        {
            ErrorMessageString = errorMessage;
        }

        public string ErrorMessageString { get; private set; }

        public abstract EntityValidationResult Validate(object value, EntityValidationContext entityValidationContext);
    }
}