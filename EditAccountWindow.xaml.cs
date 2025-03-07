using Microsoft.Win32;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace RivalsAccountManager
{
    public partial class EditAccountWindow : Window
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct DWM_BLURBEHIND
        {
            public uint dwFlags;
            public bool fEnable;
            public IntPtr hRgnBlur;
            public bool fTransitionOnMaximized;
        }

        [DllImport("dwmapi.dll", PreserveSig = false)]
        private static extern void DwmEnableBlurBehindWindow(IntPtr hwnd, ref DWM_BLURBEHIND pBlurBehind);

        public AccountInfo EditedAccount { get; private set; }
        private readonly bool _isNewAccount;

        public EditAccountWindow(AccountInfo account)
        {
            InitializeComponent();
            _isNewAccount = account == null;
            EditedAccount = _isNewAccount ? new AccountInfo { ID = DateTime.Now.Ticks.ToString() } : account;
            DataContext = EditedAccount;
            Title = _isNewAccount ? "Add Account" : "Edit Account";
            if (!_isNewAccount && !string.IsNullOrEmpty(EditedAccount.Password))
            {
                PasswordBox.Password = EditedAccount.Password;
            }
            Loaded += EditAccountWindow_Loaded; // Hook up Loaded event
        }
        private void EditAccountWindow_Loaded(object sender, RoutedEventArgs e)
        {
            IntPtr hwnd = new WindowInteropHelper(this).Handle;
            var bb = new DWM_BLURBEHIND
            {
                dwFlags = 0x1, // DWM_BB_ENABLE
                fEnable = true,
                hRgnBlur = IntPtr.Zero,
                fTransitionOnMaximized = false
            };
            DwmEnableBlurBehindWindow(hwnd, ref bb);
        }
        private void SelectAvatar_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.png;*.jpg)|*.png;*.jpg",
                Title = "Select Avatar Image"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                string selectedFile = openFileDialog.FileName;
                string avatarsFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Avatars");
                if (!Directory.Exists(avatarsFolder))
                {
                    Directory.CreateDirectory(avatarsFolder);
                }            
                string avatarFileName = $"{EditedAccount.ID}.png";
                string avatarPath = Path.Combine(avatarsFolder, avatarFileName);
                File.Copy(selectedFile, avatarPath, true);
                EditedAccount.AvatarPath = Path.Combine("Avatars", avatarFileName); // Relative path
                
            }
        }
        private void ExitApp_Click(object sender, MouseButtonEventArgs e)
        {
            DialogResult = false;
            Close(); // Exits the entire application
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            EditedAccount.Password = PasswordBox.Password;
            DialogResult = true;
            Close();
            
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }
    }
}