using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VTRPG.Core.Authentication.Data
{
    /// <summary>
    /// Defines different states or errors that can occure wile registering an account
    /// </summary>
    public enum RegisterState
    {
        OK,
        UsernameTaken,
        SaltTaken,
        Error,
        Unknown
    }

    /// <summary>
    /// Defines different states or errors that can occure wile authenticating
    /// </summary>
    public enum AuthState
    {
        OK,
        UnknownUser,
        BadPassword,
        Locked,
        Disabled,
        Error,
        Unknown
    }
}
