using System.Linq;
using Northwind.Builders;
using Northwind.Entities;
using NUnit.Framework;

namespace NHibernateWorkshop.Associations.ManyToManyFixtures
{
    [TestFixture]
    public class UserUserGroups : AutoRollbackFixture
    {
        private User _user;
        private UserGroup _userGroup1;
        private UserGroup _userGroup2;

        protected override void AfterSetUp()
        {
            _user = new UserBuilder().Build();
            _userGroup1 = new UserGroupBuilder().WithName("g1").Build();
            _userGroup2 = new UserGroupBuilder().WithName("g2").Build();
        }

        [Test]
        public void groups_can_be_added_to_user()
        {
            _user.AddUserGroup(_userGroup1);
            _user.AddUserGroup(_userGroup2);
            Session.Save(_user);
            Flush();

            Assert.IsTrue(_userGroup1.Users.Contains(_user));
            Assert.IsTrue(_userGroup2.Users.Contains(_user));
            Clear();

            var retrievedUser = Session.Get<User>(_user.Id);
            Assert.IsTrue(retrievedUser.UserGroups.Contains(_userGroup1));
            Assert.IsTrue(retrievedUser.UserGroups.Contains(_userGroup2));
        }

        [Test]
        public void groups_can_be_removed_from_user()
        {
            _user.AddUserGroup(_userGroup1);
            _user.AddUserGroup(_userGroup2);
            Session.Save(_user);
            Flush();
            _user.RemoveUserGroup(_userGroup1);
            Flush();

            Assert.IsFalse(_userGroup1.Users.Contains(_user));
            Assert.IsTrue(_userGroup2.Users.Contains(_user));
            Clear();

            var retrievedUser = Session.Get<User>(_user.Id);
            Assert.IsFalse(retrievedUser.UserGroups.Contains(_userGroup1));
            Assert.IsTrue(retrievedUser.UserGroups.Contains(_userGroup2));
        }

        [Test]
        public void deleting_a_user_does_not_delete_its_groups()
        {
            _user.AddUserGroup(_userGroup1);
            _user.AddUserGroup(_userGroup2);
            Session.Save(_user);
            Flush();
            
            Session.Delete(_user);
            _userGroup1.RemoveUser(_user);
            _userGroup2.RemoveUser(_user);
            FlushAndClear();

            Assert.IsNull(Session.Get<User>(_user.Id));
            Assert.IsNotNull(Session.Get<UserGroup>(_userGroup1.Id));
            Assert.IsNotNull(Session.Get<UserGroup>(_userGroup2.Id));
        }
    }
}