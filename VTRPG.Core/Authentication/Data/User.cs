using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VTRPG.Core.SaveManager.Helper;
using VTRPG.Core.Permissions.Data;

namespace VTRPG.Core.Authentication.Data
{
    public class UserDetails : DataUpdated
    {
        private int _UID;
        public int UID
        {
            get { return _UID; }
            set
            {
                _UID = value;
                UpdatedEvent?.Invoke();
            }
        }

        private string _Name;
        public string Name
        {
            get { return _Name; }
            set
            {
                _Name = value;
                UpdatedEvent?.Invoke();
            }
        }

        private string _Character_Name;
        public string Character_Name
        {
            get { return _Character_Name; }
            set
            {
                _Character_Name = value;
                UpdatedEvent?.Invoke();
            }
        }

        private Color _UserColor;
        public Color UserColor
        {
            get { return _UserColor; }
            set
            {
                _UserColor = value;
                UpdatedEvent?.Invoke();
            }
        }

        private bool _isGM;
        public bool isGM
        {
            get { return _isGM; }
            set
            {
                _isGM = value;
                UpdatedEvent?.Invoke();
            }
        }

        public virtual event UpdatedDataEventHandler UpdatedEvent;
    }

    public class User : UserDetails
    {
        private List<PermissionEntry> _UserPermissions;
        public IEnumerable<PermissionEntry> UserPermissions { get; }

        public void AddUserPermission(string Node, bool Enabled)
        {
            _UserPermissions.Add(new PermissionEntry() { Node = Node, Enabled = Enabled });
            PermissionsUpdatedEvent?.Invoke(Node, PermissionsUpdatedOpperation.Add);
        }
        public void RemoveUserPermission(string Node)
        {
            _UserPermissions.RemoveAll(x => x.Node == Node);
            PermissionsUpdatedEvent?.Invoke(Node, PermissionsUpdatedOpperation.Remove);
        }
        public void UpdateUserPermission(string Node, bool Enabled)
        {
            foreach (PermissionEntry entry in _UserPermissions.FindAll(x => x.Node == Node))
            {
                entry.Enabled = Enabled;
            }
            PermissionsUpdatedEvent?.Invoke(Node, PermissionsUpdatedOpperation.Update);
        }
        public bool HasUserPermission(string Node, bool CheckForEntry = false)
        {
            List<string> Nodes = Node.Split('.').ToList();

            while (true)
            {

                if (_UserPermissions.Find(x => x.Node == Nodes.ToString()) == null)
                {
                    Nodes.RemoveAt(Nodes.Count - 1);

                    if (Nodes.Count <= 0)
                    {
                        return false;
                    }
                    continue;
                }
                else
                {
                    if (CheckForEntry)
                    {
                        return true;
                    }
                    else
                    {
                        if (_UserPermissions.Find(x => x.Node == Nodes.ToString()).Enabled)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
        }

        private List<Group> _Groups;
        public IEnumerable<string> Groups { get; }

        public void AddGroup(string Node, bool Enabled)
        {

        }
        public void RemoveGroup(string Node)
        {

        }
        public bool HasGroupPermission(string Node, bool CheckForEntry = false)
        {
            foreach(Group group in _Groups)
            {
                group.HasPermission
            }
        }

        public bool HasPermission(string Node, bool CheckForEntry = false)
        {

        }

        public event PermissionsUpdated PermissionsUpdatedEvent;

    }

    internal class Login : DataUpdated
    {
        private int _UID;
        public int UID
        {
            get { return _UID; }
            set
            {
                _UID = value;
                UpdatedEvent?.Invoke();
            }
        }

        private string _Username;
        public string Username
        {
            get { return _Username; }
            set
            {
                _Username = value;
                UpdatedEvent?.Invoke();
            }
        }

        private string _Salt;
        public string Salt
        {
            get { return _Salt; }
            set
            {
                _Salt = value;
                UpdatedEvent?.Invoke();
            }
        }

        private string _Hash;
        public string Hash
        {
            get { return _Hash; }
            set
            {
                _Hash = value;
                UpdatedEvent?.Invoke();
            }
        }

        private int _Security_Attempts;
        public int Security_Attempts
        {
            get { return _Security_Attempts; }
            set
            {
                _Security_Attempts = value;
                UpdatedEvent?.Invoke();
            }
        }

        private string _Security_LastAttempt;
        public string Security_LastAttempt
        {
            get { return _Security_LastAttempt; }
            set
            {
                _Security_LastAttempt = value;
                UpdatedEvent?.Invoke();
            }
        }

        private int _Security_Disabled;
        public int Security_Disabled
        {
            get { return _Security_Disabled; }
            set
            {
                _Security_Disabled = value;
                UpdatedEvent?.Invoke();
            }
        }

        public event UpdatedDataEventHandler UpdatedEvent;
    }
}
