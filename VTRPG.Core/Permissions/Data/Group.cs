using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VTRPG.Core.Permissions.Data
{
    public class Group
    {
        private SaveManager.SaveManager _Manager;

        internal Group(SaveManager.SaveManager Manager, int ID)
        {
            _Manager = Manager;
            GID = ID;
        }

        public int GID { get; private set; }
        public string Name
        {
            get
            {
                using (SQLiteConnection conn = new SQLiteConnection(_Manager.SQLiteConnectionString).OpenAndReturn())
                using (SQLiteCommand cmd = new SQLiteCommand($"SELECT Name FROM tblGroups WHERE GID = {GID};", conn))
                {
                    string name = cmd.ExecuteScalar().ToString();
                    conn.Close();
                    return name;
                }
            }
            set
            {
                using (SQLiteConnection conn = new SQLiteConnection(_Manager.SQLiteConnectionString).OpenAndReturn())
                using (SQLiteCommand cmd = new SQLiteCommand($"UPDATE tblGroups SET Name = \"{value}\" WHERE GID = {GID};", conn))
                {
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
        }

        public string Description
        {
            get
            {
                using (SQLiteConnection conn = new SQLiteConnection(_Manager.SQLiteConnectionString).OpenAndReturn())
                using (SQLiteCommand cmd = new SQLiteCommand($"SELECT Description FROM tblGroups WHERE GID = {GID};", conn))
                {
                    string name = cmd.ExecuteScalar().ToString();
                    conn.Close();
                    return name;
                }
            }
            set
            {
                using (SQLiteConnection conn = new SQLiteConnection(_Manager.SQLiteConnectionString).OpenAndReturn())
                using (SQLiteCommand cmd = new SQLiteCommand($"UPDATE tblGroups SET Description = \"{value}\" WHERE GID = {GID};", conn))
                {
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
        }

        public IEnumerable<Authentication.Data.User> Members
        {
            get
            {
                using (SQLiteConnection conn = new SQLiteConnection(_Manager.SQLiteConnectionString).OpenAndReturn())
                using (SQLiteCommand cmd = new SQLiteCommand($"SELECT UID FROM tblGroupMembers WHERE GID = {GID};", conn))
                {
                    List<Authentication.Data.User> Users = new List<Authentication.Data.User>();

                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                        while (reader.Read())
                        {
                            Users.Add(new Authentication.Data.User(_Manager, reader.GetInt32(0)));
                        }

                    conn.Close();
                    return Users;
                }
            }
        }

        public bool AddMember(int UID)
        {
            if (!_Manager.AuthManager.HasUser(UID))
                throw new Authentication.Data.UnknownUserException();

            if (HasMamber(UID))
                throw new UserAlreadyMemberException();

            using (SQLiteConnection conn = new SQLiteConnection(_Manager.SQLiteConnectionString).OpenAndReturn())
            using (SQLiteTransaction transaction = conn.BeginTransaction())
            {
                try
                {
                    using (SQLiteCommand cmd = new SQLiteCommand($"INSERT INTO tblGroupMembers (\"GID\", \"UID\") VALUES ({GID}, {UID})", conn, transaction))
                        cmd.ExecuteNonQuery();
                    transaction.Commit();

                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }
        public bool RemoveMember(int UID)
        {
            if (!HasMamber(UID))
                throw new UserNotMemberException();

            using (SQLiteConnection conn = new SQLiteConnection(_Manager.SQLiteConnectionString).OpenAndReturn())
            using (SQLiteTransaction transaction = conn.BeginTransaction())
            {
                try
                {
                    using (SQLiteCommand cmd = new SQLiteCommand($"DELETE FROM tblGroupMembers WHERE \"GID\" = {GID} AND  \"UID\" = {UID}", conn, transaction))
                        cmd.ExecuteNonQuery();
                    transaction.Commit();

                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }
        public bool HasMamber(int UID)
        {
            if (!_Manager.AuthManager.HasUser(UID))
                throw new Authentication.Data.UnknownUserException();

            using (SQLiteConnection conn = new SQLiteConnection(_Manager.SQLiteConnectionString).OpenAndReturn())
            {
                try
                {
                    int Count = 0;
                    using (SQLiteCommand cmd = new SQLiteCommand($"SELECT COUNT(UID) as 'Count' FROM tblGroupMembers WHERE \"GID\" = {GID} AND  \"UID\" = {UID}", conn))
                        Count = Convert.ToInt32(cmd.ExecuteScalar());

                    if (Count == 0)
                        return false;
                    return true;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public IReadOnlyList<KeyValuePair<string, bool>> Permissions
        {
            get
            {
                using (SQLiteConnection conn = new SQLiteConnection(_Manager.SQLiteConnectionString).OpenAndReturn())
                using (SQLiteCommand cmd = new SQLiteCommand($"SELECT Node, Allow FROM tblGroupPermissions WHERE GID = {GID};", conn))
                {
                    List<KeyValuePair<string, bool>> Perms = new List<KeyValuePair<string, bool>>();

                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                        while (reader.Read())
                        {
                            Perms.Add(new KeyValuePair<string, bool>(reader.GetString(0), reader.GetBoolean(1)));
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
            using (SQLiteCommand cmd = new SQLiteCommand($"INSERT INTO tblGroupPermissions (\"GID\", \"Node\", \"Allow\") VALUES ({GID}, \"{Node}\", {Convert.ToInt32(Allow)});", conn, transaction))
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
            using (SQLiteCommand cmd = new SQLiteCommand($"DELETE FROM tblGroupPermissions WHERE GID = {GID} AND Node = \"{Node}\";", conn, transaction))
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
                    using (SQLiteCommand cmd = new SQLiteCommand($"SELECT Node FROM tblGroupPermissions WHERE GID = {GID} AND Node = \"{PermissionsManager.CombineNodes(Nodes)}\" AND Allow = 1;", conn))
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
            using (SQLiteCommand cmd = new SQLiteCommand($"SELECT Node FROM tblGroupPermissions WHERE GID = {GID} AND Node = \"{Node}\";", conn))
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
            using (SQLiteCommand cmd = new SQLiteCommand($"UPDATE tblGroupPermissions SET \"Allow\" = {Convert.ToInt32(Allow)} WHERE GID = {GID} AND Node = \"{Node}\";", conn, transaction))
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
    }
}
