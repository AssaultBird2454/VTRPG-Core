using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace VTRPG.Core.SaveManager
{
    public delegate void SaveFileChangedEventHandeler(string File);

    public class SaveManager
    {
        public SaveManager(string SaveFile = "")
        {
            _SaveFile = SaveFile;
        }

        public event SaveFileChangedEventHandeler SaveFileChangedEvent;

        private string _SaveFile = "";
        public string SaveFile
        {
            get
            {
                return _SaveFile;
            }
            set
            {
                SaveFile = value;
                SaveFileChangedEvent?.Invoke(value);
            }
        }
        public string SQLiteConnectionString { get { return $"Data Source={SaveFile};"; } }
    }
}
