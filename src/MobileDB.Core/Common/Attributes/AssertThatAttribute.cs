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