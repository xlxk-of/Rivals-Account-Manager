using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace RivalsAccountManager
{
    public partial class AccountControl : UserControl
    {
        public AccountControl()
        {
            InitializeComponent();
        }

        private void EmailTextBlock_MouseEnter(object sender, MouseEventArgs e)
        {
            if (DataContext is AccountInfo account)
            {
                EmailTextBlock.Text = account.Email;
            }
        }

        private void EmailTextBlock_MouseLeave(object sender, MouseEventArgs e)
        {
            EmailTextBlock.Text = "•••••••••••";
        }
        private void EmailTextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && sender is TextBlock emailTextBlock)
            {
                // Use a new mutable SolidColorBrush instead of Brushes.Gray
                emailTextBlock.Background = new SolidColorBrush(Colors.Gray);
                emailTextBlock.Foreground = Brushes.White; // Brushes.White is fine as-is since we’re not animating it

                // Optional: Copy to clipboard
                Clipboard.SetText(emailTextBlock.Text);
            }
            e.Handled = true;
        }

        private void EmailTextBlock_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && sender is TextBlock emailTextBlock)
            {
                // Animate Background from Gray to Transparent
                var fadeOut = new ColorAnimation
                {
                    From = Colors.Gray,
                    To = Color.FromArgb(0, 0, 0, 0), // Fully transparent
                    Duration = TimeSpan.FromSeconds(0.3)
                };
                emailTextBlock.Background.BeginAnimation(SolidColorBrush.ColorProperty, fadeOut);

                // Reset Foreground immediately (no animation needed here)
                emailTextBlock.Foreground = Brushes.Gray;
            }
            e.Handled = true;
        }


        private void UserTextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && sender is TextBlock UserTextBlock)
            {
                // Use a new mutable SolidColorBrush instead of Brushes.Gray
                UserTextBlock.Background = new SolidColorBrush(Colors.White);
                UserTextBlock.Foreground = Brushes.Gray; // Brushes.White is fine as-is since we’re not animating it

                // Optional: Copy to clipboard
                Clipboard.SetText(UserTextBlock.Text);
            }
            e.Handled = true;
        }

        private void UserTextBlock_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && sender is TextBlock UserTextBlock)
            {
                // Animate Background from Gray to Transparent
                var fadeOut = new ColorAnimation
                {
                    From = Colors.Gray,
                    To = Color.FromArgb(0, 0, 0, 0), // Fully transparent
                    Duration = TimeSpan.FromSeconds(0.3)
                };
                UserTextBlock.Background.BeginAnimation(SolidColorBrush.ColorProperty, fadeOut);

                // Reset Foreground immediately (no animation needed here)
                UserTextBlock.Foreground = Brushes.White;
            }
            e.Handled = true;
        }

        public event EventHandler LoginClicked;
        private void Login_Click(object sender, MouseButtonEventArgs e)
        {
            LoginClicked?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler EditClicked;
        private void Edit_Click(object sender, MouseButtonEventArgs e)
        {
            EditClicked?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler DeleteClicked;
        private void Delete_Click(object sender, MouseButtonEventArgs e)
        {
            DeleteClicked?.Invoke(this, EventArgs.Empty);
        }

        private void ToggleNote_Click(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is AccountInfo account)
            {
                account.NoteExpanded = !account.NoteExpanded;
            }
        }

        public event EventHandler StatsClicked;
        private void Stats_Click(object sender, MouseButtonEventArgs e)
        {
            StatsClicked?.Invoke(this, EventArgs.Empty);
        }
    }
}