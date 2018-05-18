using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CoreTest
{
    [TestClass]
    public class PermissionsManagerTests
    {
        VTRPG.Core.SaveManager.SaveManager Manager;

        [TestMethod]
        public void GroupCreation()
        {
            VTRPG.Core.SaveManager.SaveManager Manager;
            try
            {
                try
                {
                    File.Delete(Path.Combine(Environment.CurrentDirectory, "Group_GroupCreation.vtrpg.db"));
                }
                catch { }
                Manager = new VTRPG.Core.SaveManager.SaveManager(Path.Combine(Environment.CurrentDirectory, "Group_GroupCreation.vtrpg.db"));
                Manager.InitSave();
            }
            catch
            {
                Assert.Inconclusive();
                return;
            }

            string Name = "Test Group";
            string Desc = "This Group is a test";
            VTRPG.Core.Permissions.Data.Group group = Manager.PermissionsManager.CreateGroup(Name, Desc);

            Assert.AreEqual(Name, group.Name);
            Assert.AreEqual(Desc, group.Description);

            try
            {
                File.Delete(Path.Combine(Environment.CurrentDirectory, "Group_GroupCreation.vtrpg.db"));
            }
            catch { }
        }

        [TestMethod]
        public void GroupMembership()
        {
            VTRPG.Core.SaveManager.SaveManager Manager;

            VTRPG.Core.Permissions.Data.Group group;
            VTRPG.Core.Authentication.Data.User user1;
            VTRPG.Core.Authentication.Data.User user2;
            try
            {
                try
                {
                    File.Delete(Path.Combine(Environment.CurrentDirectory, "Group_GroupMembership.vtrpg.db"));
                }
                catch { }
                Manager = new VTRPG.Core.SaveManager.SaveManager(Path.Combine(Environment.CurrentDirectory, "Group_GroupMembership.vtrpg.db"));
                Manager.InitSave();

                group = Manager.PermissionsManager.CreateGroup("Test Group", "This Group is a test");
                user1 = Manager.AuthManager.RegisterUser("User 1", "User 1", System.Drawing.Color.AliceBlue, "User 1", "User 1", 1, true);
                user2 = Manager.AuthManager.RegisterUser("User 2", "User 2", System.Drawing.Color.AliceBlue, "User 2", "User 2", 1, true);
            }
            catch
            {
                Assert.Inconclusive();
                return;
            }

            // Add
            group.AddMember(user1.UID);

            // Test
            try
            {
                if (!group.HasMamber(user1.UID)) { Assert.Fail("Failed to identify a group member"); }
            }
            catch (VTRPG.Core.Authentication.Data.UnknownUserException)
            {
                Assert.Fail("Failed to identify that a user does exist");
            }

            try
            {
                if (group.HasMamber(user2.UID))
                    Assert.Fail("Failed to identify that a user is not a member of a group");
            }
            catch (VTRPG.Core.Authentication.Data.UnknownUserException)
            {
                Assert.Fail("Failed to identify that a user does exist");
            }

            try
            {
                if (group.HasMamber(3))
                    Assert.Fail("Failed to identify that a user does not exist");
            }
            catch (VTRPG.Core.Authentication.Data.UnknownUserException) { }

            // Clean Up
            try
            {
                File.Delete(Path.Combine(Environment.CurrentDirectory, "Group_GroupMembership.vtrpg.db"));
            }
            catch { }
        }
    }
}
