using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VTRPG.Core.Authentication;
using VTRPG.Core.Authentication.Data;

namespace VTRPG.SaveEditor.UI.Auth
{
    /// <summary>
    /// Interaction logic for User.xaml
    /// </summary>
    public partial class User : Window
    {
        int UserID = 0;
        AuthenticationManager Manager;
        User_DB UserData;
        Login_DB LoginData;

        public User(AuthenticationManager _Manager, int _UserID = 0)
        {
            UserID = _UserID;
            Manager = _Manager;

            InitializeComponent();
            Load();
        }

        public void Load()
        {
            if (UserID != 0)
            {
                UserData = Manager.GetUserDB(UserID);
                LoginData = Manager.GetLoginDB(UserID);

                // User Details
                // -=-=-=-=-=-=-=-=-=-=-=-
                txtUID.Text = UserID.ToString();// UID
                txtUserName.Text = UserData.Name;// User Name
                txtCharName.Text = UserData.Character_Name;// Character Name this user plays as
                cpiUserColor.SelectedColor = new Color()
                {
                    R = UserData.UserColor.R,
                    G = UserData.UserColor.G,
                    B = UserData.UserColor.B,
                    A = UserData.UserColor.A
                };// Color to identify this user
                chbIsGM.IsChecked = UserData.isGM;// Is the user a GM user?
                // Account Details
                // -=-=-=-=-=-=-=-=-=-=-=-
                txtLoginName.Text = LoginData.Username;
                chbEnabled.IsChecked = !LoginData.Disabled;
                chbLocked.IsChecked = LoginData.Locked;

                if (chbLocked.IsChecked == true)
                {
                    lblLockoutDuration.Content = $"Locked Until:\n{LoginData.LockedUntil.ToLongTimeString()}";
                    lblLockoutDuration.Visibility = Visibility.Visible;
                    btnUnlock.IsEnabled = true;
                }
                else
                {
                    lblLockoutDuration.Visibility = Visibility.Hidden;
                    btnUnlock.IsEnabled = false;
                }
            }
            // Ignore
        }

        public void Save()
        {
            if (UserID == 0)
            {
                // Create
            }
            else
            {
                // Save
            }
        }

        public void LoadUserPermissions()
        {

        }
        public void LoadUserGroups()
        {

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Save();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnReload_Click(object sender, RoutedEventArgs e)
        {
            Load();
        }
    }
}
