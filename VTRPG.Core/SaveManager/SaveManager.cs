using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace VTRPG.Core.SaveManager
{
    public delegate void SaveFileChangedEventHandeler(string File);
    public delegate void SettingsManagerChangedEventHandeler(SettingsManager.SettingsManager Mgr);
    public delegate void AuthManagerChangedEventHandeler(Authentication.AuthenticationManager Mgr);
  
    public class SaveManager
    {
        public SaveManager(string SaveFile = "")
        {
            _SaveFile = SaveFile;
            SettingsManager = new SettingsManager.SettingsManager(this);
            AuthManager = new Authentication.AuthenticationManager(this);
        }

        public event SaveFileChangedEventHandeler SaveFileChangedEvent;
        public event SettingsManagerChangedEventHandeler SettingsManagerChangedEvent;
        public event AuthManagerChangedEventHandeler AuthManagerChangedEvent;

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

        public void InitSave()
        {
            SettingsManager.InitSaveFile();
            AuthManager.InitSaveFile();
        }

        private SettingsManager.SettingsManager _SettingsManager;
        public SettingsManager.SettingsManager SettingsManager
        {
            get { return _SettingsManager; }
            private set
            {
                _SettingsManager = value;
                SettingsManagerChangedEvent?.Invoke(value);
            }
        }

        private Authentication.AuthenticationManager _AuthManager;
        public Authentication.AuthenticationManager AuthManager
        {
            get { return _AuthManager; }
            private set
            {
                _AuthManager = value;
                AuthManagerChangedEvent?.Invoke(value);
            }
        }
    }
}
