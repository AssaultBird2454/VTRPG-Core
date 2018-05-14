using System;
using System.Drawing;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VTRPG.Core.Authentication.Data;

namespace CoreTest
{
    [TestClass]
    public class AuthenticationTests
    {
        [TestMethod]
        public void UserRegristration()
        {
            VTRPG.Core.SaveManager.SaveManager Manager;
            try
            {
                try
                {
                    File.Delete(Path.Combine(Environment.CurrentDirectory, "Auth_UserRegristration.vtrpg.db"));
                }
                catch { }
                Manager = new VTRPG.Core.SaveManager.SaveManager(Path.Combine(Environment.CurrentDirectory, "Auth_UserRegristration.vtrpg.db"));
                Manager.InitSave();
            }
            catch
            {
                Assert.Inconclusive();
                return;
            }

            Manager.AuthManager.RegisterUser("Tasman Leach", "Gillbates", Color.Green, "AssaultBird2454", "Ninja", 40000, true);

            try
            {
                File.Delete(Path.Combine(Environment.CurrentDirectory, "Auth_UserRegristration.vtrpg.db"));
            }
            catch { }
        }

        [TestMethod]
        public void UserAuthentication()
        {
            VTRPG.Core.SaveManager.SaveManager Manager;

            // Setup
            try
            {
                File.Delete(Path.Combine(Environment.CurrentDirectory, "Auth_UserAuthentication.vtrpg.db"));
            }
            catch { }
            Manager = new VTRPG.Core.SaveManager.SaveManager(Path.Combine(Environment.CurrentDirectory, "Auth_UserAuthentication.vtrpg.db"));
            Manager.InitSave();

            Manager.AuthManager.RegisterUser("User A", "Char A", Color.Green, "Account A", "Password A", 40000, false);
            Manager.AuthManager.RegisterUser("User B", "Char B", Color.Green, "Account B", "Password B", 40000, false);
            Manager.AuthManager.RegisterUser("User D", "Char D", Color.Green, "Account D", "Password D", 40000, false);
            Manager.AuthManager.RegisterUser("User E", "Char E", Color.Green, "Account E", "Password E", 40000, false);

            // Test
            Manager.AuthManager.AuthenticateUser("Account A", "Password A");
            try { Manager.AuthManager.AuthenticateUser("Account A", "Password B"); } catch (BadPasswordException) { } catch (Exception ex) { Assert.Fail("Failed to identify that the password is incorrect", ex); }
            try { Manager.AuthManager.AuthenticateUser("Account C", "Password A"); } catch (UnknownUserException) { } catch (Exception ex) { Assert.Fail("Failed to identify that the user does not exist", ex); }
            try
            {
                Manager.AuthManager.LockAccount(Manager.AuthManager.GetUser("User D").UID);
                Manager.AuthManager.AuthenticateUser("Account D", "Password D");
            }
            catch (AccountLockedException) { }
            catch (Exception ex) { Assert.Fail("Failed to identify that the account is locked", ex); }
            try
            {
                Manager.AuthManager.AccountState(Manager.AuthManager.GetUser("User E").UID, false);
                Manager.AuthManager.AuthenticateUser("Account E", "Password E");
            }
            catch (AccountDisabledException) { }
            catch (Exception ex) { Assert.Fail("Failed to identify that the account is disabled", ex); }

            // Clean-up
            try
            {
                File.Delete(Path.Combine(Environment.CurrentDirectory, "Auth_UserAuthentication.vtrpg.db"));
            }
            catch { }
        }

        [TestMethod]
        public void AuthHashFunction()
        {
            VTRPG.Core.Authentication.AuthenticationManager auth = new VTRPG.Core.Authentication.AuthenticationManager(null);
            auth.HashPassword("Ninja", auth.GenerateSalt());
        }
    }
}
