using System.Reflection;
using MobileDB.Common.Attributes;
using MobileDB.Contracts;
using MobileDB.Exceptions;

namespace MobileDB.Common.Validation
{
    public class EntityValidator : IEntityValidator
    {
        public void ValidateEntity(object entity)
        {
            var type = entity.GetType();
            var properties = type.GetRuntimeProperties();

            foreach (var propertyInfo in properties)
            {
                var validationAttributes = propertyInfo.GetCustomAttributes<ValidationAttribute>();

                var context = new EntityValidationContext
                {
                    MemberName = propertyInfo.Name,
                    ObjectInstance = entity,
                    ObjectType = type
                };

                foreach (var validationAttribute in validationAttributes)
                {
                    var result = validationAttribute.Validate(propertyInfo.GetValue(entity), context);

                    if (!result.Succeeded)
                    {
                        throw new EntityValidationException(result.ErrorMessage);
                    }
                }
            }
        }
    }
}