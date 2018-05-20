using System;
using System.Collections.Generic;
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
            Assert.AreEqual(Manager.PermissionsManager.GroupExists(1), true);
            Assert.AreEqual(Manager.PermissionsManager.GroupExists(Name), true);
            Assert.AreEqual(Manager.PermissionsManager.GroupExists(2), false);
            Assert.AreEqual(Manager.PermissionsManager.GroupExists("Fake"), false);

            Assert.AreEqual(Manager.PermissionsManager.GetGroup(1).GID, 1);
            Assert.AreEqual(Manager.PermissionsManager.GetGroup(Name).GID, 1);
            Assert.AreEqual(Manager.PermissionsManager.GetGroup(2), null);
            Assert.AreEqual(Manager.PermissionsManager.GetGroup("Fake"), null);

            Manager.PermissionsManager.DeleteGroup(1);
            Assert.AreEqual(Manager.PermissionsManager.GroupExists(1), false);
            Assert.AreEqual(Manager.PermissionsManager.GroupExists(Name), false);

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

            // Remove
            group.RemoveMember(user1.UID);

            // Test
            try
            {
                if (group.HasMamber(user1.UID))
                    Assert.Fail("Failed to identify that a user is not a member of a group");
            }
            catch (VTRPG.Core.Authentication.Data.UnknownUserException)
            {
                Assert.Fail("Failed to identify that a user does exist");
            }

            // Clean Up
            try
            {
                File.Delete(Path.Combine(Environment.CurrentDirectory, "Group_GroupMembership.vtrpg.db"));
            }
            catch { }
        }

        [TestMethod]
        public void GroupPermissions()
        {
            VTRPG.Core.SaveManager.SaveManager Manager;

            VTRPG.Core.Permissions.Data.Group group;
            try
            {
                try
                {
                    File.Delete(Path.Combine(Environment.CurrentDirectory, "Group_GroupPermissions.vtrpg.db"));
                }
                catch { }
                Manager = new VTRPG.Core.SaveManager.SaveManager(Path.Combine(Environment.CurrentDirectory, "Group_GroupPermissions.vtrpg.db"));
                Manager.InitSave();

                group = Manager.PermissionsManager.CreateGroup("Test Group", "This Group is a test");
            }
            catch
            {
                Assert.Inconclusive();
                return;
            }

            // Test
            bool add1 = group.AddPermission("System.Test.Permission");
            bool add2 = group.AddPermission("System.Test.Permission");
            bool add3 = group.AddPermission("System.Plugins", false);

            Assert.AreEqual(add1, true);
            Assert.AreEqual(add2, false);
            Assert.AreEqual(add3, true);

            IReadOnlyList<KeyValuePair<string, bool>> perms = group.Permissions;

            Assert.AreEqual(perms[0].Value, true);
            Assert.AreEqual(perms[1].Value, false);

            Assert.AreEqual(group.HasPermissionEntry("System.Test.Permission"), true);
            Assert.AreEqual(group.HasPermissionEntry("NoPermission"), false);

            Assert.AreEqual(group.HasPermission("System.Test.Permission"), true);
            Assert.AreEqual(group.HasPermission("System.Plugins"), false);

            group.UpdatePermission("System.Test.Permission", false);
            Assert.AreEqual(group.HasPermission("System.Test.Permission"), false);

            try
            {
                File.Delete(Path.Combine(Environment.CurrentDirectory, "Group_GroupPermissions.vtrpg.db"));
            }
            catch { }
        }
    }
}
