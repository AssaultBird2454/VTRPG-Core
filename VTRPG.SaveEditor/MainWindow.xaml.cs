using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using VTRPG.Core.SaveManager;

namespace VTRPG.SaveEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SaveManager SaveManager;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void mitNewCampaign_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.CheckPathExists = false;
            dialog.CheckFileExists = false;
            dialog.Title = "Create a new VTRPG Campaign Save";
            dialog.DefaultExt = ".vtrpg";
            dialog.Filter = "Virtual TRPG Campaign Save (*.vtrpg)|*.vtrpg|All files (*.*)|*.*";

            bool? selected = dialog.ShowDialog();

            if (selected == true)
            {
                SaveManager = new SaveManager(dialog.FileName);
                SaveManager.InitSave();
                Load();

                tacMain.IsEnabled = true;
            }
        }

        private void mitOpenCampaign_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.CheckPathExists = true;
            dialog.CheckFileExists = true;
            dialog.Multiselect = false;
            dialog.Title = "Open a VTRPG Campaign Save";
            dialog.DefaultExt = ".vtrpg";
            dialog.Filter = "Virtual TRPG Campaign Save (*.vtrpg)|*.vtrpg|All files (*.*)|*.*";

            bool? selected = dialog.ShowDialog();

            if (selected == true)
            {
                SaveManager = new SaveManager(dialog.FileName);
                SaveManager.Load();
                Load();

                tacMain.IsEnabled = true;
            }
        }

        private void mitAbout_Click(object sender, RoutedEventArgs e)
        {

        }

        private void mitQuit_Click(object sender, RoutedEventArgs e)
        {

        }

        #region Base
        private void Load()
        {

        }
        #endregion

        #region Basic Settings
        private void btnSaveChanges_Basic_Click(object sender, RoutedEventArgs e)
        {

        }
        private void btnLoadChanges_Basic_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnSaveChanges_Campaign_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnLoadChanges_Campaign_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnSaveChanges_System_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnLoadChanges_System_Click(object sender, RoutedEventArgs e)
        {

        }

        #endregion

        #region Auth
        private void btnCreateUser_Click(object sender, RoutedEventArgs e)
        {
            UI.Auth.User user = new UI.Auth.User();
            bool? pass = user.ShowDialog();

            if (pass == true)
            {
                SaveManager.AuthManager.RegisterUser();
            }
        }

        private void btnEditUser_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnDeleteUser_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion
    }
}
