using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RivalsAccountManager
{
    public partial class MainWindow : Window
    {
        private string iniFilePath = "SteamAccounts.ini";
        private string steamExePath = "";
        private Dictionary<string, AccountInfo> accounts = new Dictionary<string, AccountInfo>();

        public MainWindow()
        {
            InitializeComponent();
            ReadINI();
            BuildAccountsUI();
            BackgroundVideo.Play(); // Start the video manually after initialization
        }

        private void ReadINI()
        {
            try
            {
                if (File.Exists(iniFilePath))
                {
                    steamExePath = IniFileHelper.ReadValue(iniFilePath, "Steam", "Path", "");
                    string accountList = IniFileHelper.ReadValue(iniFilePath, "Accounts", "List", "");
                    if (!string.IsNullOrEmpty(accountList))
                    {
                        string[] accountIDs = accountList.Split(',');
                        foreach (string id in accountIDs)
                        {
                            if (string.IsNullOrEmpty(id)) continue;
                            AccountInfo account = new AccountInfo
                            {
                                ID = id,
                                Username = IniFileHelper.ReadValue(iniFilePath, id, "Username", ""),
                                Email = IniFileHelper.ReadValue(iniFilePath, id, "Email", ""),
                                Password = IniFileHelper.ReadValue(iniFilePath, id, "Password", ""),
                                Note = IniFileHelper.ReadValue(iniFilePath, id, "Note", ""),
                                NoteExpanded = IniFileHelper.ReadValue(iniFilePath, id, "NoteExpanded", "0") == "1",
                                AvatarPath = IniFileHelper.ReadValue(iniFilePath, id, "AvatarPath", "")
                            };
                            accounts[id] = account;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading settings: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveINI()
        {
            try
            {
                IniFileHelper.WriteValue(iniFilePath, "Steam", "Path", steamExePath);
                string accountList = string.Join(",", accounts.Keys);
                IniFileHelper.WriteValue(iniFilePath, "Accounts", "List", accountList);
                foreach (var pair in accounts)
                {
                    var account = pair.Value;
                    IniFileHelper.WriteValue(iniFilePath, account.ID, "Username", account.Username);
                    IniFileHelper.WriteValue(iniFilePath, account.ID, "Email", account.Email);
                    IniFileHelper.WriteValue(iniFilePath, account.ID, "Password", account.Password);
                    IniFileHelper.WriteValue(iniFilePath, account.ID, "Note", account.Note);
                    IniFileHelper.WriteValue(iniFilePath, account.ID, "NoteExpanded", account.NoteExpanded ? "1" : "0");
                    IniFileHelper.WriteValue(iniFilePath, account.ID, "AvatarPath", account.AvatarPath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving settings: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BuildAccountsUI()
        {
            AccountsPanel.Children.Clear();
            foreach (var pair in accounts)
            {
                AddAccountToUI(pair.Value);
            }
        }

        private void AddAccountToUI(AccountInfo account)
        {
            AccountControl control = new AccountControl { DataContext = account };
            control.LoginClicked += (s, e) => LoginAccount(account.ID);
            control.EditClicked += (s, e) => EditAccount(account.ID);
            control.DeleteClicked += (s, e) => DeleteAccount(account.ID);
            control.StatsClicked += (s, e) => OpenTrackerProfile(account.Username);
            AccountsPanel.Children.Add(control);
        }

        private void LoginAccount(string accountID)
        {
            if (string.IsNullOrEmpty(steamExePath))
            {
                MessageBox.Show("Steam path not set!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            AccountInfo account = accounts[accountID];
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = steamExePath,
                    Arguments = $"-login {account.Email} {account.Password}",
                    UseShellExecute = false,
                    CreateNoWindow = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error launching Steam: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BackgroundVideo_MediaOpened(object sender, RoutedEventArgs e)
        {
            
         
        }

        private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void BackgroundVideo_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            MessageBox.Show("Media failed: " + e.ErrorException.Message);
        }

        private void BackgroundVideo_MediaEnded(object sender, RoutedEventArgs e)
        {
            BackgroundVideo.Position = TimeSpan.Zero; // Rewind to start
            BackgroundVideo.Play(); // Restart playback
        }

        private void ExitApp_Click(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown(); // Exits the entire application
        }

        private void EditAccount(string accountID)
        {
            AccountInfo account = accounts[accountID];
            EditAccountWindow editWindow = new EditAccountWindow(account);
            if (editWindow.ShowDialog() == true)
            {
                SaveINI();
            }
        }

        private void DeleteAccount(string accountID)
        {
            if (MessageBox.Show("Are you sure you want to delete this account?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                accounts.Remove(accountID);
                SaveINI();
                BuildAccountsUI();
            }
        }
        private void Grid_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            // Route mouse wheel events to the ScrollViewer
            double scrollAmount = (e.Delta/120) * -1; // Adjust sensitivity (negative for natural direction)
            AccountsScrollViewer.ScrollToHorizontalOffset(AccountsScrollViewer.HorizontalOffset + (scrollAmount));
            e.Handled = true; // Prevent event from bubbling up
                
            
            
        }
        
        private void OpenTrackerProfile(string username)
        {
            string trackerUrl = $"https://tracker.gg/marvel-rivals/profile/ign/{username}/overview?mode=competitive";
            try
            {
                Process.Start(new ProcessStartInfo { FileName = trackerUrl, UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening browser: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddAccount_Click(object sender, MouseButtonEventArgs e)
        {
            EditAccountWindow addWindow = new EditAccountWindow(null);
            if (addWindow.ShowDialog() == true)
            {
                AccountInfo newAccount = addWindow.EditedAccount;
                accounts[newAccount.ID] = newAccount;
                SaveINI();
                AddAccountToUI(newAccount);
            }
        }

        private void SetSteamPath_Click(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Executable files (*.exe)|*.exe",
                Title = "Select Steam Executable"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                steamExePath = openFileDialog.FileName;
                SaveINI();
            }
        }
    }
}