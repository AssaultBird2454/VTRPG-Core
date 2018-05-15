using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VTRPG.Core.Permissions
{
    public class PermissionsManager
    {
        #region Base
        public PermissionsManager(SaveManager.SaveManager SaveManager)
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

        public void InitSaveFile()
        {
            using (SQLiteConnection conn = new SQLiteConnection(Manager.SQLiteConnectionString).OpenAndReturn())
            using (SQLiteTransaction transaction = conn.BeginTransaction())
            {
                try
                {
                    // Init Groups Table
                    using (SQLiteCommand cmd = new SQLiteCommand(Permissions.Save_Create_tblGroups, conn, transaction))
                        cmd.ExecuteNonQuery();
                    // Init Groups Permissions Table
                    using (SQLiteCommand cmd = new SQLiteCommand(Permissions.Save_Create_tblGroupPermissions, conn, transaction))
                        cmd.ExecuteNonQuery();
                    // Init Group Members Table
                    using (SQLiteCommand cmd = new SQLiteCommand(Permissions.Save_Create_tblGroupMembers, conn, transaction))
                        cmd.ExecuteNonQuery();
                    // Init User Permissions Table (Depends on the Auth Manager)
                    using (SQLiteCommand cmd = new SQLiteCommand(Permissions.Save_Create_tblUserPermissions, conn, transaction))
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

        #region Permissions Checks

        #endregion

        #region Groups
        public Data.Group CreateGroup(string Name, string Desc)
        {
            using (SQLiteConnection conn = new SQLiteConnection(Manager.SQLiteConnectionString).OpenAndReturn())
            using (SQLiteTransaction transaction = conn.BeginTransaction())
            {
                try
                {
                    using (SQLiteCommand cmd = new SQLiteCommand($"INSERT INTO tblGroups (\"Name\", \"Description\") VALUES (\"{Name}\", \"{Desc}\")", conn, transaction))
                        cmd.ExecuteNonQuery();

                    int ID = 0;
                    using (SQLiteCommand cmd = new SQLiteCommand("SELECT last_insert_rowid();", conn, transaction))
                        ID = Convert.ToInt32(cmd.ExecuteScalar());

                    transaction.Commit();
                    return new Data.Group(Manager, ID);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }
        #endregion

        #region Users

        #endregion
    }
}
