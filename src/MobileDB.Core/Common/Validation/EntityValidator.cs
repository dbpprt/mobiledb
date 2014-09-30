#region Copyright (C) 2014 Dennis Bappert

// The MIT License (MIT)

// Copyright (c) 2014 Dennis Bappert

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

#endregion

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