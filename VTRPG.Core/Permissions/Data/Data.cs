﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VTRPG.Core.Permissions.Data
{
    public class UserAlreadyMemberException : Exception { }
    public class UserNotMemberException : Exception { }

    public enum PermissionsUpdatedOpperation { Add, Remove, Update }
    public delegate void PermissionsUpdated(string Node, PermissionsUpdatedOpperation Opperation);

    public class PermissionEntry
    {
        public string Node { get; set; }
        public bool Enabled { get; set; }
    }
}
