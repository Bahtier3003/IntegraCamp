using System.Collections.ObjectModel;
using System.Windows.Input;
using IntegraCamp.Models;
using IntegraCamp.Services;

namespace IntegraCamp.ViewModels
{
    public class CounselorsViewModel : BindableObject
    {
        private DatabaseService _db;
        private List<Counselor> _allCounselors;

        public ObservableCollection<Counselor> Counselors { get; set; } = new();

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set { _searchText = value; OnPropertyChanged(); FilterCounselors(); }
        }

        private string _selectedBuilding = "Все";
        public string SelectedBuilding
        {
            get => _selectedBuilding;
            set { _selectedBuilding = value; OnPropertyChanged(); FilterCounselors(); }
        }

        private string _selectedStatus = "Все";
        public string SelectedStatus
        {
            get => _selectedStatus;
            set { _selectedStatus = value; OnPropertyChanged(); FilterCounselors(); }
        }

        private int _totalCount;
        public int TotalCount { get => _totalCount; set { _totalCount = value; OnPropertyChanged(); } }

        private int _freeCount;
        public int FreeCount { get => _freeCount; set { _freeCount = value; OnPropertyChanged(); } }

        private int _assignedCount;
        public int AssignedCount { get => _assignedCount; set { _assignedCount = value; OnPropertyChanged(); } }

        public ICommand LoadCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand AssignCommand { get; }
        public ICommand ReleaseCommand { get; }
        public ICommand AddCommand { get; }

        public List<string> Buildings => new List<string> { "Все", "Солнечная", "Морская", "Жемчужная", "Звездная", "Изумрудная" };
        public List<string> Statuses => new List<string> { "Все", "Свободен", "Закреплен" };

        public CounselorsViewModel()
        {
            _db = new DatabaseService();
            LoadCommand = new Command(async () => await LoadCounselors());
            DeleteCommand = new Command<Counselor>(async (c) => await DeleteCounselor(c));
            EditCommand = new Command<Counselor>(async (c) => await EditCounselor(c));
            AssignCommand = new Command<Counselor>(async (c) => await AssignCounselor(c));
            ReleaseCommand = new Command<Counselor>(async (c) => await ReleaseCounselor(c));
            AddCommand = new Command(async () => await AddCounselor());
        }

        public async Task LoadCounselors()
        {
            await _db.Init();
            _allCounselors = await _db.GetAllCounselors();
            await UpdateStats();
            FilterCounselors();
        }

        private async Task UpdateStats()
        {
            TotalCount = _allCounselors.Count;
            FreeCount = await _db.GetFreeCount();
            AssignedCount = await _db.GetAssignedCount();
        }

        private void FilterCounselors()
        {
            var filtered = _allCounselors.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchText))
                filtered = filtered.Where(c => c.FullName.Contains(SearchText, StringComparison.OrdinalIgnoreCase));

            if (SelectedBuilding != "Все")
                filtered = filtered.Where(c => c.Building == SelectedBuilding);

            if (SelectedStatus != "Все")
                filtered = filtered.Where(c => c.Status == SelectedStatus);

            Counselors.Clear();
            foreach (var c in filtered)
                Counselors.Add(c);
        }

        private async Task DeleteCounselor(Counselor counselor)
        {
            bool confirm = await Application.Current.MainPage.DisplayAlert("Удаление",
                $"Удалить {counselor.FullName}?", "Да", "Нет");

            if (confirm)
            {
                if (counselor.Status == "Закреплен")
                {
                    bool extraConfirm = await Application.Current.MainPage.DisplayAlert("Внимание!",
                        "Вожатый закреплен за отрядом! Удалить?", "Да", "Нет");
                    if (!extraConfirm) return;
                }

                await _db.DeleteCounselor(counselor);
                await LoadCounselors();
            }
        }

        private async Task EditCounselor(Counselor counselor)
        {
            var navigation = Application.Current.MainPage.Navigation;
            var editPage = new Views.CounselorFormPage(counselor);
            await navigation.PushAsync(editPage);
        }

        private async Task AddCounselor()
        {
            var navigation = Application.Current.MainPage.Navigation;
            var addPage = new Views.CounselorFormPage();
            await navigation.PushAsync(addPage);
        }

        private async Task AssignCounselor(Counselor counselor)
        {
            string squadNumber = await Application.Current.MainPage.DisplayPromptAsync("Назначение",
                "Введите номер отряда:", keyboard: Keyboard.Numeric);

            if (!string.IsNullOrEmpty(squadNumber) && int.TryParse(squadNumber, out int squad))
            {
                counselor.SquadNumber = squad;
                counselor.Status = "Закреплен";
                await _db.UpdateCounselor(counselor);
                await LoadCounselors();
                await Application.Current.MainPage.DisplayAlert("Успех", "Вожатый назначен", "OK");
            }
        }

        private async Task ReleaseCounselor(Counselor counselor)
        {
            bool confirm = await Application.Current.MainPage.DisplayAlert("Освобождение",
                $"Освободить {counselor.FullName} от отряда?", "Да", "Нет");

            if (confirm)
            {
                counselor.SquadNumber = null;
                counselor.Status = "Свободен";
                await _db.UpdateCounselor(counselor);
                await LoadCounselors();
            }
        }
    }
}