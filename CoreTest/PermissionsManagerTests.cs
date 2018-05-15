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
    }
}
