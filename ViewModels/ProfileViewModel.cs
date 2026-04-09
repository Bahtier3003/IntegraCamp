using System.Windows.Input;
using IntegraCamp.Models;
using IntegraCamp.Services;

namespace IntegraCamp.ViewModels
{
    public class ProfileViewModel : BindableObject
    {
        private DatabaseService _db;
        private UserProfile _profile;

        public UserProfile Profile
        {
            get => _profile;
            set { _profile = value; OnPropertyChanged(); }
        }

        private bool _isEditing;
        public bool IsEditing
        {
            get => _isEditing;
            set { _isEditing = value; OnPropertyChanged(); OnPropertyChanged(nameof(IsViewing)); }
        }

        public bool IsViewing => !IsEditing;

        public ICommand LoadCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public ProfileViewModel()
        {
            _db = new DatabaseService();
            LoadCommand = new Command(async () => await LoadProfile());
            EditCommand = new Command(() => IsEditing = true);
            SaveCommand = new Command(async () => await SaveProfile());
            CancelCommand = new Command(() => { IsEditing = false; LoadProfile(); });
        }

        private async Task LoadProfile()
        {
            await _db.Init();
            Profile = await _db.GetUserProfile();
        }

        private async Task SaveProfile()
        {
            if (string.IsNullOrWhiteSpace(Profile.FullName) ||
                string.IsNullOrWhiteSpace(Profile.Position) ||
                string.IsNullOrWhiteSpace(Profile.Email))
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", "Заполните все поля", "OK");
                return;
            }

            await _db.UpdateUserProfile(Profile);
            IsEditing = false;
            await Application.Current.MainPage.DisplayAlert("Успех", "Профиль сохранен", "OK");
        }
    }
}