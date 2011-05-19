using System.Linq;
using NHibernate;
using Northwind.Builders;
using Northwind.Entities;
using NUnit.Framework;

namespace NHibernateWorkshop.Performance
{
    // NOTE: the value in these tests is in checking their output... they usually don't assert on anything
    // User has read-write caching, and its UserGroups collection has read-write caching as well
    // UserGroup has read-write caching, but its Users collection has no caching!
    [TestFixture]
    public class SecondLevelCache : Fixture
    {
        private User _user1;
        private User _user2;
        private UserGroup _userGroup1;
        private UserGroup _userGroup2;

        [TestFixtureSetUp]
        public void SetUp()
        {
            Logger.Info("...begin setup...");
            using (var session = CreateSession())
            using (var transaction = session.BeginTransaction())
            {
                _user1 = new UserBuilder().WithUserName("u1").Build();
                _user2 = new UserBuilder().WithUserName("u2").Build();
                _userGroup1 = new UserGroupBuilder().WithName("g1").Build();
                _userGroup2 = new UserGroupBuilder().WithName("g2").Build();
                _user1.AddUserGroup(_userGroup1);
                _user1.AddUserGroup(_userGroup2);
                _user2.AddUserGroup(_userGroup1);
                _user2.AddUserGroup(_userGroup2);
                session.Save(_user1);
                session.Save(_user2);
                session.Save(_userGroup1);
                session.Save(_userGroup2);
                session.Flush();
                session.Clear();

                // make sure everything is present in the cache for the tests
                session.QueryOver<User>().List();
                session.QueryOver<UserGroup>().List();

                transaction.Commit();
            }
            Logger.Info("...end setup...");
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            Logger.Info("...begin teardown...");
            using (var session = CreateSession())
            using (var transaction = session.BeginTransaction())
            {
                var u1 = session.Get<User>(_user1.Id);
                var u2 = session.Get<User>(_user2.Id);
                session.Delete(u1.Employee);
                session.Delete(u2.Employee);
                session.Delete(session.Get<UserGroup>(_userGroup1.Id));
                session.Delete(session.Get<UserGroup>(_userGroup2.Id));
                transaction.Commit();
            }
            Logger.Info("...end teardown...");
        }

        [Test]
        public void retrieving_user_after_it_has_been_put_in_cache_does_not_issue_select_statement()
        {
            using (var session = CreateSession())
            using (var transaction = session.BeginTransaction())
            {
                Logger.Info("these 2 gets won't issue select statements since both instances are in the second level cache");
                var user1 = session.Get<User>(_user1.Id);
                var user2 = session.Get<User>(_user2.Id);
                transaction.Commit();
            }
        }

        [Test]
        public void updating_user_after_it_has_been_put_in_cache_updates_both_database_and_cache()
        {
            using (var session = CreateSession())
            using (var transaction = session.BeginTransaction())
            {
                var user1 = session.Get<User>(_user1.Id);
                user1.PasswordHash = new byte[] {87, 63};
                Logger.Info("this will trigger one update statement");
                transaction.Commit();
            }

            using (var session = CreateSession())
            using (var transaction = session.BeginTransaction())
            {
                Logger.Info("this won't cause a select statement since the instance is in the cache, and its values are the updated ones");
                var user1 = session.Get<User>(_user1.Id);
                Assert.AreEqual(new byte[] { 87, 63 }, user1.PasswordHash);
                transaction.Commit();
            }
        }

        [Test]
        public void deleting_cached_object_removes_it_from_database_and_cache()
        {
            var user = new UserBuilder().Build();

            using (var session = CreateSession())
            using (var transaction = session.BeginTransaction())
            {
                session.Save(user);
                transaction.Commit();
            }

            using (var session = CreateSession())
            using (var transaction = session.BeginTransaction())
            {
                var retrievedUser = session.Get<User>(user.Id);
                session.Delete(retrievedUser.Employee); // deletes both the employee as the user because of the cascade
                transaction.Commit();
            }

            using (var session = CreateSession())
            using (var transaction = session.BeginTransaction())
            {
                Assert.IsNull(session.Get<User>(user.Id));
            }
        }

        [Test]
        public void accessing_non_cached_association_of_cached_entity_causes_select_statement()
        {
            using (var session = CreateSession())
            using (var transaction = session.BeginTransaction())
            {
                var retrievedUser = session.Get<User>(_user1.Id);
                Logger.Info("accessing the employee of the cached user will issue a select statement");
                var employeeName = retrievedUser.Employee.FirstName;
                transaction.Commit();
            }
        }

        [Test]
        public void accessing_cached_collection_does_not_issue_queries_if_collection_entities_are_cached()
        {
            using (var session = CreateSession())
            using (var transaction = session.BeginTransaction())
            {
                var user = session.Get<User>(_user1.Id);
                Logger.Info("depending on the order in which the tests run, the next statement might issue a select if the collection hasn't been cached by a previous test yet");
                user.UserGroups.GetEnumerator().MoveNext();
                transaction.Commit();
            }

            using (var session = CreateSession())
            using (var transaction = session.BeginTransaction())
            {
                var user = session.Get<User>(_user1.Id);
                Logger.Info("this doesn't cause a select statement");
                user.UserGroups.GetEnumerator().MoveNext();
                transaction.Commit();
            }
        }

        [Test]
        public void manipulating_cached_collection_updates_both_database_and_cache()
        {
            var newGroup = new UserGroupBuilder().Build();

            using (var session = CreateSession())
            using (var transaction = session.BeginTransaction())
            {
                var user = session.Get<User>(_user1.Id);
                user.AddUserGroup(newGroup);
                Logger.Info("this causes 2 insert statements");
                transaction.Commit();
            }

            using (var session = CreateSession())
            using (var transaction = session.BeginTransaction())
            {
                var user = session.Get<User>(_user1.Id);
                Logger.Info("this doesn't cause a select statement");
                Assert.IsTrue(user.UserGroups.Contains(newGroup));
                transaction.Commit();
            }

            using (var session = CreateSession())
            using (var transaction = session.BeginTransaction())
            {
                Logger.Info("this session should cause 2 delete statements, and one select statement for the usergroup's user collection, which isn't cached");
                var user = session.Get<User>(_user1.Id);
                var group = session.Get<UserGroup>(newGroup.Id);
                user.RemoveUserGroup(group);
                session.Delete(group);
                transaction.Commit();
            }

            using (var session = CreateSession())
            using (var transaction = session.BeginTransaction())
            {
                var user = session.Get<User>(_user1.Id);
                Logger.Info("this causes a select because the object is no longer in the cache");
                Assert.IsNull(session.Get<UserGroup>(newGroup.Id)); 
                Logger.Info("this doesn't cause a select statement");
                Assert.AreEqual(0, user.UserGroups.Count(g => g.Id == newGroup.Id));
                transaction.Commit();
            }
        }

        [Test]
        public void cached_query_of_non_cached_entities_results_in_select_per_entity_in_result()
        {
            using (var session = CreateSession())
            using (var transaction = session.BeginTransaction())
            {
                Logger.Info("this causes one statement which retrieves all employees");
                var employees = session.QueryOver<Employee>().Cacheable().List();
                employees.GetEnumerator().MoveNext();
                transaction.Commit();
            }

            using (var session = CreateSession())
            using (var transaction = session.BeginTransaction())
            {
                // you never want this to happen in production code :p
                Logger.Info("the query is in the query cache, so there won't be one query to retrieve all employees. But employees aren't cached entities, so you get a select per employee!");
                var employees = session.QueryOver<Employee>().Cacheable().List();
                employees.GetEnumerator().MoveNext();
                transaction.Commit();
            }
        }

        [Test]
        public void cached_query_of_cached_entities_results_in_no_selects()
        {
            using (var session = CreateSession())
            using (var transaction = session.BeginTransaction())
            {
                Logger.Info("this causes one statement which retrieves all users");
                var users = session.QueryOver<User>().Cacheable().List();
                users.GetEnumerator().MoveNext();
                transaction.Commit();
            }

            using (var session = CreateSession())
            using (var transaction = session.BeginTransaction())
            {
                Logger.Info("the query is in the query cache, so there won't be one query to retrieve all users. And since all Users have been cached already, there are no extra statements");
                var users = session.QueryOver<User>().Cacheable().List();
                users.GetEnumerator().MoveNext();
                transaction.Commit();
            }
        }
    }
}