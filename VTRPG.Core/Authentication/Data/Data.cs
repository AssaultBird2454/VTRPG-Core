using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VTRPG.Core.Authentication.Data
{
    public class UsernameTakenException : Exception { }
    public class RegristrationErrorException : Exception
    {
        public Exception ex;
    }

    public class UnknownUserException : Exception { }
    public class BadPasswordException : Exception { }
    public class AccountLockedException : Exception { }
    public class AccountDisabledException : Exception { }
}
