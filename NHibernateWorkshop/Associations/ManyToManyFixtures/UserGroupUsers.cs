using System.Linq;
using Northwind.Builders;
using Northwind.Entities;
using NUnit.Framework;

namespace NHibernateWorkshop.Associations.ManyToManyFixtures
{
    [TestFixture]
    public class UserGroupUsers : AutoRollbackFixture
    {
        private UserGroup _userGroup;
        private User _user1;
        private User _user2;

        protected override void AfterSetUp()
        {
            _userGroup = new UserGroupBuilder().Build();
            _user1 = new UserBuilder().WithUserName("u1").Build();
            _user2 = new UserBuilder().WithUserName("u2").Build();
        }

        [Test]
        public void users_can_be_added_to_a_group()
        {
            _userGroup.AddUser(_user1);
            _userGroup.AddUser(_user2);
            Session.Save(_userGroup);
            Flush();

            Assert.IsTrue(_user1.UserGroups.Contains(_userGroup));
            Assert.IsTrue(_user2.UserGroups.Contains(_userGroup));
            Clear();

            var retrievedGroup = Session.Get<UserGroup>(_userGroup.Id);
            Assert.IsTrue(retrievedGroup.Users.Contains(_user1));
            Assert.IsTrue(retrievedGroup.Users.Contains(_user2));
        }

        [Test]
        public void users_can_be_removed_from_a_group()
        {
            _userGroup.AddUser(_user1);
            _userGroup.AddUser(_user2);
            Session.Save(_userGroup);
            Flush();
            _userGroup.RemoveUser(_user2);
            Flush();

            Assert.IsTrue(_user1.UserGroups.Contains(_userGroup));
            Assert.IsFalse(_user2.UserGroups.Contains(_userGroup));
            Clear();

            var retrievedGroup = Session.Get<UserGroup>(_userGroup.Id);
            Assert.IsTrue(retrievedGroup.Users.Contains(_user1));
            Assert.IsFalse(retrievedGroup.Users.Contains(_user2));
        }

        [Test]
        public void deleting_a_group_does_not_delete_its_users()
        {
            _userGroup.AddUser(_user1);
            _userGroup.AddUser(_user2);
            Session.Save(_userGroup);
            Flush();

            Session.Delete(_userGroup);
            _user1.RemoveUserGroup(_userGroup);
            _user2.RemoveUserGroup(_userGroup);
            FlushAndClear();

            Assert.IsNull(Session.Get<UserGroup>(_userGroup.Id));
            Assert.IsNotNull(Session.Get<User>(_user1.Id));
            Assert.IsNotNull(Session.Get<User>(_user2.Id));
        }
    }
}