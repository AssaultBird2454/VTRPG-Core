using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VTRPG.Core.Authentication.Data
{
    public class User
    {
        public int UID { get; set; }
        public string Name { get; set; }
        public string Character_Name { get; set; }
        public Color UserColor { get; set; }
        
        // Firebase UID (Planned for website intergration in the future)
    }
}
