using System.Collections.ObjectModel;
using System.Windows.Input;
using IntegraCamp.Models;
using IntegraCamp.Services;

namespace IntegraCamp.ViewModels
{
    public class AssignViewModel : BindableObject
    {
        private DatabaseService _db;
        private List<Counselor> _allFreeCounselors;

        public ObservableCollection<Counselor> FreeCounselors { get; set; } = new();

        private string _selectedBuilding = "Все";
        public string SelectedBuilding
        {
            get => _selectedBuilding;
            set { _selectedBuilding = value; OnPropertyChanged(); LoadFreeCounselors(); }
        }

        private string _squadNumber;
        public string SquadNumber
        {
            get => _squadNumber;
            set { _squadNumber = value; OnPropertyChanged(); }
        }

        private Counselor _selectedCounselor;
        public Counselor SelectedCounselor
        {
            get => _selectedCounselor;
            set { _selectedCounselor = value; OnPropertyChanged(); }
        }

        public ICommand LoadCommand { get; }
        public ICommand AssignCommand { get; }

        public List<string> Buildings => new List<string> { "Все", "Солнечная", "Морская", "Жемчужная", "Звездная", "Изумрудная" };

        public AssignViewModel()
        {
            _db = new DatabaseService();
            LoadCommand = new Command(async () => await LoadFreeCounselors());
            AssignCommand = new Command(async () => await AssignCounselor(), () => CanAssign());
        }

        private bool CanAssign()
        {
            return SelectedCounselor != null && !string.IsNullOrWhiteSpace(SquadNumber);
        }

        public async Task LoadFreeCounselors()
        {
            await _db.Init();
            _allFreeCounselors = await _db.GetFreeCounselors();

            var filtered = _allFreeCounselors.AsEnumerable();
            if (SelectedBuilding != "Все")
                filtered = filtered.Where(c => c.Building == SelectedBuilding);

            FreeCounselors.Clear();
            foreach (var c in filtered)
                FreeCounselors.Add(c);
        }

        private async Task AssignCounselor()
        {
            if (SelectedCounselor != null && int.TryParse(SquadNumber, out int squad))
            {
                SelectedCounselor.SquadNumber = squad;
                SelectedCounselor.Status = "Закреплен";
                await _db.UpdateCounselor(SelectedCounselor);

                await Application.Current.MainPage.DisplayAlert("Успех",
                    $"{SelectedCounselor.FullName} назначен на отряд {squad}", "OK");

                SquadNumber = string.Empty;
                SelectedCounselor = null;
                await LoadFreeCounselors();

                // Обновляем команду
                ((Command)AssignCommand).ChangeCanExecute();
            }
        }
    }
}