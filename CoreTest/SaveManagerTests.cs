using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CoreTest
{
    [TestClass]
    public class SaveManagerTests
    {
        VTRPG.Core.SaveManager.SaveManager Manager;
        
        [TestMethod]
        public void SaveFileInit()
        {
            try
            {
                File.Delete(Path.Combine(Environment.CurrentDirectory, "SaveFileInit.vtrpg.db"));
            }
            catch { }

            Manager = new VTRPG.Core.SaveManager.SaveManager(Path.Combine(Environment.CurrentDirectory, "SaveFileInit.vtrpg.db"));
            Manager.InitSave();

            try
            {
                File.Delete(Path.Combine(Environment.CurrentDirectory, "SaveFileInit.vtrpg.db"));
            }
            catch { }
        }
    }
}
