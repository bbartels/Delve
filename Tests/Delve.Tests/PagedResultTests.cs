using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Delve.Models;
using Delve.Tests.Models;

namespace Delve.Tests
{
    [TestClass]
    internal class PagedResultTests
    {
        private IPagedResult<User> _pagedUsers;
        private ResourceParameter<User> _param;
        private IList<User> _users;

        [TestInitialize]
        public void Initialize()
        {
            _users = Repository.GetUsers();
            _param = new ResourceParameter<User>();
        }

        [TestMethod]
        public void PagedResult_ValidParameters_Succeeds()
        {
            _pagedUsers = new PagedResult<User>(_users, 1, 5, _users.Count);
            Assert.AreEqual(_pagedUsers.PageSize, 5);
            Assert.IsTrue(_pagedUsers.HasNext);
            Assert.IsFalse(_pagedUsers.HasPrevious);
        }
    }
}
