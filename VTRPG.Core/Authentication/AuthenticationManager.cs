using SimpleCrypto;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VTRPG.Core.Authentication.Data;

namespace VTRPG.Core.Authentication
{
    public class AuthenticationManager
    {
        public AuthenticationManager(SaveManager.SaveManager SaveManager)
        {
            Manager = SaveManager;
        }

        private SaveManager.SaveManager Manager;
        public SaveManager.SaveManager SaveManager
        {
            get { return Manager; }
            set
            {
                Manager = value;
                Manager.SaveFileChangedEvent += Manager_SaveFileChangedEvent;

            }
        }

        // Get Set Lockout Setting (Key: Base_Auth_Lockout_Time) (Default: 30)

        private void Manager_SaveFileChangedEvent(string File)
        {

        }

        public RegisterState RegisterUser(User User)
        {
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(SaveManager.SQLiteConnectionString))
                using (SQLiteCommand cmd = new SQLiteCommand($"SELECT UID, Hash, Salt, Iterations FROM tblAuth WHERE Username = \"{ User.Name }\";", conn))
                {
                    conn.Open();

                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                return RegisterState.Error;
            }
            return RegisterState.Unknown;
        }

        public AuthState AuthenticateUser(string Username, string Password)
        {
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(SaveManager.SQLiteConnectionString))
                using (SQLiteCommand cmd = new SQLiteCommand($"SELECT UID, Hash, Salt, Iterations, Security_LastAttempt, Security_Attempts, Security_Disabled FROM tblAuth WHERE Username = \"{ Username }\";", conn))
                {
                    conn.Open();
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        if (!reader.HasRows)
                            return AuthState.UnknownUser;

                        reader.Read();
                        if (reader.GetBoolean(6))
                        {
                            // Set the Last Attempt Time Here
                            return AuthState.Disabled;
                        }

                        if (reader.GetDateTime(4).Ticks <= DateTime.UtcNow.Subtract(new DateTime(0, 0, 0, 0, 30, 0, 0, DateTimeKind.Utc)).Ticks)
                        {
                            // Set the Last Attempt Time Here
                            return AuthState.Locked;
                        }

                        PBKDF2 PBKDF2 = new PBKDF2();
                        PBKDF2.HashIterations = reader.GetInt32(3);

                        if (PBKDF2.Compare(PBKDF2.Compute(Password, reader.GetString(2)), reader.GetString(1)))
                        {
                            // Good Password

                            conn.Close();
                            return AuthState.OK;
                        }
                        else
                        {
                            // Bad Password
                            // Set the Last Attempt Time Here

                            conn.Close();
                            return AuthState.BadPassword;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return AuthState.Error;
            }
            return AuthState.Unknown;
        }
    }
}
