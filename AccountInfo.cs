using System.ComponentModel;

namespace RivalsAccountManager
{
    public class AccountInfo : INotifyPropertyChanged
    {
        private string _id = "";
        public string ID
        {
            get => _id;
            set { _id = value; OnPropertyChanged(nameof(ID)); }
        }

        private string _username = "";
        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(nameof(Username)); }
        }

        private string _email = "";
        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(nameof(Email)); }
        }

        private string _password = "";
        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(nameof(Password)); }
        }

        private string _note = "";
        public string Note
        {
            get => _note;
            set { _note = value; OnPropertyChanged(nameof(Note)); }
        }

        private bool _noteExpanded = false;
        public bool NoteExpanded
        {
            get => _noteExpanded;
            set { _noteExpanded = value; OnPropertyChanged(nameof(NoteExpanded)); }
        }

        private string _avatarPath;
        public string AvatarPath
        {
            get => _avatarPath;
            set
            {
                _avatarPath = value;
                OnPropertyChanged(nameof(AvatarPath));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}