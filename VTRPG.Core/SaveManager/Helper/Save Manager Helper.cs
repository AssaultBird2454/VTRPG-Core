using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VTRPG.Core.SaveManager.Helper
{
    public delegate void UpdatedDataEventHandler();

    public interface DataUpdated
    {
        event UpdatedDataEventHandler UpdatedEvent;
    }
}
