using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace VTRPG.Core.Authentication.Data
{
    public class User_DB : IUser
    {
        private SaveManager.SaveManager _Manager;

        internal User_DB(SaveManager.SaveManager Manager, int ID)
        {
            UID = ID;
            _Manager = Manager;
        }

        #region Data
        public int UID { get; private set; }
        public string Name
        {
            get
            {
                using (SQLiteConnection conn = new SQLiteConnection(_Manager.SQLiteConnectionString).OpenAndReturn())
                using (SQLiteCommand cmd = new SQLiteCommand($"SELECT Name FROM tblUsers WHERE UID = {UID};", conn))
                {
                    string name = cmd.ExecuteScalar().ToString();
                    conn.Close();
                    return name;
                }
            }
            set
            {
                using (SQLiteConnection conn = new SQLiteConnection(_Manager.SQLiteConnectionString).OpenAndReturn())
                using (SQLiteCommand cmd = new SQLiteCommand($"UPDATE tblUsers SET Name = \"{value}\" WHERE UID = {UID};", conn))
                {
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
        }
        public string Character_Name
        {
            get
            {
                using (SQLiteConnection conn = new SQLiteConnection(_Manager.SQLiteConnectionString).OpenAndReturn())
                using (SQLiteCommand cmd = new SQLiteCommand($"SELECT Character_Name FROM tblUsers WHERE UID = {UID};", conn))
                {
                    string name = cmd.ExecuteScalar().ToString();
                    conn.Close();
                    return name;
                }
            }
            set
            {
                using (SQLiteConnection conn = new SQLiteConnection(_Manager.SQLiteConnectionString).OpenAndReturn())
                using (SQLiteCommand cmd = new SQLiteCommand($"UPDATE tblUsers SET Character_Name = \"{value}\" WHERE UID = {UID};", conn))
                {
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
        }
        public Color UserColor
        {
            get
            {
                using (SQLiteConnection conn = new SQLiteConnection(_Manager.SQLiteConnectionString).OpenAndReturn())
                using (SQLiteCommand cmd = new SQLiteCommand($"SELECT UserColor FROM tblUsers WHERE UID = {UID};", conn))
                {
                    Color color = (Color)new ColorConverter().ConvertFromString(cmd.ExecuteScalar().ToString());
                    conn.Close();
                    return color;
                }
            }
            set
            {
                using (SQLiteConnection conn = new SQLiteConnection(_Manager.SQLiteConnectionString).OpenAndReturn())
                using (SQLiteCommand cmd = new SQLiteCommand($"UPDATE tblUsers SET UserColor = \"{new ColorConverter().ConvertToString(value)}\" WHERE UID = {UID};", conn))
                {
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
        }
        public bool isGM
        {
            get
            {
                using (SQLiteConnection conn = new SQLiteConnection(_Manager.SQLiteConnectionString).OpenAndReturn())
                using (SQLiteCommand cmd = new SQLiteCommand($"SELECT isGM FROM tblUsers WHERE UID = {UID};", conn))
                {
                    bool gm = (bool)cmd.ExecuteScalar();
                    conn.Close();
                    return gm;
                }
            }
            set
            {
                using (SQLiteConnection conn = new SQLiteConnection(_Manager.SQLiteConnectionString).OpenAndReturn())
                using (SQLiteCommand cmd = new SQLiteCommand($"UPDATE isGM SET UserColor = \"{value}\" WHERE UID = {UID};", conn))
                {
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
        }
        #endregion

        #region Permissions
        public IReadOnlyList<KeyValuePair<string, bool>> Permissions
        {
            get
            {
                using (SQLiteConnection conn = new SQLiteConnection(_Manager.SQLiteConnectionString).OpenAndReturn())
                {
                    List<KeyValuePair<string, bool>> Perms = new List<KeyValuePair<string, bool>>();
                    using (SQLiteCommand cmd = new SQLiteCommand($"SELECT Node, Allow FROM tblUserPermissions WHERE UID = {UID};", conn))
                    {
                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                            while (reader.Read())
                            {
                                Perms.Add(new KeyValuePair<string, bool>(reader.GetString(0), reader.GetBoolean(1)));
                            }
                    }

                    foreach (VTRPG.Core.Permissions.Data.Group group in _Manager.PermissionsManager.Groups())
                        if (group.HasMamber(UID))
                            using (SQLiteCommand cmd = new SQLiteCommand($"SELECT Node, Allow FROM tblGroupPermissions WHERE GID = {group.GID};", conn))
                            {
                                using (SQLiteDataReader reader = cmd.ExecuteReader())
                                    while (reader.Read())
                                    {
                                        Perms.Add(new KeyValuePair<string, bool>(reader.GetString(0), reader.GetBoolean(1)));
                                    }
                            }

                    conn.Close();
                    return Perms;
                }
            }
        }

        public bool AddPermission(string Node, bool Allow = true)
        {
            if (HasPermissionEntry(Node))
                return false;

            using (SQLiteConnection conn = new SQLiteConnection(_Manager.SQLiteConnectionString).OpenAndReturn())
            using (SQLiteTransaction transaction = conn.BeginTransaction())
            using (SQLiteCommand cmd = new SQLiteCommand($"INSERT INTO tblUserPermissions (\"UID\", \"Node\", \"Allow\") VALUES ({UID}, \"{Node}\", {Convert.ToInt32(Allow)});", conn, transaction))
            {
                try
                {
                    int Changes = cmd.ExecuteNonQuery();
                    transaction.Commit();
                    conn.Close();

                    if (Changes == 0)
                        return false;
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    conn.Close();
                    throw ex;
                }
            }
        }
        public bool RemovePermission(string Node)
        {
            if (!HasPermissionEntry(Node))
                return false;

            using (SQLiteConnection conn = new SQLiteConnection(_Manager.SQLiteConnectionString).OpenAndReturn())
            using (SQLiteTransaction transaction = conn.BeginTransaction())
            using (SQLiteCommand cmd = new SQLiteCommand($"DELETE FROM tblUserPermissions WHERE UID = {UID} AND Node = \"{Node}\";", conn, transaction))
            {
                try
                {
                    int Changes = cmd.ExecuteNonQuery();
                    transaction.Commit();
                    conn.Close();

                    if (Changes == 0)
                        return false;
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    conn.Close();
                    throw ex;
                }
            }
        }
        public bool HasPermission(string Node)
        {
            List<string> Nodes = Node.Split('.').ToList();

            using (SQLiteConnection conn = new SQLiteConnection(_Manager.SQLiteConnectionString).OpenAndReturn())
                while (true)
                {
                    using (SQLiteCommand cmd = new SQLiteCommand($"SELECT Node FROM tblUserPermissions WHERE UID = {UID} AND Node = \"{Core.Permissions.PermissionsManager.CombineNodes(Nodes)}\" AND Allow = 1;", conn))
                    {

                        if (cmd.ExecuteScalar() == null)
                        {
                            Nodes.RemoveAt(Nodes.Count - 1);

                            if (Nodes.Count <= 0)
                            {
                                conn.Close();
                                return false;
                            }

                            continue;
                        }
                        else
                        {
                            conn.Close();
                            return true;
                        }
                    }
                }
        }
        public bool HasPermissionEntry(string Node)
        {
            using (SQLiteConnection conn = new SQLiteConnection(_Manager.SQLiteConnectionString).OpenAndReturn())
            using (SQLiteCommand cmd = new SQLiteCommand($"SELECT Node FROM tblUserPermissions WHERE UID = {UID} AND Node = \"{Node}\";", conn))
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
        }
        public bool UpdatePermission(string Node, bool Allow)
        {
            if (!HasPermissionEntry(Node))
                return false;

            using (SQLiteConnection conn = new SQLiteConnection(_Manager.SQLiteConnectionString).OpenAndReturn())
            using (SQLiteTransaction transaction = conn.BeginTransaction())
            using (SQLiteCommand cmd = new SQLiteCommand($"UPDATE tblUserPermissions SET \"Allow\" = {Convert.ToInt32(Allow)} WHERE UID = {UID} AND Node = \"{Node}\";", conn, transaction))
            {
                try
                {
                    int Changes = cmd.ExecuteNonQuery();
                    transaction.Commit();
                    conn.Close();

                    if (Changes == 0)
                        return false;
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    conn.Close();
                    throw ex;
                }
            }
        }
        // Firebase UID (Planned for website intergration in the future)
        #endregion
    }
    public class User : IUser
    {
        internal User(SaveManager.SaveManager Manager, int ID)
        {
            UID = ID;
        }

        #region Data
        public int UID { get; set; }
        public string Name { get; set; }
        public string Character_Name { get; set; }
        public Color UserColor { get; set; }
        public bool isGM { get; set; }
        #endregion

        #region Permissions
        public IReadOnlyList<KeyValuePair<string, bool>> Permissions { get; }

        public bool AddPermission(string Node, bool Allow = true)
        {
            throw new NotImplementedException();
        }
        public bool RemovePermission(string Node)
        {
            throw new NotImplementedException();
        }
        public bool HasPermission(string Node)
        {
            throw new NotImplementedException();
        }
        public bool HasPermissionEntry(string Node)
        {
            throw new NotImplementedException();
        }
        public bool UpdatePermission(string Node, bool Allow)
        {
            throw new NotImplementedException();
        }
        // Firebase UID (Planned for website intergration in the future)
        #endregion
    }
    public interface IUser
    {
        #region Data
        int UID { get; }
        string Name { get; set; }
        string Character_Name { get; set; }
        Color UserColor { get; set; }
        bool isGM { get; set; }
        #endregion

        #region Permissions
        IReadOnlyList<KeyValuePair<string, bool>> Permissions { get; }

        bool AddPermission(string Node, bool Allow = true);
        bool RemovePermission(string Node);
        bool HasPermission(string Node);
        bool HasPermissionEntry(string Node);
        bool UpdatePermission(string Node, bool Allow);
        // Firebase UID (Planned for website intergration in the future)
        #endregion
    }
}
