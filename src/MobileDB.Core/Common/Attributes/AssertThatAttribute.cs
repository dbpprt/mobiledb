using System;
using MobileDB.Common.ExpressiveAnnotations;
using MobileDB.Common.Validation;

namespace MobileDB.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public sealed class AssertThatAttribute : ValidationAttribute
    {
        private const string DefaultErrorMessage =
            "Assertion for {0} field is not satisfied by the following logic: {1}.";

        public AssertThatAttribute(string expression)
            : base(DefaultErrorMessage)
        {
            Parser = new Parser();
            Parser.RegisterMethods();

            Expression = expression;
        }

        private Func<object, bool> CachedValidationFunc { get; set; }
        private Parser Parser { get; set; }

        public string Expression { get; set; }

        public void Compile(Type validationContextType, bool force = false)
        {
            if (force)
            {
                CachedValidationFunc = Parser.Parse(validationContextType, Expression);
                return;
            }
            if (CachedValidationFunc == null)
                CachedValidationFunc = Parser.Parse(validationContextType, Expression);
        }

        public string FormatErrorMessage(string displayName, string expression)
        {
            return string.Format(ErrorMessageString, displayName, expression);
        }

        public override EntityValidationResult Validate(object value, EntityValidationContext entityValidationContext)
        {
            if (entityValidationContext == null)
                throw new ArgumentNullException("entityValidationContext", "EntityValidationContext not provided.");

            if (value != null)
            {
                if (CachedValidationFunc == null)
                    CachedValidationFunc = Parser.Parse(entityValidationContext.ObjectType, Expression);
                if (!CachedValidationFunc(entityValidationContext.ObjectInstance))
                    return
                        new EntityValidationResult(FormatErrorMessage(entityValidationContext.DisplayName, Expression));
            }

            return EntityValidationResult.Success();
        }
    }
}