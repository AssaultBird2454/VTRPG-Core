using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace VTRPG.Core.SettingsManager
{
    public class SettingsManager
    {
        public SettingsManager(SaveManager.SaveManager SaveManager)
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
        internal void InitSaveFile()
        {
            using (SQLiteConnection conn = new SQLiteConnection(Manager.SQLiteConnectionString).OpenAndReturn())
            using (SQLiteTransaction transaction = conn.BeginTransaction())
            {
                try
                {
                    // Init Settings Table
                    using (SQLiteCommand cmd = new SQLiteCommand(Settings.Save_Create_tblSettings, conn, transaction))
                        cmd.ExecuteNonQuery();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();// Rollback
                    throw ex;
                }
            }
        }

        private void Manager_SaveFileChangedEvent(string File)
        {

        }

        public object GetValue(string Name)
        {
            using (SQLiteConnection conn = new SQLiteConnection(Manager.SQLiteConnectionString).OpenAndReturn())
                try
                {
                    using (SQLiteCommand cmd = new SQLiteCommand($"SELECT Value FROM tblSettings WHERE Key = \"{Name}\";", conn))
                    {
                        object obj = cmd.ExecuteScalar();

                        conn.Close();

                        return obj;
                    }
                }
                catch
                {
                    conn.Close();
                    return null;
                }
        }
        public bool SetValue(string Name, object Value)
        {
            using (SQLiteConnection conn = new SQLiteConnection(Manager.SQLiteConnectionString).OpenAndReturn())
            using (SQLiteTransaction transaction = conn.BeginTransaction())
                try
                {
                    if (HasSetting(Name))
                    {
                        using (SQLiteCommand cmd = new SQLiteCommand($"UPDATE tblSettings SET Value = \"{Value}\" WHERE Key = \"{Name}\";", conn, transaction))
                            cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        using (SQLiteCommand cmd = new SQLiteCommand($"INSERT INTO tblSettings VALUES (\"{Name}\", \"{Value}\")", conn, transaction))
                            cmd.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    conn.Close();
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    conn.Close();
                    return false;
                }
        }
        public bool HasSetting(string Name)
        {
            using (SQLiteConnection conn = new SQLiteConnection(Manager.SQLiteConnectionString).OpenAndReturn())
                try
                {
                    using (SQLiteCommand cmd = new SQLiteCommand($"SELECT count(Key) AS 'Entrys' FROM tblSettings WHERE Key = \"{Name}\";", conn))
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
    }
}
