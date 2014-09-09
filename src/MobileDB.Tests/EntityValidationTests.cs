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