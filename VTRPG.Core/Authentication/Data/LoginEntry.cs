using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VTRPG.Core.Authentication.Data
{
    public class LoginEntry
    {
        public int UID { get; set; }
        public string Username { get; set; }
        public string Salt { get; set; }
        public string Hash { get; set; }
        public bool Disabled { get; set; }
        public bool Locked { get; set; }
    }
}
