using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Delve.Models.Validation;

namespace Delve.Tests
{
    [TestClass]
    public class ValidationBuilderTests
    {
        private TestValidator _validator;

        [TestInitialize]
        public void Initialize()
        {
            _validator = new TestValidator();
        }

        [TestMethod]
        public void CanSelect_CorrectExpression_Succeeds()
        {
            _validator.CanSelectTest("FirstName", u => u.FirstName);
            _validator.CanSelectTest("Date", u => u.DateOfBirth);
            _validator.CanSelectTest("Id", u => u.Id);
            _validator.CanSelectTest("Age", u => Math.Round((DateTime.Now - u.DateOfBirth).TotalDays, 2));
            _validator.CanSelectTest("Test", u => u.Test);
            Assert.IsTrue(true);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidValidationBuilderException))]
        public void CanSelect_DuplicateKey_ThrowsException()
        {
            _validator.CanSelectTest("Id", u => u.Id);
            _validator.CanSelectTest("Id", u => u.FirstName);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidValidationBuilderException))]
        public void CanFilter_InvalidType_ThrowsException()
        {
            _validator.CanFilterTest("Test", u => u.Test);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidValidationBuilderException))]
        public void AllowAll_InvalidFilterType_ThrowsException()
        {
            _validator.AllowAllTest("Test", u => u.Test);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidValidationBuilderException))]
        public void AllowAll_DuplicateKey_ThrowsException()
        {
            _validator.AllowAllTest("Id", u => u.Id);
            _validator.CanSelectTest("Id", u => u.DateOfBirth);
        }

        [TestMethod]
        public void AllowAll_DuplicateExpression_Succeeds()
        {
            _validator.AllowAllTest("Id", u => u.Id);
            _validator.AllowAllTest("identification", u => u.Id);
        }

        [TestMethod]
        public void AllowAll_NavigationProperty_Succeeds()
        {
            _validator.AllowAllTest("RoleId", u => u.UserRoles.Select(x => x.RoleId));
        }
    }
}
