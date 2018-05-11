using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace VTRPG.Core.SaveManager
{
    public class SaveManager
    {
        public SaveManager(string SaveFile = "")
        {
            _SaveFile = SaveFile;
        }

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
                // Event
            }
        }
        public string SQLiteConnectionString { get { return $"Data Source={SaveFile};"; } }

        /// <summary>
        /// Creates a new savefile
        /// </summary>
        public void Init()
        {
            using (SQLiteConnection conn = new SQLiteConnection())
            using (SQLiteTransaction trans = conn.BeginTransaction())
            {
                conn.Open();
                using (SQLiteCommand cmd = new SQLiteCommand("", conn, trans))
                {

                }
                using (SQLiteCommand cmd = new SQLiteCommand("", conn, trans))
                {

                }
                conn.Close();
            }
        }
    }
}
