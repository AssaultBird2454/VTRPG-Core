using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace VTRPG.Core.Authentication.Data
{
    public class User
    {
        private SaveManager.SaveManager _Manager;

        public User(SaveManager.SaveManager Manager, int ID)
        {
            UID = ID;
            _Manager = Manager;
        }

        public int UID { get; private set; }
        public string Name
        {
            get
            {
                using (SQLiteConnection conn = new SQLiteConnection(_Manager.SQLiteConnectionString).OpenAndReturn())
                using (SQLiteCommand cmd = new SQLiteCommand($"SELECT Name FROM tblUsers WHERE UID = {UID};", conn))
                {
                    string name = cmd.ExecuteScalar().ToString();
                    conn.Close();
                    return name;
                }
            }
            set
            {
                using (SQLiteConnection conn = new SQLiteConnection(_Manager.SQLiteConnectionString).OpenAndReturn())
                using (SQLiteCommand cmd = new SQLiteCommand($"UPDATE tblUsers SET Name = \"{value}\" WHERE UID = {UID};", conn))
                {
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
        }
        public string Character_Name
        {
            get
            {
                using (SQLiteConnection conn = new SQLiteConnection(_Manager.SQLiteConnectionString).OpenAndReturn())
                using (SQLiteCommand cmd = new SQLiteCommand($"SELECT Character_Name FROM tblUsers WHERE UID = {UID};", conn))
                {
                    string name = cmd.ExecuteScalar().ToString();
                    conn.Close();
                    return name;
                }
            }
            set
            {
                using (SQLiteConnection conn = new SQLiteConnection(_Manager.SQLiteConnectionString).OpenAndReturn())
                using (SQLiteCommand cmd = new SQLiteCommand($"UPDATE tblUsers SET Character_Name = \"{value}\" WHERE UID = {UID};", conn))
                {
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
        }
        public Color UserColor {
            get
            {
                using (SQLiteConnection conn = new SQLiteConnection(_Manager.SQLiteConnectionString).OpenAndReturn())
                using (SQLiteCommand cmd = new SQLiteCommand($"SELECT UserColor FROM tblUsers WHERE UID = {UID};", conn))
                {
                    Color color = (Color)new ColorConverter().ConvertFromString(cmd.ExecuteScalar().ToString());
                    conn.Close();
                    return color;
                }
            }
            set
            {
                using (SQLiteConnection conn = new SQLiteConnection(_Manager.SQLiteConnectionString).OpenAndReturn())
                using (SQLiteCommand cmd = new SQLiteCommand($"UPDATE tblUsers SET UserColor = \"{new ColorConverter().ConvertToString(value)}\" WHERE UID = {UID};", conn))
                {
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
        }
        public bool isGM {
            get
            {
                using (SQLiteConnection conn = new SQLiteConnection(_Manager.SQLiteConnectionString).OpenAndReturn())
                using (SQLiteCommand cmd = new SQLiteCommand($"SELECT isGM FROM tblUsers WHERE UID = {UID};", conn))
                {
                    bool gm = (bool)cmd.ExecuteScalar();
                    conn.Close();
                    return gm;
                }
            }
            set
            {
                using (SQLiteConnection conn = new SQLiteConnection(_Manager.SQLiteConnectionString).OpenAndReturn())
                using (SQLiteCommand cmd = new SQLiteCommand($"UPDATE isGM SET UserColor = \"{value}\" WHERE UID = {UID};", conn))
                {
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
        }

        // Firebase UID (Planned for website intergration in the future)
    }
}
