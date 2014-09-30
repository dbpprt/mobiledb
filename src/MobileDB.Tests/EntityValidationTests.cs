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
using MobileDB.Common.Attributes;
using MobileDB.Common.Validation;
using MobileDB.Exceptions;
using Xunit;

namespace MobileDB.Tests
{
    [Trait("Tests the entity validation mechanisms", "")]
    public class EntityValidationTests
    {
        private static EntityValidator _validator = new EntityValidator();

        [Fact]
        public void Validator_Works()
        {
            Assert.Throws<EntityValidationException>(
                () => _validator.ValidateEntity(new EntityDatetimeValidation
                {
                    Future = DateTime.Now.Subtract(new TimeSpan(12421, 1, 0, 0))
                }));
        }

        [Fact]
        public void Validator_Caching_Works()
        {
            Assert.Throws<EntityValidationException>(
                () => _validator.ValidateEntity(new EntityDatetimeValidation
                {
                    Future = DateTime.Now.Subtract(new TimeSpan(12421, 1, 0, 0))
                }));

            Assert.Throws<EntityValidationException>(
                () => _validator.ValidateEntity(new EntityDatetimeValidation
                {
                    Future = DateTime.Now.Subtract(new TimeSpan(12421, 1, 0, 0))
                }));
        }

        private class EntityDatetimeValidation
        {
            [AssertThat("Future >= Today()")]
            public DateTime? Future { get; set; }
        }
    }
}