using SimpleCrypto;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VTRPG.Core.Authentication.Data;

namespace VTRPG.Core.Authentication
{
    public class AuthenticationManager
    {
        #region Base
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
            }
        }
        #endregion

        #region Time
        const string FMT = "o";
        public string GetDateTimeNow_String
        {
            get
            {
                DateTime now = DateTime.Now;
                return now.ToString(FMT);
            }
        }
        public DateTime GetDateTimeNow_Obj
        {
            get
            {
                return DateTime.ParseExact(GetDateTimeNow_String, FMT, CultureInfo.InvariantCulture);
            }
        }
        #endregion

        #region Settings
        public int Setting_LockoutTime
        {
            get
            {
                try
                {
                    return Convert.ToInt32(SaveManager.SettingsManager.GetValue("Base_Auth_Lockout_Time"));
                }
                catch
                {
                    return Convert.ToInt32(Authentication.Setting_Default_LockoutTime);
                }
            }
            set
            {
                SaveManager.SettingsManager.SetValue("Base_Auth_Lockout_Time", value);
            }
        }

        public int Setting_LockoutAttemptCount
        {
            get
            {
                try
                {
                    return Convert.ToInt32(SaveManager.SettingsManager.GetValue("Base_Auth_Lockout_Count"));
                }
                catch
                {
                    return Convert.ToInt32(Authentication.Setting_Default_AttemptCount);
                }
            }
            set
            {
                SaveManager.SettingsManager.SetValue("Base_Auth_Lockout_Count", value);
            }
        }
        #endregion

        public void InitSaveFile()
        {
            using (SQLiteConnection conn = new SQLiteConnection(Manager.SQLiteConnectionString).OpenAndReturn())
            using (SQLiteTransaction transaction = conn.BeginTransaction())
            {
                try
                {
                    // Init Users Table
                    using (SQLiteCommand cmd = new SQLiteCommand(Authentication.Save_Create_tblUsers, conn, transaction))
                        cmd.ExecuteNonQuery();
                    // Init Auth Table
                    using (SQLiteCommand cmd = new SQLiteCommand(Authentication.Save_Create_tblAuth, conn, transaction))
                        cmd.ExecuteNonQuery();

                    transaction.Commit();// Commit
                }
                catch
                {
                    transaction.Rollback();// Rollback
                    // Failed
                }
            }

            // Insert Default Settings into Settings table
            Manager.SettingsManager.SetValue("Base_Auth_Lockout_Time", Authentication.Setting_Default_LockoutTime);
            Manager.SettingsManager.SetValue("Base_Auth_Lockout_Count", Authentication.Setting_Default_AttemptCount);
        }

        public User RegisterUser(string Name, string Character_Name, Color UserColor, string Username, string Password, int HashIterations, bool isGM = false)
        {
            using (SQLiteConnection conn = new SQLiteConnection(SaveManager.SQLiteConnectionString).OpenAndReturn())
            {
                using (SQLiteTransaction transaction = conn.BeginTransaction())
                    try
                    {
                        string Salt = GenerateSalt(HashIterations);

                        using (SQLiteCommand cmd = new SQLiteCommand($"INSERT INTO tblUsers (\"Name\", \"Character_Name\", \"UserColor\", \"isGM\") VALUES (\"{Name}\", \"{Character_Name}\", \"{new ColorConverter().ConvertToString(UserColor)}\", {Convert.ToInt32(isGM)});", conn, transaction))
                            cmd.ExecuteNonQuery();
                        int ID = Convert.ToInt32(new SQLiteCommand("SELECT last_insert_rowid();", conn, transaction).ExecuteScalar());
                        using (SQLiteCommand cmd = new SQLiteCommand($"INSERT INTO tblAuth (\"UID\", \"Username\", \"Salt\", \"Hash\", \"Security_Attempts\", \"Security_LastAttempt\", \"Security_Disabled\") VALUES ({ID}, \"{Username}\", \"{Salt}\", \"{HashPassword(Password, Salt)}\", 0, \"{GetDateTimeNow_String}\", 0);", conn, transaction))
                            cmd.ExecuteNonQuery();

                        transaction.Commit();
                        conn.Close();
                        return GetUser(ID);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        conn.Close();
                        throw new RegristrationErrorException() { ex = ex };
                    }
            }
        }
        /// <summary>
        /// Attempts to authenticate a user
        /// </summary>
        /// <param name="Username">The username to the account being attempted</param>
        /// <param name="Password">The password to the account being logged into</param>
        /// <returns>The user object of the account that was succsfully authenticated</returns>
        /// <exception cref="UnknownUserException">Thrown when an attempt to authenticate results in not finding the user specified</exception>
        /// <exception cref="AccountDisabledException">Thrown when an attempt to authenticate against a disabled account is made</exception>
        /// <exception cref="AccountLockedException">Thrown when an attempt to authenticate against a locked account is made</exception>
        /// <exception cref="BadPasswordException">Thrown when an attempt to authenticate with a bad password</exception>
        public User AuthenticateUser(string Username, string Password)
        {
            using (SQLiteConnection conn = new SQLiteConnection(SaveManager.SQLiteConnectionString))
            using (SQLiteCommand cmd = new SQLiteCommand($"SELECT UID, Hash, Salt, Security_LastAttempt, Security_Attempts, Security_Disabled FROM tblAuth WHERE Username = \"{ Username }\";", conn))
            {
                conn.Open();
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.HasRows)
                        throw new UnknownUserException();

                    reader.Read();
                    int ID = reader.GetInt32(0);

                    if (reader.GetBoolean(5))
                    {
                        conn.Close();
                        FailedAttempt(ID);
                        throw new AccountDisabledException();
                    }

                    if (reader.GetDateTime(3).Ticks >= GetDateTimeNow_Obj.AddMinutes(-Setting_LockoutTime).Ticks && reader.GetInt32(4) >= Setting_LockoutAttemptCount)
                    {
                        conn.Close();
                        FailedAttempt(ID);
                        throw new AccountLockedException();
                    }
                    else if (reader.GetDateTime(3).Ticks <= GetDateTimeNow_Obj.AddMinutes(-Setting_LockoutTime).Ticks)
                    {
                        FailedAttempt(ID, true, conn, false);
                    }

                    string PasswordHash = HashPassword(Password, reader.GetString(2));

                    if (PasswordHash.Equals(reader.GetString(1)))
                    {
                        // Good Password
                        FailedAttempt(ID, true, conn, false);
                        conn.Close();
                        return GetUser(ID);
                    }
                    else
                    {
                        // Bad Password
                        FailedAttempt(ID, false, conn, false);
                        conn.Close();
                        throw new BadPasswordException();
                    }
                }
            }
        }

        /// <summary>
        /// Indicates that the user has failed a password attempt and will increase the attempt count by 1 each call (Attempts set to 0 if reset is true)
        /// </summary>
        /// <param name="UID">The user that failed auth</param>
        /// <param name="Reset">Defines if the action is to reset the counter</param>
        private bool FailedAttempt(int UID, bool Reset = false, SQLiteConnection Connection = null, bool Close = true)
        {
            if (Connection == null)
                Connection = new SQLiteConnection(SaveManager.SQLiteConnectionString).OpenAndReturn();

            string query_Add = $"UPDATE tblAuth SET Security_LastAttempt = \"{GetDateTimeNow_String}\", Security_Attempts = (SELECT Security_Attempts FROM tblAuth WHERE UID = {UID}) + 1 WHERE UID = {UID};";
            string query_Reset = $"UPDATE tblAuth SET Security_LastAttempt = \"{GetDateTimeNow_String}\", Security_Attempts = 0 WHERE UID = {UID};";
            using (SQLiteTransaction transaction = Connection.BeginTransaction())
                try
                {
                    if (Reset)
                    {
                        using (SQLiteCommand cmd = new SQLiteCommand(query_Reset, Connection, transaction))
                            cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        using (SQLiteCommand cmd = new SQLiteCommand(query_Add, Connection, transaction))
                            cmd.ExecuteNonQuery();
                    }

                    transaction.Commit();

                    if (Close)
                    {
                        Connection.Close();
                        Connection.Dispose();
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    if (Close)
                    {
                        Connection.Close();
                        Connection.Dispose();
                    }
                    return false;
                }
        }
        public bool UnlockAccount(int ID)
        {
            return FailedAttempt(ID, true);
        }
        public bool LockAccount(int ID)
        {
            string query_Lock = $"UPDATE tblAuth SET Security_LastAttempt = \"{GetDateTimeNow_String}\", Security_Attempts = {Setting_LockoutAttemptCount} WHERE UID = {ID};";
            using (SQLiteConnection Connection = new SQLiteConnection(SaveManager.SQLiteConnectionString).OpenAndReturn())
            using (SQLiteTransaction transaction = Connection.BeginTransaction())
                try
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(query_Lock, Connection, transaction))
                        cmd.ExecuteNonQuery();

                    transaction.Commit();
                    Connection.Close();
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Connection.Close();
                    return false;
                }
        }

        public bool AccountState(int ID, bool Enabled)
        {
            string query_Disabled = $"UPDATE tblAuth SET Security_Disabled = 1 WHERE UID = {ID};";
            string query_Enabled = $"UPDATE tblAuth SET Security_Disabled = 0 WHERE UID = {ID};";
            using (SQLiteConnection Connection = new SQLiteConnection(SaveManager.SQLiteConnectionString).OpenAndReturn())
            using (SQLiteTransaction transaction = Connection.BeginTransaction())
                try
                {
                    if (Enabled)
                        using (SQLiteCommand cmd = new SQLiteCommand(query_Enabled, Connection, transaction))
                            cmd.ExecuteNonQuery();
                    else
                        using (SQLiteCommand cmd = new SQLiteCommand(query_Disabled, Connection, transaction))
                            cmd.ExecuteNonQuery();

                    transaction.Commit();
                    Connection.Close();
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Connection.Close();
                    return false;
                }
        }

        public User GetUser(int ID)
        {
            if (HasUser(ID))
                return new User(Manager, ID);
            return null;
        }
        public User GetUser(string Name)
        {
            using (SQLiteConnection conn = new SQLiteConnection(SaveManager.SQLiteConnectionString))
            using (SQLiteCommand cmd = new SQLiteCommand($"SELECT UID FROM tblUsers WHERE Name = \"{ Name }\";", conn))
            {
                conn.Open();
                try
                {
                    User user = new User(Manager, Convert.ToInt32(cmd.ExecuteScalar()));
                    conn.Close();
                    return user;
                }
                catch
                {
                    conn.Close();
                    return null;
                }
            }
        }

        public bool HasUser(int UID)
        {
            using (SQLiteConnection conn = new SQLiteConnection(Manager.SQLiteConnectionString).OpenAndReturn())
                try
                {
                    using (SQLiteCommand cmd = new SQLiteCommand($"SELECT count(UID) AS 'Users' FROM tblUsers WHERE UID = \"{UID}\";", conn))
                        if (Convert.ToInt32(cmd.ExecuteScalar()) >= 1)
                        {
                            conn.Close();
                            return true;
                        }
                        else
                        {
                            conn.Close();
                            return false;
                        }
                }
                catch { return false; }
        }
        public bool HasUser(string Name)
        {
            using (SQLiteConnection conn = new SQLiteConnection(Manager.SQLiteConnectionString).OpenAndReturn())
                try
                {
                    using (SQLiteCommand cmd = new SQLiteCommand($"SELECT count(UID) AS 'Users' FROM tblUsers WHERE Name = \"{Name}\";", conn))
                        if (Convert.ToInt32(cmd.ExecuteScalar()) >= 1)
                        {
                            conn.Close();
                            return true;
                        }
                        else
                        {
                            conn.Close();
                            return false;
                        }
                }
                catch { return false; }
        }

        public string HashPassword(string Password, string Salt)
        {
            PBKDF2 crypto = new PBKDF2();

            return crypto.Compute(Password, Salt);
        }
        public string GenerateSalt(int HashIterations = 100000)
        {
            PBKDF2 crypto = new PBKDF2();
            crypto.HashIterations = HashIterations;

            return crypto.GenerateSalt();
        }
    }
}
