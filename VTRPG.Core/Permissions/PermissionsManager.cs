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

                    // Init UserDeleted Trigger (Depends on the Auth Manager)
                    using (SQLiteCommand cmd = new SQLiteCommand(Permissions.Save_Create_trgPermissions_UserDeleted, conn, transaction))
                        cmd.ExecuteNonQuery();
                    // Init GroupDeleted Trigger
                    using (SQLiteCommand cmd = new SQLiteCommand(Permissions.Save_Create_trgPermissions_GroupDeleted, conn, transaction))
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
        public IReadOnlyList<Data.Group> Groups()
        {
            using (SQLiteConnection conn = new SQLiteConnection(Manager.SQLiteConnectionString).OpenAndReturn())
            using (SQLiteCommand cmd = new SQLiteCommand("SELECT GID FROM tblGroups", conn))
            {
                try
                {
                    List<Data.Group> GroupList = new List<Data.Group>();
                    using (SQLiteDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                            GroupList.Add(new Data.Group(Manager, dr.GetInt32(0)));
                    }

                    conn.Close();
                    return GroupList;
                }
                catch (Exception ex)
                {
                    conn.Close();
                    throw ex;
                }
            }
        }

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

        public Data.Group GetGroup(int GID)
        {
            if (!GroupExists(GID))
                return null;

            // Get Group
            return new Data.Group(Manager, GID);// ID
        }
        public Data.Group GetGroup(string Name)
        {
            if (!GroupExists(Name))
                return null;

            using (SQLiteConnection conn = new SQLiteConnection(Manager.SQLiteConnectionString).OpenAndReturn())
            using (SQLiteCommand cmd = new SQLiteCommand($"SELECT GID FROM tblGroups WHERE Name = \"{Name}\";", conn))
            {
                try
                {
                    int ID = Convert.ToInt32(cmd.ExecuteScalar());

                    conn.Close();
                    return new Data.Group(Manager, ID);
                }
                catch
                {
                    conn.Close();
                    return null;
                }
            }
        }

        public bool DeleteGroup(int GID)
        {
            if (!GroupExists(GID))
                return false;

            using (SQLiteConnection conn = new SQLiteConnection(Manager.SQLiteConnectionString).OpenAndReturn())
            using (SQLiteTransaction transaction = conn.BeginTransaction())
            using (SQLiteCommand cmd = new SQLiteCommand($"DELETE FROM tblGroups WHERE GID = {GID}", conn, transaction))
            {
                try
                {
                    if (cmd.ExecuteNonQuery() >= 1)
                    {
                        transaction.Commit();
                        conn.Close();
                        return true;
                    }
                    else
                    {
                        transaction.Commit();
                        conn.Close();
                        return false;
                    }
                }
                catch
                {
                    transaction.Rollback();
                    conn.Close();
                    return false;
                }
            }
        }

        public bool GroupExists(int GID)
        {
            using (SQLiteConnection conn = new SQLiteConnection(Manager.SQLiteConnectionString).OpenAndReturn())
            using (SQLiteCommand cmd = new SQLiteCommand($"SELECT GID FROM tblGroups WHERE GID = {GID};", conn))
            {
                try
                {
                    if (cmd.ExecuteScalar() == null)
                    {
                        conn.Close();
                        return false;
                    }
                    else
                    {
                        conn.Close();
                        return true;
                    }
                }
                catch
                {
                    conn.Close();
                    return false;
                }
            }
        }
        public bool GroupExists(string Name)
        {
            using (SQLiteConnection conn = new SQLiteConnection(Manager.SQLiteConnectionString).OpenAndReturn())
            using (SQLiteCommand cmd = new SQLiteCommand($"SELECT GID FROM tblGroups WHERE Name = \"{Name}\";", conn))
            {
                try
                {
                    if (cmd.ExecuteScalar() == null)
                    {
                        conn.Close();
                        return false;
                    }
                    else
                    {
                        conn.Close();
                        return true;
                    }
                }
                catch
                {
                    conn.Close();
                    return false;
                }
            }
        }
        #endregion

        #region Users

        #endregion

        #region Helper
        public static string CombineNodes(List<string> Node)
        {
            string CheckNode = "";
            for (int i = 0; i <= Node.Count - 1; i++)
            {
                CheckNode += Node[i];
                if (i < Node.Count - 1)
                    CheckNode += '.';
            }

            return CheckNode;
        }
        #endregion
    }
}
