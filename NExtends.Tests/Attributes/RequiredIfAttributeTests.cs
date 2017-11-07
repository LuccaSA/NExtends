using NExtends.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace NExtends.Tests.Attributes
{
    public class RequiredIfAttributeTests
    {

        private class ClassWithRequiredAttribute
        {
            public bool HasRequired { get; set; }

            [RequiredIf("HasRequired", true)]
            public DateTime? RequiredProperty { get; set; }
        }

        [Fact]
        public void RequiredIfAttributeRequiredWithError()
        {
            var obj = new ClassWithRequiredAttribute
            {
                HasRequired = true
            };
            var results = Validate(obj);
            Assert.Single(results);
        }


        [Fact]
        public void RequiredIfAttributeRequiredWithoutError()
        {
            var obj = new ClassWithRequiredAttribute
            {
                HasRequired = true,
                RequiredProperty = DateTime.Now
            };
            var results = Validate(obj);
            Assert.Empty(results);
        }

        [Fact]
        public void RequiredIfAttributeNotRequired()
        {
            var obj = new ClassWithRequiredAttribute
            {
                HasRequired = false
            };
            var results = Validate(obj);
            Assert.Empty(results);
        }

        private HashSet<ValidationResult> Validate(ClassWithRequiredAttribute obj)
        {
            var validationContext = new ValidationContext(obj);
            var results = new HashSet<ValidationResult>();
            Validator.TryValidateObject(obj, validationContext, results, validateAllProperties: true);
            return results;
        }

    }
}
